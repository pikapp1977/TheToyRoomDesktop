# TheToyRoomDesktop

A Windows desktop application for managing your collectible toy collection. Built with WPF and .NET 8.

## Features

- **Dashboard**: View collection statistics and recent items at a glance
- **My Collection**: Browse, search, and manage your collectibles
- **Add/Edit Items**: Add new items or edit existing ones with autocomplete for manufacturers and decos
- **Reports**: Filter by manufacturer and export to Excel
- **Importer**: Bulk import collectibles from Excel with validation
- **Settings**: View database information and application details

## System Requirements

- Windows 10 or later
- .NET 8 Runtime (or SDK for development)

## Installation

### Using Pre-built Release
1. Download the latest release
2. Extract the ZIP file
3. Run `TheToyRoomDesktop.exe`

### Building from Source
1. Install .NET 8 SDK from https://dotnet.microsoft.com/download
2. Clone or download this repository
3. Open command prompt in the project directory
4. Run:
   ```
   dotnet restore
   dotnet build
   dotnet run
   ```

## Database

The application uses SQLite for data storage. The database file is located at:
```
%LOCALAPPDATA%\TheToyRoom\thetoyroom.db
```

### Database Schema

**Collectibles Table:**
- Id (Primary Key)
- Name (Required)
- Manufacturer (Required)
- Character
- DecoId (Foreign Key to Decos)
- Reissue (Boolean)
- Stylized (Boolean)
- OriginalPrice
- CurrentValue
- ImagePath
- DateAdded
- DateAcquired
- Notes

**Manufacturers Table:**
- Id (Primary Key)
- Name (Unique)
- DateAdded

**Decos Table:**
- Id (Primary Key)
- Name (Unique)
- DateAdded

## Features in Detail

### Search and Filter
- Real-time search across Name, Character, Manufacturer, and Deco fields
- Filter by manufacturer in Reports view
- Case-insensitive searching

### Excel Import/Export
- Export your collection or filtered results to Excel
- Download import template with proper headers
- Bulk import with validation
- Automatic duplicate detection
- Only Name and Manufacturer are required fields

### Image Support
- Store image paths for your collectibles
- Preview images in the View Item dialog
- Images are stored on your local file system

### Autocomplete
- Manufacturer and Deco fields feature autocomplete
- Type to filter existing entries
- Automatically creates new entries when needed

## Usage Tips

1. **Adding Items**: Click "Add Item" from the menu and fill in the required fields (Name and Manufacturer)
2. **Searching**: Use the search box on the Collection page to quickly find items
3. **Editing**: Double-click an item in the Collection view to see details, or click Edit to modify
4. **Importing**: Download the template, fill it out in Excel, then upload and import
5. **Exporting**: Use the Reports page to filter by manufacturer and export to Excel

## Data Management

### Backup
To backup your collection, copy the database file from:
```
%LOCALAPPDATA%\TheToyRoom\thetoyroom.db
```

### Restore
To restore from a backup, replace the database file with your backup copy.

## Technologies Used

- **Framework**: WPF (Windows Presentation Foundation)
- **.NET**: .NET 8
- **Database**: SQLite with System.Data.SQLite.Core
- **Excel**: DocumentFormat.OpenXml for reading/writing Excel files

## Project Structure

```
TheToyRoomDesktop/
├── Models/              # Data models (Collectible, Manufacturer, Deco)
├── Services/            # Business logic and data access
│   ├── DatabaseService.cs
│   ├── CollectibleService.cs
│   ├── ManufacturerService.cs
│   ├── DecoService.cs
│   └── ExportService.cs
├── Views/               # WPF Pages and Windows
│   ├── HomePage.xaml
│   ├── CollectionPage.xaml
│   ├── AddItemPage.xaml
│   ├── ReportsPage.xaml
│   ├── ImporterPage.xaml
│   ├── SettingsPage.xaml
│   └── ViewItemWindow.xaml
├── App.xaml            # Application resources and startup
└── MainWindow.xaml     # Main application window with navigation

```

## Compatibility with TheToyRoom Web Application

This desktop application uses the same database schema as the TheToyRoom Blazor web application. You can share the same database file between both applications if needed.

## License

This project is provided as-is for personal use.

## Support

For issues or questions, please refer to the project documentation or create an issue in the repository.
