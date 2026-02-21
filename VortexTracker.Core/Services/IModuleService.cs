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
}
