namespace VortexTracker.Core.Services;

public interface IWindowingService
{
    void ShowMainWindow();
    Task ShowDialogAsync(string viewModelKey, object? parameter = null);
    Task<bool> ShowConfirmDialogAsync(string message, string title);
    void ActivateMainWindow();
    void CloseMainWindow();
}
