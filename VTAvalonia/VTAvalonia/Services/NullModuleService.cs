using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VortexTracker.Core.Services;

namespace VTAvalonia.Services;

public sealed class NullModuleService : IModuleService
{
    public ModuleInfo? Current => null;
    public bool IsPlaying => false;
    public bool LoopingEnabled => true;

    public ModuleInfo CreateNew() => new ModuleInfo("", "New Module", "", 1);

    public Task<ModuleInfo?> LoadAsync(string filePath, CancellationToken cancellationToken = default)
        => Task.FromResult<ModuleInfo?>(null);

    public Task<bool> SaveAsTextAsync(string filePath, CancellationToken cancellationToken = default)
        => Task.FromResult(false);

    public IReadOnlyList<int> GetPositions() => Array.Empty<int>();

    public IReadOnlyList<string> GetPatternLines(int patternIndex) => Array.Empty<string>();

    public bool UpdatePatternLines(int patternIndex, IReadOnlyList<string> lines) => false;

    public int GetLoopPosition() => 0;

    public bool UpdatePositions(IReadOnlyList<int> positions, int loopPosition) => false;

    public PatternLineData? GetPatternLineData(int patternIndex, int lineIndex, int channelIndex = 0) => null;

    public bool UpdatePatternLineData(int patternIndex, int lineIndex, PatternLineData data, int channelIndex = 0) => false;

    public ModuleHeader? GetModuleHeader() => null;

    public bool UpdateModuleHeader(ModuleHeader header) => false;

    public bool DuplicatePosition(int positionIndex) => false;

    public bool ClonePosition(int positionIndex) => false;

    public bool TransposePattern(int patternIndex, int semitones) => false;

    public bool ExpandPattern(int patternIndex) => false;

    public bool CompressPattern(int patternIndex) => false;

    public bool SwapChannels(int patternIndex, bool rightDirection) => false;

    public void ToggleLooping() { }
}
