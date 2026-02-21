namespace VortexTracker.Core.Services;

public interface IFileDialogService
{
    Task<string?> OpenFileAsync(FileDialogOptions options, CancellationToken cancellationToken = default);
    Task<string?> SaveFileAsync(FileDialogOptions options, CancellationToken cancellationToken = default);
}

public sealed record FileDialogOptions(
    string Title,
    string? InitialDirectory = null,
    string? DefaultExtension = null,
    IReadOnlyList<FileDialogFilter>? Filters = null);

public sealed record FileDialogFilter(string Name, IReadOnlyList<string> Extensions);
