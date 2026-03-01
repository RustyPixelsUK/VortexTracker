using System.Collections.Generic;
using System.IO.Ports;
using VortexTracker.Core.Services;

namespace VTAvalonia.Services;

public sealed class DotNetSerialPortService : ISerialPortService
{
    private SerialPort? _port;

    public IReadOnlyList<string> GetAvailablePorts()
        => SerialPort.GetPortNames();

    public void Open(string portName, int baudRate)
    {
        Close();
        _port = new SerialPort(portName, baudRate);
        _port.Open();
    }

    public void Close()
    {
        _port?.Dispose();
        _port = null;
    }
}
