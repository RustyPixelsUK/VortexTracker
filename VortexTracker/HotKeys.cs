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
// Version 2.6.1
// (c)2022-2025 Dexus (Volutar), https://github.com/Volutar
// 
// Version 3.0+ (C# port)
// (c)2025 Ben Baker, https://github.com/benbaker76

using LibVT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace VortexTracker
{
    public enum HotKeyType
    {
        Options,
        // ---
        PlayStop,
        PlayFromLine,
        PlayFromStart,
        PlayPatternFromLine,
        PlayPatternFromStart,
        Stop,
        // ---
        ToggleLooping,
        ToggleLoopingAll,
        // ---
        ToggleSamples,
        // ---
        Undo,
        Redo,
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
        // --
        TracksManager,
        // ---
        SetLoopPosition,
        // ---
        InsertPositions,
        DeletePositions,
        DuplicatePositions,
        ClonePositions,
        // ---
        ToggleChip,
        ToggleChannels,
        // ---
        UseLastNoteParams,
        MoveBetweenPatterns,
        // ---
        JumpPatternStart,
        JumpPatternEnd,
        JumpLineStart,
        JumpLineEnd,
    };

    public class HotKey
    {
        public HotKeyType Type { get; set; }
        public string Name { get; set; }
        public string DefaultShortcutText { get; set; }
        public string ShortcutText { get; set; }
        public Action Action { get; set; }

        public HotKey(HotKeyType type, string name, string shortcutText, Action action)
        {
            Type = type;
            Name = name;
            DefaultShortcutText = shortcutText;
            ShortcutText = shortcutText;
            Action = action;
        }
    }

    public class HotKeys
    {
        public static string[] SystemHotKeys =
        {
            "Ctrl+A", "Ctrl+V", "Ctrl+N", "Ctrl+O",
            "Ctrl+C", "Ctrl+X", "Ctrl+V", "Ctrl+W",
            "Ctrl+S", "Ctrl+Shift+S", "Shift+Ctrl+S",
            "Alt+Space", "Alt+F4"
        };

        public static readonly HotKey[] AllHotKeys =
        {
            new HotKey(HotKeyType.Options, "Options", "Ctrl+Shift+O", () => UIActionManager.Instance.Execute(null, UIActionType.Options)), // Globals.MainForm.Options1.PerformClick()),
            // ---
            new HotKey(HotKeyType.PlayStop, "Play/Stop", "Space", () => UIActionManager.Instance.Execute(null, UIActionType.PlayStop)),
            new HotKey(HotKeyType.PlayFromLine, "Play From Line", "F5", () => UIActionManager.Instance.Execute(null, UIActionType.PlayFromLine)),
            new HotKey(HotKeyType.PlayFromStart, "Play Track From Start", "F6", () => UIActionManager.Instance.Execute(null, UIActionType.PlayFromStart)),
            new HotKey(HotKeyType.PlayPatternFromLine, "Play Pattern From Line", "F7", () => UIActionManager.Instance.Execute(null, UIActionType.PlayPatternFromLine)),
            new HotKey(HotKeyType.PlayPatternFromStart, "Play Pattern From Start", "F8", () => UIActionManager.Instance.Execute(null, UIActionType.PlayPatternFromStart)),
            new HotKey(HotKeyType.Stop, "Stop", "Esc", () => UIActionManager.Instance.Execute(null, UIActionType.Stop)),
            // ---
            new HotKey(HotKeyType.ToggleLooping, "Toggle Looping", "Ctrl+L", () => UIActionManager.Instance.Execute(null, UIActionType.ToggleLooping)), //Globals.MainForm.ToggleLooping1.Checked = !Globals.MainForm.ToggleLooping1.Checked),
            new HotKey(HotKeyType.ToggleLoopingAll, "Toggle Looping All", "Ctrl+Alt+L", () => UIActionManager.Instance.Execute(null, UIActionType.ToggleLoopingAll)), // Globals.MainForm.ToggleLoopingAll1.Checked = !Globals.MainForm.ToggleLoopingAll1.Checked),
            // ---
            new HotKey(HotKeyType.ToggleSamples, "Toggle Samples", "Ctrl+M", () => UIActionManager.Instance.Execute(null, UIActionType.ToggleSamples)), // Globals.MainForm.Togglesamples1.PerformClick()),
            // ---
            new HotKey(HotKeyType.Undo, "Undo", "Ctrl+Z", () => UIActionManager.Instance.Execute(null, UIActionType.Undo)),
            new HotKey(HotKeyType.Redo, "Redo", "Ctrl+Shift+Z", () => UIActionManager.Instance.Execute(null, UIActionType.Redo)),
            // ---
            new HotKey(HotKeyType.CopyToModPlug, "Copy Pattern To OpenMPT", "Ctrl+Shift+M", () => UIActionManager.Instance.Execute(null, UIActionType.CopyToModPlug)),
            new HotKey(HotKeyType.CopyToRenoise, "Copy Pattern To Renoise", "Ctrl+Shift+R", () => UIActionManager.Instance.Execute(null, UIActionType.CopyToRenoise)),
            new HotKey(HotKeyType.CopyToFami, "Copy Pattern To FamiTracker", "Ctrl+Shift+F", () => UIActionManager.Instance.Execute(null, UIActionType.CopyToFami)),
            // ---
            new HotKey(HotKeyType.TransposeUp1, "Transpose +1", "Num +", () => UIActionManager.Instance.Execute(null, UIActionType.TransposeDown1)),
            new HotKey(HotKeyType.TransposeDown1, "Transpose -1", "Num -", () => UIActionManager.Instance.Execute(null, UIActionType.TransposeDown1)),
            new HotKey(HotKeyType.TransposeUp3, "Transpose +3", "Shift+Num +", () => UIActionManager.Instance.Execute(null, UIActionType.TransposeDown3)),
            new HotKey(HotKeyType.TransposeDown3, "Transpose -3", "Shift+Num -", () => UIActionManager.Instance.Execute(null, UIActionType.TransposeDown3)),
            new HotKey(HotKeyType.TransposeUp5, "Transpose +5", "Ctrl+Shift+Num +", () => UIActionManager.Instance.Execute(null, UIActionType.TransposeUp5)),
            new HotKey(HotKeyType.TransposeDown5, "Transpose -5", "Ctrl+Shift+Num -", () => UIActionManager.Instance.Execute(null, UIActionType.TransposeDown5)),
            new HotKey(HotKeyType.TransposeUp12, "Transpose +12", "Ctrl+Num +", () => UIActionManager.Instance.Execute(null, UIActionType.TransposeUp12)),
            new HotKey(HotKeyType.TransposeDown12, "Transpose -12", "Ctrl+Num -", () => UIActionManager.Instance.Execute(null, UIActionType.TransposeDown12)),
            // ---
            new HotKey(HotKeyType.ExpandPattern, "Expand Pattern", "Ctrl+Shift+/", () => UIActionManager.Instance.Execute(null, UIActionType.ExpandPattern)), // Globals.MainForm.ExpandTwice1.PerformClick()),
            new HotKey(HotKeyType.CompressPattern, "Compress Pattern", "Ctrl+Shift+Num *", () => UIActionManager.Instance.Execute(null, UIActionType.CompressPattern)), // Globals.MainForm.Compresspattern1.PerformClick()),
            new HotKey(HotKeyType.SplitPattern, "Split Pattern", "Alt+X", () => UIActionManager.Instance.Execute(null, UIActionType.SplitPattern)), // Globals.MainForm.Splitpattern1.PerformClick()),
            new HotKey(HotKeyType.PatternPacker, "Pattern Packer", "Ctrl+P", () => UIActionManager.Instance.Execute(null, UIActionType.PatternPacker)),
            // ---
            new HotKey(HotKeyType.SwapChannelsLeft, "Swap Selected Channels Left", "Ctrl+Alt+Left", () => UIActionManager.Instance.Execute(null, UIActionType.SwapChannelsLeft)),
            new HotKey(HotKeyType.SwapChannelsRight, "Swap Selected Channels Right", "Ctrl+Alt+Right", () => UIActionManager.Instance.Execute(null, UIActionType.SwapChannelsRight)),
            // ---
            new HotKey(HotKeyType.TracksManager, "Show Tracks Manager", "Ctrl+T", () => UIActionManager.Instance.Execute(null, UIActionType.TracksManager)), // Globals.MainForm.Tracksmanager1.PerformClick()),
            // ---
            new HotKey(HotKeyType.SetLoopPosition, "Set Loop Position", "L", () => UIActionManager.Instance.Execute(null, UIActionType.SetLoopPosition)), // Globals.MainForm.Setloopposition1.PerformClick()),
            // ---
            new HotKey(HotKeyType.InsertPositions, "Insert Positions", "Ins", () => UIActionManager.Instance.Execute(null, UIActionType.InsertPositions)), // Globals.MainForm.Insertposition1.PerformClick()),
            new HotKey(HotKeyType.DeletePositions, "Delete Positions", "Del", () => UIActionManager.Instance.Execute(null, UIActionType.DeletePositions)), // Globals.MainForm.Deleteposition1.PerformClick()),
            new HotKey(HotKeyType.DuplicatePositions, "Duplicate Positions", "Ctrl+D", () => UIActionManager.Instance.Execute(null, UIActionType.DuplicatePositions)), // Globals.MainForm.DuplicatePosition1.PerformClick()),
            new HotKey(HotKeyType.ClonePositions, "Clone Positions", "Ctrl+Shift+D", () => UIActionManager.Instance.Execute(null, UIActionType.ClonePositions)), // Globals.MainForm.ClonePosition1.PerformClick()),
            // ---
            new HotKey(HotKeyType.ToggleChip, "Toggle Chip", "Ctrl+Alt+C", () => UIActionManager.Instance.Execute(null, UIActionType.ToggleChip)),
            new HotKey(HotKeyType.ToggleChannels, "Toggle Channels", "Ctrl+Alt+A", () => UIActionManager.Instance.Execute(null, UIActionType.ToggleChannels)),
            // ---
            new HotKey(HotKeyType.UseLastNoteParams, "Use Last Note Params - On/Off", "Shift+Esc", () => UIActionManager.Instance.Execute(null, UIActionType.UseLastNoteParams)),
            new HotKey(HotKeyType.MoveBetweenPatterns, "Move Between Patterns - On/Off", "Shift+`", () => UIActionManager.Instance.Execute(null, UIActionType.MoveBetweenPatterns)),
            // ---
            new HotKey(HotKeyType.JumpPatternStart, "Jump To The Pattern First Line", "Ctrl+Home", () => UIActionManager.Instance.Execute(null, UIActionType.JumpPatternStart)),
            new HotKey(HotKeyType.JumpPatternEnd, "Jump To The Pattern Last Line", "Ctrl+End", () => UIActionManager.Instance.Execute(null, UIActionType.JumpPatternEnd)),
            new HotKey(HotKeyType.JumpLineStart, "Jump To The Line Start", "Home", () => UIActionManager.Instance.Execute(null, UIActionType.JumpLineStart)),
            new HotKey(HotKeyType.JumpLineEnd, "Jump To The Line End", "End", () => UIActionManager.Instance.Execute(null, UIActionType.JumpLineEnd)),
        };

        static HotKeys()
        {
            InitOptionsHotKeys();
        }

        public static void InitOptionsHotKeys()
        {
            ListView listView = Globals.OptionsForm.HotKeyList;
            listView.Items.Clear();

            foreach (var hotKey in AllHotKeys)
            {
                ListViewItem listViewItem = new ListViewItem(hotKey.Name);
                listViewItem.SubItems.Add(hotKey.ShortcutText);
                listViewItem.Tag = hotKey;

                listView.Items.Add(listViewItem);
            }
        }

        public static void SetDefaultHotKeys()
        {
            foreach (var hotKey in AllHotKeys)
                AssignHotKey(hotKey, hotKey.DefaultShortcutText);
        }

        public static void ReAssignHotKey(HotKey hotKey, string shortCutText)
        {
            if (SystemHotKeys.Contains(shortCutText))
            {
                MessageBox.Show(Globals.MainForm, $"Error: {hotKey.Name} Key {shortCutText} is a system key.");
                return;
            }

            if ((new[] { HotKeyType.JumpPatternStart, HotKeyType.JumpPatternEnd, HotKeyType.JumpLineStart, HotKeyType.JumpLineEnd }.Contains(hotKey.Type))
                && shortCutText.StartsWith("Shift"))
            {
                return;
            }

            for (int i = 0; i < AllHotKeys.Length; i++)
            {
                if (AllHotKeys[i] != hotKey && AllHotKeys[i].ShortcutText == shortCutText)
                {
                    var errorMessage = $"{hotKey.Name} Key {shortCutText} already assigned to {AllHotKeys[i].Name}. Assign anyway?";
                    
                    if (MessageBox.Show(Globals.MainForm, errorMessage, Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                        return;

                    AssignHotKey(AllHotKeys[i], "");
                }
            }

            AssignHotKey(hotKey, shortCutText);
        }

        public static void AssignHotKey(HotKey hotKey, string shortCutText)
        {
            hotKey.ShortcutText = shortCutText;

            ListViewItem listViewItem = Globals.OptionsForm.GetHotKeyListItem(hotKey);
            listViewItem.SubItems[1].Text = shortCutText;
        }

        public static bool HandleHotKey(Keys keyData)
        {
            foreach (var hotKey in AllHotKeys)
            {
                if (string.IsNullOrEmpty(hotKey.ShortcutText))
                    continue;

                var shortcut = TextToShortcut(hotKey.ShortcutText);

                if ((Keys)shortcut == keyData)
                {
                    hotKey.Action?.Invoke();
                    return true;
                }
            }

            return false;
        }

        public static bool MatchesShortcut(string shortcutText, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(shortcutText))
                return false;

            Keys keyOnly = e.KeyCode;
            Keys modifiers = e.Modifiers;

            var expected = (Keys)TextToShortcut(shortcutText);
            Keys expectedKey = expected & Keys.KeyCode;
            Keys expectedMods = expected & (Keys.Control | Keys.Shift | Keys.Alt);

            return (keyOnly == expectedKey) && (modifiers == expectedMods);
        }

        public static void ReadConfig(IniFile iniFile)
        {
            foreach (HotKey hotKey in AllHotKeys)
                ReAssignHotKey(hotKey, iniFile.GetValue("HotKeys", hotKey.Type.ToString(), hotKey.DefaultShortcutText));
        }

        public static void WriteConfig(IniFile iniFile)
        {
            foreach (HotKey hotKey in AllHotKeys)
                iniFile.SetValue("HotKeys", hotKey.Type.ToString(), hotKey.ShortcutText);
        }

        public static Shortcut TextToShortcut(string shortcutText)
        {
            if (string.IsNullOrWhiteSpace(shortcutText))
                return Shortcut.None;

            shortcutText = shortcutText.ToUpperInvariant().Replace("CTRL", "Control");

            if (Enum.TryParse(shortcutText.Replace("+", ""), out Shortcut parsed))
                return parsed;

            Keys keys = Keys.None;
            foreach (string part in shortcutText.Split('+'))
            {
                switch (part.Trim())
                {
                    case "CTRL":
                    case "CONTROL": keys |= Keys.Control; break;
                    case "SHIFT": keys |= Keys.Shift; break;
                    case "ALT": keys |= Keys.Alt; break;
                    default:
                        if (Enum.TryParse(part.Trim(), true, out Keys keyPart))
                            keys |= keyPart;
                        break;
                }
            }

            return (Shortcut)keys;
        }

        public static string ShortcutToText(Shortcut shortcut)
        {
            if (shortcut == Shortcut.None)
                return "";

            Keys keys = (Keys)shortcut;
            var parts = new List<string>();

            if (keys.HasFlag(Keys.Control)) parts.Add("Ctrl");
            if (keys.HasFlag(Keys.Shift)) parts.Add("Shift");
            if (keys.HasFlag(Keys.Alt)) parts.Add("Alt");

            Keys keyOnly = keys & ~Keys.Control & ~Keys.Shift & ~Keys.Alt;

            if (keyOnly != Keys.None)
                parts.Add(keyOnly.ToString());

            return string.Join("+", parts);
        }
    }
}

