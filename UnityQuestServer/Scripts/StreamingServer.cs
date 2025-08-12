using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.WebRTC;
using UnityEngine.Networking;

namespace QuestCameraStreamer
{
    /// <summary>
    /// Manages WebRTC streaming server for broadcasting camera feed
    /// </summary>
    public class StreamingServer : MonoBehaviour
    {
        [Header("Streaming Configuration")]
        [SerializeField] private int streamingPort = 8080;
        [SerializeField] private int signalingPort = 8081;
        [SerializeField] private int videoBitrate = 5000000; // 5 Mbps
        [SerializeField] private int videoFramerate = 30;
        
        [Header("References")]
        [SerializeField] private PassthroughManager passthroughManager;
        
        private RTCPeerConnection peerConnection;
        private MediaStream localStream;
        private VideoStreamTrack videoTrack;
        private List<RTCDataChannel> dataChannels = new List<RTCDataChannel>();
        
        private bool isStreaming = false;
        private string localIPAddress;
        
        public delegate void StreamingStatusChanged(bool isStreaming, string ipAddress);
        public event StreamingStatusChanged OnStreamingStatusChanged;
        
        public bool IsStreaming => isStreaming;
        public string LocalIPAddress => localIPAddress;
        
        private void Awake()
        {
            // Initialize WebRTC
            WebRTC.Initialize(WebRTCSettings.LimitTextureSize);
            
            if (passthroughManager == null)
            {
                passthroughManager = GetComponent<PassthroughManager>();
            }
        }
        
        private void Start()
        {
            localIPAddress = GetLocalIPAddress();
            StartCoroutine(InitializeWebRTC());
        }
        
        private IEnumerator InitializeWebRTC()
        {
            yield return new WaitForSeconds(1f);
            
            // Wait for passthrough to be ready
            while (passthroughManager.CameraTexture == null)
            {
                yield return null;
            }
            
            SetupVideoStream();
        }
        
        private void SetupVideoStream()
        {
            // Create video stream from render texture
            var texture = passthroughManager.CameraTexture;
            if (texture == null)
            {
                Debug.LogError("Camera texture is null!");
                return;
            }
            
            // Create video track from render texture
            videoTrack = new VideoStreamTrack(texture);
            
            // Create media stream
            localStream = new MediaStream();
            localStream.AddTrack(videoTrack);
            
            Debug.Log("Video stream setup complete");
        }
        
        public void StartStreaming()
        {
            if (isStreaming) return;
            
            StartCoroutine(StartStreamingCoroutine());
        }
        
        private IEnumerator StartStreamingCoroutine()
        {
            isStreaming = true;
            
            // Create peer connection configuration
            var config = new RTCConfiguration
            {
                iceServers = new[]
                {
                    new RTCIceServer
                    {
                        urls = new[] { "stun:stun.l.google.com:19302" }
                    }
                }
            };
            
            peerConnection = new RTCPeerConnection(ref config);
            
            // Add tracks to peer connection
            foreach (var track in localStream.GetTracks())
            {
                peerConnection.AddTrack(track, localStream);
            }
            
            // Set up event handlers
            peerConnection.OnIceCandidate = OnIceCandidate;
            peerConnection.OnIceConnectionChange = OnIceConnectionChange;
            peerConnection.OnDataChannel = OnDataChannel;
            
            // Create offer
            var offerOptions = new RTCOfferAnswerOptions
            {
                offerToReceiveVideo = false,
                offerToReceiveAudio = false
            };
            
            var offer = peerConnection.CreateOffer(ref offerOptions);
            yield return offer;
            
            if (offer.IsError)
            {
                Debug.LogError($"Failed to create offer: {offer.Error.message}");
                StopStreaming();
                yield break;
            }
            
            var desc = offer.Desc;
            var setLocalDesc = peerConnection.SetLocalDescription(ref desc);
            yield return setLocalDesc;
            
            if (setLocalDesc.IsError)
            {
                Debug.LogError($"Failed to set local description: {setLocalDesc.Error.message}");
                StopStreaming();
                yield break;
            }
            
            OnStreamingStatusChanged?.Invoke(true, localIPAddress);
            Debug.Log($"Streaming started on {localIPAddress}:{streamingPort}");
        }
        
        public void StopStreaming()
        {
            if (!isStreaming) return;
            
            isStreaming = false;
            
            // Close data channels
            foreach (var channel in dataChannels)
            {
                channel.Close();
            }
            dataChannels.Clear();
            
            // Close peer connection
            if (peerConnection != null)
            {
                peerConnection.Close();
                peerConnection.Dispose();
                peerConnection = null;
            }
            
            OnStreamingStatusChanged?.Invoke(false, localIPAddress);
            Debug.Log("Streaming stopped");
        }
        
        private void OnIceCandidate(RTCIceCandidate candidate)
        {
            Debug.Log($"ICE candidate: {candidate.Candidate}");
            // In a real implementation, send this to the signaling server
        }
        
        private void OnIceConnectionChange(RTCIceConnectionState state)
        {
            Debug.Log($"ICE connection state: {state}");
            
            switch (state)
            {
                case RTCIceConnectionState.Connected:
                    Debug.Log("Client connected!");
                    break;
                case RTCIceConnectionState.Disconnected:
                case RTCIceConnectionState.Failed:
                    Debug.Log("Client disconnected");
                    break;
            }
        }
        
        private void OnDataChannel(RTCDataChannel channel)
        {
            Debug.Log($"Data channel received: {channel.Label}");
            dataChannels.Add(channel);
            
            channel.OnOpen = () => Debug.Log($"Data channel {channel.Label} opened");
            channel.OnClose = () => Debug.Log($"Data channel {channel.Label} closed");
            channel.OnMessage = (bytes) => HandleDataChannelMessage(channel, bytes);
        }
        
        private void HandleDataChannelMessage(RTCDataChannel channel, byte[] data)
        {
            string message = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log($"Received message: {message}");
            
            // Handle control messages from client
            if (message == "REQUEST_STREAM_INFO")
            {
                var info = new
                {
                    width = passthroughManager.CameraTexture.width,
                    height = passthroughManager.CameraTexture.height,
                    framerate = videoFramerate,
                    bitrate = videoBitrate
                };
                
                string jsonInfo = JsonUtility.ToJson(info);
                channel.Send(System.Text.Encoding.UTF8.GetBytes(jsonInfo));
            }
        }
        
        private string GetLocalIPAddress()
        {
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to get local IP: {e.Message}");
            }
            
            return "127.0.0.1";
        }
        
        private void OnDestroy()
        {
            StopStreaming();
            
            if (videoTrack != null)
            {
                videoTrack.Dispose();
            }
            
            if (localStream != null)
            {
                localStream.Dispose();
            }
            
            WebRTC.Dispose();
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                StopStreaming();
            }
        }
    }
}
