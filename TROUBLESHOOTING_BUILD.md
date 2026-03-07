# Build Troubleshooting Guide

## Current Build Status

The GitHub Actions workflow is being debugged. The improved workflow now includes:
- Detailed verbose output
- Project structure display
- Better error messages
- File size verification

## Common Build Issues

### 1. Missing XAML Files

**Symptom**: Build fails with "cannot find App.xaml" or similar

**Fix**: Ensure all XAML files are committed:
```bash
git add *.xaml Views/*.xaml
git commit -m "Add missing XAML files"
git push
```

### 2. Code-Behind Files Not Found

**Symptom**: Build fails referencing .xaml.cs files

**Fix**: Make sure all code-behind files are committed:
```bash
git add *.xaml.cs Views/*.xaml.cs
git commit -m "Add code-behind files"
git push
```

### 3. Package Restore Fails

**Symptom**: "Unable to restore NuGet packages"

**Fix**: Verify `.csproj` has correct package references and SDK version

### 4. PublishSingleFile Issues

**Symptom**: Published exe not created

**Possible solutions**:
- Remove `-p:PublishSingleFile=true` temporarily
- Check for incompatible packages
- Verify target framework is correct

## Debugging Locally

To test the build before pushing:

```powershell
# Clean previous builds
dotnet clean

# Restore packages
dotnet restore -v detailed

# Build Debug
dotnet build -c Debug -v detailed

# Publish Release
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -v detailed

# Check output
dir bin\Release\net8.0-windows\win-x64\publish
```

## Current Workflow Improvements

The workflow now:
1. ✅ Shows all project files before build
2. ✅ Uses verbose output for all dotnet commands
3. ✅ Builds Debug first to catch issues
4. ✅ Verifies each step before proceeding
5. ✅ Shows file sizes when successful
6. ✅ Better error messages with directory listings

## Viewing Build Logs

1. Go to [Actions tab](https://github.com/pikapp1977/TheToyRoomDesktop/actions)
2. Click on the failed workflow run
3. Click on "build" job
4. Expand each step to see detailed output
5. Look for red error messages

## Expected Successful Output

When working correctly, you should see:

```
✅ Checkout code
✅ Setup .NET
✅ Display project structure (lists all files)
✅ Restore dependencies
✅ Build project (Debug)
✅ Publish application
✅ Verify publish output (shows TheToyRoomDesktop.exe ~80MB)
✅ Download Inno Setup
✅ Verify Inno Setup
✅ Create installer (shows .exe ~80MB)
✅ Upload portable
✅ Upload installer
```

## Common Fixes

### Missing Files
```bash
git status
git add .
git commit -m "Add missing files"
git push
```

### Wrong Project Name
Check that workflow uses correct project name:
- File: `.github/workflows/build.yml`
- Line: `dotnet restore TheToyRoomDesktop.csproj`

### Path Issues
Verify paths in `TheToyRoomDesktopSetup.iss`:
```inno
Source: "bin\Release\net8.0-windows\win-x64\publish\TheToyRoomDesktop.exe"
```

## Next Steps

After the current build completes (or fails), check the logs and:

1. If it succeeds: ✅ Download artifacts and test
2. If it fails: Look for the first red error message
3. Update this guide with the solution

## Getting Help

If builds continue to fail:
1. Check the workflow logs (Actions tab)
2. Look at the first error message
3. Compare with working TimeTrackerWPF repository
4. Check that all files from local project are committed

## Quick Health Check

Run locally to verify project structure:

```powershell
# Should show .csproj
dir *.csproj

# Should show XAML files
dir *.xaml
dir Views\*.xaml

# Should show code files
dir *.cs
dir Models\*.cs
dir Services\*.cs
dir Views\*.cs

# Test restore
dotnet restore

# Test build
dotnet build
```

If any of these fail locally, fix them before pushing to GitHub.
