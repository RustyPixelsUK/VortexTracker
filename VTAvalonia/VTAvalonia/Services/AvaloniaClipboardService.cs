using Avalonia.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using VortexTracker.Core.Services;

namespace VTAvalonia.Services;

public sealed class AvaloniaClipboardService : IClipboardService
{
    private readonly Func<TopLevel?> _topLevelProvider;

    public AvaloniaClipboardService(Func<TopLevel?> topLevelProvider)
    {
        _topLevelProvider = topLevelProvider;
    }

    public async Task SetTextAsync(string text, CancellationToken cancellationToken = default)
    {
        var clipboard = _topLevelProvider()?.Clipboard;
        if (clipboard == null)
            return;

        await clipboard.SetTextAsync(text);
    }

    public async Task<string?> GetTextAsync(CancellationToken cancellationToken = default)
    {
        var clipboard = _topLevelProvider()?.Clipboard;
        if (clipboard == null)
            return null;

        return await clipboard.GetTextAsync();
    }
}
