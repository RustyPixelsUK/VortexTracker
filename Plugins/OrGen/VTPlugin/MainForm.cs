using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks.Dataflow;
using System.Windows.Forms;
using LibVT;
using LibVT.Plugins;

namespace VTPlugin
{
    public partial class MainForm : Form
    {
        private enum KeyType
        {
            Normal,
            Pressed
        }

        private static readonly string[] _whiteNotes = { "C", "D", "E", "F", "G", "A", "B" };
        private static readonly string[] _blackNotes = { "C#", "D#", "F#", "G#", "A#" };
        private static readonly int[] _blackKeyOffsetInOctave = new int[] { 12, 28, 60, 76, 92 };

        private Label[] _labels = new Label[64];

        private Bitmap _keyboardBitmap = null;

        private bool _mouseDown = false;

        private string _selectedNote = "C-3";
        private string _baseNote = "C-3";

        private IHost _host = null;

        private Ornament _ornament = null;

        private string _fileName = String.Empty;

        public MainForm(IHost host)
        {
            _host = host;

            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("VTPlugin.Resources.Images.Keyboard.png"))
                this._keyboardBitmap = new Bitmap(resourceStream);

            InitializeComponent();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (!this.Visible)
                return;

            CenterForm();
            InitLabels();

            _ornament = _host.GetCurrentOrnament();

            UpdateOrnament();
        }

        private void CenterForm()
        {
            Form parent = Owner;
            int x = parent.Left + (parent.Width - Width) / 2;
            int y = parent.Top + (parent.Height - Height) / 2;
            Location = new Point(x, y);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;

            Owner?.Activate();

            this.Hide();
        }

