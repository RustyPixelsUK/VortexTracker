using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LibVT;
using System.Linq;
using VortexTracker.Core.Services;
using VTAvalonia.Services;
using VTAvalonia.ViewModels;
using VTAvalonia.Views;

namespace VTAvalonia
{
    public partial class App : Application
    {
        public IPlatformPathsService PlatformPaths { get; } = new AvaloniaPlatformPathsService();
        public IAudioOutputService AudioOutput { get; } = new OpenAlAudioOutputService();
        public IMidiService Midi { get; } = new RtMidiMidiService();
        public ISerialPortService SerialPort { get; } = new DotNetSerialPortService();
        public AppSettingsManager Settings { get; private set; } = null!;

        public static App? Instance => Current as App;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // Wire AppEvents so LibVT can show message boxes and fire UI-thread events.
            AppEvents.IsOnUIThread = () => Dispatcher.UIThread.CheckAccess();
            AppEvents.Dispatcher = e =>
            {
                switch (e.EventType)
                {
                    case EventType.MessageBox:
                        // Synchronous message box on the UI thread.
                        // Params: message, caption, buttons, icon
                        var msg     = e.Params.Length > 0 ? e.Params[0]?.ToString() ?? "" : "";
                        var caption = e.Params.Length > 1 ? e.Params[1]?.ToString() ?? "" : "";
                        // YesNo → return 6 (IDYES), otherwise return 1 (IDOK)
                        var buttons = e.Params.Length > 2 ? e.Params[2] : null;
                        bool isYesNo = buttons is MyMessageBoxButtons b &&
                                       (b == MyMessageBoxButtons.YesNo || b == MyMessageBoxButtons.YesNoCancel);
                        // Show a native Avalonia dialog asynchronously but block the caller via the
                        // AppEventArgs wait handle so SendEvent can return the result.
                        e.Result = 1; // default IDOK / No
                        var dialog = new Avalonia.Controls.Window
                        {
                            Title = caption,
                            Width = 420, Height = 160,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner,
                            CanResize = false
                        };
                        var stack = new Avalonia.Controls.StackPanel { Margin = new Avalonia.Thickness(16), Spacing = 16 };
                        stack.Children.Add(new Avalonia.Controls.TextBlock { Text = msg, TextWrapping = Avalonia.Media.TextWrapping.Wrap });
                        var btnRow = new Avalonia.Controls.StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right, Spacing = 8 };
                        if (isYesNo)
                        {
                            var yes = new Avalonia.Controls.Button { Content = "Yes" };
                            var no  = new Avalonia.Controls.Button { Content = "No" };
                            yes.Click += (_, _) => { e.Result = 6; dialog.Close(); };
                            no.Click  += (_, _) => { e.Result = 7; dialog.Close(); };
                            btnRow.Children.Add(yes);
                            btnRow.Children.Add(no);
                        }
                        else
                        {
                            var ok = new Avalonia.Controls.Button { Content = "OK" };
                            ok.Click += (_, _) => { e.Result = 1; dialog.Close(); };
                            btnRow.Children.Add(ok);
                        }
                        stack.Children.Add(btnRow);
                        dialog.Content = stack;
                        dialog.Closed += (_, _) => e.Complete();
                        // Show without an owner so it works before the main window exists.
                        _ = dialog.ShowDialog(GetMainWindow());
                        break;

                    default:
                        e.Complete();
                        break;
                }
            };

            // Load persisted settings and apply to LibVT statics before any service
            // that touches WaveOutAPI / AY / Main is constructed.
            Settings = new AppSettingsManager(PlatformPaths);
            Settings.Load();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                DisableAvaloniaDataAnnotationValidation();
                desktop.MainWindow = new MainWindow();
                var dispatcher = new AvaloniaUIActionDispatcher();
                var fileDialogService = new AvaloniaFileDialogService(() => desktop.MainWindow);
                var clipboardService = new AvaloniaClipboardService(() => desktop.MainWindow);
                var windowingService = new AvaloniaWindowingService(() => desktop.MainWindow);
                var moduleService = new LibVtModuleService();
                var playbackService = new LibVtPlaybackService(moduleService);
                desktop.MainWindow.DataContext = new MainViewModel(dispatcher, fileDialogService, clipboardService, windowingService, moduleService, playbackService);
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                var mainView = new MainView();
                var dispatcher = new AvaloniaUIActionDispatcher();
                var fileDialogService = new AvaloniaFileDialogService(() => TopLevel.GetTopLevel(mainView));
                var clipboardService = new AvaloniaClipboardService(() => TopLevel.GetTopLevel(mainView));
                var windowingService = new AvaloniaWindowingService(() => TopLevel.GetTopLevel(mainView));
                var moduleService = new LibVtModuleService();
                var playbackService = new LibVtPlaybackService(moduleService);
                mainView.DataContext = new MainViewModel(dispatcher, fileDialogService, clipboardService, windowingService, moduleService, playbackService);
                singleViewPlatform.MainView = mainView;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private Window? GetMainWindow() =>
            (ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

        private void DisableAvaloniaDataAnnotationValidation()
        {
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}