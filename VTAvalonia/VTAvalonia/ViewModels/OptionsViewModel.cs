using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using VortexTracker.Core;
using VortexTracker.Core.Services;
using VTAvalonia.Services;

namespace VTAvalonia.ViewModels;

public sealed class OptionsViewModel : ViewModelBase
{
    // ── Static option lists ───────────────────────────────────────────────────

    private static readonly (string Label, int Hz)[] s_chipFreqs =
    [
        ("894 887 Hz – Atari ST / MSX 50 Hz",  894887),
        ("831 303 Hz – Atari ST / MSX 60 Hz",  831303),
        ("1 773 400 Hz – ZX 128 PAL",          1773400),
        ("1 750 000 Hz – ZX 128 / Pentagon",   1750000),
        ("1 000 000 Hz",                       1000000),
        ("1 500 000 Hz",                       1500000),
        ("2 000 000 Hz",                       2000000),
        ("3 500 000 Hz",                       3500000),
        ("1 520 640 Hz",                       1520640),
        ("1 611 062 Hz",                       1611062),
        ("1 706 861 Hz",                       1706861),
        ("1 808 356 Hz",                       1808356),
        ("1 915 886 Hz",                       1915886),
        ("2 029 811 Hz",                       2029811),
        ("2 150 510 Hz",                       2150510),
        ("2 278 386 Hz",                       2278386),
        ("2 413 866 Hz",                       2413866),
        ("2 557 401 Hz",                       2557401),
        ("2 709 472 Hz",                       2709472),
        ("2 870 586 Hz",                       2870586),
        ("3 041 280 Hz",                       3041280),
        ("Custom…",                            0),
    ];

    private static readonly (string Label, int Hz)[] s_intFreqs =
    [
        ("48 828 Hz – ZX Spectrum", 48828),
        ("50 000 Hz – 50 Hz",       50000),
        ("60 000 Hz – 60 Hz",       60000),
        ("100 000 Hz",              100000),
        ("200 000 Hz",              200000),
        ("48 000 Hz",               48000),
        ("Custom…",                 0),
    ];

    private static readonly int[] s_sampleRateValues  = [11025, 22050, 44100, 48000, 88200, 96000, 192000];
    private static readonly int[] s_sampleBitValues   = [8, 16, 24, 32];
    private static readonly int[] s_channelValues     = [1, 2];

    private const string DefaultAudioDevice = "Default Device";
    private const string NoMidiDevice  = "(None)";
    private const string NoSerialPort  = "(None)";

    // Exposed to XAML via binding
    public string[] SampleRateOptions  { get; } = ["11 025 Hz", "22 050 Hz", "44 100 Hz", "48 000 Hz", "88 200 Hz", "96 000 Hz", "192 000 Hz"];
    public string[] SampleBitOptions   { get; } = ["8-bit", "16-bit", "24-bit", "32-bit"];
    public string[] ChannelsOptions    { get; } = ["Mono", "Stereo"];
    public string[] ChipTypeOptions    { get; } = ["AY-3-8910", "YM2149F"];
    public string[] RenderEngineOptions{ get; } = ["Legacy", "Legacy + panning", "Ayumi"];
    public string[] DcTypeOptions      { get; } = ["None", "FIR low-pass", "IIR high-pass"];
    public string[] FeaturesLevelOptions { get; } = ["PT 3.5 and earlier", "Vortex Tracker II / PT 3.6", "PT 3.7", "Auto-detect"];
    public string[] ModuleHeaderOptions{ get; } = ["Vortex Tracker header", "Pro Tracker header", "Auto-detect"];
    public string[] NoteTableOptions   { get; } = ["ProTracker 3.3", "Sound Tracker", "ASM / PSC", "RealSound", "Natural", "Custom"];
    public string[] LoopModeOptions    { get; } = ["No looping", "Loop current module", "Loop all modules"];
    public string[] ChipFreqOptions    { get; } = Array.ConvertAll(s_chipFreqs, x => x.Label);
    public string[] IntFreqOptions     { get; } = Array.ConvertAll(s_intFreqs, x => x.Label);

    // ── Dependencies ─────────────────────────────────────────────────────────

    private readonly AppSettingsManager? _settingsManager;
    private readonly IAudioOutputService? _audioOutput;
    private readonly IMidiService? _midi;
    private readonly ISerialPortService? _serialPort;

    // ── Events / commands ─────────────────────────────────────────────────────

    public event Action? CloseRequested;

