namespace VortexTracker.Core.Actions;

public static class UIActionCatalogFactory
{
    public static (UIActionCatalog Catalog, IReadOnlyList<UIActionDefinition> Definitions) CreateDefault()
    {
        var catalog = new UIActionCatalog();
        var definitions = new List<UIActionDefinition>
        {
            // File
            new(UIActionType.FileNew,              "File", "New"),
            new(UIActionType.FileOpen,             "File", "Open..."),
            new(UIActionType.FileSave,             "File", "Save"),
            new(UIActionType.FileSaveAs,           "File", "Save As..."),
            new(UIActionType.Options,              "File", "Options"),
            new(UIActionType.FileExit,             "File", "Exit"),
            // Play
            new(UIActionType.PlayStop,             "Play", "Play/Stop"),
            new(UIActionType.PlayFromStart,        "Play", "Play From Start"),
            new(UIActionType.PlayPatternFromLine,  "Play", "Play Pattern From Line"),
            new(UIActionType.PlayPatternFromStart, "Play", "Play Pattern From Start"),
            new(UIActionType.Stop,                 "Play", "Stop"),
            new(UIActionType.ToggleLooping,        "Play", "Toggle Looping"),
            // Edit
            new(UIActionType.Undo,                 "Edit", "Undo"),
            new(UIActionType.Redo,                 "Edit", "Redo"),
            new(UIActionType.EditCut,              "Edit", "Cut"),
            new(UIActionType.EditCopy,             "Edit", "Copy"),
            new(UIActionType.EditPaste,            "Edit", "Paste"),
            new(UIActionType.TransposeUp1,         "Edit", "Transpose +1"),
            new(UIActionType.TransposeDown1,       "Edit", "Transpose -1"),
            new(UIActionType.TransposeUp12,        "Edit", "Transpose +12"),
            new(UIActionType.TransposeDown12,      "Edit", "Transpose -12"),
            new(UIActionType.ExpandPattern,        "Edit", "Expand Pattern"),
            new(UIActionType.CompressPattern,      "Edit", "Compress Pattern"),
            new(UIActionType.SwapChannelsLeft,     "Edit", "Swap Channels Left"),
            new(UIActionType.SwapChannelsRight,    "Edit", "Swap Channels Right"),
            new(UIActionType.SetLoopPosition,      "Edit", "Set Loop Position"),
            new(UIActionType.InsertPositions,      "Edit", "Insert Position"),
            new(UIActionType.DeletePositions,      "Edit", "Delete Position"),
            new(UIActionType.DuplicatePositions,   "Edit", "Duplicate Position"),
            new(UIActionType.ClonePositions,       "Edit", "Clone Position"),
            // Help
            new(UIActionType.HelpAbout,            "Help", "About"),
        };

        foreach (var definition in definitions)
            catalog.Add(definition);

        return (catalog, definitions);
    }
}
