using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using TheToyRoomDesktop.Models;

namespace TheToyRoomDesktop.Views;

public partial class ReportsPage : Page
{
    private List<CollectibleWithGainLoss> _allItems = new();
    private List<string> _manufacturers = new();

    public class CollectibleWithGainLoss
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Character { get; set; }
        public string? DecoName { get; set; }
        public string Manufacturer { get; set; } = "";
        public decimal OriginalPrice { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal GainLoss => CurrentValue - OriginalPrice;
    }

    public ReportsPage()
    {
        InitializeComponent();
        Loaded += ReportsPage_Loaded;
    }

    private async void ReportsPage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        try
        {
            var collectibles = await App.CollectibleService!.GetAllCollectiblesAsync();
            
            _allItems = collectibles.Select(c => new CollectibleWithGainLoss
            {
                Id = c.Id,
                Name = c.Name,
                Character = c.Character,
                DecoName = c.DecoName,
                Manufacturer = c.Manufacturer,
                OriginalPrice = c.OriginalPrice,
                CurrentValue = c.CurrentValue
            }).ToList();

            _manufacturers = collectibles.Select(c => c.Manufacturer).Distinct().OrderBy(m => m).ToList();
            _manufacturers.Insert(0, "All Manufacturers");
            
            ManufacturerFilter.ItemsSource = _manufacturers;
            ManufacturerFilter.SelectedIndex = 0;
            
            UpdateDisplay(_allItems);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateDisplay(List<CollectibleWithGainLoss> items)
    {
        ReportsGrid.ItemsSource = items;
        
        TotalItemsText.Text = items.Count.ToString();
        
        var totalOriginal = items.Sum(i => i.OriginalPrice);
        var totalCurrent = items.Sum(i => i.CurrentValue);
        var totalGainLoss = totalCurrent - totalOriginal;
        
        TotalOriginalText.Text = totalOriginal.ToString("C2");
        TotalCurrentText.Text = totalCurrent.ToString("C2");
        TotalGainLossText.Text = totalGainLoss.ToString("C2");
        TotalGainLossText.Foreground = totalGainLoss >= 0 ? Brushes.Green : Brushes.Red;
    }

    private void ManufacturerFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ManufacturerFilter.SelectedItem is string manufacturer && manufacturer != "All Manufacturers")
        {
            var filtered = _allItems.Where(i => i.Manufacturer == manufacturer).ToList();
            UpdateDisplay(filtered);
        }
        else
        {
            UpdateDisplay(_allItems);
        }
    }

    private void ClearFilter_Click(object sender, RoutedEventArgs e)
    {
        ManufacturerFilter.SelectedIndex = 0;
    }

    private async void ExportExcel_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = $"MyCollectionReport_{DateTime.Now:MMddyyyy}.xlsx",
                Title = "Export to Excel"
            };

            if (dialog.ShowDialog() == true)
            {
                var items = (ReportsGrid.ItemsSource as List<CollectibleWithGainLoss>) ?? new();
                var collectibles = await App.CollectibleService!.GetAllCollectiblesAsync();
                
                // Filter based on current view
                var filteredCollectibles = collectibles
                    .Where(c => items.Any(i => i.Id == c.Id))
                    .ToList();

                var excelData = App.ExportService!.ExportCollectionToExcel(filteredCollectibles, "Collection Report");
                await File.WriteAllBytesAsync(dialog.FileName, excelData);
                
                MessageBox.Show($"Report exported successfully to:\n{dialog.FileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error exporting to Excel: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
