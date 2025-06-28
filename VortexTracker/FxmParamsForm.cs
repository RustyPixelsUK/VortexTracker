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
using System.Windows.Forms;
using System.Collections;
using LibVT;

namespace VortexTracker
{
    public partial class FxmParamsForm : Form
    {
        public FxmParamsForm()
        {
            InitializeComponent();
        }

        public void LengthBox_TextChanged(object sender, EventArgs e)
        {
            if (LengthBox.Modified)
            {
                CheckAll();
            }
        }

        public void LoopInterruptBox_TextChanged(object sender, EventArgs e)
        {
            if (LoopInterruptBox.Modified)
            {
                CheckAll();
            }
        }

        public void InitialTempoBox_TextChanged(object sender, EventArgs e)
        {
            if (InitialTempoBox.Modified)
            {
                CheckAll();
            }
        }

        public void PatternSizeBox_TextChanged(object sender, EventArgs e)
        {
            if (PatternSizeBox.Modified)
            {
                CheckAll();
            }
        }

        public void GlobalTransposeBox_TextChanged(object sender, EventArgs e)
        {
            if (GlobalTransposeBox.Modified)
            {
                CheckAll();
            }
        }

        public void AmadAndSixBox_ValueChanged(object sender, EventArgs e)
        {
            if (AmadAndSixBox.Modified)
            {
                CheckAll();
            }
        }

        private void TFXMParams_Load(object sender, EventArgs e)
        {
            CheckAll();
        }

        private void FxmParamsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;

            Owner?.Activate();

            this.Hide();
        }

        public void CheckAll()
        {
            bool flg;
            string s;
            int i;
            int l;
            bool success;
            s = LengthBox.Text.Trim();
            success = int.TryParse(s, out l);

            flg = success && l > 0 && l <= 2167500;
            if (flg)
            {
                s = LoopInterruptBox.Text.Trim();
                success = int.TryParse(s, out i);
                flg = success && (i >= 0) && (i < l);
            }

            if (flg)
            {
                s = InitialTempoBox.Text.Trim();
                success = int.TryParse(s, out i);
                flg = success && (i >= 1 && i <= 255);
            }

            if (flg)
            {
                s = PatternSizeBox.Text.Trim();
                success = int.TryParse(s, out i);
                flg = success && (i > 0) && (i <= VTModule.MaxPatternLength);
            }

            if (flg)
            {
                s = GlobalTransposeBox.Text.Trim();
                success = int.TryParse(s, out i);
                flg = success && (i + 95 >= 0 && i + 95 <= 190);
            }

            if (flg)
            {
                s = AmadAndSixBox.Text.Trim();
                success = int.TryParse(s, out i);
                flg = success && (i == 0 || i == 1 || i == 3 || i == 7 || i == 15 || i == 31);
            }

            StartButton.Enabled = flg;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void DefaultButton_Click(object sender, EventArgs e)
        {
        }
    }
}

