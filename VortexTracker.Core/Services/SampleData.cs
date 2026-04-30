namespace VortexTracker.Core.Services;

/// <summary>
/// Immutable snapshot of a single sample's metadata and formatted tick lines.
/// </summary>
public sealed record SampleData(
    int Index,
    int Length,
    int Loop,
    bool Enabled,
    IReadOnlyList<string> TickLines);

/// <summary>
/// Represents a single sample tick for editing purposes.
/// </summary>
public sealed record SampleTickData(
    bool MixerTone,
    bool MixerNoise,
    bool EnvelopeEnabled,
    short AddToTone,
    bool ToneAccumulation,
    sbyte AddToEnvelopeOrNoise,
    bool EnvelopeOrNoiseAccumulation,
    byte Amplitude,
    bool AmplitudeSliding,
    bool AmplitudeSlideUp);
