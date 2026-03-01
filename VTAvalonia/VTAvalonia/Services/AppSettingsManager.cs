using LibVT;
using System.IO;
using VortexTracker.Core;
using VortexTracker.Core.Services;

namespace VTAvalonia.Services;

/// <summary>
/// Loads and saves <see cref="AppSettings"/> to/from the shared Config.ini file and
/// applies the values to the LibVT global statics (<see cref="WaveOutAPI"/>,
/// <see cref="AY"/>, <see cref="Main"/>).  The INI path mirrors the WinForms host so
/// both apps share the same config on disk.
/// </summary>
public sealed class AppSettingsManager
{
    private const string Section = "General";
    private const string FileName = "Config.ini";

    private readonly IPlatformPathsService _platformPaths;

    public AppSettings Current { get; private set; } = new();

    public AppSettingsManager(IPlatformPathsService platformPaths)
    {
        _platformPaths = platformPaths;
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Reads Config.ini (creating it if absent), populates <see cref="Current"/>,
    /// and applies the values to the LibVT statics so audio is configured before
    /// the first call to <see cref="WaveOutAPI.Initialize"/>.
    /// </summary>
    public AppSettings Load()
    {
        var ini = OpenIni();
        var s = new AppSettings();

        // Audio
        s.AudioOutputDevice   = ini.GetValue(Section, "WODevice",           (string?)null);
        s.SampleRate          = ini.GetValue(Section, "SampleRate",          44100);
        s.SampleBit           = ini.GetValue(Section, "SampleBit",           16);
        s.NumberOfChannels    = ini.GetValue(Section, "NumberOfChannels",    2);
        s.BufferLengthMs      = ini.GetValue(Section, "BufLen_ms",           100);
        s.BufferCount         = ini.GetValue(Section, "NumberOfBuffers",     3);

        // Chip emulation
        s.ChipType            = ini.GetValue(Section, "ChipType",            2);
        s.RenderEngine        = ini.GetValue(Section, "RenderEngine",        2);
        s.DCType              = ini.GetValue(Section, "DCType",              1);
        s.DCCutOff            = ini.GetValue(Section, "DCCutOff",            3);
        s.DefaultChipFreq     = ini.GetValue(Section, "DefaultChipFreq",     1_750_000);
        s.DefaultIntFreq      = ini.GetValue(Section, "DefaultIntFreq",      48_828);
        s.FilteringEnabled    = ini.GetValue(Section, "Filtering",           true);
        s.FilterLength        = ini.GetValue(Section, "FilterQ",             64);

        // Module interpretation
        s.DefaultNoteTable    = ini.GetValue(Section, "DefaultTable",        2);
        s.FeaturesLevel       = ini.GetValue(Section, "FeaturesLevel",       3);
        s.DetectFeaturesLevel = ini.GetValue(Section, "DetectFeaturesLevel", true);
        s.VortexModuleHeader  = ini.GetValue(Section, "VortexModuleHeader",  true);
        s.DetectModuleHeader  = ini.GetValue(Section, "DetectModuleHeader",  true);

        // Playback
        s.LoopMode            = ini.GetValue(Section, "LoopMode",            1);
        s.GlobalVolume        = ini.GetValue(Section, "GlobalVolume",        56);

        // MIDI / Serial
        s.MidiOutputDevice    = ini.GetValue(Section, "MidiOutputDevice",    (string?)null);
        s.SerialPortName      = ini.GetValue(Section, "SerialPortName",      (string?)null);
        s.SerialBaudRate      = ini.GetValue(Section, "SerialBaudRate",      2_000_000);

        // Navigation
        s.LastDirectory       = ini.GetValue(Section, "LastDirectory",       string.Empty);
        s.RecentFiles         = new string[6];
        for (int i = 0; i < 6; i++)
            s.RecentFiles[i]  = ini.GetValue(Section, $"Recent{i}",          string.Empty);

        // Window
        s.WindowX             = ini.GetValue(Section, "WindowX",             -1);
        s.WindowY             = ini.GetValue(Section, "WindowY",             -1);
        s.WindowWidth         = ini.GetValue(Section, "WindowWidth",         -1);
        s.WindowHeight        = ini.GetValue(Section, "WindowHeight",        -1);
        s.WindowMaximized     = ini.GetValue(Section, "WindowMaximized",     false);

        // Display
        s.ColorThemeName      = ini.GetValue(Section, "ColorThemeName",      "Default");

        Current = s;
        ApplyToStatics(s);
        return s;
    }

    /// <summary>
    /// Applies <paramref name="settings"/> to the LibVT statics and writes Config.ini.
    /// </summary>
    public void Save(AppSettings settings)
    {
        Current = settings;
        ApplyToStatics(settings);

        var ini = OpenIni();

        ini.SetValue(Section, "WODevice",            settings.AudioOutputDevice ?? string.Empty);
        ini.SetValue(Section, "SampleRate",          settings.SampleRate);
        ini.SetValue(Section, "SampleBit",           settings.SampleBit);
        ini.SetValue(Section, "NumberOfChannels",    settings.NumberOfChannels);
        ini.SetValue(Section, "BufLen_ms",           settings.BufferLengthMs);
        ini.SetValue(Section, "NumberOfBuffers",     settings.BufferCount);

        ini.SetValue(Section, "ChipType",            settings.ChipType);
        ini.SetValue(Section, "RenderEngine",        settings.RenderEngine);
        ini.SetValue(Section, "DCType",              settings.DCType);
        ini.SetValue(Section, "DCCutOff",            settings.DCCutOff);
        ini.SetValue(Section, "DefaultChipFreq",     settings.DefaultChipFreq);
        ini.SetValue(Section, "DefaultIntFreq",      settings.DefaultIntFreq);
        ini.SetValue(Section, "Filtering",           settings.FilteringEnabled);
        ini.SetValue(Section, "FilterQ",             settings.FilterLength);

        ini.SetValue(Section, "DefaultTable",        settings.DefaultNoteTable);
        ini.SetValue(Section, "FeaturesLevel",       settings.FeaturesLevel);
        ini.SetValue(Section, "DetectFeaturesLevel", settings.DetectFeaturesLevel);
        ini.SetValue(Section, "VortexModuleHeader",  settings.VortexModuleHeader);
        ini.SetValue(Section, "DetectModuleHeader",  settings.DetectModuleHeader);

        ini.SetValue(Section, "LoopMode",            settings.LoopMode);
        ini.SetValue(Section, "GlobalVolume",        settings.GlobalVolume);

        ini.SetValue(Section, "MidiOutputDevice",    settings.MidiOutputDevice ?? string.Empty);
        ini.SetValue(Section, "SerialPortName",      settings.SerialPortName ?? string.Empty);
        ini.SetValue(Section, "SerialBaudRate",      settings.SerialBaudRate);

        ini.SetValue(Section, "LastDirectory",       settings.LastDirectory);
        for (int i = 0; i < 6; i++)
            ini.SetValue(Section, $"Recent{i}", settings.RecentFiles.Length > i ? settings.RecentFiles[i] : string.Empty);

        ini.SetValue(Section, "WindowX",             settings.WindowX);
        ini.SetValue(Section, "WindowY",             settings.WindowY);
        ini.SetValue(Section, "WindowWidth",         settings.WindowWidth);
        ini.SetValue(Section, "WindowHeight",        settings.WindowHeight);
        ini.SetValue(Section, "WindowMaximized",     settings.WindowMaximized);

        ini.SetValue(Section, "ColorThemeName",      settings.ColorThemeName);

        ini.SetValue(Section, "ConfigInitialized",   true);

        ini.Save();
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private string IniPath => Path.Combine(_platformPaths.GetAppDataPath(), FileName);

    private IniFile OpenIni() => new IniFile(IniPath);

    private static void ApplyToStatics(AppSettings s)
    {
        // Audio
        WaveOutAPI.WODevice       = string.IsNullOrEmpty(s.AudioOutputDevice) ? null : s.AudioOutputDevice;
        WaveOutAPI.SampleRate     = s.SampleRate;
        WaveOutAPI.SampleBit      = s.SampleBit;
        WaveOutAPI.NumberOfChannels = s.NumberOfChannels;
        WaveOutAPI.BufferLengthMs = s.BufferLengthMs;
        WaveOutAPI.BufferCount    = s.BufferCount;

        // Chip / render
        AY.EmulatingChip = s.ChipType == 1 ? ChipType.AY : ChipType.YM;
        AY.Set_Engine(s.RenderEngine);
        AY.DCType   = s.DCType;
        AY.DCCutOff = s.DCCutOff;

        // Filter — only safe to call when not playing
        if (!WaveOutAPI.IsPlaying)
            AY.SetFilter(s.FilteringEnabled, s.FilterLength);

        // Chip/interrupt frequencies
        Main.DefaultChipFreq = s.DefaultChipFreq;
        Main.DefaultIntFreq  = s.DefaultIntFreq;

        // Module behaviour
        VTModule.FeaturesLevel       = (FeaturesLevel)s.FeaturesLevel;
        VTModule.DetectFeaturesLevel = s.DetectFeaturesLevel;
        VTModule.VortexModuleHeader  = s.VortexModuleHeader;
        VTModule.DetectModuleHeader  = s.DetectModuleHeader;

        // Playback / volume
        AY.LoopAllowed          = s.LoopMode == 1;
        Main.LoopAllAllowed     = s.LoopMode == 2;
        Main.GlobalVolume       = s.GlobalVolume;
    }
}
