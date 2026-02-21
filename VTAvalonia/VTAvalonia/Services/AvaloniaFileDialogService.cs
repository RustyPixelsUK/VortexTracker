using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VortexTracker.Core.Services;

namespace VTAvalonia.Services;

public sealed class AvaloniaFileDialogService : IFileDialogService
{
    private readonly Func<TopLevel?> _topLevelProvider;

    public AvaloniaFileDialogService(Func<TopLevel?> topLevelProvider)
    {
        _topLevelProvider = topLevelProvider;
    }

    public async Task<string?> OpenFileAsync(FileDialogOptions options, CancellationToken cancellationToken = default)
    {
        var topLevel = _topLevelProvider();
        if (topLevel?.StorageProvider is null)
            return null;

        var results = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = options.Title,
            SuggestedStartLocation = await ResolveStartLocationAsync(topLevel, options, cancellationToken),
            AllowMultiple = false,
            FileTypeFilter = BuildFilters(options)
        });

        return results.FirstOrDefault()?.Path.LocalPath;
    }

    public async Task<string?> SaveFileAsync(FileDialogOptions options, CancellationToken cancellationToken = default)
    {
        var topLevel = _topLevelProvider();
        if (topLevel?.StorageProvider is null)
            return null;

        var result = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = options.Title,
            SuggestedStartLocation = await ResolveStartLocationAsync(topLevel, options, cancellationToken),
            DefaultExtension = options.DefaultExtension,
            FileTypeChoices = BuildFilters(options)
        });

        return result?.Path.LocalPath;
    }

    private static IReadOnlyList<FilePickerFileType>? BuildFilters(FileDialogOptions options)
    {
        if (options.Filters == null || options.Filters.Count == 0)
            return null;

        return options.Filters
            .Select(filter => new FilePickerFileType(filter.Name)
            {
                Patterns = filter.Extensions
                    .Select(ext => ext.StartsWith("*.") ? ext : $"*.{ext}")
                    .ToList()
            })
            .ToList();
    }

    private static async Task<IStorageFolder?> ResolveStartLocationAsync(TopLevel topLevel, FileDialogOptions options, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(options.InitialDirectory))
            return null;

        return await topLevel.StorageProvider.TryGetFolderFromPathAsync(options.InitialDirectory);
    }
}
