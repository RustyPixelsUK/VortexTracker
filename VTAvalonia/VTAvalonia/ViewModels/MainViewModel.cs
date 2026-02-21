using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using LibVT;
using VortexTracker;
using VortexTracker.Core.Actions;
using VortexTracker.Core.Services;
using VTAvalonia.Services;

namespace VTAvalonia.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly UIActionCatalog _actionCatalog;
        public UIActionCatalog ActionCatalog => _actionCatalog;
        public ObservableCollection<PositionEntry> Positions { get; } = new();
        public ObservableCollection<string> PatternLines { get; } = new();
        public IUIActionDispatcher ActionDispatcher { get; }
        private readonly IFileDialogService _fileDialogService;
        private readonly IClipboardService _clipboardService;
        private readonly IWindowingService _windowingService;
        private readonly IModuleService _moduleService;
        private readonly IPlaybackService _playbackService;
        private readonly Stack<LineEdit> _undoStack = new();
        private readonly Stack<LineEdit> _redoStack = new();

        private string _status = "Ready";
        private int _selectedPatternIndex;
        private int _selectedPatternLineIndex = -1;
        private string _selectedPatternLineText = string.Empty;
        private bool _isUpdatingLineText;
        private bool _isDirty;
        private string? _currentFilePath;
        private string _windowTitle = "Vortex Tracker";
        private string _statusColor = "#000000";
        private string _moduleTitle = "-";
        private string _moduleAuthor = "-";
        private int _moduleChipCount;
        private int _selectedPositionIndex = -1;
        private int _loopPositionIndex;
        private int _patternLength;
        private bool _isUpdatingPatternLength;
        private string _channelANote = "---";
        private int _channelASample;
        private int _channelAOrnament;
        private int _channelAVolume;
        private string _channelBNote = "---";
        private int _channelBSample;
        private int _channelBOrnament;
        private int _channelBVolume;
        private string _channelCNote = "---";
        private int _channelCSample;
        private int _channelCOrnament;
        private int _channelCVolume;
        private string _moduleHeaderTitle = string.Empty;
        private string _moduleHeaderAuthor = string.Empty;
        private int _moduleHeaderSpeed = 3;
        private int _moduleHeaderNoteTable;
        private bool _isLooping = true;
        private bool _isPlaying;

        public string Status
        {
            get => _status;
            set
            {
                if (SetProperty(ref _status, value))
                    StatusColor = value == "Pattern parse failed" ? "#D13438" : "#000000";
            }
        }

        public string StatusColor
        {
            get => _statusColor;
            set => SetProperty(ref _statusColor, value);
        }

        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (SetProperty(ref _isDirty, value))
                    UpdateWindowTitle();
            }
        }

        public string? CurrentFilePath
        {
            get => _currentFilePath;
            set
            {
                if (SetProperty(ref _currentFilePath, value))
                    UpdateWindowTitle();
            }
        }

        public string WindowTitle
        {
            get => _windowTitle;
            private set => SetProperty(ref _windowTitle, value);
        }

        public string ModuleTitle
        {
            get => _moduleTitle;
            private set => SetProperty(ref _moduleTitle, value);
        }

        public string ModuleAuthor
        {
            get => _moduleAuthor;
            private set => SetProperty(ref _moduleAuthor, value);
        }

        public int ModuleChipCount
        {
            get => _moduleChipCount;
            private set => SetProperty(ref _moduleChipCount, value);
        }

        public string ModuleInfo => $"{ModuleTitle} | {ModuleAuthor} | Chips: {ModuleChipCount}";

        public string PatternCursorInfo =>
            $"Pat {SelectedPatternIndex:D2}  Line {(SelectedPatternLineIndex < 0 ? 0 : SelectedPatternLineIndex):D3}/{PatternLength:D3}";

        public string PlayStopLabel => IsPlaying ? "■ Stop" : "▶ Play";

        public string ModuleHeaderTitle
        {
            get => _moduleHeaderTitle;
            set => SetProperty(ref _moduleHeaderTitle, value);
        }

        public string ModuleHeaderAuthor
        {
            get => _moduleHeaderAuthor;
            set => SetProperty(ref _moduleHeaderAuthor, value);
        }

        public int ModuleHeaderSpeed
        {
            get => _moduleHeaderSpeed;
            set => SetProperty(ref _moduleHeaderSpeed, value);
        }

        public int ModuleHeaderNoteTable
        {
            get => _moduleHeaderNoteTable;
            set => SetProperty(ref _moduleHeaderNoteTable, value);
        }

        public bool IsLooping
        {
            get => _isLooping;
            set => SetProperty(ref _isLooping, value);
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (SetProperty(ref _isPlaying, value))
                    OnPropertyChanged(nameof(PlayStopLabel));
            }
        }

        public int SelectedPatternIndex
        {
            get => _selectedPatternIndex;
            set
            {
                if (SetProperty(ref _selectedPatternIndex, value))
                    RefreshPatternLines();
            }
        }

        public int LoopPositionIndex
        {
            get => _loopPositionIndex;
            set
            {
                if (!SetProperty(ref _loopPositionIndex, value))
                    return;

                UpdateLoopPosition();
            }
        }

        public int PatternLength
        {
            get => _patternLength;
            set
            {
                if (_isUpdatingPatternLength)
                {
                    _patternLength = value;
                    return;
                }

                if (SetProperty(ref _patternLength, value))
                {
                    UpdatePatternLength();
                    OnPropertyChanged(nameof(PatternCursorInfo));
                }
            }
        }

        public int SelectedPositionIndex
        {
            get => _selectedPositionIndex;
            set
            {
                if (!SetProperty(ref _selectedPositionIndex, value))
                    return;

                if (_selectedPositionIndex >= 0 && _selectedPositionIndex < Positions.Count)
                    SelectedPatternIndex = Positions[_selectedPositionIndex].PatternIndex;
            }
        }

        public int SelectedPatternLineIndex
        {
            get => _selectedPatternLineIndex;
            set
            {
                if (SetProperty(ref _selectedPatternLineIndex, value))
                {
                    UpdateSelectedLineText();
                    OnPropertyChanged(nameof(PatternCursorInfo));
                }
            }
        }

        public string SelectedPatternLineText
        {
            get => _selectedPatternLineText;
            set
            {
                if (_isUpdatingLineText)
                {
                    _selectedPatternLineText = value;
                    return;
                }

                SetProperty(ref _selectedPatternLineText, value);
            }
        }

        public string ChannelANote
        {
            get => _channelANote;
            set => SetProperty(ref _channelANote, value);
        }

        public int ChannelASample
        {
            get => _channelASample;
            set => SetProperty(ref _channelASample, value);
        }

        public int ChannelAOrnament
        {
            get => _channelAOrnament;
            set => SetProperty(ref _channelAOrnament, value);
        }

        public int ChannelAVolume
        {
            get => _channelAVolume;
            set => SetProperty(ref _channelAVolume, value);
        }

        public string ChannelBNote
        {
            get => _channelBNote;
            set => SetProperty(ref _channelBNote, value);
        }

        public int ChannelBSample
        {
            get => _channelBSample;
            set => SetProperty(ref _channelBSample, value);
        }

        public int ChannelBOrnament
        {
            get => _channelBOrnament;
            set => SetProperty(ref _channelBOrnament, value);
        }

        public int ChannelBVolume
        {
            get => _channelBVolume;
            set => SetProperty(ref _channelBVolume, value);
        }

        public string ChannelCNote
        {
            get => _channelCNote;
            set => SetProperty(ref _channelCNote, value);
        }

        public int ChannelCSample
        {
            get => _channelCSample;
            set => SetProperty(ref _channelCSample, value);
        }

        public int ChannelCOrnament
        {
            get => _channelCOrnament;
            set => SetProperty(ref _channelCOrnament, value);
        }

        public int ChannelCVolume
        {
            get => _channelCVolume;
            set => SetProperty(ref _channelCVolume, value);
        }

        public IRelayCommand<UIActionType> ExecuteActionTypeCommand { get; }
        public IRelayCommand<string> MoveLineCommand { get; }
        public IRelayCommand ApplyLineEditCommand { get; }
        public IRelayCommand InsertLineCommand { get; }
        public IRelayCommand DeleteLineCommand { get; }
        public IRelayCommand InsertPositionCommand { get; }
        public IRelayCommand DeletePositionCommand { get; }
        public IRelayCommand DuplicatePositionCommand { get; }
        public IRelayCommand ClonePositionCommand { get; }
        public IRelayCommand SetLoopPositionCommand { get; }
        public IRelayCommand ApplyChannelACommand { get; }
        public IRelayCommand ApplyChannelBCommand { get; }
        public IRelayCommand ApplyChannelCCommand { get; }
        public IRelayCommand ApplyModuleHeaderCommand { get; }
        public IRelayCommand<string> TransposeCommand { get; }

        public MainViewModel()
            : this(new AvaloniaUIActionDispatcher(), new NullFileDialogService(), new NullClipboardService(), new NullWindowingService(), new NullModuleService(), new NullPlaybackService())
        {
        }

        public MainViewModel(IUIActionDispatcher dispatcher, IFileDialogService fileDialogService, IClipboardService clipboardService, IWindowingService windowingService, IModuleService moduleService, IPlaybackService playbackService)
        {
            var (catalog, definitions) = UIActionCatalogFactory.CreateDefault();
            _actionCatalog = catalog;
            ActionDispatcher = dispatcher;
            _fileDialogService = fileDialogService;
            _clipboardService = clipboardService;
            _windowingService = windowingService;
            _moduleService = moduleService;
            _playbackService = playbackService;
            ActionDispatcher.Executed += (_, action) => _ = HandleActionSafeAsync(action);
            _playbackService.PlaybackStateChanged += (_, playing) => { Status = playing ? "Playing" : "Stopped"; IsPlaying = playing; };
            ExecuteActionTypeCommand = new RelayCommand<UIActionType>(ExecuteActionType);
            MoveLineCommand = new RelayCommand<string>(MoveLine);
            ApplyLineEditCommand = new RelayCommand(ApplySelectedLineEdit);
            InsertLineCommand = new RelayCommand(InsertLine);
            DeleteLineCommand = new RelayCommand(DeleteLine);
            InsertPositionCommand = new RelayCommand(InsertPosition);
            DeletePositionCommand = new RelayCommand(DeletePosition);
            DuplicatePositionCommand = new RelayCommand(DuplicatePosition);
            ClonePositionCommand = new RelayCommand(ClonePosition);
            SetLoopPositionCommand = new RelayCommand(SetLoopPosition);
            ApplyChannelACommand = new RelayCommand(ApplyChannelA);
            ApplyChannelBCommand = new RelayCommand(ApplyChannelB);
            ApplyChannelCCommand = new RelayCommand(ApplyChannelC);
            ApplyModuleHeaderCommand = new RelayCommand(ApplyModuleHeader);
            TransposeCommand = new RelayCommand<string>(Transpose);
            SeedActions(definitions);
        }

        private void SeedActions(IEnumerable<UIActionDefinition> definitions) { /* kept for catalog registration */ }

        private void ExecuteActionType(UIActionType actionType)
        {
            ActionDispatcher.Execute(actionType);
        }

        private void MoveLine(string? deltaStr)
        {
            if (PatternLines.Count == 0 || !int.TryParse(deltaStr, out var delta))
                return;

            var next = Math.Clamp(SelectedPatternLineIndex + delta, 0, PatternLines.Count - 1);
            SelectedPatternLineIndex = next;
        }

        private async Task HandleActionSafeAsync(UIActionType action)
        {
            try
            {
                await HandleActionAsync(action);
            }
            catch (Exception ex)
            {
                Status = $"Error: {ex.Message}";
                StatusColor = "#D13438";
            }
        }

        /// <summary>
        /// Returns true if it is safe to discard the current module (either not dirty,
        /// or the user confirmed they want to discard unsaved changes).
        /// </summary>
        private async Task<bool> ConfirmDiscardAsync()
        {
            if (!IsDirty)
                return true;

            var fileName = string.IsNullOrWhiteSpace(CurrentFilePath)
                ? "Untitled"
                : System.IO.Path.GetFileName(CurrentFilePath);

            var result = await _windowingService.ShowConfirmDialogAsync(
                $"'{fileName}' has unsaved changes. Discard them?",
                "Unsaved Changes");

            return result;
        }

        private async Task HandleActionAsync(UIActionType action)
        {
            switch (action)
            {
                case UIActionType.FileOpen:
                {
                    if (!await ConfirmDiscardAsync())
                        break;

                    var path = await _fileDialogService.OpenFileAsync(new FileDialogOptions(
                        "Open Module",
                        DefaultExtension: "vt2",
                        Filters: new List<FileDialogFilter>
                        {
                            new("Vortex Tracker Modules", new[] { "vt2", "pt1", "pt2", "pt3", "stc", "stp", "sqt", "asc", "psc", "fls", "gtr", "ftc", "psm", "fxm", "ay" })
                        }));

                    if (path == null)
                    {
                        Status = "Open canceled";
                        break;
                    }

                    var module = await _moduleService.LoadAsync(path);
                    if (module == null)
                    {
                        Status = $"Failed to load: {System.IO.Path.GetFileName(path)}";
                        break;
                    }

                    CurrentFilePath = module.FilePath;
                    IsDirty = false;
                    ModuleTitle = module.Title;
                    ModuleAuthor = module.Author;
                    ModuleChipCount = module.ChipCount;
                    LoadModuleHeaderFields();
                    LoadModuleIntoEditor();
                    Status = $"Loaded: {module.Title} by {module.Author} ({module.ChipCount} chip(s))";
                    break;
                }
                case UIActionType.FileSave:
                {
                    // Save to the known path directly; fall back to Save-As dialog
                    var path = CurrentFilePath;
                    if (string.IsNullOrWhiteSpace(path))
                        goto case UIActionType.FileSaveAs;

                    var saved = await _moduleService.SaveAsTextAsync(path);
                    Status = saved ? $"Saved: {System.IO.Path.GetFileName(path)}" : "Nothing to save";
                    if (saved) IsDirty = false;
                    break;
                }
                case UIActionType.FileSaveAs:
                {
                    var path = await _fileDialogService.SaveFileAsync(new FileDialogOptions(
                        "Save Module",
                        DefaultExtension: "vt2",
                        Filters: new List<FileDialogFilter>
                        {
                            new("Vortex Tracker Modules", new[] { "vt2" })
                        }));

                    if (path == null)
                    {
                        Status = "Save canceled";
                        break;
                    }

                    var saved = await _moduleService.SaveAsTextAsync(path);
                    if (!saved)
                    {
                        Status = "Nothing to save";
                        break;
                    }

                    Status = $"Saved: {System.IO.Path.GetFileName(path)}";
                    CurrentFilePath = path;
                    IsDirty = false;
                    break;
                }
                case UIActionType.EditCopy:
                {
                    var line = GetSelectedPatternLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        Status = "Nothing to copy";
                        break;
                    }

                    await _clipboardService.SetTextAsync(line);
                    Status = "Copied pattern line";
                    break;
                }
                case UIActionType.EditCut:
                {
                    var line = GetSelectedPatternLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        Status = "Nothing to cut";
                        break;
                    }

                    await _clipboardService.SetTextAsync(line);
                    ApplyLineEdit(string.Empty);
                    Status = "Cut pattern line";
                    break;
                }
                case UIActionType.EditPaste:
                {
                    var text = await _clipboardService.GetTextAsync();
                    if (string.IsNullOrEmpty(text))
                    {
                        Status = "Clipboard empty";
                        break;
                    }

                    ApplyLineEdit(text);
                    Status = "Pasted pattern line";
                    break;
                }
                case UIActionType.Undo:
                {
                    if (!TryUndo())
                    {
                        Status = "Nothing to undo";
                        break;
                    }

                    Status = "Undo";
                    break;
                }
                case UIActionType.Redo:
                {
                    if (!TryRedo())
                    {
                        Status = "Nothing to redo";
                        break;
                    }

                    Status = "Redo";
                    break;
                }
                case UIActionType.FileNew:
                {
                    if (!await ConfirmDiscardAsync())
                        break;

                    var info = _moduleService.CreateNew();
                    CurrentFilePath = null;
                    IsDirty = false;
                    ModuleTitle = info.Title;
                    ModuleAuthor = info.Author;
                    ModuleChipCount = info.ChipCount;
                    LoadModuleHeaderFields();
                    LoadModuleIntoEditor();
                    Status = "New module created";
                    break;
                }
                case UIActionType.PlayStop:
                {
                    if (IsPlaying)
                    {
                        await _playbackService.StopAsync();
                        Status = "Stopped";
                    }
                    else
                    {
                        var played = await _playbackService.PlayFromStartAsync();
                        Status = played ? "Playing" : "No module loaded";
                    }
                    break;
                }
                case UIActionType.ToggleLooping:
                {
                    _moduleService.ToggleLooping();
                    IsLooping = _moduleService.LoopingEnabled;
                    Status = IsLooping ? "Looping on" : "Looping off";
                    break;
                }
                case UIActionType.PlayPatternFromLine:
                {
                    var lineIdx = Math.Max(0, SelectedPatternLineIndex);
                    var played = await _playbackService.PlayPatternFromLineAsync(SelectedPatternIndex, lineIdx);
                    Status = played ? $"Playing pattern {SelectedPatternIndex} from line {lineIdx}" : "No module loaded";
                    break;
                }
                case UIActionType.TransposeUp1:
                    TransposeCurrentPattern(1);
                    break;
                case UIActionType.TransposeDown1:
                    TransposeCurrentPattern(-1);
                    break;
                case UIActionType.TransposeUp12:
                    TransposeCurrentPattern(12);
                    break;
                case UIActionType.TransposeDown12:
                    TransposeCurrentPattern(-12);
                    break;
                case UIActionType.ExpandPattern:
                {
                    if (!_moduleService.ExpandPattern(SelectedPatternIndex))
                    {
                        Status = "Cannot expand: pattern too long";
                        break;
                    }
                    RefreshPatternLines();
                    IsDirty = true;
                    Status = "Pattern expanded";
                    break;
                }
                case UIActionType.CompressPattern:
                {
                    if (!_moduleService.CompressPattern(SelectedPatternIndex))
                    {
                        Status = "Cannot compress pattern";
                        break;
                    }
                    RefreshPatternLines();
                    IsDirty = true;
                    Status = "Pattern compressed";
                    break;
                }
                case UIActionType.SwapChannelsLeft:
                {
                    if (!_moduleService.SwapChannels(SelectedPatternIndex, false))
                    {
                        Status = "Swap failed";
                        break;
                    }
                    RefreshPatternLines();
                    IsDirty = true;
                    Status = "Channels swapped left";
                    break;
                }
                case UIActionType.SwapChannelsRight:
                {
                    if (!_moduleService.SwapChannels(SelectedPatternIndex, true))
                    {
                        Status = "Swap failed";
                        break;
                    }
                    RefreshPatternLines();
                    IsDirty = true;
                    Status = "Channels swapped right";
                    break;
                }
                case UIActionType.SetLoopPosition:
                    SetLoopPosition();
                    break;
                case UIActionType.InsertPositions:
                    InsertPosition();
                    break;
                case UIActionType.DeletePositions:
                    DeletePosition();
                    break;
                case UIActionType.DuplicatePositions:
                    DuplicatePosition();
                    break;
                case UIActionType.ClonePositions:
                    ClonePosition();
                    break;
                case UIActionType.FileExit:
                {
                    if (!await ConfirmDiscardAsync())
                        break;

                    _windowingService.ActivateMainWindow();
                    _windowingService.CloseMainWindow();
                    Status = "Exiting...";
                    break;
                }
                case UIActionType.HelpAbout:
                {
                    await _windowingService.ShowDialogAsync("About");
                    Status = "About shown";
                    break;
                }
                case UIActionType.Options:
                {
                    await _windowingService.ShowDialogAsync("Options");
                    Status = "Options shown";
                    break;
                }
                case UIActionType.PlayFromStart:
                {
                    var played = await _playbackService.PlayFromStartAsync();
                    Status = played ? "Playing" : "No module loaded";
                    break;
                }
                case UIActionType.PlayPatternFromStart:
                {
                    var played = await _playbackService.PlayPatternFromStartAsync(SelectedPatternIndex);
                    Status = played ? $"Playing pattern {SelectedPatternIndex}" : "No module loaded";
                    break;
                }
                case UIActionType.PlayLine:
                {
                    var lineIdx = Math.Max(0, SelectedPatternLineIndex);
                    var played = await _playbackService.PlayLineAsync(SelectedPatternIndex, lineIdx);
                    Status = played ? $"Previewing line {lineIdx}" : "No module loaded";
                    break;
                }
                case UIActionType.Stop:
                {
                    await _playbackService.StopAsync();
                    Status = "Stopped";
                    break;
                }
                default:
                    Status = $"Executed: {action}";
                    break;
            }
        }

        private void RefreshPositions()
        {
            Positions.Clear();
            var positions = _moduleService.GetPositions();
            for (int i = 0; i < positions.Count; i++)
                Positions.Add(new PositionEntry(i, positions[i], i == LoopPositionIndex));

            SelectedPositionIndex = Positions.Count > 0 ? 0 : -1;
            LoopPositionIndex = _moduleService.GetLoopPosition();
        }

        private void RefreshPatternLines()
        {
            PatternLines.Clear();
            foreach (var line in _moduleService.GetPatternLines(SelectedPatternIndex))
                PatternLines.Add(line);

            SelectedPatternLineIndex = PatternLines.Count > 0 ? 0 : -1;
            _isUpdatingPatternLength = true;
            PatternLength = PatternLines.Count;
            _isUpdatingPatternLength = false;
            _undoStack.Clear();
            _redoStack.Clear();
        }

        private string? GetSelectedPatternLine()
        {
            if (SelectedPatternLineIndex < 0 || SelectedPatternLineIndex >= PatternLines.Count)
                return null;

            return PatternLines[SelectedPatternLineIndex];
        }

        private void ApplyLineEdit(string newValue)
        {
            if (SelectedPatternLineIndex < 0 || SelectedPatternLineIndex >= PatternLines.Count)
                return;

            var oldValue = PatternLines[SelectedPatternLineIndex];
            if (oldValue == newValue)
                return;

            PatternLines[SelectedPatternLineIndex] = newValue;
            if (!SyncPatternToModule())
            {
                PatternLines[SelectedPatternLineIndex] = oldValue;
                Status = "Pattern parse failed";
                return;
            }

            _undoStack.Push(new LineEdit(SelectedPatternLineIndex, oldValue, newValue));
            _redoStack.Clear();
            UpdateSelectedLineText();
            IsDirty = true;
        }

        private void UpdateSelectedLineText()
        {
            _isUpdatingLineText = true;
            SelectedPatternLineText = GetSelectedPatternLine() ?? string.Empty;
            _isUpdatingLineText = false;
            UpdateChannelAFields();
        }

        private void ApplySelectedLineEdit()
        {
            if (SelectedPatternLineIndex < 0)
                return;

            ApplyLineEdit(SelectedPatternLineText);
        }

        private void InsertLine()
        {
            if (SelectedPatternLineIndex < 0)
                return;

            if (PatternLines.Count >= VTModule.MaxPatternLength)
            {
                Status = "Pattern at max length";
                return;
            }

            var updated = new List<string>(PatternLines);
            updated.Insert(SelectedPatternLineIndex, string.Empty);

            if (!TryUpdatePatternLines(updated))
                return;

            PatternLines.Insert(SelectedPatternLineIndex, string.Empty);
            SelectedPatternLineIndex = Math.Clamp(SelectedPatternLineIndex + 1, 0, PatternLines.Count - 1);
            IsDirty = true;
            _isUpdatingPatternLength = true;
            PatternLength = PatternLines.Count;
            _isUpdatingPatternLength = false;
        }

        private void DeleteLine()
        {
            if (SelectedPatternLineIndex < 0 || PatternLines.Count == 0)
                return;

            if (PatternLines.Count == 1)
            {
                Status = "Cannot delete last line";
                return;
            }

            var updated = new List<string>(PatternLines);
            updated.RemoveAt(SelectedPatternLineIndex);

            if (!TryUpdatePatternLines(updated))
                return;

            PatternLines.RemoveAt(SelectedPatternLineIndex);
            SelectedPatternLineIndex = Math.Clamp(SelectedPatternLineIndex, 0, PatternLines.Count - 1);
            IsDirty = true;
            _isUpdatingPatternLength = true;
            PatternLength = PatternLines.Count;
            _isUpdatingPatternLength = false;
        }

        private bool TryUndo()
        {
            if (_undoStack.Count == 0)
                return false;

            var edit = _undoStack.Pop();
            if (edit.Index < 0 || edit.Index >= PatternLines.Count)
                return false;

            PatternLines[edit.Index] = edit.OldValue;
            if (!SyncPatternToModule())
            {
                PatternLines[edit.Index] = edit.NewValue;
                _undoStack.Push(edit);
                return false;
            }

            _redoStack.Push(edit);
            IsDirty = true;
            UpdateSelectedLineText();
            return true;
        }

        private bool TryRedo()
        {
            if (_redoStack.Count == 0)
                return false;

            var edit = _redoStack.Pop();
            if (edit.Index < 0 || edit.Index >= PatternLines.Count)
                return false;

            PatternLines[edit.Index] = edit.NewValue;
            if (!SyncPatternToModule())
            {
                PatternLines[edit.Index] = edit.OldValue;
                _redoStack.Push(edit);
                return false;
            }

            _undoStack.Push(edit);
            IsDirty = true;
            UpdateSelectedLineText();
            return true;
        }

        private bool SyncPatternToModule()
        {
            if (!_moduleService.UpdatePatternLines(SelectedPatternIndex, PatternLines))
                return false;

            return true;
        }

        private bool TryUpdatePatternLines(IReadOnlyList<string> lines)
        {
            if (!_moduleService.UpdatePatternLines(SelectedPatternIndex, lines))
            {
                Status = "Pattern parse failed";
                return false;
            }

            return true;
        }

        private void LoadModuleHeaderFields()
        {
            var header = _moduleService.GetModuleHeader();
            if (header == null)
                return;

            ModuleHeaderTitle     = header.Title;
            ModuleHeaderAuthor    = header.Author;
            ModuleHeaderSpeed     = header.InitialDelay;
            ModuleHeaderNoteTable = header.NoteTable;
        }

        /// <summary>
        /// Populates Positions and PatternLines from the currently loaded module.
        /// Always shows the pattern referenced by the first position.
        /// </summary>
        private void LoadModuleIntoEditor()
        {
            RefreshPositions();

            // Navigate to the pattern referenced by the first position entry
            var positions = _moduleService.GetPositions();
            int firstPattern = positions.Count > 0 ? positions[0] : 0;

            // Suppress the automatic RefreshPatternLines triggered by the setter
            _selectedPatternIndex = firstPattern;
            OnPropertyChanged(nameof(SelectedPatternIndex));

            RefreshPatternLines();
        }

        private void ApplyModuleHeader()
        {
            var header = new ModuleHeader(
                ModuleHeaderTitle,
                ModuleHeaderAuthor,
                ModuleHeaderSpeed,
                ModuleHeaderNoteTable,
                _moduleService.GetModuleHeader()?.ChipFreq ?? LibVT.Main.DefaultChipFreq,
                _moduleService.GetModuleHeader()?.IntFreq  ?? LibVT.Main.DefaultIntFreq);

            if (!_moduleService.UpdateModuleHeader(header))
            {
                Status = "Module header update failed";
                return;
            }

            ModuleTitle  = header.Title;
            ModuleAuthor = header.Author;
            IsDirty = true;
            Status = "Module header updated";
        }

        private void Transpose(string? semitonesStr)
        {
            if (!int.TryParse(semitonesStr, out var semitones))
                return;

            if (!_moduleService.TransposePattern(SelectedPatternIndex, semitones))
            {
                Status = "Transpose failed";
                return;
            }

            RefreshPatternLines();
            IsDirty = true;
            Status = $"Transposed {(semitones > 0 ? "+" : "")}{semitones}";
        }

        private void TransposeCurrentPattern(int semitones) => Transpose(semitones.ToString());

        private void DuplicatePosition()
        {
            if (SelectedPositionIndex < 0)
                return;

            if (!_moduleService.DuplicatePosition(SelectedPositionIndex))
            {
                Status = "Duplicate position failed";
                return;
            }

            var prevIndex = SelectedPositionIndex;
            RefreshPositions();
            SelectedPositionIndex = Math.Clamp(prevIndex + 1, 0, Positions.Count - 1);
            IsDirty = true;
            Status = "Position duplicated";
        }

        private void ClonePosition()
        {
            if (SelectedPositionIndex < 0)
                return;

            if (!_moduleService.ClonePosition(SelectedPositionIndex))
            {
                Status = "Clone position failed (no free pattern slot)";
                return;
            }

            var prevIndex = SelectedPositionIndex;
            RefreshPositions();
            SelectedPositionIndex = Math.Clamp(prevIndex + 1, 0, Positions.Count - 1);
            IsDirty = true;
            Status = "Position cloned";
        }

        private void SetLoopPosition()
        {
            if (SelectedPositionIndex < 0)
                return;

            LoopPositionIndex = SelectedPositionIndex;
            Status = $"Loop set to position {SelectedPositionIndex}";
        }

        private void InsertPosition()
        {
            var index = SelectedPositionIndex < 0 ? 0 : SelectedPositionIndex;
            var updated = new List<int>();
            foreach (var entry in Positions)
                updated.Add(entry.PatternIndex);

            if (updated.Count >= VTModule.MaxPatternCount)
            {
                Status = "Positions at max length";
                return;
            }

            updated.Insert(index, SelectedPatternIndex);
            if (!_moduleService.UpdatePositions(updated, _moduleService.GetLoopPosition()))
            {
                Status = "Position update failed";
                return;
            }

            RefreshPositions();
            SelectedPositionIndex = Math.Clamp(index + 1, 0, Positions.Count - 1);
            IsDirty = true;
        }

        private void DeletePosition()
        {
            if (SelectedPositionIndex < 0 || Positions.Count == 0)
                return;

            if (Positions.Count == 1)
            {
                Status = "Cannot delete last position";
                return;
            }

            var updated = new List<int>();
            for (int i = 0; i < Positions.Count; i++)
            {
                if (i == SelectedPositionIndex)
                    continue;
                updated.Add(Positions[i].PatternIndex);
            }

            if (!_moduleService.UpdatePositions(updated, _moduleService.GetLoopPosition()))
            {
                Status = "Position update failed";
                return;
            }

            RefreshPositions();
            SelectedPositionIndex = Math.Clamp(SelectedPositionIndex, 0, Positions.Count - 1);
            IsDirty = true;
        }

        private void UpdateWindowTitle()
        {
            var fileName = string.IsNullOrWhiteSpace(CurrentFilePath)
                ? "Untitled"
                : Path.GetFileName(CurrentFilePath);
            var dirtyMarker = IsDirty ? "*" : string.Empty;
            WindowTitle = $"Vortex Tracker {dirtyMarker} - {fileName}";
        }

        private void UpdateLoopPosition()
        {
            var positions = new List<int>();
            foreach (var entry in Positions)
                positions.Add(entry.PatternIndex);

            if (!_moduleService.UpdatePositions(positions, LoopPositionIndex))
            {
                Status = "Position update failed";
                return;
            }

            for (int i = 0; i < Positions.Count; i++)
                Positions[i] = Positions[i] with { IsLoop = i == LoopPositionIndex };

            IsDirty = true;
        }

        private void UpdatePatternLength()
        {
            var length = Math.Clamp(PatternLength, 1, VTModule.MaxPatternLength);
            if (length == PatternLines.Count)
                return;

            var updated = new List<string>(PatternLines);
            if (length > updated.Count)
            {
                while (updated.Count < length)
                    updated.Add(string.Empty);
            }
            else
            {
                updated.RemoveRange(length, updated.Count - length);
            }

            if (!TryUpdatePatternLines(updated))
                return;

            PatternLines.Clear();
            foreach (var line in updated)
                PatternLines.Add(line);

            SelectedPatternLineIndex = Math.Clamp(SelectedPatternLineIndex, 0, PatternLines.Count - 1);
            IsDirty = true;
        }

        private void UpdateChannelAFields()
        {
            var dataA = _moduleService.GetPatternLineData(SelectedPatternIndex, SelectedPatternLineIndex, 0);
            var dataB = _moduleService.GetPatternLineData(SelectedPatternIndex, SelectedPatternLineIndex, 1);
            var dataC = _moduleService.GetPatternLineData(SelectedPatternIndex, SelectedPatternLineIndex, 2);

            if (dataA != null) { ChannelANote = dataA.Note; ChannelASample = dataA.Sample; ChannelAOrnament = dataA.Ornament; ChannelAVolume = dataA.Volume; }
            if (dataB != null) { ChannelBNote = dataB.Note; ChannelBSample = dataB.Sample; ChannelBOrnament = dataB.Ornament; ChannelBVolume = dataB.Volume; }
            if (dataC != null) { ChannelCNote = dataC.Note; ChannelCSample = dataC.Sample; ChannelCOrnament = dataC.Ornament; ChannelCVolume = dataC.Volume; }
        }

        private void ApplyChannelA() => ApplyChannel(0, ChannelANote, ChannelASample, ChannelAOrnament, ChannelAVolume, "A");
        private void ApplyChannelB() => ApplyChannel(1, ChannelBNote, ChannelBSample, ChannelBOrnament, ChannelBVolume, "B");
        private void ApplyChannelC() => ApplyChannel(2, ChannelCNote, ChannelCSample, ChannelCOrnament, ChannelCVolume, "C");

        private void ApplyChannel(int ch, string note, int sample, int ornament, int volume, string label)
        {
            if (SelectedPatternLineIndex < 0)
                return;

            var data = new PatternLineData(note, sample, ornament, volume);
            if (!_moduleService.UpdatePatternLineData(SelectedPatternIndex, SelectedPatternLineIndex, data, ch))
            {
                Status = "Pattern parse failed";
                return;
            }

            RefreshPatternLines();
            IsDirty = true;
        }

        private sealed record LineEdit(int Index, string OldValue, string NewValue);
        public sealed record PositionEntry(int Index, int PatternIndex, bool IsLoop)
        {
            public string LoopMarker => IsLoop ? "L" : "";
        }
    }
}
