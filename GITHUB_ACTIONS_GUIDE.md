# GitHub Actions Setup Guide

## 🎉 What Was Set Up

Your repository now has automated build and installer creation through GitHub Actions, just like your TimeTrackerWPF project!

### Files Created

1. **`.github/workflows/build.yml`** - GitHub Actions workflow
2. **`TheToyRoomDesktopSetup.iss`** - Inno Setup installer script
3. **`.github/workflows/README.md`** - Workflow documentation

## 🚀 What Happens Automatically

Every time you push code to the `main` branch, GitHub Actions will:

1. ✅ Restore all NuGet packages
2. ✅ Build the project
3. ✅ Create a self-contained Windows executable
4. ✅ Install Inno Setup
5. ✅ Create a Windows installer
6. ✅ Upload both files as artifacts

## 📦 What You Get

After each successful build, you can download:

### 1. Portable Executable
- **Artifact Name**: `TheToyRoomDesktop-Portable`
- **File**: `TheToyRoomDesktop.exe`
- **Size**: ~70-80 MB (self-contained)
- **Use Case**: Run directly without installation

### 2. Windows Installer
- **Artifact Name**: `TheToyRoomDesktop-Installer`
- **File**: `TheToyRoomDesktopSetup_v1.0.0.exe`
- **Size**: ~70-80 MB
- **Features**:
  - Installs to `C:\Program Files\TheToyRoomDesktop`
  - Creates Start Menu shortcut
  - Optional Desktop shortcut
  - Professional installer UI
  - Includes uninstaller

## 🔍 How to Access Builds

### From GitHub Actions Tab

1. Go to: https://github.com/pikapp1977/TheToyRoomDesktop/actions
2. Click on the most recent workflow run
3. Scroll to the bottom "Artifacts" section
4. Download either artifact

### From a Specific Commit

1. Go to your repository
2. Click on "Commits"
3. Find the commit you want
4. Click the green checkmark (✓) or red X (✗)
5. Click "Details" → View workflow run

## 🎯 Triggering a Build

### Automatic Triggers

The workflow runs automatically on:
- Push to `main` branch
- Pull requests to `main` branch

### Manual Trigger

1. Go to Actions tab
2. Click "Build WPF Installer" in the left sidebar
3. Click "Run workflow" button
4. Select branch (usually `main`)
5. Click green "Run workflow" button

## 📝 Updating the Version Number

To release a new version:

### 1. Update Inno Setup Script

Edit `TheToyRoomDesktopSetup.iss`:

```inno
#define MyAppVersion "1.0.1"  ; Change this line
```

### 2. Commit and Push

```bash
git add TheToyRoomDesktopSetup.iss
git commit -m "Bump version to 1.0.1"
git push
```

### 3. Wait for Build

The workflow will automatically start and create the new installer with the updated version.

## 🎁 Creating a Release

After a successful build:

### Step 1: Create Release on GitHub

1. Go to **Releases** → **Create a new release**
2. **Tag**: `v1.0.0` (must start with 'v')
3. **Title**: `TheToyRoomDesktop v1.0.0`
4. **Target**: `main` branch

### Step 2: Download Artifacts

1. Go to the Actions tab
2. Find the successful workflow run
3. Download both artifacts:
   - `TheToyRoomDesktop-Portable`
   - `TheToyRoomDesktop-Installer`

### Step 3: Attach to Release

1. Extract the downloaded zip files
2. Attach files to your release:
   - `TheToyRoomDesktop.exe`
   - `TheToyRoomDesktopSetup_v1.0.0.exe`

### Step 4: Write Release Notes

Example release notes:

```markdown
## TheToyRoomDesktop v1.0.0

### New Features
- Complete WPF desktop application
- Dashboard with collection statistics
- Full CRUD operations for collectibles
- Search and filter functionality
- Excel import/export
- Image support
- Reports by manufacturer

### Installation

**Option 1: Installer (Recommended)**
Download `TheToyRoomDesktopSetup_v1.0.0.exe` and run it.

**Option 2: Portable**
Download `TheToyRoomDesktop.exe` and run directly (no installation needed).

### Requirements
- Windows 10 or later
- ~80 MB disk space

### Database
The application stores its database at:
`%LOCALAPPDATA%\TheToyRoom\thetoyroom.db`

### Notes
This is compatible with the TheToyRoom Blazor web application database.
```

### Step 5: Publish Release

Click "Publish release" - your users can now download the installer!

## 🔧 Troubleshooting

### Build Fails

**Check Logs:**
1. Click on the failed workflow run
2. Click on "build" job
3. Expand failed steps to see error messages

**Common Issues:**
- Missing files in repository
- Syntax error in `.csproj` file
- Incorrect paths in workflow

### Artifacts Not Available

**Possible Causes:**
- Workflow didn't complete successfully
- Artifacts expire after 90 days
- Must be logged in to download

### Installer Issues

**Inno Setup Compilation Fails:**
- Check `TheToyRoomDesktopSetup.iss` syntax
- Verify publish directory exists
- Check file paths are correct

## 📊 Build Status Badge

Add a build status badge to your README:

```markdown
![Build Status](https://github.com/pikapp1977/TheToyRoomDesktop/workflows/Build%20WPF%20Installer/badge.svg)
```

Result: Shows green ✓ or red ✗ indicating build status

## 🎨 Customizing the Installer

Edit `TheToyRoomDesktopSetup.iss` to customize:

### Change Install Location

```inno
DefaultDirName={autopf}\YourCustomFolder
```

### Add License File

```inno
[Files]
Source: "LICENSE.txt"; DestDir: "{app}"; Flags: ignoreversion
```

### Add More Icons

```inno
[Icons]
Name: "{group}\Documentation"; Filename: "{app}\README.txt"
```

### Change Installer Graphics

Add custom wizard images and sidebar graphics.

## 🔄 Workflow Configuration

The workflow is defined in `.github/workflows/build.yml`:

### Key Settings

```yaml
runs-on: windows-latest  # Must be Windows for WPF
dotnet-version: '8.0.x'  # .NET 8
```

### Build Configuration

```yaml
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

- **Release**: Optimized build
- **win-x64**: Windows 64-bit
- **self-contained**: Includes .NET runtime
- **PublishSingleFile**: Single .exe file

## 📈 Build Metrics

Track your builds:
- **Build Time**: ~3-5 minutes typically
- **Artifact Size**: ~70-80 MB
- **Retention**: 90 days (configurable)

## 🔐 Security Notes

- No secrets required for public repository
- Artifacts are publicly downloadable
- Consider code signing for production releases

## 📚 Additional Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Inno Setup Documentation](https://jrsoftware.org/ishelp/)
- [.NET Publish Documentation](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish)

## ✅ Next Steps

1. ✅ **Done**: GitHub Actions is set up
2. ✅ **Done**: Installer script is configured
3. 🔄 **Next**: Wait for first build to complete
4. 📦 **Then**: Create your first release
5. 🎉 **Finally**: Share your application!

## 🎯 Quick Commands

```bash
# Check workflow status locally
git log --oneline -5

# Force trigger a build
git commit --allow-empty -m "Trigger build"
git push

# Download latest artifacts (requires GitHub CLI)
gh run download
```

---

**Your automated build pipeline is now active!** 🚀

Every push to `main` will automatically build and package your application.
