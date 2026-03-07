@echo off
REM TheToyRoomDesktop - Local Build Script
REM This script builds the application locally on Windows

echo ========================================
echo TheToyRoomDesktop - Local Build
echo ========================================
echo.

REM Change to the directory where this script is located
cd /d "%~dp0"
echo Working directory: %CD%
echo.

REM Check if dotnet is installed
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] .NET SDK not found!
    echo.
    echo Please install .NET 8 SDK from:
    echo https://dotnet.microsoft.com/download/dotnet/8.0
    echo.
    pause
    exit /b 1
)

echo [OK] .NET SDK found
dotnet --version
echo.

REM Check if we're in the right directory
if not exist "TheToyRoomDesktop.csproj" (
    echo [ERROR] TheToyRoomDesktop.csproj not found
    echo Current directory: %CD%
    echo Please run this script from the project directory
    pause
    exit /b 1
)

echo [OK] Project file found
echo.

REM Clean previous builds
echo [1/5] Cleaning previous builds...
if exist "bin" rmdir /s /q bin
if exist "obj" rmdir /s /q obj
echo [OK] Clean complete
echo.

REM Restore dependencies
echo [2/5] Restoring NuGet packages...
dotnet restore TheToyRoomDesktop.csproj
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Restore failed!
    pause
    exit /b 1
)
echo [OK] Restore complete
echo.

REM Build Debug
echo [3/5] Building Debug configuration...
echo This will show any compilation errors...
echo.
dotnet build TheToyRoomDesktop.csproj -c Debug
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Build failed!
    echo.
    echo The errors above show exactly what needs to be fixed.
    echo.
    echo Save the error messages and we can fix them!
    echo.
    pause
    exit /b 1
)
echo [OK] Debug build complete
echo.

REM Build Release
echo [4/5] Building Release configuration...
dotnet build TheToyRoomDesktop.csproj -c Release
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Release build failed!
    pause
    exit /b 1
)
echo [OK] Release build complete
echo.

REM Publish
echo [5/5] Publishing application...
dotnet publish TheToyRoomDesktop.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Publish failed!
    pause
    exit /b 1
)
echo [OK] Publish complete
echo.

REM Show results
echo ========================================
echo BUILD SUCCESSFUL!
echo ========================================
echo.
echo Output files:
echo  - Debug:   bin\Debug\net8.0-windows\TheToyRoomDesktop.exe
echo  - Release: bin\Release\net8.0-windows\TheToyRoomDesktop.exe
echo  - Publish: bin\Release\net8.0-windows\win-x64\publish\TheToyRoomDesktop.exe
echo.
echo To run the application:
echo   bin\Debug\net8.0-windows\TheToyRoomDesktop.exe
echo.
echo Or run the published version (single file):
echo   bin\Release\net8.0-windows\win-x64\publish\TheToyRoomDesktop.exe
echo.

pause
