@echo off
REM TheToyRoomDesktop - Git Push Script (Windows)
REM This script helps you push the project to GitHub

echo ==================================
echo TheToyRoomDesktop - Git Setup
echo ==================================
echo.

REM Check if git is installed
where git >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo X Git is not installed. Please install Git first.
    echo   Download from: https://git-scm.com/download/win
    pause
    exit /b 1
)

echo [OK] Git is installed
echo.

REM Check if we're in the right directory
if not exist "TheToyRoomDesktop.csproj" (
    echo X Error: Not in TheToyRoomDesktop directory
    echo   Please run this script from the project root
    pause
    exit /b 1
)

echo [OK] In correct directory
echo.

REM Check if git repo is initialized
if not exist ".git" (
    echo [*] Initializing Git repository...
    git init
    echo [OK] Git repository initialized
    echo.
)

REM Check if remote is set
git remote | findstr "origin" >nul
if %ERRORLEVEL% NEQ 0 (
    echo [*] Adding remote repository...
    git remote add origin https://github.com/pikapp1977/TheToyRoomDesktop.git
    echo [OK] Remote repository added
    echo.
) else (
    echo [OK] Remote repository already configured
    echo.
)

REM Show git status
echo [*] Current Git Status:
echo ---------------------
git status
echo.

REM Ask if user wants to continue
set /p CONTINUE="Do you want to commit and push all files? (y/n) "
if /i not "%CONTINUE%"=="y" (
    echo X Aborted by user
    pause
    exit /b 0
)

REM Stage all files
echo.
echo [*] Staging all files...
git add .
echo [OK] Files staged
echo.

REM Commit
echo [*] Creating commit...
git commit -m "Initial commit: TheToyRoomDesktop WPF application" -m "- Complete WPF desktop application" -m "- Replicates all TheToyRoom web app functionality" -m "- Models: Collectible, Manufacturer, Deco" -m "- Services: Database, Collectible, Manufacturer, Deco, Export" -m "- Views: Home, Collection, AddItem, Reports, Importer, Settings" -m "- Full documentation included" -m "- .NET 8 with SQLite and OpenXml"

if %ERRORLEVEL% EQU 0 (
    echo [OK] Commit created
    echo.
) else (
    echo X Commit failed
    pause
    exit /b 1
)

REM Set main branch
echo [*] Setting main branch...
git branch -M main
echo [OK] Main branch set
echo.

REM Push to GitHub
echo [*] Pushing to GitHub...
echo    (You may be prompted for credentials)
echo.

git push -u origin main

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ==================================
    echo [OK] SUCCESS!
    echo ==================================
    echo.
    echo Your code has been pushed to:
    echo https://github.com/pikapp1977/TheToyRoomDesktop
    echo.
    echo Next steps:
    echo 1. Visit the repository on GitHub
    echo 2. Add a description and topics
    echo 3. Verify all files are present
    echo 4. Test the application
    echo.
) else (
    echo.
    echo X Push failed
    echo.
    echo Common issues:
    echo 1. Authentication - You may need a Personal Access Token
    echo 2. Repository may already have content
    echo.
    echo If repository has content, try:
    echo   git pull origin main --allow-unrelated-histories
    echo   git push -u origin main
    echo.
)

pause
