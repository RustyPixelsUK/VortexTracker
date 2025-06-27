using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace VortexTracker
{
    public class FileBrowser : ListBox
    {
        private List<string> FilePath = new();
        private List<string> DirPath = new();
        public bool DontOpenItem = false;
        public Control ParentWin = null;
        public bool PreviewPlaying = false;
        private string _currentDir = string.Empty;
        private string _fileExt = "vts";
        private bool _mouseDown = false;
        private ToolTip _toolTip;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string CurrentDir
        {
            get => _currentDir;
            set => _currentDir = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string FileExt
        {
            get => _fileExt;
            set => _fileExt = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DriveSelect DriveSelectBox { get; set; }

        public FileBrowser(Control parent) :
            base()
        {
            this.Parent = parent;
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.BackColor = Color.White;
            this.DoubleBuffered = true;
            this.Font = new Font("Arial", 9);
            this.ItemHeight = 18;

            this.DrawItem += OnDrawItem;
            this.MouseDown += OnMouseDown;
            this.MouseMove += OnMouseMove;
            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;
            this.Click += OnClick;
            this.DoubleClick += OnDoubleClick;
            this.SelectedIndexChanged += OnSelectedIndexChanged;
        }

        public int GetSelectedIndex()
        {
            if (this.SelectedIndices.Count > 0)
                return this.SelectedIndices[0];
            return -1;
        }

        public string GetSelectedFileName()
        {
            int index = GetSelectedIndex();
            return index != -1 ? this.Items[index].ToString() : string.Empty;
        }

        public string GetSelectedFullPath()
        {
            string fileName = GetSelectedFileName();
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            if (!fileName.StartsWith("["))
            {
                foreach (string path in FilePath)
                {
                    if (Path.GetFileName(path).Equals(fileName, StringComparison.OrdinalIgnoreCase))
                        return path;
                }
            }

            if (fileName == "[..]")
            {
                return !string.IsNullOrEmpty(Path.GetFileName(CurrentDir)) ? CurrentDir : string.Empty;
            }

            if (fileName.StartsWith("["))
            {
                string name = fileName[1..^1]; // strip brackets
                foreach (string path in DirPath)
                {
                    if (Path.GetFileName(path).Equals(name, StringComparison.OrdinalIgnoreCase))
                        return path;
                }
            }

            return string.Empty;
        }

        private void OnDrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= this.Items.Count)
                return;

            string itemText = this.Items[e.Index].ToString();
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            bool isSpecial = itemText.StartsWith("[") && itemText.EndsWith("]");

            e.DrawBackground();

            using Brush textBrush = new SolidBrush(MainForm.ThemeColors[isSelected ? (int)ThemeColor.SamOrnSelText : (int)ThemeColor.SamOrnText]);
            Font itemFont = isSpecial ? new Font(this.Font, FontStyle.Bold) : this.Font;
            e.Graphics.DrawString(itemText, itemFont, textBrush, e.Bounds);

            e.DrawFocusRectangle();
        }

        public void InitDir()
        {
            if (!string.IsNullOrEmpty(CurrentDir) && PathNotFound(CurrentDir, false))
                return;

            ReadDir();

            if (this.Items.Count > 0)
            {
                DontOpenItem = true;
                this.SelectedIndex = 0;
             }
        }

        public void ReadDir()
        {
            this.Items.Clear();

            DirPath.Clear();
            FilePath.Clear();

            if (string.IsNullOrEmpty(CurrentDir))
            {
                foreach (var drive in DriveInfo.GetDrives())
                {
                    DirPath.Add(drive.Name);
                    this.Items.Add($"[{drive.Name.TrimEnd('\\')}]");
                }
                return;
            }

            try
            {
                var directories = Directory.GetDirectories(CurrentDir)
                    .Where(d => !IsSystemDirectory(d)).ToList();
                DirPath.AddRange(directories);

                var files = Directory.GetFiles(CurrentDir, $"*.{FileExt}");
                FilePath.AddRange(files);

                this.Items.Add("[..]");
                foreach (var dir in directories)
                    this.Items.Add($"[{Path.GetFileName(dir)}]");
                foreach (var file in files)
                    this.Items.Add(Path.GetFileName(file));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ParentWin, $"Error Reading Directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool InHomeDir()
        {
            string dir = string.Empty;
            if (FileExt == "vts")
                dir = MainForm.SamplesDefaultDir;
            else if (FileExt == "vto")
                dir = MainForm.OrnamentsDefaultDir;

            string target = Path.GetFullPath(Path.Combine(MainForm.VortexDocumentsDir, dir));
            return CurrentDir.StartsWith(target, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsSystemDirectory(string path)
        {
            string[] sysDirs = new[]
            {
            "$RECYCLE.BIN", "SYSTEM VOLUME INFORMATION", "WINDOWS",
            "PROGRAM FILES", "PROGRAM FILES (X86)", "RECOVERY",
            "PROGRAMDATA", ".TRASHES", ".TEMPORARYITEMS"
        };

            string dirName = Path.GetFileName(path).ToUpperInvariant();
            return sysDirs.Contains(dirName);
        }

        private void OnClick(object sender, EventArgs e) => OpenItem(preview: true);

        private void OnDoubleClick(object sender, EventArgs e) => OpenItem(preview: false);

        private void OnSelectedIndexChanged(object? sender, EventArgs e)
        {
            if (DontOpenItem)
            {
                DontOpenItem = false;
                return;
            }

            if (_mouseDown)
            {
                _mouseDown = false;
                return;
            }

            OpenItem(preview: true);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left && this.Items.Count > 0)
                this.SelectedIndex = 0;
            else if (e.KeyCode == Keys.Right && this.Items.Count > 0)
            {
                this.SelectedIndex = this.Items.Count - 1;
                string item = this.Items[this.SelectedIndex].ToString();
                if (!item.StartsWith("["))
                    OpenItem(preview: true);
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                OpenItem(preview: false);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _mouseDown = true;

            if (e.Button != MouseButtons.Right || string.IsNullOrEmpty(CurrentDir))
                return;

            int index = this.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                this.SelectedIndex = index;
                string selectedItem = this.Items[index].ToString();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            int index = this.IndexFromPoint(e.Location);
            if (index == ListBox.NoMatches)
            {
                this.ToolTipText = string.Empty;
                return;
            }

            string itemText = this.Items[index].ToString();
            using Graphics g = this.CreateGraphics();
            SizeF textSize = g.MeasureString(itemText, this.Font);
            this.ToolTipText = textSize.Width > this.Width ? itemText : string.Empty;
        }

        private string ToolTipText
        {
            set
            {
                _toolTip ??= new ToolTip();
                if (string.IsNullOrEmpty(value)) _toolTip.Hide(this);
                else _toolTip.Show(value, this, this.PointToClient(Cursor.Position), 2000);
            }
        }

        private void OpenItem(bool preview)
        {
            if (DontOpenItem)
            {
                DontOpenItem = false;
                return;
            }

            string item = this.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(item)) return;

            if (item == "[..]")
            {
                string newDir = Path.GetFullPath(Path.Combine(CurrentDir, ".."));
                CurrentDir = (newDir == CurrentDir || Path.GetPathRoot(newDir) == newDir) ? "" : newDir;
                InitDir();
            }
            else if (item.StartsWith("[") && item.EndsWith("]"))
            {
                string dirName = item[1..^1];
                if (dirName.Length == 2 && dirName[1] == ':')
                    CurrentDir = dirName + "\\";
                else
                {
                    foreach (var dir in DirPath)
                    {
                        if (Path.GetFileName(dir).Equals(dirName, StringComparison.OrdinalIgnoreCase))
                        {
                            CurrentDir = dir;
                            break;
                        }
                    }
                }
                InitDir();
            }
            else if (Path.GetExtension(item).TrimStart('.').Equals(FileExt, StringComparison.OrdinalIgnoreCase))
            {
                string fullPath = FilePath.FirstOrDefault(f => Path.GetFileName(f).Equals(item, StringComparison.OrdinalIgnoreCase));
                if (string.IsNullOrEmpty(fullPath) || !File.Exists(fullPath))
                {
                    MessageBox.Show(ParentWin, $"File \"{fullPath}\" Is Not Found.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    InitDir();
                    return;
                }

                OnFileSelected?.Invoke(this, new FileSelectedEventArgs(fullPath, preview));
            }
        }

        public bool PathNotFound(string fullPath, bool isFile)
        {
            string newDir = string.Empty;

            if (isFile && !File.Exists(fullPath))
            {
                MessageBox.Show(ParentWin, $"File \"{fullPath}\" Is Not Found.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                InitDir();
                return true;
            }

            if (!isFile && !Directory.Exists(fullPath))
            {
                MessageBox.Show(ParentWin, $"Directory \"{fullPath}\" Is Not Found.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (FileExt == "vts")
                    newDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Samples");
                else if (FileExt == "vto")
                    newDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Ornaments");

                if (!string.IsNullOrEmpty(newDir) && Directory.Exists(newDir))
                {
                    CurrentDir = newDir;
                    InitDir();
                    return true;
                }

                CurrentDir = "C:\\";
                InitDir();
                return true;
            }

            return false;
        }

        public int GetIndex(string value)
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i].ToString() == value)
                    return i;
            }
            return -1;
        }


        public event EventHandler<FileSelectedEventArgs> OnFileSelected;
    }

    public class FileSelectedEventArgs : EventArgs
    {
        public string FilePath { get; }
        public bool IsPreview { get; }

        public FileSelectedEventArgs(string path, bool preview)
        {
            FilePath = path;
            IsPreview = preview;
        }
    }
}