    public IRelayCommand SaveCommand                { get; }
    public IRelayCommand CancelCommand              { get; }
    public IRelayCommand RefreshAudioDevicesCommand { get; }
    public IRelayCommand RefreshMidiDevicesCommand  { get; }
    public IRelayCommand RefreshSerialPortsCommand  { get; }

    // ── Dynamic device / port lists ───────────────────────────────────────────

    public ObservableCollection<string> AudioDevices { get; } = [];
    public ObservableCollection<string> MidiDevices  { get; } = [];
    public ObservableCollection<string> SerialPorts  { get; } = [];

    // ── Audio ─────────────────────────────────────────────────────────────────

    private string _selectedAudioDevice = DefaultAudioDevice;
    public string SelectedAudioDevice
    {
        get => _selectedAudioDevice;
        set => SetProperty(ref _selectedAudioDevice, value);
    }

    private int _sampleRateIndex = 2;
    public int SampleRateIndex
    {
        get => _sampleRateIndex;
        set => SetProperty(ref _sampleRateIndex, value);
    }

    private int _sampleBitIndex = 1;
    public int SampleBitIndex
    {
        get => _sampleBitIndex;
        set => SetProperty(ref _sampleBitIndex, value);
    }

    private int _channelsIndex = 1;
    public int ChannelsIndex
    {
        get => _channelsIndex;
        set => SetProperty(ref _channelsIndex, value);
    }

    private int _bufferLengthMs = 100;
    public int BufferLengthMs
    {
        get => _bufferLengthMs;
        set => SetProperty(ref _bufferLengthMs, value);
    }

    private int _bufferCount = 3;
    public int BufferCount
    {
        get => _bufferCount;
        set => SetProperty(ref _bufferCount, value);
    }

    // ── Chip ──────────────────────────────────────────────────────────────────

    private int _chipTypeIndex = 1;
    public int ChipTypeIndex
    {
        get => _chipTypeIndex;
        set => SetProperty(ref _chipTypeIndex, value);
    }

    private int _renderEngineIndex = 2;
    public int RenderEngineIndex
    {
        get => _renderEngineIndex;
        set => SetProperty(ref _renderEngineIndex, value);
    }

    private int _chipFreqIndex = 3;
    public int ChipFreqIndex
    {
        get => _chipFreqIndex;
        set
        {
            if (SetProperty(ref _chipFreqIndex, value))
                OnPropertyChanged(nameof(IsCustomChipFreq));
        }
    }

    private int _customChipFreq = 1_750_000;
    public int CustomChipFreq
    {
        get => _customChipFreq;
        set => SetProperty(ref _customChipFreq, value);
    }

    public bool IsCustomChipFreq => _chipFreqIndex == s_chipFreqs.Length - 1;

    private int _intFreqIndex = 0;
    public int IntFreqIndex
    {
        get => _intFreqIndex;
        set
        {
            if (SetProperty(ref _intFreqIndex, value))
                OnPropertyChanged(nameof(IsCustomIntFreq));
        }
    }

    private int _customIntFreq = 48_828;
    public int CustomIntFreq
    {
        get => _customIntFreq;
        set => SetProperty(ref _customIntFreq, value);
    }

    public bool IsCustomIntFreq => _intFreqIndex == s_intFreqs.Length - 1;

    private int _dcTypeIndex = 1;
    public int DcTypeIndex
    {
        get => _dcTypeIndex;
        set => SetProperty(ref _dcTypeIndex, value);
    }

    private int _dcCutOff = 3;
    public int DcCutOff
    {
        get => _dcCutOff;
        set => SetProperty(ref _dcCutOff, value);
    }

    private bool _filteringEnabled = true;
    public bool FilteringEnabled
    {
        get => _filteringEnabled;
        set => SetProperty(ref _filteringEnabled, value);
    }

    private int _filterLength = 64;
    public int FilterLength
    {
        get => _filterLength;
        set => SetProperty(ref _filterLength, value);
    }

    // ── Module ────────────────────────────────────────────────────────────────

    private int _featuresLevelIndex = 3;
    public int FeaturesLevelIndex
    {
        get => _featuresLevelIndex;
        set => SetProperty(ref _featuresLevelIndex, value);
    }

    private int _moduleHeaderIndex = 2;
    public int ModuleHeaderIndex
    {
        get => _moduleHeaderIndex;
        set => SetProperty(ref _moduleHeaderIndex, value);
    }

    private int _defaultNoteTableIndex = 2;
    public int DefaultNoteTableIndex
    {
        get => _defaultNoteTableIndex;
        set => SetProperty(ref _defaultNoteTableIndex, value);
    }

