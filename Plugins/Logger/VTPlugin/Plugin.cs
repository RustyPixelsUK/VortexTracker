using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LibVT;
using LibVT.Plugins;

namespace VTPlugin
{
    public class Plugin : IPlugin, IDisposable
    {
        private IHost _host = null;
        private MainForm _mainForm = null;

        private bool _disposed = false;

        public void Initialize(IHost host)
        {
            _host = host;
            _mainForm = new MainForm(_host);
        }

        private string Timestamp() => DateTime.Now.ToString("HH:mm:ss.fff");

        public void OnAppEvent(object sender, AppEventArgs e) => Log("AppEvent", e);
        public void OnUIEvent(object sender, UIEventArgs e) => Log("UIEvent", e);
        public void OnRegisterEvent(object sender, RegisterEventArgs e) => Log("RegisterEvent", e);
        public void OnPlaybackEvent(object sender, PlaybackEventArgs e) => Log("PlaybackEvent", e);
        public void OnMidiMessageEvent(object sender, MidiMessageEventArgs e) => Log("MidiMessageEvent", e);

        private void Log(string tag, EventArgs e)
        {
            if (!_mainForm.Visible)
                return;

            string logMessage = $"[{Timestamp()}] {tag}: {e}";

            if (_mainForm.LogTextBox.InvokeRequired)
                _mainForm.LogTextBox.BeginInvoke(new Action(() => AppendLog(logMessage)));
            else
                AppendLog(logMessage);
        }

        public void AppendLog(string logMessage)
        {
            _mainForm.LogTextBox.AppendText(logMessage + Environment.NewLine);
            _mainForm.LogTextBox.SelectionStart = _mainForm.LogTextBox.Text.Length;
            _mainForm.LogTextBox.ScrollToCaret();
        }

        public void ShowMainForm(object? parent)
        {
            _mainForm.Owner = (Form)parent;
            _mainForm.Visible = !_mainForm.Visible;
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
                    // Clean up managed resources
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
