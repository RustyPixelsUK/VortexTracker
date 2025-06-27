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
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Diagnostics;
using OpenTK.GLControl;

namespace VortexTracker
{
    public class Tracks : Control
    {
        public int CharWidth = 0;
        public int CharHeight = 0;
        public int CursorX = 0;
        public int CursorY = 0;
        public int SelectionX = 0;
        public int SelectionY = 0;
        public int ShownFrom = 0;
        public int VisibleLineCount = 0;
        public int CenterLineIndex = 0;
        public bool ReturnAfterPlay = false;
        public int ReturnCursorY = 0;
        public int ReturnShownFrom = 0;
        public int ReturnPosition = 0;
        public int HLStep = 0;
        public Pattern ShownPattern = null;
        private bool BigCaret = false;
        public Keys KeyPressed = Keys.None;
        public bool Clicked = false;
        public int CurrentMidiNote = 0;
        public LastNoteParams[] LastNoteParams;
        private bool CaretVisible = false;
        public ChildForm ParentWin = null;
        public short[] SeperatorX = new short[6];
        private short Shift = 0;
        public short PatternCharCount = 0;
        public short PatternWidth = 0;
        public ChannelState[] ChannelState;
        public bool RedrawDisabled = false;
        public bool ManualBitBlt = false;
        public bool SelectionVisible = false;

        private VortexTracker.Rendering.OpenGL _openGL = null;

