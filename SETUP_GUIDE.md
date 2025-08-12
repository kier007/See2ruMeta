# Meta Quest Camera Streaming System - Complete Setup Guide

## Overview

This system allows you to stream the Meta Quest's passthrough camera feed to an Android device over your local network, similar to DroidCam but for VR headsets.

## System Architecture

1. **Unity Quest Server** - Runs on Meta Quest, captures passthrough camera and streams via WebRTC
2. **Android Client** - Native Android app that receives and displays the video stream

## Prerequisites

### For Unity Quest Server
- Unity 2022.3 LTS or later
- Unity Android Build Support
- Meta Quest 2/3/Pro with Developer Mode enabled
- USB-C cable for deployment

### For Android Client
- Android Studio Arctic Fox or later
- Android device running Android 7.0+ (API 24+)
- Same Wi-Fi network as Quest

## Part 1: Setting Up Unity Quest Server

### Step 1: Create Unity Project

1. Open Unity Hub and create new 3D project
2. Name it "QuestCameraStreamer"
3. Wait for project to load

### Step 2: Import Required Packages

1. Open Window > Package Manager
2. Click "+" and select "Add package from git URL"
3. Add these packages one by one:
   - `com.unity.webrtc` (version 3.0.0-pre.7)
   - Install Meta XR SDK from: https://developer.oculus.com/downloads/package/unity-integration/

### Step 3: Configure Project Settings

1. **Build Settings (File > Build Settings)**
   - Switch Platform to Android
   - Set Texture Compression: ASTC
   - Set Target Architecture: ARM64

2. **Player Settings (Edit > Project Settings > Player)**
   - Company Name: Your Company
   - Product Name: Quest Camera Streamer
   - Package Name: com.questcamerastreamer.server
   - Minimum API Level: Android 10.0 (API 29)
   - Target API Level: Automatic (highest installed)
   - Configuration: IL2CPP
   - Target Architectures: ARM64 only

3. **XR Plug-in Management (Edit > Project Settings > XR Plug-in Management)**
   - Check "Oculus" under Android settings
   - Under Oculus settings:
     - Enable "Quest 2" and/or "Quest 3/Pro"
     - Rendering Mode: Multi-view
     - Enable Passthrough

### Step 4: Import Scripts

1. Create folder: Assets/Scripts
2. Copy all .cs files from `D:\MetaQuestCameraStreamer\UnityQuestServer\Scripts\` to Assets/Scripts
3. Wait for Unity to compile

### Step 5: Setup Scene

1. Create empty GameObject, name it "StreamingManager"
2. Add these components to StreamingManager:
   - PassthroughManager.cs
   - StreamingServer.cs
   - VRUIManager.cs
3. Link components in Inspector:
   - StreamingServer: Set Passthrough Manager reference
   - VRUIManager: Set both StreamingServer and PassthroughManager references

### Step 6: Configure Permissions

Create file: Assets/Plugins/Android/AndroidManifest.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
    <uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
    <uses-permission android:name="android.permission.CAMERA" />
    <uses-feature android:name="android.hardware.vr.passthrough" android:required="true" />
</manifest>
```

### Step 7: Build and Deploy

1. Connect Quest via USB
2. Enable Developer Mode on Quest (Settings > System > Developer)
3. File > Build and Run
4. Name APK: QuestCameraStreamer.apk
5. Wait for deployment

## Part 2: Setting Up Android Client

### Step 1: Open in Android Studio

1. Open Android Studio
2. Select "Open an Existing Project"
3. Navigate to `D:\MetaQuestCameraStreamer\AndroidClient`
4. Wait for Gradle sync

### Step 2: Configure Gradle

Ensure these files are properly configured:
- `app/build.gradle` - Dependencies and build settings
- `settings.gradle` - Project configuration

### Step 3: Build and Install

1. Connect Android device via USB
2. Enable Developer Options and USB Debugging
3. Click "Run" button or press Shift+F10
4. Select your device
5. Wait for installation

## Usage Instructions

### On Meta Quest:

1. Put on Quest headset
2. Launch "Quest Camera Streamer" from App Library (Unknown Sources)
3. You'll see:
   - Passthrough view of your environment
   - Floating UI panel with IP address
   - Start/Stop streaming buttons
4. Note the IP address displayed
5. Tap "Start Streaming"

### On Android Device:

1. Launch "Quest Camera Viewer" app
2. Enter the IP address from Quest
3. Tap "Connect"
4. Wait for connection (few seconds)
5. Video stream will appear fullscreen

### Controls:

**Quest (VR):**
- Menu button: Toggle UI visibility
- Trigger: Select UI buttons
- Opacity slider: Adjust passthrough transparency

**Android:**
- Fullscreen: Hide/show controls
- Disconnect: End streaming session
- Back button: Exit app

## Troubleshooting

### Connection Issues

1. **Cannot connect:**
   - Ensure both devices on same Wi-Fi network
   - Check IP address is correct
   - Disable firewall temporarily
   - Restart both apps

2. **Black screen on Android:**
   - Check Quest passthrough is enabled
   - Restart Quest app
   - Verify streaming started on Quest

3. **Lag or stuttering:**
   - Move closer to Wi-Fi router
   - Reduce video bitrate in StreamingServer.cs
   - Close other apps on both devices

### Build Issues

1. **Unity build fails:**
   - Verify all packages installed
   - Check Android SDK/NDK paths
   - Clear Unity cache

2. **Android Studio build fails:**
   - Sync Gradle files
   - Clean and rebuild project
   - Update dependencies

## Performance Optimization

### Unity Settings:
- Reduce render texture resolution in PassthroughManager.cs
- Lower video bitrate in StreamingServer.cs
- Adjust framerate (default 30fps)

### Android Settings:
- Enable hardware acceleration
- Use landscape orientation only
- Keep screen on during streaming

## Network Requirements

- Minimum: 5 Mbps bandwidth
- Recommended: 10+ Mbps for smooth streaming
- Latency: <50ms for best experience
- Protocol: WebRTC over UDP

## Security Notes

- Stream is unencrypted by default
- Only use on trusted networks
- Consider adding authentication for production use

## Future Enhancements

Potential improvements:
- HTTPS/WSS for secure streaming
- Audio streaming support
- Recording capabilities
- Multiple client connections
- Automatic device discovery
- Custom video codec selection

## References

- Meta Quest Passthrough API Documentation
- Unity WebRTC Package Documentation
- Android WebRTC Implementation Guide
- DroidCam (inspiration for functionality)

## Support

For issues or questions:
1. Check troubleshooting section
2. Review Unity console logs
3. Check Android Logcat output
4. Verify network connectivity

## License

This project is for educational purposes. Ensure compliance with Meta's developer terms when distributing.
