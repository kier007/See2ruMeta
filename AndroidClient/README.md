# Meta Quest Camera Stream Viewer - Android Client

Native Android application that receives and displays the camera feed from Meta Quest.

## Features

- WebRTC-based video streaming client
- Low-latency video playback
- Simple connection interface
- Full-screen video display
- Connection status indicators

## Requirements

- Android 7.0 (API level 24) or higher
- Android Studio Arctic Fox or later
- Network connection on same LAN as Quest device

## Setup Instructions

1. Open the project in Android Studio
2. Sync Gradle dependencies
3. Build and run on Android device

## Usage

1. Start the Quest streaming server application
2. Note the IP address displayed in Quest VR UI
3. Enter the IP address in the Android app
4. Tap "Connect" to start viewing the stream

## Architecture

- **MainActivity**: Main UI and connection management
- **WebRTCClient**: Handles WebRTC connection and video streaming
- **VideoRenderer**: Manages video display and rendering
- **NetworkManager**: Handles network discovery and connection state

## Dependencies

- WebRTC Android SDK
- ExoPlayer for video playback
- Kotlin Coroutines for async operations
- Material Design Components
