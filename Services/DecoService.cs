using System.Data.SQLite;
using TheToyRoomDesktop.Models;

namespace TheToyRoomDesktop.Services;

public class DecoService
{
    private readonly DatabaseService _databaseService;

    public DecoService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<List<Deco>> GetAllDecosAsync()
    {
        var decos = new List<Deco>();

        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        var query = "SELECT * FROM Decos ORDER BY Name ASC";
        using var command = new SQLiteCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            decos.Add(new Deco
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                DateAdded = DateTime.Parse(reader.GetString(2))
            });
        }

        return decos;
    }

    public async Task<Deco?> GetDecoByNameAsync(string name)
    {
        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        var query = "SELECT * FROM Decos WHERE Name = @Name COLLATE NOCASE";
        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@Name", name);
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Deco
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                DateAdded = DateTime.Parse(reader.GetString(2))
            };
        }

        return null;
    }

    public async Task<int> AddDecoAsync(string name)
    {
        var existing = await GetDecoByNameAsync(name);
        if (existing != null)
        {
            return existing.Id;
        }

        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        var query = @"INSERT INTO Decos (Name, DateAdded)
                     VALUES (@Name, @DateAdded);
                     SELECT last_insert_rowid();";

        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@Name", name.Trim());
        command.Parameters.AddWithValue("@DateAdded", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task DeleteDecoAsync(int id)
    {
        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        var query = "DELETE FROM Decos WHERE Id = @Id";
        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        await command.ExecuteNonQueryAsync();
    }
}
