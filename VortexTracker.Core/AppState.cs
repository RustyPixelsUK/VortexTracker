namespace VortexTracker.Core;

public sealed class AppState
{
    public string? CurrentFilePath { get; set; }
    public bool IsDirty { get; set; }
}
