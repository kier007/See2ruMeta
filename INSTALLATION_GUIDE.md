# 📱 See2ruMeta Installation Guide

## 🎯 Quick Install (If you have pre-built APKs)

### For Meta Quest 3:
```bash
adb install -r See2ruMeta_Quest3.apk
```

### For Android:
```bash
adb install -r See2ruMeta_Android.apk
```

---

## 📋 Prerequisites

### Required Software:
- ✅ Unity 2022.3 LTS (for building Quest app)
- ✅ Android Studio (for building Android app)
- ✅ ADB (Android Debug Bridge)
- ✅ Meta Quest Developer Hub (optional but recommended)

### Required Hardware:
- ✅ Meta Quest 3 with Developer Mode enabled
- ✅ Android device (phone/tablet) with Android 7.0+
- ✅ USB-C cable for Quest 3
- ✅ USB cable for Android device
- ✅ Same Wi-Fi network for both devices

---

## 🔧 Part 1: Preparing Your Devices

### 1.1 Enable Developer Mode on Quest 3

1. **On your phone:**
   - Install Meta Quest app
   - Log in with your Meta account
   - Go to Menu → Devices → Select your Quest 3
   - Go to Settings → Developer Mode → Toggle ON

2. **On Quest 3:**
   - Put on headset
   - Go to Settings → System → Developer
   - Enable USB Connection Dialog

### 1.2 Enable USB Debugging on Android

1. Go to Settings → About Phone
2. Tap "Build Number" 7 times to enable Developer Options
3. Go to Settings → Developer Options
4. Enable "USB Debugging"
5. Enable "Install via USB" (if available)

---

## 🏗️ Part 2: Building the Applications

### 2.1 Building Quest 3 App (Unity)

**Step 1: Open Unity Project**
```bash
1. Launch Unity Hub
2. Click "Open" → Navigate to: D:\See2ruMeta\UnityQuestServer
3. Open with Unity 2022.3 LTS
```

**Step 2: Configure Build Settings**
```
File → Build Settings:
- Platform: Android ✓
- Texture Compression: ASTC
- Target Architecture: ARM64
```

**Step 3: Configure Player Settings**
```
Edit → Project Settings → Player:
- Package Name: com.see2rumeta.quest
- Minimum API Level: 29
- Target API Level: 32
- Scripting Backend: IL2CPP
```

**Step 4: Import Meta XR SDK**
```
Window → Package Manager:
1. Click "+" → Add package from git URL
2. Add: com.meta.xr.sdk.core
3. Import all Meta XR packages
```

**Step 5: Build APK**
```
1. File → Build Settings
2. Click "Build"
3. Save as: See2ruMeta_Quest3.apk
4. Wait for build to complete (5-10 minutes)
```

### 2.2 Building Android App

**Option A: Using Android Studio**

1. Open Android Studio
2. Open project: `D:\See2ruMeta\AndroidClient`
3. Wait for Gradle sync
4. Build → Build Bundle(s) / APK(s) → Build APK(s)
5. APK location: `app\build\outputs\apk\debug\app-debug.apk`

**Option B: Using Command Line**
```bash
cd D:\See2ruMeta\AndroidClient
gradlew.bat assembleDebug
```

---

## 📲 Part 3: Installing Applications

### 3.1 Install on Quest 3

**Method 1: Using ADB (Recommended)**

1. Connect Quest 3 via USB-C
2. Allow USB debugging when prompted in headset
3. Open Command Prompt/PowerShell
4. Run:
```bash
# Check if Quest 3 is connected
adb devices

# Install the app
adb install -r See2ruMeta_Quest3.apk
```

**Method 2: Using SideQuest**

1. Download and install SideQuest from https://sidequestvr.com
2. Connect Quest 3 via USB-C
3. Drag and drop APK into SideQuest
4. Click "Install APK"

**Method 3: Using Meta Quest Developer Hub**

1. Download Meta Quest Developer Hub
2. Connect Quest 3
3. Go to Device Manager → Apps
4. Click "Install APK" and select file

### 3.2 Install on Android Device

