using System.IO;
using System.Windows;
using System.Windows.Controls;
using TheToyRoomDesktop.Models;

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
            
            // Load grids
            ManufacturersGrid.ItemsSource = manufacturers.OrderBy(m => m.Name).ToList();
            DecosGrid.ItemsSource = decos.OrderBy(d => d.Name).ToList();
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

    private async void AddManufacturer_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new TextInputDialog("Add New Manufacturer", "Enter manufacturer name:");
        dialog.Owner = Window.GetWindow(this);
        
        if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.ResponseText))
        {
            var manufacturerName = dialog.ResponseText.Trim();
            
            // Check if it already exists
            var existing = await App.ManufacturerService!.GetManufacturerByNameAsync(manufacturerName);
            if (existing != null)
            {
                MessageBox.Show($"Manufacturer '{manufacturerName}' already exists.", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            try
            {
                await App.ManufacturerService!.AddManufacturerAsync(manufacturerName);
                await LoadSettings(); // Refresh
                MessageBox.Show($"Manufacturer '{manufacturerName}' added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding manufacturer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void EditManufacturer_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int id)
        {
            var manufacturers = await App.ManufacturerService!.GetAllManufacturersAsync();
            var manufacturer = manufacturers.FirstOrDefault(m => m.Id == id);
            
            if (manufacturer != null)
            {
                var dialog = new TextInputDialog("Edit Manufacturer", $"Edit manufacturer name:");
                dialog.Owner = Window.GetWindow(this);
                dialog.SetInitialValue(manufacturer.Name);
                
                if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.ResponseText))
                {
                    var newName = dialog.ResponseText.Trim();
                    
                    // Check if unchanged
                    if (newName.Equals(manufacturer.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return; // No change
                    }
                    
                    // Check if new name already exists
                    var existing = await App.ManufacturerService!.GetManufacturerByNameAsync(newName);
                    if (existing != null && existing.Id != id)
                    {
                        MessageBox.Show($"Manufacturer '{newName}' already exists.", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    
                    try
                    {
                        // Update all collectibles that use this manufacturer
                        var collectibles = await App.CollectibleService!.GetAllCollectiblesAsync();
                        var affectedItems = collectibles.Where(c => c.Manufacturer.Equals(manufacturer.Name, StringComparison.OrdinalIgnoreCase)).ToList();
                        
                        var confirmMessage = affectedItems.Count > 0 
                            ? $"This will update {affectedItems.Count} collectible(s) from '{manufacturer.Name}' to '{newName}'.\n\nContinue?"
                            : $"Rename '{manufacturer.Name}' to '{newName}'?";
                        
                        var result = MessageBox.Show(
                            confirmMessage,
                            "Confirm Rename",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question
                        );
                        
                        if (result == MessageBoxResult.Yes)
                        {
                            // Update the manufacturer name in the lookup table
                            var oldName = manufacturer.Name;
                            manufacturer.Name = newName;
                            await App.ManufacturerService!.UpdateManufacturerAsync(manufacturer);
                            
                            // Update all collectibles with the old name
                            foreach (var item in affectedItems)
                            {
                                item.Manufacturer = newName;
                                await App.CollectibleService!.UpdateCollectibleAsync(item);
                            }
                            
                            await LoadSettings();
                            MessageBox.Show($"Successfully renamed to '{newName}' and updated {affectedItems.Count} collectible(s)!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error renaming manufacturer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }

    private async void DeleteManufacturer_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int id)
        {
            var manufacturers = await App.ManufacturerService!.GetAllManufacturersAsync();
            var manufacturer = manufacturers.FirstOrDefault(m => m.Id == id);
            
            if (manufacturer != null)
            {
                // Check if any collectibles use this manufacturer
                var collectibles = await App.CollectibleService!.GetAllCollectiblesAsync();
                var inUse = collectibles.Any(c => c.Manufacturer.Equals(manufacturer.Name, StringComparison.OrdinalIgnoreCase));
                
                if (inUse)
                {
                    MessageBox.Show(
                        $"Cannot delete '{manufacturer.Name}' because it is used by one or more collectibles.\n\nTip: Use 'Edit' to rename it instead.",
                        "Cannot Delete",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }
                
                var result = MessageBox.Show(
                    $"Are you sure you want to delete manufacturer '{manufacturer.Name}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await App.ManufacturerService!.DeleteManufacturerAsync(id);
                        await LoadSettings();
                        MessageBox.Show($"Manufacturer '{manufacturer.Name}' deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting manufacturer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }

    private async void AddDeco_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new TextInputDialog("Add New Deco", "Enter deco name:");
        dialog.Owner = Window.GetWindow(this);
        
        if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.ResponseText))
        {
            var decoName = dialog.ResponseText.Trim();
            
            // Check if it already exists
            var existing = await App.DecoService!.GetDecoByNameAsync(decoName);
            if (existing != null)
            {
                MessageBox.Show($"Deco '{decoName}' already exists.", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            try
            {
                await App.DecoService!.AddDecoAsync(decoName);
                await LoadSettings(); // Refresh
                MessageBox.Show($"Deco '{decoName}' added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding deco: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void EditDeco_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int id)
        {
            var decos = await App.DecoService!.GetAllDecosAsync();
            var deco = decos.FirstOrDefault(d => d.Id == id);
            
            if (deco != null)
            {
                var dialog = new TextInputDialog("Edit Deco", $"Edit deco name:");
                dialog.Owner = Window.GetWindow(this);
                dialog.SetInitialValue(deco.Name);
                
                if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.ResponseText))
                {
                    var newName = dialog.ResponseText.Trim();
                    
                    // Check if unchanged
                    if (newName.Equals(deco.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return; // No change
                    }
                    
                    // Check if new name already exists
                    var existing = await App.DecoService!.GetDecoByNameAsync(newName);
                    if (existing != null && existing.Id != id)
                    {
                        MessageBox.Show($"Deco '{newName}' already exists.", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    
                    try
                    {
                        // Count affected collectibles
                        var collectibles = await App.CollectibleService!.GetAllCollectiblesAsync();
                        var affectedCount = collectibles.Count(c => c.DecoId == id);
                        
                        var confirmMessage = affectedCount > 0 
                            ? $"This will update {affectedCount} collectible(s) from '{deco.Name}' to '{newName}'.\n\nContinue?"
                            : $"Rename '{deco.Name}' to '{newName}'?";
                        
                        var result = MessageBox.Show(
                            confirmMessage,
                            "Confirm Rename",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question
                        );
                        
                        if (result == MessageBoxResult.Yes)
                        {
                            // Update the deco name (foreign key relationship handles collectibles automatically)
                            deco.Name = newName;
                            await App.DecoService!.UpdateDecoAsync(deco);
                            
                            await LoadSettings();
                            MessageBox.Show($"Successfully renamed to '{newName}'! {affectedCount} collectible(s) updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error renaming deco: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }

    private async void DeleteDeco_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int id)
        {
            var decos = await App.DecoService!.GetAllDecosAsync();
            var deco = decos.FirstOrDefault(d => d.Id == id);
            
            if (deco != null)
            {
                // Check if any collectibles use this deco
                var collectibles = await App.CollectibleService!.GetAllCollectiblesAsync();
                var inUse = collectibles.Any(c => c.DecoId == id);
                
                if (inUse)
                {
                    MessageBox.Show(
                        $"Cannot delete '{deco.Name}' because it is used by one or more collectibles.\n\nTip: Use 'Edit' to rename it instead.",
                        "Cannot Delete",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }
                
                var result = MessageBox.Show(
                    $"Are you sure you want to delete deco '{deco.Name}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await App.DecoService!.DeleteDecoAsync(id);
                        await LoadSettings();
                        MessageBox.Show($"Deco '{deco.Name}' deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting deco: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
