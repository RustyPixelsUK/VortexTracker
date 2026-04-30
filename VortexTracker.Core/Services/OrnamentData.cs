namespace VortexTracker.Core.Services;

/// <summary>
/// Immutable snapshot of a single ornament's metadata and offsets.
/// </summary>
public sealed record OrnamentData(
    int Index,
    int Length,
    int Loop,
    IReadOnlyList<sbyte> Offsets);
