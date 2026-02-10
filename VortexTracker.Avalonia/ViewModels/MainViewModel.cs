using System;
using System.Linq;
using LibVT;

namespace VortexTracker.Avalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string _title = "Vortex Tracker - Avalonia Cross-Platform";
    
    public string Title
    {
        get => _title;
        set { _title = value; OnPropertyChanged(); }
    }
    
    public TrackerViewModel Tracker { get; } = new();
    
    public MainViewModel()
    {
        LoadDemoModule();
    }
    
    private void LoadDemoModule()
    {
        var vtm = new VTM
        {
            Title = "Cross-Platform Demo",
            Author = "Avalonia PoC",
            ChipFreq = 1750000,
            IntFreq = 50,
            InitialDelay = 6
        };
        
        // Create demo pattern
        vtm.Patterns[0] = new Pattern { Length = 64 };
        vtm.Positions.Length = 4;
        vtm.Positions.Value[0] = 0;
        vtm.Positions.Value[1] = 0;
        vtm.Positions.Value[2] = 0;
        vtm.Positions.Value[3] = 0;
        vtm.Positions.Loop = 0;
        
        // Add melody
        int[] scale = { 0, 2, 4, 5, 7, 9, 11, 12 };
        for (int i = 0; i < 8; i++)
        {
            int line = i * 8;
            vtm.Patterns[0].Lines[line].Channel[0].Note = (sbyte)(36 + scale[i]);
            vtm.Patterns[0].Lines[line].Channel[0].Sample = 1;
            vtm.Patterns[0].Lines[line].Channel[0].Volume = 15;
            
            vtm.Patterns[0].Lines[line].Channel[1].Note = (sbyte)(36 + scale[i] + 4);
            vtm.Patterns[0].Lines[line].Channel[1].Sample = 1;
            vtm.Patterns[0].Lines[line].Channel[1].Volume = 12;
            
            if (i % 2 == 0)
            {
                vtm.Patterns[0].Lines[line].Channel[2].Note = (sbyte)(24 + scale[i / 2]);
                vtm.Patterns[0].Lines[line].Channel[2].Sample = 1;
                vtm.Patterns[0].Lines[line].Channel[2].Volume = 10;
            }
        }
        
        // Create simple sample
        vtm.Samples[1] = new Sample { Length = 4, Loop = 0 };
        vtm.Samples[1].Ticks[0].Amplitude = 15;
        vtm.Samples[1].Ticks[1].Amplitude = 12;
        vtm.Samples[1].Ticks[2].Amplitude = 8;
        vtm.Samples[1].Ticks[3].Amplitude = 4;
        
        // Create simple ornament
        vtm.Ornaments[0] = new Ornament { Length = 1, Loop = 0 };
        vtm.Ornaments[0].Offsets[0] = 0;
        
        Tracker.Module = vtm;
        Tracker.CurrentPosition = 0;
        Tracker.CurrentSample = 1;
        Tracker.CurrentOrnament = 0;
        
        Title = $"Vortex Tracker - {vtm.Title}";
        Tracker.StatusText = "Demo loaded - Load PT3 file to edit";
    }
    
    public void LoadFile(string filePath)
    {
        try
        {
            Tracker.StatusText = $"Loading {System.IO.Path.GetFileName(filePath)}...";
            
            var vtm = new VTM();
            var data = System.IO.File.ReadAllBytes(filePath);
            var fileType = ModuleType.PT3File;
            VTM dummy = null;
            
            WaveOutAPI.ConvertTrackerModule(filePath, data, fileType, 0, 0, 0, "", "", vtm, ref dummy, ref dummy);
            
            Tracker.Module = vtm;
            Tracker.CurrentPosition = 0;
            Tracker.CurrentSample = 1;
            Tracker.CurrentOrnament = 0;
            
            Title = $"Vortex Tracker - {vtm.Title}";
            
            int pc = vtm.Patterns.Count(p => p != null);
            Tracker.StatusText = $"? {vtm.Title} | {vtm.Author} | {vtm.Positions.Length} pos | {pc} pat";
        }
        catch (Exception ex)
        {
            Tracker.StatusText = $"? Error: {ex.Message}";
        }
    }
}



