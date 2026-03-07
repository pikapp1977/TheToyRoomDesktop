# TheToyRoomDesktop - Project Information

## Overview

TheToyRoomDesktop is a Windows desktop application that replicates all functionality from the TheToyRoom Blazor Server web application. It provides a native Windows experience for managing collectible toy collections.

## Architecture

### Technology Stack
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Platform**: .NET 8.0 Windows
- **Database**: SQLite via System.Data.SQLite.Core
- **Excel Processing**: DocumentFormat.OpenXml 3.0.1
- **Design Pattern**: MVVM-inspired with Services layer

### Project Structure

```
TheToyRoomDesktop/
│
├── Models/                          # Data models
│   ├── Collectible.cs              # Main collectible entity
│   ├── Manufacturer.cs             # Manufacturer lookup entity
│   └── Deco.cs                     # Deco/decoration lookup entity
│
├── Services/                        # Business logic layer
│   ├── DatabaseService.cs          # SQLite connection and schema management
│   ├── CollectibleService.cs       # CRUD operations for collectibles
│   ├── ManufacturerService.cs      # Manufacturer management
│   ├── DecoService.cs              # Deco management
│   └── ExportService.cs            # Excel import/export functionality
│
├── Views/                           # WPF pages and windows
│   ├── HomePage.xaml/cs            # Dashboard with statistics
│   ├── CollectionPage.xaml/cs      # Main collection list with search
│   ├── AddItemPage.xaml/cs         # Add/Edit form
│   ├── ReportsPage.xaml/cs         # Reports with Excel export
│   ├── ImporterPage.xaml/cs        # Bulk import from Excel
│   ├── SettingsPage.xaml/cs        # Settings and database info
│   └── ViewItemWindow.xaml/cs      # Item details modal
│
├── App.xaml/cs                      # Application startup and resources
├── MainWindow.xaml/cs               # Main window with navigation
└── TheToyRoomDesktop.csproj         # Project configuration

```

## Database Schema

The application uses the exact same SQLite schema as the web application:

### Tables

#### Collectibles
- **Id**: INTEGER PRIMARY KEY AUTOINCREMENT
- **Name**: TEXT NOT NULL
- **Manufacturer**: TEXT NOT NULL
- **Character**: TEXT (optional)
- **DecoId**: INTEGER (FK to Decos)
- **Reissue**: INTEGER (0/1 boolean)
- **Stylized**: INTEGER (0/1 boolean)
- **OriginalPrice**: REAL
- **CurrentValue**: REAL
- **ImagePath**: TEXT (optional)
- **DateAdded**: TEXT (ISO format)
- **DateAcquired**: TEXT (ISO format, optional)
- **Notes**: TEXT (optional)

#### Manufacturers
- **Id**: INTEGER PRIMARY KEY AUTOINCREMENT
- **Name**: TEXT UNIQUE NOT NULL
- **DateAdded**: TEXT (ISO format)

#### Decos
- **Id**: INTEGER PRIMARY KEY AUTOINCREMENT
- **Name**: TEXT UNIQUE NOT NULL
- **DateAdded**: TEXT (ISO format)

## Key Features Implemented

### 1. Dashboard (HomePage)
- Total items count
- Total original value
- Total current value
- Total gain/loss (color-coded)
- Recent 10 items grid

### 2. My Collection (CollectionPage)
- Full collection grid with sortable columns
- Real-time search (Name, Character, Manufacturer, Deco)
- View/Edit/Delete actions per item
- Double-click to view details
- Refresh functionality

### 3. Add/Edit Item (AddItemPage)
- Form validation (Name and Manufacturer required)
- Autocomplete for Manufacturer and Deco
- Reissue/Stylized checkboxes
- Price fields with decimal validation
- Date picker for Date Acquired
- Image file browser with preview
- Notes text area
- Edit mode when passed existing item

### 4. View Item (ViewItemWindow)
- Modal dialog showing all item details
- Image display (if available)
- Formatted fields
- Close button

### 5. Reports (ReportsPage)
- Filter by manufacturer dropdown
- Clear filter button
- Export to Excel button
- Statistics summary (items, original, current, gain/loss)
- Filtered grid with gain/loss column

### 6. Importer (ImporterPage)
- Download Excel template
- File picker for upload
- Parse and validate Excel file
- Preview grid with checkboxes
- Select All / Deselect All
- Validation feedback for skipped rows
- Duplicate detection
- Bulk import selected items

### 7. Settings (SettingsPage)
- Database path display
- Database file size
- Total items count
- Manufacturers count
- Decos count
- Application version info
- Refresh button

## Service Layer Details

