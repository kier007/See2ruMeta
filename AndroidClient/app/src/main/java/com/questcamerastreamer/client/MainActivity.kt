package com.questcamerastreamer.client

import android.os.Bundle
import android.view.View
import android.view.WindowManager
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import com.questcamerastreamer.client.databinding.ActivityMainBinding
import com.questcamerastreamer.client.webrtc.WebRTCClient
import kotlinx.coroutines.launch
import org.webrtc.SurfaceViewRenderer

class MainActivity : AppCompatActivity() {
    
    private lateinit var binding: ActivityMainBinding
    private lateinit var webRTCClient: WebRTCClient
    private var isConnected = false
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        
        // Keep screen on while streaming
        window.addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON)
        
        binding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(binding.root)
        
        setupUI()
        initializeWebRTC()
    }
    
    private fun setupUI() {
        // Set default IP (can be changed by user)
        binding.ipAddressInput.setText("192.168.1.100")
        
        binding.connectButton.setOnClickListener {
            if (!isConnected) {
                connectToQuest()
            } else {
                disconnect()
            }
        }
        
        binding.fullscreenButton.setOnClickListener {
            toggleFullscreen()
        }
        
        // Initially hide video view and controls
        binding.videoContainer.visibility = View.GONE
        binding.videoControls.visibility = View.GONE
    }
    
    private fun initializeWebRTC() {
        webRTCClient = WebRTCClient(
            context = this,
            surfaceViewRenderer = binding.remoteVideoView
        )
        
        webRTCClient.onConnectionStateChange = { state ->
            runOnUiThread {
                handleConnectionStateChange(state)
            }
        }
        
        webRTCClient.onError = { error ->
            runOnUiThread {
                showError(error)
            }
        }
    }
    
    private fun connectToQuest() {
        val ipAddress = binding.ipAddressInput.text.toString().trim()
        
        if (ipAddress.isEmpty()) {
            showError("Please enter a valid IP address")
            return
        }
        
        // Validate IP address format
        if (!isValidIPAddress(ipAddress)) {
            showError("Invalid IP address format")
            return
        }
        
        binding.progressBar.visibility = View.VISIBLE
        binding.statusText.text = "Connecting to $ipAddress..."
        binding.connectButton.isEnabled = false
        
        lifecycleScope.launch {
            try {
                webRTCClient.connect(ipAddress, 8080)
            } catch (e: Exception) {
                runOnUiThread {
                    showError("Connection failed: ${e.message}")
                    resetConnectionUI()
                }
            }
        }
    }
    
    private fun disconnect() {
        webRTCClient.disconnect()
        resetConnectionUI()
        
        binding.videoContainer.visibility = View.GONE
        binding.videoControls.visibility = View.GONE
        binding.connectionLayout.visibility = View.VISIBLE
    }
    
    private fun handleConnectionStateChange(state: WebRTCClient.ConnectionState) {
        when (state) {
            WebRTCClient.ConnectionState.CONNECTING -> {
                binding.statusText.text = "Establishing connection..."
            }
            WebRTCClient.ConnectionState.CONNECTED -> {
                isConnected = true
                binding.progressBar.visibility = View.GONE
                binding.connectButton.text = "Disconnect"
                binding.connectButton.isEnabled = true
                binding.statusText.text = "Connected"
                
                // Show video view
                binding.connectionLayout.visibility = View.GONE
                binding.videoContainer.visibility = View.VISIBLE
                binding.videoControls.visibility = View.VISIBLE
                
                showMessage("Connected to Quest camera stream")
            }
            WebRTCClient.ConnectionState.DISCONNECTED -> {
                isConnected = false
                resetConnectionUI()
                showMessage("Disconnected from stream")
            }
            WebRTCClient.ConnectionState.FAILED -> {
                isConnected = false
                resetConnectionUI()
                showError("Connection failed")
            }
        }
    }
    
    private fun resetConnectionUI() {
        binding.progressBar.visibility = View.GONE
        binding.connectButton.text = "Connect"
        binding.connectButton.isEnabled = true
        binding.statusText.text = "Not connected"
        isConnected = false
    }
    
    private fun toggleFullscreen() {
        if (binding.videoControls.visibility == View.VISIBLE) {
            // Enter fullscreen
            binding.videoControls.visibility = View.GONE
            supportActionBar?.hide()
            
            window.decorView.systemUiVisibility = (
                View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY
                or View.SYSTEM_UI_FLAG_FULLSCREEN
                or View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
                or View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                or View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                or View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
            )
        } else {
            // Exit fullscreen
            binding.videoControls.visibility = View.VISIBLE
            supportActionBar?.show()
            
            window.decorView.systemUiVisibility = View.SYSTEM_UI_FLAG_VISIBLE
        }
    }
    
    private fun isValidIPAddress(ip: String): Boolean {
        val parts = ip.split(".")
        if (parts.size != 4) return false
        
        return parts.all { part ->
            try {
                val num = part.toInt()
                num in 0..255
            } catch (e: NumberFormatException) {
                false
            }
        }
    }
    
    private fun showMessage(message: String) {
        Toast.makeText(this, message, Toast.LENGTH_SHORT).show()
    }
    
    private fun showError(error: String) {
        Toast.makeText(this, error, Toast.LENGTH_LONG).show()
        binding.statusText.text = "Error: $error"
    }
    
    override fun onDestroy() {
        super.onDestroy()
        webRTCClient.release()
    }
    
    override fun onPause() {
        super.onPause()
        if (isConnected) {
            webRTCClient.pauseVideo()
        }
    }
    
    override fun onResume() {
        super.onResume()
        if (isConnected) {
            webRTCClient.resumeVideo()
        }
    }
}
