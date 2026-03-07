using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace TheToyRoomDesktop.Views;

public partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
        Loaded += SettingsPage_Loaded;
    }

    private async void SettingsPage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadSettings();
    }

    private async Task LoadSettings()
    {
        try
        {
            // Get database path
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "TheToyRoom"
            );
            var dbPath = Path.Combine(appDataPath, "thetoyroom.db");
            
            DbPathText.Text = dbPath;
            
            // Get file size
            if (File.Exists(dbPath))
            {
                var fileInfo = new FileInfo(dbPath);
                var sizeInKb = fileInfo.Length / 1024.0;
                DbSizeText.Text = $"{sizeInKb:F2} KB";
            }
            else
            {
                DbSizeText.Text = "Database not found";
            }
            
            // Get counts
            var collectibles = await App.CollectibleService!.GetAllCollectiblesAsync();
            var manufacturers = await App.ManufacturerService!.GetAllManufacturersAsync();
            var decos = await App.DecoService!.GetAllDecosAsync();
            
            TotalItemsText.Text = collectibles.Count.ToString();
            ManufacturersText.Text = manufacturers.Count.ToString();
            DecosText.Text = decos.Count.ToString();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        await LoadSettings();
    }
}
