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

using System;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.LinkLabel;
using System.Runtime.InteropServices;
using LibVT;

namespace VortexTracker
{
    public partial class TrackInfoForm : Form
    {
        public TrackInfoForm(Form parent)
        {
            Owner = parent;

            InitializeComponent();
        }

        public void SetRTFText(string rtfText)
        {
            Info.Rtf = rtfText;
        }

        public void Init(VTM vtm)
        {
            string title = "Track Info";

            SetRTFText(vtm.Info);

            if (vtm.Title.Trim() != "")
                title = vtm.Title;

            if (vtm.Author.Trim() != "")
                title += $" by {vtm.Author}";

            this.Text = title;
            this.Left = Globals.MainForm.Left + (Globals.MainForm.Width / 2) - (this.Width / 2);
            this.Top = Globals.MainForm.Top + (Globals.MainForm.Height / 2) - (this.Height / 2);
        }

        private void TrackInfoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;

            Owner?.Activate();

            this.Hide();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}

