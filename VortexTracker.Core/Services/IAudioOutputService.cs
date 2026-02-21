namespace VortexTracker.Core.Services;

public interface IAudioOutputService
{
    IReadOnlyList<string> GetOutputDevices();
    void SetOutputDevice(string? deviceName);
}