        private void InitLabels()
        {
            int cellWidth = NotePanel.ClientSize.Width / 8;
            int cellHeight = NotePanel.ClientSize.Height / 8;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    int index = y + x * 8;

                    _labels[index] = new Label();
                    _labels[index].Font = new Font("Courier New", 8, FontStyle.Bold, GraphicsUnit.Point, 0);
                    _labels[index].Tag = index;
                    _labels[index].Name = $"Note_{index:X2}";
                    _labels[index].Parent = NotePanel;
                    _labels[index].AutoSize = true;
                    _labels[index].TextAlign = ContentAlignment.MiddleCenter;
                    _labels[index].Text = $"{index:X2}|+00";
                    _labels[index].Location = new Point(x * cellWidth, y * cellHeight);
                    _labels[index].Cursor = Cursors.Hand;
                    _labels[index].Click += NoteLabel_Click;
                }
            }
        }

        private void UpdateOrnament()
        {
            ClearLabelText();

            if (_ornament == null)
                _ornament = new Ornament();

            for (int i = 0; i < _ornament.Length; i++)
            {
                int noteOffset = _ornament.Offsets[i];
                _labels[i].Text = $"{i:X2}|{noteOffset.ToString("+00;-00;+00")}";
            }

            UpdateLabelBackgroundColors();
            UpdateLabelIndices();
            KeyboardPictureBox.Invalidate();
            _host.RedrawOrnaments();
        }

        private void NoteLabel_Click(object? sender, EventArgs e)
        {
            Label noteLabel = (Label)sender;
            int index = (int)noteLabel.Tag;

            if (index > _ornament.Length)
                return;

            _ornament.Loop = index;

            UpdateLabelBackgroundColors();
            UpdateLabelIndices();
            _host.RedrawOrnaments();
        }

        private void ReadOrnament(string fileName)
        {
            ClearLabelText();
            UpdateLabelBackgroundColors();
            UpdateLabelIndices();

            string[] lines = File.ReadAllLines(fileName);
            int startIndex = Array.IndexOf(lines, "[Ornament]") + 1;
            string ornamentLine = lines[startIndex];

            if (Ornament.RecognizeOrnamentString(ornamentLine, _ornament))
            {
                for (int i = 0; i < _ornament.Length; i++)
                {
                    int noteOffset = _ornament.Offsets[i];
                    _labels[i].Text = $"{i:X2}|{noteOffset.ToString("+00;-00;+00")}";
                }
            }

            UpdateLabelBackgroundColors();
            UpdateLabelIndices();
        }

        private void WriteOrnament(string fileName)
        {
            List<string> lines = new List<string>();
            List<string> offsets = new List<string>();

            lines.Add("[Ornament]");
            lines.Add(_ornament.ToString());

            File.WriteAllLines(fileName, lines);
        }

        private void ClearLabelText()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    int index = y + x * 8;

                    Label noteLabel = _labels[index];

                    noteLabel.Text = $"{index:X2}|+00";
                }
            }
        }

        private void UpdateLabelBackgroundColors()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    int index = y + x * 8;

                    Label noteLabel = _labels[index];

                    if (index == _ornament.Length)
                    {
                        noteLabel.BackColor = Color.DarkGray;
                    }
                    else if (index > _ornament.Length)
                    {
                        noteLabel.BackColor = Color.LightGray;
                    }
                    else if (index >= _ornament.Loop)
                    {
                        noteLabel.BackColor = Color.Plum;
                    }
                    else
                    {
                        noteLabel.BackColor = Color.LightGray;
                    }
                }
            }
        }

        private void UpdateLabelIndices()
        {
            int noteIndex = Math.Min(_ornament.Length, 63);

            BaseLabel.Text = $"Base key: {_baseNote}";
            OffsetLabel.Text = $"Offset to: {_selectedNote}";
            LengthLabel.Text = $"Length: #{noteIndex:X2} Loop: #{_ornament.Loop:X2}";
        }

        private void KeyboardPictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (_keyboardBitmap == null)
                return;

            Graphics g = e.Graphics;

            const int totalOctaves = 5;
            const int totalKeys = totalOctaves * 7;

            for (int i = 0; i < totalKeys; i++)
            {
                DrawKey(g, i, false, 0, KeyType.Normal);

                if (_mouseDown)
                    DrawKey(g, _selectedNote, i, Color.Lime, true);

                DrawKey(g, _baseNote, i, Color.Cyan, true);
                DrawKey(g, _baseNote, i, Color.Cyan, false);

                if (_mouseDown)
                    DrawKey(g, _selectedNote, i, Color.Lime, false);
            }
        }

        private bool DrawKey(Graphics g, string noteName, int index, Color color, bool drawKey)
        {
            int keyOctave = index / 7;

            char note;
            bool isBlack;
            int octave;

            GetNoteInfo(noteName, out note, out isBlack, out octave);

            int keyIndex = octave * 7 + NoteToIndex(note);
            int blackIndex = BlackIndexFromNote(noteName);

            if (isBlack && blackIndex != -1 && keyOctave == octave)
            {
                int[] whiteIndex = BlackToWhite(blackIndex, keyOctave * 7);

                if (whiteIndex.Contains(index))
                {
                    if (drawKey)
                        DrawKey(g, index, true, blackIndex, KeyType.Pressed);
                    else
                        DrawRectangle(g, index, true, color, blackIndex);

                    return true;
                }
            }

            if (index == keyIndex)
            {
                if (drawKey)
                    DrawKey(g, index, false, 0, KeyType.Pressed);
                else
                    DrawRectangle(g, index, false, color);

                return true;
            }

            return false;
        }

        private void DrawKey(Graphics g, int index, bool isBlack, int blackIndex, KeyType keyType)
        {
            const int keyWidth = 16;
            const int keyHeight = 80;

            int[] whiteKeySrcXs = new int[] { 0, 16, 32, 0, 16, 16, 32 };
            int[,] blackKeySrcXs = new int[,]
            {
                { 48, 64, 32, 0, 16, 16, 32 },
                { 0, 80, 96, 0, 16, 16, 32 },
                { 0, 16, 32, 48, 64, 16, 32 },
                { 0, 16, 32, 0, 80, 64, 32 },
                { 0, 16, 32, 0, 16, 80, 96 },
            };

            int keyIndexInOctave = index % 7;
            int destX = index * keyWidth;
            int srcX = isBlack ? blackKeySrcXs[blackIndex, keyIndexInOctave] : whiteKeySrcXs[keyIndexInOctave];
            int offsetX = (keyType == KeyType.Pressed ? 48 : 0);

            Rectangle srcRect = new Rectangle(offsetX + srcX, 0, keyWidth, keyHeight);
            Rectangle destRect = new Rectangle(destX, 0, keyWidth, keyHeight);

            g.DrawImage(_keyboardBitmap, destRect, srcRect, GraphicsUnit.Pixel);
        }

        private void DrawRectangle(Graphics g, int index, bool isBlack, Color color, int blackIndex = -1)
        {
            const int keyWidth = 16;
            Rectangle rect;

            if (isBlack && blackIndex != -1)
            {
                int octave = index / 7;
                int offsetX = octave * 7 * keyWidth + _blackKeyOffsetInOctave[blackIndex];

                rect = new Rectangle(offsetX + 1, 38, 5, 5);
            }
            else
            {
                int destX = index * keyWidth;
                rect = new Rectangle(destX + 5, 68, 6, 6);
            }

            using (Brush brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, rect);
            }
        }

        private int NoteToIndex(char note)
        {
            switch (note)
            {
                case 'C': return 0;
                case 'D': return 1;
                case 'E': return 2;
                case 'F': return 3;
                case 'G': return 4;
                case 'A': return 5;
                case 'B': return 6;
                default: return -1;
            }
        }

        private int WhiteToBlack(int index)
        {
            int[] whiteToBlackIndex = new int[] { -1, 0, 0, 1, 1, -1, -1, 2, 2, 3, 3, 4, 4, -1 };

            return whiteToBlackIndex[index];
        }

        private int BlackIndexFromNote(string note)
        {
            if (note[1] != '#')
                return -1;

            string noteName = note.Substring(0, 2);

            for (int i = 0; i < _blackNotes.Length; i++)
            {
                if (_blackNotes[i] == noteName)
                    return i;
            }

            return -1;
        }

        private int[] BlackToWhite(int index, int offset)
        {
            switch (index)
            {
                case 0: return new int[] { 0 + offset, 1 + offset };
                case 1: return new int[] { 1 + offset, 2 + offset };
                case 2: return new int[] { 3 + offset, 4 + offset };
                case 3: return new int[] { 4 + offset, 5 + offset };
                case 4: return new int[] { 5 + offset, 6 + offset };
            }

            return new int[] { };
        }

        private int NoteToSemitoneIndex(string note)
        {
            // Extract note and octave
            bool isBlack = note.Contains("#");
            string notePart = isBlack ? note.Substring(0, 2) : note.Substring(0, 1);
            int octave = int.Parse(note.Substring(isBlack ? 2 : 2, 1));

            // Map note to semitone offset within the octave
            Dictionary<string, int> noteOffsets = new()
            {
                { "C", 0 }, { "C#", 1 },
                { "D", 2 }, { "D#", 3 },
                { "E", 4 },
                { "F", 5 }, { "F#", 6 },
                { "G", 7 }, { "G#", 8 },
                { "A", 9 }, { "A#", 10 },
                { "B", 11 }
            };

            int noteOffset = noteOffsets[notePart];

            return octave * 12 + noteOffset;
        }

        private void GetNoteInfo(string noteName, out char note, out bool isBlack, out int octave)
        {
            note = noteName[0];
            isBlack = noteName[1] == '#';
            octave = int.Parse(noteName[2].ToString()) - 1;
        }

        private void UpdateKeys(int x, int y, ref string noteName)
        {
            if (x < 0 || x >= KeyboardPictureBox.Width || y < 0 || y >= KeyboardPictureBox.Height)
                return;

            bool isBlack = y < 56;
            int keyIndex = x / 16;
            int octave = keyIndex / 7;
            int whiteIndexInOctave = keyIndex % 7;
            int blackIndex = WhiteToBlack((x / 8) % 14);

            if (isBlack && blackIndex != -1)
            {
                noteName = _blackNotes[blackIndex] + (octave + 1);
            }
            else
            {
                noteName = _whiteNotes[whiteIndexInOctave] + "-" + (octave + 1);
            }
        }

        private void KeyboardPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                UpdateKeys(e.X, e.Y, ref _selectedNote);

                _mouseDown = true;

                KeyboardPictureBox.Invalidate();
                _host.RedrawOrnaments();
            }
            else if (e.Button == MouseButtons.Right)
            {
                UpdateKeys(e.X, e.Y, ref _baseNote);

                UpdateLabelIndices();

                KeyboardPictureBox.Invalidate();
                _host.RedrawOrnaments();
            }
        }

        private void KeyboardPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                UpdateKeys(e.X, e.Y, ref _selectedNote);

                KeyboardPictureBox.Invalidate();
                _host.RedrawOrnaments();
            }
        }

        private void KeyboardPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;

            if (e.Button == MouseButtons.Left)
            {
                UpdateKeys(e.X, e.Y, ref _selectedNote);

                int selectedKeyIndex = NoteToSemitoneIndex(_selectedNote);
                int baseKeyIndex = NoteToSemitoneIndex(_baseNote);

                int noteOffset = selectedKeyIndex - baseKeyIndex;
                int noteIndex = Math.Min(_ornament.Length, 63);

                Label noteLabel = _labels[noteIndex];
                noteLabel.Text = $"{noteIndex:X2}|{noteOffset.ToString("+00;-00;+00")}";

                _ornament.Offsets[noteIndex] = (sbyte)noteOffset;
                _ornament.Length = noteIndex;

                if (_ornament.Length < 64)
                    _ornament.Length++;

                UpdateLabelBackgroundColors();
                UpdateLabelIndices();

                KeyboardPictureBox.Invalidate();
                _host.RedrawOrnaments();
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            if (_ornament.Length > 0)
                _ornament.Length--;

            Label noteLabel = _labels[_ornament.Length];
            noteLabel.Text = $"{_ornament.Length:X2}|+00";

            _ornament.Offsets[_ornament.Length] = 0;
            _ornament.Loop = Math.Min(_ornament.Loop, _ornament.Length);

            UpdateLabelBackgroundColors();
            UpdateLabelIndices();

            KeyboardPictureBox.Invalidate();
            _host.RedrawOrnaments();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            _fileName = "";
            _selectedNote = "C-3";
            _baseNote = "C-3";

            Array.Clear(_ornament.Offsets, 0, _ornament.Offsets.Length);
            _ornament.Length = 0;
            _ornament.Loop = 0;

            ClearLabelText();
            UpdateLabelBackgroundColors();
            UpdateLabelIndices();

            KeyboardPictureBox.Invalidate();
            _host.RedrawOrnaments();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Ornament files (*.vto)|*.vto|All files (*.*)|*.*";
            openFileDialog.Title = "Load Ornament File";
            openFileDialog.FileName = _fileName;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _fileName = Path.GetFileName(openFileDialog.FileName);

                ReadOrnament(openFileDialog.FileName);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Ornament files (*.vto)|*.vto|All files (*.*)|*.*";
            saveFileDialog.Title = "Save Ornament File";
            saveFileDialog.FileName = _fileName;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _fileName = Path.GetFileName(saveFileDialog.FileName);

                WriteOrnament(saveFileDialog.FileName);
            }
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            _ornament = _host.GetPreviousOrnament();
            UpdateOrnament();
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            _host.PlayOrnament();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            _ornament = _host.GetNextOrnament();
            UpdateOrnament();
        }
    }
}