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

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VortexTracker
{
    public partial class AboutForm : Form
    {
        private string _buildDate = "";

        public AboutForm()
        {
            InitializeComponent();

            Assembly assembly = Assembly.GetExecutingAssembly();

            // Load a embedded resources
            using (Stream resourceStream = assembly.GetManifestResourceStream("VortexTracker.Resources.Images.Vortex3.png"))
            {
                if (resourceStream != null)
                {
                    this.ProgramIcon.Image = new Bitmap(resourceStream);
                }
            }

            Version.Text = MainForm.HalfVersString;
            BuildDateLabel.Text = $"Date of build: {MainForm.BuildDateTime.ToString()}";
        }

        private void AboutForm_FormClosing(object sender, FormClosingEventArgs e)
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

