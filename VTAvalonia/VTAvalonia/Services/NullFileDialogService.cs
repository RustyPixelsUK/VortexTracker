using VortexTracker.Core.Services;
using System.Threading;
using System.Threading.Tasks;

namespace VTAvalonia.Services;

public sealed class NullFileDialogService : IFileDialogService
{
    public Task<string?> OpenFileAsync(FileDialogOptions options, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>(null);

    public Task<string?> SaveFileAsync(FileDialogOptions options, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>(null);
}
