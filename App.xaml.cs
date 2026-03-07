using System.Windows;
using TheToyRoomDesktop.Services;

namespace TheToyRoomDesktop;

public partial class App : Application
{
    public static DatabaseService? DatabaseService { get; private set; }
    public static CollectibleService? CollectibleService { get; private set; }
    public static ManufacturerService? ManufacturerService { get; private set; }
    public static DecoService? DecoService { get; private set; }
    public static ExportService? ExportService { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Set up database path
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TheToyRoom"
        );
        
        if (!Directory.Exists(appDataPath))
        {
            Directory.CreateDirectory(appDataPath);
        }

        var dbPath = Path.Combine(appDataPath, "thetoyroom.db");

        // Initialize services
        DatabaseService = new DatabaseService(dbPath);
        CollectibleService = new CollectibleService(DatabaseService);
        ManufacturerService = new ManufacturerService(DatabaseService);
        DecoService = new DecoService(DatabaseService);
        ExportService = new ExportService();
    }
}
