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
using System.Windows.Forms;
using VortexTracker.Controls;

namespace VortexTracker
{
    public partial class TracksManagerForm : Form
    {
        public TracksManagerForm(Form parent)
        {
            Owner = parent;

            InitializeComponent();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (!this.Visible)
                return;

            CenterForm();

            LinesUpDown.Maximum = VTModule.MaxPatternLength;
            LinesUpDown.Value = VTModule.MaxPatternLength;
            L1LineUpDown.Maximum = VTModule.MaxPatternLength - 1;
            L2LineUpDown.Maximum = VTModule.MaxPatternLength - 1;
        }

        private void CenterForm()
        {
            Form parent = Owner;
            int x = parent.Left + (parent.Width - Width) / 2;
            int y = parent.Top + (parent.Height - Height) / 2;
            Location = new Point(x, y);
        }

        public void TracksOp(int fPat, int fLin, int fChn, int tPat, int tLin, int tChn, int trOp, bool makeUndo)
        {
            // FPLen,TPLen,
            int i, j;
            ChannelLine channelLine;
            Pattern oldPat = null;
            bool flg;
            ChildForm currentWindow;

            if (Globals.MainForm.MdiChildren.Length == 0)
                return;

            oldPat = null;
            currentWindow = (ChildForm)Globals.MainForm.ActiveMdiChild;

            if (currentWindow.VTM.Patterns[fPat] == null && currentWindow.VTM.Patterns[tPat] == null)
                return;

            currentWindow.ValidatePattern2(fPat);
            currentWindow.ValidatePattern2(tPat);

            if (trOp == 0)
            {
                oldPat = new Pattern();
                oldPat = currentWindow.VTM.Patterns[tPat];
            }
            else
            {
                if (MessageBox.Show(this, "This Operation Cannot be Undone. Are You Sure to Continue?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                currentWindow.DisposeUndo(true);
            }
            // FPLen := VTM.Patterns[FPat].Length;
            // TPLen := VTM.Patterns[TPat].Length;
            flg = false;

            for (i = 0; i < LinesUpDown.Value; i++)
            {
                // if (i + FLin >= FPLen) or (i + TLin >= TPLen) then break;
                // Work with all pattern lines even if it greater then pattern length
                if (i + fLin >= VTModule.MaxPatternLength || i + tLin >= VTModule.MaxPatternLength)
                    break;

                flg = true;

                if (EnvelopeColumn.Checked)
                {
                    j = currentWindow.VTM.Patterns[fPat].Lines[i + fLin].Envelope;

                    switch (trOp)
                    {
                        case 0:
                            currentWindow.VTM.Patterns[tPat].Lines[i + tLin].Envelope = (ushort)j;
                            break;
                        case 1:
                            currentWindow.VTM.Patterns[tPat].Lines[i + tLin].Envelope = (ushort)j;
                            currentWindow.VTM.Patterns[fPat].Lines[i + fLin].Envelope = 0;
                            break;
                        case 2:
                            currentWindow.VTM.Patterns[fPat].Lines[i + fLin].Envelope = currentWindow.VTM.Patterns[tPat].Lines[i + tLin].Envelope;
                            currentWindow.VTM.Patterns[tPat].Lines[i + tLin].Envelope = (ushort)j;
                            break;
                    }
                }

                if (NoiseColumn.Checked)
                {
                    j = currentWindow.VTM.Patterns[fPat].Lines[i + fLin].Noise;
                    switch (trOp)
                    {
                        case 0:
                            currentWindow.VTM.Patterns[tPat].Lines[i + tLin].Noise = (byte)j;
                            break;
                        case 1:
                            currentWindow.VTM.Patterns[tPat].Lines[i + tLin].Noise = (byte)j;
                            currentWindow.VTM.Patterns[fPat].Lines[i + fLin].Noise = 0;
                            break;
                        case 2:
                            currentWindow.VTM.Patterns[fPat].Lines[i + fLin].Noise = currentWindow.VTM.Patterns[tPat].Lines[i + tLin].Noise;
                            currentWindow.VTM.Patterns[tPat].Lines[i + tLin].Noise = (byte)j;
                            break;
                    }
                }

                channelLine = currentWindow.VTM.Patterns[fPat].Lines[i + fLin].Channel[fChn];
                
                switch (trOp)
                {
                    case 0:
                        currentWindow.VTM.Patterns[tPat].Lines[i + tLin].Channel[tChn] = channelLine;
                        break;
                    case 1:
                        currentWindow.VTM.Patterns[tPat].Lines[i + tLin].Channel[tChn] = channelLine;
                        currentWindow.VTM.Patterns[fPat].Lines[i + fLin].Channel[fChn].Note = -1;
                        currentWindow.VTM.Patterns[fPat].Lines[i + fLin].Channel[fChn].Sample = 0;
                        currentWindow.VTM.Patterns[fPat].Lines[i + fLin].Channel[fChn].Ornament = 0;
                        currentWindow.VTM.Patterns[fPat].Lines[i + fLin].Channel[fChn].Volume = 0;
                        currentWindow.VTM.Patterns[fPat].Lines[i + fLin].Channel[fChn].Envelope = 0;
                        currentWindow.VTM.Patterns[fPat].Lines[i + fLin].Channel[fChn].AdditionalCommand.Number = 0;
                        currentWindow.VTM.Patterns[fPat].Lines[i + fLin].Channel[fChn].AdditionalCommand.Delay = 0;
                        currentWindow.VTM.Patterns[fPat].Lines[i + fLin].Channel[fChn].AdditionalCommand.Parameter = 0;
                        break;
                    case 2:
                        currentWindow.VTM.Patterns[fPat].Lines[i + fLin].Channel[fChn] = currentWindow.VTM.Patterns[tPat].Lines[i + tLin].Channel[tChn];
                        currentWindow.VTM.Patterns[tPat].Lines[i + tLin].Channel[tChn] = channelLine;
                        break;
                }
            }

            if (flg && makeUndo)
            {
                currentWindow.SongChanged = true;
                currentWindow.BackupSongChanged = true;
                if (trOp == 0)
                {
                    currentWindow.AddUndo(TChangeAction.TracksManagerCopy, tPat, 0);
                    currentWindow.ChangeList[currentWindow.ChangeCount - 1].Pattern = oldPat;
                }
            }
            else if (trOp == 0)
            {
                //oldPat.Dispose();
            }

            if (currentWindow.PatternIndex == tPat || currentWindow.PatternIndex == fPat)
                currentWindow.Tracks.RedrawTracks();
        }

        public void CopyRightButton_Click(object sender, EventArgs e)
        {
            TracksOp((int)L1PatternUpDown.Value, (int)L1LineUpDown.Value, (int)L1ChannelUpDown.Value, (int)L2PatternUpDown.Value, (int)L2LineUpDown.Value, (int)L2ChannelUpDown.Value, 0, true);
        }

        public void CopyLeftButton_Click(object sender, EventArgs e)
        {
            TracksOp((int)L2PatternUpDown.Value, (int)L2LineUpDown.Value, (int)L2ChannelUpDown.Value, (int)L1PatternUpDown.Value, (int)L1LineUpDown.Value, (int)L1ChannelUpDown.Value, 0, true);
        }

        public void MoveRightButton_Click(object sender, EventArgs e)
        {
            TracksOp((int)L1PatternUpDown.Value, (int)L1LineUpDown.Value, (int)L1ChannelUpDown.Value, (int)L2PatternUpDown.Value, (int)L2LineUpDown.Value, (int)L2ChannelUpDown.Value, 1, true);
        }

        public void MoveLeftButton_Click(object sender, EventArgs e)
        {
            TracksOp((int)L2PatternUpDown.Value, (int)L2LineUpDown.Value, (int)L2ChannelUpDown.Value, (int)L1PatternUpDown.Value, (int)L1LineUpDown.Value, (int)L1ChannelUpDown.Value, 1, true);
        }

        public void SwapButton_Click(object sender, EventArgs e)
        {
            TracksOp((int)L1PatternUpDown.Value, (int)L1LineUpDown.Value, (int)L1ChannelUpDown.Value, (int)L2PatternUpDown.Value, (int)L2LineUpDown.Value, (int)L2ChannelUpDown.Value, 2, true);
        }

        public void Transp(int Pat, int Lin, int Chn)
        {
            bool[] Chans = new bool[3];

            if (Globals.MainForm.MdiChildren.Length == 0)
                return;

            Chans[0] = false;
            Chans[1] = false;
            Chans[2] = false;
            Chans[Chn] = true;
            Globals.MainForm.TransposeColumns((ChildForm)Globals.MainForm.ActiveMdiChild, Pat, EnvelopeColumn.Checked, Chans, Lin, Lin + (int)LinesUpDown.Value - 1, (int)SemitonesUpDown.Value, true);
        }

        public void Location1Button_Click(object sender, EventArgs e)
        {
            Transp((int)L1PatternUpDown.Value, (int)L1LineUpDown.Value, (int)L1ChannelUpDown.Value);
        }

        public void Location2Button_Click(object sender, EventArgs e)
        {
            Transp((int)L2PatternUpDown.Value, (int)L2LineUpDown.Value, (int)L2ChannelUpDown.Value);
        }

        public void UpDown6_7ChangingEx(object sender, ValueChangingEventArgs e)
        {
            e.Cancel = e.NewValue < 0 || e.NewValue > 2;

            if (e.Cancel)
                return;

            if (sender == L1ChannelUpDown)
                L1ChannelUpDown.Value = e.NewValue;
            else
                L2ChannelUpDown.Value = e.NewValue;
        }

        public void Edit6_7KeyDown(NumericUpDown upDown, object sender, Keys key)
        {
            switch (key)
            {
                case Keys.Up:
                    L1ChannelUpDown.Value = upDown.Value + 1;
                    break;
                case Keys.Down:
                    L1ChannelUpDown.Value = upDown.Value - 1;
                    break;
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                    upDown.Value = key - Keys.D0;
                    break;
                case Keys.A:
                case Keys.B:
                case Keys.C:
                    upDown.Value = key - Keys.A;
                    break;
                case Keys.NumPad0:
                case Keys.NumPad1:
                case Keys.NumPad2:
                    upDown.Value = key - Keys.NumPad0;
                    break;
            }
        }

        /* public void Edit6_7KeyDown(object sender, KeyEventArgs e)
        {
            if (sender == Edit6)
            {
                Edit6_7KeyDown_SetChanUpDown(UpDown6, sender, e.KeyCode);
            }
            else
            {
                Edit6_7KeyDown_SetChanUpDown(UpDown7, sender, e.KeyCode);
            }
        } */

        public void Edit6_7KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = '\0';
        }

        private void TracksManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;

            Owner?.Activate();

            this.Hide();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}

