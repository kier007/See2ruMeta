# See2ruMeta Quick Installation Script
$adb = "D:\See2ruMeta\Tools\platform-tools\adb.exe"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   See2ruMeta Quick Installer" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check for connected devices
Write-Host "Checking for connected devices..." -ForegroundColor Yellow
Write-Host ""
& $adb devices -l
Write-Host ""

# Menu
Write-Host "What would you like to do?" -ForegroundColor Green
Write-Host ""
Write-Host "1. Prepare Quest 3 for installation" -ForegroundColor White
Write-Host "2. Prepare Android device for installation" -ForegroundColor White
Write-Host "3. Install pre-built APKs (if available)" -ForegroundColor White
Write-Host "4. Build and install from source" -ForegroundColor White
Write-Host "5. Check device status" -ForegroundColor White
Write-Host ""

$choice = Read-Host "Enter your choice (1-5)"

switch ($choice) {
    "1" {
        Write-Host ""
        Write-Host "QUEST 3 PREPARATION STEPS:" -ForegroundColor Cyan
        Write-Host "=========================" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "1. On your phone:" -ForegroundColor Yellow
        Write-Host "   - Open Meta Quest app" -ForegroundColor White
        Write-Host "   - Go to Menu -> Devices" -ForegroundColor White
        Write-Host "   - Select your Quest 3" -ForegroundColor White
        Write-Host "   - Settings -> Developer Mode -> ON" -ForegroundColor White
        Write-Host ""
        Write-Host "2. On Quest 3:" -ForegroundColor Yellow
        Write-Host "   - Connect USB-C cable to PC" -ForegroundColor White
        Write-Host "   - Put on headset" -ForegroundColor White
        Write-Host "   - Allow USB debugging when prompted" -ForegroundColor White
        Write-Host ""
        Write-Host "3. Verify connection:" -ForegroundColor Yellow
        & $adb devices
        Write-Host ""
        Write-Host "You should see your Quest 3 listed above" -ForegroundColor Green
    }
    
    "2" {
        Write-Host ""
        Write-Host "ANDROID DEVICE PREPARATION:" -ForegroundColor Cyan
        Write-Host "===========================" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "1. Enable Developer Options:" -ForegroundColor Yellow
        Write-Host "   - Settings -> About Phone" -ForegroundColor White
        Write-Host "   - Tap 'Build Number' 7 times" -ForegroundColor White
        Write-Host ""
        Write-Host "2. Enable USB Debugging:" -ForegroundColor Yellow
        Write-Host "   - Settings -> Developer Options" -ForegroundColor White
        Write-Host "   - Enable 'USB Debugging'" -ForegroundColor White
        Write-Host "   - Enable 'Install via USB'" -ForegroundColor White
        Write-Host ""
        Write-Host "3. Connect and verify:" -ForegroundColor Yellow
        Write-Host "   - Connect USB cable to PC" -ForegroundColor White
        Write-Host "   - Allow USB debugging when prompted" -ForegroundColor White
        Write-Host ""
        & $adb devices
        Write-Host ""
        Write-Host "You should see your Android device listed above" -ForegroundColor Green
    }
    
    "3" {
        Write-Host ""
        Write-Host "Looking for pre-built APKs..." -ForegroundColor Yellow
        
        # Check for Quest APK
        $questApk = "D:\See2ruMeta\Builds\Quest\See2ruMeta_Quest3.apk"
        if (Test-Path $questApk) {
            Write-Host "Found Quest 3 APK!" -ForegroundColor Green
            Write-Host "Installing to Quest 3..." -ForegroundColor Yellow
            & $adb install -r $questApk
        } else {
            Write-Host "Quest 3 APK not found. Please build in Unity first." -ForegroundColor Red
            Write-Host "Location needed: $questApk" -ForegroundColor Gray
        }
        
        # Check for Android APK
        $androidApk = "D:\See2ruMeta\AndroidClient\app\build\outputs\apk\debug\app-debug.apk"
        if (Test-Path $androidApk) {
            Write-Host ""
            Write-Host "Found Android APK!" -ForegroundColor Green
            Write-Host "Installing to Android device..." -ForegroundColor Yellow
            & $adb install -r $androidApk
        } else {
            Write-Host ""
            Write-Host "Android APK not found. Please build in Android Studio first." -ForegroundColor Red
            Write-Host "Location needed: $androidApk" -ForegroundColor Gray
        }
    }
    
    "4" {
        Write-Host ""
        Write-Host "BUILD FROM SOURCE:" -ForegroundColor Cyan
        Write-Host "==================" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "For Quest 3 (Unity):" -ForegroundColor Yellow
        Write-Host "1. Open Unity Hub" -ForegroundColor White
        Write-Host "2. Add project: D:\See2ruMeta\UnityQuestServer" -ForegroundColor White
        Write-Host "3. Open with Unity 2022.3 LTS" -ForegroundColor White
        Write-Host "4. File -> Build Settings -> Android" -ForegroundColor White
        Write-Host "5. Build to: D:\See2ruMeta\Builds\Quest\See2ruMeta_Quest3.apk" -ForegroundColor White
        Write-Host ""
        Write-Host "For Android (Android Studio):" -ForegroundColor Yellow
        Write-Host "1. Open Android Studio" -ForegroundColor White
        Write-Host "2. Open project: D:\See2ruMeta\AndroidClient" -ForegroundColor White
        Write-Host "3. Build -> Build APK(s)" -ForegroundColor White
        Write-Host ""
        Write-Host "Or build Android from command line:" -ForegroundColor Yellow
        Write-Host "cd D:\See2ruMeta\AndroidClient" -ForegroundColor Gray
        Write-Host ".\gradlew.bat assembleDebug" -ForegroundColor Gray
    }
    
    "5" {
        Write-Host ""
        Write-Host "DEVICE STATUS:" -ForegroundColor Cyan
        Write-Host "==============" -ForegroundColor Cyan
        Write-Host ""
        & $adb devices -l
        Write-Host ""
        
        $devices = & $adb devices
        if ($devices -match "device") {
            Write-Host "Devices are connected and ready!" -ForegroundColor Green
        } else {
            Write-Host "No devices detected. Please check:" -ForegroundColor Red
            Write-Host "- USB cables are connected" -ForegroundColor Yellow
            Write-Host "- Developer mode is enabled" -ForegroundColor Yellow
            Write-Host "- USB debugging is allowed" -ForegroundColor Yellow
        }
    }
    
    default {
        Write-Host "Invalid choice. Please run the script again." -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