        public Tracks(Control parent) :
            base(parent, null)
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.Opaque |
                ControlStyles.StandardClick |
                ControlStyles.UseTextForAccessibility |
                ControlStyles.StandardDoubleClick |
                ControlStyles.FixedHeight |
                ControlStyles.Selectable,
                true);
            this.UpdateStyles();
            this.DoubleBuffered = true;
            this.TabStop = true;
            ChannelState = new ChannelState[3];
            ChannelState[0] = new ChannelState();
            ChannelState[1] = new ChannelState();
            ChannelState[2] = new ChannelState();
            HLStep = 4;
            KeyPressed = 0;
            ShownFrom = 0;
            CaretVisible = false;
            CursorX = 0;
            CursorY = 0;
            Clicked = false;
            ShownPattern = null;
            RedrawDisabled = false;
        }

        ~Tracks()
        {
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            this.Font = Globals.MainForm.EditorFont;

            _openGL = new VortexTracker.Rendering.OpenGL(this, new Font[] { this.Font }, Color.White, DrawTracks);
            _openGL.KeyDown += (s, e) => this.OnKeyDown(e);
            _openGL.KeyUp += (s, e) => this.OnKeyUp(e);
            _openGL.Leave += (s, e) => this.OnLeave(e);
            _openGL.MouseDown += (s, e) => this.OnMouseDown(e);
            _openGL.MouseMove += (s, e) => this.OnMouseMove(e);
            _openGL.MouseWheel += (s, e) => this.OnMouseWheel(e);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_ERASEBKGND:
                    // Prevent flickering by skipping background erase
                    m.Result = IntPtr.Zero;
                    return;

                case WM_SYSCHAR:
                    if ((GetKeyState(Keys.Menu) & 0x8000) != 0)
                    {
                        PeekMessage(out _, IntPtr.Zero, WM_CHAR, WM_CHAR, PM_REMOVE);
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

                    HideCaret();
                    CreateCaret();
                    SetCaretPosition();
                    RedrawTracks();
                    ShowCaret();
                    ParentWin.ShowStat();
                    m.Result = new IntPtr(-1);
                    break;
                case WM_KILLFOCUS:
                    HideCaret();
                    //DestroyCaret();
                    RemoveSelection();
                    RedrawTracks();
                    CaretVisible = false;
                    Clicked = false;
                    m.Result = new IntPtr(-1);
                    break;
            }

            base.WndProc(ref m);
        }

        public void SelectAll()
        {
            ShownFrom = 0;
            CursorY = CenterLineIndex;
            CursorX = 0;
            SelectionY = ShownPattern == null ? VTModule.DefaultPatternLength - 1 : ShownPattern.Length - 1;
            SelectionX = 48;
            HideCaret();
            ShowSelection();
            RedrawTracks();
            RecreateCaret();
            SetCaretPosition();
            ShowCaret();
            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
            activeForm.ShowStat();
        }

        public void FitNumberOfLines()
        {
            VisibleLineCount = (this.Height / CharHeight) + 1;
            CenterLineIndex = VisibleLineCount / 2;
            CursorY = CenterLineIndex;
        }

        public int CurrentPatternLine()
        {
            return ShownFrom - CenterLineIndex + CursorY;
        }

        public int CurrentChannel()
        {
            return MainForm.ChanAlloc[(CursorX - 8) / 14];
        }

        public bool IsTrackPlaying()
        {
            if (ParentWin == null)
                return false;

            return ParentWin.PlayStopState == PlayStopState.Stop && (AY.PlayMode == PlayModes.PlayModule || AY.PlayMode == PlayModes.PlayPattern);
        }

        public bool IsSelected()
        {
            return SelectionX != CursorX || SelectionY != CurrentPatternLine();
        }

        public void ShowSelection()
        {
            SelectionVisible = true;
            
            _openGL.ShowSelection();

            if (IsTrackPlaying())
            {
                _openGL.HideSelection();
                SelectionVisible = false;
            }
        }

        public void DrawSelection()
        {
            int x1, x2;
            int y1, y2;
            int width;

            if (!SelectionVisible)
                return;

            y1 = SelectionY - ShownFrom + CenterLineIndex;
            y2 = CursorY;

            if (y1 > y2)
            {
                y2 = y1;
                y1 = CursorY;
            }

            if (y1 < 0)
                y1 = 0;

            if (y2 >= VisibleLineCount)
                y2 = VisibleLineCount - 1;

            if (y1 > y2)
                return;

            x1 = SelectionX;
            x2 = CursorX;

            if (x1 > x2)
            {
                x1 = x2;
                x2 = SelectionX;
            }

            width = 1;

            if (x2 == 8 || x2 == 22 || x2 == 36)
                width = 3;

            _openGL.SetSelectionRect(Rectangle.FromLTRB((x1 + MainForm.TracksCursorXLeft) * CharWidth, y1 * CharHeight, (x2 + MainForm.TracksCursorXLeft + width) * CharWidth, (y2 + 1) * CharHeight));
        }

        public void RemoveSelection()
        {
            SelectionVisible = false;
            _openGL.HideSelection();
            SelectionX = CursorX;
            SelectionY = CurrentPatternLine();
        }

        public void ResetLastNoteParams(byte pattern, byte line, byte channel)
        {
            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
            ChannelLine channelLine = activeForm.VTM.Patterns[pattern].Lines[line].Channel[channel];
            activeForm.Tracks.LastNoteParams[channel].Line = line;
            activeForm.Tracks.LastNoteParams[channel].Sample = channelLine.Sample;
            activeForm.Tracks.LastNoteParams[channel].Envelope = channelLine.Envelope;
            activeForm.Tracks.LastNoteParams[channel].Ornament = channelLine.Ornament;
            activeForm.Tracks.LastNoteParams[channel].Volume = channelLine.Volume;
        }

        public void RemoveSelection(bool isShiftDown)
        {
            if (isShiftDown)
                return;

            HideCaret();
            RemoveSelection();
            ShowCaret();
        }

        public void JumpToPatStart(KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;

            RemoveSelection(isShiftDown);
            ShownFrom = 0;
            CursorY = CenterLineIndex;

            if (isShiftDown)
                ShowSelection();
            else
                RemoveSelection();

            HideCaret();
            RedrawTracks();
            SetCaretPosition();
            ShowCaret();
        }

        public void JumpToPatEnd(KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;

            RemoveSelection(isShiftDown);

            int patternLength;

            if (ShownPattern == null)
                patternLength = VTModule.DefaultPatternLength;
            else
                patternLength = ShownPattern.Length;

            ShownFrom = patternLength - 1;
            CursorY = CenterLineIndex;

            if (isShiftDown)
                ShowSelection();
            else
                RemoveSelection();

            HideCaret();
            RedrawTracks();
            SetCaretPosition();
            ShowCaret();
        }

        public void JumpToLineStart(KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;

            RemoveSelection(isShiftDown);

            CursorX = 0;

            if (isShiftDown)
                ShowSelection();
            else
                RemoveSelection();

            HideCaret();
            RedrawTracks();
            RecreateCaret();
            SetCaretPosition();
            ShowCaret();
        }

        public void JumpToLineEnd(KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;

            RemoveSelection(isShiftDown);
            CursorX = 48;

            if (isShiftDown)
                ShowSelection();
            else
                RemoveSelection();

            HideCaret();
            RedrawTracks();
            RecreateCaret();
            SetCaretPosition();
            ShowCaret();
        }

        public void InitMetrics()
        {
            int charHalfWidth;
            this.Font = Globals.MainForm.EditorFont;
            Size sz = _openGL.MeasureText(this.Font, "0");
            CharWidth = sz.Width;
            CharHeight = sz.Height;

            // Pattern width
            if (MainForm.DecBaseLinesOn)
            {
                PatternCharCount = 53;
                Shift = 1;
            }
            else
            {
                PatternCharCount = 52;
                Shift = 0;
            }

            PatternWidth = (short)(PatternCharCount * CharWidth + 2);

            // Separators
            charHalfWidth = (CharWidth / 2);
            SeperatorX[0] = (short)((2 + Shift) * CharWidth + charHalfWidth);
            SeperatorX[1] = (short)((7 + Shift) * CharWidth + charHalfWidth);
            SeperatorX[2] = (short)((10 + Shift) * CharWidth + charHalfWidth - 1);
            SeperatorX[3] = (short)((24 + Shift) * CharWidth + charHalfWidth - 1);
            SeperatorX[4] = (short)((38 + Shift) * CharWidth + charHalfWidth - 1);
            SeperatorX[5] = PatternWidth;

            // Pattern box size
            this.ClientSize = new Size(PatternWidth, this.ClientSize.Height);
        }

        public void RedrawTracks()
        {
            _openGL.Invalidate();
        }

        public static Color BlendColor(Color color1, Color color2, double ratio)
        {
            ratio = Math.Max(0, Math.Min(1, ratio));

            int r = (int)Math.Round(ratio * color2.R + (1 - ratio) * color1.R);
            int g = (int)Math.Round(ratio * color2.G + (1 - ratio) * color1.G);
            int b = (int)Math.Round(ratio * color2.B + (1 - ratio) * color1.B);

            return Color.FromArgb(r, g, b);
        }

        public void DrawTracks()
        {
            int line, line2, i, j, n, i1, curY = 0, top, toLine, num;
            Pattern prevPat, nextPat;
            int prevPatNum, nextPatNum;
            int curPatSepTop, curPatSepBottom;
            int prevPatSepBottom, prevPatSepTop;
            int nextPatSepTop = 0, nextPatSepBottom;
            Color chanBgColor, chanTextColor, chanNoteColor, chanNoteParamsColor, chanNoteCommandsColor;
            Color textColor, bgColor;
            int x, y;
            string s;
            bool llDown = false;
            int temLen;
            string llStr;
            int patternLength;
            const float lld = 0.8f;

            if (RedrawDisabled)
                return;

            if (!this.ParentWin.Visible)
                return;

            if (this.ParentWin.VTM == null)
                return;

            if (ShownPattern == null)
                return;

            if (this.ParentWin.IsDisposed)
                return;

            if (this.ClientSize.Width <= 0 || this.ClientSize.Height <= 0)
                return;

            num = 0;
            top = 0;
            prevPat = null;
            nextPat = null;
            y = (CenterLineIndex - ShownFrom);
            n = VisibleLineCount - y;
            patternLength = ShownPattern.Length;

            if (patternLength < n)
                n = patternLength;

            if (y < 0)
            {
                i1 = -y;
                n += y;
                y = 0;
            }
            else
                i1 = 0;

            // Calculate previous and next pattern number
            if (ParentWin.PositionIndex > 0)
            {
                prevPatNum = ParentWin.VTM.Positions.Value[ParentWin.PositionIndex - 1];
                prevPat = ParentWin.VTM.Patterns[prevPatNum];
            }
            else
                prevPatNum = -1;

            if (ParentWin.PositionIndex < ParentWin.VTM.Positions.Length - 1)
            {
                nextPatNum = ParentWin.VTM.Positions.Value[ParentWin.PositionIndex + 1];
                nextPat = ParentWin.VTM.Patterns[nextPatNum];
            }
            else
                nextPatNum = -1;

            // Clear bitmap
            bgColor = MainForm.ThemeColors[(int)ThemeColor.Background];
            _openGL.FillRect(this.Font, new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height), bgColor);
            //Render.FillRect(g, new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height), bgColor);

            if (ParentWin.LLTemplate == 0)
                llStr = "";
            else
                llStr = ParentWin.LLTemplates[ParentWin.LLTemplate - 1];

            temLen = llStr.Length;

            // Draw previous pattern lines
            prevPatSepTop = 0;
            prevPatSepBottom = -1;

            if (prevPatNum != -1 && y > 0)
            {
                top = 0;         

                for (line = y; line >= 1; line--)
                {
                    s = VTModule.GetOutPatternLineString(prevPatNum, prevPat, line, MainForm.ChanAlloc, true, MainForm.DecBaseLinesOn, MainForm.DecBaseNoiseOn);
                    line2 = prevPat.Length - line;

                    if (line2 % HLStep == 0 && prevPatNum >= 0 && s.Trim() != "" && HLStep != 256)
                        bgColor = MainForm.ThemeColors[(int)ThemeColor.OutHlBackground];
                    else if (prevPatNum < 0)
                        bgColor = MainForm.ThemeColors[(int)ThemeColor.Background];
                    else
                        bgColor = MainForm.ThemeColors[(int)ThemeColor.OutBackground];

                    textColor = MainForm.ThemeColors[(int)ThemeColor.OutText];
                    llDown = false;

                    if (temLen != 0 && y != curY && llStr[line2 % temLen] == ' ')
                        llDown = true;

                    if (llDown)
                        textColor = BlendColor(textColor, bgColor, lld);

                    _openGL.DrawText(this.Font, 0, top, s, textColor, bgColor);
                    //Render.Print(g, this.Font, 0, top, s, textColor, bgColor);

                    // Line numbers
                    if (prevPatNum != -1 && s.Trim() != "")
                    {
                        _openGL.DrawText(this.Font, 3, top, MainForm.DecBaseLinesOn ? $"{line2:D3}" : $"{line2:X2}", textColor, bgColor);
                        //Render.Print(g, this.Font, 3, top, MainForm.DecBaseLinesOn ? $"{line2:D3}" : $"{line2:X2}", textColor, bgColor);
                    }

                    // Fill last 2 pixels
                    _openGL.FillRect(this.Font, Rectangle.FromLTRB(PatternWidth - 2, top, PatternWidth + 1, top + CharHeight), bgColor);
                    //Render.FillRect(g, Rectangle.FromLTRB(PatWidth - 2, top, PatWidth + 1, top + CelH), bgColor);

                    top += CharHeight;

                    if (s.Trim() == "")
                        prevPatSepTop = top;
                }
                prevPatSepBottom = top;
            }

            // Calculate current pattern top Y coordinate
            y = y * CharHeight;
            curY = CharHeight * CenterLineIndex;
            curPatSepTop = y;

            if (prevPatSepBottom == -1)
                prevPatSepBottom = y;

            // No previous pattern: draw top horinontal line
            if (prevPatNum == -1 && y > 0)
            {
                bgColor = MainForm.ThemeColors[(int)ThemeColor.OutSeparators];
                _openGL.FillRect(this.Font, Rectangle.FromLTRB(0, y - 1, PatternWidth + 2, y), bgColor);
                //Render.FillRect(g, Rectangle.FromLTRB(0, y - 1, PatWidth + 2, y), bgColor);
            }

            // --- Draw current pattern lines
            // All channels is muted - simple draw pattern
            if (ChannelState[0].Muted && ChannelState[1].Muted && ChannelState[2].Muted)
            {
                for (line = i1; line < i1 + n; line++)
                {
                    // Draw pattern line
                    s = VTModule.GetPatternLineString(ShownPattern, line, MainForm.ChanAlloc, false, false, MainForm.DecBaseLinesOn, MainForm.DecBaseNoiseOn);

                    llDown = false;

                    if (temLen != 0 && y != curY && llStr[line % temLen] == ' ')
                        llDown = true;

                    if (y == curY)
                    {
                        bgColor = MainForm.ThemeColors[(int)ThemeColor.SelLineBackground];
                        textColor = MainForm.ThemeColors[(int)ThemeColor.SelLineText];
                    }
                    else if (line % HLStep == 0 && HLStep != 256)
                    {
                        bgColor = MainForm.ThemeColors[(int)ThemeColor.OutHlBackground];
                        textColor = MainForm.ThemeColors[(int)ThemeColor.OutText];
                    }
                    else
                    {
                        bgColor = MainForm.ThemeColors[(int)ThemeColor.OutBackground];
                        textColor = MainForm.ThemeColors[(int)ThemeColor.OutText];
                    }

                    if (llDown)
                        textColor = BlendColor(textColor, bgColor, lld);

                    _openGL.DrawText(this.Font, 0, y, s, textColor, bgColor);
                    //Render.Print(g, this.Font, 0, y, s, textColor, bgColor);

                    // Fill last 2 pixels
                    _openGL.FillRect(this.Font, Rectangle.FromLTRB(PatternWidth - 2, y, PatternWidth + 1, y + CharHeight), bgColor);
                    //Render.FillRect(g, Rectangle.FromLTRB(PatWidth - 2, y, PatWidth + 1, y + CelH), bgColor);

                    // Draw line numbers
                    _openGL.DrawText(this.Font, 3, y, MainForm.DecBaseLinesOn ? $"{line:D3}" : $"{line:X2}", textColor, bgColor);
                    //Render.Print(g, this.Font, 3, y, MainForm.DecBaseLinesOn ? $"{line:D3}" : $"{line:X2}", textColor, bgColor);

                    y += CharHeight;
                }
            }
            else
            {
                // Not all channels muted
                for (line = i1; line < i1 + n; line++)
                {
                    llDown = false;

                    if (temLen != 0 && y != curY && llStr[line % temLen] == ' ')
                        llDown = true;

                    if (y == curY)
                    {
                        // Selected line
                        bgColor = MainForm.ThemeColors[(int)ThemeColor.SelLineBackground];
                        textColor = MainForm.ThemeColors[(int)ThemeColor.SelLineText];
                    }
                    else if ((line % HLStep == 0) && (HLStep != 256))
                    {
                        // Highlighted line
                        bgColor = MainForm.ThemeColors[(int)ThemeColor.HighlBackground];
                        textColor = MainForm.ThemeColors[(int)ThemeColor.HighlText];
                    }
                    else
                    {
                        // Default line
                        bgColor = MainForm.ThemeColors[(int)ThemeColor.Background];
                        textColor = MainForm.ThemeColors[(int)ThemeColor.Text];
                    }

                    if (llDown)
                        textColor = BlendColor(textColor, bgColor, lld);

                    // Fill last 2 pixels
                    _openGL.FillRect(this.Font, Rectangle.FromLTRB(PatternWidth - 2, y, PatternWidth + 1, y + CharHeight), bgColor);
                    //Render.FillRect(g, Rectangle.FromLTRB(PatWidth - 2, y, PatWidth + 1, y + CelH), bgColor);
                    
                    // Empty line layer and line number
                    s = "   .... .. --- .... .... --- .... .... --- .... ....";

                    _openGL.DrawText(this.Font, 0, y, MainForm.DecBaseLinesOn ? $" {s}" : s, textColor, bgColor);
                    //Render.Print(g, this.Font, 0, y, ' ' + s, textColor, bgColor);

                    if (y == curY)
                        textColor = MainForm.ThemeColors[(int)ThemeColor.SelLineNum];
                    else if (line % HLStep == 0 && HLStep != 256)
                        textColor = MainForm.ThemeColors[(int)ThemeColor.HighlLineNum];
                    else
                        textColor = MainForm.ThemeColors[(int)ThemeColor.LineNum];

                    if (llDown)
                        textColor = BlendColor(textColor, bgColor, lld);

                    _openGL.DrawText(this.Font, 3, y, MainForm.DecBaseLinesOn ? $"{line:D3}" : $"{line:X2}", textColor, bgColor);
                    //Render.Print(g, this.Font, 3, y, MainForm.DecBaseLinesOn ? $"{line:D3}" : $"{line:X2}", textColor, bgColor);

                    // Envelope
                    if (ShownPattern.Lines[line].Envelope > 0)
                    {
                        num = ShownPattern.Lines[line].Envelope;

                        // Get HEX envelope string
                        if (num < 16)
                            s = num.ToString("X1");
                        else if (num < 256)
                            s = num.ToString("X2");
                        else if (num < 0x1000)
                            s = num.ToString("X3");
                        else
                            s = num.ToString("X4");

                        // Get envelope as note string
                        if (MainForm.EnvelopeAsNote)
                        {
                            num = VTModule.GetNoteByEnvelope(ShownPattern.Lines[line].Envelope);

                            if (num >= 0 && num <= 60)
                                s = VTModule.NoteToStr(num);

                        }

                        // Calculate envelope string X coordinate
                        // 3 - 3 char from left
                        // 4 - max envelope length
                        // Shift - 1/0 char
                        x = (3 + 4 - s.Length + Shift) * CharWidth;

                        if (y == curY)
                            textColor = MainForm.ThemeColors[(int)ThemeColor.SelEnvelope];
                        else
                            textColor = MainForm.ThemeColors[(int)ThemeColor.Envelope];

                        if (llDown)
                            textColor = BlendColor(textColor, bgColor, lld);

                        _openGL.DrawText(this.Font, x, y, s, textColor, bgColor);
                        //Render.Print(g, this.Font, x, y, s, textColor, bgColor);
                    }

                    // Noise
                    if (ShownPattern.Lines[line].Noise > 0)
                    {
                        num = ShownPattern.Lines[line].Noise;

                        // Get noise value string
                        if (MainForm.DecBaseNoiseOn)
                            s = num.ToString();
                        else if (num < 16)
                            s = num.ToString("X1");
                        else
                            s = num.ToString("X2");

                        // Calculate noise X coordinate
                        // 8 - char from left
                        // 2 - max noise length
                        // Shift - 1/0 char
                        x = (8 + 2 - s.Length + Shift) * CharWidth;
                        if (y == curY)
                            textColor = MainForm.ThemeColors[(int)ThemeColor.SelNoise];
                        else
                            textColor = MainForm.ThemeColors[(int)ThemeColor.Noise];
                        
                        if (llDown)
                            textColor = BlendColor(textColor, bgColor, lld);

                        _openGL.DrawText(this.Font, x, y, s, textColor, bgColor);
                        //Render.Print(g, this.Font, x, y, s, textColor, bgColor);
                    }

                    // Draw channels
                    for (i = 0; i < 3; i++)
                    {
                        j = MainForm.ChanAlloc[i];

                        // Channel muted
                        // Prepare channel colors
                        if (ChannelState[j].Muted)
                        {
                            // Muted + Selected line
                            if (y == curY)
                            {
                                chanTextColor = MainForm.ThemeColors[(int)ThemeColor.SelLineText];
                                chanNoteColor = MainForm.ThemeColors[(int)ThemeColor.SelLineText];
                                chanNoteParamsColor = MainForm.ThemeColors[(int)ThemeColor.SelLineText];
                                chanNoteCommandsColor = MainForm.ThemeColors[(int)ThemeColor.SelLineText];
                                chanBgColor = MainForm.ThemeColors[(int)ThemeColor.SelLineBackground];
                            }
                            // Muted + Highlight line
                            else if (line % HLStep == 0 && HLStep != 256)
                            {
                                chanTextColor = MainForm.ThemeColors[(int)ThemeColor.OutText];
                                chanNoteColor = MainForm.ThemeColors[(int)ThemeColor.OutText];
                                chanNoteParamsColor = MainForm.ThemeColors[(int)ThemeColor.OutText];
                                chanNoteCommandsColor = MainForm.ThemeColors[(int)ThemeColor.OutText];
                                chanBgColor = MainForm.ThemeColors[(int)ThemeColor.OutHlBackground];
                            }
                            // Muted line
                            else
                            {
                                chanTextColor = MainForm.ThemeColors[(int)ThemeColor.OutText];
                                chanNoteColor = MainForm.ThemeColors[(int)ThemeColor.OutText];
                                chanNoteParamsColor = MainForm.ThemeColors[(int)ThemeColor.OutText];
                                chanNoteCommandsColor = MainForm.ThemeColors[(int)ThemeColor.OutText];
                                chanBgColor = MainForm.ThemeColors[(int)ThemeColor.OutBackground];
                            }
                        }
                        else
                        {
                            // Selected line
                            if (y == curY)
                            {
                                chanTextColor = MainForm.ThemeColors[(int)ThemeColor.SelLineText];
                                chanNoteColor = MainForm.ThemeColors[(int)ThemeColor.SelNote];
                                chanNoteParamsColor = MainForm.ThemeColors[(int)ThemeColor.SelNoteParams];
                                chanNoteCommandsColor = MainForm.ThemeColors[(int)ThemeColor.SelNoteCommands];
                                chanBgColor = MainForm.ThemeColors[(int)ThemeColor.SelLineBackground];
                            }
                            // Hightlight line
                            else if (line % HLStep == 0 && HLStep != 256)
                            {
                                chanTextColor = MainForm.ThemeColors[(int)ThemeColor.HighlText];
                                chanNoteColor = MainForm.ThemeColors[(int)ThemeColor.Note];
                                chanNoteParamsColor = MainForm.ThemeColors[(int)ThemeColor.NoteParams];
                                chanNoteCommandsColor = MainForm.ThemeColors[(int)ThemeColor.NoteCommands];
                                chanBgColor = MainForm.ThemeColors[(int)ThemeColor.HighlBackground];
                            }
                            // Line
                            else
                            {
                                chanTextColor = MainForm.ThemeColors[(int)ThemeColor.Text];
                                chanNoteColor = MainForm.ThemeColors[(int)ThemeColor.Note];
                                chanNoteParamsColor = MainForm.ThemeColors[(int)ThemeColor.NoteParams];
                                chanNoteCommandsColor = MainForm.ThemeColors[(int)ThemeColor.NoteCommands];
                                chanBgColor = MainForm.ThemeColors[(int)ThemeColor.Background];
                            }
                        }

                        if (llDown)
                        {
                            chanTextColor = BlendColor(chanTextColor, chanBgColor, lld);
                            chanNoteColor = BlendColor(chanNoteColor, chanBgColor, lld);
                            chanNoteParamsColor = BlendColor(chanNoteParamsColor, chanBgColor, lld);
                            chanNoteCommandsColor = BlendColor(chanNoteCommandsColor, chanBgColor, lld);
                        }
                        
                        textColor = chanTextColor;
                        bgColor = chanBgColor;

                        // Channel muted
                        if (ChannelState[j].Muted)
                        {
                            switch (i)
                            {
                                case 0:
                                    // X coord
                                    num = 11;
                                    break;
                                case 1:
                                    num = 25;
                                    break;
                                case 2:
                                    num = 39;
                                    break;
                            }
                           
                            x = (num + Shift) * CharWidth;
                            num = CharWidth / 2;
                            
                            _openGL.DrawText(this.Font, x - num, y, " ", textColor, bgColor);
                            //Render.Print(g, this.Font, x - num, y, " ", textColor, bgColor);

                            if (i != 2)
                            {
                                _openGL.DrawText(this.Font, x + (12 * CharWidth) + num, y, " ", textColor, bgColor);
                                //Render.Print(g, this.Font, x + (12 * CelW) + num, y, " ", textColor, bgColor);
                            }
                            
                            _openGL.DrawText(this.Font, x, y, "--- .... ....", textColor, bgColor);
                            //Render.Print(g, this.Font, x, y, "--- .... ....", textColor, bgColor);
                        }

                        // Note
                        if (ShownPattern.Lines[line].Channel[j].Note != -1)
                        {
                            switch (i)
                            {
                                case 0:
                                    // Note X coord
                                    num = 11;
                                    break;
                                case 1:
                                    num = 25;
                                    break;
                                case 2:
                                    num = 39;
                                    break;
                            }

                            x = (num + Shift) * CharWidth;
                            textColor = chanNoteColor;
                            
                            _openGL.DrawText(this.Font, x, y, VTModule.NoteToStr(ShownPattern.Lines[line].Channel[j].Note), textColor, bgColor);
                            //Render.Print(g, this.Font, x, y, TVTM.NoteToStr(ShownPattern.Items[line].Channel[j].Note), textColor, bgColor);
                        }

                        textColor = chanNoteParamsColor;

                        // Sample
                        if (ShownPattern.Lines[line].Channel[j].Sample > 0)
                        {
                            switch (i)
                            {
                                case 0:
                                    // Sample X coord
                                    num = 15;
                                    break;
                                case 1:
                                    num = 29;
                                    break;
                                case 2:
                                    num = 43;
                                    break;
                            }
                           
                            x = (num + Shift) * CharWidth;

                            // Get sample char
                            num = ShownPattern.Lines[line].Channel[j].Sample;
                            if (num < 16)
                                s = num.ToString("X1");
                            else
                                s = Char.ToString((char)(num + (int)('A') - 10));

                            _openGL.DrawText(this.Font, x, y, s, textColor, bgColor);
                            //Render.Print(g, this.Font, x, y, s, textColor, bgColor);
                        }
                        
                        // Note envelope
                        if (ShownPattern.Lines[line].Channel[j].Envelope > 0)
                        {
                            switch (i)
                            {
                                case 0:
                                    // Envelope X coord
                                    num = 16;
                                    break;
                                case 1:
                                    num = 30;
                                    break;
                                case 2:
                                    num = 44;
                                    break;
                            }
                            
                            x = (num + Shift) * CharWidth;
                            
                            _openGL.DrawText(this.Font, x, y, ShownPattern.Lines[line].Channel[j].Envelope.ToString("X1"), textColor, bgColor);
                            //Render.Print(g, this.Font, x, y, ShownPattern.Items[line].Channel[j].Envelope.ToString("X1"), textColor, bgColor);
                        }
                        
                        // Ornament
                        if (ShownPattern.Lines[line].Channel[j].Ornament > 0)
                        {
                            switch (i)
                            {
                                case 0:
                                    // Ornament X coord
                                    num = 17;
                                    break;
                                case 1:
                                    num = 31;
                                    break;
                                case 2:
                                    num = 45;
                                    break;
                            }
                            
                            x = (num + Shift) * CharWidth;
                            
                            _openGL.DrawText(this.Font, x, y, ShownPattern.Lines[line].Channel[j].Ornament.ToString("X1"), textColor, bgColor);
                            //Render.Print(g, this.Font, x, y, ShownPattern.Items[line].Channel[j].Ornament.ToString("X1"), textColor, bgColor);
                        }

                        // Volume
                        if (ShownPattern.Lines[line].Channel[j].Volume > 0)
                        {
                            switch (i)
                            {
                                case 0:
                                    // Volume X coord
                                    num = 18;
                                    break;
                                case 1:
                                    num = 32;
                                    break;
                                case 2:
                                    num = 46;
                                    break;
                            }
                            
                            x = (num + Shift) * CharWidth;
                            
                            _openGL.DrawText(this.Font, x, y, ShownPattern.Lines[line].Channel[j].Volume.ToString("X1"), textColor, bgColor);
                            //Render.Print(g, this.Font, x, y, ShownPattern.Items[line].Channel[j].Volume.ToString("X1"), textColor, bgColor);
                        }

                        textColor = chanNoteCommandsColor;

                        // Command number
                        if (ShownPattern.Lines[line].Channel[j].AdditionalCommand.Number > 0)
                        {
                            switch (i)
                            {
                                case 0:
                                    // X coord
                                    num = 20;
                                    break;
                                case 1:
                                    num = 34;
                                    break;
                                case 2:
                                    num = 48;
                                    break;
                            }
                            
                            x = (num + Shift) * CharWidth;
                            
                            _openGL.DrawText(this.Font, x, y, ShownPattern.Lines[line].Channel[j].AdditionalCommand.Number.ToString("X1"), textColor, bgColor);
                            //Render.Print(g, this.Font, x, y, ShownPattern.Items[line].Channel[j].Additional_Command.Number.ToString("X1"), textColor, bgColor);
                        }

                        // Command delay
                        if (ShownPattern.Lines[line].Channel[j].AdditionalCommand.Delay > 0)
                        {
                            switch (i)
                            {
                                case 0:
                                    // X coord
                                    num = 21;
                                    break;
                                case 1:
                                    num = 35;
                                    break;
                                case 2:
                                    num = 49;
                                    break;
                            }
                            
                            x = (num + Shift) * CharWidth;
                            
                            _openGL.DrawText(this.Font, x, y, ShownPattern.Lines[line].Channel[j].AdditionalCommand.Delay.ToString("X1"), textColor, bgColor);
                            //Render.Print(g, this.Font, x, y, ShownPattern.Items[line].Channel[j].Additional_Command.Delay.ToString("X1"), textColor, bgColor);
                        }

                        // Command parameter
                        if (ShownPattern.Lines[line].Channel[j].AdditionalCommand.Parameter > 0)
                        {
                            num = ShownPattern.Lines[line].Channel[j].AdditionalCommand.Parameter;
                            if (num < 16)
                                s = num.ToString("X1");
                            else
                                s = num.ToString("X2");

                            switch (i)
                            {
                                case 0:
                                    // X coord
                                    num = 22;
                                    break;
                                case 1:
                                    num = 36;
                                    break;
                                case 2:
                                    num = 50;
                                    break;
                            }
                            
                            x = (num + 2 - s.Length + Shift) * CharWidth;
                            
                            _openGL.DrawText(this.Font, x, y, s, textColor, bgColor);
                            //Render.Print(g, this.Font, x, y, s, textColor, bgColor);
                        }
                    }

                    y += CharHeight;
                }
            }

            curPatSepBottom = y;

            // Draw next pattern lines
            nextPatSepBottom = 0;

            if (nextPatNum != -1 && y < CharHeight * VisibleLineCount)
            {
                top = y;
                toLine = VisibleLineCount - (y / CharHeight);
                nextPatSepTop = y;   

                for (line = 0; line <= toLine; line++)
                {
                    s = VTModule.GetOutPatternLineString(nextPatNum, nextPat, line, MainForm.ChanAlloc, false, MainForm.DecBaseLinesOn, MainForm.DecBaseNoiseOn);

                    if (nextPatNum < 0)
                        bgColor = MainForm.ThemeColors[(int)ThemeColor.Background];
                    else if (line % HLStep == 0 && nextPatNum >= 0 && s.Trim() != "" && HLStep != 256)
                        bgColor = MainForm.ThemeColors[(int)ThemeColor.OutHlBackground];
                    else
                        bgColor = MainForm.ThemeColors[(int)ThemeColor.OutBackground];

                    textColor = MainForm.ThemeColors[(int)ThemeColor.OutText];
                    llDown = false;

                    if (temLen != 0 && y != curY && llStr[line % temLen] == ' ')
                        llDown = true;

                    if (llDown)
                        textColor = BlendColor(textColor, bgColor, lld);

                    _openGL.DrawText(this.Font, 0, top, s, textColor, bgColor);
                    //Render.Print(g, this.Font, 0, top, s, textColor, bgColor);
                    
                    if (nextPatNum != -1 && s.Trim() != "")
                    {
                        _openGL.DrawText(this.Font, 3, top, MainForm.DecBaseLinesOn ? $"{line:D3}" : $"{line:X2}", textColor, bgColor);
                        //Render.Print(g, this.Font, 3, top, MainForm.DecBaseLinesOn ? $"{line:D3}" : $"{line:X2}", textColor, bgColor);
                    }

                    // Fill last 2 pixels
                    _openGL.FillRect(this.Font, Rectangle.FromLTRB(PatternWidth - 2, top, PatternWidth + 1, top + CharHeight), bgColor);
                    //Render.FillRect(g, Rectangle.FromLTRB(PatWidth - 2, top, PatWidth + 1, top + CelH), bgColor);
                    
                    if (s.Trim() == "" && nextPatSepBottom == 0)
                        nextPatSepBottom = top;

                    top += CharHeight;
                }

                if (nextPatSepBottom == 0)
                    nextPatSepBottom = top;
            }
            else
            {
                // No next pattern: draw bottom horizontal line
                bgColor = MainForm.ThemeColors[(int)ThemeColor.OutSeparators];
                
                _openGL.FillRect(this.Font, Rectangle.FromLTRB(0, y, PatternWidth + 2, y + 1), bgColor);
                //Render.FillRect(g, Rectangle.FromLTRB(0, y, PatWidth + 2, y + 1), bgColor);
            }

            // Separators
            if (!MainForm.DisableSeparators)
            {
                // Previous pattern separators
                if (prevPatNum != -1)
                {
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.OutSeparators];
                    
                    // Short previous pattern - draw top line
                    if (prevPatSepTop > 0)
                    {
                        _openGL.FillRect(this.Font, Rectangle.FromLTRB(0, prevPatSepTop - 1, PatternWidth + 2, prevPatSepTop), bgColor);
                        //Render.FillRect(g, Rectangle.FromLTRB(0, prevPatSepTop - 1, PatWidth + 2, prevPatSepTop), bgColor);
                    }

                    bgColor = MainForm.ThemeColors[(int)ThemeColor.OutSeparators];
                    
                    _openGL.FillRect(this.Font, Rectangle.FromLTRB(SeperatorX[1], prevPatSepTop, SeperatorX[1] + 2, prevPatSepBottom), bgColor);
                    //Render.FillRect(g, Rectangle.FromLTRB(SepCX[1], prevPatSepTop, SepCX[1] + 2, prevPatSepBottom), bgColor);
                    _openGL.FillRect(this.Font, Rectangle.FromLTRB(SeperatorX[2], prevPatSepTop, SeperatorX[2] + 2, prevPatSepBottom), bgColor);
                    //Render.FillRect(g, Rectangle.FromLTRB(SepCX[2], prevPatSepTop, SepCX[2] + 2, prevPatSepBottom), bgColor);
                    _openGL.FillRect(this.Font, Rectangle.FromLTRB(SeperatorX[3], prevPatSepTop, SeperatorX[3] + 2, prevPatSepBottom), bgColor);
                    //Render.FillRect(g, Rectangle.FromLTRB(SepCX[3], prevPatSepTop, SepCX[3] + 2, prevPatSepBottom), bgColor);
                    _openGL.FillRect(this.Font, Rectangle.FromLTRB(SeperatorX[4], prevPatSepTop, SeperatorX[4] + 2, prevPatSepBottom), bgColor);
                    //Render.FillRect(g, Rectangle.FromLTRB(SepCX[4], prevPatSepTop, SepCX[4] + 2, prevPatSepBottom), bgColor);
                }

                // Current pattern separators
                bgColor = MainForm.ThemeColors[(int)ThemeColor.Separators];
                
                _openGL.FillRect(this.Font, Rectangle.FromLTRB(SeperatorX[1], curPatSepTop, SeperatorX[1] + 2, curPatSepBottom), bgColor);
                //Render.FillRect(g, Rectangle.FromLTRB(SepCX[1], curPatSepTop, SepCX[1] + 2, curPatSepBottom), bgColor);
                _openGL.FillRect(this.Font, Rectangle.FromLTRB(SeperatorX[2], curPatSepTop, SeperatorX[2] + 2, curPatSepBottom), bgColor);
                //Render.FillRect(g, Rectangle.FromLTRB(SepCX[2], curPatSepTop, SepCX[2] + 2, curPatSepBottom), bgColor);
                _openGL.FillRect(this.Font, Rectangle.FromLTRB(SeperatorX[3], curPatSepTop, SeperatorX[3] + 2, curPatSepBottom), bgColor);
                //Render.FillRect(g, Rectangle.FromLTRB(SepCX[3], curPatSepTop, SepCX[3] + 2, curPatSepBottom), bgColor);
                _openGL.FillRect(this.Font, Rectangle.FromLTRB(SeperatorX[4], curPatSepTop, SeperatorX[4] + 2, curPatSepBottom), bgColor);
                //Render.FillRect(g, Rectangle.FromLTRB(SepCX[4], curPatSepTop, SepCX[4] + 2, curPatSepBottom), bgColor);

                // Next pattern separators
                if (nextPatNum != -1)
                {
                    bgColor = MainForm.ThemeColors[(int)ThemeColor.OutSeparators];

                    // Short next pattern - draw bottom line
                    if (nextPatSepBottom < top)
                    {
                        _openGL.FillRect(this.Font, Rectangle.FromLTRB(0, nextPatSepBottom, PatternWidth + 2, nextPatSepBottom + 1), bgColor);
                        //Render.FillRect(g, Rectangle.FromLTRB(0, nextPatSepBottom, PatWidth + 2, nextPatSepBottom + 1), bgColor);
                    }

                    _openGL.FillRect(this.Font, Rectangle.FromLTRB(SeperatorX[1], nextPatSepTop, SeperatorX[1] + 2, nextPatSepBottom), bgColor);
                    //Render.FillRect(g, Rectangle.FromLTRB(SepCX[1], nextPatSepTop, SepCX[1] + 2, nextPatSepBottom), bgColor);
                    _openGL.FillRect(this.Font, Rectangle.FromLTRB(SeperatorX[2], nextPatSepTop, SeperatorX[2] + 2, nextPatSepBottom), bgColor);
                    //Render.FillRect(g, Rectangle.FromLTRB(SepCX[2], nextPatSepTop, SepCX[2] + 2, nextPatSepBottom), bgColor);
                    _openGL.FillRect(this.Font, Rectangle.FromLTRB(SeperatorX[3], nextPatSepTop, SeperatorX[3] + 2, nextPatSepBottom), bgColor);
                    //Render.FillRect(g, Rectangle.FromLTRB(SepCX[3], nextPatSepTop, SepCX[3] + 2, nextPatSepBottom), bgColor);
                    _openGL.FillRect(this.Font, Rectangle.FromLTRB(SeperatorX[4], nextPatSepTop, SeperatorX[4] + 2, nextPatSepBottom), bgColor);
                    //Render.FillRect(g, Rectangle.FromLTRB(SepCX[4], nextPatSepTop, SepCX[4] + 2, nextPatSepBottom), bgColor);
                }
            }

            // Right border if pattern editor width < window width (small font size)
            if (PatternWidth < this.ClientSize.Width)
            {
                bgColor = MainForm.ThemeColors[(int)ThemeColor.OutSeparators];
                if (prevPatNum != -1)
                {
                    _openGL.FillRect(this.Font, Rectangle.FromLTRB(PatternWidth + 1, prevPatSepTop, PatternWidth + 2, prevPatSepBottom), bgColor);
                    //Render.FillRect(g, Rectangle.FromLTRB(PatWidth + 1, prevPatSepTop, PatWidth + 2, prevPatSepBottom), bgColor);
                }

                _openGL.FillRect(this.Font, Rectangle.FromLTRB(PatternWidth + 1, curPatSepTop, PatternWidth + 2, curPatSepBottom), bgColor);
                //Render.FillRect(g, Rectangle.FromLTRB(PatWidth + 1, curPatSepTop, PatWidth + 2, curPatSepBottom), bgColor);
                
                if (nextPatNum != -1)
                {
                    _openGL.FillRect(this.Font, Rectangle.FromLTRB(PatternWidth + 1, nextPatSepTop, PatternWidth + 2, nextPatSepBottom), bgColor);
                    //Render.FillRect(g, Rectangle.FromLTRB(PatWidth + 1, nextPatSepTop, PatWidth + 2, nextPatSepBottom), bgColor);
                }
            }

            // Separator between line number and envelope
            if (prevPatNum != -1)
            {
                bgColor = MainForm.ThemeColors[(int)ThemeColor.OutSeparators];
                _openGL.FillRect(this.Font, Rectangle.FromLTRB(SeperatorX[0], prevPatSepTop, SeperatorX[0] + 2, prevPatSepBottom), bgColor);
                //Render.FillRect(g, Rectangle.FromLTRB(SepCX[0], prevPatSepTop, SepCX[0] + 2, prevPatSepBottom), bgColor);
            }
            
            bgColor = MainForm.ThemeColors[(int)ThemeColor.Separators];
            _openGL.FillRect(this.Font, Rectangle.FromLTRB(SeperatorX[0], curPatSepTop, SeperatorX[0] + 2, curPatSepBottom), bgColor);
            //Render.FillRect(g, Rectangle.FromLTRB(SepCX[0], curPatSepTop, SepCX[0] + 2, curPatSepBottom), bgColor);

            if (nextPatNum != -1)
            {
                bgColor = MainForm.ThemeColors[(int)ThemeColor.OutSeparators];
                _openGL.FillRect(this.Font, Rectangle.FromLTRB(SeperatorX[0], nextPatSepTop, SeperatorX[0] + 2, nextPatSepBottom), bgColor);
                //Render.FillRect(g, Rectangle.FromLTRB(SepCX[0], nextPatSepTop, SepCX[0] + 2, nextPatSepBottom), bgColor);
            }

            DrawSelection();
            DrawBars();
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            return hi switch
            {
                0 => Color.FromArgb(v, t, p),
                1 => Color.FromArgb(q, v, p),
                2 => Color.FromArgb(p, v, t),
                3 => Color.FromArgb(p, q, v),
                4 => Color.FromArgb(t, p, v),
                _ => Color.FromArgb(v, p, q),
            };
        }

        public void DrawBars()
        {
            int index = Array.IndexOf(ChildForm.PlayingWindow, ParentWin);

            if (index == -1)
                return;

            SoundChip soundChip = AY.SoundChip[index];

            if (soundChip == null)
                return;

            Rectangle specRect = new Rectangle(CharWidth * (11 + Shift), CharHeight * CenterLineIndex / 2, 42 * CharWidth, CharHeight * CenterLineIndex / 2);
            int barOffsetX = specRect.X;
            int barOffsetY = specRect.Y;
            int barWidth = (int)Math.Round((float)specRect.Width / soundChip.SpecLevels.Length);
            int barHeight = specRect.Height;
            int channelWidth = CharWidth * 14;

            /* for (int i = 0; i < soundChip.SpecLevels.Length; i++)
            {
                Color color = Color.White;
                SpecType specType = soundChip.SpecTypes[i];
                float level = soundChip.SpecLevels[i];

                switch (specType)
                {
                    case SpecType.ChannelA:
                        color = Color.FromArgb(128, Color.White);
                        break;
                    case SpecType.ChannelB:
                        color = Color.FromArgb(128, Color.White);
                        break;
                    case SpecType.ChannelC:
                        color = Color.FromArgb(128, Color.White);
                        break;
                    case SpecType.Envelope:
                        color = Color.FromArgb(128, Color.White);
                        break;
                }

                _openGL.DrawBar("Bar2", barOffsetX + (i * barWidth), barOffsetY + barHeight, barWidth, barHeight, level, color);
            } */

            for (int i = 0; i < soundChip.SpecLevels.Length; i++)
            {
                float level = soundChip.SpecLevels[i];
                SpecType specType = soundChip.SpecTypes[i];
                float hue = 360f * (1f - (float)i / (soundChip.SpecLevels.Length - 1));
                Color baseColor = ColorFromHSV(hue, 1.0f, 1.0f);
                Color color = Color.FromArgb(128, baseColor);

                _openGL.DrawBar("Bar3", barOffsetX + (i * barWidth), barOffsetY + barHeight, barWidth, barHeight, level, color);
            }

            barOffsetX = specRect.X + (CharWidth * 6);
            barWidth = CharWidth * 3;

            for (int i = 0; i < 3; i++)
                _openGL.DrawBar("Bar1", barOffsetX + (i * channelWidth), barOffsetY + barHeight, barWidth, barHeight, soundChip.VULevel[i], Color.White);
        }

        public void DoBitBlt()
        {
        }

        public void DoRefresh()
        {
            HideCaret();
            RemoveSelection();
            RedrawTracks();
            RecreateCaret();
            SetCaretPosition();
            ShowCaret();
        }

        public void DoHint()
        {
            string s = "";
            //#if DEBUG
            //            return;
            //#endif
            if (IsSelected())
            {
                //this.ShowHint = false;
                ParentWin.ToolTip.Hide(this);
                return;
            }

            ParentWin.ToolTip.AutoPopDelay = 9000;

            switch (CursorX)
            {
                // 0 .. 3
                case 0:
                case 1:
                case 2:
                case 3:
                    s = "Envelope Generator Period (Hex Range 0-FFFF).\rSet Envelope Type to 1-E.";
                    break;
                // 5 .. 6
                case 5:
                case 6:
                    if (MainForm.DecBaseNoiseOn)
                        s = "Noise Generator Base Period (Decimal Range 0-31).";
                    else
                        s = "Noise Generator Base Period (Hex Range 0-1F).";
                    break;
                case 8:
                case 22:
                case 36:
                    s = "Note From C-1 to B-8.\rNumpad 1-8 to Octave.\rA to R-- (Release).";
                    break;
                case 12:
                case 26:
                case 40:
                    s = "Sample (1-9, A-V).\rUsed With Note or R--.\r\rCtrl+Enter, Ctrl+Click -- Edit Sample.";
                    break;
                case 13:
                case 27:
                case 41:
                    s = "Envelope Type (Hex 1-E) or Envelope Off (F).\r0th Ornament can be set Only With 1-F.";
                    break;
                case 14:
                case 28:
                case 42:
                    s = "Ornament (Hex 0-F). 0th Ornament can be set\rOnly With Envelope Type or off (1-F).\r\rCtrl+Enter, Ctrl+Click -- Edit Ornament.";
                    break;
                case 15:
                case 29:
                case 43:
                    s = "Volume (Hex 1-F).\rUse R-- Instead of Volume 0.";
                    break;
                case 17:
                case 31:
                case 45:
                    s = "Special Command:\r1 - Tone Slide Down\r2 - Tone Slide Up\r3 - Tone Portamento\r4 - Sample Sffset\r5 - Ornament Offset\r6 - Vibrato\r9 - Envelope Slide Down\rA - Envelope Slide Up\rB - Set Speed";
                    break;
                case 18:
                case 32:
                case 46:
                    s = "Delay for Commands 1-3, 9-A (1-F for Change Period, 0 for Stop).";
                    break;
                case 19:
                case 33:
                case 47:
                    s = "Hi Digit for Commands 1-5, 9-B. Hex: 0-F.\rOR 1st Parameter for Command 6 (Vibrato)\r(1-F to Sound on Period, 0 to Stop).";
                    break;
                case 20:
                case 34:
                case 48:
                    s = "Lo Digit for Commands 1-5, 9-B. Hex: 0-F\rOR Second Parameter for Command 6 (Vibrato)\r(1-F to Sound off Period, 0 to Stop After Sound on Period).";
                    break;
            }
            
            if (!new int[] { 19, 33, 47, 20, 34, 48 }.Contains(CursorX))
            {
                s += "\r\rCtrl + Space to AutoStep On/Off\r";
                s += "Ctrl + 0..9 - Step for AutoStep.\r";
                s += "Numpad 0 to AutoEnvelope.";
            }

            Globals.MainForm.StatusBar.Items[0].Text = s;

            if (MainForm.DisableHints)
            {
                //this.ShowHint = false;
                ParentWin.ToolTip.Hide(this);
            }
            else
            {
                ParentWin.ToolTip.SetToolTip(this, s);
                //this.ShowHint = true;
                //this.Hint = s;
            }
        }

        public void CreateCaret()
        {
            DoHint();

            if (CursorX == 8 || CursorX == 22 || CursorX == 36)
            {
                BigCaret = true;
                _openGL.SetCaretSize(CharWidth * 3, CharHeight);
                //CreateCaret(_openGL.Handle, 0, CelW * 3, CelH);
            }
            else
            {
                BigCaret = false;
                _openGL.SetCaretSize(CharWidth, CharHeight);
                //CreateCaret(_openGL.Handle, 0, CelW, CelH);
            }
        }

        public void RecreateCaret()
        {
            DoHint();

            if (CursorX == 8 || CursorX == 22 || CursorX == 36)
            {
                if (!BigCaret)
                {
                    CaretVisible = false;
                    //DestroyCaret();
                    CreateCaret();
                    ShowCaret();
                }
            }
            else if (BigCaret)
            {
                CaretVisible = false;
                //DestroyCaret();
                CreateCaret();
                ShowCaret();
            }
        }

        public void SetCaretPosition()
        {
            _openGL.SetCaretPos(CharWidth * (MainForm.TracksCursorXLeft + CursorX), CharHeight * CursorY);
            //SetCaretPos(CelW * (MainForm.TracksCursorXLeft + CursorX), CelH * CursorY);
        }

        public void ShowCaret()
        {
            if (CaretVisible || IsTrackPlaying() || IsSelected())
                return;

            _openGL.ShowCaret();
            //ShowCaret(_openGL.Handle);
            CaretVisible = true;
        }

        public void HideCaret()
        {
            if (!CaretVisible)
                return;

            _openGL.HideCaret();
            //HideCaret(_openGL.Handle);
            CaretVisible = false;
        }

        public void CopyToClipboard()
        {
            int x1, x2, y1, y2;
            bool repaintDisabled = false;

            if (ParentWin.EnvelopeAsNoteCheckBox.Checked)
            {
                Globals.MainForm.RedrawOff();
                repaintDisabled = true;
                ParentWin.EnvelopeAsNoteCheckBox.Checked = false;
                RedrawTracks();
            }

            x2 = CursorX;
            x1 = SelectionX;
            if (x1 > x2) { x1 = x2; x2 = SelectionX; }
            y1 = SelectionY;
            y2 = CurrentPatternLine();
            if (y1 > y2) { y1 = y2; y2 = SelectionY; }

            byte ch = 0;

            if (x2 >= 8 && x1 <= 8)
            {
                ch = (byte)MainForm.ChanAlloc[0];
                MainForm.TracksCopy.Ornament = x2 >= 14;
                MainForm.TracksCopy.Command = x2 >= 17;
            }
            else if (x2 >= 22 && x1 <= 22 && x1 > 8)
            {
                ch = (byte)MainForm.ChanAlloc[1];
                MainForm.TracksCopy.Ornament = x2 >= 28;
                MainForm.TracksCopy.Command = x2 >= 31;
            }
            else if (x2 >= 36 && x1 <= 36 && x1 > 22)
            {
                ch = (byte)MainForm.ChanAlloc[2];
                MainForm.TracksCopy.Ornament = x2 >= 36;
                MainForm.TracksCopy.Command = x2 >= 39;
            }

            MainForm.TracksCopy.Channel = ch;
            MainForm.TracksCopy.SrcWindow = ParentWin;
            MainForm.TracksCopy.FromLine = (byte)y1;
            MainForm.TracksCopy.ToLine = (byte)y2;
            MainForm.TracksCopy.Pattern = ShownPattern;
            MainForm.TracksCopy.PatNum = ParentWin.PatternIndex;
            MainForm.LastClipboard = LastClipboard.Tracks;

            StringBuilder sb = new StringBuilder();
            sb.Append(ChildForm.ClipHdrPat);

            for (int i = y1; i <= y2; i++)
            {
                string line = VTModule.GetPatternLineString(ShownPattern, i, MainForm.ChanAlloc, true, true, MainForm.DecBaseLinesOn, MainForm.DecBaseNoiseOn);
                char[] arr = line.ToCharArray();

                for (int l = 0; l < x1; l++)
                    if (MainForm.TracksCursorXLeft + 1 + l < arr.Length)
                        arr[MainForm.TracksCursorXLeft + 1 + l] = ' ';

                int pad = (Array.IndexOf(ChildForm.NotePoses, x2) >= 0) ? MainForm.TracksCursorXLeft : 0;
                for (int l = x2 + pad; l < 49 && MainForm.TracksCursorXLeft + 1 + l < arr.Length; l++)
                    arr[MainForm.TracksCursorXLeft + 1 + l] = ' ';

                sb.AppendLine(new string(arr));
            }

            Clipboard.SetText(sb.ToString());

            if (repaintDisabled)
            {
                ParentWin.EnvelopeAsNoteCheckBox.Checked = true;
                RedrawTracks();
                Globals.MainForm.RedrawOn();
            }
        }

        public void CutToClipboard()
        {
            CopyToClipboard();
            ClearSelection();
        }

        public bool PasteFromClipboard_GetStr(string lps, ref string s)
        {
            int ps = lps.IndexOf('\r');

            if (ps == -1)
                return false;

            s = s.Substring(ps);

            return true;
        }

        public void PasteFromClipboard(bool merge)
        {
            int i, j, k, l = 0, m, c;
            int x1, x2, y1, y2;
            int newEnvelope;
            int newNoise;
            AdditionalCommand[] newCommand = new AdditionalCommand[3];
            string clipboardText;
            int sz;

            if (!Clipboard.ContainsText())
                return;

            try
            {
                IDataObject dataObj = Clipboard.GetDataObject();
                if (dataObj == null || !dataObj.GetDataPresent(DataFormats.Text))
                    return;
                clipboardText = ((string)dataObj.GetData(DataFormats.Text)).Trim();
            }
            catch
            {
                return;
            }

            // Check for ModPlug Tracker format
            if (Regex.IsMatch(clipboardText, "^ModPlug Tracker", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline))
            {
                ParentWin.PasteModPlugPattern(clipboardText);
                return;
            }

            // Check for Renoise XML clipboard format
            if (Regex.IsMatch(clipboardText, "^<\\?xml version=\"1\\.0\" encoding=\"UTF-8\"\\?>\\s+<PatternClipboard", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline))
            {
                ParentWin.PasteRenoisePattern(clipboardText);
                return;
            }

            // Convert to ANSI bytes to simulate original Delphi memory block access
            byte[] ansiBytes = Encoding.Default.GetBytes(clipboardText + "\r\n");
            sz = ansiBytes.Length;

            int cursor = 0;
            string GetStr()
            {
                int end = Array.IndexOf(ansiBytes, (byte)'\r', cursor);
                if (end == -1 || end >= sz) return null;
                string result = Encoding.Default.GetString(ansiBytes, cursor, end - cursor);
                cursor = end + 2; // skip \r\n
                return result;
            }

            string header = GetStr();

            if (header == null || header + "\r\n" != ChildForm.ClipHdrPat)
                return;

            int[,] nums = new int[VTModule.MaxPatternLength, 33];

            for (int y = 0; y < VTModule.MaxPatternLength; y++)
                for (int x = 0; x <= 32; x++)
                    nums[y, x] = -1;

            int lineCount = 0;

            while (cursor + 2 < sz && lineCount < VTModule.MaxPatternLength)
            {
                string s = GetStr();

                if (s == null || s.Length != 49)
                    break;

                if (MainForm.DecBaseLinesOn)
                    s = s.Substring(2 - 1, s.Length);

                if (s.Length != 49)
                    break;

                for (j = 0; j < 4; j++)
                {
                    if (s[j + 1] != ' ')
                    {
                        if (!VTModule.SGetNumber(s[j + 1].ToString(), 15, out i))
                            return;

                        nums[l, j] = i;
                    }
                }

                if (s[6] != ' ')
                {
                    if (MainForm.DecBaseNoiseOn)
                    {
                        if (!VTModule.SGetDecNumber(s[6].ToString(), 3, out i))
                            return;
                    }
                    else if (!VTModule.SGetNumber(s[6].ToString(), 1, out i))
                        return;

                    // if not SGetNumber(s[6], 1, i) then
                    // exit;
                    nums[l, 4] = i;
                }

                if (s[7] != ' ')
                {
                    if (MainForm.DecBaseNoiseOn)
                    {
                        if (!VTModule.SGetDecNumber(s[7].ToString(), 9, out i))
                            return;
                    }
                    else if (!VTModule.SGetNumber(s[7].ToString(), 15, out i))
                        return;

                    nums[l, 5] = i;
                }

                for (k = 0; k < 3; k++)
                {
                    if (s[9 + k * 14] != ' ')
                    {
                        if (!VTModule.SGetNote(s.Substring(9 + k * 14 - 1, 3), out i))
                            return;

                        nums[l, 6 + k * 9] = i + 256;
                    }

                    if (s[13 + k * 14] != ' ')
                    {
                        if (!VTModule.SGetNumber(s[13 + k * 14].ToString(), 31, out i))
                            return;

                        nums[l, 7 + k * 9] = i;
                    }

                    for (j = 0; j < 3; j++)
                    {
                        if (s[14 + k * 14 + j] != ' ')
                        {
                            if (!VTModule.SGetNumber(s[14 + k * 14 + j].ToString(), 15, out i))
                                return;

                            nums[l, 8 + k * 9 + j] = i;
                        }
                    }

                    for (j = 0; j < 4; j++)
                    {
                        if (s[18 + k * 14 + j] != ' ')
                        {
                            if (!VTModule.SGetNumber(s[18 + k * 14 + j].ToString(), 15, out i))
                                return;

                            nums[l, 11 + k * 9 + j] = i;
                        }
                    }
                }
                l++;
            }

            if (l == 0)
                return;

            i = 0;

            while (i <= 32 && nums[0, i] < 0)
                i++;

            if (i == 33)
                return;

            j = 32;

            while (j >= 0 && nums[0, j] < 0)
                j--;

            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
            activeForm.SongChanged = true;
            activeForm.BackupSongChanged = true;
            activeForm.ValidatePattern2(activeForm.PatternIndex);
            activeForm.AddUndo(TChangeAction.InsertPatternFromClipboard, 0, 0);
            activeForm.ChangeList[activeForm.ChangeCount - 1].Pattern = new Pattern();
            activeForm.ChangeList[activeForm.ChangeCount - 1].Pattern = activeForm.Tracks.ShownPattern;

            x2 = CursorX;
            x1 = SelectionX;

            if (x1 > x2)
            {
                x1 = x2;
                x2 = SelectionX;
            }

            y1 = SelectionY;
            y2 = CurrentPatternLine();

            if (y1 > y2)
            {
                y1 = y2;
                y2 = SelectionY;
            }

            if (x1 == x2 && y1 == y2)
            {
                x2 = 48;
                y2 = ShownPattern.Length - 1;
            }

            if (l > y2 - y1 + 1)
                l = y2 - y1 + 1;

            c = l;

            for (l = 0; l < c; l++)
            {
                m = x1;
                newEnvelope = ShownPattern.Lines[y1 + l].Envelope;
                newNoise = ShownPattern.Lines[y1 + l].Noise;
                newCommand[0] = ShownPattern.Lines[y1 + l].Channel[0].AdditionalCommand;
                newCommand[1] = ShownPattern.Lines[y1 + l].Channel[1].AdditionalCommand;
                newCommand[2] = ShownPattern.Lines[y1 + l].Channel[2].AdditionalCommand;

                for (k = i; k <= j; k++)
                {
                    if (nums[l, k] >= 0)
                    {
                        if (ChildForm.NotePoses.Contains(m))
                        {
                            if (nums[l, k] >= 256 - 2)
                            {
                                if (!merge || nums[l, k] != 255)
                                    ShownPattern.Lines[y1 + l].Channel[MainForm.ChanAlloc[(m - 8) / 14]].Note = (sbyte)(nums[l, k] - 256);
                            }
                        }
                        else
                        {
                            if (m == 0 && nums[l, k] >= 256 - 2)
                            {
                                int e = (int)Math.Round(
                                    VTModule.GetNoteFreq(((ChildForm)ParentWin).VTM.NoteTable, nums[l, k] - 256) / 16.0);

                                if (e >= 2 && e < 0x10000)
                                    newEnvelope = e;
                                else
                                    newEnvelope = 0;
                            }
                            else if (m == 5 && !MainForm.DecBaseNoiseOn)
                                sz = 1;
                            else if (m == 5 && MainForm.DecBaseNoiseOn)
                                sz = 3;
                            else if (m == 6 && MainForm.DecBaseNoiseOn)
                                sz = 9;
                            else if (ChildForm.SamPoses.Contains(m))
                                sz = 31;
                            else if (ChildForm.OrnPoses.Contains(m))
                                sz = 31;
                            else
                                sz = 15;

                            if (nums[l, k] <= sz)
                            {
                                sz = (m - 8) / 14;

                                if (sz >= 0)
                                    sz = MainForm.ChanAlloc[sz];

                                switch (m)
                                {
                                    case 0:
                                        if (nums[l, k] < 256 - 2)
                                            newEnvelope = newEnvelope & 0xFFF | (nums[l, k] << 12);
                                        break;
                                    case 1:
                                        newEnvelope = newEnvelope & 0xF0FF | (nums[l, k] << 8);
                                        break;
                                    case 2:
                                        newEnvelope = newEnvelope & 0xFF0F | (nums[l, k] << 4);
                                        break;
                                    case 3:
                                        newEnvelope = newEnvelope & 0xFFF0 | nums[l, k];
                                        break;
                                    case 5:
                                        if (MainForm.DecBaseNoiseOn) // fix for dec noise
                                            newNoise = 10 * nums[l, k];
                                        else
                                            newNoise = newNoise & 15 | (nums[l, k] << 4);
                                        break;
                                    case 6:
                                        if (MainForm.DecBaseNoiseOn) // fix for dec noise
                                            newNoise = newNoise + nums[l, k];
                                        else
                                            newNoise = newNoise & 0xF0 | nums[l, k];
                                        break;
                                    case 12:
                                    case 26:
                                    case 40:
                                        if (!merge || nums[l, k] != 0)
                                            ShownPattern.Lines[y1 + l].Channel[sz].Sample = (byte)nums[l, k];
                                        break;
                                    case 13:
                                    case 27:
                                    case 41:
                                        if (!merge || nums[l, k] != 0)
                                            ShownPattern.Lines[y1 + l].Channel[sz].Envelope = (byte)nums[l, k];
                                        break;
                                    case 14:
                                    case 28:
                                    case 42:
                                        if (!merge || nums[l, k] != 0)
                                            ShownPattern.Lines[y1 + l].Channel[sz].Ornament = (byte)nums[l, k];
                                        break;
                                    case 15:
                                    case 29:
                                    case 43:
                                        if (!merge || nums[l, k] != 0)
                                            ShownPattern.Lines[y1 + l].Channel[sz].Volume = (sbyte)nums[l, k];
                                        break;
                                    case 17:
                                    case 31:
                                    case 45:
                                        newCommand[sz].Number = (byte)nums[l, k];
                                        break;
                                    case 18:
                                    case 32:
                                    case 46:
                                        newCommand[sz].Delay = (byte)nums[l, k];
                                        break;
                                    case 19:
                                    case 33:
                                    case 47:
                                        newCommand[sz].Parameter = (byte)(newCommand[sz].Parameter & 15 | (nums[l, k] << 4));
                                        break;
                                    case 20:
                                    case 34:
                                    case 48:
                                        newCommand[sz].Parameter = (byte)(newCommand[sz].Parameter & 0xF0 | nums[l, k]);
                                        break;
                                }
                            }
                        }

                        if (m >= 48)
                            break;

                        m++;

                        if (ChildForm.ColSpace(m))
                            m++;
                        else if (m == 9 || m == 23 || m == 37)
                            m += 3;

                        if (m > x2)
                            break;
                    }
                }

                if (!merge || newEnvelope != 0)
                    ShownPattern.Lines[y1 + l].Envelope = (byte)newEnvelope;

                if (!merge || newNoise != 0)
                    ShownPattern.Lines[y1 + l].Noise = (byte)newNoise;

                if (!merge || newCommand[0].Number != 0)
                    ShownPattern.Lines[y1 + l].Channel[0].AdditionalCommand = newCommand[0];

                if (!merge || newCommand[1].Number != 0)
                    ShownPattern.Lines[y1 + l].Channel[1].AdditionalCommand = newCommand[1];

                if (!merge || newCommand[2].Number != 0)
                    ShownPattern.Lines[y1 + l].Channel[2].AdditionalCommand = newCommand[2];
            }

            CursorY = y1 - ShownFrom + CenterLineIndex;
            CursorX = x1;

            RemoveSelection();
            RecreateCaret();
            SetCaretPosition();

            activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
            activeForm.DoStep(y1, true, false);
            activeForm.ChangeList[activeForm.ChangeCount - 1].NewParams.Params.PatternCursorY = CursorY;
            activeForm.ChangeList[activeForm.ChangeCount - 1].NewParams.Params.PatternShownFrom = ShownFrom;
            activeForm.CalcTotLen();
            activeForm.ShowStat();

            HideCaret();
            RedrawTracks();
            ShowCaret();
        }

        public void ClearSelection()
        {
            int x1 = SelectionX;
            int x2 = CursorX;
            int y1 = SelectionY;
            int y2 = CurrentPatternLine();
            int c;
            int m;
            bool one;

            if (x1 > x2)
            {
                x1 = x2;
                x2 = SelectionX;
            }

            if (y1 > y2)
            {
                y1 = y2;
                y2 = SelectionY;
            }

            one = y1 == y2 && x1 == x2;

            ParentWin.SongChanged = true;
            ParentWin.BackupSongChanged = true;
            ParentWin.ValidatePattern2(ParentWin.PatternIndex);

            if (!one)
            {
                ParentWin.AddUndo(TChangeAction.PatternClearSelection, 0, 0);
                ParentWin.ChangeList[ParentWin.ChangeCount - 1].Pattern = new Pattern();
                ParentWin.ChangeList[ParentWin.ChangeCount - 1].Pattern = ParentWin.VTM.Patterns[ParentWin.PatternIndex];
                ParentWin.ChangeList[ParentWin.ChangeCount - 1].NewParams.Params.PatternCursorY = CursorY;
                ParentWin.ChangeList[ParentWin.ChangeCount - 1].NewParams.Params.PatternShownFrom = ShownFrom;
            }

            for (; y1 <= y2; y1++)
            {
                m = x1;

                do
                {
                    c = (m - 8) / 14;

                    if (c >= 0)
                        c = MainForm.ChanAlloc[c];

                    if (ChildForm.NotePoses.Contains(m))
                    {
                        if (one)
                            ParentWin.ChangeNote(ParentWin.PatternIndex, y1, c, -1);
                        else
                            ShownPattern.Lines[y1].Channel[c].Note = -1;
                    }
                    else if (one)
                    {
                        ParentWin.ChangeTracks(ParentWin.PatternIndex, y1, c, m, 0, true);
                    }
                    else
                    {
                        switch (m)
                        {
                            case 0:
                                ShownPattern.Lines[y1].Envelope = (ushort)(ShownPattern.Lines[y1].Envelope & 0xFFF);
                                break;
                            case 1:
                                ShownPattern.Lines[y1].Envelope = (ushort)(ShownPattern.Lines[y1].Envelope & 0xF0FF);
                                break;
                            case 2:
                                ShownPattern.Lines[y1].Envelope = (ushort)(ShownPattern.Lines[y1].Envelope & 0xFF0F);
                                break;
                            case 3:
                                ShownPattern.Lines[y1].Envelope = (ushort)(ShownPattern.Lines[y1].Envelope & 0xFFF0);
                                break;
                            case 5:
                                ShownPattern.Lines[y1].Noise = (byte)(ShownPattern.Lines[y1].Noise & 15);
                                break;
                            case 6:
                                ShownPattern.Lines[y1].Noise = (byte)(ShownPattern.Lines[y1].Noise & 0xF0);
                                break;
                            case 12:
                            case 26:
                            case 40:
                                ShownPattern.Lines[y1].Channel[c].Sample = 0;
                                break;
                            case 13:
                            case 27:
                            case 41:
                                ShownPattern.Lines[y1].Channel[c].Envelope = 0;
                                break;
                            case 14:
                            case 28:
                            case 42:
                                ShownPattern.Lines[y1].Channel[c].Ornament = 0;
                                break;
                            case 15:
                            case 29:
                            case 43:
                                ShownPattern.Lines[y1].Channel[c].Volume = 0;
                                break;
                            case 17:
                            case 31:
                            case 45:
                                ShownPattern.Lines[y1].Channel[c].AdditionalCommand.Number = 0;
                                break;
                            case 18:
                            case 32:
                            case 46:
                                ShownPattern.Lines[y1].Channel[c].AdditionalCommand.Delay = 0;
                                break;
                            case 19:
                            case 33:
                            case 47:
                                ShownPattern.Lines[y1].Channel[c].AdditionalCommand.Parameter = (byte)(ShownPattern.Lines[y1].Channel[c].AdditionalCommand.Parameter & 0x0F);
                                break;
                            case 20:
                            case 34:
                            case 48:
                                ShownPattern.Lines[y1].Channel[c].AdditionalCommand.Parameter = (byte)(ShownPattern.Lines[y1].Channel[c].AdditionalCommand.Parameter & 0xF0);
                                break;
                        }
                    }

                    if (m >= 48)
                        break;

                    m++;

                    if (ChildForm.ColSpace(m))
                        m++;
                    else if (m == 9 || m == 23 || m == 37)
                        m += 3;
                }
                while (m <= x2);
            }

            HideCaret();
            RedrawTracks();
            ShowCaret();

            this.ParentWin.CalcTotLen();
            this.ParentWin.ShowStat();
        }
    }
}
