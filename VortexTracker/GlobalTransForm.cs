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

namespace VortexTracker
{
    public partial class GlobalTransForm : Form
    {
        public GlobalTransForm(Form parent)
        {
            Owner = parent;

            InitializeComponent();
        }

        /* public void Edit2Exit(object sender, EventArgs e)
        {
            Edit2.Text = (UpDown1.Value).ToString();
        } */

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (!this.Visible)
                return;

            CenterForm();

            if (Globals.MainForm.MdiChildren.Length != 0)
            {
                PatternNumUpDown.Value = ((ChildForm)(Globals.MainForm.ActiveMdiChild)).PatternIndex;
            }
            //Edit8.SelectAll();
            //Edit8.Focus();
        }

        private void CenterForm()
        {
            Form parent = Owner;
            int x = parent.Left + (parent.Width - Width) / 2;
            int y = parent.Top + (parent.Height - Height) / 2;
            Location = new Point(x, y);
        }

        /* public void Edit8Exit(object sender, EventArgs e)
        {
            Edit8.Text = (UpDown8.Value).ToString();
        } */

        public void TransposeButton_Click(object sender, EventArgs e)
        {
            bool[] Chans = new bool[3];
            ChildForm CurrentWindow;

            if (Globals.MainForm.MdiChildren.Length == 0)
                return;

            if (!ChannelATrack.Checked && !ChannelBTrack.Checked && !ChannelCTrack.Checked && !EnvelopeTrack.Checked)
                return;

            if (NumSemitonesUpDown.Value == 0)
                return;

            CurrentWindow = (ChildForm)Globals.MainForm.ActiveMdiChild;
            Chans[0] = ChannelATrack.Checked;
            Chans[1] = ChannelBTrack.Checked;
            Chans[2] = ChannelCTrack.Checked;

            if (WholeModule.Checked)
            {
                if (MessageBox.Show(this, "This Operation Cannot be Undone. Are You Sure You Want to Continue?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    CurrentWindow.DisposeUndo(true);

                    for (int i = 0; i < VTModule.MaxPatternIndex; i++)
                        Globals.MainForm.TransposeColumns(CurrentWindow, i, EnvelopeTrack.Checked, Chans, 0, VTModule.MaxPatternLength - 1, (int)NumSemitonesUpDown.Value, false);
                }
            }
            else
                Globals.MainForm.TransposeColumns(CurrentWindow, (int)PatternNumUpDown.Value, EnvelopeTrack.Checked, Chans, 0, VTModule.MaxPatternLength - 1, (int)NumSemitonesUpDown.Value, true);
        }

        private void GlobalTransForm_FormClosing(object sender, FormClosingEventArgs e)
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

