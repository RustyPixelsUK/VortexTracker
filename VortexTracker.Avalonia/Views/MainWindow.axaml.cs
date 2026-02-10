using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using VortexTracker.Avalonia.ViewModels;
using System;
using System.Linq;

namespace VortexTracker.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainViewModel ViewModel => (MainViewModel)DataContext!;
    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
        
        // Keyboard shortcuts
        KeyDown += OnWindowKeyDown;
    }
    
    // File Operations
    public async void OnLoadClick(object sender, RoutedEventArgs e)
    {
        var fileTypes = new FilePickerFileType[]
        {
            new("ProTracker 3") { Patterns = new[] { "*.pt3" } },
            new("ProTracker 2") { Patterns = new[] { "*.pt2" } },
            new("All Files") { Patterns = new[] { "*.*" } }
        };
        
        var options = new FilePickerOpenOptions
        {
            Title = "Open Tracker Module",
            AllowMultiple = false,
            FileTypeFilter = fileTypes
        };
        
        var result = await StorageProvider.OpenFilePickerAsync(options);
        
        if (result.Count > 0)
        {
            var file = result[0];
            var path = file.TryGetLocalPath();
            if (!string.IsNullOrEmpty(path))
            {
                ViewModel.LoadFile(path);
            }
        }
    }
    
    // Playback Controls
    public void OnPlayClick(object sender, RoutedEventArgs e)
    {
        ViewModel.Tracker.Play();
    }
    
    public void OnStopClick(object sender, RoutedEventArgs e)
    {
        ViewModel.Tracker.Stop();
    }
    
    // Position Navigation
    public void OnFirstPosClick(object sender, RoutedEventArgs e)
    {
        ViewModel.Tracker.CurrentPosition = 0;
    }
    
    public void OnPrevPosClick(object sender, RoutedEventArgs e)
    {
        ViewModel.Tracker.PrevPosition();
    }
    
    public void OnNextPosClick(object sender, RoutedEventArgs e)
    {
        ViewModel.Tracker.NextPosition();
    }
    
    public void OnLastPosClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel.Tracker.Module != null)
            ViewModel.Tracker.CurrentPosition = ViewModel.Tracker.PositionCount - 1;
    }
    
    // Sample/Ornament Navigation
    public void OnPrevSampleClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel.Tracker.CurrentSample > 0)
            ViewModel.Tracker.CurrentSample--;
    }
    
    public void OnNextSampleClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel.Tracker.Module != null && ViewModel.Tracker.CurrentSample < ViewModel.Tracker.Module.Samples.Length - 1)
            ViewModel.Tracker.CurrentSample++;
    }
    
    public void OnPrevOrnamentClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel.Tracker.CurrentOrnament > 0)
            ViewModel.Tracker.CurrentOrnament--;
    }
    
    public void OnNextOrnamentClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel.Tracker.Module != null && ViewModel.Tracker.CurrentOrnament < ViewModel.Tracker.Module.Ornaments.Length - 1)
            ViewModel.Tracker.CurrentOrnament++;
    }
    
    // Keyboard Shortcuts
    private void OnWindowKeyDown(object sender, KeyEventArgs e)
    {
        // Ctrl+O = Open
        if (e.KeyModifiers == KeyModifiers.Control && e.Key == Key.O)
        {
            OnLoadClick(this, new RoutedEventArgs());
            e.Handled = true;
        }
        
        // Space = Play/Stop
        if (e.Key == Key.Space)
        {
            if (ViewModel.Tracker.IsPlaying)
                OnStopClick(this, new RoutedEventArgs());
            else
                OnPlayClick(this, new RoutedEventArgs());
            e.Handled = true;
        }
        
        // Page Up/Down = Navigate positions
        if (e.Key == Key.PageUp)
        {
            ViewModel.Tracker.PrevPosition();
            e.Handled = true;
        }
        if (e.Key == Key.PageDown)
        {
            ViewModel.Tracker.NextPosition();
            e.Handled = true;
        }
        
        // Home/End = First/Last position
        if (e.KeyModifiers == KeyModifiers.Control && e.Key == Key.Home)
        {
            OnFirstPosClick(this, new RoutedEventArgs());
            e.Handled = true;
        }
        if (e.KeyModifiers == KeyModifiers.Control && e.Key == Key.End)
        {
            OnLastPosClick(this, new RoutedEventArgs());
            e.Handled = true;
        }
        
        // F1-F4 = Sample 1-4
        if (e.Key >= Key.F1 && e.Key <= Key.F4)
        {
            ViewModel.Tracker.CurrentSample = (int)(e.Key - Key.F1) + 1;
            e.Handled = true;
        }
    }
}
