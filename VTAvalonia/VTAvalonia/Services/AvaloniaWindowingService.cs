using Avalonia.Controls;
using System;
using System.Threading.Tasks;
using VortexTracker.Core.Services;
using VTAvalonia.Views;

namespace VTAvalonia.Services;

public sealed class AvaloniaWindowingService : IWindowingService
{
    private readonly Func<TopLevel?> _topLevelProvider;

    public AvaloniaWindowingService(Func<TopLevel?> topLevelProvider)
    {
        _topLevelProvider = topLevelProvider;
    }

    public void ShowMainWindow()
    {
        if (_topLevelProvider() is Window window)
            window.Show();
    }

    public Task ShowDialogAsync(string viewModelKey, object? parameter = null)
    {
        if (_topLevelProvider() is not Window owner)
            return Task.CompletedTask;

        Window? dialog = viewModelKey switch
        {
            "About" => new AboutWindow(),
            "Options" => new OptionsWindow(),
            _ => null
        };

        return dialog != null ? dialog.ShowDialog(owner) : Task.CompletedTask;
    }

    public async Task<bool> ShowConfirmDialogAsync(string message, string title)
    {
        if (_topLevelProvider() is not Window owner)
            return true;

        var tcs = new System.Threading.Tasks.TaskCompletionSource<bool>();

        var dialog = new Window
        {
            Title = title,
            Width = 420,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        var stack = new Avalonia.Controls.StackPanel { Margin = new Avalonia.Thickness(16), Spacing = 16 };
        stack.Children.Add(new Avalonia.Controls.TextBlock
        {
            Text = message,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap
        });

        var btnRow = new Avalonia.Controls.StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
            Spacing = 8
        };

        var discardBtn = new Avalonia.Controls.Button { Content = "Discard" };
        var cancelBtn  = new Avalonia.Controls.Button { Content = "Cancel" };
        discardBtn.Click += (_, _) => { tcs.TrySetResult(true);  dialog.Close(); };
        cancelBtn.Click  += (_, _) => { tcs.TrySetResult(false); dialog.Close(); };
        btnRow.Children.Add(discardBtn);
        btnRow.Children.Add(cancelBtn);
        stack.Children.Add(btnRow);
        dialog.Content = stack;

        await dialog.ShowDialog(owner);
        return await tcs.Task;
    }

    public void ActivateMainWindow()
    {
        if (_topLevelProvider() is Window window)
        {
            window.Activate();
            window.BringIntoView();
        }
    }

    public void CloseMainWindow()
    {
        if (_topLevelProvider() is Window window)
            window.Close();
    }
}
