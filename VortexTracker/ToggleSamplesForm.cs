// 
// This is part of Vortex Tracker II project
// 
// (c)2000-2009 S.V.Bulba
// Author: Sergey Bulba, vorobey@mail.khstu.ru
// Support page: http://bulba.untergrund.net/
// 
// Version 2.0 and later
// (c)2017-2019 Ivan Pirog, ivan.pirog@gmail.com
// 
// C# Port by Ben Baker https://baker76.com

using LibVT;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace VortexTracker
{
    public partial class ToggleSamplesForm : Form
    {
        public static CheckBox[] ToglSam = new CheckBox[32];

        public ToggleSamplesForm(Form parent)
        {
            Owner = parent;

            InitializeComponent();
        }

        private void ToggleSamplesForm_Load(object sender, EventArgs e)
        {
            int x = 8, y = 8;

            for (int i = 1; i < 32; i++)
            {
                ToglSam[i] = new CheckBox();
                CheckBox checkBox = ToglSam[i];
                checkBox.Parent = this;
                checkBox.Top = y;
                y += checkBox.Height + 8;
                checkBox.Left = x;

                if (i % 8 == 0)
                {
                    x += 40;
                    y = 8;
                }

                checkBox.Text = VTModule.SampToStr(i);
                checkBox.Width = 32;
                checkBox.Tag = i;
                checkBox.Checked = true;
                checkBox.Click += CheckBox_Click;
            }

            this.ClientSize = new Size(4 * 40 + 4, 8 * (ToglSam[1].Height + 8) + 8);
        }

        private void ToggleSamplesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;

            Owner?.Activate();

            this.Hide();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (!this.Visible)
                return;

            CenterForm();
            CheckUsedSamples();
        }

        private void CenterForm()
        {
            Form parent = Owner;
            int x = parent.Left + (parent.Width - Width) / 2;
            int y = parent.Top + (parent.Height - Height) / 2;
            Location = new Point(x, y);
        }

        public void CheckUsedSamples()
        {
            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
            VTM vtm = activeForm.VTM;

            for (int i = 1; i < 32; i++)
                ToglSam[i].Enabled = false;

            for (int posNum = 0; posNum < vtm.Positions.Length; posNum++)
            {
                Pattern pattern = vtm.Patterns[vtm.Positions.Value[posNum]];

                for (int patLine = 0; patLine < pattern.Length; patLine++)
                {
                    if (pattern.Lines[patLine].Channel[0].Sample > 0)
                        ToglSam[pattern.Lines[patLine].Channel[0].Sample].Enabled = true;

                    if (pattern.Lines[patLine].Channel[1].Sample > 0)
                        ToglSam[pattern.Lines[patLine].Channel[1].Sample].Enabled = true;

                    if (pattern.Lines[patLine].Channel[2].Sample > 0)
                        ToglSam[pattern.Lines[patLine].Channel[2].Sample].Enabled = true;
                }
            }
        }

        public void CheckBox_Click(object sender, EventArgs e)
        {
            if (Globals.MainForm.MdiChildren.Length == 0)
                return;

            CheckBox checkBox = (CheckBox)sender;
            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;

            if (activeForm.VTM == null)
                return;

            int sam = (int)checkBox.Tag;
            activeForm.ValidateSample2(sam);
            activeForm.VTM.Samples[sam].Enabled = checkBox.Checked;
        }
    }
}

