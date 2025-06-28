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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VortexTracker.Win32;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Diagnostics;
using VortexTracker.Rendering;

namespace VortexTracker
{
    public class Samples : Control
    {
        public Font ArrowsFont = null;
        public int ArrowsFontW = 0;
        public int ArrowsFontH = 0;
        public int InputSNumber = 0;
        public int CharWidth = 0;
        public int CharHeight = 0;
        public int CursorX = 0;
        public int CursorY = 0;
        public int ShownFrom = 0;
        public int LineCount = 0;
        public Sample ShownSample = null;
        public int BigCaret = 0;
        public bool ToneShiftAsNote = false;
        public bool CaretVisible = false;
        public bool IsSelecting = false;
        public bool IsColSelecting = false;
        public int SelectionStart = 0;
        public int SelectionEnd = 0;
        public bool IsLineTesting = false;
        public int CurrentMidiNote = 0;
        public bool SamplesDontScroll = false;
        public ChildForm ParentWin = null;
        public bool UndoSaved = false;
        public int HintLastX = 0;
        public int HintLastY = 0;
        public int CopiedSample = -1;

        public int XPosLineNums = 0;
        public int XPosTone = 3;
        public int XPosNoise = 4;
        public int XPosEnvelope = 5;
        public int XPosToneSign = 7;
        public int XPosToneValue = 8;
        public int XPosToneAccumulationSign = 11;
        public int XPosNoiseSign = 13;
        public int XPosNoiseBracket1 = 16;
        public int XPosNoiseBracket2 = 19;
        public int XPosNoise1 = 14;
        public int XPosNoise2 = 17;
        public int XPosNoiseAccumulationSign = 20;
        public int XPosAmplitude = 22;
        public int XPosAmplitudeAccumulationSign = 23;
        public int XPosAmplitudeBars = 25;

        //public int Line;
        //public int X;
        //public int Y;
        public int SampleLength;
        //public int LastDataLine;
        public int LoopLine;
        //public int SepX;
        public bool SelectionLine;
        public bool Selection;
        public bool Highlight;
        public bool EmptySample;
        public sbyte BaseNote;
        public ushort BaseNoteFreq;
        public ushort LineFreq;
        public ushort PrevLineFreq;
        public ushort FreqAccum;
        public string PrevNoteStr;
        public NoteTableType NoteTableType;
        public ushort[] NoteTable;
        public Sample Sample;

        private System.Windows.Forms.Timer _doubleClickTimer;
        public bool MouseDoubleClicked;

        public Samples(Control parent)
            : base(parent, "Samples")
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.Opaque |
                ControlStyles.StandardClick |
                ControlStyles.StandardDoubleClick |
                ControlStyles.UseTextForAccessibility |
                ControlStyles.FixedHeight,
                true);
            this.UpdateStyles();
            this.DoubleBuffered = true;
            //Bitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            this.TabStop = true;
            //this.ParentColor = false;
            //this.BevelKind = bkTile;
            //this.BevelInner = bvLowered;
            LineCount = 16;
            CursorX = 0;
            CursorY = 0;
            ShownFrom = 0;
            ShownSample = null;
            CaretVisible = false;
            //ArrowsFont = new Font("Arrows", 8);

            if (MainForm.TryGetFontFamily("Arrows", out FontFamily fontFamily))
                ArrowsFont = new Font(fontFamily, 8);

