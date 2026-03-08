using System.Data.SQLite;
using TheToyRoomDesktop.Models;

namespace TheToyRoomDesktop.Services;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(string dbPath)
    {
        _connectionString = $"Data Source={dbPath};Version=3;";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        // Create Collectibles table
        var createCollectiblesTable = @"
            CREATE TABLE IF NOT EXISTS Collectibles (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Manufacturer TEXT NOT NULL,
                Character TEXT,
                OriginalPrice REAL NOT NULL,
                CurrentValue REAL NOT NULL,
                ImagePath TEXT,
                DateAdded TEXT NOT NULL,
                DateAcquired TEXT,
                Notes TEXT
            )";

        using var command = new SQLiteCommand(createCollectiblesTable, connection);
        command.ExecuteNonQuery();
        
        // Create Manufacturers table
        var createManufacturersTable = @"
            CREATE TABLE IF NOT EXISTS Manufacturers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE,
                DateAdded TEXT NOT NULL
            )";

        using var manufacturersCommand = new SQLiteCommand(createManufacturersTable, connection);
        manufacturersCommand.ExecuteNonQuery();
        
        // Add DateAcquired column if it doesn't exist (for existing databases)
        var alterTableQuery = @"
            ALTER TABLE Collectibles ADD COLUMN DateAcquired TEXT";
        
        try
        {
            using var alterCommand = new SQLiteCommand(alterTableQuery, connection);
            alterCommand.ExecuteNonQuery();
        }
        catch (SQLiteException)
        {
            // Column already exists, ignore
        }
        
        // Add Character column if it doesn't exist (for existing databases)
        var alterTableCharacterQuery = @"
            ALTER TABLE Collectibles ADD COLUMN Character TEXT";
        
        try
        {
            using var alterCharacterCommand = new SQLiteCommand(alterTableCharacterQuery, connection);
            alterCharacterCommand.ExecuteNonQuery();
        }
        catch (SQLiteException)
        {
            // Column already exists, ignore
        }

        // Create Decos table
        var createDecosTable = @"
            CREATE TABLE IF NOT EXISTS Decos (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE,
                DateAdded TEXT NOT NULL
            )";

        using var createDecosCommand = new SQLiteCommand(createDecosTable, connection);
        createDecosCommand.ExecuteNonQuery();

        // Add DecoId column if it doesn't exist
        var alterTableDecoIdQuery = @"
            ALTER TABLE Collectibles ADD COLUMN DecoId INTEGER";
        
        try
        {
            using var alterDecoIdCommand = new SQLiteCommand(alterTableDecoIdQuery, connection);
            alterDecoIdCommand.ExecuteNonQuery();
        }
        catch (SQLiteException)
        {
            // Column already exists, ignore
        }

        // Add Reissue column if it doesn't exist
        var alterTableReissueQuery = @"
            ALTER TABLE Collectibles ADD COLUMN Reissue INTEGER";
        
        try
        {
            using var alterReissueCommand = new SQLiteCommand(alterTableReissueQuery, connection);
            alterReissueCommand.ExecuteNonQuery();
        }
        catch (SQLiteException)
        {
            // Column already exists, ignore
        }

        // Add Stylized column if it doesn't exist
        var alterTableStylizedQuery = @"
            ALTER TABLE Collectibles ADD COLUMN Stylized INTEGER";
        
        try
        {
            using var alterStylizedCommand = new SQLiteCommand(alterTableStylizedQuery, connection);
            alterStylizedCommand.ExecuteNonQuery();
        }
        catch (SQLiteException)
        {
            // Column already exists, ignore
        }

        // Create Classifications table
        var createClassificationsTable = @"
            CREATE TABLE IF NOT EXISTS Classifications (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE,
                DateAdded TEXT NOT NULL
            )";

        using var createClassificationsCommand = new SQLiteCommand(createClassificationsTable, connection);
        createClassificationsCommand.ExecuteNonQuery();

        // Add ClassificationId column if it doesn't exist
        var alterTableClassificationIdQuery = @"
            ALTER TABLE Collectibles ADD COLUMN ClassificationId INTEGER";
        
        try
        {
            using var alterClassificationIdCommand = new SQLiteCommand(alterTableClassificationIdQuery, connection);
            alterClassificationIdCommand.ExecuteNonQuery();
        }
        catch (SQLiteException)
        {
            // Column already exists, ignore
        }
    }

    public SQLiteConnection GetConnection()
    {
        return new SQLiteConnection(_connectionString);
    }
}
