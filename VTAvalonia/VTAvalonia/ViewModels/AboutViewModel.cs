using System.Reflection;

namespace VTAvalonia.ViewModels;

public sealed class AboutViewModel : ViewModelBase
{
    public string ProductName { get; } = "Vortex Tracker";
    public string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
}
