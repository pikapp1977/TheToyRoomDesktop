# Git Repository Setup Guide

## Repository Information

**Repository URL**: https://github.com/pikapp1977/TheToyRoomDesktop.git

## Initial Setup

If you haven't already initialized the git repository locally, follow these steps:

### 1. Initialize Git Repository

```bash
cd /mnt/c/users/admin/documents/TheToyRoomDesktop
git init
```

### 2. Add Remote Repository

```bash
git remote add origin https://github.com/pikapp1977/TheToyRoomDesktop.git
```

### 3. Configure Git (if not already configured)

```bash
git config user.name "Your Name"
git config user.email "your.email@example.com"
```

## Committing and Pushing Code

### 1. Check Status

```bash
git status
```

This will show all files that are untracked or modified.

### 2. Add All Files

```bash
git add .
```

Or add specific files:

```bash
git add README.md
git add TheToyRoomDesktop.csproj
git add Models/
git add Services/
# etc.
```

### 3. Commit Changes

```bash
git commit -m "Initial commit: TheToyRoomDesktop WPF application"
```

### 4. Set Default Branch (if needed)

```bash
git branch -M main
```

### 5. Push to GitHub

```bash
git push -u origin main
```

If the repository already has content, you may need to pull first:

```bash
git pull origin main --allow-unrelated-histories
git push -u origin main
```

## Daily Workflow

### Making Changes

1. **Check current status**:
   ```bash
   git status
   ```

2. **Pull latest changes** (if working with others):
   ```bash
   git pull
   ```

3. **Make your changes** to files

4. **Stage changes**:
   ```bash
   git add .
   ```

5. **Commit with descriptive message**:
   ```bash
   git commit -m "Add feature: Excel import functionality"
   ```

6. **Push to GitHub**:
   ```bash
   git push
   ```

## Useful Git Commands

### Viewing History

```bash
# View commit history
git log

# View compact history
git log --oneline

# View file changes
git diff
```

### Branch Management

```bash
# Create new branch
git branch feature-name

# Switch to branch
git checkout feature-name

# Create and switch to new branch
git checkout -b feature-name

# List all branches
git branch -a

# Merge branch into main
git checkout main
git merge feature-name

# Delete branch
git branch -d feature-name
```

### Undoing Changes

```bash
# Discard changes in working directory
git checkout -- filename

# Unstage file
git reset HEAD filename

# Undo last commit (keep changes)
git reset --soft HEAD~1

# Undo last commit (discard changes)
git reset --hard HEAD~1
```

## Recommended Commit Message Format

Use clear, descriptive commit messages:

```
✅ Good examples:
- "Add Excel import functionality to ImporterPage"
- "Fix: Resolve null reference in CollectionService"
- "Update README with installation instructions"
- "Refactor: Simplify database connection logic"

❌ Bad examples:
- "update"
- "fix bug"
- "changes"
```

## Suggested First Commits

Here's a recommended structure for your initial commits:

### Commit 1: Project Structure
```bash
git add TheToyRoomDesktop.csproj .gitignore
git commit -m "Initial project setup: WPF .NET 8 application with dependencies"
```

### Commit 2: Models
```bash
git add Models/
git commit -m "Add data models: Collectible, Manufacturer, Deco"
```

### Commit 3: Services
```bash
git add Services/
git commit -m "Add service layer: Database, Collectible, Manufacturer, Deco, Export services"
```

### Commit 4: Core UI
```bash
git add App.xaml App.xaml.cs MainWindow.xaml MainWindow.xaml.cs
git commit -m "Add core application UI: App and MainWindow with navigation"
```

### Commit 5: Views
```bash
git add Views/
git commit -m "Add all view pages: Home, Collection, AddItem, Reports, Importer, Settings, ViewItem"
```

### Commit 6: Documentation
```bash
git add README.md PROJECT_INFO.md GIT_SETUP.md
git commit -m "Add comprehensive documentation and setup guides"
```

### Push Everything
```bash
git push -u origin main
```

## Repository Structure on GitHub

After pushing, your repository should look like this:

