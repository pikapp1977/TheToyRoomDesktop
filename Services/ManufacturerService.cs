using System.Data.SQLite;
using TheToyRoomDesktop.Models;

namespace TheToyRoomDesktop.Services;

public class ManufacturerService
{
    private readonly DatabaseService _databaseService;

    public ManufacturerService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<List<Manufacturer>> GetAllManufacturersAsync()
    {
        var manufacturers = new List<Manufacturer>();

        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        var query = "SELECT * FROM Manufacturers ORDER BY Name ASC";
        using var command = new SQLiteCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            manufacturers.Add(new Manufacturer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                DateAdded = DateTime.Parse(reader.GetString(2))
            });
        }

        return manufacturers;
    }

    public async Task<Manufacturer?> GetManufacturerByNameAsync(string name)
    {
        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        var query = "SELECT * FROM Manufacturers WHERE Name = @Name COLLATE NOCASE";
        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@Name", name);
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Manufacturer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                DateAdded = DateTime.Parse(reader.GetString(2))
            };
        }

        return null;
    }

    public async Task<int> AddManufacturerAsync(string name)
    {
        // Check if manufacturer already exists
        var existing = await GetManufacturerByNameAsync(name);
        if (existing != null)
        {
            return existing.Id;
        }

        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        var query = @"INSERT INTO Manufacturers (Name, DateAdded)
                     VALUES (@Name, @DateAdded);
                     SELECT last_insert_rowid();";

        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@Name", name.Trim());
        command.Parameters.AddWithValue("@DateAdded", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task DeleteManufacturerAsync(int id)
    {
        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        var query = "DELETE FROM Manufacturers WHERE Id = @Id";
        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        await command.ExecuteNonQueryAsync();
    }
}

    public async Task UpdateManufacturerAsync(Manufacturer manufacturer)
    {
        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        var query = @"UPDATE Manufacturers 
                     SET Name = @Name
                     WHERE Id = @Id";

        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@Id", manufacturer.Id);
        command.Parameters.AddWithValue("@Name", manufacturer.Name.Trim());

        await command.ExecuteNonQueryAsync();
    }
