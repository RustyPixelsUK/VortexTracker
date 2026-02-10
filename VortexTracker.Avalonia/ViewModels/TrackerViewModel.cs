using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using LibVT;
using VortexTracker.Avalonia.Audio;

namespace VortexTracker.Avalonia.ViewModels;

public class TrackerViewModel : ViewModelBase, IDisposable
{
    private VTM? _module;
    private int _currentPosition;
    private int _currentPattern;
    private bool _isPlaying;
    private string _statusText = "Ready";
    private AudioEngineStub? _audioEngine;
    
    public VTM? Module
    {
        get => _module;
        set
        {
            _module = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasModule));
            RefreshAll();
            
            // Load module into audio engine
            if (_audioEngine != null && _module != null)
            {
                _audioEngine.LoadModule(_module);
            }
        }
    }
    
    public bool HasModule => _module != null;
    
    public int CurrentPosition
    {
        get => _currentPosition;
        set
        {
            if (_currentPosition != value && _module != null && value >= 0 && value < _module.Positions.Length)
            {
                _currentPosition = value;
                OnPropertyChanged();
                CurrentPattern = _module.Positions.Value[value];
            }
        }
    }
    
    public int CurrentPattern
    {
        get => _currentPattern;
        set
        {
            if (_currentPattern != value)
            {
                _currentPattern = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PatternLines));
                OnPropertyChanged(nameof(CurrentPatternInfo));
            }
        }
    }
    
    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            _isPlaying = value;
            OnPropertyChanged();
            StatusText = value ? "Playing..." : "Stopped";
        }
    }
    
    public string StatusText
    {
        get => _statusText;
        set
        {
            _statusText = value;
            OnPropertyChanged();
        }
    }
    
    // Module Info
    public string Title => _module?.Title ?? "No Module";
    public string Author => _module?.Author ?? "";
    public int PositionCount => _module?.Positions.Length ?? 0;
    public int PatternCount => _module?.Patterns.Count(p => p != null) ?? 0;
    public string CurrentPatternInfo => $"Pattern {_currentPattern:X2} (Length: {CurrentPatternLength})";
    public int CurrentPatternLength => _module?.Patterns[_currentPattern]?.Length ?? 0;
    
    // Position List
    public ObservableCollection<PositionViewModel> Positions { get; } = new();
    
    // Pattern Lines
    public ObservableCollection<PatternLineViewModel> PatternLines { get; } = new();
    
    // Sample/Ornament Info
    private int _currentSample;
    private int _currentOrnament;
    
    public int CurrentSample
    {
        get => _currentSample;
        set
        {
            _currentSample = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SampleLines));
        }
    }
    
    public int CurrentOrnament
    {
        get => _currentOrnament;
        set
        {
            _currentOrnament = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(OrnamentLines));
        }
    }
    
    public ObservableCollection<SampleLineViewModel> SampleLines { get; } = new();
    public ObservableCollection<OrnamentLineViewModel> OrnamentLines { get; } = new();
    
    private void RefreshAll()
    {
        if (_module == null) return;
        
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(Author));
        OnPropertyChanged(nameof(PositionCount));
        OnPropertyChanged(nameof(PatternCount));
        
        RefreshPositions();
        RefreshPattern();
        RefreshSample();
        RefreshOrnament();
    }
    
    private void RefreshPositions()
    {
        Positions.Clear();
        if (_module == null) return;
        
        for (int i = 0; i < _module.Positions.Length; i++)
        {
            Positions.Add(new PositionViewModel
            {
                Index = i,
                PatternNumber = _module.Positions.Value[i],
                IsLoop = i == _module.Positions.Loop,
                IsSelected = i == _currentPosition
            });
        }
    }
    
    private void RefreshPattern()
    {
        PatternLines.Clear();
        if (_module == null || _currentPattern < 0) return;
        
        var pattern = _module.Patterns[_currentPattern];
        if (pattern == null) return;
        
        for (int line = 0; line < pattern.Length; line++)
        {
            PatternLines.Add(new PatternLineViewModel
            {
                LineNumber = line,
                Envelope = pattern.Lines[line].Envelope,
                Noise = pattern.Lines[line].Noise,
                ChannelA = FormatChannel(pattern.Lines[line].Channel[0]),
                ChannelB = FormatChannel(pattern.Lines[line].Channel[1]),
                ChannelC = FormatChannel(pattern.Lines[line].Channel[2])
            });
        }
    }
    
    private void RefreshSample()
    {
        SampleLines.Clear();
        if (_module == null || _currentSample < 0 || _currentSample >= _module.Samples.Length) return;
        
        var sample = _module.Samples[_currentSample];
        if (sample == null) return;
        
        for (int i = 0; i < sample.Length; i++)
        {
            SampleLines.Add(new SampleLineViewModel
            {
                LineNumber = i,
                ToneMask = sample.Ticks[i].Mixer_Ton,
                NoiseMask = sample.Ticks[i].Mixer_Noise,
                Tone = sample.Ticks[i].AddToTone,
                Noise = sample.Ticks[i].Add_to_Envelope_or_Noise,
                Amplitude = sample.Ticks[i].Amplitude
            });
        }
    }
    
    private void RefreshOrnament()
    {
        OrnamentLines.Clear();
        if (_module == null || _currentOrnament < 0 || _currentOrnament >= _module.Ornaments.Length) return;
        
        var ornament = _module.Ornaments[_currentOrnament];
        if (ornament == null) return;
        
        for (int i = 0; i < ornament.Length; i++)
        {
            OrnamentLines.Add(new OrnamentLineViewModel
            {
                LineNumber = i,
                Note = ornament.Offsets[i]
            });
        }
    }
    
    private string FormatChannel(ChannelLine ch)
    {
        string note = VTModule.NoteToStr(ch.Note);
        string sample = VTModule.SampToStr(ch.Sample);
        string envelope = VTModule.Int1DToStr(ch.Envelope);
        string ornament = VTModule.Int1DToStr(ch.Ornament);
        string volume = VTModule.Int1DToStr(ch.Volume);
        string cmd = VTModule.Int1DToStr(ch.AdditionalCommand.Number);
        string delay = VTModule.Int1DToStr(ch.AdditionalCommand.Delay);
        string param = VTModule.Int2DToStr(ch.AdditionalCommand.Parameter);
        
        return $"{note} {sample}{envelope}{ornament}{volume} {cmd}{delay}{param}";
    }
    
    public void NextPosition()
    {
        if (CurrentPosition < PositionCount - 1)
            CurrentPosition++;
    }
    
    public void PrevPosition()
    {
        if (CurrentPosition > 0)
            CurrentPosition--;
    }
    
    public void NextPattern()
    {
        if (_module != null && _currentPattern < 255)
            CurrentPattern++;
    }
    
    public void PrevPattern()
    {
        if (_currentPattern > 0)
            CurrentPattern--;
    }
    
    public TrackerViewModel()
    {
        InitializeAudio();
    }
    
    private void InitializeAudio()
    {
        try
        {
            var backend = _audioEngine = new AudioEngineStub();
            _audioEngine.Initialize();
            _audioEngine.PositionChanged += OnAudioPositionChanged;
            StatusText = "Audio engine initialized (OpenAL)";
        }
        catch (Exception ex)
        {
            StatusText = $"Audio init failed: {ex.Message}";
        }
    }
    
    private void OnAudioPositionChanged(object? sender, PlaybackPositionEventArgs e)
    {
        // Update UI from audio thread
        global::Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (e.Position != _currentPosition)
            {
                CurrentPosition = e.Position;
            }
        });
    }
    
    public void Play()
    {
        if (_audioEngine == null || _module == null) return;
        
        _audioEngine.Play();
        IsPlaying = true;
        StatusText = "?? Playing...";
    }
    
    public void Stop()
    {
        if (_audioEngine == null) return;
        
        _audioEngine.Stop();
        IsPlaying = false;
        StatusText = "?? Stopped";
    }
    
    public void PlayFromPosition(int position)
    {
        if (_audioEngine == null || _module == null) return;
        
        _audioEngine.SetPosition(position);
        _audioEngine.Play();
        IsPlaying = true;
        StatusText = $"?? Playing from position {position:X2}";
    }
    
    public void Dispose()
    {
        _audioEngine?.Dispose();
    }
}

