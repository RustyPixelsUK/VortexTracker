namespace VortexTracker.Core.Actions;

public sealed record UIActionDefinition(
    UIActionType ActionType,
    string Category,
    string Text,
    string? Hint = null,
    int ImageIndex = -1,
    string? Shortcut = null);
