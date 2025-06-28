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
// Version 2.6 - 2.6.1
// (c)2022-2025 Dexus (Volutar), https://github.com/Volutar
// 
// Version 3.0+ (C# port)
// (c)2025 Ben Baker, https://github.com/benbaker76

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
        public AboutForm()
        {
            InitializeComponent();

            Assembly assembly = Assembly.GetExecutingAssembly();

            // Load a embedded resources
            using (Stream resourceStream = assembly.GetManifestResourceStream("VortexTracker.Resources.Images.Vortex3.png"))
            {
                if (resourceStream != null)
                    this.ProgramIcon.Image = new Bitmap(resourceStream);
            }

            MyProductName.Text = Application.ProductName;
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

