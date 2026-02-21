using System.Threading.Tasks;
using VortexTracker.Core.Services;

namespace VTAvalonia.Services;

public sealed class NullWindowingService : IWindowingService
{
    public void ShowMainWindow() { }
    public Task ShowDialogAsync(string viewModelKey, object? parameter = null) => Task.CompletedTask;
    public Task<bool> ShowConfirmDialogAsync(string message, string title) => Task.FromResult(true);
    public void ActivateMainWindow() { }
    public void CloseMainWindow() { }
}
