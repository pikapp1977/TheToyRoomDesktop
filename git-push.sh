#!/bin/bash

# TheToyRoomDesktop - Git Push Script
# This script helps you push the project to GitHub

echo "=================================="
echo "TheToyRoomDesktop - Git Setup"
echo "=================================="
echo ""

# Check if git is installed
if ! command -v git &> /dev/null; then
    echo "❌ Git is not installed. Please install Git first."
    echo "   Download from: https://git-scm.com/download/win"
    exit 1
fi

echo "✅ Git is installed"
echo ""

# Check if we're in the right directory
if [ ! -f "TheToyRoomDesktop.csproj" ]; then
    echo "❌ Error: Not in TheToyRoomDesktop directory"
    echo "   Please run this script from the project root"
    exit 1
fi

echo "✅ In correct directory"
echo ""

# Check if git repo is initialized
if [ ! -d ".git" ]; then
    echo "📦 Initializing Git repository..."
    git init
    echo "✅ Git repository initialized"
    echo ""
fi

# Check if remote is set
if ! git remote | grep -q "origin"; then
    echo "🔗 Adding remote repository..."
    git remote add origin https://github.com/pikapp1977/TheToyRoomDesktop.git
    echo "✅ Remote repository added"
    echo ""
else
    echo "✅ Remote repository already configured"
    echo ""
fi

# Show git status
echo "📋 Current Git Status:"
echo "---------------------"
git status
echo ""

# Ask if user wants to continue
read -p "Do you want to commit and push all files? (y/n) " -n 1 -r
echo ""

if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "❌ Aborted by user"
    exit 0
fi

# Stage all files
echo ""
echo "📦 Staging all files..."
git add .
echo "✅ Files staged"
echo ""

# Commit
echo "💾 Creating commit..."
git commit -m "Initial commit: TheToyRoomDesktop WPF application

- Complete WPF desktop application
- Replicates all TheToyRoom web app functionality
- Models: Collectible, Manufacturer, Deco
- Services: Database, Collectible, Manufacturer, Deco, Export
- Views: Home, Collection, AddItem, Reports, Importer, Settings
- Full documentation included
- .NET 8 with SQLite and OpenXml"

if [ $? -eq 0 ]; then
    echo "✅ Commit created"
    echo ""
else
    echo "❌ Commit failed"
    exit 1
fi

# Set main branch
echo "🌿 Setting main branch..."
git branch -M main
echo "✅ Main branch set"
echo ""

# Push to GitHub
echo "🚀 Pushing to GitHub..."
echo "   (You may be prompted for credentials)"
echo ""

git push -u origin main

if [ $? -eq 0 ]; then
    echo ""
    echo "=================================="
    echo "✅ SUCCESS!"
    echo "=================================="
    echo ""
    echo "Your code has been pushed to:"
    echo "https://github.com/pikapp1977/TheToyRoomDesktop"
    echo ""
    echo "Next steps:"
    echo "1. Visit the repository on GitHub"
    echo "2. Add a description and topics"
    echo "3. Verify all files are present"
    echo "4. Test the application"
    echo ""
else
    echo ""
    echo "❌ Push failed"
    echo ""
    echo "Common issues:"
    echo "1. Authentication - You may need a Personal Access Token"
    echo "2. Repository may already have content"
    echo ""
    echo "If repository has content, try:"
    echo "  git pull origin main --allow-unrelated-histories"
    echo "  git push -u origin main"
    echo ""
fi
