using System.Windows;
using TheToyRoomDesktop.Views;

namespace TheToyRoomDesktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // Load Home view by default
        MainFrame.Navigate(new HomePage());
    }

    private void Home_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new HomePage());
    }

    private void Collection_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new CollectionPage());
    }

    private void AddItem_Click(object sender, RoutedEventArgs e)
    {
        var addItemWindow = new AddItemWindow();
        addItemWindow.Owner = this;
        if (addItemWindow.ShowDialog() == true && addItemWindow.ItemSaved)
        {
            // Refresh the current page if it's the Collection page
            if (MainFrame.Content is CollectionPage collectionPage)
            {
                collectionPage.RefreshCollection();
            }
            else if (MainFrame.Content is HomePage homePage)
            {
                // Navigate to collection to show the new item
                MainFrame.Navigate(new CollectionPage());
            }
        }
    }

    private void Reports_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new ReportsPage());
    }

    private void Importer_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new ImporterPage());
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new SettingsPage());
    }
}
