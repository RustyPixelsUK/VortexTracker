using LibVT;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using VortexTracker.Core.Services;

namespace VTAvalonia.Services;

public sealed class OpenAlAudioOutputService : IAudioOutputService
{
    public IReadOnlyList<string> GetOutputDevices()
    {
        try
        {
            return ALC.GetStringList(GetEnumerationStringList.DeviceSpecifier).ToList();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    public void SetOutputDevice(string? deviceName)
    {
        WaveOutAPI.WODevice = deviceName;
    }
}
