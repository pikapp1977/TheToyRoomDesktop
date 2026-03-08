using System.IO;
using System.Data.SQLite;
using TheToyRoomDesktop.Models;

namespace TheToyRoomDesktop.Services;

public class CollectibleService
{
    private readonly DatabaseService _databaseService;

    public CollectibleService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<List<Collectible>> GetAllCollectiblesAsync()
    {
        var collectibles = new List<Collectible>();

        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        var query = @"SELECT c.Id, c.Name, c.Manufacturer, c.Character, c.DecoId, c.Reissue, c.Stylized, 
                            c.OriginalPrice, c.CurrentValue, c.ImagePath, c.DateAdded, c.DateAcquired, c.Notes,
                            c.ClassificationId,
                            d.Name as DecoName,
                            cl.Name as ClassificationName
                     FROM Collectibles c 
                     LEFT JOIN Decos d ON c.DecoId = d.Id
                     LEFT JOIN Classifications cl ON c.ClassificationId = cl.Id 
                     ORDER BY c.DateAdded DESC";
        using var command = new SQLiteCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            // Read by column name instead of index to avoid order issues
            var id = reader.GetInt32(reader.GetOrdinal("Id"));
            var name = reader.GetString(reader.GetOrdinal("Name"));
            var manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"));
            
            var characterOrdinal = reader.GetOrdinal("Character");
            var character = reader.IsDBNull(characterOrdinal) ? null : reader.GetString(characterOrdinal);
            
            var originalPrice = reader.GetDecimal(reader.GetOrdinal("OriginalPrice"));
            var currentValue = reader.GetDecimal(reader.GetOrdinal("CurrentValue"));
            
            var imagePathOrdinal = reader.GetOrdinal("ImagePath");
            var imagePath = reader.IsDBNull(imagePathOrdinal) ? null : reader.GetString(imagePathOrdinal);
            
            // Parse DateAdded
            var dateAddedStr = reader.GetString(reader.GetOrdinal("DateAdded"));
            DateTime dateAdded = DateTime.Now;
            if (!string.IsNullOrEmpty(dateAddedStr))
            {
                if (!DateTime.TryParse(dateAddedStr, out dateAdded))
                {
                    dateAdded = DateTime.Now;
                }
            }
            
            // Parse DateAcquired
            var dateAcquiredOrdinal = reader.GetOrdinal("DateAcquired");
            DateTime? dateAcquired = null;
            if (!reader.IsDBNull(dateAcquiredOrdinal))
            {
                var dateAcquiredStr = reader.GetString(dateAcquiredOrdinal);
                if (!string.IsNullOrEmpty(dateAcquiredStr))
                {
                    if (DateTime.TryParse(dateAcquiredStr, out var parsedAcquired))
                    {
                        dateAcquired = parsedAcquired;
                    }
                }
            }
            
            // Read Notes
            var notesOrdinal = reader.GetOrdinal("Notes");
            var notes = reader.IsDBNull(notesOrdinal) ? null : reader.GetString(notesOrdinal);

            // Read DecoId
            var decoIdOrdinal = reader.GetOrdinal("DecoId");
            int? decoId = reader.IsDBNull(decoIdOrdinal) ? null : reader.GetInt32(decoIdOrdinal);

            // Read DecoName
            var decoNameOrdinal = reader.GetOrdinal("DecoName");
            string? decoName = reader.IsDBNull(decoNameOrdinal) ? null : reader.GetString(decoNameOrdinal);

            // Read Reissue
            var reissueOrdinal = reader.GetOrdinal("Reissue");
            bool reissue = reader.IsDBNull(reissueOrdinal) ? false : reader.GetInt32(reissueOrdinal) == 1;

            // Read Stylized
            var stylizedOrdinal = reader.GetOrdinal("Stylized");
            bool stylized = reader.IsDBNull(stylizedOrdinal) ? false : reader.GetInt32(stylizedOrdinal) == 1;

            // Read ClassificationId
            var classificationIdOrdinal = reader.GetOrdinal("ClassificationId");
            int? classificationId = reader.IsDBNull(classificationIdOrdinal) ? null : reader.GetInt32(classificationIdOrdinal);

            // Read ClassificationName
            var classificationNameOrdinal = reader.GetOrdinal("ClassificationName");
            string? classificationName = reader.IsDBNull(classificationNameOrdinal) ? null : reader.GetString(classificationNameOrdinal);
            
            collectibles.Add(new Collectible
            {
                Id = id,
                Name = name,
                Manufacturer = manufacturer,
                Character = character,
                DecoId = decoId,
                DecoName = decoName,
                ClassificationId = classificationId,
                ClassificationName = classificationName,
                Reissue = reissue,
                Stylized = stylized,
                OriginalPrice = originalPrice,
                CurrentValue = currentValue,
                ImagePath = imagePath,
                DateAdded = dateAdded,
                DateAcquired = dateAcquired,
                Notes = notes
            });
        }

