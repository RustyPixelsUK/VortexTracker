namespace VortexTracker.Core.Services;

public interface IPlaybackService
{
    bool IsPlaying { get; }
    event EventHandler<bool>? PlaybackStateChanged;
    Task<bool> PlayFromStartAsync(CancellationToken cancellationToken = default);
    Task<bool> PlayPatternFromStartAsync(int patternIndex, CancellationToken cancellationToken = default);
    Task<bool> PlayPatternFromLineAsync(int patternIndex, int lineIndex, CancellationToken cancellationToken = default);
    Task<bool> PlayLineAsync(int patternIndex, int lineIndex, CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}