    // ── MIDI ──────────────────────────────────────────────────────────────────

    private string _selectedMidiDevice = NoMidiDevice;
    public string SelectedMidiDevice
    {
        get => _selectedMidiDevice;
        set => SetProperty(ref _selectedMidiDevice, value);
    }

    // ── Serial ────────────────────────────────────────────────────────────────

    private string _selectedSerialPort = NoSerialPort;
    public string SelectedSerialPort
    {
        get => _selectedSerialPort;
        set => SetProperty(ref _selectedSerialPort, value);
    }

    private int _serialBaudRate = 2_000_000;
    public int SerialBaudRate
    {
        get => _serialBaudRate;
        set => SetProperty(ref _serialBaudRate, value);
    }

    // ── Playback ──────────────────────────────────────────────────────────────

    private int _loopModeIndex = 1;
    public int LoopModeIndex
    {
        get => _loopModeIndex;
        set => SetProperty(ref _loopModeIndex, value);
    }

    private int _globalVolume = 56;
    public int GlobalVolume
    {
        get => _globalVolume;
        set => SetProperty(ref _globalVolume, value);
    }

    // ── Constructors ──────────────────────────────────────────────────────────

    public OptionsViewModel() : this(null, null, null, null) { }

    public OptionsViewModel(
        AppSettingsManager? settingsManager,
        IAudioOutputService? audioOutput,
        IMidiService? midi,
        ISerialPortService? serialPort)
    {
        _settingsManager = settingsManager;
        _audioOutput     = audioOutput;
        _midi            = midi;
        _serialPort      = serialPort;

        SaveCommand                = new RelayCommand(Save);
        CancelCommand              = new RelayCommand(Cancel);
        RefreshAudioDevicesCommand = new RelayCommand(RefreshAudioDevices);
        RefreshMidiDevicesCommand  = new RelayCommand(RefreshMidiDevices);
        RefreshSerialPortsCommand  = new RelayCommand(RefreshSerialPorts);

        if (settingsManager is not null)
            LoadFromSettings(settingsManager.Current);

        RefreshAudioDevices();
        RefreshMidiDevices();
        RefreshSerialPorts();
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void LoadFromSettings(AppSettings s)
    {
        // Audio
        _selectedAudioDevice = string.IsNullOrEmpty(s.AudioOutputDevice) ? DefaultAudioDevice : s.AudioOutputDevice;
        _sampleRateIndex     = IndexOf(s_sampleRateValues, s.SampleRate, 2);
        _sampleBitIndex      = IndexOf(s_sampleBitValues,  s.SampleBit,  1);
        _channelsIndex       = s.NumberOfChannels == 2 ? 1 : 0;
        _bufferLengthMs      = s.BufferLengthMs;
        _bufferCount         = s.BufferCount;

        // Chip
        _chipTypeIndex     = s.ChipType == 1 ? 0 : 1;
        _renderEngineIndex = Math.Clamp(s.RenderEngine, 0, 2);
        _chipFreqIndex     = FreqToChipIndex(s.DefaultChipFreq);
        _customChipFreq    = s.DefaultChipFreq;
        _intFreqIndex      = FreqToIntIndex(s.DefaultIntFreq);
        _customIntFreq     = s.DefaultIntFreq;
        _dcTypeIndex       = Math.Clamp(s.DCType, 0, 2);
        _dcCutOff          = Math.Clamp(s.DCCutOff, 3, 10);
        _filteringEnabled  = s.FilteringEnabled;
        _filterLength      = s.FilterLength;

        // Module
        _featuresLevelIndex  = s.DetectFeaturesLevel ? 3 : Math.Clamp(s.FeaturesLevel, 0, 2);
        _moduleHeaderIndex   = s.DetectModuleHeader ? 2 : (s.VortexModuleHeader ? 0 : 1);
        _defaultNoteTableIndex = Math.Clamp(s.DefaultNoteTable, 0, 5);

        // MIDI / Serial
        _selectedMidiDevice = string.IsNullOrEmpty(s.MidiOutputDevice) ? NoMidiDevice : s.MidiOutputDevice;
        _selectedSerialPort = string.IsNullOrEmpty(s.SerialPortName)   ? NoSerialPort  : s.SerialPortName;
        _serialBaudRate     = s.SerialBaudRate;

        // Playback
        _loopModeIndex = Math.Clamp(s.LoopMode, 0, 2);
        _globalVolume  = s.GlobalVolume;
    }

    private AppSettings ToSettings()
    {
        // Clone current settings so unedited fields (e.g. window position) are preserved.
        var s = _settingsManager?.Current ?? new AppSettings();

        s.AudioOutputDevice = _selectedAudioDevice == DefaultAudioDevice ? null : _selectedAudioDevice;
        s.SampleRate        = ValueAt(s_sampleRateValues, _sampleRateIndex, 44100);
        s.SampleBit         = ValueAt(s_sampleBitValues,  _sampleBitIndex,  16);
        s.NumberOfChannels  = ValueAt(s_channelValues,    _channelsIndex,   2);
        s.BufferLengthMs    = _bufferLengthMs;
        s.BufferCount       = _bufferCount;

        s.ChipType        = _chipTypeIndex == 0 ? 1 : 2;
        s.RenderEngine    = _renderEngineIndex;
        s.DefaultChipFreq = ChipIndexToFreq(_chipFreqIndex, _customChipFreq);
        s.DefaultIntFreq  = IntIndexToFreq(_intFreqIndex, _customIntFreq);
        s.DCType          = _dcTypeIndex;
        s.DCCutOff        = _dcCutOff;
        s.FilteringEnabled = _filteringEnabled;
        s.FilterLength    = _filterLength;

        s.DetectFeaturesLevel = _featuresLevelIndex == 3;
        s.FeaturesLevel       = _featuresLevelIndex < 3 ? _featuresLevelIndex : 3;
        s.DetectModuleHeader  = _moduleHeaderIndex == 2;
        s.VortexModuleHeader  = _moduleHeaderIndex == 0;
        s.DefaultNoteTable    = _defaultNoteTableIndex;

        s.MidiOutputDevice = _selectedMidiDevice == NoMidiDevice ? null : _selectedMidiDevice;
        s.SerialPortName   = _selectedSerialPort  == NoSerialPort  ? null : _selectedSerialPort;
        s.SerialBaudRate   = _serialBaudRate;

        s.LoopMode    = _loopModeIndex;
        s.GlobalVolume = _globalVolume;

        return s;
    }

    private void Save()
    {
        _settingsManager?.Save(ToSettings());
        CloseRequested?.Invoke();
    }

    private void Cancel() => CloseRequested?.Invoke();

    private void RefreshAudioDevices()
    {
        var current = _selectedAudioDevice;
        AudioDevices.Clear();
        AudioDevices.Add(DefaultAudioDevice);
        if (_audioOutput is not null)
            foreach (var d in _audioOutput.GetOutputDevices())
                AudioDevices.Add(d);
        SelectedAudioDevice = AudioDevices.Contains(current) ? current : DefaultAudioDevice;
    }

    private void RefreshMidiDevices()
    {
        var current = _selectedMidiDevice;
        MidiDevices.Clear();
        MidiDevices.Add(NoMidiDevice);
        if (_midi is not null)
            foreach (var d in _midi.GetOutputDevices())
                MidiDevices.Add(d);
        SelectedMidiDevice = MidiDevices.Contains(current) ? current : NoMidiDevice;
    }

    private void RefreshSerialPorts()
    {
        var current = _selectedSerialPort;
        SerialPorts.Clear();
        SerialPorts.Add(NoSerialPort);
        if (_serialPort is not null)
            foreach (var p in _serialPort.GetAvailablePorts())
                SerialPorts.Add(p);
        SelectedSerialPort = SerialPorts.Contains(current) ? current : NoSerialPort;
    }

    // ── Frequency lookup helpers ──────────────────────────────────────────────

    private static int FreqToChipIndex(int freq)
    {
        for (int i = 0; i < s_chipFreqs.Length - 1; i++)
            if (s_chipFreqs[i].Hz == freq) return i;
        return s_chipFreqs.Length - 1;
    }

    private static int ChipIndexToFreq(int index, int custom)
        => index >= 0 && index < s_chipFreqs.Length - 1 ? s_chipFreqs[index].Hz : custom;

    private static int FreqToIntIndex(int freq)
    {
        for (int i = 0; i < s_intFreqs.Length - 1; i++)
            if (s_intFreqs[i].Hz == freq) return i;
        return s_intFreqs.Length - 1;
    }

    private static int IntIndexToFreq(int index, int custom)
        => index >= 0 && index < s_intFreqs.Length - 1 ? s_intFreqs[index].Hz : custom;

    private static int IndexOf(int[] arr, int value, int fallback)
    {
        int i = Array.IndexOf(arr, value);
        return i < 0 ? fallback : i;
    }

    private static int ValueAt(int[] arr, int index, int fallback)
        => index >= 0 && index < arr.Length ? arr[index] : fallback;
}

