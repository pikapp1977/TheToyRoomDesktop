# GitHub Actions Workflow

## Build WPF Installer

This workflow automatically builds the TheToyRoomDesktop application and creates an installer using Inno Setup.

### What It Does

1. **Restores Dependencies** - Restores all NuGet packages
2. **Publishes Application** - Creates a self-contained executable for Windows x64
3. **Creates Installer** - Uses Inno Setup to create a Windows installer
4. **Uploads Artifacts** - Makes both the portable .exe and installer available for download

### When It Runs

- **On Push** - Whenever code is pushed to the `main` branch
- **On Pull Request** - When a PR is created targeting `main`
- **Manual Trigger** - Can be run manually from the Actions tab

### Outputs

After the workflow completes, you can download:

1. **TheToyRoomDesktop-Portable** - Standalone executable
   - File: `TheToyRoomDesktop.exe`
   - No installation required
   - Self-contained with all dependencies

2. **TheToyRoomDesktop-Installer** - Windows installer
   - File: `TheToyRoomDesktopSetup_v1.0.0.exe`
   - Installs to Program Files
   - Creates Start Menu shortcuts
   - Option for Desktop shortcut
   - Includes uninstaller

### How to Download Artifacts

1. Go to the [Actions tab](https://github.com/pikapp1977/TheToyRoomDesktop/actions)
2. Click on the most recent workflow run
3. Scroll down to the "Artifacts" section
4. Download either the portable version or installer

### Inno Setup Configuration

The installer is configured via `TheToyRoomDesktopSetup.iss`:

- **App Name**: The Toy Room Desktop
- **Version**: 1.0.0 (update this in the .iss file)
- **Install Location**: `C:\Program Files\TheToyRoomDesktop`
- **Requirements**: Windows 10 or later
- **Uninstaller**: Automatically created

### Updating Version Number

To release a new version:

1. Update the version in `TheToyRoomDesktopSetup.iss`:
   ```
   #define MyAppVersion "1.0.1"
   ```

2. Commit and push:
   ```bash
   git add TheToyRoomDesktopSetup.iss
   git commit -m "Bump version to 1.0.1"
   git push
   ```

3. The workflow will automatically build the new version

### Creating a Release

After the workflow completes successfully:

1. Go to **Releases** → **Create a new release**
2. Create a new tag (e.g., `v1.0.0`)
3. Download the artifacts from the workflow run
4. Attach both files to the release:
   - `TheToyRoomDesktop.exe` (portable)
   - `TheToyRoomDesktopSetup_v1.0.0.exe` (installer)
5. Write release notes
6. Publish release

### Troubleshooting

**Build fails:**
- Check that all project files are committed
- Verify `.csproj` file is correct
- Check workflow logs for specific errors

**Installer creation fails:**
- Verify `TheToyRoomDesktopSetup.iss` paths are correct
- Check that publish output exists
- Review Inno Setup compiler output in logs

**Artifacts not found:**
- Ensure workflow completed successfully (green checkmark)
- Check that artifacts section appears at bottom of workflow run
- Artifacts expire after 90 days by default

### Manual Local Build

To build locally without GitHub Actions:

```bash
# Restore and publish
dotnet restore
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# Install Inno Setup from https://jrsoftware.org/isdl.php
# Then compile the installer:
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" TheToyRoomDesktopSetup.iss
```

### Workflow File Location

`.github/workflows/build.yml`

This file must be in your repository for GitHub Actions to discover and run it.
