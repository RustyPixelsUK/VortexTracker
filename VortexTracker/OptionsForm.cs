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
using System.Threading;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Globalization;
using LibVT;
using System.Drawing;
using System.Drawing.Text;
using System.Diagnostics;
using RtMidi.Net;
using OpenTK.Audio.OpenAL;

namespace VortexTracker
{
    public partial class OptionsForm : Form
    {
        private int _clicksCounter = 0;
        private int _clicksCounter1 = 0;

        private Panel _curColorPanel = null;
        private string _colorVar;

        private bool _chanAllocChanged;
        private bool _panoramChanged;
        private Rectangle _newSIze;

        private int _savedChanAllocIndex;
        private int _savedDefaultChipFreq;
        private int _savedStdChannelsAllocation;
        private int _savedInterrupt_Freq;
        private int _savedSampleRate;
        private int _savedSampleBit;
        private int _savedNumberOfChannels;
        private int _savedBufLen_ms;
        private int _savedNumberOfBuffers;
        private string _savedWODevice;
        private ChipType _savedChipType;
        private int _savedEngineIndex;
        private FeaturesLevel _savedFeaturesLevel;
        private bool _savedDetectFeaturesLevel;
        private bool _savedVortexModuleHeader;
        private bool _savedDetectModuleHeader;
        private bool _savedIsFilt;
        private int _savedFilt_M;
        private int _savedPrior;
        private bool _savedEnvelopeAsNote = false;
        private bool _savedDecBaseLinesOn;
        private bool _savedDecBaseNoiseOn;
        //private bool _savedTestForever;
        private bool _savedHighlightSpeedOn;
        private bool _savedDupNoteParams = false;
        private bool _savedMoveBetweenPatrns = false;
        private short _savedDefaultTable;
        private bool _savedDisableSeparators;
        private bool _savedAutoBackupsOn;
        private byte _savedAutoBackupsMins;
        private Font _savedEditorFont;
        private string _savedThemeName;
        private byte[] _savedPanoram = new byte[3];
        private bool _savedDisableHints;
        private string _savedTemplateSongPath;
        private byte _savedStartupAction;
        private int _savedWinThemeIndex;
        private bool _savedDisableCtrlClick;
        private bool _savedDisableInfoWin;
        private int _savedManualChipFreq;
        //private int _savedManualIntFreq;
        private int _savedPositionSize;
        private int _savedDCType;
        private int _savedDCCutOff;

