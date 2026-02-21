using System;
using System.Threading;
using System.Threading.Tasks;
using VortexTracker.Core.Services;

namespace VTAvalonia.Services;

public sealed class NullPlaybackService : IPlaybackService
{
    public bool IsPlaying => false;
    public event EventHandler<bool>? PlaybackStateChanged;

    public Task<bool> PlayFromStartAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(false);

    public Task<bool> PlayPatternFromStartAsync(int patternIndex, CancellationToken cancellationToken = default)
        => Task.FromResult(false);

    public Task<bool> PlayPatternFromLineAsync(int patternIndex, int lineIndex, CancellationToken cancellationToken = default)
        => Task.FromResult(false);

    public Task<bool> PlayLineAsync(int patternIndex, int lineIndex, CancellationToken cancellationToken = default)
        => Task.FromResult(false);

    public Task StopAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
