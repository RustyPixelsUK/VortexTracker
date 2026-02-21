using System.Threading;
using System.Threading.Tasks;
using VortexTracker.Core.Services;

namespace VTAvalonia.Services;

public sealed class NullClipboardService : IClipboardService
{
    public Task SetTextAsync(string text, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task<string?> GetTextAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<string?>(null);
}
