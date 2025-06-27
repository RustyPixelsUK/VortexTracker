using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VortexTracker
{
    public class Globals
    {
        public static MainForm MainForm = null;
        public static OptionsForm OptionsForm = null;
        public static ExportZXForm ExportZXForm = null;
        public static TracksManagerForm TracksManagerForm = null;
        public static GlobalTransForm GlobalTransForm = null;
        public static TurboSoundForm TurboSoundForm = null;
        public static ToggleSamplesForm ToggleSamplesForm = null;
        public static ProgressBarForm ProgressBarForm = null;
        public static ExportWavOptionsForm ExportWavOptionsForm = null;
        public static TrackInfoForm TrackInfoForm = null;
        public static PluginManagerForm PluginManagerForm = null;

        public static bool InputQuery(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return (dialogResult == DialogResult.OK);
        }

        public static void SetFixedString(string input, byte[] target)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            Array.Clear(target, 0, target.Length);
            Array.Copy(bytes, target, Math.Min(bytes.Length, target.Length));
        }

        public static string GetFixedString(byte[] source)
        {
            return Encoding.ASCII.GetString(source).TrimEnd('\0');
        }

        public static void MouseToCell(DataGridView grid, int x, int y, out int col, out int row)
        {
            var hit = grid.HitTest(x, y);
            col = hit.ColumnIndex;
            row = hit.RowIndex;
        }

        public static long FileAge(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    DateTime lastWriteTime = File.GetLastWriteTime(fileName);
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, lastWriteTime.Kind);
                    return (long)(lastWriteTime - epoch).TotalSeconds;
                }
            }
            catch
            {
            }

            return -1;
        }

        public static bool IsFileWritable(string filePath)
        {
            try
            {
                // If file doesn't exist, just check if we can create it
                if (!File.Exists(filePath))
                {
                    // Try creating and deleting a test file in the same directory
                    string dir = Path.GetDirectoryName(filePath);
                    string testPath = Path.Combine(dir ?? "", Path.GetRandomFileName());

                    using (FileStream fs = File.Create(testPath, 1, FileOptions.DeleteOnClose)) { }

                    return true;
                }

                // Try opening with write access
                using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Write)) { }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
