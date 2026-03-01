using System;
using System.IO;
using VortexTracker.Core.Services;

namespace VTAvalonia.Services;

public sealed class AvaloniaPlatformPathsService : IPlatformPathsService
{
    private const string AppName = "VortexTracker";

    public string GetAppDataPath()
    {
        var path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            AppName);
        Directory.CreateDirectory(path);
        return path;
    }

    public string GetDocumentsPath()
        => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
}