```
TheToyRoomDesktop/
├── .gitignore
├── README.md
├── PROJECT_INFO.md
├── GIT_SETUP.md
├── TheToyRoomDesktop.csproj
├── App.xaml
├── App.xaml.cs
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── Models/
│   ├── Collectible.cs
│   ├── Manufacturer.cs
│   └── Deco.cs
├── Services/
│   ├── DatabaseService.cs
│   ├── CollectibleService.cs
│   ├── ManufacturerService.cs
│   ├── DecoService.cs
│   └── ExportService.cs
└── Views/
    ├── HomePage.xaml
    ├── HomePage.xaml.cs
    ├── CollectionPage.xaml
    ├── CollectionPage.xaml.cs
    ├── AddItemPage.xaml
    ├── AddItemPage.xaml.cs
    ├── ReportsPage.xaml
    ├── ReportsPage.xaml.cs
    ├── ImporterPage.xaml
    ├── ImporterPage.xaml.cs
    ├── SettingsPage.xaml
    ├── SettingsPage.xaml.cs
    ├── ViewItemWindow.xaml
    └── ViewItemWindow.xaml.cs
```

## GitHub Features to Enable

Once your code is pushed, consider enabling these GitHub features:

### 1. **Add Topics** (tags)
   - wpf
   - dotnet
   - csharp
   - desktop-application
   - sqlite
   - collection-manager
   - inventory-management

### 2. **Edit Repository Description**
   Example: "A Windows desktop application for managing collectible toy collections. Built with WPF, .NET 8, and SQLite."

### 3. **Add License** (if desired)
   Choose from: MIT, Apache 2.0, GPL, etc.

### 4. **Create Releases**
   When you have a stable version, create a release with compiled binaries

### 5. **Enable Issues**
   Track bugs and feature requests

### 6. **Create Wiki** (optional)
   For extended documentation

## Syncing with Original Web App

If you want to track changes from the original TheToyRoom web application:

```bash
# Add the web app as a second remote
git remote add webapp https://github.com/yourusername/TheToyRoom.git

# View remotes
git remote -v

# Fetch from web app (to see changes)
git fetch webapp
```

## Troubleshooting

### Problem: "fatal: remote origin already exists"
**Solution**:
```bash
git remote remove origin
git remote add origin https://github.com/pikapp1977/TheToyRoomDesktop.git
```

### Problem: Authentication issues
**Solution**: Use a personal access token instead of password
1. Go to GitHub Settings → Developer settings → Personal access tokens
2. Generate new token with 'repo' scope
3. Use token as password when prompted

### Problem: "fatal: refusing to merge unrelated histories"
**Solution**:
```bash
git pull origin main --allow-unrelated-histories
```

### Problem: Large files or binaries accidentally committed
**Solution**:
```bash
# Remove file from git but keep locally
git rm --cached filename

# Update .gitignore
echo "filename" >> .gitignore

# Commit changes
git commit -m "Remove accidentally committed file"
git push
```

## Best Practices

1. **Commit often**: Small, focused commits are better than large ones
2. **Write clear messages**: Describe what and why, not how
3. **Pull before push**: Always pull latest changes before pushing
4. **Use branches**: Keep main branch stable, use feature branches
5. **Review changes**: Use `git diff` before committing
6. **Don't commit secrets**: Never commit passwords, API keys, etc.
7. **Keep .gitignore updated**: Exclude build artifacts and local files

## Quick Reference

```bash
# Clone repository (if starting fresh)
git clone https://github.com/pikapp1977/TheToyRoomDesktop.git

# Create and push to repository (from existing code)
git init
git add .
git commit -m "Initial commit"
git remote add origin https://github.com/pikapp1977/TheToyRoomDesktop.git
git branch -M main
git push -u origin main

# Daily workflow
git pull                           # Get latest changes
# ... make changes ...
git add .                          # Stage all changes
git commit -m "Description"        # Commit with message
git push                           # Push to GitHub

# Check status at any time
git status
```

## Next Steps

After pushing your code:

1. ✅ Verify all files are on GitHub
2. ✅ Add a description and topics to your repository
3. ✅ Create a release when ready
4. ✅ Share the repository URL with collaborators
5. ✅ Set up branch protection rules (optional)
6. ✅ Enable GitHub Actions for CI/CD (optional)

Your repository is now set up and ready for collaborative development!
