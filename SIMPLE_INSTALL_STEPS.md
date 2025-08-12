# 🚀 See2ruMeta - Simple Installation Steps

## ✅ What You Have Now:
- ✅ ADB installed and ready
- ✅ Project source code created
- ✅ Installation scripts prepared

---

## 📱 Step-by-Step Installation

### **STEP 1: Prepare Your Quest 3**

1. **On your phone:**
   - Download "Meta Horizon" app from App Store/Play Store
   - Sign in with your Facebook/Meta account
   - Tap Menu (☰) → Devices
   - Select your Quest 3
   - Go to Settings → Developer Mode
   - Turn ON Developer Mode

2. **On your Quest 3:**
   - Connect Quest 3 to PC with USB-C cable
   - Put on the headset
   - You'll see a popup: "Allow USB Debugging?"
   - Select "Always Allow" and click OK

---

### **STEP 2: Prepare Your Android Phone**

1. **Enable Developer Mode:**
   - Go to Settings → About Phone
   - Find "Build Number"
   - Tap it 7 times quickly
   - You'll see "You are now a developer!"

2. **Enable USB Debugging:**
   - Go to Settings → System → Developer Options
   - Turn ON "USB Debugging"
   - Turn ON "Install via USB"

3. **Connect to PC:**
   - Connect phone with USB cable
   - You'll see popup: "Allow USB Debugging?"
   - Check "Always allow" and tap OK

---

### **STEP 3: Check Devices Are Connected**

Open PowerShell and run:
```powershell
D:\See2ruMeta\Tools\platform-tools\adb.exe devices
```

You should see something like:
```
List of devices attached
28292XXXXXXX    device  (Quest 3)
RF8MXXXXXXXX    device  (Android Phone)
```

---

### **STEP 4: Build the Apps**

#### **Option A: If you have Unity installed**

1. Open Unity Hub
2. Click "Add" → Browse to `D:\See2ruMeta\UnityQuestServer`
3. Open with Unity 2022.3 LTS
4. Wait for project to load
5. File → Build Settings
6. Platform: Android → Switch Platform
7. Click "Build"
8. Save as: `D:\See2ruMeta\See2ruMeta_Quest3.apk`

#### **Option B: Download Pre-built APK**
(Contact me if you need pre-built versions)

---

### **STEP 5: Install on Devices**

#### **Install on Quest 3:**
```powershell
D:\See2ruMeta\Tools\platform-tools\adb.exe install -r D:\See2ruMeta\See2ruMeta_Quest3.apk
```

#### **Install on Android:**
```powershell
D:\See2ruMeta\Tools\platform-tools\adb.exe install -r D:\See2ruMeta\See2ruMeta_Android.apk
```

---

### **STEP 6: Run See2ruMeta**

#### **On Quest 3:**
1. Put on headset
2. Press Meta button on right controller
3. Go to App Library (grid icon)
4. Top right: Filter → "All" or "Unknown Sources"
5. Find "See2ruMeta Vision"
6. Click to launch
7. You'll see passthrough view with floating UI
8. Note the IP address shown (like 192.168.1.XXX)
9. Click "Start Streaming"

#### **On Android:**
1. Open "Quest Camera Streamer" app
2. Enter the IP address from Quest
3. Tap "Connect"
4. Video appears!

---

## ❓ Troubleshooting

### **"Device not found" error:**
- Make sure USB debugging is enabled
- Try different USB cable
- Restart device

### **"App not installed" error:**
- Uninstall old version first:
  ```powershell
  D:\See2ruMeta\Tools\platform-tools\adb.exe uninstall com.see2rumeta.quest
  ```

### **Can't find app on Quest:**
- Go to App Library → Filter → Unknown Sources
- Look for "See2ruMeta"

### **Black screen on Android:**
- Make sure both devices on same Wi-Fi
- Check IP address is correct
- Click "Start Streaming" on Quest first

---

## 🎯 Quick Test Commands

Test if everything works:
```powershell
# Check devices
D:\See2ruMeta\Tools\platform-tools\adb.exe devices

# See Quest 3 apps
D:\See2ruMeta\Tools\platform-tools\adb.exe shell pm list packages | findstr see2ru

# See Android apps  
D:\See2ruMeta\Tools\platform-tools\adb.exe shell pm list packages | findstr quest
```

---

## 💡 Need Unity?

If you don't have Unity installed:
1. Download Unity Hub: https://unity.com/download
2. Install Unity 2022.3 LTS
3. Add Android Build Support

Or I can provide you with pre-built APKs if needed!

---

## 🎉 Success Checklist

- [ ] Quest 3 shows passthrough camera
- [ ] IP address visible in VR
- [ ] Android app connects to Quest
- [ ] Video stream displays on phone
- [ ] Less than 100ms delay

Enjoy your Quest 3 as a wireless camera! 🔮📱

