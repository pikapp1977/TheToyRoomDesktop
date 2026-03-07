# Local Build Guide

## Prerequisites

### Option 1: Visual Studio 2022 (Recommended)
Download and install **Visual Studio 2022 Community** (free):
- https://visualstudio.microsoft.com/downloads/
- During installation, select **.NET desktop development** workload
- This includes everything you need

### Option 2: .NET SDK Only (Command Line)
Download and install **.NET 8 SDK**:
- https://dotnet.microsoft.com/download/dotnet/8.0
- Choose "SDK" for your OS (Windows x64)
- Install with default options

## Quick Start

### Using the Build Script (Easiest)

1. Open Windows Explorer
2. Navigate to: `C:\users\admin\documents\TheToyRoomDesktop`
3. **Double-click**: `build-local.bat`
4. The script will:
   - Check for .NET SDK
   - Clean previous builds
   - Restore packages
   - Build Debug configuration
   - Build Release configuration
   - Publish single-file executable
5. If successful, run: `run-app.bat`

### Using Visual Studio

1. **Open Visual Studio 2022**
2. **File** → **Open** → **Project/Solution**
3. Navigate to `C:\users\admin\documents\TheToyRoomDesktop`
4. Open `TheToyRoomDesktop.csproj`
5. Press **F5** (or click **Start**)
6. Visual Studio will:
   - Restore packages
   - Build the project
   - Run the application
7. If there are errors, they'll appear in the **Error List** window

### Using Command Line

```cmd
cd C:\users\admin\documents\TheToyRoomDesktop

# Restore packages
dotnet restore

# Build
dotnet build

# Run
dotnet run
```

## Expected First Build Issues

### Common Errors You Might See

#### 1. XAML Errors
```
Error: The name 'xyz' does not exist in the namespace
```
**Fix**: Check namespace declarations in XAML files

#### 2. Namespace Mismatches
```
Error: The type or namespace name 'Services' could not be found
```
**Fix**: Verify `using` statements in .cs files

#### 3. Missing References
```
Error: The type or namespace name 'Collectible' could not be found
```
**Fix**: Add proper `using TheToyRoomDesktop.Models;`

## Fixing Errors

### Step-by-Step Error Resolution

1. **Build the project**:
   ```cmd
   dotnet build
   ```

2. **Read the first error** (ignore subsequent errors - they often cascade)

3. **Open the file** mentioned in the error

4. **Fix the issue**:
   - Add missing `using` statements
   - Fix namespace declarations
   - Correct XAML syntax

5. **Build again**:
   ```cmd
   dotnet build
   ```

6. **Repeat** until no errors

### Common Fixes

#### Add Missing Using Statements

If you see errors about types not found, add these to the top of .cs files:

```csharp
using TheToyRoomDesktop.Models;
using TheToyRoomDesktop.Services;
using System.Windows;
using System.Windows.Controls;
```

#### Fix XAML Namespaces

In XAML files, ensure you have:

```xml
<Window x:Class="TheToyRoomDesktop.Views.HomePage"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
```

## Project Structure

```
TheToyRoomDesktop/
├── App.xaml                    # Application entry
├── App.xaml.cs
├── MainWindow.xaml             # Main window
├── MainWindow.xaml.cs
├── Models/                     # Data models
│   ├── Collectible.cs
│   ├── Manufacturer.cs
│   └── Deco.cs
├── Services/                   # Business logic
│   ├── DatabaseService.cs
│   ├── CollectibleService.cs
│   ├── ManufacturerService.cs
│   ├── DecoService.cs
│   └── ExportService.cs
└── Views/                      # UI pages
    ├── HomePage.xaml
    ├── CollectionPage.xaml
    ├── AddItemPage.xaml
    ├── ReportsPage.xaml
    ├── ImporterPage.xaml
    ├── SettingsPage.xaml
    └── ViewItemWindow.xaml
```

## Build Output Locations

After successful build:

### Debug Build
```
bin\Debug\net8.0-windows\TheToyRoomDesktop.exe
```
- Not self-contained
- Requires .NET runtime
- Smaller file size (~100KB)

