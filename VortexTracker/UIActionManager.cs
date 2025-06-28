// 
// This is part of Vortex Tracker II project
// 
// (c)2000-2009 S.V.Bulba
// Author: Sergey Bulba, vorobey@mail.khstu.ru
// Support page: http://bulba.untergrund.net/
// 
// Version 1.5 - 2.6
// (c)2017-2021 Ivan Pirog, ivan.pirog@gmail.com
// 
// Version 2.6 - 2.6.1
// (c)2022-2025 Dexus (Volutar), https://github.com/Volutar
// 
// Version 3.0+ (C# port)
// (c)2025 Ben Baker, https://github.com/benbaker76

using LibVT;
using LibVT.Plugins;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexTracker
{
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
    };

    public sealed class UIActionManager
    {
        private readonly Dictionary<Component, UIAction> _byComponent = new();
        private readonly List<UIAction> _actions = new();

        #region -- public API ----------------------------------------------------

        public void AddActions(params UIAction[] actions)
        {
            _actions.AddRange(actions);
        }

        public void AddEvents(UIActionType actionType, EventHandler<EventArgs> execute, EventHandler<EventArgs> update)
        {
            foreach(var act in _actions.Where(a => a.ActionType == actionType))
                act.AddEvents(execute, update);
        }

        public void AddComponents(UIActionType actionType, params Component[] components)
        {
            foreach (var act in _actions.Where(a => a.ActionType == actionType))
            {
                foreach (Component component in components)
                {
                    if (_byComponent.ContainsKey(component))
                        throw new InvalidOperationException($"Component {component} already registered.");

                    _byComponent[component] = act;
                }

                act.AddComponents(components);
            }
        }

        public void Execute(object sender, UIActionType actionType)
        {
            foreach (var action in _actions.Where(a => a.ActionType == actionType))
                action.InternalExecute(sender, EventArgs.Empty);
        }

        public void Update(UIActionType actionType)
        {
            foreach (var action in _actions.Where(a => a.ActionType == actionType))
                action.InternalUpdate();
        }

        public void Execute(object sender, EventArgs e) => OnComponentExecute(sender, e);

        public void Update(object sender, EventArgs e)
        {
            if (sender is Component c && _byComponent.TryGetValue(c, out var act))
                act.InternalUpdate();
        }

        public bool IsEnabled(UIActionType actionType)
        {
            if (TryGetAction(actionType, out UIAction action))
                return action.Enabled;

            return false;
        }

        public void SetEnabled(Component c, bool value)
        {
            if (_byComponent.TryGetValue(c, out var act))
                act.Enabled = value;       // UpdateComponents() fires inside the setter
        }

        public void SetEnabled(UIActionType actionType, bool value)
        {
            if (TryGetAction(actionType, out UIAction action))
                action.Enabled = value;
        }

        public void SetChecked(UIActionType actionType, bool value)
        {
            if (TryGetAction(actionType, out UIAction action))
                action.Checked = value;
        }

        public void SetText(UIActionType actionType, string value)
        {
            if (TryGetAction(actionType, out UIAction action))
                action.Text = value;
        }

        public void SetImageIndex(UIActionType actionType, int value)
        {
            if (TryGetAction(actionType, out UIAction action))
                action.ImageIndex = value;
        }

        private bool TryGetAction(UIActionType actionType, out UIAction action)
        {
            action = _actions.FirstOrDefault(a => a.ActionType == actionType);
            return (action != null);
        }

        #endregion

        #region -- internals -----------------------------------------------------

        private void OnComponentExecute(object sender, EventArgs e)
        {
            if (sender is not Component c || !_byComponent.TryGetValue(c, out var action))
                return;

            VortexTracker.PluginManager.RaiseUIEvent(this, new UIEventArgs(action.ActionType.ToString()));

            action.InternalExecute(sender, EventArgs.Empty);   // run execute callback once
            action.InternalUpdate(sender, EventArgs.Empty);    // run update callback oce
            action.InternalUpdate();             // refresh every bound control
        }

        public static UIActionManager Instance { get; } = new UIActionManager();

        private UIActionManager() { }          // singleton; optional
        #endregion
    }
}