public class PositionViewModel
{
    public int Index { get; set; }
    public int PatternNumber { get; set; }
    public bool IsLoop { get; set; }
    public bool IsSelected { get; set; }
    public string DisplayText => $"{Index:X2}: {PatternNumber:X2}";
}

public class PatternLineViewModel
{
    public int LineNumber { get; set; }
    public int Envelope { get; set; }
    public int Noise { get; set; }
    public string ChannelA { get; set; } = "";
    public string ChannelB { get; set; } = "";
    public string ChannelC { get; set; } = "";
    
    public string LineNumberHex => $"{LineNumber:X2}";
    public string EnvelopeStr => VTModule.Int4DToStr(Envelope);
    public string NoiseStr => VTModule.Int2DToStr(Noise);
}

public class SampleLineViewModel
{
    public int LineNumber { get; set; }
    public bool ToneMask { get; set; }
    public bool NoiseMask { get; set; }
    public int Tone { get; set; }
    public int Noise { get; set; }
    public int Amplitude { get; set; }
    
    public string LineNumberHex => $"{LineNumber:X2}";
    public string ToneStr => ToneMask ? "T" : "-";
    public string NoiseStr => NoiseMask ? "N" : "-";
    public string ToneValue => Tone.ToString("X3");
    public string NoiseValue => Noise.ToString("X2");
    public string AmplitudeValue => Amplitude.ToString("X2");
}

public class OrnamentLineViewModel
{
    public int LineNumber { get; set; }
    public sbyte Note { get; set; }
    
    public string LineNumberHex => $"{LineNumber:X2}";
    public string NoteStr => VTModule.NoteToStr(Note);
}





