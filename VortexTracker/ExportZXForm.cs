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
    public partial class ExportZXForm : Form
    {
        public static int ZXCompAddr = 0xC000;
        public static ushort ZxPlayerSize = 0;
        public static ushort ZxDataSize = 0;
        public static int ZXModSize1 = 0;
        public static int ZXModSize2 = 0;
        public static int BlockSize = 0;
        public static int TmpAddr = 0;

        public ExportZXForm(Form parent)
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
            HexAddressBox.Text = ZXCompAddr.ToString("X");
            DecAddressBox.Text = ZXCompAddr.ToString();
            TmpAddr = ZXCompAddr;
            ShowZXStat();
        }

        private void CenterForm()
        {
            Form parent = Owner;
            int x = parent.Left + (parent.Width - Width) / 2;
            int y = parent.Top + (parent.Height - Height) / 2;
            Location = new Point(x, y);
        }

        public void DecAddressBox_TextChanged(object sender, EventArgs e)
        {
            if (DecAddressBox.Modified)
            {
                if (!int.TryParse(DecAddressBox.Text.Trim(), out int address))
                    return;

                HexAddressBox.Text = address.ToString("X");
                TmpAddr = address;
                ShowZXStat();
            }
        }

        public void HexAddressBox_TextChanged(object sender, EventArgs e)
        {
            string s;
            int i, j;

            if (HexAddressBox.Modified)
            {
                s = HexAddressBox.Text.Trim().ToUpper();
                i = j = 0;

                while (j < s.Length)
                {
                    j++;
                    if (!(s[j] == '0' || s[j] == 'A'))
                        return;

                    if (s[j] >= '0' && s[j] <= '9')
                        i = i * 16 + (int)(s[j]) - (int)('0');
                    else
                        i = i * 16 + (int)(s[j]) - (int)('A') + 10;
                }

                DecAddressBox.Text = i.ToString();
                TmpAddr = i;
                ShowZXStat();
            }
        }

        public void ShowZXStat()
        {
            int i;
            int blockSize = ZXModSize1 + ZXModSize2;

            if (FormatGroup.SelectedIndex != 1)
                blockSize += ZxPlayerSize + ZxDataSize;
            else
                blockSize += 16;

            CompilationAddressValue.Text = $"{65536 - blockSize:X}):";
            i = TmpAddr & 65535;

            if (i + blockSize > 65536)
                i = 65536 - blockSize;

            ZXCompAddr = i;

            if (ZXModSize2 == 0)
            {
                Module2AddressValue.Text = "----";
                Module2LengthValue.Text = "----";
            }
            else
            {
                Module2LengthValue.Text = $"{ZXModSize2:X}";
            }

            CurrentPosValue.Text = "(----)";

            if (FormatGroup.SelectedIndex == 1)
            {
                InitCallValue.Text = "----";
                PlayCallValue.Text = "----";
                MuteCallValue.Text = "----";
                SetupByteValue.Text = "(----)";
                VariablesAddressValue.Text = "----";
                VariablesLengthValue.Text = "----";
                PlayerCodesLengthValue.Text = "----";
                ModuleAddressValue.Text = $"{ZXCompAddr:X}";
                if (ZXModSize2 != 0)
                    Module2AddressValue.Text = $"{(ZXCompAddr + ZXModSize1):X}";
            }
            else
            {
                InitCallValue.Text = $"{ZXCompAddr:X}";
                PlayCallValue.Text = $"{(ZXCompAddr + 5):X}";
                MuteCallValue.Text = $"{(ZXCompAddr + 8):X}";
                SetupByteValue.Text = $"({ZXCompAddr + 10:X})";
                if (ZXModSize2 == 0)
                    CurrentPosValue.Text = $"({ZXCompAddr + 11:X})";
                VariablesAddressValue.Text = $"{(ZXCompAddr + ZxPlayerSize):X}";
                VariablesLengthValue.Text = $"{ZxDataSize:X}";
                PlayerCodesLengthValue.Text = $"{ZxPlayerSize:X}";
                ModuleAddressValue.Text = $"{(ZXCompAddr + ZxPlayerSize + ZxDataSize):X}";
                if (ZXModSize2 != 0)
                    Module2AddressValue.Text = $"{(ZXCompAddr + ZxPlayerSize + ZxDataSize + ZXModSize1):X}";
            }

            ModuleLengthValue.Text = $"{ZXModSize1:X}";
        }

        public void FormatGroup_Click(object sender, EventArgs e)
        {
            ShowZXStat();
        }

        private void ExportZXForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;

            Owner?.Activate();

            this.Hide();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }
    }
}

