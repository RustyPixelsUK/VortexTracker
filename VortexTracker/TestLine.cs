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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VortexTracker.Win32;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using VortexTracker.Rendering;

namespace VortexTracker
{
    public class TestLine : Control
    {
        public int CharWidth = 0;
        public int CharHeight = 0;
        public int CursorX = 0;
        public bool BigCaret = false;
        public Keys KeyPressed = 0;
        public int TestOct = 0;
        public bool TestSample = false;
        public ChildForm ParentWin = null;
        public int CurrentMidiNote = 0;
        public int NoteCounter = 0;
        public bool Preview = false;
        public int[] Arp;

        public TestLine(Control parent)
            : base(parent, null)
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.StandardClick |
                ControlStyles.UseTextForAccessibility |
                ControlStyles.StandardDoubleClick |
                ControlStyles.FixedHeight,
                true);
            this.UpdateStyles();
            this.DoubleBuffered = true;
            this.TabStop = true;
            //this.ParentColor = false;
            //this.BevelKind = bkTile;
            //this.BevelInner = bvLowered;
            KeyPressed = 0;
            this.Font = Globals.MainForm.TestLineFont;
            TestOct = 4;
            CursorX = 8;
            NoteCounter = 0;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case Win32.WM_SYSCHAR:
                    if (GetKeyState(Keys.Menu) < 0)
                        PeekMessage(out _, IntPtr.Zero, WM_CHAR, WM_CHAR, Win32.PM_REMOVE);
                    break;
                case Win32.WM_GETDLGCODE:
                    m.Result = new IntPtr(unchecked((int)(~DLGC_WANTTAB)));
                    return;
                case Win32.WM_SETFOCUS:
                    IntPtr focusedWnd = m.WParam; 
                    if (!IsWindow(focusedWnd))
                        m.WParam = IntPtr.Zero;
                    Redraw();
                    CreateCaret();
                    SetCaretPosition();
                    ShowCaret(this.Handle);
                    ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
                    activeForm.ShowStat();
                    m.Result = new IntPtr(-1);
                    break;
                case WM_KILLFOCUS:
                    DestroyCaret();
                    Redraw();
                    m.Result = new IntPtr(-1);
                    break;
            }
            base.WndProc(ref m);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //Redraw(e.Graphics.GetHdc());
            //e.Graphics.ReleaseHdc();
            Redraw(e.Graphics);
        }

        public void Redraw()
        {
            this.Invalidate();
            this.Update();
        }

        public void Redraw(Graphics g)
        {
            if (!ParentWin.Visible)
                return;

            if (ParentWin.IsClosed)
                return;

            if (ParentWin.VTM == null)
                return;

            Line line = ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0];
            string s = VTModule.Int4DToStr(line.Envelope) + '|' + VTModule.Int2DToStr(line.Noise) + '|';
            ChannelLine channel = line.Channel[0];
            s += VTModule.NoteToStr(channel.Note) + ' ' + VTModule.SampToStr(channel.Sample) + VTModule.Int1DToStr(channel.Envelope) + VTModule.Int1DToStr(channel.Ornament) + VTModule.Int1DToStr(channel.Volume) + ' ' + VTModule.Int1DToStr(channel.AdditionalCommand.Number) + VTModule.Int1DToStr(channel.AdditionalCommand.Delay) + VTModule.Int2DToStr(channel.AdditionalCommand.Parameter);

            GDI.DrawText(g, this.Font, 0, 0, s, this.ForeColor);
        }

        public void CreateCaret()
        {
            if (CursorX == 8)
            {
                BigCaret = true;
                Win32.CreateCaret(this.Handle, 0, CharWidth * 3, CharHeight);
            }
            else
            {
                BigCaret = false;
                Win32.CreateCaret(this.Handle, 0, CharWidth, CharHeight);
            }
        }

        public void SetCaretPosition()
        {
            SetCaretPos(CharWidth * CursorX, 0);
        }

        public void RecreateCaret()
        {
            if (CursorX == 8)
            {
                if (!BigCaret)
                {
                    DestroyCaret();
                    CreateCaret();
                    ShowCaret(this.Handle);
                }
            }
            else if (BigCaret)
            {
                DestroyCaret();
                CreateCaret();
                ShowCaret(this.Handle);
            }
        }

        public void TestLineMidiOn(int note)
        {
            if (!WaveOutAPI.IsPlaying || AY.PlayMode == PlayModes.PlayLine)
                VTModule.PlayArgs[0].ChannelParams[VTModule.CenterChannel].Note = (byte)ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].Note;

            ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].Note = (sbyte)note;
            ParentWin.DoAutoEnv(-1, TestSample ? 1 : 0, 0);
            
            HideCaret(this.Handle);
            Redraw();
            ShowCaret(this.Handle);

            ParentWin.RestartPlayingLine(-(TestSample ? 1 : 0) - 1);
            ParentWin.PlayStopState = PlayStopState.Stop;

            this.CurrentMidiNote = note;

            if (TestSample)
            {
                ParentWin.Samples.HideCaret();
                ParentWin.Samples.Redraw();
                ParentWin.Samples.ShowCaret();
            }
        }

        public void TestLineMidiOff(int note)
        {
            if (this.CurrentMidiNote != note)
                return;

            if (AY.PlayMode == PlayModes.PlayLine && WaveOutAPI.IsPlaying &&
                ((ChildForm.PlayingWindow[1] == ParentWin) ||
                (ChildForm.PlayingWindow[2] == ParentWin) ||
                (ChildForm.PlayingWindow[3] == ParentWin)))
            {
                WaveOutAPI.ResetPlaying();
                ParentWin.PlayStopState = PlayStopState.Play;
            }

            KeyPressed = 0;
        }

        public void PlayCurrentNote()
        {
            if (ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].Note < 0)
                return;

            ParentWin.PlayStopState = PlayStopState.Stop;

            if (!WaveOutAPI.IsPlaying || AY.PlayMode == PlayModes.PlayLine)
            {
                VTModule.PlayArgs[0].ChannelParams[VTModule.CenterChannel].Note = (byte)ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].Note;

                if (Preview)
                {
                    ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 2].Channel[0] = ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0];
                    if (TestSample)
                        ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 2].Channel[0].Sample = VTModule.PreviewSampleIndex;
                    else
                        ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 2].Channel[0].Ornament = VTModule.PreviewOrnamentIndex;
                }
            }

            ParentWin.DoAutoEnv(-1, TestSample ? 1 : 0, 0);

            HideCaret(ParentWin.Handle);
            Redraw();
            ShowCaret(ParentWin.Handle);

            if (Preview)
                ParentWin.RestartPlayingLine(-(TestSample ? 3 : 2) - 1);
            else
                ParentWin.RestartPlayingLine(-(TestSample ? 1 : 0) - 1);

            Preview = false;
        }

        // Testline
        public void TestLineKeyDown_DoNoteKey(object sender, KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool isCtrlDown = (e.Modifiers & Keys.Control) == Keys.Control;
            int note;

            if (e.KeyValue >= 256)
                return;

            note = MainForm.NoteKeys[e.KeyValue];

            if (note <= -2)
                return;

            if (note > 32)
            {
                TestOct = note & 31;

                if (ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].Note >= 0)
                    note = (ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].Note % 12) + (TestOct - 1) * 12;
                else
                    note = ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].Note;
            }
            else if (note >= 0)
                note += (TestOct - 1) * 12;

            if (note >= 96)
                return;

            if (isShiftDown)
            {
                if (note < 96 - 12)
                    note += 12;
            }
            else if (isShiftDown && isCtrlDown)
            {
                if (note >= 12)
                    note -= 12;
            }

            if (TestSample)
                ParentWin.Samples.RecalcBaseNote(note);

            KeyPressed = e.KeyCode;

            if (ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].Note >= 0)
            {
                if (!WaveOutAPI.IsPlaying || AY.PlayMode == PlayModes.PlayLine)
                    VTModule.PlayArgs[0].ChannelParams[VTModule.CenterChannel].Note = (byte)ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].Note;
            }

            ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].Note = (sbyte)note;
            ParentWin.DoAutoEnv(-1, TestSample ? 1 : 0, 0);
            
            HideCaret(this.Handle);
            Redraw();
            ShowCaret(this.Handle);
            
            ParentWin.PlayStopState = PlayStopState.Stop;
            ParentWin.RestartPlayingLine(-(TestSample ? 1 : 0) - 1);

            if (TestSample)
            {
                ParentWin.Samples.HideCaret();
                ParentWin.Samples.Redraw();
                ParentWin.Samples.ShowCaret();
            }
            else
            {
                ParentWin.Ornaments.HideCaret();
                ParentWin.Ornaments.Redraw();
                ParentWin.Ornaments.ShowCaret();
            }
        }

        public void TestLineKeyDown_DoOtherKeys(object sender, KeyEventArgs e)
        {
            int i;
            int index;

            if (CursorX == 5)
                i = 1;
            else if (CursorX == 12)
                i = 31;
            else
                i = 15;

            if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                index = e.KeyCode - Keys.D0;
            else
                index = e.KeyCode - Keys.A + 10;

            if (index < 0 || index > i)
                return;

            KeyPressed = e.KeyCode;

            switch (CursorX)
            {
                case 0:
                    // 0..3:
                    // begin
                    // Note := NoteKeys[Key];
                    // TChildWin(ParWind).VTM.ReservedPattern.Items[Ord(TestSample)].Envelope :=
                    // round(GetNoteFreq(TChildWin(ParWind).VTM.Ton_Table, Note) / 16);
                    // end;
                    ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Envelope = (byte)(ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Envelope & 0xFFF | (index << 12));
                    break;
                case 1:
                    ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Envelope = (byte)(ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Envelope & 0xF0FF | (index << 8));
                    break;
                case 2:
                    ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Envelope = (byte)(ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Envelope & 0xFF0F | (index << 4));
                    break;
                case 3:
                    ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Envelope = (byte)(ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Envelope & 0xFFF0 | index);
                    break;
                case 5:
                    ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Noise = (byte)(ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Noise & 15 | (index << 4));
                    break;
                case 6:
                    ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Noise = (byte)(ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Noise & 0xF0 | index);
                    break;
                case 12:
                    ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].Sample = (byte)index;
                    if (index > 0 && TestSample)
                        ParentWin.SampleNumUpDown.Value = index;
                    break;
                case 13:
                    ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].Envelope = (byte)index;
                    ParentWin.DoAutoEnv(-1, TestSample ? 1 : 0, 0);
                    break;
                case 14:
                    ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].Ornament = (byte)index;
                    if (index > 0 && !TestSample)
                        ParentWin.OrnamentNumUpDown.Value = index;
                    break;
                case 15:
                    ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].Volume = (sbyte)index;
                    break;
                case 17:
                    ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].AdditionalCommand.Number = (byte)index;
                    break;
                case 18:
                    ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].AdditionalCommand.Delay = (byte)index;
                    break;
                case 19:
                    ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].AdditionalCommand.Parameter = (byte)(ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].AdditionalCommand.Parameter & 15 | (index << 4));
                    break;
                case 20:
                    ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].AdditionalCommand.Parameter = (byte)(ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].AdditionalCommand.Parameter & 0xF0 | index);
                    break;
            }

            HideCaret(this.Handle);
            Redraw();
            ShowCaret(this.Handle);

            if (WaveOutAPI.IsPlaying && AY.PlayMode == PlayModes.PlayModule)
                return;

            ParentWin.PlayStopState = PlayStopState.Stop;
            ParentWin.RestartPlayingLine(-(TestSample ? 1 : 0) - 1);
        }

        public void TestLine_KeyDown(object sender, KeyEventArgs e)
        {
            bool isNoneDown = (e.Modifiers == Keys.None);
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool isCtrlDown = (e.Modifiers & Keys.Control) == Keys.Control;
            bool isAltDown = (e.Modifiers & Keys.Alt) == Keys.Alt;
            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
            short note;

            // Change octave: Alt + 0..9
            if (isAltDown && (e.KeyValue >= 49 && e.KeyValue <= 56))
            {
                if (KeyPressed == e.KeyCode)
                    return;

                KeyPressed = e.KeyCode;
                TestOct = (int)(e.KeyCode - 48);
                
                note = (short)((ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].Note % 12) + (TestOct - 1) * 12);
                
                if (TestSample)
                    ParentWin.Samples.RecalcBaseNote(note);
                
                ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].Note = (sbyte)note;
                
                HideCaret(this.Handle);
                Redraw();
                ShowCaret(this.Handle);

                if (TestSample && ParentWin.SampleToneShiftAsNoteCheckBox.Checked)
                {
                    ParentWin.Samples.HideCaret();
                    ParentWin.Samples.Redraw();
                    ParentWin.Samples.ShowCaret();
                }

                if (!TestSample && ParentWin.OrnamentToneShiftAsNoteCheckBox.Checked)
                {
                    ParentWin.Ornaments.HideCaret();
                    ParentWin.Ornaments.Redraw();
                    ParentWin.Ornaments.ShowCaret();
                }

                PlayCurrentNote();
                return;
            }

            if (isNoneDown)
            {
                switch (e.KeyCode)
                {
                    case Keys.Up:
                        // Octave UP
                        if (CursorX == 8)
                        {
                            Line line = ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0];
                            note = line.Channel[0].Note;

                            if (note < 96 - 12)
                            {
                                note += 12;
                                TestOct++;
                            }

                            if (!WaveOutAPI.IsPlaying || AY.PlayMode == PlayModes.PlayLine)
                                VTModule.PlayArgs[0].ChannelParams[VTModule.CenterChannel].Note = (byte)note;

                            if (TestSample)
                                ParentWin.Samples.RecalcBaseNote(note);

                            line.Channel[0].Note = (sbyte)note;
                            ParentWin.DoAutoEnv(-1, TestSample ? 1 : 0, 0);
                            
                            if (TestSample && ParentWin.SampleToneShiftAsNoteCheckBox.Checked)
                            {
                                ParentWin.Samples.HideCaret();
                                ParentWin.Samples.Redraw();
                                ParentWin.Samples.ShowCaret();
                            }

                            if (!TestSample && ParentWin.OrnamentToneShiftAsNoteCheckBox.Checked)
                            {
                                ParentWin.Ornaments.HideCaret();
                                ParentWin.Ornaments.Redraw();
                                ParentWin.Ornaments.ShowCaret();
                            }                            
                        }
                        // Change Sample UP
                        else if (CursorX == 12)
                        {
                            ChannelLine channelLine = ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0];
                            if (channelLine.Sample + 1 <= 31)
                            {
                                if (TestSample)
                                    ParentWin.ChangeSample(channelLine.Sample + 1, true, true);
                                else
                                    channelLine.Sample++;
                            }                           
                        }
                        // Change Envelope up
                        else if (CursorX == 13)
                        {
                            ChannelLine channelLine = ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0];
                            if (channelLine.Envelope + 1 <= 15)
                                channelLine.Envelope++;                            
                        }
                        // Change Ornament up
                        else if (CursorX == 14)
                        {
                            ChannelLine channelLine = ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0];
                            if (channelLine.Ornament + 1 <= 15)
                                channelLine.Ornament++;
                        }
                        // Change Volume up
                        else if (CursorX == 15)
                        {
                            ChannelLine channelLine = ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0];
                            if (channelLine.Volume + 1 <= 15)
                                channelLine.Volume++;
                        }
                        // Change Noise up
                        else if (CursorX == 5 || CursorX == 6)
                        {
                            Line line = ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0];
                            if (line.Noise + 1 <= 31)
                                line.Noise++;
                        }
                        // Change Global Envelope up
                        else if (CursorX >= 0 && CursorX <= 4)
                        {
                            Line line = ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0];
                            if (line.Envelope + 1 <= 0xffff)
                                line.Envelope++;
                        }
                        Redraw();
                        break;
                    case Keys.Down:
                        // Octave DOWN
                        if (CursorX == 8)
                        {
                            Line line = ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0];
                            note = line.Channel[0].Note;

                            if (note >= 12)
                            {
                                note -= 12;
                                TestOct--;
                            }

                            if (TestOct == 0)
                                TestOct = 1;

                            if (!WaveOutAPI.IsPlaying || AY.PlayMode == PlayModes.PlayLine)
                                VTModule.PlayArgs[0].ChannelParams[VTModule.CenterChannel].Note = (byte)note;
                            
                            if (TestSample)
                                ParentWin.Samples.RecalcBaseNote(note);
                            
                            line.Channel[0].Note = (sbyte)note;
                            ParentWin.DoAutoEnv(-1, TestSample ? 1 : 0, 0);

                            if (TestSample && ParentWin.SampleToneShiftAsNoteCheckBox.Checked)
                            {
                                ParentWin.Samples.HideCaret();
                                ParentWin.Samples.Redraw();
                                ParentWin.Samples.ShowCaret();
                            }

                            if (!TestSample && ParentWin.OrnamentToneShiftAsNoteCheckBox.Checked)
                            {
                                ParentWin.Ornaments.HideCaret();
                                ParentWin.Ornaments.Redraw();
                                ParentWin.Ornaments.ShowCaret();
                            }
                        }
                        // Change sample DOWN
                        else if (CursorX == 12)
                        {
                            ChannelLine channelLine = ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0];
                            if (channelLine.Sample - 1 > 0)
                            {
                                if (TestSample)
                                    ParentWin.ChangeSample(channelLine.Sample - 1, true, true);
                                else
                                    channelLine.Sample--;
                            }
                        }
                        // Change Envelope down
                        else if (CursorX == 13)
                        {
                            ChannelLine channelLine = ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0];
                            if (channelLine.Envelope > 0)
                                channelLine.Envelope--;
                        }
                        // Change Ornament down
                        else if (CursorX == 14)
                        {
                            ChannelLine channelLine = ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0];
                            if (channelLine.Ornament > 0)
                                channelLine.Ornament--;
                        }
                        // Change Volume down
                        else if (CursorX == 15)
                        {
                            ChannelLine channelLine = ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0];
                            if (channelLine.Volume - 1 > 0)
                                channelLine.Volume--;
                        }
                        // Change Noise down
                        else if (CursorX == 5 || CursorX == 6)
                        {
                            Line line = ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0];
                            if (line.Noise > 0)
                                line.Noise--;
                        }
                        // Change Global Envelope up
                        else if (CursorX >= 0 && CursorX <= 4)
                        {
                            Line line = ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0];
                            if (line.Envelope > 0)
                                line.Envelope--;
                        }
                        Redraw();
                        break;
                    case Keys.Left:
                        if (CursorX > 0)
                        {
                            if (CursorX == 12)
                                CursorX -= 4;
                            else if (ChildForm.ColSpace(CursorX - 1))
                                CursorX -= 2;
                            else
                                CursorX--;
                            RecreateCaret();
                            SetCaretPosition();
                            Redraw();
                        }
                        break;
                    case Keys.Right:
                        if (CursorX < 20)
                        {
                            CursorX++;
                            if (ChildForm.ColSpace(CursorX))
                                CursorX++;
                            else if (CursorX == 9)
                                CursorX += 3;
                            RecreateCaret();
                            SetCaretPosition();
                            Redraw();
                        }
                        break;
                    case Keys.Oemtilde:
                        if (TestSample)
                        {
                            if (ParentWin.Samples.CanFocus)
                                ParentWin.Samples.Focus();
                        }
                        else if (ParentWin.Ornaments.CanFocus)
                            ParentWin.Ornaments.Focus();
                        break;
                    default:
                        if (KeyPressed != e.KeyCode)
                        {
                            if (ChildForm.NotePoses.Contains(CursorX))
                                TestLineKeyDown_DoNoteKey(sender, e);
                            else
                                TestLineKeyDown_DoOtherKeys(sender, e);
                        }
                        break;
                }
            }
            else if (isCtrlDown)
            {
                switch (e.KeyCode)
                {
                    case Keys.Right:
                        if (CursorX < 17)
                        {
                            CursorX = ChildForm.ColTabs[ChildForm.ColTab(CursorX) + 1];
                            RecreateCaret();
                            SetCaretPosition();
                            Redraw();
                        }
                        break;
                    case Keys.Left:
                        if (CursorX > 4)
                        {
                            CursorX = ChildForm.ColTabs[ChildForm.ColTab(CursorX) - 1];
                            RecreateCaret();
                            SetCaretPosition();
                            Redraw();
                        }
                        break;
                    case Keys.Add:
                    case Keys.Subtract:
                        if (TestSample)
                        {
                            if (ParentWin.Samples.CanFocus)
                                activeForm.Samples_KeyDown(sender, e);
                        }
                        else if (ParentWin.Ornaments.CanFocus)
                            activeForm.Samples_KeyDown(sender, e);
                        break;
                    case Keys.Return:
                        if (CursorX == 12)
                            OpenSample();
                        else if (CursorX == 14)
                            OpenOrnament();
                        break;
                }
            }
            else if ((isCtrlDown && isShiftDown) || isShiftDown)
            {
                if (KeyPressed != Keys.KeyCode)
                {
                    if (ChildForm.NotePoses.Contains(CursorX))
                        TestLineKeyDown_DoNoteKey(sender, e);
                }
            }
        }

        public void OpenSample()
        {
            // Get sample num
            byte sample = ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].Sample;
            
            // Copy all tesline params
            if (!TestSample)
                ParentWin.VTM.ReservedPattern.Lines[1] = ParentWin.VTM.ReservedPattern.Lines[0];
            
            // Select sample
            ParentWin.SampleNumUpDown.Value = sample;
            
            // Activate samples tab
            ParentWin.ActivateTab(1);
           
            // Set focus
            if (ParentWin.SampleTestLine.Enabled && ParentWin.SampleTestLine.CanFocus)
            {
                ParentWin.SampleTestLine.CursorX = 8;
                ParentWin.SampleTestLine.Focus();
            }

            HideCaret(ParentWin.SampleTestLine.Handle);
            ParentWin.SampleTestLine.CreateCaret();
            ParentWin.SampleTestLine.SetCaretPosition();
            ShowCaret(ParentWin.SampleTestLine.Handle);
        }

        public void OpenOrnament()
        {
            byte Ornament;

            // Get ornament num
            Ornament = ParentWin.VTM.ReservedPattern.Lines[TestSample ? 1 : 0].Channel[0].Ornament;

            // Copy all tesline params
            if (TestSample)
                ParentWin.VTM.ReservedPattern.Lines[0] = ParentWin.VTM.ReservedPattern.Lines[1];

            // Select ornament
            ParentWin.OrnamentNumUpDown.Value = Ornament;

            // Activate ornaments tab
            ParentWin.ActivateTab(2);

            // Set focus
            if (ParentWin.OrnamentTestLine.Enabled && ParentWin.OrnamentTestLine.CanFocus)
            {
                ParentWin.OrnamentTestLine.CursorX = 8;
                ParentWin.OrnamentTestLine.Focus();
            }

            HideCaret(ParentWin.OrnamentTestLine.Handle);
            ParentWin.OrnamentTestLine.CreateCaret();
            ParentWin.OrnamentTestLine.SetCaretPosition();
            ShowCaret(ParentWin.OrnamentTestLine.Handle);
        }

        public void TestLine_MouseDown(object sender, MouseEventArgs e)
        {
            bool isShiftDown = (GetAsyncKeyState(Keys.ShiftKey) & 0x8000) != 0;
            bool isCtrlDown = (GetAsyncKeyState(Keys.ControlKey) & 0x8000) != 0;
            int x1 = e.X / CharWidth;

            // Ctrl+Click on sample position -> open sample
            if (x1 == 12 && isCtrlDown)
            {
                OpenSample();
                return;
            }

            // Ctrl+Click on ornament position -> open ornament
            if (x1 == 14 && isCtrlDown)
            {
                OpenOrnament();
                return;
            }

            if (x1 >= 9 && x1 <= 10)
                x1 = 8;

            if (!ChildForm.ColSpace(x1))
                CursorX = x1;

            if (this.Focused)
            {
                HideCaret(this.Handle);
                RecreateCaret();
                SetCaretPosition();
                Redraw();
                ShowCaret(this.Handle);
            }
            else
                this.Focus();
        }

        public void TestLine_KeyUp(object sender, KeyEventArgs e)
        {
            if (KeyPressed == e.KeyCode)
            {
                if (AY.PlayMode == PlayModes.PlayLine && WaveOutAPI.IsPlaying && ChildForm.PlayingWindow[1] == ParentWin)
                {
                    WaveOutAPI.ResetPlaying();
                    ParentWin.PlayStopState = PlayStopState.Play;
                }

                KeyPressed = 0;
            }
        }

        public void TestLine_Leave(object sender, EventArgs e)
        {
            KeyPressed = 0;
        }
    }
}
