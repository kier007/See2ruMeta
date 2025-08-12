@echo off
echo ============================================
echo    See2ruMeta Installation Script
echo ============================================
echo.

:menu
echo Select installation option:
echo 1. Install on Meta Quest 3
echo 2. Install on Android Device
echo 3. Build Unity Project (Quest 3)
echo 4. Build Android Project
echo 5. Install Both (Quest 3 + Android)
echo 6. Check Device Connections
echo 7. Exit
echo.
set /p choice="Enter your choice (1-7): "

if "%choice%"=="1" goto install_quest
if "%choice%"=="2" goto install_android
if "%choice%"=="3" goto build_unity
if "%choice%"=="4" goto build_android
if "%choice%"=="5" goto install_both
if "%choice%"=="6" goto check_devices
if "%choice%"=="7" goto end

:check_devices
echo.
echo Checking connected devices...
echo.
echo Quest 3 / Android devices:
adb devices
echo.
pause
goto menu

:install_quest
echo.
echo Installing See2ruMeta on Quest 3...
echo Please ensure:
echo - Quest 3 is connected via USB-C
echo - Developer Mode is enabled
echo - USB Debugging allowed
echo.
pause

if not exist "Builds\Quest\See2ruMeta_Quest3.apk" (
    echo ERROR: APK not found! Please build Unity project first.
    pause
    goto menu
)

echo Installing APK...
adb install -r "Builds\Quest\See2ruMeta_Quest3.apk"
if %errorlevel% equ 0 (
    echo.
    echo ✅ Successfully installed on Quest 3!
    echo.
    echo To launch: Go to App Library → Filter → All → See2ruMeta Vision
) else (
    echo.
    echo ❌ Installation failed. Please check:
    echo - Device is connected
    echo - Developer mode enabled
    echo - USB debugging allowed
)
echo.
pause
goto menu

:install_android
echo.
echo Installing See2ruMeta on Android Device...
echo Please ensure:
echo - Android device is connected via USB
echo - USB Debugging is enabled
echo.
pause

if not exist "AndroidClient\app\build\outputs\apk\debug\app-debug.apk" (
    echo ERROR: APK not found! Please build Android project first.
    pause
    goto menu
)

echo Installing APK...
adb install -r "AndroidClient\app\build\outputs\apk\debug\app-debug.apk"
if %errorlevel% equ 0 (
    echo.
    echo ✅ Successfully installed on Android!
    echo.
    echo App name: Quest Camera Streamer
) else (
    echo.
    echo ❌ Installation failed. Please check USB debugging is enabled.
)
echo.
pause
goto menu

:build_unity
echo.
echo Building Unity Project for Quest 3...
echo.
echo This requires Unity 2022.3 LTS to be installed.
echo Please build manually in Unity:
echo.
echo 1. Open Unity Hub
echo 2. Open project: D:\See2ruMeta\UnityQuestServer
echo 3. File → Build Settings → Android
echo 4. Build to: D:\See2ruMeta\Builds\Quest\See2ruMeta_Quest3.apk
echo.
pause
goto menu

:build_android
echo.
echo Building Android Project...
cd AndroidClient
echo Running Gradle build...
call gradlew.bat assembleDebug
if %errorlevel% equ 0 (
    echo.
    echo ✅ Build successful!
    echo APK location: AndroidClient\app\build\outputs\apk\debug\app-debug.apk
) else (
    echo.
    echo ❌ Build failed. Please check Android Studio and Gradle setup.
)
cd ..
echo.
pause
goto menu

:install_both
echo.
echo Installing on both Quest 3 and Android...
call :install_quest
call :install_android
goto menu

:end
echo.
echo Thank you for using See2ruMeta!
echo.
pause
exit
