package com.questcamerastreamer.client.webrtc

import android.content.Context
import android.util.Log
import kotlinx.coroutines.*
import okhttp3.*
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.RequestBody.Companion.toRequestBody
import org.webrtc.*
import java.io.IOException
import java.util.concurrent.TimeUnit

class WebRTCClient(
    private val context: Context,
    private val surfaceViewRenderer: SurfaceViewRenderer
) {
    
    companion object {
        private const val TAG = "WebRTCClient"
        private const val STUN_SERVER = "stun:stun.l.google.com:19302"
    }
    
    enum class ConnectionState {
        CONNECTING,
        CONNECTED,
        DISCONNECTED,
        FAILED
    }
    
    private var peerConnectionFactory: PeerConnectionFactory? = null
    private var peerConnection: PeerConnection? = null
    private var eglBase: EglBase? = null
    private var videoTrack: VideoTrack? = null
    private var dataChannel: DataChannel? = null
    
    private val okHttpClient = OkHttpClient.Builder()
        .connectTimeout(10, TimeUnit.SECONDS)
        .readTimeout(10, TimeUnit.SECONDS)
        .build()
    
    private var signalingWebSocket: WebSocket? = null
    
    var onConnectionStateChange: ((ConnectionState) -> Unit)? = null
    var onError: ((String) -> Unit)? = null
    
    init {
        initializePeerConnectionFactory()
        setupSurfaceViewRenderer()
    }
    
    private fun initializePeerConnectionFactory() {
        val options = PeerConnectionFactory.InitializationOptions.builder(context)
            .setEnableInternalTracer(true)
            .createInitializationOptions()
        PeerConnectionFactory.initialize(options)
        
        eglBase = EglBase.create()
        
        val encoderFactory = DefaultVideoEncoderFactory(
            eglBase?.eglBaseContext,
            true,
            true
        )
        
        val decoderFactory = DefaultVideoDecoderFactory(eglBase?.eglBaseContext)
        
        peerConnectionFactory = PeerConnectionFactory.builder()
            .setVideoEncoderFactory(encoderFactory)
            .setVideoDecoderFactory(decoderFactory)
            .createPeerConnectionFactory()
    }
    
    private fun setupSurfaceViewRenderer() {
        surfaceViewRenderer.init(eglBase?.eglBaseContext, null)
        surfaceViewRenderer.setEnableHardwareScaler(true)
        surfaceViewRenderer.setMirror(false)
        surfaceViewRenderer.setScalingType(RendererCommon.ScalingType.SCALE_ASPECT_FIT)
    }
    
    suspend fun connect(ipAddress: String, port: Int) = withContext(Dispatchers.IO) {
        try {
            onConnectionStateChange?.invoke(ConnectionState.CONNECTING)
            
            // Create peer connection
            createPeerConnection()
            
            // Connect to signaling server
            connectToSignalingServer(ipAddress, port)
            
            // Send offer to start negotiation
            createAndSendOffer()
            
        } catch (e: Exception) {
            Log.e(TAG, "Connection failed", e)
            onError?.invoke(e.message ?: "Unknown error")
            onConnectionStateChange?.invoke(ConnectionState.FAILED)
        }
    }
    
    private fun createPeerConnection() {
        val iceServers = listOf(
            PeerConnection.IceServer.builder(STUN_SERVER).createIceServer()
        )
        
        val rtcConfig = PeerConnection.RTCConfiguration(iceServers).apply {
            tcpCandidatePolicy = PeerConnection.TcpCandidatePolicy.ENABLED
            bundlePolicy = PeerConnection.BundlePolicy.MAXBUNDLE
            rtcpMuxPolicy = PeerConnection.RtcpMuxPolicy.REQUIRE
            continualGatheringPolicy = PeerConnection.ContinualGatheringPolicy.GATHER_CONTINUALLY
            keyType = PeerConnection.KeyType.ECDSA
            enableDtlsSrtp = true
        }
        
        val observer = object : PeerConnection.Observer {
            override fun onIceCandidate(candidate: IceCandidate) {
                Log.d(TAG, "onIceCandidate: ${candidate.sdp}")
                sendIceCandidate(candidate)
            }
            
            override fun onIceCandidatesRemoved(candidates: Array<out IceCandidate>?) {
                Log.d(TAG, "onIceCandidatesRemoved")
            }
            
            override fun onSignalingChange(state: PeerConnection.SignalingState?) {
                Log.d(TAG, "onSignalingChange: $state")
            }
            
            override fun onIceConnectionChange(state: PeerConnection.IceConnectionState?) {
                Log.d(TAG, "onIceConnectionChange: $state")
                when (state) {
                    PeerConnection.IceConnectionState.CONNECTED -> {
                        onConnectionStateChange?.invoke(ConnectionState.CONNECTED)
                    }
                    PeerConnection.IceConnectionState.DISCONNECTED -> {
                        onConnectionStateChange?.invoke(ConnectionState.DISCONNECTED)
                    }
                    PeerConnection.IceConnectionState.FAILED -> {
                        onConnectionStateChange?.invoke(ConnectionState.FAILED)
                    }
                    else -> {}
                }
            }
            
            override fun onIceConnectionReceivingChange(receiving: Boolean) {
                Log.d(TAG, "onIceConnectionReceivingChange: $receiving")
            }
            
            override fun onIceGatheringChange(state: PeerConnection.IceGatheringState?) {
                Log.d(TAG, "onIceGatheringChange: $state")
            }
            
            override fun onAddStream(stream: MediaStream) {
                Log.d(TAG, "onAddStream")
                stream.videoTracks?.firstOrNull()?.let { track ->
                    videoTrack = track
                    track.addSink(surfaceViewRenderer)
                }
            }
            
            override fun onRemoveStream(stream: MediaStream?) {
                Log.d(TAG, "onRemoveStream")
            }
            
            override fun onDataChannel(channel: DataChannel) {
                Log.d(TAG, "onDataChannel: ${channel.label()}")
                dataChannel = channel
                setupDataChannel()
            }
            
            override fun onRenegotiationNeeded() {
                Log.d(TAG, "onRenegotiationNeeded")
            }
            
            override fun onAddTrack(receiver: RtpReceiver?, streams: Array<out MediaStream>?) {
                Log.d(TAG, "onAddTrack")
                receiver?.track()?.let { track ->
                    if (track is VideoTrack) {
                        videoTrack = track
                        track.addSink(surfaceViewRenderer)
                    }
                }
            }
        }
        
        peerConnection = peerConnectionFactory?.createPeerConnection(rtcConfig, observer)
        
        // Create data channel for control messages
        val dcInit = DataChannel.Init().apply {
            ordered = true
            negotiated = false
            maxRetransmits = -1
            maxRetransmitTimeMs = -1
            id = -1
        }
        dataChannel = peerConnection?.createDataChannel("control", dcInit)
        setupDataChannel()
    }
    
    private fun setupDataChannel() {
        dataChannel?.registerObserver(object : DataChannel.Observer {
            override fun onBufferedAmountChange(amount: Long) {}
            
            override fun onStateChange() {
                Log.d(TAG, "Data channel state: ${dataChannel?.state()}")
                if (dataChannel?.state() == DataChannel.State.OPEN) {
                    // Request stream info
                    sendDataChannelMessage("REQUEST_STREAM_INFO")
                }
            }
            
            override fun onMessage(buffer: DataChannel.Buffer) {
                val data = ByteArray(buffer.data.remaining())
                buffer.data.get(data)
                val message = String(data)
                Log.d(TAG, "Received data channel message: $message")
            }
        })
    }
    
    private fun sendDataChannelMessage(message: String) {
        val buffer = DataChannel.Buffer(
            java.nio.ByteBuffer.wrap(message.toByteArray()),
            false
        )
        dataChannel?.send(buffer)
    }
    
    private fun connectToSignalingServer(ipAddress: String, port: Int) {
        val request = Request.Builder()
            .url("ws://$ipAddress:$port/signaling")
            .build()
        
        val listener = object : WebSocketListener() {
            override fun onOpen(webSocket: WebSocket, response: Response) {
                Log.d(TAG, "WebSocket connected")
            }
            
            override fun onMessage(webSocket: WebSocket, text: String) {
                handleSignalingMessage(text)
            }
            
            override fun onFailure(webSocket: WebSocket, t: Throwable, response: Response?) {
                Log.e(TAG, "WebSocket failure", t)
                onError?.invoke("WebSocket connection failed")
                onConnectionStateChange?.invoke(ConnectionState.FAILED)
            }
            
            override fun onClosed(webSocket: WebSocket, code: Int, reason: String) {
                Log.d(TAG, "WebSocket closed: $reason")
            }
        }
        
        signalingWebSocket = okHttpClient.newWebSocket(request, listener)
    }
    
    private fun handleSignalingMessage(message: String) {
        // Parse signaling messages (SDP offers/answers, ICE candidates)
        try {
            // This is a simplified implementation
            // In a real app, you'd parse JSON messages and handle different types
            Log.d(TAG, "Signaling message: $message")
        } catch (e: Exception) {
            Log.e(TAG, "Failed to handle signaling message", e)
        }
    }
    
    private fun createAndSendOffer() {
        val constraints = MediaConstraints().apply {
            mandatory.add(MediaConstraints.KeyValuePair("OfferToReceiveVideo", "true"))
            mandatory.add(MediaConstraints.KeyValuePair("OfferToReceiveAudio", "false"))
        }
        
        peerConnection?.createOffer(object : SdpObserver {
            override fun onCreateSuccess(sdp: SessionDescription) {
                peerConnection?.setLocalDescription(object : SdpObserver {
                    override fun onCreateSuccess(p0: SessionDescription?) {}
                    override fun onSetSuccess() {
                        // Send offer to signaling server
                        sendSdpToSignalingServer(sdp)
                    }
                    override fun onCreateFailure(p0: String?) {}
                    override fun onSetFailure(p0: String?) {}
                }, sdp)
            }
            
            override fun onSetSuccess() {}
            override fun onCreateFailure(error: String?) {
                Log.e(TAG, "Failed to create offer: $error")
                onError?.invoke("Failed to create offer")
            }
            override fun onSetFailure(error: String?) {}
        }, constraints)
    }
    
    private fun sendSdpToSignalingServer(sdp: SessionDescription) {
        val message = """
            {
                "type": "${sdp.type}",
                "sdp": "${sdp.description}"
            }
        """.trimIndent()
        
        signalingWebSocket?.send(message)
    }
    
    private fun sendIceCandidate(candidate: IceCandidate) {
        val message = """
            {
                "type": "ice",
                "candidate": "${candidate.sdp}",
                "sdpMLineIndex": ${candidate.sdpMLineIndex},
                "sdpMid": "${candidate.sdpMid}"
            }
        """.trimIndent()
        
        signalingWebSocket?.send(message)
    }
    
    fun disconnect() {
        videoTrack?.removeSink(surfaceViewRenderer)
        videoTrack = null
        
        dataChannel?.close()
        dataChannel = null
        
        peerConnection?.close()
        peerConnection = null
        
        signalingWebSocket?.close(1000, "User disconnected")
        signalingWebSocket = null
        
        onConnectionStateChange?.invoke(ConnectionState.DISCONNECTED)
    }
    
    fun pauseVideo() {
        videoTrack?.setEnabled(false)
    }
    
    fun resumeVideo() {
        videoTrack?.setEnabled(true)
    }
    
    fun release() {
        disconnect()
        
        surfaceViewRenderer.release()
        eglBase?.release()
        eglBase = null
        
        peerConnectionFactory?.dispose()
        peerConnectionFactory = null
    }
}
