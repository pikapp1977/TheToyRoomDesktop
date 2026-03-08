using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TheToyRoomDesktop.Models;

namespace TheToyRoomDesktop.Views;

public partial class CollectionPage : Page
{
    private List<Collectible> _allCollectibles = new();

    public CollectionPage()
    {
        InitializeComponent();
        Loaded += CollectionPage_Loaded;
    }

    private async void CollectionPage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadCollectibles();
    }

    private async Task LoadCollectibles()
    {
        try
        {
            _allCollectibles = await App.CollectibleService!.GetAllCollectiblesAsync();
            CollectionGrid.ItemsSource = _allCollectibles;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading collectibles: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public async void RefreshCollection()
    {
        await LoadCollectibles();
        SearchBox.Text = "";
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var searchTerm = SearchBox.Text.ToLower();
        
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            CollectionGrid.ItemsSource = _allCollectibles;
            return;
        }

        var filtered = _allCollectibles.Where(c =>
            c.Name.ToLower().Contains(searchTerm) ||
            (c.Character?.ToLower().Contains(searchTerm) ?? false) ||
            c.Manufacturer.ToLower().Contains(searchTerm) ||
            (c.DecoName?.ToLower().Contains(searchTerm) ?? false)
        ).ToList();

        CollectionGrid.ItemsSource = filtered;
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        await LoadCollectibles();
        SearchBox.Text = "";
    }

    private void View_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int id)
        {
            var item = _allCollectibles.FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                var viewWindow = new ViewItemWindow(item);
                viewWindow.Owner = Window.GetWindow(this);
                viewWindow.ShowDialog();
            }
        }
    }

    private void Edit_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int id)
        {
            var item = _allCollectibles.FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                var editWindow = new AddItemWindow(item);
                editWindow.Owner = Window.GetWindow(this);
                if (editWindow.ShowDialog() == true && editWindow.ItemSaved)
                {
                    RefreshCollection();
                }
            }
        }
    }

    private async void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int id)
        {
            var item = _allCollectibles.FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete '{item.Name}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await App.CollectibleService!.DeleteCollectibleAsync(id);
                        await LoadCollectibles();
                        MessageBox.Show("Item deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }

    private void CollectionGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (CollectionGrid.SelectedItem is Collectible item)
        {
            var viewWindow = new ViewItemWindow(item);
            viewWindow.Owner = Window.GetWindow(this);
            viewWindow.ShowDialog();
        }
    }
}
