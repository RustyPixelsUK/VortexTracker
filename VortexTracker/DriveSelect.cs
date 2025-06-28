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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VortexTracker
{
    public class DriveSelect : ComboBox
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FileBrowser FileBrowser { get; set; }

        public DriveSelect(Control parent) :
            base()
        {
            this.Parent = parent;
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Font = new Font(this.Font, FontStyle.Bold);
            this.BackColor = Color.White;
            this.DoubleBuffered = true;

            this.DrawItem += OnDrawItem;
            this.SelectedIndexChanged += OnSelectedIndexChanged;
        }

        public void FillDiskDrives()
        {
            if (FileBrowser == null || string.IsNullOrEmpty(FileBrowser.CurrentDir))
                return;

            Items.Clear();

            List<string> drives = DriveInfo.GetDrives().Select(d => d.Name[0] + ":").ToList();
            string currentDrive = FileBrowser.CurrentDir[0].ToString();
            int curItemIndex = 0;
            int homeIndex = 0;
            int quickIndex = -1;
            string quickDir = string.Empty;

            for (int i = 0; i < drives.Count; i++)
            {
                Items.Add(drives[i]);
                if (drives[i][0].ToString() == currentDrive)
                    curItemIndex = i;
            }

            if (FileBrowser.FileExt == "vts")
            {
                string samplesHome = Path.Combine(MainForm.VortexDocumentsDir, MainForm.SamplesDefaultDir);
                if (Directory.Exists(samplesHome))
                {
                    Items.Add("*Samples");
                    homeIndex = Items.Count - 1;
                }

                if (!string.IsNullOrEmpty(MainForm.SamplesQuickDir) && Directory.Exists(MainForm.SamplesQuickDir))
                {
                    Items.Add(Path.GetFileName(MainForm.SamplesQuickDir));
                    quickDir = MainForm.SamplesQuickDir;
                    quickIndex = Items.Count - 1;
                }
            }
            else if (FileBrowser.FileExt == "vto")
            {
                string ornamentsHome = Path.Combine(MainForm.VortexDocumentsDir, MainForm.OrnamentsDefaultDir);
                if (Directory.Exists(ornamentsHome))
                {
                    Items.Add("*Ornaments");
                    homeIndex = Items.Count - 1;
                }

                if (!string.IsNullOrEmpty(MainForm.OrnamentsQuickDir) && Directory.Exists(MainForm.OrnamentsQuickDir))
                {
                    Items.Add(Path.GetFileName(MainForm.OrnamentsQuickDir));
                    quickDir = MainForm.OrnamentsQuickDir;
                    quickIndex = Items.Count - 1;
                }
            }

            if (!string.IsNullOrEmpty(quickDir) && FileBrowser.CurrentDir.StartsWith(quickDir, StringComparison.OrdinalIgnoreCase))
            {
                curItemIndex = quickIndex;
            }
            else if (FileBrowser.InHomeDir())
            {
                curItemIndex = homeIndex;
            }

            if (curItemIndex >= 0 && curItemIndex < Items.Count)
                SelectedIndex = curItemIndex;
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            string text = this.SelectedItem?.ToString();
            if (FileBrowser == null || string.IsNullOrEmpty(text)) return;

            if (text == "*Samples")
            {
                FileBrowser.CurrentDir = Path.Combine(MainForm.VortexDocumentsDir, MainForm.SamplesDefaultDir);
            }
            else if (text == "*Ornaments")
            {
                FileBrowser.CurrentDir = Path.Combine(MainForm.VortexDocumentsDir, MainForm.OrnamentsDefaultDir);
            }
            else if (text.Length == 2 && text[1] == ':')
            {
                FileBrowser.CurrentDir = text + "\\";
            }
            else
            {
                if (FileBrowser.FileExt == "vts")
                    FileBrowser.CurrentDir = MainForm.SamplesQuickDir;
                else if (FileBrowser.FileExt == "vto")
                    FileBrowser.CurrentDir = MainForm.OrnamentsQuickDir;
            }

            FileBrowser.InitDir();
        }

        private void OnDrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= Items.Count) return;

            string itemText = Items[e.Index].ToString();
            string displayText = itemText;
            int iconIndex = 45; // Default drive icon

            if (itemText == "*Samples" || itemText == "*Ornaments")
            {
                iconIndex = 46; // Home folder icon
                displayText = itemText.Substring(1);
            }
            else if (itemText.Length < 2 || itemText[1] != ':')
            {
                iconIndex = 47; // Quick access icon
            }

            e.DrawBackground();

            Image icon = Globals.MainForm.ImageList1.Images[iconIndex];
            int iconSize = e.Bounds.Height - 4;
            Rectangle iconRect = new Rectangle(e.Bounds.Left + 2, e.Bounds.Top + 2, iconSize, iconSize);
            Rectangle textRect = new Rectangle(iconRect.Right + 6, e.Bounds.Top, e.Bounds.Width - iconRect.Width - 8, e.Bounds.Height);

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                using SolidBrush backgroundBrush = new SolidBrush(MainForm.ThemeColors[(int)ThemeColor.SamOrnSelBackground]);
                e.Graphics.FillRectangle(backgroundBrush, e.Bounds);

                using SolidBrush textBrush = new SolidBrush(MainForm.ThemeColors[(int)ThemeColor.SamOrnSelText]);
                e.Graphics.DrawString(displayText, Font, textBrush, textRect);
            }
            else
            {
                using SolidBrush backgroundBrush = new SolidBrush(MainForm.ThemeColors[(int)ThemeColor.SamOrnBackground]);
                e.Graphics.FillRectangle(new SolidBrush(BackColor), e.Bounds);

                using SolidBrush textBrush = new SolidBrush(MainForm.ThemeColors[(int)ThemeColor.SamOrnText]);
                e.Graphics.DrawString(displayText, Font, textBrush, textRect);
            }

            if (icon != null)
            {
                e.Graphics.DrawImage(icon, iconRect);
            }

            e.DrawFocusRectangle();
        }
    }
}
