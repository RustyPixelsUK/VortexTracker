namespace VortexTracker.Core.Services;

public interface IMidiService
{
    IReadOnlyList<string> GetOutputDevices();
    void SetOutputDevice(string? deviceName);
}
