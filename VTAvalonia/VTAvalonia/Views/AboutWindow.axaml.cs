using Avalonia.Controls;
using Avalonia.Interactivity;
using VTAvalonia.ViewModels;

namespace VTAvalonia.Views;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        DataContext = new AboutViewModel();
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
