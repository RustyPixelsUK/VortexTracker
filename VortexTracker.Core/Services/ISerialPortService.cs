namespace VortexTracker.Core.Services;

public interface ISerialPortService
{
    IReadOnlyList<string> GetAvailablePorts();
    void Open(string portName, int baudRate);
    void Close();
}
