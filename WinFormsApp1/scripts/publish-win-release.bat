@echo off
setlocal
cd /d "%~dp0\..\WinFormsApp1"

echo.
echo === Publishing ERP (self-contained, win-x64) ===
echo End users will NOT need to install .NET Desktop Runtime.
echo.

dotnet publish WinFormsApp1.csproj ^
  -c Release ^
  -r win-x64 ^
  --self-contained true ^
  -p:PublishSingleFile=true ^
  -p:IncludeNativeLibrariesForSelfExtract=true ^
  -p:EnableCompressionInSingleFile=true ^
  -p:PublishReadyToRun=true ^
  -o "..\publish\ERP-WinRelease"

if errorlevel 1 (
  echo.
  echo Publish FAILED.
  pause
  exit /b 1
)

echo.
echo === Done ===
echo Copy this folder to end users:
echo   %cd%\..\publish\ERP-WinRelease
echo.
echo Run: WinFormsApp1.exe
echo.
pause
