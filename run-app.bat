@echo off
REM Run TheToyRoomDesktop application

if exist "bin\Debug\net8.0-windows\TheToyRoomDesktop.exe" (
    echo Starting TheToyRoomDesktop...
    start "" "bin\Debug\net8.0-windows\TheToyRoomDesktop.exe"
) else (
    echo Application not built yet!
    echo Run build-local.bat first
    pause
)
