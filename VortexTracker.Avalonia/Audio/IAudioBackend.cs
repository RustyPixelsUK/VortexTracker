using System;

namespace VortexTracker.Avalonia.Audio;

/// <summary>
/// Platform-agnostic audio backend interface
/// </summary>
public interface IAudioBackend : IDisposable
{
    /// <summary>
    /// Initialize audio with given sample rate and channels
    /// </summary>
    void Initialize(int sampleRate, int channels, int bufferSize);
    
    /// <summary>
    /// Start audio playback
    /// </summary>
    void Start();
    
    /// <summary>
    /// Stop audio playback
    /// </summary>
    void Stop();
    
    /// <summary>
    /// Submit audio samples for playback
    /// </summary>
    void SubmitSamples(short[] samples, int count);
    
    /// <summary>
    /// Check if audio is currently playing
    /// </summary>
    bool IsPlaying { get; }
    
    /// <summary>
    /// Get number of samples queued for playback
    /// </summary>
    int QueuedSamples { get; }
}
