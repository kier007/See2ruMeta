# See2ruMeta ADB Setup Script
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "   See2ruMeta ADB Setup Script" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Create tools directory
$toolsPath = "D:\See2ruMeta\Tools"
$adbPath = "$toolsPath\platform-tools"

Write-Host "Creating tools directory..." -ForegroundColor Green
New-Item -ItemType Directory -Force -Path $toolsPath | Out-Null

# Download Platform Tools
$downloadUrl = "https://dl.google.com/android/repository/platform-tools-latest-windows.zip"
$zipPath = "$toolsPath\platform-tools.zip"

Write-Host "Downloading Android Platform Tools..." -ForegroundColor Green
Write-Host "   This may take a few minutes..." -ForegroundColor Gray

try {
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    Invoke-WebRequest -Uri $downloadUrl -OutFile $zipPath -UseBasicParsing
    Write-Host "Download complete!" -ForegroundColor Green
} catch {
    Write-Host "Download failed. Please check your internet connection." -ForegroundColor Red
    exit 1
}

# Extract Platform Tools
Write-Host "Extracting Platform Tools..." -ForegroundColor Green
Expand-Archive -Path $zipPath -DestinationPath $toolsPath -Force
Write-Host "Extraction complete!" -ForegroundColor Green

# Clean up zip file
Remove-Item $zipPath -Force

# Test ADB
Write-Host ""
Write-Host "Testing ADB installation..." -ForegroundColor Green
& "$adbPath\adb.exe" version

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "ADB Setup Complete!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "ADB Location: $adbPath" -ForegroundColor Yellow
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Connect your Quest 3 via USB-C" -ForegroundColor White
Write-Host "2. Connect your Android device via USB" -ForegroundColor White
Write-Host "3. Enable Developer Mode on both devices" -ForegroundColor White
Write-Host ""