        public OptionsForm(Form parent)
        {
            Owner = parent;

            InitializeComponent();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            _clicksCounter = 0;
            _clicksCounter1 = 0;
            OptionsTabControl.Focus();
            MIDIDeviceName.Text = Globals.MainForm.MidiInputDeviceInfo?.Name;
            MainForm.DisableUpdateChilds = false;
            UpdateAudioSettings();

            //APan.TickStyle = tsManual;
            APanTrackBar.Minimum = 0;
            APanTrackBar.Maximum = 255;
            APanTrackBar.TickFrequency = 64;
            /* Win32.SendMessage(APan.Handle, Win32.TBM_SETTIC, 0, 0);
            Win32.SendMessage(APan.Handle, Win32.TBM_SETTIC, 0, 64);
            Win32.SendMessage(APan.Handle, Win32.TBM_SETTIC, 0, 128);
            Win32.SendMessage(APan.Handle, Win32.TBM_SETTIC, 0, 192);
            Win32.SendMessage(APan.Handle, Win32.TBM_SETTIC, 0, 255); */
            APanTrackBar.MouseUp += APanTrackBar_MouseUp;
            //BPan.TickStyle = tsManual;
            BPanTrackBar.Minimum = 0;
            BPanTrackBar.Maximum = 255;
            BPanTrackBar.TickFrequency = 64;
            /* Win32.SendMessage(BPan.Handle, Win32.TBM_SETTIC, 0, 0);
            Win32.SendMessage(BPan.Handle, Win32.TBM_SETTIC, 0, 64);
            Win32.SendMessage(BPan.Handle, Win32.TBM_SETTIC, 0, 128);
            Win32.SendMessage(BPan.Handle, Win32.TBM_SETTIC, 0, 192);
            Win32.SendMessage(BPan.Handle, Win32.TBM_SETTIC, 0, 255); */
            BPanTrackBar.MouseUp += BPanTrackBar_MouseUp;
            //CPan.TickStyle = tsManual;
            CPanTrackBar.Minimum = 0;
            CPanTrackBar.Maximum = 255;
            CPanTrackBar.TickFrequency = 64;
            /* Win32.SendMessage(CPan.Handle, Win32.TBM_SETTIC, 0, 0);
            Win32.SendMessage(CPan.Handle, Win32.TBM_SETTIC, 0, 64);
            Win32.SendMessage(CPan.Handle, Win32.TBM_SETTIC, 0, 128);
            Win32.SendMessage(CPan.Handle, Win32.TBM_SETTIC, 0, 192);
            Win32.SendMessage(CPan.Handle, Win32.TBM_SETTIC, 0, 255); */
            CPanTrackBar.MouseUp += CPanTrackBar_MouseUp;
            AyumiDCFiltBox.Left = DownsamplingBox.Left;
            AyumiDCFiltBox.Top = DownsamplingBox.Top;
            OptionsTabControl.SelectedIndex = 0;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (!this.Visible)
                return;

            CenterForm();
            MainForm.DisableUpdateChilds = true;
            VortexTracker.HotKeys.InitOptionsHotKeys();
            InitFonts();
            InitFileAssociations();
            ColorThemes.FillColorThemesList();
            _savedPositionSize = MainForm.PositionSize;
            _savedDisableCtrlClick = MainForm.DisableCtrlClick;
            DisableCtrlClickOpt.Checked = MainForm.DisableCtrlClick;
            _savedDisableInfoWin = MainForm.DisableInfoWin;
            DisableInfoWinOpt.Checked = MainForm.DisableInfoWin;
            StartsAction.SelectedIndex = Globals.MainForm.StartupAction;
            _savedStartupAction = Globals.MainForm.StartupAction;
            TemplateSong.Text = Globals.MainForm.TemplateSongPath;
            _savedTemplateSongPath = Globals.MainForm.TemplateSongPath;
            DisablePatSeparators.Checked = MainForm.DisableSeparators;
            _savedDisableSeparators = MainForm.DisableSeparators;
            DefaultFreqTableUpDown.Value = Globals.MainForm.DefaultTable;
            _savedDefaultTable = Globals.MainForm.DefaultTable;
            TableName.Text = VTModule.NoteTableNames[Globals.MainForm.DefaultTable];
            BackupEveryMins.Value = MainForm.AutoBackupsMins;
            _savedAutoBackupsMins = MainForm.AutoBackupsMins;
            AutoSaveBackups.Checked = MainForm.AutoBackupsOn;
            _savedAutoBackupsOn = MainForm.AutoBackupsOn;
            BackupEveryMins.Enabled = MainForm.AutoBackupsOn;
            DecNumbersLines.Checked = MainForm.DecBaseLinesOn;
            _savedDecBaseLinesOn = MainForm.DecBaseLinesOn;
            DecNumbersNoise.Checked = MainForm.DecBaseNoiseOn;
            _savedDecBaseNoiseOn = MainForm.DecBaseNoiseOn;
            _savedThemeName = MainForm.ColorThemeName;
            _savedEditorFont = Globals.MainForm.EditorFont;
            SoundChipBox.SelectedIndex = (int)(AY.EmulatingChip) - 1;
            _savedChipType = AY.EmulatingChip;
            _savedStdChannelsAllocation = AY.StdChannelsAllocation;
            ChanMapBox.SelectedIndex = MainForm.ChanAllocIndex;
            HighlightSpeedPosition.Checked = MainForm.HighlightSpeedOn;
            _savedHighlightSpeedOn = MainForm.HighlightSpeedOn;
            _savedChanAllocIndex = MainForm.ChanAllocIndex;
            _savedPanoram[0] = MainForm.Panoram[0];
            _savedPanoram[1] = MainForm.Panoram[1];
            _savedPanoram[2] = MainForm.Panoram[2];
            APanTrackBar.Value = MainForm.Panoram[0];
            BPanTrackBar.Value = MainForm.Panoram[1];
            CPanTrackBar.Value = MainForm.Panoram[2];
            APanTextBox.Text = MainForm.Panoram[0].ToString();
            BPanTextBox.Text = MainForm.Panoram[1].ToString();
            CPanTextBox.Text = MainForm.Panoram[2].ToString();
            _chanAllocChanged = false;
            _panoramChanged = false;
            DisableHintsOpt.Checked = MainForm.DisableHints;
            _savedDisableHints = MainForm.DisableHints;
            _savedWinThemeIndex = Globals.MainForm.WinThemeIndex;
            WinColorsBox.SelectedIndex = Globals.MainForm.WinThemeIndex;
            _savedManualChipFreq = Main.ManualChipFreq;

            if (Main.ManualChipFreq > 0)
                ChipFreqTextBox.Text = Main.ManualChipFreq.ToString();

            _savedDefaultChipFreq = Main.DefaultChipFreq;

            switch (Main.DefaultChipFreq)
            {
                case 894887:
                    ChipFreqBox.SelectedIndex = 0;
                    break;
                case 831303:
                    ChipFreqBox.SelectedIndex = 1;
                    break;
                case 1773400:
                    ChipFreqBox.SelectedIndex = 2;
                    break;
                case 1750000:
                    ChipFreqBox.SelectedIndex = 3;
                    break;
                case 1000000:
                    ChipFreqBox.SelectedIndex = 4;
                    break;
                case 1500000:
                    ChipFreqBox.SelectedIndex = 5;
                    break;
                case 2000000:
                    ChipFreqBox.SelectedIndex = 6;
                    break;
                case 3500000:
                    ChipFreqBox.SelectedIndex = 7;
                    break;
                case 1520640:
                    ChipFreqBox.SelectedIndex = 8;
                    break;
                case 1611062:
                    ChipFreqBox.SelectedIndex = 9;
                    break;
                case 1706861:
                    ChipFreqBox.SelectedIndex = 10;
                    break;
                case 1808356:
                    ChipFreqBox.SelectedIndex = 11;
                    break;
                case 1915886:
                    ChipFreqBox.SelectedIndex = 12;
                    break;
                case 2029811:
                    ChipFreqBox.SelectedIndex = 13;
                    break;
                case 2150510:
                    ChipFreqBox.SelectedIndex = 14;
                    break;
                case 2278386:
                    ChipFreqBox.SelectedIndex = 15;
                    break;
                case 2413866:
                    ChipFreqBox.SelectedIndex = 16;
                    break;
                case 2557401:
                    ChipFreqBox.SelectedIndex = 17;
                    break;
                case 2709472:
                    ChipFreqBox.SelectedIndex = 18;
                    break;
                case 2870586:
                    ChipFreqBox.SelectedIndex = 19;
                    break;
                case 3041280:
                    ChipFreqBox.SelectedIndex = 20;
                    break;
                default:
                    // Form1.EdChipFrq.Text := IntToStr(AY_Freq);
                    ChipFreqBox.SelectedIndex = 21;
                    break;
            }

            _savedInterrupt_Freq = Main.DefaultIntFreq;

            switch (Main.DefaultIntFreq)
            {
                case 48828:
                    IntFreqBox.SelectedIndex = 0;
                    break;
                case 50000:
                    IntFreqBox.SelectedIndex = 1;
                    break;
                case 60000:
                    IntFreqBox.SelectedIndex = 2;
                    break;
                case 100000:
                    IntFreqBox.SelectedIndex = 3;
                    break;
                case 200000:
                    IntFreqBox.SelectedIndex = 4;
                    break;
                case 48000:
                    IntFreqBox.SelectedIndex = 5;
                    break;
                default:
                    double frequency = Main.DefaultIntFreq / 1000.0;
                    IntFrequencyTextBox.Text = Convert.ToString(frequency);
                    IntFreqBox.SelectedIndex = 6;
                    break;
            }

            _savedEngineIndex = AY.RenderEngine;
            SoundEngineBox.SelectedIndex = AY.RenderEngine;
            FeaturesLevelBox.SelectedIndex = VTModule.DetectFeaturesLevel ? (int)FeaturesLevel.AutoDetect : (int)VTModule.FeaturesLevel;
            _savedFeaturesLevel = VTModule.FeaturesLevel;
            _savedDetectFeaturesLevel = VTModule.DetectFeaturesLevel;
            if (VTModule.DetectModuleHeader)
                SaveHeadBox.SelectedIndex = 2;
            else if (VTModule.VortexModuleHeader)
                SaveHeadBox.SelectedIndex = 0;
            else
                SaveHeadBox.SelectedIndex = 1;
            _savedVortexModuleHeader = VTModule.VortexModuleHeader;
            _savedDetectModuleHeader = VTModule.DetectModuleHeader;

            switch (WaveOutAPI.SampleRate)
            {
                case 11025:
                    SampleRateBox.SelectedIndex = 0;
                    break;
                case 22050:
                    SampleRateBox.SelectedIndex = 1;
                    break;
                case 44100:
                    SampleRateBox.SelectedIndex = 2;
                    break;
                case 48000:
                    SampleRateBox.SelectedIndex = 3;
                    break;
                case 88200:
                    SampleRateBox.SelectedIndex = 4;
                    break;
                case 96000:
                    SampleRateBox.SelectedIndex = 5;
                    break;
                case 192000:
                    SampleRateBox.SelectedIndex = 6;
                    break;
            }

            _savedSampleRate = WaveOutAPI.SampleRate;

            if (AY.RenderEngine < 2 && WaveOutAPI.SampleBit > 16)
            {
                WaveOutAPI.SampleBit = 16;
                BitRateBox.SelectedIndex = 1;
            }

            _savedSampleBit = WaveOutAPI.SampleBit;

            switch (WaveOutAPI.SampleBit)
            {
                case 8:
                    BitRateBox.SelectedIndex = 0;
                    break;
                case 16:
                    BitRateBox.SelectedIndex = 1;
                    break;
                case 24:
                    BitRateBox.SelectedIndex = 2;
                    break;
                case 32:
                    BitRateBox.SelectedIndex = 3;
                    break;
            }

            ChannelsBox.SelectedIndex = (WaveOutAPI.NumberOfChannels == 2 ? 1 : 0);
            _savedNumberOfChannels = WaveOutAPI.NumberOfChannels;
            BufferLengthTrackBar.Value = WaveOutAPI.BufferLengthMs;
            _savedBufLen_ms = WaveOutAPI.BufferLengthMs;
            BufferCountTrackBar.Value = WaveOutAPI.BufferCount;
            _savedNumberOfBuffers = WaveOutAPI.BufferCount;
            if (WaveOutAPI.WODevice != null)
            {
                if (!WaveOutDeviceCombo.Visible)
                {
                    WaveOutGetDeviceListButton_Click(null, EventArgs.Empty);
                }
            }

            if (WaveOutDeviceCombo.Visible)
            {
                int index = 0;

                for (int i = 1; i < WaveOutDeviceCombo.Items.Count; i++)
                {
                    if (WaveOutDeviceCombo.Items[i].ToString() == WaveOutAPI.WODevice)
                    {
                        index = i;
                        break;
                    }
                }

                WaveOutDeviceCombo.SelectedIndex = index;
            }
            _savedWODevice = WaveOutAPI.WODevice;
            _savedIsFilt = AY.FilterEnabled;
            FilterCheckBox.Checked = AY.FilterEnabled;
            _savedFilt_M = AY.FilterLength;
            FilterNKTrackBar.Value = (int)Math.Round(Math.Log(AY.FilterLength) / Math.Log(2.0));
            _savedPrior = (int)MainForm.Priority;
            AppPriorityBox.SelectedIndex = (MainForm.Priority == ProcessPriorityClass.Normal ? 0 : 1);
            _savedDCType = AY.DCType;
            SetDCType();
            _savedDCCutOff = AY.DCCutOff;
            DCCutOffTrackBar.Value = AY.DCCutOff;
            MainForm.DisableUpdateChilds = false;
        }

        private void CenterForm()
        {
            Form parent = Owner;
            int x = parent.Left + (parent.Width - Width) / 2;
            int y = parent.Top + (parent.Height - Height) / 2;
            Location = new Point(x, y);
        }

