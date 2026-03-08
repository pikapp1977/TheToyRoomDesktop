using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using TheToyRoomDesktop.Models;

namespace TheToyRoomDesktop.Views;

public partial class AddItemWindow : Window
{
    private Collectible? _editItem;
    private List<Manufacturer> _manufacturers = new();
    private List<Deco> _decos = new();
    private List<Classification> _classifications = new();
    private string? _selectedImagePath;
    public bool ItemSaved { get; private set; } = false;

    public AddItemWindow(Collectible? editItem = null)
    {
        InitializeComponent();
        _editItem = editItem;
        Loaded += AddItemWindow_Loaded;
    }

    private async void AddItemWindow_Loaded(object sender, RoutedEventArgs e)
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
            _classifications = await App.ClassificationService!.GetAllClassificationsAsync();
            
            ManufacturerComboBox.ItemsSource = _manufacturers.Select(m => m.Name).ToList();
            DecoComboBox.ItemsSource = _decos.Select(d => d.Name).ToList();
            ClassificationComboBox.ItemsSource = _classifications.Select(c => c.Name).ToList();
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
        ClassificationComboBox.Text = item.ClassificationName;
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

    private void ClassificationComboBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var text = ClassificationComboBox.Text.ToLower();
        if (string.IsNullOrWhiteSpace(text))
        {
            ClassificationComboBox.ItemsSource = _classifications.Select(c => c.Name).ToList();
            return;
        }

        var filtered = _classifications
            .Where(c => c.Name.ToLower().Contains(text))
            .Select(c => c.Name)
            .Take(10)
            .ToList();
        
        ClassificationComboBox.ItemsSource = filtered;
    }

    private async void AddManufacturer_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new TextInputDialog("Add New Manufacturer", "Enter manufacturer name:");
        if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.ResponseText))
        {
            var manufacturerName = dialog.ResponseText.Trim();
            
            // Check if it already exists
            var existing = await App.ManufacturerService!.GetManufacturerByNameAsync(manufacturerName);
            if (existing != null)
            {
                MessageBox.Show($"Manufacturer '{manufacturerName}' already exists.", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Information);
                ManufacturerComboBox.Text = manufacturerName;
                return;
            }
            
            try
            {
                await App.ManufacturerService!.AddManufacturerAsync(manufacturerName);
                await LoadDropdownData(); // Refresh the list
                ManufacturerComboBox.Text = manufacturerName; // Select the new one
                MessageBox.Show($"Manufacturer '{manufacturerName}' added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding manufacturer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void AddDeco_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new TextInputDialog("Add New Deco", "Enter deco name:");
        if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.ResponseText))
        {
            var decoName = dialog.ResponseText.Trim();
            
            // Check if it already exists
            var existing = await App.DecoService!.GetDecoByNameAsync(decoName);
            if (existing != null)
            {
                MessageBox.Show($"Deco '{decoName}' already exists.", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Information);
                DecoComboBox.Text = decoName;
                return;
            }
            
            try
            {
                await App.DecoService!.AddDecoAsync(decoName);
                await LoadDropdownData(); // Refresh the list
                DecoComboBox.Text = decoName; // Select the new one
                MessageBox.Show($"Deco '{decoName}' added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding deco: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void AddClassification_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new TextInputDialog("Add New Classification", "Enter classification name:");
        if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.ResponseText))
        {
            var classificationName = dialog.ResponseText.Trim();
            
            // Check if it already exists
            var existing = await App.ClassificationService!.GetClassificationByNameAsync(classificationName);
            if (existing != null)
            {
                MessageBox.Show($"Classification '{classificationName}' already exists.", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Information);
                ClassificationComboBox.Text = classificationName;
                return;
            }
            
            try
            {
                await App.ClassificationService!.AddClassificationAsync(classificationName);
                await LoadDropdownData(); // Refresh the list
                ClassificationComboBox.Text = classificationName; // Select the new one
                MessageBox.Show($"Classification '{classificationName}' added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding classification: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void BrowseImage_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp",
            Title = "Select Image"
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
        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath);
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

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(NameBox.Text))
        {
            MessageBox.Show("Please enter a name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(ManufacturerComboBox.Text))
        {
            MessageBox.Show("Please enter a manufacturer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            // Get or create manufacturer
            var manufacturerName = ManufacturerComboBox.Text.Trim();
            var manufacturer = await App.ManufacturerService!.GetManufacturerByNameAsync(manufacturerName);
            if (manufacturer == null)
            {
                await App.ManufacturerService!.AddManufacturerAsync(manufacturerName);
            }

            // Get or create deco if provided
            int? decoId = null;
            if (!string.IsNullOrWhiteSpace(DecoComboBox.Text))
            {
                var decoName = DecoComboBox.Text.Trim();
                decoId = await App.DecoService!.AddDecoAsync(decoName);
            }

            // Get or create classification if provided
            int? classificationId = null;
            if (!string.IsNullOrWhiteSpace(ClassificationComboBox.Text))
            {
                var classificationName = ClassificationComboBox.Text.Trim();
                classificationId = await App.ClassificationService!.AddClassificationAsync(classificationName);
            }

            var collectible = new Collectible
            {
                Name = NameBox.Text.Trim(),
                Character = string.IsNullOrWhiteSpace(CharacterBox.Text) ? null : CharacterBox.Text.Trim(),
                Manufacturer = manufacturerName,
                DecoId = decoId,
                ClassificationId = classificationId,
                Reissue = ReissueCheckBox.IsChecked ?? false,
                Stylized = StylizedCheckBox.IsChecked ?? false,
                OriginalPrice = decimal.TryParse(OriginalPriceBox.Text, out var op) ? op : 0,
                CurrentValue = decimal.TryParse(CurrentValueBox.Text, out var cv) ? cv : 0,
                DateAcquired = DateAcquiredPicker.SelectedDate,
                ImagePath = _selectedImagePath,
                Notes = string.IsNullOrWhiteSpace(NotesBox.Text) ? null : NotesBox.Text.Trim()
            };

            if (_editItem != null)
            {
                // Update existing item
                collectible.Id = _editItem.Id;
                collectible.DateAdded = _editItem.DateAdded;
                await App.CollectibleService!.UpdateCollectibleAsync(collectible);
                MessageBox.Show("Item updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Add new item
                collectible.DateAdded = DateTime.Now;
                await App.CollectibleService!.AddCollectibleAsync(collectible);
                MessageBox.Show("Item added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            ItemSaved = true;
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