### DatabaseService
- Initializes SQLite connection
- Creates tables if not exist
- Handles schema migrations (ALTER TABLE for new columns)
- Provides connection factory method

### CollectibleService
- GetAllCollectiblesAsync(): Returns all collectibles with JOIN to Decos
- AddCollectibleAsync(): Inserts new collectible
- UpdateCollectibleAsync(): Updates existing collectible
- DeleteCollectibleAsync(): Deletes collectible and associated image
- GetTotalOriginalValueAsync(): Calculates sum of original prices
- GetTotalCurrentValueAsync(): Calculates sum of current values

### ManufacturerService
- GetAllManufacturersAsync(): Returns all manufacturers sorted by name
- GetManufacturerByNameAsync(): Case-insensitive lookup
- AddManufacturerAsync(): Creates or returns existing (duplicate prevention)
- DeleteManufacturerAsync(): Removes manufacturer

### DecoService
- GetAllDecosAsync(): Returns all decos sorted by name
- GetDecoByNameAsync(): Case-insensitive lookup
- AddDecoAsync(): Creates or returns existing (duplicate prevention)
- DeleteDecoAsync(): Removes deco

### ExportService
- ExportCollectionToExcel(): Creates XLSX with collection data
- GenerateImportTemplate(): Creates XLSX template with headers and example
- ParseImportFile(): Reads XLSX and validates rows
- ImportResult class: Contains parsed items and skipped row messages

## UI/UX Design

### Color Scheme (Purple Theme)
- Primary: #6f42c1 (purple)
- Primary Dark: #5a32a3
- Secondary: #e9ecef (light gray)
- Background: #f8f9fa
- Success: #28a745 (green)
- Danger: #dc3545 (red)
- Info: #007bff (blue)

### Navigation
- Top menu bar with 6 main sections
- Frame-based navigation for seamless page transitions
- Consistent header and styling across pages

### Responsive Elements
- Scrollable content areas
- DataGrid with horizontal scrolling when needed
- Form layouts optimized for desktop

## Data Flow

1. **Application Startup**:
   - App.xaml.cs initializes all services
   - Database connection established
   - Schema migrations run if needed
   - MainWindow loads with HomePage

2. **Service Access**:
   - All services available via static App properties
   - Services inject DatabaseService for connections
   - Async/await pattern throughout

3. **Page Navigation**:
   - Menu clicks navigate Frame to new pages
   - Pages can navigate programmatically via NavigationService
   - State preserved during navigation

## Compatibility

### Database Compatibility
- 100% compatible with TheToyRoom Blazor web app database
- Can share the same .db file between applications
- Schema migrations are backward compatible

### Excel Format
- Uses OpenXML for modern .xlsx format
- Template and export have identical structure
- Import validates required fields (Name, Manufacturer)

## Building and Deployment

### Development Build
```bash
dotnet restore
dotnet build
dotnet run
```

### Release Build
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

### Output
- Executable: TheToyRoomDesktop.exe
- Database: %LOCALAPPDATA%\TheToyRoom\thetoyroom.db
- No web server required
- Runs entirely offline

## Testing Checklist

- [ ] Dashboard loads and displays statistics
- [ ] Collection page shows all items
- [ ] Search filters results correctly
- [ ] Add new item with all fields
- [ ] Edit existing item
- [ ] Delete item with confirmation
- [ ] View item in modal
- [ ] Manufacturer autocomplete works
- [ ] Deco autocomplete works
- [ ] Image selection and preview
- [ ] Export to Excel from Reports
- [ ] Filter by manufacturer
- [ ] Download import template
- [ ] Parse Excel file
- [ ] Import selected items
- [ ] Duplicate detection
- [ ] Settings displays database info

## Known Limitations

1. Image files are stored by path reference, not embedded
2. No network/cloud sync capabilities
3. Single-user desktop application (no concurrent access)
4. Windows-only (WPF limitation)

## Future Enhancements (Ideas)

- Drag-and-drop image upload
- Bulk edit functionality
- Advanced filtering options
- Chart/graph visualizations
- Barcode scanner integration
- Cloud backup integration
- Print collection list
- Custom themes

## Differences from Web Version

### Advantages
- Native Windows performance
- No web browser required
- Better file system integration
- More responsive UI
- Offline-first by design

### Disadvantages
- Windows-only (web is cross-platform)
- No remote access
- Larger file size
- Requires .NET runtime

## Development Notes

- All async operations use proper async/await
- Proper error handling with MessageBox dialogs
- Case-insensitive database queries with COLLATE NOCASE
- Services initialized once at startup via dependency injection pattern
- XAML resources define consistent styling
- Frame navigation for clean page transitions