        // Disable Tab key for the Options Window
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                // Tab key is disabled
                return true; // Indicate that the key has been processed.
            }
            return base.ProcessDialogKey(keyData);
        }

        public void StopAndStart()
        {
            if (WaveOutAPI.IsPlaying)
            {
                WaveOutAPI.ResetPlaying();
                ChildForm.PlayingWindow[1].RerollToLine(1);
                WaveOutAPI.UnResetPlaying();
            }
        }

        public int GetValue(string s)
        {
            s = s.Trim();
            if (!int.TryParse(s, out int result))
                return -1;
            return result;
        }

        public double GetValueF(string s)
        {
            s = s.Replace(',', '.');
            s = s.Trim();
            if (!double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                return -1;
            return result;
        }

        public void SoundChipBox_Click(object sender, EventArgs e)
        {
            if (AY.EmulatingChip == (ChipType)(SoundChipBox.SelectedIndex + 1))
                return;

            Globals.MainForm.SetEmulatingChip((ChipType)(SoundChipBox.SelectedIndex + 1));

            // Ayumi render
            if (AY.RenderEngine == 2 && AY.AyumiChip[1] != null && AY.AyumiChip[2] != null && AY.AyumiChip[3] != null)
            {
                for (int i = 1; i <= AY.ChipCount; i++)
                    AY.AyumiChip[i].SetChipType(AY.EmulatingChip == ChipType.YM);
            }
            else
            {
                StopAndStart();
            }
        }

        public void IntFreqBox_Click(object sender, EventArgs e)
        {
            int frequency;

            switch (IntFreqBox.SelectedIndex)
            {
                case 0:
                    Main.DefaultIntFreq = 48828;
                    break;
                case 1:
                    Main.DefaultIntFreq = 50000;
                    break;
                case 2:
                    Main.DefaultIntFreq = 60000;
                    break;
                case 3:
                    Main.DefaultIntFreq = 100000;
                    break;
                case 4:
                    Main.DefaultIntFreq = 200000;
                    break;
                case 5:
                    Main.DefaultIntFreq = 48000;
                    break;
                case 6:
                    frequency = GetValue(IntFrequencyTextBox.Text);

                    if (frequency < 0)
                        return;
                    Main.DefaultIntFreq = frequency * 1000;
                    break;
                default:
                    return;
            }

            if (Globals.MainForm.MdiChildren.Length == 0)
                return;

            _clicksCounter1++;

            if (_clicksCounter1 == 3)
            {
                _clicksCounter1 = 0;

                ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;

                activeForm.TabControl.SelectedIndex = 3;
                activeForm.ManualHz.Left = activeForm.ChipFreqBox.Buttons[20].Left + 95;
                activeForm.ManualIntFreq.Left = activeForm.IntFreqBox.Buttons[6].Left + 95;
                activeForm.HelpBox.Left = activeForm.IntFreqBox.Left - 4;
                activeForm.HelpBox.Top = activeForm.IntFreqBox.Top - 4;
                activeForm.HelpBox.Width = activeForm.IntFreqBox.Left + activeForm.IntFreqBox.Width;
                activeForm.HelpBox.Height = activeForm.IntFreqBox.Height + 8;
                activeForm.HelpBox.Visible = true;

                for (int i = 0; i < 16; i++)
                {
                    if (i % 2 == 0)
                    {
                        activeForm.HelpBox.BackColor = System.Drawing.SystemColors.ButtonFace;
                        activeForm.HelpBox.ForeColor = System.Drawing.SystemColors.ButtonFace;
                    }
                    else
                    {
                        activeForm.HelpBox.BackColor = System.Drawing.Color.Red;
                        activeForm.HelpBox.ForeColor = System.Drawing.Color.Red;
                    }

                    //activeForm.HelpShape1.Repaint();
                    //activeForm.HelpShape1.Refresh();
                    //Thread.CurrentThread.Sleep(80);
                    activeForm.HelpBox.Invalidate();
                    activeForm.HelpBox.Update();
                    Thread.Sleep(80);
                }

                activeForm.HelpBox.Visible = false;
            }
        }

        public void ChanMapBox_Click(object sender, EventArgs e)
        {
            if (MainForm.ChanAllocIndex == ChanMapBox.SelectedIndex)
                return;

            Globals.MainForm.RedrawOff();
            Globals.MainForm.SetChannelsAllocation(ChanMapBox.SelectedIndex);

            APanTrackBar.Value = MainForm.Panoram[0];
            BPanTrackBar.Value = MainForm.Panoram[1];
            CPanTrackBar.Value = MainForm.Panoram[2];

            Globals.MainForm.RedrawOn();

            if (AY.RenderEngine == 2)
                AY.UpdatePanoram();
            else if (WaveOutAPI.IsPlaying)
                ChildForm.PlayingWindow[1].StopAndRestart();
        }

        public void FeaturesLevelBox_Click(object sender, EventArgs e)
        {
            VTModule.FeaturesLevel = (FeaturesLevel)FeaturesLevelBox.SelectedIndex;
            VTModule.DetectFeaturesLevel = VTModule.FeaturesLevel == FeaturesLevel.AutoDetect;

            if (VTModule.DetectFeaturesLevel)
                VTModule.FeaturesLevel = FeaturesLevel.VTII_PT36;
        }

        public void BufferLengthTrackBar_ValueChanged(object sender, EventArgs e)
        {
            AY.SetBuffers(BufferLengthTrackBar.Value, WaveOutAPI.BufferCount);
            BufferLengthValue.Text = (WaveOutAPI.BufferLengthMs).ToString() + " ms";
            TotalLengthValue.Text = (WaveOutAPI.BufferLengthMs * WaveOutAPI.BufferCount).ToString() + " ms";
            HeardAfterMSLabel.Text = TotalLengthValue.Text;
        }

        public void BufferCountTrackBar_ValueChanged(object sender, EventArgs e)
        {
            AY.SetBuffers(WaveOutAPI.BufferLengthMs, BufferCountTrackBar.Value);
            BufferCountValue.Text = (WaveOutAPI.BufferCount).ToString();
            TotalLengthValue.Text = (WaveOutAPI.BufferLengthMs * WaveOutAPI.BufferCount).ToString() + " ms";
            HeardAfterMSLabel.Text = TotalLengthValue.Text;
        }

        public void SampleRateBox_Click(object sender, EventArgs e)
        {
            switch (SampleRateBox.SelectedIndex)
            {
                case 0:
                    AY.SetSampleRate(11025);
                    break;
                case 1:
                    AY.SetSampleRate(22050);
                    break;
                case 2:
                    AY.SetSampleRate(44100);
                    break;
                case 3:
                    AY.SetSampleRate(48000);
                    break;
                case 4:
                    AY.SetSampleRate(88200);
                    break;
                case 5:
                    AY.SetSampleRate(96000);
                    break;
                case 6:
                    if (AY.RenderEngine == 2)
                        AY.SetSampleRate(192000);
                    break;
            }
        }

        public void BitRateBox_Click(object sender, EventArgs e)
        {
            switch (BitRateBox.SelectedIndex)
            {
                case 0:
                    AY.SetBitRate(8);
                    break;
                case 1:
                    AY.SetBitRate(16);
                    break;
                case 2:
                    if (AY.RenderEngine == 2)
                        AY.SetBitRate(24);
                    break;
                case 3:
                    if (AY.RenderEngine == 2)
                        AY.SetBitRate(32);
                    break;
            }
        }

        public void ChannelsBox_Click(object sender, EventArgs e)
        {
            AY.SetNChans(ChannelsBox.SelectedIndex + 1);
        }

        public void SoundEngineBox_Click(object sender, EventArgs e)
        {
            if (SoundEngineBox.SelectedIndex == AY.RenderEngine)
                return;

            UpdateAudioSettings();
            AY.Set_Engine(SoundEngineBox.SelectedIndex);
        }

        public void ChipFreqBox_Click(object sender, EventArgs e)
        {
            int i;
            int frequency;

            switch (ChipFreqBox.SelectedIndex)
            {
                case 0:
                    frequency = 894887;
                    break;
                case 1:
                    frequency = 831303;
                    break;
                case 2:
                    frequency = 1773400;
                    break;
                case 3:
                    frequency = 1750000;
                    break;
                case 4:
                    frequency = 1000000;
                    break;
                case 5:
                    frequency = 1500000;
                    break;
                case 6:
                    frequency = 2000000;
                    break;
                case 7:
                    frequency = 3500000;
                    break;
                case 8:
                    frequency = 1520640;
                    break;
                case 9:
                    frequency = 1611062;
                    break;
                case 10:
                    frequency = 1706861;
                    break;
                case 11:
                    frequency = 1808356;
                    break;
                case 12:
                    frequency = 1915886;
                    break;
                case 13:
                    frequency = 2029811;
                    break;
                case 14:
                    frequency = 2150510;
                    break;
                case 15:
                    frequency = 2278386;
                    break;
                case 16:
                    frequency = 2413866;
                    break;
                case 17:
                    frequency = 2557401;
                    break;
                case 18:
                    frequency = 2709472;
                    break;
                case 19:
                    frequency = 2870586;
                    break;
                case 20:
                    frequency = 3041280;
                    break;
                case 21:
                    if (!ChipFreqTextBox.Focused && ChipFreqTextBox.CanFocus)
                    {
                        ChipFreqTextBox.SelectAll();
                        // EdChipFrq.SetFocus;
                    }

                    frequency = GetValue(ChipFreqTextBox.Text);

                    if (frequency < 0)
                        return;
                    break;
                default:
                    return;
            }

            Main.DefaultChipFreq = frequency;

            if (Globals.MainForm.MdiChildren.Length == 0)
                return;

            _clicksCounter++;

            if (_clicksCounter == 3)
            {
                _clicksCounter = 0;

                ChildForm activeForm = ((ChildForm)(Globals.MainForm.ActiveMdiChild));

                activeForm.TabControl.SelectedIndex = 3;
                activeForm.ManualHz.Left = activeForm.ChipFreqBox.Buttons[20].Left + 95;
                activeForm.ManualIntFreq.Left = activeForm.IntFreqBox.Buttons[6].Left + 95;
                activeForm.HelpBox.Left = activeForm.ChipFreqBox.Left - 4;
                activeForm.HelpBox.Top = activeForm.ChipFreqBox.Top - 4;
                activeForm.HelpBox.Width = activeForm.ChipFreqBox.Left + activeForm.ChipFreqBox.Width;
                activeForm.HelpBox.Height = activeForm.ChipFreqBox.Height + 8;
                activeForm.HelpBox.Visible = true;

                for (i = 0; i < 16; i++)
                {
                    if (i % 2 == 0)
                    {
                        activeForm.HelpBox.BackColor = System.Drawing.SystemColors.ButtonFace;
                        activeForm.HelpBox.ForeColor = System.Drawing.SystemColors.ButtonFace;
                    }
                    else
                    {
                        activeForm.HelpBox.BackColor = System.Drawing.Color.Red;
                        activeForm.HelpBox.ForeColor = System.Drawing.Color.Red;
                    }

                    //activeForm.HelpShape1.Repaint;
                    //activeForm.HelpShape1.Refresh();
                    //Thread.CurrentThread.Sleep(80);
                    activeForm.HelpBox.Invalidate();
                    activeForm.HelpBox.Update();
                    Thread.Sleep(80);
                }

                activeForm.HelpBox.Visible = false;
            }
        }

        public void WaveOutGetDeviceListButton_Click(object sender, EventArgs e)
        {
            WaveOutDeviceCombo.Visible = false;
            WaveOutDeviceCombo.Items.Clear();
            WaveOutDeviceCombo.Items.Add("Default Device");

            var devices = ALC.GetStringList(GetEnumerationStringList.DeviceSpecifier).ToList();
            WaveOutDeviceCombo.Items.AddRange(devices.ToArray());

            // Try to select previously used device if still present
            if (WaveOutAPI.WODevice != null && devices.Contains(WaveOutAPI.WODevice))
            {
                WaveOutDeviceCombo.SelectedIndex = devices.IndexOf(WaveOutAPI.WODevice) + 1;
            }
            else
            {
                WaveOutAPI.WODevice = null; // default
                WaveOutDeviceCombo.SelectedIndex = 0;
            }

            WaveOutDeviceCombo_SelectedIndexChanged(sender, e);
            WaveOutDeviceCombo.Visible = true;
        }

        public void WaveOutDeviceCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WaveOutDeviceCombo.SelectedIndex == 0)
            {
                // Default device
                WaveOutAPI.WODevice = null;
                WaveOutTextBox.Text = "Default Device";
            }
            else
            {
                WaveOutAPI.WODevice = WaveOutDeviceCombo.Items[WaveOutDeviceCombo.SelectedIndex].ToString();
                WaveOutTextBox.Text = WaveOutAPI.WODevice;
            }
        }

        public void SaveHeadBox_Click(object sender, EventArgs e)
        {
            VTModule.VortexModuleHeader = SaveHeadBox.SelectedIndex != 1;
            VTModule.DetectModuleHeader = SaveHeadBox.SelectedIndex == 2;
        }

        public void LockAudioOptions()
        {
            SampleRateBox.Enabled = false;
            BitRateBox.Enabled = false;
            ChannelsBox.Enabled = false;
            BuffersBox.Enabled = false;
            HeardAfterLabel.Visible = true;
            HeardAfterMSLabel.Visible = true;
            WaveOutBox.Enabled = false;
        }

        public void UnlockAudioOptions()
        {
            SampleRateBox.Enabled = true;
            BitRateBox.Enabled = true;
            ChannelsBox.Enabled = true;
            BuffersBox.Enabled = true;
            HeardAfterLabel.Visible = false;
            HeardAfterMSLabel.Visible = false;
            WaveOutBox.Enabled = true;
        }

        public void SetDCType()
        {
            DCOff.Checked = false;
            DCAyumi.Checked = false;
            DCWbcbz7.Checked = false;

            switch (AY.DCType)
            {
                case 0:
                    DCOff.Checked = true;
                    break;
                case 1:
                    DCAyumi.Checked = true;
                    break;
                case 2:
                    DCWbcbz7.Checked = true;
                    break;
            }
        }

        public void UpdateAudioSettings()
        {
            FilterCheckBox.Enabled = SoundEngineBox.SelectedIndex == 0;
            FilterNKTrackBar.Enabled = SoundEngineBox.SelectedIndex == 0;

            if (WaveOutAPI.IsPlaying)
            {
                LockAudioOptions();
            }
            else
            {
                UnlockAudioOptions();
                BitRateBox.Buttons[0].Enabled = SoundEngineBox.SelectedIndex != 2;
                BitRateBox.Buttons[2].Enabled = SoundEngineBox.SelectedIndex == 2;
                BitRateBox.Buttons[3].Enabled = SoundEngineBox.SelectedIndex == 2;
                SampleRateBox.Buttons[6].Enabled = SoundEngineBox.SelectedIndex == 2;
            }

            SetDCType();

            // Ayumi Engine
            if (SoundEngineBox.SelectedIndex == 2)
            {
                AyumiDCFiltBox.Visible = true;
                DownsamplingBox.Visible = false;
                DCCutOffLab.Visible = DCWbcbz7.Checked;
                DCCutOffTrackBar.Visible = DCWbcbz7.Checked;

                if (WaveOutAPI.SampleBit == 8)
                {
                    AY.SetBitRate(16);
                    BitRateBox.SelectedIndex = 1;
                }
            }

            // VT Engine
            if (SoundEngineBox.SelectedIndex < 2)
            {
                AyumiDCFiltBox.Visible = false;
                DownsamplingBox.Visible = true;

                if (WaveOutAPI.SampleRate > 96000)
                {
                    AY.SetSampleRate(96000);
                    SampleRateBox.SelectedIndex = 5;
                }

                if (WaveOutAPI.SampleBit > 16)
                {
                    AY.SetBitRate(16);
                    BitRateBox.SelectedIndex = 1;
                }
            }
        }

        public void FiltChk_Click(object sender, EventArgs e)
        {
            AY.SetFilter(FilterCheckBox.Checked, AY.FilterLength);
        }

        public void FiltNK_ValueChanged(object sender, EventArgs e)
        {
            AY.SetFilter(AY.FilterEnabled, (int)Math.Round(Math.Exp(FilterNKTrackBar.Value * Math.Log(2.0))));
        }

        public void AppPriorityBox_Click(object sender, EventArgs e)
        {
            Globals.MainForm.SetPriority(AppPriorityBox.SelectedIndex == 0 ? ProcessPriorityClass.Normal : ProcessPriorityClass.High);
        }

        public void OnColorChange(object sender)
        {
            ColorDialog colorDialog = (ColorDialog)sender;
            _curColorPanel.BackColor = colorDialog.Color;
            Globals.MainForm.PrepareColors();
            RepaintChilds(false);
        }

        public void SelectColor(Panel colorBox, ref Color colorBar, int tab)
        {
            Globals.MainForm.SetChildsTab(tab);

            _curColorPanel = colorBox;

            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.FullOpen = true;
                colorDialog.Color = colorBox.BackColor;

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    OnColorChange(colorDialog);
                    colorBox.BackColor = colorDialog.Color;
                    colorBar = colorDialog.Color;
                    ColorThemes.UpdateCurrentTheme();
                }
            }
        }

        public void ColorThemesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ColorTheme theme = ColorThemes.GetCurrentColorTheme();

            if (MainForm.ColorThemeName == theme.Name)
                return;

            MainForm.ChildsEventsBlocked = true;
            Globals.MainForm.RedrawOff();

            for (int i = 0; i < ColorThemesList.Items.Count; i++)
            {
                if (ColorThemesList.GetSelected(i))
                {
                    ColorThemes.SetColorTheme(ColorThemes.VTColorThemes[i]);
                    ColorThemes.SetupColorBars(theme);
                    break;
                }
            }

            Globals.MainForm.RedrawOn();
            MainForm.ChildsEventsBlocked = false;
        }

        public void DecNumbersLines_Click(object sender, EventArgs e)
        {
            MainForm.DecBaseLinesOn = DecNumbersLines.Checked;

            if (DecNumbersLines.Checked)
            {
                MainForm.TracksCursorXLeft = 4;
                MainForm.OrnXShift = 1;
                MainForm.OrnCharCount = 10;
            }
            else
            {
                MainForm.TracksCursorXLeft = 3;
                MainForm.OrnXShift = 0;
                MainForm.OrnCharCount = 9;
            }

            if (!MainForm.DisableUpdateChilds)
                RepaintChilds(true);
        }

        public void DecNumbersNoise_Click(object sender, EventArgs e)
        {
            MainForm.DecBaseNoiseOn = DecNumbersNoise.Checked;

            if (!MainForm.DisableUpdateChilds)
                RepaintChilds(false);
        }

        public void HighlightSpeedPosition_Click(object sender, EventArgs e)
        {
            MainForm.HighlightSpeedOn = HighlightSpeedPosition.Checked;

            if (!MainForm.DisableUpdateChilds)
                RepaintChilds(false);
        }

        public void MIDINextDeviceButton_Click(object sender, EventArgs e)
        {
            if (Globals.MainForm.MidiInputDeviceInfo == null)
                return;

            var inputDevices = MidiManager.GetAvailableDevices();
            var midiPort = Globals.MainForm.MidiInputDeviceInfo.Port;

            if (midiPort < inputDevices.Count - 1)
            {
                try
                {
                    Globals.MainForm.DestroyMidiInputClient();
                    Globals.MainForm.CreateMidiInputClient(midiPort + 1);
                    MIDIDeviceName.Text = Globals.MainForm.MidiInputDeviceInfo.Name;
                }
                catch
                {
                    Globals.MainForm.DestroyMidiInputClient();
                    MIDIDeviceName.Text = "(None)";
                }
            }
        }

        public void MIDIPrevDeviceButton_Click(object sender, EventArgs e)
        {
            if (Globals.MainForm.MidiInputDeviceInfo == null)
                return;

            var midiPort = Globals.MainForm.MidiInputDeviceInfo.Port;

            if (midiPort > 0)
            {
                try
                {
                    Globals.MainForm.DestroyMidiInputClient();
                    Globals.MainForm.CreateMidiInputClient(midiPort - 1);
                    MIDIDeviceName.Text = Globals.MainForm.MidiInputDeviceInfo.Name;
                }
                catch
                {
                    Globals.MainForm.DestroyMidiInputClient();
                    MIDIDeviceName.Text = "(None)";
                }
            }
        }

        public void MIDIStopButton_Click(object sender, EventArgs e)
        {
        }

        public void HotKeyList_KeyDown(object sender, KeyEventArgs e)
        {
            string shortCutText;
            // If hotkey is not selected - exit
            if (HotKeyList.SelectedItems.Count == 0)
                return;

            // If user pressed only Ctrl, Alt, Shift without key - exit
            if (e.KeyValue >= 16 && e.KeyValue <= 18)
                return;

            // If user pressed up/down keys - exit
            if (e.KeyValue == 38 || e.KeyValue == 40)
                return;

            // If user press left/right without Ctrl or Alt
            if ((e.KeyValue == 37 || e.KeyValue == 39) && ((e.Modifiers & Keys.Alt) != Keys.Alt) && ((e.Modifiers & Keys.Control) != Keys.Control))
                return;

            // Get text of pressed shortcut
            shortCutText = VortexTracker.HotKeys.ShortcutToText((Shortcut)(e.KeyCode | Keys.Shift));

            // If user pressed only A..Z - 0..9 - exit
            if (shortCutText.Length == 1)
            {
                MessageBox.Show(this, "Can\'t assing the \"" + e.KeyCode.ToString() + "\" key.");
                return;
            }

            ListViewItem listViewItem = HotKeyList.SelectedItems[0];
            HotKey hotKey = (HotKey)listViewItem.Tag;

            VortexTracker.HotKeys.ReAssignHotKey(hotKey, shortCutText);
            //Shift = new object[] {};
            e.SuppressKeyPress = true;
        }

        public void HotKeyList_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = '\0';
        }

        public void DisablePatSeparators_Click(object sender, EventArgs e)
        {
            if (MainForm.DisableSeparators == DisablePatSeparators.Checked)
                return;

            MainForm.DisableSeparators = DisablePatSeparators.Checked;

            if (!MainForm.DisableUpdateChilds)
                RepaintChilds(true);
        }

        public void AutoSaveBackups_MouseUp(object sender, MouseEventArgs e)
        {
            BackupEveryMins.Enabled = AutoSaveBackups.Checked;
            MainForm.AutoBackupsOn = AutoSaveBackups.Checked;
            //MainForm.AutoBackupsMins = BackupEveryMins.Position;
            Globals.MainForm.ChangeBackupTimer();
        }

        public void DefaultFreqTableUpDown_ValueChanged(object sender, EventArgs e)
        {
            //Globals.MainForm.DefaultTable = UpDown2.Value;
            //TableName.Text = TVTM.TableNames[UpDown2.Value];
        }

        public void BackupEveryMins_ValueChanged(object sender, EventArgs e)
        {
            //if (MainForm.AutoBackupsMins == BackupEveryMins.Position)
            //{
            //    return;
            //}
            //MainForm.AutoBackupsMins = BackupEveryMins.Position;
            Globals.MainForm.ChangeBackupTimer();
        }

        public ListViewItem GetHotKeyListItem(HotKey hotKey)
        {
            foreach (ListViewItem listViewItem in HotKeyList.Items)
            {
                if (hotKey == listViewItem.Tag)
                    return listViewItem;
            }

            return null;
        }

        public void InitFileAssociations()
        {
            FileAssocList.Items.Clear();

            for (int i = 0; i < MainForm.FileAssociations.GetLength(0); i++)
            {
                ListViewItem listItem = new ListViewItem();

                listItem.Checked = MainForm.FileAssociations[i, 0] == "1";
                listItem.Text = MainForm.FileAssociations[i, 1];
                listItem.SubItems.Add(MainForm.FileAssociations[i, 3]);

                FileAssocList.Items.Add(listItem);
            }
        }

        public void ApplyFileAssociations()
        {
            for (int i = 0; i <= FileAssocList.Items.Count - 1; i++)
            {
                if (FileAssocList.Items[i].Checked)
                {
                    //FileAssociations[i, 0] = '1';
                }
                else
                {
                    //MainForm.FileAssociations[i, 0] = '0';
                }
            }
            Globals.MainForm.SetFileAssociations();
        }

        /* public int InitFonts_EnumFontsProc(ref TEnumLogFont elf, ref TNewTextMetric tm, int FontType, IntPtr Data)
        {
            int result;
             result = ((int)Win32.FIXED_PITCH == (elf.elfLogFont.lfPitchAndFamily && Win32.FIXED_PITCH));
            return result;
        } */

        public void InitFonts()
        {
            string[] fontsBlackList = { "Courier", "Default", "Fixedsys", "Terminal", "Arrows" };
            bool fontValid;
            string fontName;
            FontsList.Items.Clear();

            // Add internal fonts
            foreach (InternalFont font in MainForm.InternalFonts)
            {
                if (font.Name == "Arrows" || font.Style == FontStyle.Bold)
                    continue;

                FontsList.Items.Add(font.Name);
            }

            /* if (MainForm.TryGetAllResourceNames("Fonts", "*.ttf", out string[] resourceNames))
            {
                foreach (string resourceName in resourceNames)
                {
                    MainForm.TryGetResourceFileName(resourceName, out string fileName);

                    fontName = Path.GetFileNameWithoutExtension(fileName);

                    FontsList.Items.Add(fontName);
                }
            } */

            /* for (i = 0; i < MainForm.InternalFonts.GetLength(0); i++)
            {
                fontName = MainForm.InternalFonts[i, 1];
                if ((FontsList.Items.IndexOf(fontName) == -1) && (Array.IndexOf(fontsBlackList, fontName) == -1) && Globals.MainForm.IsFontValid(fontName))
                {
                    FontsList.Items.Add(fontName);
                }
            } */

            // No installed fonts
            /* using (InstalledFontCollection installedFontCollection = new InstalledFontCollection())
            {
                // Get only monospaced fonts
                foreach (FontFamily fontFamily in installedFontCollection.Families)
                {
                    fontName = fontFamily.Name;
                    fontValid = (Array.IndexOf(fontsBlackList, fontName) == -1) && (FontsList.Items.IndexOf(fontName) == -1); // && Globals.MainForm.IsFontValid(fontName);
                    
                    if (fontValid)
                        FontsList.Items.Add(fontName);
                }
            } */

            FontBoldButton.Checked = (Globals.MainForm.EditorFont.Style == FontStyle.Bold);
            FontSizeUpDown.Value = (int)Globals.MainForm.EditorFont.Size;

            for (int i = 0; i < FontsList.Items.Count; i++)
            {
                if (FontsList.Items[i].ToString() != Globals.MainForm.EditorFont.Name)
                    continue;

                FontsList.SetSelected(i, true);

                break;
            }
        }

        public void ChangeFont()
        {
            if (FontsList.SelectedIndex == -1)
                return;

            string fontName = FontsList.SelectedItem.ToString();
            bool cantChange = (Globals.MainForm.EditorFont.Name == fontName) && ((int)Globals.MainForm.EditorFont.Size == FontSizeUpDown.Value) && ((FontBoldButton.Checked && (Globals.MainForm.EditorFont.Style == FontStyle.Bold) || (!FontBoldButton.Checked && (Globals.MainForm.EditorFont.Style == FontStyle.Regular))));

            if (cantChange)
                return;

            if (!MainForm.TryGetFontFamily(fontName, out FontFamily fontFamily))
                return;

            Globals.MainForm.EditorFont = new Font(fontFamily, (float)FontSizeUpDown.Value, FontBoldButton.Checked ? FontStyle.Bold : FontStyle.Regular);
            MainForm.EditorFontChanged = true;

            if (MainForm.DisableUpdateChilds)
                return;

            RepaintChilds(true);
        }

        public void FontsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeFont();
        }

        public void FontSizeUpDown_ValueChanged(object sender, EventArgs e)
        {
            int newValue = (int)FontSizeUpDown.Value;

            if (newValue == -1 || newValue < 12 || newValue > 30)
                return;

            ChangeFont();
        }

        public void FontBoldButton_Click(object sender, EventArgs e)
        {
            ChangeFont();
        }

        public string CurrentThemeName()
        {
            string result = "VT Theme";

            for (int i = 0; i < ColorThemesList.Items.Count; i++)
            {
                if (ColorThemesList.GetSelected(i))
                    return ColorThemesList.Items[i].ToString();
            }

            return result;
        }

        public void SaveThemeButton_Click(object sender, EventArgs e)
        {
            // repeat
            // NewName := InputBox('Vortex Tracker II', 'Enter theme name', NewName) then
            // until ValidColorThemeName(NewName);
            SaveThemeDialog.Title = "Save Color Theme";
            SaveThemeDialog.DefaultExt = "vtt";
            SaveThemeDialog.FileName = ColorThemes.SelectedThemeName();

            if (SaveThemeDialog.ShowDialog(Globals.MainForm) == DialogResult.OK)
            {
                SaveThemeDialog.InitialDirectory = Path.GetDirectoryName(SaveThemeDialog.FileName);
                ColorThemes.SaveColorTheme(SaveThemeDialog.FileName, ColorThemes.SelectedThemeName());
            }
        }

        public void LoadThemeButton_Click(object sender, EventArgs e)
        {
            LoadThemeDialog.Title = "Load Color Theme";
            LoadThemeDialog.DefaultExt = "vtt";

            if (LoadThemeDialog.ShowDialog(Globals.MainForm) == DialogResult.OK)
            {
                LoadThemeDialog.InitialDirectory = Path.GetDirectoryName(LoadThemeDialog.FileName);
                ColorThemes.LoadColorTheme(LoadThemeDialog.FileName);
            }
        }

        public void RenameThemeButton_Click(object sender, EventArgs e)
        {
            ColorThemes.RenameSelectedTheme();
        }

        public void DeleteThemeButton_Click(object sender, EventArgs e)
        {
            ColorThemes.DeleteSelectedTheme();
        }

        public void CopyThemeButton_Click(object sender, EventArgs e)
        {
            ColorThemes.CloneColorTheme();
        }

        public void RepaintChilds(bool windowSizeChanged)
        {
            Rectangle newSize;
            MainForm mainForm = Globals.MainForm;

            mainForm.RedrawOff();

            MainForm.ChildsEventsBlocked = true;

            if (windowSizeChanged)
            {
                mainForm.RedrawChilds();
                mainForm.AutoMetricsForChilds(mainForm.WindowState);
                mainForm.SetChildsPosition(mainForm.WindowState);
                newSize = mainForm.GetSizeForChilds(mainForm.WindowState, false);
                mainForm.AutoCutChilds(newSize);
                mainForm.SetWindowSize(newSize);
                mainForm.AutoToolBarPosition(newSize);
            }
            else
            {
                mainForm.RedrawChilds();
            }

            MainForm.ChildsEventsBlocked = false;
            mainForm.NumberOfLinesChanged = false;
            MainForm.EditorFontChanged = false;
            mainForm.RedrawOn();
        }

        public void ColBackground_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColBackground, ref MainForm.ColorTheme.Colors[(int)ThemeColor.Background], 1);
        }

        public void ColSelLineBackground_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSelLineBackground, ref MainForm.ColorTheme.Colors[(int)ThemeColor.SelLineBackground], 1);
        }

        public void ColHighlBackground_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColHighlBackground, ref MainForm.ColorTheme.Colors[(int)ThemeColor.HighlBackground], 1);
        }

        public void ColOutBackground_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColOutBackground, ref MainForm.ColorTheme.Colors[(int)ThemeColor.OutBackground], 1);
        }

        public void ColOutHlBackground_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColOutHlBackground, ref MainForm.ColorTheme.Colors[(int)ThemeColor.OutHlBackground], 1);
        }

        public void ColText_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColText, ref MainForm.ColorTheme.Colors[(int)ThemeColor.Text], 1);
        }

        public void ColHighlText_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColHighlText, ref MainForm.ColorTheme.Colors[(int)ThemeColor.HighlText], 1);
        }

        public void ColOutText_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColOutText, ref MainForm.ColorTheme.Colors[(int)ThemeColor.OutText], 1);
        }

        public void ColLineNum_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColLineNum, ref MainForm.ColorTheme.Colors[(int)ThemeColor.LineNum], 1);
        }

        public void ColEnvelope_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColEnvelope, ref MainForm.ColorTheme.Colors[(int)ThemeColor.Envelope], 1);
        }

        public void ColNoise_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColNoise, ref MainForm.ColorTheme.Colors[(int)ThemeColor.Noise], 1);
        }

        public void ColNote_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColNote, ref MainForm.ColorTheme.Colors[(int)ThemeColor.Note], 1);
        }

        public void ColNoteParams_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColNoteParams, ref MainForm.ColorTheme.Colors[(int)ThemeColor.NoteParams], 1);
        }

        public void ColNoteCommands_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColNoteCommands, ref MainForm.ColorTheme.Colors[(int)ThemeColor.NoteCommands], 1);
        }

        public void ColSeparators_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSeparators, ref MainForm.ColorTheme.Colors[(int)ThemeColor.Separators], 1);
        }

        public void ColOutSeparators_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColOutSeparators, ref MainForm.ColorTheme.Colors[(int)ThemeColor.OutSeparators], 1);
        }

        public void ColSelLineText_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSelLineText, ref MainForm.ColorTheme.Colors[(int)ThemeColor.SelLineText], 1);
        }

        public void ColSelLineNum_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSelLineNum, ref MainForm.ColorTheme.Colors[(int)ThemeColor.SelLineNum], 1);
        }

        public void ColHighlLineNum_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColHighlLineNum, ref MainForm.ColorTheme.Colors[(int)ThemeColor.HighlLineNum], 1);
        }

        public void ColSelEnvelope_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSelEnvelope, ref MainForm.ColorTheme.Colors[(int)ThemeColor.SelEnvelope], 1);
        }

        public void ColSelNoise_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSelNoise, ref MainForm.ColorTheme.Colors[(int)ThemeColor.SelNoise], 1);
        }

        public void ColSelNote_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSelNote, ref MainForm.ColorTheme.Colors[(int)ThemeColor.SelNote], 1);
        }

        public void ColSelNoteParams_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSelNoteParams, ref MainForm.ColorTheme.Colors[(int)ThemeColor.SelNoteParams], 1);
        }

        public void ColSelNoteCommands_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSelNoteCommands, ref MainForm.ColorTheme.Colors[(int)ThemeColor.SelNoteCommands], 1);
        }

        public void ColSamOrnBackground_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSamOrnBackground, ref MainForm.ColorTheme.Colors[(int)ThemeColor.SamOrnBackground], 2);
        }

        public void ColSamOrnText_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSamOrnText, ref MainForm.ColorTheme.Colors[(int)ThemeColor.SamOrnText], 2);
        }

        public void ColSamOrnLineNum_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSamOrnLineNum, ref MainForm.ColorTheme.Colors[(int)ThemeColor.SamOrnLineNum], 2);
        }

        public void ColSamNoise_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSamNoise, ref MainForm.ColorTheme.Colors[(int)ThemeColor.SamNoise], 2);
        }

        public void ColSamOrnSeparators_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSamOrnSeparators, ref MainForm.ColorTheme.Colors[(int)ThemeColor.SamOrnSeparators], 2);
        }

        public void ColSamOrnTone_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSamOrnTone, ref MainForm.ColorTheme.Colors[(int)ThemeColor.SamOrnTone], 2);
        }

        public void ColSamOrnSelBackground_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSamOrnSelBackground, ref MainForm.ColorTheme.Colors[(int)ThemeColor.SamOrnSelBackground], 2);
        }

        public void ColSamOrnSelText_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSamOrnSelText, ref MainForm.ColorTheme.Colors[(int)ThemeColor.SamOrnSelText], 2);
        }

        public void ColSamOrnSelLineNum_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSamOrnSelLineNum, ref MainForm.ColorTheme.Colors[(int)ThemeColor.SamOrnSelLineNum], 2);
        }

        public void ColSamSelNoise_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSamSelNoise, ref MainForm.ColorTheme.Colors[(int)ThemeColor.SamSelNoise], 2);
        }

        public void ColSamOrnSelTone_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColSamOrnSelTone, ref MainForm.ColorTheme.Colors[(int)ThemeColor.SamOrnSelTone], 2);
        }

        public void ColFullScreenBackground_MouseDown(object sender, MouseEventArgs e)
        {
            SelectColor(ColFullScreenBackground, ref MainForm.ColorTheme.Colors[(int)ThemeColor.FullScreenBackground], 1);
        }

        public void APan_ValueChanged(object sender, EventArgs e)
        {
            APanTextBox.Text = (APanTrackBar.Value).ToString();
            if (MainForm.Panoram[0] == APanTrackBar.Value)
            {
                return;
            }
            AY.IndexA_L = (byte)(255 - APanTrackBar.Value);
            AY.IndexA_R = (byte)APanTrackBar.Value;
            MainForm.Panoram[0] = (byte)APanTrackBar.Value;
            if (AY.RenderEngine == 2)
            {
                AY.UpdatePanoram();
            }
        }

        public void BPan_ValueChanged(object sender, EventArgs e)
        {
            BPanTextBox.Text = (BPanTrackBar.Value).ToString();
            if (MainForm.Panoram[1] == BPanTrackBar.Value)
            {
                return;
            }
            AY.IndexB_L = (byte)(255 - BPanTrackBar.Value);
            AY.IndexB_R = (byte)BPanTrackBar.Value;
            MainForm.Panoram[1] = (byte)BPanTrackBar.Value;
            if (AY.RenderEngine == 2)
            {
                AY.UpdatePanoram();
            }
        }

        public void CPan_ValueChanged(object sender, EventArgs e)
        {
            CPanTextBox.Text = (CPanTrackBar.Value).ToString();
            if (MainForm.Panoram[2] == CPanTrackBar.Value)
            {
                return;
            }
            AY.IndexC_L = (byte)(255 - CPanTrackBar.Value);
            AY.IndexC_R = (byte)CPanTrackBar.Value;
            MainForm.Panoram[2] = (byte)CPanTrackBar.Value;
            if (AY.RenderEngine == 2)
            {
                AY.UpdatePanoram();
            }
        }

        public void APanTrackBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (AY.RenderEngine != 2)
            {
                AY.UpdatePanoram();
            }
        }

        public void BPanTrackBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (AY.RenderEngine != 2)
            {
                AY.UpdatePanoram();
            }
        }

        public void CPanTrackBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (AY.RenderEngine != 2)
            {
                AY.UpdatePanoram();
            }
        }

        public void DisableHintsOpt_Click(object sender, EventArgs e)
        {
            MainForm.DisableHints = DisableHintsOpt.Checked;
        }

        public void StartsAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.MainForm.StartupAction = (byte)StartsAction.SelectedIndex;
        }

        public void BrowseTemplate_Click(object sender, EventArgs e)
        {
            TemplateDialog.Title = "Startup Template Song";
            TemplateDialog.DefaultExt = "vt2";
            if (Directory.Exists(Path.GetDirectoryName(TemplateSong.Text)))
            {
                TemplateDialog.InitialDirectory = Path.GetDirectoryName(TemplateSong.Text);
            }
            if (TemplateDialog.ShowDialog(Globals.MainForm) == DialogResult.OK)
            {
                TemplateDialog.InitialDirectory = Path.GetDirectoryName(TemplateDialog.FileName);
                TemplateSong.Text = TemplateDialog.FileName;
                Globals.MainForm.TemplateSongPath = TemplateDialog.FileName;
            }
        }

        public void WinColorsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.MainForm.WinThemeIndex = WinColorsBox.SelectedIndex;
            ColorThemes.SetWindowColors(WinColorsBox.SelectedIndex);
            Globals.MainForm.VolumeTrackBar.Enabled = false;
            Globals.MainForm.VolumeTrackBar.Enabled = true;
        }

        public void AllFileAssoc_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= FileAssocList.Items.Count - 1; i++)
            {
                FileAssocList.Items[i].Checked = true;
            }
        }

        public void NoneFileAssoc_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= FileAssocList.Items.Count - 1; i++)
            {
                FileAssocList.Items[i].Checked = false;
            }
        }

        public void DisableCtrlClickOpt_Click(object sender, EventArgs e)
        {
            MainForm.DisableCtrlClick = DisableCtrlClickOpt.Checked;
        }

        public void FileAssocList_Click(object sender, EventArgs e)
        {
            MainForm.FileAssocChanged = true;
        }

        public void APanTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            int i;
            i = GetValue(APanTextBox.Text);
            if (i < 0)
            {
                return;
            }
            APanTrackBar.Value = i;
        }

        public void BPanTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            int i;
            i = GetValue(BPanTextBox.Text);
            if (i < 0)
            {
                return;
            }
            BPanTrackBar.Value = i;
        }

        public void CPanTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            int i;
            i = GetValue(CPanTextBox.Text);
            if (i < 0)
            {
                return;
            }
            CPanTrackBar.Value = i;
        }

        public void EdIntFrqKeyUp(object sender, KeyEventArgs e)
        {
            string prevValue = "";
            double newValue;
            double f;
            bool wrong;
            if (IntFreqBox.SelectedIndex != 6)
            {
                IntFreqBox.SelectedIndex = 6;
            }
            newValue = GetValueF(IntFrequencyTextBox.Text);
            if (newValue < 0)
            {
                return;
            }
            if ((prevValue == "") && (IntFrequencyTextBox.Text.Length > 1))
            {
                f = Main.DefaultIntFreq / 1000;
                prevValue = Convert.ToString(f);
            }
            f = Helpers.Frac(Convert.ToSingle(IntFrequencyTextBox.Text));
            wrong = Convert.ToString(f).Length > 5;
            newValue = newValue * 1000;
            wrong = wrong || ((newValue < 1000) || (newValue > 2000000));
            if (wrong)
            {
                IntFrequencyTextBox.Text = prevValue;
                IntFrequencyTextBox.SelectionStart = prevValue.Length;
                return;
            }
            prevValue = IntFrequencyTextBox.Text;
            Main.DefaultIntFreq = (int)Math.Round(newValue);
        }

        public void EdIntFrqKeyPress(object sender, KeyPressEventArgs e)
        {
            bool wrong;
            wrong = !(e.KeyChar == '0' || e.KeyChar == '.' || e.KeyChar == ',') && (e.KeyChar != 0x08);
            wrong = wrong || ((IntFrequencyTextBox.Text.IndexOf(',') != -1) && (e.KeyChar == '.' || e.KeyChar == ','));
            wrong = wrong || ((IntFrequencyTextBox.Text == "") && (e.KeyChar == '.' || e.KeyChar == ','));
            if (wrong)
            {
                e.KeyChar = '\0';
                return;
            }
            if (e.KeyChar == '.')
            {
                e.KeyChar = ',';
                return;
            }
        }

        public void EdChipFrqKeyUp(object sender, KeyEventArgs e)
        {
            if (ChipFreqBox.SelectedIndex != 21)
                ChipFreqBox.SelectedIndex = 21;

            int newValue = GetValue(ChipFreqTextBox.Text);

            if (newValue < 0 || newValue < 700000 || newValue > 3546800)
                return;

            Main.ManualChipFreq = newValue;
        }

        public void EdChipFrqKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar >= '0' && e.KeyChar <= '9') && (e.KeyChar != (char)0x08))
            {
                e.KeyChar = '\0';
                return;
            }
        }

        public void FormKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Escape) && (OptionsTabControl.SelectedTab != HotKeys))
            {
                CancelButton.PerformClick();
            }
        }

        public void DCOffMouseUp(object sender, MouseEventArgs e)
        {
            AY.DCType = 0;
            UpdateAudioSettings();

            for (int i = 1; i <= AY.ChipCount; i++)
            {
                if (AY.AyumiChip[i] == null)
                    continue;

                AY.AyumiChip[i].SetDCType(AY.DCType);
            }
        }

        public void DCAyumiMouseUp(object sender, MouseEventArgs e)
        {
            AY.DCType = 1;
            UpdateAudioSettings();

            for (int i = 1; i <= AY.ChipCount; i++)
            {
                if (AY.AyumiChip[i] == null)
                    continue;

                AY.AyumiChip[i].SetDCType(AY.DCType);
            }
        }

        public void DCWbcbz7MouseUp(object sender, MouseEventArgs e)
        {
            AY.DCType = 2;
            UpdateAudioSettings();

            for (int i = 1; i <= AY.ChipCount; i++)
            {
                if (AY.AyumiChip[i] == null)
                    continue;

                AY.AyumiChip[i].SetDCType(AY.DCType);
            }
        }

        public void DCCutOffBarChange(object sender)
        {
            AY.DCCutOff = DCCutOffTrackBar.Value;

            for (int i = 1; i <= AY.ChipCount; i++)
            {
                if (AY.AyumiChip[i] == null)
                    continue;

                AY.AyumiChip[i].SetDCCutoff(AY.DCCutOff);
            }
        }

        public void DisableInfoWinOpt_MouseUp(object sender, MouseEventArgs e)
        {
            MainForm.DisableInfoWin = DisableInfoWinOpt.Checked;
        }

        public void DecPositionsSize_Click(object sender, EventArgs e)
        {
            if (MainForm.PositionSize == 0)
                return;

            Globals.MainForm.SetChildsTab(1);
            MainForm.EditorFontChanged = true;
            MainForm.PositionSize--;
            RepaintChilds(true);
        }

        public void IncPositionsSize_Click(object sender, EventArgs e)
        {
            if (MainForm.PositionSize == 5)
                return;

            Globals.MainForm.SetChildsTab(1);
            MainForm.EditorFontChanged = true;
            MainForm.PositionSize++;
            RepaintChilds(true);
        }

        private void OptionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;

            Owner?.Activate();

            this.Hide();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Globals.OptionsForm.ApplyFileAssociations();
            Globals.MainForm.WriteConfig();

            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Globals.OptionsForm.InitFileAssociations();
            Globals.MainForm.SetFileAssociations();
            Main.ManualChipFreq = _savedManualChipFreq;
            MainForm.DisableCtrlClick = _savedDisableCtrlClick;
            MainForm.DisableInfoWin = _savedDisableInfoWin;
            MainForm.PositionSize = _savedPositionSize;

            if (_savedStartupAction != Globals.MainForm.StartupAction)
                Globals.MainForm.StartupAction = _savedStartupAction;

            if (_savedTemplateSongPath != Globals.MainForm.TemplateSongPath)
                Globals.MainForm.TemplateSongPath = _savedTemplateSongPath;

            MainForm.TracksCursorXLeft = _savedDecBaseLinesOn ? 4 : 3;

            if (_savedEditorFont != Globals.MainForm.EditorFont)
                MainForm.EditorFontChanged = true;

            if (_savedWinThemeIndex != Globals.MainForm.WinThemeIndex)
            {
                Globals.MainForm.WinThemeIndex = _savedWinThemeIndex;
                ColorThemes.SetWindowColors(Globals.MainForm.WinThemeIndex);
                Globals.MainForm.VolumeTrackBar.Enabled = false;
                Globals.MainForm.VolumeTrackBar.Enabled = true;
            }

            MainForm.DecBaseLinesOn = _savedDecBaseLinesOn;
            MainForm.DecBaseNoiseOn = _savedDecBaseNoiseOn;
            MainForm.EnvelopeAsNote = _savedEnvelopeAsNote;
            MainForm.HighlightSpeedOn = _savedHighlightSpeedOn;
            MainForm.DupNoteParams = _savedDupNoteParams;
            MainForm.MoveBetweenPatrns = _savedMoveBetweenPatrns;
            Globals.MainForm.DefaultTable = _savedDefaultTable;
            MainForm.DisableSeparators = _savedDisableSeparators;
            Globals.MainForm.EditorFont = _savedEditorFont;
            MainForm.AutoBackupsOn = _savedAutoBackupsOn;
            MainForm.AutoBackupsMins = _savedAutoBackupsMins;
            MainForm.DisableHints = _savedDisableHints;

            ColorThemes.SetColorThemeByName(_savedThemeName);
            Globals.MainForm.ChangeBackupTimer();

            if (this.MdiChildren.Length != 0)
            {
                Globals.MainForm.RedrawOff();
                MainForm.ChildsEventsBlocked = true;
                MainForm.EditorFontChanged = true;
                Globals.MainForm.RedrawChilds();
                Globals.MainForm.AutoMetricsForChilds(this.WindowState);
                Globals.MainForm.SetChildsPosition(this.WindowState);
                _newSIze = Globals.MainForm.GetSizeForChilds(this.WindowState, false);
                Globals.MainForm.AutoCutChilds(_newSIze);
                Globals.MainForm.AutoToolBarPosition(_newSIze);
                Globals.MainForm.SetWindowSize(_newSIze);
                Globals.MainForm.NumberOfLinesChanged = false;
                MainForm.EditorFontChanged = false;
                MainForm.ChildsEventsBlocked = false;
                Globals.MainForm.RedrawOn();
            }

            if (_savedChanAllocIndex != MainForm.ChanAllocIndex)
            {
                Globals.MainForm.SetChannelsAllocation(_savedChanAllocIndex);
                _panoramChanged = true;
            }

            Globals.MainForm.SetEmulatingChip(_savedChipType);

            if (_savedDefaultChipFreq != Main.DefaultChipFreq)
                Main.DefaultChipFreq = _savedDefaultChipFreq;

            if (_savedStdChannelsAllocation != AY.StdChannelsAllocation)
            {
                UIActionManager.Instance.SetText(UIActionType.ToggleChannels, AY.SetStdChannelsAllocation(_savedStdChannelsAllocation));
                _chanAllocChanged = true;
            }

            if (_savedPanoram[0] != MainForm.Panoram[0] || _savedPanoram[1] != MainForm.Panoram[1] || _savedPanoram[2] != MainForm.Panoram[2])
            {
                MainForm.Panoram[0] = _savedPanoram[0];
                MainForm.Panoram[1] = _savedPanoram[1];
                MainForm.Panoram[2] = _savedPanoram[2];
                _panoramChanged = true;
            }

            if (_chanAllocChanged || _panoramChanged)
            {
                AY.IndexA_L = (byte)(255 - MainForm.Panoram[0]);
                AY.IndexA_R = MainForm.Panoram[0];
                AY.IndexB_L = (byte)(255 - MainForm.Panoram[1]);
                AY.IndexB_R = MainForm.Panoram[1];
                AY.IndexC_L = (byte)(255 - MainForm.Panoram[2]);
                AY.IndexC_R = MainForm.Panoram[2];
                AY.CalculateLevelTables();

                if (AY.RenderEngine == 2)
                    AY.UpdatePanoram();
                else if (WaveOutAPI.IsPlaying)
                {
                    WaveOutAPI.ResetPlaying();
                    ChildForm.PlayingWindow[0].RerollToLine(0);
                    WaveOutAPI.UnResetPlaying();
                }
            }

            if (_savedInterrupt_Freq != Main.DefaultIntFreq)
                Main.DefaultIntFreq = _savedInterrupt_Freq;

            if (AY.RenderEngine != _savedEngineIndex)
                AY.Set_Engine(_savedEngineIndex);

            AY.DCType = _savedDCType;
            AY.DCCutOff = _savedDCCutOff;

            for (int i = 0; i < AY.ChipCount; i++)
            {
                if (AY.AyumiChip[i] == null)
                    continue;

                AY.AyumiChip[i].SetDCType(AY.DCType);
                AY.AyumiChip[i].SetDCCutoff(AY.DCCutOff);
            }

            VTModule.FeaturesLevel = _savedFeaturesLevel;
            VTModule.DetectFeaturesLevel = _savedDetectFeaturesLevel;
            VTModule.VortexModuleHeader = _savedVortexModuleHeader;
            VTModule.DetectModuleHeader = _savedDetectModuleHeader;

            if (!WaveOutAPI.WOThreadActive())
            {
                if (_savedSampleRate != WaveOutAPI.SampleRate)
                    AY.SetSampleRate(_savedSampleRate);

                if (_savedSampleBit != WaveOutAPI.SampleBit)
                    AY.SetBitRate(_savedSampleBit);

                if (_savedNumberOfChannels != WaveOutAPI.NumberOfChannels)
                    AY.SetNChans(_savedNumberOfChannels);

                if (_savedBufLen_ms != WaveOutAPI.BufferLengthMs || _savedNumberOfBuffers != WaveOutAPI.BufferCount)
                    AY.SetBuffers(_savedBufLen_ms, _savedNumberOfBuffers);

                WaveOutAPI.WODevice = _savedWODevice;
            }

            AY.SetFilter(_savedIsFilt, _savedFilt_M);
            Globals.MainForm.SetPriority((ProcessPriorityClass)_savedPrior);

            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }
    }
}