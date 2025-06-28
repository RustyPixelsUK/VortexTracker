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
    public partial class ExportWavOptionsForm : Form
    {
        public ExportWavOptionsForm(Form parent)
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

            SampleRate.SelectedIndex = MainForm.ExportSampleRate;
            BitRate.SelectedIndex = MainForm.ExportBitRate;
            Channels.SelectedIndex = MainForm.ExportChannels;
            Chip.SelectedIndex = MainForm.ExportChip;
            LoopRepeats.Value = MainForm.ExportRepeats;
        }

        private void CenterForm()
        {
            Form parent = Owner;
            int x = parent.Left + (parent.Width - Width) / 2;
            int y = parent.Top + (parent.Height - Height) / 2;
            Location = new Point(x, y);
        }

        // Public declarations
        public int GetSampleRate()
        {
            int result = 44100;

            switch (MainForm.ExportSampleRate)
            {
                case 0:
                    result = 22050;
                    break;
                case 1:
                    result = 44100;
                    break;
                case 2:
                    result = 48000;
                    break;
                case 3:
                    result = 88200;
                    break;
                case 4:
                    result = 96000;
                    break;
                case 5:
                    result = 192000;
                    break;
            }

            return result;
        }

        public int GetBitRate()
        {
            int result = 16;

            switch (MainForm.ExportBitRate)
            {
                case 0:
                    result = 16;
                    break;
                case 1:
                    result = 24;
                    break;
                case 2:
                    result = 32;
                    break;
            }

            return result;
        }

        public int GetNumChannels()
        {
            int result = 2;

            switch (MainForm.ExportChannels)
            {
                case 0:
                    result = 1;
                    break;
                case 1:
                    result = 2;
                    break;
            }

            return result;
        }

        public ChipType GetChip()
        {
            ChipType result = ChipType.YM;

            switch (MainForm.ExportChip)
            {
                case 0:
                    result = ChipType.AY;
                    break;
                case 1:
                    result = ChipType.YM;
                    break;
            }

            return result;
        }

        public int GetRepeats()
        {
            int result = MainForm.ExportRepeats;
            return result;
        }

        public void ExportButton_Click(object sender, EventArgs e)
        {
            MainForm.ExportSampleRate = SampleRate.SelectedIndex;
            MainForm.ExportBitRate = BitRate.SelectedIndex;
            MainForm.ExportChannels = Channels.SelectedIndex;
            MainForm.ExportChip = Chip.SelectedIndex;
            MainForm.ExportRepeats = (int)LoopRepeats.Value;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        public void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}

