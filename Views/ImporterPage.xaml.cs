using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using TheToyRoomDesktop.Models;

namespace TheToyRoomDesktop.Views;

public partial class ImporterPage : Page
{
    private string? _selectedFilePath;
    private List<ImportItem> _importItems = new();

    public class ImportItem
    {
        public bool IsSelected { get; set; } = true;
        public string Name { get; set; } = "";
        public string? Character { get; set; }
        public string Manufacturer { get; set; } = "";
        public string? DecoName { get; set; }
        public bool Reissue { get; set; }
        public bool Stylized { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal CurrentValue { get; set; }
        public DateTime? DateAcquired { get; set; }
        public string? Notes { get; set; }
    }

    public ImporterPage()
    {
        InitializeComponent();
    }

    private void DownloadTemplate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = $"ImportTemplate_{DateTime.Now:MMddyyyy}.xlsx",
                Title = "Save Import Template"
            };

            if (dialog.ShowDialog() == true)
            {
                var templateData = App.ExportService!.GenerateImportTemplate();
                File.WriteAllBytes(dialog.FileName, templateData);
                MessageBox.Show($"Template downloaded successfully to:\n{dialog.FileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error downloading template: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ChooseFile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Excel Files|*.xlsx;*.xls",
            Title = "Select Import File"
        };

        if (dialog.ShowDialog() == true)
        {
            _selectedFilePath = dialog.FileName;
            FilePathBox.Text = dialog.FileName;
            ParseButton.IsEnabled = true;
            PreviewBorder.Visibility = Visibility.Collapsed;
        }
    }

    private void ParseFile_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_selectedFilePath) || !File.Exists(_selectedFilePath))
        {
            MessageBox.Show("Please select a valid file.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using var fileStream = File.OpenRead(_selectedFilePath);
            var result = App.ExportService!.ParseImportFile(fileStream);

            _importItems = result.Items.Select(c => new ImportItem
            {
                IsSelected = true,
                Name = c.Name,
                Character = c.Character,
                Manufacturer = c.Manufacturer,
                DecoName = c.DecoName,
                Reissue = c.Reissue,
                Stylized = c.Stylized,
                OriginalPrice = c.OriginalPrice,
                CurrentValue = c.CurrentValue,
                DateAcquired = c.DateAcquired,
                Notes = c.Notes
            }).ToList();

            // Update summary
            SummaryText.Text = $"Found {result.Items.Count} valid items ready to import.";
            
            if (result.SkippedRows.Any())
            {
                SkippedText.Text = $"Skipped {result.SkippedRows.Count} rows:\n" + string.Join("\n", result.SkippedRows);
                SkippedText.Visibility = Visibility.Visible;
            }
            else
            {
                SkippedText.Visibility = Visibility.Collapsed;
            }

            PreviewGrid.ItemsSource = _importItems;
            PreviewBorder.Visibility = Visibility.Visible;

            if (result.Items.Count == 0)
            {
                MessageBox.Show("No valid items found in the file.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error parsing file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SelectAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var item in _importItems)
        {
            item.IsSelected = true;
        }
        PreviewGrid.Items.Refresh();
    }

    private void DeselectAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var item in _importItems)
        {
            item.IsSelected = false;
        }
        PreviewGrid.Items.Refresh();
    }

    private async void ImportItems_Click(object sender, RoutedEventArgs e)
    {
        var selectedItems = _importItems.Where(i => i.IsSelected).ToList();
        
        if (!selectedItems.Any())
        {
            MessageBox.Show("Please select at least one item to import.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var importedCount = 0;
            var skippedCount = 0;

            foreach (var item in selectedItems)
            {
                // Check for duplicates
                var existing = await App.CollectibleService!.GetAllCollectiblesAsync();
                var duplicate = existing.FirstOrDefault(c => 
                    c.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase) && 
                    c.Manufacturer.Equals(item.Manufacturer, StringComparison.OrdinalIgnoreCase));

                if (duplicate != null)
                {
                    skippedCount++;
                    continue;
                }

                // Add manufacturer if needed
                await App.ManufacturerService!.AddManufacturerAsync(item.Manufacturer);

                // Add deco if provided
                int? decoId = null;
                if (!string.IsNullOrWhiteSpace(item.DecoName))
                {
                    decoId = await App.DecoService!.AddDecoAsync(item.DecoName);
                }

                // Create collectible
                var collectible = new Collectible
                {
                    Name = item.Name,
                    Character = item.Character,
                    Manufacturer = item.Manufacturer,
                    DecoId = decoId,
                    Reissue = item.Reissue,
                    Stylized = item.Stylized,
                    OriginalPrice = item.OriginalPrice,
                    CurrentValue = item.CurrentValue,
                    DateAcquired = item.DateAcquired,
                    DateAdded = DateTime.Now,
                    Notes = item.Notes
                };

                await App.CollectibleService!.AddCollectibleAsync(collectible);
                importedCount++;
            }

            var message = $"Successfully imported {importedCount} items.";
            if (skippedCount > 0)
            {
                message += $"\n{skippedCount} duplicate items were skipped.";
            }

            MessageBox.Show(message, "Import Complete", MessageBoxButton.OK, MessageBoxImage.Information);

            // Reset the form
            _selectedFilePath = null;
            FilePathBox.Text = "";
            ParseButton.IsEnabled = false;
            PreviewBorder.Visibility = Visibility.Collapsed;
            _importItems.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error importing items: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
