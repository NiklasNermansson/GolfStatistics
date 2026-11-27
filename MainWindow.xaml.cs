using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;

namespace GolfStatistics;

public sealed partial class MainWindow : Window
{
    private readonly SpreadsheetCellReader _cellReader = new();

    public MainWindow()
    {
        InitializeComponent();

        var cachedValue = _cellReader.ReadCachedValue();
        CellValueTextBox.Text = cachedValue ?? string.Empty;
        StatusTextBlock.Text = cachedValue is null
            ? "No cached value. Retrieve from Google to sign in and fetch it."
            : "Showing the locally cached value.";
    }

    private async void RetrieveButton_Click(object sender, RoutedEventArgs args)
    {
        RetrieveButton.IsEnabled = false;
        StatusTextBlock.Text = "Retrieving from Google Sheets...";

        try
        {
            var clientSecretsPath = _cellReader.GetClientSecretsPath();
            if (clientSecretsPath is null)
            {
                var picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".json");
                WinRT.Interop.InitializeWithWindow.Initialize(
                    picker,
                    WinRT.Interop.WindowNative.GetWindowHandle(this));

                var file = await picker.PickSingleFileAsync();
                if (file is null)
                {
                    StatusTextBlock.Text = "Retrieval canceled.";
                    return;
                }

                clientSecretsPath = file.Path;
            }

            StatusTextBlock.Text = "Signing in and retrieving from Google Sheets...";
            CellValueTextBox.Text = await _cellReader.RetrieveAsync(clientSecretsPath);
            StatusTextBlock.Text = "Value updated from Google Sheets.";
        }
        catch (Exception exception)
        {
            StatusTextBlock.Text = $"Retrieval failed: {exception.Message}";
        }
        finally
        {
            RetrieveButton.IsEnabled = true;
        }
    }
}
