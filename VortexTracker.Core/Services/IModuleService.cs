namespace VortexTracker.Core.Services;

public interface IModuleService
{
    ModuleInfo? Current { get; }
    ModuleInfo CreateNew();
    Task<ModuleInfo?> LoadAsync(string filePath, CancellationToken cancellationToken = default);
    Task<bool> SaveAsTextAsync(string filePath, CancellationToken cancellationToken = default);
    IReadOnlyList<int> GetPositions();
    IReadOnlyList<string> GetPatternLines(int patternIndex);
    bool UpdatePatternLines(int patternIndex, IReadOnlyList<string> lines);
    int GetLoopPosition();
    bool UpdatePositions(IReadOnlyList<int> positions, int loopPosition);
    PatternLineData? GetPatternLineData(int patternIndex, int lineIndex, int channelIndex = 0);
    bool UpdatePatternLineData(int patternIndex, int lineIndex, PatternLineData data, int channelIndex = 0);
    ModuleHeader? GetModuleHeader();
    bool UpdateModuleHeader(ModuleHeader header);
    bool DuplicatePosition(int positionIndex);
    bool ClonePosition(int positionIndex);
    bool TransposePattern(int patternIndex, int semitones);
    bool ExpandPattern(int patternIndex);
    bool CompressPattern(int patternIndex);
    bool SwapChannels(int patternIndex, bool rightDirection);
    bool IsPlaying { get; }
    void ToggleLooping();
    bool LoopingEnabled { get; }

    // Sample access
    int SampleCount { get; }
    SampleData? GetSample(int index);
    bool UpdateSampleTick(int sampleIndex, int tickIndex, SampleTickData tick);
    bool UpdateSampleLength(int sampleIndex, int length);
    bool UpdateSampleLoop(int sampleIndex, int loop);
    bool ClearSample(int sampleIndex);

    // Ornament access
    int OrnamentCount { get; }
    OrnamentData? GetOrnament(int index);
    bool UpdateOrnamentOffset(int ornamentIndex, int offsetIndex, sbyte value);
    bool UpdateOrnamentLength(int ornamentIndex, int length);
    bool UpdateOrnamentLoop(int ornamentIndex, int loop);
    bool ClearOrnament(int ornamentIndex);
}
