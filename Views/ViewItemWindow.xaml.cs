using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using TheToyRoomDesktop.Models;

namespace TheToyRoomDesktop.Views;

public partial class ViewItemWindow : Window
{
    public ViewItemWindow(Collectible item)
    {
        InitializeComponent();
        LoadItemDetails(item);
    }

    private void LoadItemDetails(Collectible item)
    {
        NameText.Text = item.Name;
        CharacterText.Text = item.Character ?? "N/A";
        ManufacturerText.Text = item.Manufacturer;
        DecoText.Text = item.DecoName ?? "N/A";
        ClassificationText.Text = item.ClassificationName ?? "N/A";
        ReissueText.Text = item.Reissue ? "Yes" : "No";
        StylizedText.Text = item.Stylized ? "Yes" : "No";
        OriginalPriceText.Text = item.OriginalPrice.ToString("C2");
        CurrentValueText.Text = item.CurrentValue.ToString("C2");
        DateAcquiredText.Text = item.DateAcquired?.ToString("MM/dd/yyyy") ?? "N/A";
        DateAddedText.Text = item.DateAdded.ToString("MM/dd/yyyy");
        NotesText.Text = string.IsNullOrWhiteSpace(item.Notes) ? "N/A" : item.Notes;

        // Load image if exists
        if (!string.IsNullOrWhiteSpace(item.ImagePath) && File.Exists(item.ImagePath))
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(item.ImagePath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                ItemImage.Source = bitmap;
                ImageBorder.Visibility = Visibility.Visible;
            }
            catch
            {
                // Image failed to load, keep it hidden
            }
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
