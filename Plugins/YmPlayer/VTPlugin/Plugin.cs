using LibVT;
using LibVT.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace VTPlugin
{
    public class Plugin : IPlugin, IDisposable
    {
        private IHost _host = null;
        private MainForm _mainForm = null;
        private IniFile _iniFile = null;
        private SerialPort _serialPort = null;

        private readonly Channel<RegisterEventArgs> _frameCh =
        Channel.CreateBounded<RegisterEventArgs>(new BoundedChannelOptions(80)
        {
            SingleWriter = false,           // RegisterEvent may arrive on many threads
            SingleReader = true,            // one pump thread
            FullMode = BoundedChannelFullMode.Wait   // <- never blocks
        });

        private Thread? _pumpThread;
        private CancellationTokenSource? _pumpCts;

        private bool _disposed = false;

        public void Initialize(IHost host)
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string iniFileName = Path.Combine(assemblyFolder, "YmPlayer.ini");

            _host = host;
            _iniFile = new IniFile(iniFileName);
            _mainForm = new MainForm(this, _host, _iniFile);
            _serialPort = new SerialPort();

            TryOpenSerialPort();
        }

        public void OnAppEvent(object sender, AppEventArgs e) { }
        public void OnUIEvent(object sender, UIEventArgs e) { }
        public void OnRegisterEvent(object sender, RegisterEventArgs e)
        {
            if (_serialPort == null || !_serialPort.IsOpen)
                return;

            _frameCh.Writer.TryWrite(e);
        }

        public void OnPlaybackEvent(object sender, PlaybackEventArgs e) { }
        public void OnMidiMessageEvent(object sender, MidiMessageEventArgs e) { }

        public void ShowMainForm(object? parent)
        {
            _mainForm.Owner = (Form)parent;
            _mainForm.Visible = !_mainForm.Visible;
        }

        public string[] GetPortList()
        {
            return SerialPort.GetPortNames()
                      .OrderBy(p => int.TryParse(p[3..], out var n) ? n : int.MaxValue)
                      .ToArray();
        }

        public bool TryOpenSerialPort()
        {
            if (_serialPort.IsOpen)
                _serialPort.Close();

            try
            {
                ReadConfig();

                _serialPort.Open();

                StartPump();

                WriteConfig();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open {_serialPort.PortName}:\n{ex.Message}", "Serial", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        public void StartPump()
        {
            StopPump();

            _pumpCts = new CancellationTokenSource();
            var token = _pumpCts.Token;

            _pumpThread = new Thread(() =>
            {
                try
                {
                    PumpLoopAsync(token).GetAwaiter().GetResult();
                }
                catch
                {
                }
            })
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };

            _pumpThread.Start();
        }

        public void StopPump()
        {
            if (_pumpThread == null)
                return;

            _pumpCts?.Cancel();

            _frameCh.Writer.TryComplete();

            if (!_pumpThread.Join(TimeSpan.FromSeconds(2)))
                _pumpThread.Interrupt();

            _pumpCts?.Dispose();
            _pumpCts = null;
            _pumpThread = null;
        }

        private async Task PumpLoopAsync(CancellationToken token)
        {
            var sw = Stopwatch.StartNew();
            long nextUs = sw.ElapsedTicks * 1_000_000 / Stopwatch.Frequency;
            long lastSlot = 0;

            var reader = _frameCh!.Reader;          // channel is fresh for this run

            while (!token.IsCancellationRequested)
            {
                var f = await reader.ReadAsync(token);

                byte[] frame = new byte[17];
                frame[0] = (byte)f.ChipIndex;
                Array.Copy(f.Registers, 0, frame, 1, f.Registers.Length);
                _serialPort.Write(frame, 0, frame.Length);

                if (f.ChipIndex != 0)
                    continue;     // timing only on chip 0

                // ─── slot scheduling ────────────────────────────────────────────────
                nextUs += f.SlotUs;

                if (lastSlot != f.SlotUs) lastSlot = f.SlotUs;

                // If we ever slip behind, jump to the *next* slot boundary
                long nowUs = sw.ElapsedTicks * 1_000_000 / Stopwatch.Frequency;
                while (nextUs <= nowUs) nextUs += f.SlotUs;

                // Busy‑/sleep‑wait until the boundary
                do
                {
                    nowUs = sw.ElapsedTicks * 1_000_000 / Stopwatch.Frequency;
                    long diff = nextUs - nowUs;
                    if (diff > 2_000) Thread.Sleep(1);   // 1 ms
                    else Thread.SpinWait(75);
                }
                while (nextUs > nowUs && !token.IsCancellationRequested);
            }
        }

        private void ReadConfig()
        {
            var ports = GetPortList();
            _serialPort.PortName = _iniFile.GetValue("Serial", "Port", ports.Length > 0 ? ports[0] : "COM1");
            _serialPort.BaudRate = _iniFile.GetValue<int>("Serial", "BaudRate", 2_000_000);
            _serialPort.DataBits = _iniFile.GetValue<int>("Serial", "DataBits", 8);
            _serialPort.Parity = _iniFile.GetValue<Parity>("Serial", "Parity", Parity.None);
            _serialPort.StopBits = _iniFile.GetValue<StopBits>("Serial", "StopBits", StopBits.One);
        }

        private void WriteConfig()
        {
            _iniFile.SetValue("Serial", "Port", _serialPort.PortName);
            _iniFile.SetValue("Serial", "BaudRate", _serialPort.BaudRate);
            _iniFile.SetValue("Serial", "DataBits", _serialPort.DataBits);
            _iniFile.SetValue<Parity>("Serial", "Parity", _serialPort.Parity);
            _iniFile.SetValue<StopBits>("Serial", "StopBits", _serialPort.StopBits);
            _iniFile.Save();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // remove this from gc finalizer list
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposed) // dispose once only
            {
                if (disposing) // called from Dispose
                {
                    StopPump();
                    _serialPort?.Dispose();
                }
                // Clean up unmanaged resources here.
            }
            _disposed = true;
        }

        private static readonly Assembly _assembly = typeof(Plugin).Assembly;

        public string Name => _assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;
        public string Version => _assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        public string Author => _assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
        public string Description => _assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
        public PluginType PluginType => PluginType.Standard;
        public string PluginVersion => "1.00";
    }
}
