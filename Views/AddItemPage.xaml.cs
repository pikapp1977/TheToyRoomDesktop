using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using TheToyRoomDesktop.Models;

namespace TheToyRoomDesktop.Views;

public partial class AddItemPage : Page
{
    private Collectible? _editItem;
    private List<Manufacturer> _manufacturers = new();
    private List<Deco> _decos = new();
    private string? _selectedImagePath;

    public AddItemPage(Collectible? editItem = null)
    {
        InitializeComponent();
        _editItem = editItem;
        Loaded += AddItemPage_Loaded;
    }

    private async void AddItemPage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadDropdownData();
        
        if (_editItem != null)
        {
            TitleText.Text = "Edit Item";
            SaveButton.Content = "Update Item";
            LoadItemForEdit(_editItem);
        }
    }

    private async Task LoadDropdownData()
    {
        try
        {
            _manufacturers = await App.ManufacturerService!.GetAllManufacturersAsync();
            _decos = await App.DecoService!.GetAllDecosAsync();
            
            ManufacturerComboBox.ItemsSource = _manufacturers.Select(m => m.Name).ToList();
            DecoComboBox.ItemsSource = _decos.Select(d => d.Name).ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading dropdown data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadItemForEdit(Collectible item)
    {
        NameBox.Text = item.Name;
        CharacterBox.Text = item.Character;
        ManufacturerComboBox.Text = item.Manufacturer;
        DecoComboBox.Text = item.DecoName;
        ReissueCheckBox.IsChecked = item.Reissue;
        StylizedCheckBox.IsChecked = item.Stylized;
        OriginalPriceBox.Text = item.OriginalPrice.ToString("F2");
        CurrentValueBox.Text = item.CurrentValue.ToString("F2");
        DateAcquiredPicker.SelectedDate = item.DateAcquired;
        NotesBox.Text = item.Notes;
        
        if (!string.IsNullOrWhiteSpace(item.ImagePath))
        {
            _selectedImagePath = item.ImagePath;
            ImagePathBox.Text = item.ImagePath;
            LoadImagePreview(item.ImagePath);
        }
    }

    private void ManufacturerComboBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var text = ManufacturerComboBox.Text.ToLower();
        if (string.IsNullOrWhiteSpace(text))
        {
            ManufacturerComboBox.ItemsSource = _manufacturers.Select(m => m.Name).ToList();
            return;
        }

        var filtered = _manufacturers
            .Where(m => m.Name.ToLower().Contains(text))
            .Select(m => m.Name)
            .Take(10)
            .ToList();
        
        ManufacturerComboBox.ItemsSource = filtered;
    }

    private void DecoComboBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var text = DecoComboBox.Text.ToLower();
        if (string.IsNullOrWhiteSpace(text))
        {
            DecoComboBox.ItemsSource = _decos.Select(d => d.Name).ToList();
            return;
        }

        var filtered = _decos
            .Where(d => d.Name.ToLower().Contains(text))
            .Select(d => d.Name)
            .Take(10)
            .ToList();
        
        DecoComboBox.ItemsSource = filtered;
    }

    private void BrowseImage_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp",
            Title = "Select an Image"
        };

        if (dialog.ShowDialog() == true)
        {
            _selectedImagePath = dialog.FileName;
            ImagePathBox.Text = dialog.FileName;
            LoadImagePreview(dialog.FileName);
        }
    }

    private void LoadImagePreview(string imagePath)
    {
        if (File.Exists(imagePath))
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                ImagePreview.Source = bitmap;
                ImagePreviewBorder.Visibility = Visibility.Visible;
            }
            catch
            {
                ImagePreviewBorder.Visibility = Visibility.Collapsed;
            }
        }
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(NameBox.Text))
        {
            MessageBox.Show("Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(ManufacturerComboBox.Text))
        {
            MessageBox.Show("Manufacturer is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(OriginalPriceBox.Text, out var originalPrice) || originalPrice < 0)
        {
            MessageBox.Show("Please enter a valid Original Price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(CurrentValueBox.Text, out var currentValue) || currentValue < 0)
        {
            MessageBox.Show("Please enter a valid Current Value.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            // Get or create manufacturer
            var manufacturerName = ManufacturerComboBox.Text.Trim();
            await App.ManufacturerService!.AddManufacturerAsync(manufacturerName);

            // Get or create deco if provided
            int? decoId = null;
            if (!string.IsNullOrWhiteSpace(DecoComboBox.Text))
            {
                var decoName = DecoComboBox.Text.Trim();
                decoId = await App.DecoService!.AddDecoAsync(decoName);
            }

            var collectible = new Collectible
            {
                Id = _editItem?.Id ?? 0,
                Name = NameBox.Text.Trim(),
                Character = string.IsNullOrWhiteSpace(CharacterBox.Text) ? null : CharacterBox.Text.Trim(),
                Manufacturer = manufacturerName,
                DecoId = decoId,
                Reissue = ReissueCheckBox.IsChecked ?? false,
                Stylized = StylizedCheckBox.IsChecked ?? false,
                OriginalPrice = originalPrice,
                CurrentValue = currentValue,
                DateAcquired = DateAcquiredPicker.SelectedDate,
                DateAdded = _editItem?.DateAdded ?? DateTime.Now,
                ImagePath = _selectedImagePath,
                Notes = string.IsNullOrWhiteSpace(NotesBox.Text) ? null : NotesBox.Text.Trim()
            };

            if (_editItem == null)
            {
                await App.CollectibleService!.AddCollectibleAsync(collectible);
                MessageBox.Show("Item added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                await App.CollectibleService!.UpdateCollectibleAsync(collectible);
                MessageBox.Show("Item updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // Navigate back to collection page
            NavigationService?.Navigate(new CollectionPage());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new CollectionPage());
    }
}
