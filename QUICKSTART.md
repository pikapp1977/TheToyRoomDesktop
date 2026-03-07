# TheToyRoomDesktop - Quick Start Guide

## 🚀 Getting Started in 5 Minutes

### Option 1: Push to GitHub (Recommended First Step)

**Windows Users:**
```cmd
# Double-click this file:
git-push.bat
```

**Linux/Mac/WSL Users:**
```bash
bash git-push.sh
```

This will automatically:
- Initialize git repository
- Add remote origin
- Commit all files
- Push to GitHub

### Option 2: Manual Git Setup

```bash
cd /mnt/c/users/admin/documents/TheToyRoomDesktop

# Initialize and push
git init
git add .
git commit -m "Initial commit: TheToyRoomDesktop WPF application"
git remote add origin https://github.com/pikapp1977/TheToyRoomDesktop.git
git branch -M main
git push -u origin main
```

## 🏗️ Building the Application

### Prerequisites
- .NET 8 SDK: https://dotnet.microsoft.com/download

### Build Commands

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

## 📦 Project Structure at a Glance

```
TheToyRoomDesktop/
├── 📱 App & Main Window (4 files)
├── 📊 Models (3 files) - Data structures
├── ⚙️ Services (5 files) - Business logic
├── 🎨 Views (14 files) - UI pages
└── 📖 Documentation (6 files)
```

**Total: 35 files**

## ✨ Features

✅ Dashboard with statistics  
✅ Collection management (CRUD)  
✅ Search and filter  
✅ Excel export  
✅ Excel import with template  
✅ Autocomplete fields  
✅ Image support  
✅ Reports by manufacturer  

## 🗄️ Database

**Location**: `%LOCALAPPDATA%\TheToyRoom\thetoyroom.db`

**Compatible with**: TheToyRoom Blazor web application

## 📚 Documentation Files

1. **README.md** - User documentation
2. **PROJECT_INFO.md** - Technical details
3. **GIT_SETUP.md** - Git workflow guide
4. **CREATION_SUMMARY.md** - What was created
5. **QUICKSTART.md** - This file
6. **LICENSE** - MIT License

## 🔧 Troubleshooting

### "dotnet command not found"
Install .NET 8 SDK from https://dotnet.microsoft.com/download

### "Git push failed"
You may need a Personal Access Token:
1. Go to GitHub Settings → Developer settings → Personal access tokens
2. Generate new token with 'repo' scope
3. Use token as password

### Build errors
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

## 🎯 Next Steps After Pushing to GitHub

1. Visit https://github.com/pikapp1977/TheToyRoomDesktop
2. Add repository description
3. Add topics: wpf, dotnet, csharp, sqlite, desktop-application
4. Test the application
5. Create a release when ready

## 📞 Quick Reference

```bash
# Clone (if starting fresh)
git clone https://github.com/pikapp1977/TheToyRoomDesktop.git

# Build and run
dotnet restore && dotnet build && dotnet run

# Daily git workflow
git pull
# ... make changes ...
git add .
git commit -m "Description"
git push
```

## 🎉 That's It!

You now have a complete Windows desktop application for managing collectibles!

**Need more details?** Check out:
- **README.md** for user guide
- **PROJECT_INFO.md** for technical documentation
- **GIT_SETUP.md** for Git workflow

Happy collecting! 🎮🤖🎭
