namespace VortexTracker;

public enum UIActionType : int
{
    None,
    // File
    FileNew,
    NewTurboSound,
    NewTurboSound3,
    JoinTracks,
    FileOpen,
    // OpenDemosong
    FileClose,
    FileSave,
    FileSaveAs,
    SaveAsTwoModules,
    SaveAsTemplate,
    // ---
    ExportToWAV,
    ExportPSG,
    ExportYM,
    SaveSNDH,
    SaveForZX,
    // ---
    Options,
    // ---
    FileExit,
    // Play
    PlayStop,
    PlayFromLine,
    PlayFromStart,
    PlayPatternFromLine,
    PlayPatternFromStart,
    PlayLine,
    Stop,
    // ---
    ToggleLooping,
    ToggleLoopingAll,
    // Edit
    Undo,
    Redo,
    // ---
    EditCut,
    EditCopy,
    EditPaste,
    // Merge
    // ---
    CopyToModPlug,
    CopyToRenoise,
    CopyToFami,
    // ---
    TransposeUp1,
    TransposeDown1,
    TransposeUp3,
    TransposeDown3,
    TransposeUp5,
    TransposeDown5,
    TransposeUp12,
    TransposeDown12,
    // ---
    ExpandPattern,
    CompressPattern,
    SplitPattern,
    PatternPacker,
    // ---
    SwapChannelsLeft,
    SwapChannelsRight,
    // ---
    ToggleSamples,
    TracksManager,
    GlobalTransposition,
    PluginManager,
    // Window
    Maximize,
    Normal,
    // Help
    HelpAbout,
    // SetColor
    // ResetColors
    // ---
    SetLoopPosition,
    // ---
    InsertPositions,
    DeletePositions,
    DuplicatePositions,
    ClonePositions,
    // ---
    //ChangePatternsLength,
    // ---
    // RenumberPatternsLength
    // FillEmptyPositions
    ToggleChip,
    ToggleChannels,
    // ---
    UseLastNoteParams,
    MoveBetweenPatterns,
    // ---
    AutoStep0,
    AutoStep1,
    AutoStep2,
    AutoStep3,
    AutoStep4,
    AutoStep5,
    AutoStep6,
    AutoStep7,
    AutoStep8,
    AutoStep9,
    // ---
    JumpPatternStart,
    JumpPatternEnd,
    JumpLineStart,
    JumpLineEnd,
    // ---
}