        return collectibles;
    }

    public async Task AddCollectibleAsync(Collectible collectible)
    {
        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        var query = @"INSERT INTO Collectibles (Name, Manufacturer, Character, DecoId, ClassificationId, Reissue, Stylized, OriginalPrice, CurrentValue, ImagePath, DateAdded, DateAcquired, Notes)
                     VALUES (@Name, @Manufacturer, @Character, @DecoId, @ClassificationId, @Reissue, @Stylized, @OriginalPrice, @CurrentValue, @ImagePath, @DateAdded, @DateAcquired, @Notes)";

        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@Name", collectible.Name);
        command.Parameters.AddWithValue("@Manufacturer", collectible.Manufacturer);
        command.Parameters.AddWithValue("@Character", collectible.Character ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@DecoId", collectible.DecoId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@ClassificationId", collectible.ClassificationId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Reissue", collectible.Reissue ? 1 : 0);
        command.Parameters.AddWithValue("@Stylized", collectible.Stylized ? 1 : 0);
        command.Parameters.AddWithValue("@OriginalPrice", collectible.OriginalPrice);
        command.Parameters.AddWithValue("@CurrentValue", collectible.CurrentValue);
        command.Parameters.AddWithValue("@ImagePath", collectible.ImagePath ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@DateAdded", collectible.DateAdded.ToString("yyyy-MM-dd HH:mm:ss"));
        command.Parameters.AddWithValue("@DateAcquired", collectible.DateAcquired?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Notes", collectible.Notes ?? (object)DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateCollectibleAsync(Collectible collectible)
    {
        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        var query = @"UPDATE Collectibles 
                     SET Name = @Name, Manufacturer = @Manufacturer, Character = @Character, DecoId = @DecoId, 
                         ClassificationId = @ClassificationId, Reissue = @Reissue, Stylized = @Stylized, OriginalPrice = @OriginalPrice, 
                         CurrentValue = @CurrentValue, ImagePath = @ImagePath, DateAcquired = @DateAcquired, Notes = @Notes
                     WHERE Id = @Id";

        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@Id", collectible.Id);
        command.Parameters.AddWithValue("@Name", collectible.Name);
        command.Parameters.AddWithValue("@Manufacturer", collectible.Manufacturer);
        command.Parameters.AddWithValue("@Character", collectible.Character ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@DecoId", collectible.DecoId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@ClassificationId", collectible.ClassificationId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Reissue", collectible.Reissue ? 1 : 0);
        command.Parameters.AddWithValue("@Stylized", collectible.Stylized ? 1 : 0);
        command.Parameters.AddWithValue("@OriginalPrice", collectible.OriginalPrice);
        command.Parameters.AddWithValue("@CurrentValue", collectible.CurrentValue);
        command.Parameters.AddWithValue("@ImagePath", collectible.ImagePath ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@DateAcquired", collectible.DateAcquired?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Notes", collectible.Notes ?? (object)DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteCollectibleAsync(int id)
    {
        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        // First, get the image path so we can delete the file
        var selectQuery = "SELECT ImagePath FROM Collectibles WHERE Id = @Id";
        using var selectCommand = new SQLiteCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@Id", id);
        
        string? imagePath = null;
        using (var reader = await selectCommand.ExecuteReaderAsync())
        {
            if (await reader.ReadAsync())
            {
                var imagePathOrdinal = reader.GetOrdinal("ImagePath");
                if (!reader.IsDBNull(imagePathOrdinal))
                {
                    imagePath = reader.GetString(imagePathOrdinal);
                }
            }
        }

        // Delete the database record
        var deleteQuery = "DELETE FROM Collectibles WHERE Id = @Id";
        using var deleteCommand = new SQLiteCommand(deleteQuery, connection);
        deleteCommand.Parameters.AddWithValue("@Id", id);
        await deleteCommand.ExecuteNonQueryAsync();

        // Delete the image file if it exists
        if (!string.IsNullOrEmpty(imagePath))
        {
            try
            {
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }
            catch (Exception ex)
            {
                // Log but don't throw - database record is already deleted
                Console.WriteLine($"Error deleting image file: {ex.Message}");
            }
        }
    }

    public async Task<decimal> GetTotalOriginalValueAsync()
    {
        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        var query = "SELECT SUM(OriginalPrice) FROM Collectibles";
        using var command = new SQLiteCommand(query, connection);
        var result = await command.ExecuteScalarAsync();

        return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
    }

    public async Task<decimal> GetTotalCurrentValueAsync()
    {
        using var connection = _databaseService.GetConnection();
        await connection.OpenAsync();

        var query = "SELECT SUM(CurrentValue) FROM Collectibles";
        using var command = new SQLiteCommand(query, connection);
        var result = await command.ExecuteScalarAsync();

        return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
    }
}
