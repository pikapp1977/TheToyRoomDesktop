using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TheToyRoomDesktop.Views;

public partial class HomePage : Page
{
    public HomePage()
    {
        InitializeComponent();
        Loaded += HomePage_Loaded;
    }

    private async void HomePage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadDashboardData();
    }

    private async Task LoadDashboardData()
    {
        try
        {
            var collectibles = await App.CollectibleService!.GetAllCollectiblesAsync();
            
            // Update statistics
            TotalItemsText.Text = collectibles.Count.ToString();
            
            var totalOriginal = collectibles.Sum(c => c.OriginalPrice);
            var totalCurrent = collectibles.Sum(c => c.CurrentValue);
            var gainLoss = totalCurrent - totalOriginal;
            
            TotalOriginalText.Text = totalOriginal.ToString("C2");
            TotalCurrentText.Text = totalCurrent.ToString("C2");
            GainLossText.Text = gainLoss.ToString("C2");
            
            // Color the gain/loss based on positive or negative
            GainLossText.Foreground = gainLoss >= 0 ? Brushes.Green : Brushes.Red;
            
            // Show recent 10 items
            RecentItemsGrid.ItemsSource = collectibles.Take(10).ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
