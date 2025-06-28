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
    public partial class TurboSoundForm : Form
    {
        public TurboSoundForm(Form parent)
        {
            Owner = parent;

            InitializeComponent();
        }

        public void FormCreate(object sender, EventArgs e)
        {
            ListBox1.Items.Add("2nd soundchip is disabled");
            ListBox1.SelectedIndex = 0;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (!this.Visible)
                return;

            CenterForm();
        }

        private void CenterForm()
        {
            Form parent = Owner;
            int x = parent.Left + (parent.Width - Width) / 2;
            int y = parent.Top + (parent.Height - Height) / 2;
            Location = new Point(x, y);
        }

        public void ListBox1KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '\r':
                    this.DialogResult = DialogResult.OK;
                    break;
                case '':
                    this.DialogResult = DialogResult.Cancel;
                    break;
            }
        }

        public void ListBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}

