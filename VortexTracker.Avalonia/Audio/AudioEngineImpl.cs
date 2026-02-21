using System;
using System.Threading;
using LibVT;

namespace VortexTracker.Avalonia.Audio;

/// <summary>
/// Audio playback engine using LibVT's WaveOutAPI and AY emulation
/// </summary>
public class AudioEngineImpl : IDisposable
{
    private VTM? _module;
    private bool _isPlaying;
    private int _currentPosition;
    private Timer? _positionTracker;
    
    public bool IsPlaying => _isPlaying && WaveOutAPI.IsPlaying;
    public int CurrentPosition => _currentPosition;
    
    public event EventHandler<PlaybackPositionEventArgs>? PositionChanged;

    public void Initialize()
    {
        // WaveOutAPI initializes itself when StartWOThread is called
        System.Diagnostics.Debug.WriteLine("Audio engine ready (using WaveOutAPI/OpenAL)");
    }

    public void LoadModule(VTM module)
    {
        bool wasPlaying = _isPlaying;
        if (wasPlaying)
            Stop();

        _module = module;
        _currentPosition = 0;
        
        if (_module != null)
        {
            System.Diagnostics.Debug.WriteLine($"Module loaded: {_module.Title}");
        }

        if (wasPlaying && _module != null)
            Play();
    }

    public void Play()
    {
        if (_module == null || _isPlaying)
            return;

        try
        {
            // Set up for playback (similar to MainForm.cs)
            AY.PlayMode = PlayModes.PlayModule;
            
            // Configure the module
            VTModule.Module_SetPointer(_module, 0); // chipIndex = 0
            VTModule.Module_SetDelay((sbyte)_module.InitialDelay);
            VTModule.Module_SetCurrentPosition(_currentPosition);
            
            // Initialize and start playback
            WaveOutAPI.InitForAllTypes(true); // true = play from module start
            WaveOutAPI.StartWOThread();
            
            _isPlaying = true;
            
            // Start position tracking
            _positionTracker = new Timer(TrackPosition, null, 0, 100); // Check every 100ms
            
            System.Diagnostics.Debug.WriteLine($"?? Playing from position {_currentPosition}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Play error: {ex.Message}");
            _isPlaying = false;
        }
    }

    public void Stop()
    {
        if (!_isPlaying)
            return;

        try
        {
            _positionTracker?.Dispose();
            _positionTracker = null;
            
            WaveOutAPI.StopPlaying();
            _isPlaying = false;
            
            System.Diagnostics.Debug.WriteLine("?? Stopped");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Stop error: {ex.Message}");
        }
    }

    public void SetPosition(int position)
    {
        if (_module == null || position < 0 || position >= _module.Positions.Length)
            return;

        bool wasPlaying = _isPlaying;
        if (wasPlaying)
            Stop();

        _currentPosition = position;
        PositionChanged?.Invoke(this, new PlaybackPositionEventArgs(position));

        if (wasPlaying)
            Play();
    }

    private void TrackPosition(object? state)
    {
        if (!_isPlaying || _module == null)
            return;

        try
        {
            // Get current position from VTModule playback state
            int pos = VTModule.Module_GetCurrentPosition(0); // chipIndex = 0
            
            if (pos != _currentPosition && pos >= 0 && pos < _module.Positions.Length)
            {
                _currentPosition = pos;
                PositionChanged?.Invoke(this, new PlaybackPositionEventArgs(pos));
            }
        }
        catch
        {
            // Ignore errors during position tracking
        }
    }

    public void Dispose()
    {
        _positionTracker?.Dispose();
        Stop();
        GC.SuppressFinalize(this);
    }
}

public class PlaybackPositionEventArgs : EventArgs
{
    public int Position { get; }

    public PlaybackPositionEventArgs(int position)
    {
        Position = position;
    }
}
