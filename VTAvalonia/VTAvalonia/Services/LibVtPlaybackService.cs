using Avalonia.Threading;
using LibVT;
using System;
using System.Threading;
using System.Threading.Tasks;
using VortexTracker.Core.Services;

namespace VTAvalonia.Services;

public sealed class LibVtPlaybackService : IPlaybackService
{
    private readonly LibVtModuleService _moduleService;
    private bool _initialized;

    public LibVtPlaybackService(LibVtModuleService moduleService)
    {
        _moduleService = moduleService;

        // Fires when the WO thread exits for any reason (natural end or Stop).
        // Always dispatch to the UI thread so ViewModel property setters are safe.
        WaveOutAPI.PlaybackEnded = () =>
            Dispatcher.UIThread.Post(() => PlaybackStateChanged?.Invoke(this, false));
    }

    public bool IsPlaying => WaveOutAPI.IsPlaying;
    public event EventHandler<bool>? PlaybackStateChanged;

    public Task<bool> PlayFromStartAsync(CancellationToken cancellationToken = default)
    {
        if (!_moduleService.TryGetLoadedModules(out var vtm, out var vtm2, out var vtm3, out var chipCount))
            return Task.FromResult(false);

        EnsureInitialized();

        WaveOutAPI.EnqueuePlayModule(vtm!, vtm2, vtm3, chipCount, _moduleService.LoopingEnabled);
        PlaybackStateChanged?.Invoke(this, true);
        return Task.FromResult(true);
    }

    public Task<bool> PlayPatternFromStartAsync(int patternIndex, CancellationToken cancellationToken = default)
    {
        if (!_moduleService.TryGetLoadedModules(out var vtm, out var vtm2, out var vtm3, out var chipCount))
            return Task.FromResult(false);

        if (vtm == null || patternIndex < 0 || patternIndex >= vtm.Patterns.Length)
            return Task.FromResult(false);

        EnsureInitialized();

        WaveOutAPI.EnqueuePlayPattern(vtm, vtm2, vtm3, chipCount, _moduleService.LoopingEnabled, patternIndex);
        PlaybackStateChanged?.Invoke(this, true);
        return Task.FromResult(true);
    }

    public Task<bool> PlayPatternFromLineAsync(int patternIndex, int lineIndex, CancellationToken cancellationToken = default)
    {
        if (!_moduleService.TryGetLoadedModules(out var vtm, out var vtm2, out var vtm3, out var chipCount))
            return Task.FromResult(false);

        if (vtm == null || patternIndex < 0 || patternIndex >= vtm.Patterns.Length)
            return Task.FromResult(false);

        EnsureInitialized();

        WaveOutAPI.EnqueuePlayPatternLine(vtm, vtm2, vtm3, chipCount, _moduleService.LoopingEnabled, patternIndex, lineIndex);
        PlaybackStateChanged?.Invoke(this, true);
        return Task.FromResult(true);
    }

    public Task<bool> PlayLineAsync(int patternIndex, int lineIndex, CancellationToken cancellationToken = default)
    {
        if (!_moduleService.TryGetLoadedModules(out var vtm, out _, out _, out _))
            return Task.FromResult(false);

        if (vtm == null || patternIndex < 0 || patternIndex >= vtm.Patterns.Length)
            return Task.FromResult(false);

        var pattern = vtm.Patterns[patternIndex];
        if (pattern == null || lineIndex < 0 || lineIndex >= pattern.Length)
            return Task.FromResult(false);

        EnsureInitialized();

        WaveOutAPI.EnqueuePlayLine(vtm, patternIndex, lineIndex);
        PlaybackStateChanged?.Invoke(this, true);
        return Task.FromResult(true);
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        // StopPlaying joins the WO thread and then StopOpenAL tears down the context.
        // The WOThreadFunc finally block fires PlaybackEnded → PlaybackStateChanged(false).
        if (WaveOutAPI.IsPlaying)
            WaveOutAPI.StopPlaying();
        else
            PlaybackStateChanged?.Invoke(this, false);

        return Task.CompletedTask;
    }

    private void EnsureInitialized()
    {
        if (_initialized)
            return;

        WaveOutAPI.Initialize();
        _initialized = true;
    }
}
