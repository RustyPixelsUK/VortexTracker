using System;
using LibVT;

namespace VortexTracker.Avalonia.Audio;

/// <summary>
/// Simplified audio engine stub - demonstrates cross-platform audio integration point
/// Full integration requires WaveOutAPI.cs adaptation which is complex
/// </summary>
public class AudioEngineStub : IDisposable
{
    private VTM? _module;
    private bool _isPlaying;
    
    public bool IsPlaying => _isPlaying;
    public int CurrentPosition { get; private set; }
    public int CurrentLine { get; private set; }
    
    public event EventHandler<PlaybackPositionEventArgs>? PositionChanged;

    public void Initialize()
    {
        System.Diagnostics.Debug.WriteLine("Audio engine stub initialized");
    }

    public void LoadModule(VTM module)
    {
        _module = module;
        CurrentPosition = 0;
        CurrentLine = 0;
        System.Diagnostics.Debug.WriteLine($"Module loaded: {module?.Title}");
    }

    public void Play()
    {
        if (_module == null) return;
        _isPlaying = true;
        System.Diagnostics.Debug.WriteLine("?? Playback started (stub - no audio yet)");
        
        // TODO: Integrate WaveOutAPI or create simplified AY emulator
        // For now, just demonstrate the interface works
    }

    public void Stop()
    {
        _isPlaying = false;
        System.Diagnostics.Debug.WriteLine("?? Playback stopped");
    }

    public void SetPosition(int position)
    {
        if (_module == null) return;
        CurrentPosition = position;
        CurrentLine = 0;
        PositionChanged?.Invoke(this, new PlaybackPositionEventArgs(position, 0));
    }

    public void Dispose()
    {
        Stop();
    }
}

public class PlaybackPositionEventArgs : EventArgs
{
    public int Position { get; }
    public int Line { get; }

    public PlaybackPositionEventArgs(int position, int line)
    {
        Position = position;
        Line = line;
    }
}
