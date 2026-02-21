using Avalonia.Controls;
using Avalonia.Interactivity;
using VTAvalonia.ViewModels;

namespace VTAvalonia.Views;

public partial class OptionsWindow : Window
{
    public OptionsWindow()
    {
        InitializeComponent();
        DataContext = new OptionsViewModel();
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
