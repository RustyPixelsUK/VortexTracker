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

        // Public declarations
        public void SetRTFText(string rtfText)
        {
            Info.Rtf = rtfText;
            /* StringWriter ss;
            string emptyStr = "";
            ss = new StringWriter(emptyStr);
            try {
                ss.Write(RTFText);
                ss.Position = 0;
                Info.PlainText = false;
                Info.Lines.BeginUpdate;
                Info.Lines.LoadFromStream(ss);
                 Info.Lines.EndUpdate;
            } finally {
                ss.Dispose();
            } */
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