            _doubleClickTimer = new System.Windows.Forms.Timer();
            _doubleClickTimer.Interval = 500; // Duration in milliseconds
            _doubleClickTimer.Tick += DoubleClickTimer_Tick;
        }

        ~Samples()
        {
            ArrowsFont.Dispose();
        }

        public void DoHint(int x, int y)
        {
//#if DEBUG
//            return;
//#endif
            //MainForm.HintHidePause = HideHintDelay;
            if (x == -1)
                x = CursorX;

            string s = "";
            switch (x)
            {
                case 0:
                    s = "Tone Mask:\r";
                    s += "[T] - On\r";
                    s += "[.] - Off\r\r";
                    s += "Mouse Click to Toggle.";
                    break;
                case 1:
                    s = "Noise Mask:\r\r";
                    s += "[N] - On\r";
                    s += "[.] - Off\r\r";
                    s += "Mouse Click to Toggle.";
                    break;
                case 2:
                    s = "Envelope Mask:\r\r";
                    s += "[E] - On\r";
                    s += "[.] - Off\r\r";
                    s += "Mouse Click to Toggle.";
                    break;
                case 4:
                    s = "+/- Tone Shift Sign.\r\r";
                    s += "[-] - Frequency Up\r";
                    s += "[+] - Frequency Down";
                    break;
                // 5 .. 7
                case 5:
                case 6:
                case 7:
                    s = "Tone Shift Value 000-FFF.";
                    break;
                case 8:
                    s = "Tone Shift Accumulation:\r\r";
                    s += "[^] - On\r";
                    s += "[.] - Off\r\r";
                    s += "Mouse Click to Toggle.";
                    break;
                case 10:
                    s = "Noise Frequency Shift Sign (Envelope On/Off). \r\r";
                    s += "[+] - Frequency Up\r";
                    s += "[-] - Frequency Down";
                    break;
                // 11 .. 12
                case 11:
                case 12:
                    s = "Noise Frequency Shift Value (Envelope Frequency).";
                    break;
                // 14 .. 15
                case 14:
                case 15:
                    s = "Absolute Noise Frequency Value (Envelope Frequency).";
                    break;
                case 17:
                    s = "Noise Shift Accumulation:\r\r";
                    s += "[^] - On\r";
                    s += "[.] - Off";
                    break;
                case 19:
                    s = "Volume: 0-F";
                    break;
                case 20:
                    s = "Volume Shift:\r\r";
                    s += "[^] - Volume Up\r";
                    s += "[v] - Volume Down\r";
                    s += "[.] - No Volume Shift";
                    break;
            }

            if (s == "")
                return;

            s += "\r\r";
            s += "Select Lines: Shift + Up/Down\r";
            s += "Select Lines: Shift + Drag Mouse\r";
            s += "Select Columns: Ctrl + Drag Mouse\r\r";
            s += "Change Loop and Length: Drag Right Mouse Button\r";
            s += "CTRL+C, CTRL+V - to Copy/Paste";

            Globals.MainForm.StatusBar.Items[0].Text = s;

            if (MainForm.DisableHints)
            {
                ParentWin.ToolTip.Hide(this);
                //this.ShowHint = false;
                //Application.CancelHint;
            }
            else
            {
                ParentWin.ToolTip.Show(s, this);
                //this.ShowHint = true;
                //this.Hint = s;
                if (HintLastX != x || HintLastY != y)
                {
                    //Application.CancelHint;
                    //ParentWin.HideHintTimer.Enabled = false;
                    //ParentWin.ShowHintTimer.Enabled = false;
                    //ParentWin.ShowHintTimer.Interval = TChildWin.ShowHintDelay;
                    //ParentWin.ShowHintTimer.Enabled = true;
                }
            }

            HintLastX = x;
            HintLastY = y;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_ERASEBKGND:
                    m.Result = IntPtr.Zero;
                    return;
                case WM_SYSCHAR:
                    if (GetKeyState(Keys.Menu) < 0)
                    {
                        PeekMessage(out _, IntPtr.Zero, WM_CHAR, WM_CHAR, PM_REMOVE);

                        m.Result = IntPtr.Zero;
                        return;
                    }
                    break;
                case Win32.WM_GETDLGCODE:
                    m.Result = new IntPtr(unchecked((int)(~DLGC_WANTTAB)));
                    return;
                case Win32.WM_SETFOCUS:
                    IntPtr focusedWnd = m.WParam;

                    if (!IsWindow(focusedWnd))
                        m.WParam = IntPtr.Zero;

                    InputSNumber = 0;
                    HideCaret();
                    CreateCaret();
                    SetCaretPosition();
                    ShowCaret();
                    m.Result = new IntPtr(-1);
                    break;
                case Win32.WM_KILLFOCUS:
                    CaretVisible = false;
                    DestroyCaret();
                     m.Result = new IntPtr(-1);
                    break;
                case WM_LBUTTONDOWN:
                case WM_MOUSEWHEEL:
                    if (SamplesDontScroll)
                        return;
                    break;
                case WM_LBUTTONUP:
                    SamplesDontScroll = false;
                    break;
            }

            base.WndProc(ref m);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Redraw(e.Graphics);
        }

        public void SetCaretPosition()
        {
            if (!this.Focused)
                return;

            SetCaretPos(CharWidth * (3 + CursorX), CharHeight * CursorY);
        }

        public void ShowCaret()
        {
            if (CaretVisible || IsSelecting || IsColSelecting)
                return;

            Win32.ShowCaret(this.Handle);
            CaretVisible = true;
        }

        public void HideCaret()
        {
            if (!CaretVisible)
                return;

            Win32.HideCaret(this.Handle);
            CaretVisible = false;
        }

        public int CurrentLine()
        {
            return ShownFrom + CursorY;
        }

        public void RecalcBaseNote(int newBaseNote)
        {
            int line, fromLine, toLine, delta;
            ushort baseNoteFreq;
            ushort newNoteFreq;

            if (ShownSample == null)
                return;

            /* if (!ParentWin.RecalcTonesBtn.PerformClick())
            {
                return;
            } */

            ParentWin.SaveSampleUndo(ShownSample);
            baseNoteFreq = (ushort)VTModule.GetNoteFreq(ParentWin.VTM.NoteTable, ParentWin.VTM.ReservedPattern.Lines[1].Channel[0].Note);
            newNoteFreq = (ushort)VTModule.GetNoteFreq(ParentWin.VTM.NoteTable, newBaseNote);
            delta = newNoteFreq - baseNoteFreq;

            if (IsSelecting)
            {
                fromLine = SelectionStart;
                toLine = SelectionEnd;
            }
            else if (IsColSelecting && MainForm.SampleCopy.FromColumn <= 4 && MainForm.SampleCopy.ToColumn >= 4)
            {
                fromLine = MainForm.SampleCopy.FromLine;
                toLine = MainForm.SampleCopy.ToLine;
            }
            else
            {
                fromLine = 0;
                toLine = ShownSample.Length - 1;
            }

            for (line = fromLine; line <= toLine; line++)
                ShownSample.Ticks[line].AddToTone = (short)(ShownSample.Ticks[line].AddToTone - delta);

            this.ParentWin.SongChanged = true;
            this.ParentWin.BackupSongChanged = true;

            HideCaret();
            Redraw();
            ShowCaret();

            this.ParentWin.SaveSampleRedo();
        }

        public void SetNote(sbyte note, int line, sbyte volume, bool redraw, bool calcOctave, bool setTone)
        {
            int i;
            ushort noteFreq;
            ushort baseNoteFreq;
            ushort freqAccum;
            SampleTick sampleLine;

            if (note == -1)
                return;

            if (note == -3)
                return;

            this.ParentWin.SongChanged = true;
            this.ParentWin.BackupSongChanged = true;

            // -2 - Sound off (R--)
            if (note == -2)
            {
                ShownSample.Ticks[line].Mixer_Ton = false;
                ShownSample.Ticks[line].Mixer_Noise = false;
                ShownSample.Ticks[line].Envelope_Enabled = false;
                ShownSample.Ticks[line].AddToTone = 0;
                ShownSample.Ticks[line].Ton_Accumulation = false;
                ShownSample.Ticks[line].Amplitude_Sliding = false;
                ShownSample.Ticks[line].Amplitude_Slide_Up = false;
                ShownSample.Ticks[line].Envelope_or_Noise_Accumulation = false;
                ShownSample.Ticks[line].Add_to_Envelope_or_Noise = 0;

                if (line > 0)
                    ShownSample.Ticks[line].Amplitude = ShownSample.Ticks[line - 1].Amplitude;

                return;
            }

            // Get note by octave
            if (calcOctave)
                note += (sbyte)((this.ParentWin.SampleOctaveNum.Value - 1) * 12);

            // Is note out of range?
            if ((uint)note >= 96)
                return;

            // Init
            freqAccum = 0;
            noteFreq = (ushort)VTModule.GetNoteFreq(ParentWin.VTM.NoteTable, note);
            baseNoteFreq = (ushort)VTModule.GetNoteFreq(ParentWin.VTM.NoteTable, ParentWin.VTM.ReservedPattern.Lines[1].Channel[0].Note);
            
            // Calculate frequency accumulation
            for (i = 0; i < line; i++)
            {
                sampleLine = ShownSample.Ticks[i];

                if (sampleLine.Ton_Accumulation)
                    freqAccum = (ushort)(freqAccum + sampleLine.AddToTone);
            }

            // Set tone shift
            ShownSample.Ticks[line].AddToTone = (sbyte)(noteFreq - baseNoteFreq - freqAccum);
            
            // Set T mask
            if (setTone)
                ShownSample.Ticks[line].Mixer_Ton = true;

            // Set volume
            if (volume > 0)
                ShownSample.Ticks[line].Amplitude = (byte)volume;

            // Redraw samples
            if (redraw)
            {
                HideCaret();
                Redraw();
                ShowCaret();
            }
        }

        public string FindNote(SampleTick sampleLine)
        {
            string result;
            int note;
            int nearestNote;
            ushort bestDistanceFoundYet;
            ushort freqFromTable;
            ushort d;
            string sign = "";

            LineFreq = (ushort)(BaseNoteFreq + sampleLine.AddToTone + FreqAccum);

            if (sampleLine.Ton_Accumulation)
                FreqAccum = (ushort)(FreqAccum + sampleLine.AddToTone);

            if (LineFreq == BaseNoteFreq)
            {
                result = '=' + VTModule.NoteToStr(BaseNote);
                PrevLineFreq = LineFreq;
                PrevNoteStr = result;
                return result;
            }

            if (LineFreq == PrevLineFreq)
            {
                if (PrevNoteStr == "")
                    PrevNoteStr = '=' + VTModule.NoteToStr(BaseNote);
                result = PrevNoteStr;
                return result;
            }

            // Search nearest freq in the tone table
            nearestNote = -1;
            bestDistanceFoundYet = 0xFFFF;
            for (note = NoteTable.Length - 1; note >= 0; note--)
            {
                freqFromTable = NoteTable[note];

                // desired frequency found
                if (freqFromTable == LineFreq)
                {
                    result = '=' + VTModule.NoteToStr(note);
                    return result;
                }

                d = (ushort)Math.Abs(LineFreq - freqFromTable);

                if (d < bestDistanceFoundYet)
                {
                    bestDistanceFoundYet = d;
                    nearestNote = note;
                    sign = LineFreq > freqFromTable ? "<" : ">";
                }
            }

            result = sign + VTModule.NoteToStr(nearestNote);
            PrevNoteStr = result;
            PrevLineFreq = LineFreq;

            return result;
        }

        public bool IsColSelected(int colNum, int line)
        {
            return IsColSelecting && (colNum >= MainForm.SampleCopy.FromColumn) && (colNum <= MainForm.SampleCopy.ToColumn) && (line >= MainForm.SampleCopy.FromLine) && (line <= MainForm.SampleCopy.ToLine);
        }

        public void Redraw()
        {
            this.Invalidate();
            this.Update();
        }

        public void Redraw(Graphics g)
        {
            string s;
            string samStr;
            Color textColor;
            Color bgColor;
            int line;
            int x, y;
            int lastDataLine;

            if (!Visible)
                return;

            if (this.ParentWin == null)
                return;

            if (this.ParentWin.VTM == null)
                return;

            if (this.ParentWin.IsClosed)
                return;

            if (ShownSample == null)
            {
                Sample = new Sample();
                ShownSample = Sample;
                EmptySample = true;
            }
            else
                EmptySample = false;

            SampleLength = ShownSample.Length;
            LoopLine = ShownSample.Loop;

            // Get:
            // 1. Tone table of track
            // 2. Base note from testline
            // 3. Base note frequence.
            NoteTableType = ParentWin.VTM.NoteTable;
            BaseNote = ParentWin.VTM.ReservedPattern.Lines[1].Channel[0].Note;
            BaseNoteFreq = (ushort)VTModule.GetNoteFreq(NoteTableType, BaseNote);
            LineFreq = 0;
            FreqAccum = 0;
            PrevLineFreq = 0;

            switch (NoteTableType)
            {
                case NoteTableType.ProTracker:
                    NoteTable = VTModule.PT3NoteTable_PT;
                    break;
                case NoteTableType.SoundTracker:
                    NoteTable = VTModule.PT3NoteTable_ST;
                    break;
                case NoteTableType.ASM:
                    NoteTable = VTModule.PT3NoteTable_ASM;
                    break;
                case NoteTableType.Real:
                    NoteTable = VTModule.PT3NoteTable_Real;
                    break;
                case NoteTableType.Natural:
                    NoteTable = VTModule.PT3NoteTable_Natural;
                    break;
                default:
                    NoteTable = VTModule.CustomNoteTable;
                    break;
            }

            // Search last not empty line
            lastDataLine = 0;

            for (int i = VTModule.MaxSampleLength - 1; i >= ShownSample.Length; i--)
            {
                SampleTick sampleTick = ShownSample.Ticks[i];
                if (sampleTick.AddToTone != 0 || sampleTick.Amplitude != 0 || sampleTick.Add_to_Envelope_or_Noise != 0 || sampleTick.Ton_Accumulation || sampleTick.Amplitude_Sliding || sampleTick.Envelope_Enabled || sampleTick.Envelope_or_Noise_Accumulation || sampleTick.Mixer_Ton || sampleTick.Mixer_Noise)
                {
                    lastDataLine = i;
                    break;
                }
            }

            if (lastDataLine < ShownSample.Length - 1)
                lastDataLine = ShownSample.Length - 1;

            // Fill background color
            bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnBackground];
            GDI.FillRectangle(g, new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height), bgColor);

            y = 0;

            for (line = ShownFrom; line <= ShownFrom + LineCount; line++)
            {
                // Finish
                if (line >= VTModule.MaxSampleLength)
                    break;

                // Start
                if (line < LoopLine || line >= SampleLength)
                {
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnBackground];
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnText];
                    SelectionLine = false;
                }
                else
                {
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnBackground];
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelText];
                    SelectionLine = true;
                }

                Highlight = false;
                Selection = IsSelecting && line >= SelectionStart && line <= SelectionEnd;

                if (Selection)
                {
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelBackground];
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelLineNum];
                }
                else if (MainForm.HighlightSpeedOn && (this.ParentWin.VTM.InitialDelay != 0) && ((line % this.ParentWin.VTM.InitialDelay) == 0) && (line < SampleLength))
                {
                    Highlight = true;
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.HighlBackground];
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelText];
                }

                // Get line string
                samStr = VTModule.GetSampleStringForRedraw(ShownSample.Ticks[line]);
                GDI.DrawText(g, this.Font, 3 * CharWidth, y, samStr, textColor, bgColor);

                // Is T column selected?
                if (IsColSelected(1, line))
                {
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelLineNum];
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelBackground];
                    GDI.DrawText(g, this.Font, XPosTone * CharWidth, y, samStr[0].ToString(), textColor, bgColor);
                }

                // Is N column selected?
                if (IsColSelected(2, line))
                {
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelLineNum];
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelBackground];
                    GDI.DrawText(g, this.Font, XPosNoise * CharWidth, y, samStr[1].ToString(), textColor, bgColor);
                }

                // Is E column selected?
                if (IsColSelected(3, line))
                {
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelLineNum];
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelBackground];
                    GDI.DrawText(g, this.Font, XPosEnvelope * CharWidth, y, samStr[2].ToString(), textColor, bgColor);
                }

                // Is Tone Shift column selected?
                if (IsColSelected(4, line))
                {
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelLineNum];
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelBackground];
                    GDI.DrawText(g, this.Font, XPosToneSign * CharWidth, y, samStr[4].ToString(), textColor, bgColor);
                    GDI.DrawText(g, this.Font, XPosToneAccumulationSign * CharWidth, y, samStr[8].ToString(), textColor, bgColor);
                }

                // Is Noise column selected?
                if (IsColSelected(5, line))
                {
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelLineNum];
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelBackground];
                    GDI.DrawText(g, this.Font, XPosNoiseSign * CharWidth, y, samStr[10].ToString(), textColor, bgColor);
                    GDI.DrawText(g, this.Font, XPosNoiseBracket1 * CharWidth, y, "(", textColor, bgColor);
                    GDI.DrawText(g, this.Font, XPosNoiseBracket2 * CharWidth, y, ")", textColor, bgColor);
                    GDI.DrawText(g, this.Font, XPosNoiseAccumulationSign * CharWidth, y, samStr[17].ToString(), textColor, bgColor);
                }

                // Is Amplitude value column selected?
                if (IsColSelected(6, line))
                {
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelLineNum];
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelBackground];
                    GDI.DrawText(g, this.Font, XPosAmplitude * CharWidth, y, samStr[19].ToString(), textColor, bgColor);
                    GDI.DrawText(g, this.Font, XPosAmplitudeAccumulationSign * CharWidth, y, samStr[20].ToString(), textColor, bgColor);
                    GDI.DrawText(g, this.Font, XPosAmplitudeBars * CharWidth, y, new String(' ', 16), textColor, bgColor);
                }

                if (SelectionLine)
                {
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelBackground];
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelLineNum];
                }
                else
                {
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnBackground];
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnLineNum];
                }

                if (MainForm.DecBaseLinesOn)
                {
                    s = string.Format("%.2d", line % 100);
                    
                    if (line == 100)
                        s = "@1";

                    if (line == 200)
                        s = "@2";
                }
                else
                    s = line.ToString("X2");

                // Background layer for loops
                if (SelectionLine)
                {
                    GDI.DrawText(g, this.Font, XPosLineNums, y, " ", textColor, bgColor);
                    GDI.DrawText(g, this.Font, XPosLineNums + 2 * CharWidth - (CharWidth / 2), y, " ", textColor, bgColor);
                }

                // Draw line numbers
                GDI.DrawText(g, this.Font, XPosLineNums + 3, y, s, textColor, bgColor);
                bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnBackground];
                
                // Tone shift
                if (Selection || IsColSelected(4, line))
                {
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelLineNum];
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelBackground];
                }
                else if (Highlight)
                {
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelText];
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.HighlBackground];
                }
                else if (SelectionLine)
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelTone];
                else
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnTone];

                if (ToneShiftAsNote)
                {
                    if (line > lastDataLine)
                        s = "+000";
                    else if (BaseNote != -2)
                        s = FindNote(ShownSample.Ticks[line]);
                    else
                        s = "+000";

                    GDI.DrawText(g, this.Font, XPosToneValue * CharWidth, y, s[1].ToString() + s[2].ToString() + s[3].ToString(), textColor, bgColor);
                    
                    if (Highlight || Selection)
                    {
                    }
                    else if (SelectionLine)
                        textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelText];
                    else
                        textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnText];

                    GDI.DrawText(g, this.Font, XPosToneSign * CharWidth, y, s[0].ToString(), textColor, bgColor);
                }
                else
                    GDI.DrawText(g, this.Font, XPosToneValue * CharWidth, y, samStr[5].ToString() + samStr[6].ToString() + samStr[7].ToString(), textColor, bgColor);

                // Tone accumulation
                if (ShownSample.Ticks[line].Ton_Accumulation)
                {
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnText];
                    GDI.DrawTriangleUp(g, this.ArrowsFont, XPosToneAccumulationSign * CharWidth + 2, y + (CharHeight / 2) - (ArrowsFontH / 2), textColor, bgColor);
                }

                // Noise
                bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnBackground];
                
                if (Selection || IsColSelected(5, line))
                {
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelLineNum];
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelBackground];
                }
                else if (Highlight)
                {
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelText];
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.HighlBackground];
                }
                else if (SelectionLine)
                {
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamSelNoise];
                }
                else
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamNoise];

                GDI.DrawText(g, this.Font, XPosNoise1 * CharWidth, y, samStr[11].ToString() + samStr[12].ToString(), textColor, bgColor);
                GDI.DrawText(g, this.Font, XPosNoise2 * CharWidth, y, samStr[14].ToString() + samStr[15].ToString(), textColor, bgColor);
                
                // Noise accumulation
                if (ShownSample.Ticks[line].Envelope_or_Noise_Accumulation)
                {
                    if (Selection)
                        textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelLineNum];
                    else
                        textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnText];

                    GDI.DrawTriangleUp(g, this.ArrowsFont, XPosNoiseAccumulationSign * CharWidth + 2, y + (CharHeight / 2) - (ArrowsFontH / 2), textColor, bgColor);
                }
                
                // Amplitude sliding sign
                if (ShownSample.Ticks[line].Amplitude_Sliding)
                {
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnBackground];

                    if (Selection || IsColSelected(6, line))
                    {
                        textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelLineNum];
                        bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelBackground];
                    }
                    else if (Highlight)
                    {
                        textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelText];
                        bgColor = MainForm.ThemeColors[(int)ThemeColor.HighlBackground];
                    }
                    else if (SelectionLine)
                        textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelText];
                    else
                        textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnText];

                    if (ShownSample.Ticks[line].Amplitude_Slide_Up)
                        GDI.DrawTriangleUp(g, this.ArrowsFont, XPosAmplitudeAccumulationSign * CharWidth + 2, y + (CharHeight / 2) - (ArrowsFontH / 2), textColor, bgColor);
                    else
                        GDI.DrawTriangleDown(g, this.ArrowsFont, XPosAmplitudeAccumulationSign * CharWidth, y + (CharHeight / 2) - (ArrowsFontH / 2), textColor, bgColor);
                }

                // Volume
                if (Highlight)
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelText];
                else if (Selection || IsColSelected(6, line))
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelLineNum];
                else if (SelectionLine)
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelText];
                else
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnText];

                if (ShownSample.Ticks[line].Amplitude > 0)
                {
                    for (int i = 0; i < ShownSample.Ticks[line].Amplitude; i++)
                    {
                        x = (XPosAmplitudeBars + i) * CharWidth;
                        GDI.FillRectangle(g, Rectangle.FromLTRB(x, y + 2, x + CharWidth - 2, y + CharHeight - 1), bgColor);
                    }
                }

                y += CharHeight;
            }

            // Separator
            int sepX = (2 * CharWidth) + (CharWidth / 2);
            bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSeparators];
            GDI.FillRectangle(g, Rectangle.FromLTRB(sepX, 0, sepX + 2, y), bgColor);

            if (EmptySample)
                ShownSample = null;

            if (ParentWin.SamplesGrid.CurrentCell != null)
                ParentWin.RedrawSamplesCell(ParentWin.SamplesGrid.CurrentCell.ColumnIndex, true);
        }

        public void CreateCaret()
        {
            DoHint(-1, -1);

            if (CursorX == 5)
            {
                BigCaret = 1;
                Win32.CreateCaret(this.Handle, 0, CharWidth * 3, CharHeight);
            }
            else if (CursorX == 11 || CursorX == 14)
            {
                BigCaret = -1;
                Win32.CreateCaret(this.Handle, 0, CharWidth * 2, CharHeight);
            }
            else
            {
                BigCaret = 0;
                Win32.CreateCaret(this.Handle, 0, CharWidth, CharHeight);
            }
        }

        public void RecreateCaret()
        {
            DoHint(-1, -1);

            if ((CursorX == 5 && BigCaret != 1) || ((CursorX == 11 || CursorX == 14) && BigCaret != -1) || (!(CursorX == 5 || CursorX == 11 || CursorX == 14) && BigCaret != 0))
            {
                HideCaret();
                DestroyCaret();
                CreateCaret();
                ShowCaret();
            }
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseDoubleClicked = true;
                _doubleClickTimer.Start();
            }
        }

        private void DoubleClickTimer_Tick(object sender, EventArgs e)
        {
            MouseDoubleClicked = false;
            _doubleClickTimer.Stop();
        }
    }
}
