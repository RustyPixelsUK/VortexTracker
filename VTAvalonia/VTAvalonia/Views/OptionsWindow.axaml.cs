using Avalonia.Controls;
using VTAvalonia.ViewModels;

namespace VTAvalonia.Views;

public partial class OptionsWindow : Window
{
    public OptionsWindow()
    {
        InitializeComponent();

        var app = App.Instance;
        var vm  = new OptionsViewModel(
            app?.Settings,
            app?.AudioOutput,
            app?.Midi,
            app?.SerialPort);

        vm.CloseRequested += Close;
        DataContext = vm;
    }
}

