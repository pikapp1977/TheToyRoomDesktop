namespace TheToyRoomDesktop.Models;

public class Collectible
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Manufacturer { get; set; } = "";
    public string? Character { get; set; }
    public int? DecoId { get; set; }
    public string? DecoName { get; set; }
    public bool Reissue { get; set; }
    public bool Stylized { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal CurrentValue { get; set; }
    public string? ImagePath { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime? DateAcquired { get; set; }
    public string? Notes { get; set; }
}