### Release Build
```
bin\Release\net8.0-windows\TheToyRoomDesktop.exe
```
- Optimized
- Still requires .NET runtime

### Published (Self-Contained)
```
bin\Release\net8.0-windows\win-x64\publish\TheToyRoomDesktop.exe
```
- Single file
- Includes .NET runtime
- ~70-80 MB
- **This is what you want for distribution**

## Testing the Application

Once built successfully:

1. **Run the executable**:
   ```cmd
   bin\Debug\net8.0-windows\TheToyRoomDesktop.exe
   ```

2. **Test basic functionality**:
   - Dashboard loads
   - Can navigate between pages
   - Can add a test collectible
   - Database is created in `%LOCALAPPDATA%\TheToyRoom\`

3. **Check for runtime errors**:
   - Any crashes or exceptions
   - UI rendering issues
   - Database operations working

## After Successful Build

Once the application builds and runs successfully:

### 1. Commit the Working Code

If you made any fixes during the build:

```cmd
git add .
git commit -m "Fix compilation errors - application now builds successfully"
git push
```

### 2. GitHub Actions Should Work

Once you push working code, GitHub Actions will automatically:
- Build the project
- Create the installer
- Upload artifacts

### 3. Test the Installer

Run `build-local.bat` which will create the published exe, then you can test the Inno Setup installer:

```cmd
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" TheToyRoomDesktopSetup.iss
```

This will create `TheToyRoomDesktopSetup_v1.0.0.exe`

## Troubleshooting

### .NET SDK Not Found

**Error**: `'dotnet' is not recognized`

**Fix**: 
1. Download .NET 8 SDK from https://dotnet.microsoft.com/download/dotnet/8.0
2. Install with default options
3. Restart command prompt
4. Try again

### Visual Studio Won't Open Project

**Error**: Project type not supported

**Fix**:
1. Make sure you have .NET desktop development workload
2. Visual Studio Installer → Modify
3. Check **.NET desktop development**
4. Install

### Build Succeeds But App Won't Run

**Check**:
- Is SQLite.Interop.dll in the output directory?
- Are all dependencies copied?
- Try running from command line to see error messages:
  ```cmd
  cd bin\Debug\net8.0-windows
  TheToyRoomDesktop.exe
  ```

### Database Errors

The database will be created at:
```
C:\Users\admin\AppData\Local\TheToyRoom\thetoyroom.db
```

If you get database errors:
- Check the folder exists
- Check write permissions
- Delete the .db file and let it recreate

## Getting Help

### View Build Errors in Visual Studio

1. **View** → **Error List**
2. Click on each error to jump to the file
3. Read the error message carefully
4. Fix and rebuild

### Command Line Verbose Build

For more details:

```cmd
dotnet build -v detailed
```

This shows exactly what the compiler is doing.

### Check File Encoding

All .cs files should be UTF-8 encoded. If you see weird characters, re-save as UTF-8.

## Next Steps After Successful Build

1. ✅ **Test the application** thoroughly
2. ✅ **Commit any fixes** to GitHub
3. ✅ **Verify GitHub Actions** now works
4. ✅ **Create a release** with the installer
5. ✅ **Share** with users!

## Quick Reference Commands

```cmd
# Clean
dotnet clean

# Restore packages
dotnet restore

# Build Debug
dotnet build

# Build Release
dotnet build -c Release

# Run
dotnet run

# Publish single file
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true

# Build installer (after installing Inno Setup)
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" TheToyRoomDesktopSetup.iss
```

## Success Checklist

- [ ] .NET SDK installed
- [ ] Project builds without errors
- [ ] Application runs successfully
- [ ] Can add/edit/delete collectibles
- [ ] Database operations work
- [ ] Excel import/export works
- [ ] All pages load without errors
- [ ] Changes committed to Git
- [ ] GitHub Actions build succeeds
- [ ] Installer created successfully

Once all items are checked, you're ready to distribute!
