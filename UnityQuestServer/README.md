# Meta Quest Camera Streaming Server

Unity application that streams Meta Quest passthrough camera feed over the local network.

## Setup Instructions

### Prerequisites
- Unity 2022.3 LTS or later
- Meta XR SDK (formerly Oculus Integration)
- Android Build Support for Unity
- Meta Quest 2/3/Pro device

### Project Setup

1. Create a new Unity 3D project
2. Import the following packages via Package Manager:
   - XR Plugin Management
   - OpenXR Plugin
   - Meta XR SDK

3. Configure Build Settings:
   - Switch platform to Android
   - Set Texture Compression to ASTC
   - Enable IL2CPP for ARM64

4. Configure XR Settings:
   - Enable OpenXR
   - Add Meta Quest feature set
   - Enable Passthrough feature

5. Import the provided scripts into your project

### Building for Quest

1. Enable Developer Mode on your Quest
2. Connect Quest via USB
3. Build and Run from Unity

## Architecture

- **PassthroughManager.cs**: Handles Meta Quest passthrough API
- **StreamingServer.cs**: Manages WebRTC streaming server
- **NetworkManager.cs**: Handles network discovery and connection
- **UIManager.cs**: VR UI for controlling the stream

## Network Protocol

Uses WebRTC for low-latency video streaming:
- Video Codec: H.264
- Audio: Disabled (not needed for camera feed)
- Transport: UDP with STUN/TURN support
