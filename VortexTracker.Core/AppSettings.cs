namespace VortexTracker.Core;

/// <summary>
/// Platform-neutral settings model. Mirrors the keys stored in Config.ini by the
/// WinForms host so both hosts share the same file on disk.
/// </summary>
public sealed class AppSettings
{
    // ── Audio output ──────────────────────────────────────────────────────────
    /// <summary>OpenAL output device name. null = system default.</summary>
    public string? AudioOutputDevice { get; set; }
    public int SampleRate { get; set; } = 44100;
    public int SampleBit { get; set; } = 16;
    public int NumberOfChannels { get; set; } = 2;
    public int BufferLengthMs { get; set; } = 100;
    public int BufferCount { get; set; } = 3;

    // ── AY/YM chip emulation ─────────────────────────────────────────────────
    /// <summary>1 = AY, 2 = YM (default).</summary>
    public int ChipType { get; set; } = 2;
    /// <summary>0 = legacy, 1 = panning, 2 = Ayumi (default).</summary>
    public int RenderEngine { get; set; } = 2;
    public int DCType { get; set; } = 1;
    public int DCCutOff { get; set; } = 3;
    public int DefaultChipFreq { get; set; } = 1_750_000;
    public int DefaultIntFreq { get; set; } = 48_828;
    public bool FilteringEnabled { get; set; } = true;
    /// <summary>FIR filter tap count (FilterQ in WinForms). Must be a power of two.</summary>
    public int FilterLength { get; set; } = 64;

    // ── Module interpretation ─────────────────────────────────────────────────
    /// <summary>Default note table index (0–5). Matches the ComboBox order in MainView.</summary>
    public int DefaultNoteTable { get; set; } = 2;
    /// <summary>Cast to LibVT.FeaturesLevel. 3 = AutoDetect (default).</summary>
    public int FeaturesLevel { get; set; } = 3;
    public bool DetectFeaturesLevel { get; set; } = true;
    public bool VortexModuleHeader { get; set; } = true;
    public bool DetectModuleHeader { get; set; } = true;

    // ── Playback ─────────────────────────────────────────────────────────────
    /// <summary>0 = off, 1 = loop current (default), 2 = loop all.</summary>
    public int LoopMode { get; set; } = 1;
    public int GlobalVolume { get; set; } = 56;

    // ── MIDI ─────────────────────────────────────────────────────────────────
    public string? MidiOutputDevice { get; set; }

    // ── Serial port ──────────────────────────────────────────────────────────
    public string? SerialPortName { get; set; }
    public int SerialBaudRate { get; set; } = 2_000_000;

    // ── Navigation ───────────────────────────────────────────────────────────
    public string LastDirectory { get; set; } = string.Empty;
    public string[] RecentFiles { get; set; } = new string[6];

    // ── Window state ─────────────────────────────────────────────────────────
    public int WindowX { get; set; } = -1;
    public int WindowY { get; set; } = -1;
    public int WindowWidth { get; set; } = -1;
    public int WindowHeight { get; set; } = -1;
    public bool WindowMaximized { get; set; }

    // ── Display (populated by OptionsViewModel in a later step) ──────────────
    public string ColorThemeName { get; set; } = "Default";
}
