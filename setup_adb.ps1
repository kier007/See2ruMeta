# See2ruMeta ADB Setup Script
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "   See2ruMeta ADB Setup Script" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as administrator
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "⚠️  Please run this script as Administrator for PATH setup" -ForegroundColor Yellow
    Write-Host ""
}

# Create tools directory
$toolsPath = "D:\See2ruMeta\Tools"
$adbPath = "$toolsPath\platform-tools"

Write-Host "📁 Creating tools directory..." -ForegroundColor Green
New-Item -ItemType Directory -Force -Path $toolsPath | Out-Null

# Download Platform Tools
$downloadUrl = "https://dl.google.com/android/repository/platform-tools-latest-windows.zip"
$zipPath = "$toolsPath\platform-tools.zip"

Write-Host "📥 Downloading Android Platform Tools..." -ForegroundColor Green
Write-Host "   This may take a few minutes..." -ForegroundColor Gray

try {
    Invoke-WebRequest -Uri $downloadUrl -OutFile $zipPath -UseBasicParsing
    Write-Host "✅ Download complete!" -ForegroundColor Green
} catch {
    Write-Host "❌ Download failed. Please check your internet connection." -ForegroundColor Red
    exit 1
}

# Extract Platform Tools
Write-Host "📦 Extracting Platform Tools..." -ForegroundColor Green
Expand-Archive -Path $zipPath -DestinationPath $toolsPath -Force
Write-Host "✅ Extraction complete!" -ForegroundColor Green

# Clean up zip file
Remove-Item $zipPath -Force

# Add to PATH (current session)
$env:Path += ";$adbPath"
Write-Host "✅ Added ADB to current session PATH" -ForegroundColor Green

# Try to add to system PATH (requires admin)
if ($isAdmin) {
    try {
        $currentPath = [Environment]::GetEnvironmentVariable("Path", "Machine")
        if ($currentPath -notlike "*$adbPath*") {
            [Environment]::SetEnvironmentVariable("Path", "$currentPath;$adbPath", "Machine")
            Write-Host "✅ Added ADB to system PATH permanently" -ForegroundColor Green
        } else {
            Write-Host "ℹ️  ADB already in system PATH" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "⚠️  Could not add to system PATH. Add manually: $adbPath" -ForegroundColor Yellow
    }
} else {
    Write-Host ""
    Write-Host "⚠️  To add ADB to PATH permanently, run as Administrator or:" -ForegroundColor Yellow
    Write-Host "   1. Press Win+X, select 'System'" -ForegroundColor Gray
    Write-Host "   2. Click 'Advanced system settings'" -ForegroundColor Gray
    Write-Host "   3. Click 'Environment Variables'" -ForegroundColor Gray
    Write-Host "   4. Edit 'Path' and add: $adbPath" -ForegroundColor Gray
}

# Test ADB
Write-Host ""
Write-Host "🔍 Testing ADB installation..." -ForegroundColor Green
& "$adbPath\adb.exe" version

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "✅ ADB Setup Complete!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📱 Next Steps:" -ForegroundColor Yellow
Write-Host "1. Connect your Quest 3 via USB-C" -ForegroundColor White
Write-Host "2. Connect your Android device via USB" -ForegroundColor White
Write-Host "3. Run: .\check_devices.ps1" -ForegroundColor White
Write-Host ""

# Create device check script
$checkScript = @'
# Check connected devices
Write-Host "Checking for connected devices..." -ForegroundColor Cyan
& "D:\See2ruMeta\Tools\platform-tools\adb.exe" devices -l
Write-Host ""
Write-Host "If no devices shown:" -ForegroundColor Yellow
Write-Host "- Quest 3: Enable Developer Mode in Meta Quest app"
Write-Host "- Android: Enable USB Debugging in Developer Options"
'@

$checkScript | Out-File -FilePath "D:\See2ruMeta\check_devices.ps1" -Encoding UTF8
Write-Host "Created check_devices.ps1 script" -ForegroundColor Green
