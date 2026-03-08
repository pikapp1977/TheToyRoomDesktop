using System.Data.SQLite;
using TheToyRoomDesktop.Models;

namespace TheToyRoomDesktop.Services;

public class ClassificationService
{
    private readonly DatabaseService _databaseService;

    public ClassificationService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<List<Classification>> GetAllClassificationsAsync()
    {
        var classifications = new List<Classification>();

        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        var query = "SELECT * FROM Classifications ORDER BY Name ASC";
        using var command = new SQLiteCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            classifications.Add(new Classification
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                DateAdded = DateTime.Parse(reader.GetString(2))
            });
        }

        return classifications;
    }

    public async Task<Classification?> GetClassificationByNameAsync(string name)
    {
        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        var query = "SELECT * FROM Classifications WHERE Name = @Name COLLATE NOCASE";
        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@Name", name);
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Classification
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                DateAdded = DateTime.Parse(reader.GetString(2))
            };
        }

        return null;
    }

    public async Task<int> AddClassificationAsync(string name)
    {
        var existing = await GetClassificationByNameAsync(name);
        if (existing != null)
        {
            return existing.Id;
        }

        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        var query = @"INSERT INTO Classifications (Name, DateAdded)
                     VALUES (@Name, @DateAdded);
                     SELECT last_insert_rowid();";

        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@Name", name.Trim());
        command.Parameters.AddWithValue("@DateAdded", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task UpdateClassificationAsync(Classification classification)
    {
        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        var query = @"UPDATE Classifications 
                     SET Name = @Name
                     WHERE Id = @Id";

        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@Id", classification.Id);
        command.Parameters.AddWithValue("@Name", classification.Name.Trim());

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteClassificationAsync(int id)
    {
        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        var query = "DELETE FROM Classifications WHERE Id = @Id";
        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        await command.ExecuteNonQueryAsync();
    }

    public async Task SeedDefaultClassificationsAsync()
    {
        var defaults = new[] { "Official", "Licensed", "3rd Party", "KO" };
        
        foreach (var name in defaults)
        {
            await AddClassificationAsync(name);
        }
    }
}
