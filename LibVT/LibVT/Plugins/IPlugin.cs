using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibVT.Plugins
{
    public enum PluginType
    {
        Standard = 0,
        SampleEditor = 1,
        OrnamentEditor = 2,
        PatternEditor = 3,
        Utility = 4
    };

    public class UIEventArgs : EventArgs
    {
        public string Name { get; }

        public UIEventArgs(string name)
        {
            Name = name;
        }

        public override string ToString() => $"Name=\"{Name}\"";
    }

    public interface IPlugin : IDisposable
    {
        abstract void Initialize(IHost host);
        abstract void OnAppEvent(object? sender, AppEventArgs e);
        abstract void OnUIEvent(object? sender, UIEventArgs e);
        abstract void OnRegisterEvent(object? sender, RegisterEventArgs e);
        abstract void OnPlaybackEvent(object? sender, PlaybackEventArgs e);
        abstract void OnMidiMessageEvent(object sender, MidiMessageEventArgs e);
        abstract void ShowMainForm(object? parent);

        public string Name { get; }
        public string Version { get; }
        public string Author { get; }
        public string Description { get; }
        public PluginType PluginType { get; }
        public string PluginVersion { get; }
    }
}
