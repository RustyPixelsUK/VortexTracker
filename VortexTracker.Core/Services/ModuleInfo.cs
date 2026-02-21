namespace VortexTracker.Core.Services;

public sealed record ModuleInfo(string FilePath, string Title, string Author, int ChipCount);

public sealed record ModuleHeader(
    string Title,
    string Author,
    int InitialDelay,
    int NoteTable,
    int ChipFreq,
    int IntFreq);
