using RtMidi.Net;
using RtMidi.Net.Enums;
using System;
using System.Collections.Generic;
using VortexTracker.Core.Services;

namespace VTAvalonia.Services;

public sealed class RtMidiMidiService : IMidiService
{
    private string? _selectedDeviceName;

    public IReadOnlyList<string> GetOutputDevices()
    {
        var results = new List<string>();
        try
        {
            var devices = MidiManager.GetAvailableDevices();
            foreach (var d in devices)
            {
                if (d.Type == MidiDeviceType.Output)
                    results.Add(d.Name);
            }
        }
        catch
        {
            // MIDI hardware unavailable on this machine
        }
        return results;
    }

    public void SetOutputDevice(string? deviceName)
    {
        _selectedDeviceName = deviceName;
    }
}
