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
using VortexTracker.Rendering;

namespace VortexTracker
{
    public class Ornaments : Control
    {
        public int InputONumber = 0;
        public int CharWidth = 0;
        public int CharHeight = 0;
        public int CursorInt = 0;
        public int CursorX = 0;
        public int CursorY = 0;
        public int ShownFrom = 0;
        public int LineCount = 0;
        public Ornament ShownOrnament = null;
        public int NRaw = 0;
        public bool ToneShiftAsNote = false;
        public bool IsSelecting = false;
        public bool CaretVisible = false;
        public int SelectionStart = 0;
        public int SelectionEnd = 0;
        public bool IsLineTesting = false;
        public Keys KeyPressed = 0;
        public int CurrentMidiNote = 0;
        public short ClickStartLine = 0;
        public short ClickMouseCursorY = 0;
        public bool LeftMouseButton = false;
        public short ClickEndLine = 0;
        public bool RightMouseButton = false;
        public bool LoopStarted = false;
        public bool UndoSaved = false;
        public ChildForm ParentWin = null;
        public FileBrowser Browser = null;
        public ChannelLine SavedSampleTestLine;
        public int CopiedOrnament = -1;

        public Ornaments(Control parent) :
            base(parent, "Ornaments")
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.Opaque |
                ControlStyles.StandardClick |
                ControlStyles.UseTextForAccessibility |
                ControlStyles.FixedHeight,
                true);
            this.UpdateStyles();
            this.DoubleBuffered = true;
            this.TabStop = true;
            this.Font = Globals.MainForm.EditorFont;
            LineCount = MainForm.OrnColumnCount * NRaw;
            CursorX = 0;
            CursorY = 0;
            ShownFrom = 0;
            ShownOrnament = null;
            CaretVisible = false;
        }

        ~Ornaments()
        {
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
                case WM_GETDLGCODE:
                    m.Result = new IntPtr(unchecked((int)(~DLGC_WANTTAB)));
                    return;
                case WM_SETFOCUS:
                     IntPtr focusedWnd = m.WParam;

                    if (!IsWindow(focusedWnd))
                        m.WParam = IntPtr.Zero;

                    InputONumber = 0;
                    HideCaret();
                      CreateCaret(this.Handle, 0, CharWidth * 3, CharHeight);
                    SetCaretPosition();
                    ShowCaret();
                    m.Result = new IntPtr(-1);
                    break;
                case WM_KILLFOCUS:
                    CaretVisible = false;
                    DestroyCaret();
                    m.Result = new IntPtr(-1);
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

            SetCaretPos(CharWidth * (3 + CursorX + MainForm.OrnXShift), CharHeight * CursorY);
        }

        public void ShowCaret()
        {
            if (CaretVisible || IsSelecting)
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

        public void InitMetrics()
        {
            Size textSize;

            if (MainForm.DecBaseLinesOn)
            {
                MainForm.OrnCharCount = 10;
                MainForm.OrnXShift = 1;
            }
            else
            {
                MainForm.OrnCharCount = 9;
                MainForm.OrnXShift = 0;
            }

            this.Font = Globals.MainForm.EditorFont;

            using (Graphics g = Graphics.FromHwnd(this.Handle))
                textSize = TextRenderer.MeasureText(g, "0", this.Font, Size.Empty, TextFormatFlags.NoPadding);

            CharWidth = textSize.Width;
            CharHeight = textSize.Height;

            this.ClientSize = new Size(CharWidth * MainForm.OrnColumnCount * MainForm.OrnCharCount, this.ClientSize.Height);

            if (this.ClientSize.Width < 400)
                this.ClientSize = new Size(400, this.ClientSize.Height);
        }

        public void ClearSelection()
        {
            if (!IsSelecting)
                return;

            for (int i = SelectionStart; i <= SelectionEnd; i++)
                ShownOrnament.Offsets[i] = 0;

            IsSelecting = false;

            HideCaret();
            Redraw();
            ShowCaret();
        }

        public void CopyToClipBoard()
        {
            ParentWin.CopyOrnamentToBuffer(false);
        }

        public void CutToClipBoard()
        {
            if (!IsSelecting)
                return;

            CopyToClipBoard();
            ParentWin.SaveOrnamentUndo();
            ShownOrnament.Loop = 0;
            ShownOrnament.Length = 1;
            ParentWin.OrnamentLoopUpDown.Value = 0;
            ParentWin.OrnamentLenUpDown.Value = 1;
            ParentWin.SaveOrnamentRedo();
            IsSelecting = true;
            ClearSelection();
        }

        public int CurrentLine()
        {
            return ShownFrom + (CursorY + (CursorX / MainForm.OrnCharCount) * NRaw);
        }

        public void SetNote(sbyte note, int line, bool calcOctave, bool redraw)
        {
            sbyte baseNote;
            this.ParentWin.SongChanged = true;
            this.ParentWin.BackupSongChanged = true;
            ParentWin.ValidateOrnament(ParentWin.OrnamentIndex);
            baseNote = ParentWin.VTM.ReservedPattern.Lines[0].Channel[0].Note;

            if (calcOctave)
                note += (sbyte)((this.ParentWin.OrnamentOctaveNum.Value - 1) * 12);

            if ((uint)note >= 96)
                return;

            ShownOrnament.Offsets[line] = (sbyte)(note - baseNote);

            if (redraw)
            {
                HideCaret();
                Redraw();
                ShowCaret();
            }
        }

        public void Redraw()
        {
            this.Invalidate();
            this.Update();
        }

        public void Redraw(Graphics g)
        {
            int line, ornLength, loop, x, y, num, i, lastDataLine;
            string s;
            bool selLine, selection;
            sbyte baseNote;
            Color textColor;
            Color bgColor;

            if (!this.ParentWin.Visible)
                return;

            if (this.ParentWin.VTM == null)
                return;

            if (this.ParentWin.IsClosed)
                return;

            baseNote = ParentWin.VTM.ReservedPattern.Lines[0].Channel[0].Note;

            if (ShownOrnament == null)
            {
                ShownOrnament = new Ornament();
                this.ParentWin.ValidateOrnament(this.ParentWin.OrnamentIndex);
            }

            x = 0;
            y = 0;
            ornLength = ShownOrnament.Length;
            loop = ShownOrnament.Loop;

            // Search last not empty line
            lastDataLine = 0;

            for (i = VTModule.MaxOrnamentLength - 1; i >= ShownOrnament.Length; i--)
            {
                if (ShownOrnament.Offsets[i] > 0)
                {
                    lastDataLine = i;
                    break;
                }
            }

            if (lastDataLine < ShownOrnament.Length - 1)
                lastDataLine = ShownOrnament.Length - 1;

            bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnBackground];
            GDI.FillRectangle(g, new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height), bgColor);

            for (line = ShownFrom; line < ShownFrom + LineCount; line++)
            {
                if (line > VTModule.MaxOrnamentLength)
                    break;

                if (line < ornLength && line >= loop)
                {
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelTone];
                    selLine = true;
                }
                else
                {
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnTone];
                    selLine = false;
                }

                bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnBackground];
                selection = IsSelecting && line <= SelectionEnd && line >= SelectionStart;

                if (selection)
                {
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelBackground];
                    textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelLineNum];
                }
                else if (Globals.MainForm.MdiChildren.Length != 0 && ParentWin.VTM.InitialDelay != 0)
                {
                    if ((line % ParentWin.VTM.InitialDelay) == 0 && line < ornLength && MainForm.HighlightSpeedOn)
                    {
                        bgColor = MainForm.ThemeColors[(int)ThemeColor.HighlBackground];
                        textColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelText];
                    }
                }

                // Ornament items
                if (ToneShiftAsNote)
                {
                    if (line > lastDataLine)
                        s = "+00 ";
                    else if (ShownOrnament.Offsets[line] > 91)
                        s = "C-1";
                    else
                        s = $"{VTModule.NoteToStr(baseNote + ShownOrnament.Offsets[line])} ";
                }
                else if (ShownOrnament == null)
                    s = "+00 ";
                else if (ShownOrnament.Offsets[line] >= 0)
                    s = $"+{ShownOrnament.Offsets[line]:D2} ";
                else
                    s = $"-{-ShownOrnament.Offsets[line]:D2} ";

                GDI.DrawText(g, this.Font, (3 + MainForm.OrnXShift) * CharWidth + x, y, s, textColor, bgColor);

                // Line numbers
                if (selLine)
                {
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelBackground];
                    GDI.DrawText(g, this.Font, x, y, " ", textColor, bgColor);
                    GDI.DrawText(g, this.Font, x + ((2 + MainForm.OrnXShift) * CharWidth) - (CharWidth / 2), y, " ", textColor, bgColor);
                }
                else
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnBackground];

                if (MainForm.DecBaseLinesOn)
                    s = string.Format("%.3d", line);
                else
                    s = line.ToString("X2");

                textColor = MainForm.ThemeColors[selLine ? (int)ThemeColor.SamOrnSelLineNum : (int)ThemeColor.SamOrnLineNum];

                GDI.DrawText(g, this.Font, x + 2, y, s, textColor, bgColor);

                if ((line - ShownFrom) % NRaw == NRaw - 1)
                {
                    y = 0;
                    x += CharWidth * MainForm.OrnCharCount;
                }
                else
                    y += CharHeight;
            }

            // Separators
            x = 0;

            for (i = 0; i < MainForm.OrnColumnCount; i++)
            {
                num = x + (2 * CharWidth) + (CharWidth / 2) + (MainForm.OrnXShift * CharWidth);
                GDI.FillRectangle(g, Rectangle.FromLTRB(num, 0, num + 2, NRaw * CharHeight), MainForm.ThemeColors[(int)ThemeColor.SamOrnSeparators]);
                x += CharWidth * MainForm.OrnCharCount;
            }

            if (ParentWin.OrnamentsGrid.CurrentCell != null)
                ParentWin.RedrawOrnamentsCell(ParentWin.OrnamentsGrid.CurrentCell.ColumnIndex, true);
        }

        public void DoHint()
        {
            string s = "";
            //#if DEBUG
            //            return;
            //#endif

            ParentWin.ToolTip.AutomaticDelay = 9300;

            if (CursorX == 0 || CursorX == 9 || CursorX == 18 || CursorX == 27)
            {
                s = "Half Shift Tone.\r\r";
                s += "Right Mouse Button for -/+\r";
                s += "Shift + Cursor Up/Down - Select Lines\r";
                s += "Shift + Drag Mouse - Select Lines\r\r";
                s += "CTRL+C, CTRL+V - To Copy/Paste\r";
                s += "Drag Right Mouse Button for Length & Loop";
            }

            Globals.MainForm.StatusBar.Items[0].Text = s;

            if (MainForm.DisableHints)
            {
                //this.ShowHint = false;
                ParentWin.ToolTip.Hide(this);
            }
            else
            {
                //this.ShowHint = true;
                ParentWin.ToolTip.SetToolTip(this, s);
                //this.Hint = s;
            }
        }
    }

    public enum TOrnToggles
    {
        Sgn,
        SgnP,
        SgnM
    };
}