1. Connect Android device via USB
2. Allow USB debugging when prompted
3. Run:
```bash
# Check if device is connected
adb devices

# Install the app
adb install -r See2ruMeta_Android.apk
```

---

## 🚀 Part 4: Running See2ruMeta

### On Quest 3:

1. **Put on your Quest 3 headset**

2. **Navigate to App Library:**
   - Press Meta button on right controller
   - Select "App Library" (grid icon)
   
3. **Find See2ruMeta:**
   - Click filter dropdown (top right)
   - Select "Unknown Sources" or "All"
   - Look for "See2ruMeta Vision"

4. **Launch the app:**
   - Point and click on See2ruMeta Vision
   - Wait for passthrough to activate
   - Note the IP address shown (e.g., 192.168.1.100)

5. **Start streaming:**
   - Click "Start Streaming" button in VR

### On Android Device:

1. **Launch Quest Camera Streamer app**

2. **Connect to Quest 3:**
   - Enter the IP address from Quest 3
   - Tap "Connect"
   - Wait for connection (2-5 seconds)

3. **View stream:**
   - Camera feed appears automatically
   - Tap "Fullscreen" for immersive view

---

## 🔍 Troubleshooting

### Quest 3 Issues:

**App doesn't appear in library:**
- Go to App Library → Filter → Unknown Sources
- Or search for "See2ruMeta"

**Installation fails:**
```bash
# Uninstall old version first
adb uninstall com.see2rumeta.quest

# Then reinstall
adb install See2ruMeta_Quest3.apk
```

**Black screen in Quest:**
- Ensure passthrough permission is granted
- Settings → Apps → See2ruMeta → Permissions → Camera → Allow

### Android Issues:

**Cannot connect to Quest:**
- Verify both devices on same Wi-Fi
- Check IP address is correct
- Disable mobile data on Android
- Try restarting both apps

**ADB not recognized:**
```bash
# Windows - Install ADB
1. Download Platform Tools: https://developer.android.com/studio/releases/platform-tools
2. Extract to C:\adb
3. Add to PATH: setx PATH "%PATH%;C:\adb"
4. Restart terminal
```

### Network Issues:

**High latency/lag:**
- Move closer to Wi-Fi router
- Use 5GHz Wi-Fi instead of 2.4GHz
- Close other streaming apps
- Reduce video quality in settings

---

## 🎮 Quick Commands Reference

```bash
# Check connected devices
adb devices

# Install on Quest 3
adb -s [QUEST_SERIAL] install -r See2ruMeta_Quest3.apk

# Install on Android
adb -s [ANDROID_SERIAL] install -r See2ruMeta_Android.apk

# View Quest 3 logs
adb -s [QUEST_SERIAL] logcat | grep See2ruMeta

# Uninstall from Quest 3
adb uninstall com.see2rumeta.quest

# Uninstall from Android
adb uninstall com.questcamerastreamer.client

# Launch on Quest (after install)
adb shell am start -n com.see2rumeta.quest/com.unity3d.player.UnityPlayerActivity
```

---

## 📊 Verification Checklist

After installation, verify:

- [ ] Quest 3 app launches and shows passthrough
- [ ] IP address is displayed in VR UI
- [ ] "Start Streaming" button works
- [ ] Android app launches successfully
- [ ] Can enter IP address
- [ ] Connection establishes within 5 seconds
- [ ] Video stream displays on Android
- [ ] Latency is under 100ms
- [ ] Fullscreen mode works

---

## 🆘 Need Help?

### Common Solutions:

1. **Restart both devices**
2. **Reinstall apps**
3. **Check Wi-Fi connection**
4. **Update Quest 3 firmware**
5. **Clear app cache and data**

### Support Resources:

- Unity Forums: https://forum.unity.com
- Meta Quest Developer Forums: https://forums.oculusvr.com
- Android Developers: https://developer.android.com

---

## 🎉 Success!

Once installed, you should be able to:
- Stream Quest 3 passthrough camera to Android
- Control streaming from VR interface
- View high-quality, low-latency video
- Use your Quest 3 as a wireless camera!

Enjoy See2ruMeta! 🔮📱
