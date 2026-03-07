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
        MainFrame.Navigate(new AddItemPage());
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
