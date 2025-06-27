using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VortexTracker.Controls
{
    public class RadioGroup : GroupBox
    {
        private readonly List<RadioButton> _radioButtons = new();
        private readonly RadioButtonItemCollection _items;
        private int _columns = 1;
        private bool _suppressClick = false;

        public RadioGroup()
            : base()
        {
            _items = new RadioButtonItemCollection(this);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadioButtonItemCollection Items => _items;

        [DefaultValue(1)]
        public int Columns
        {
            get => _columns;
            set
            {
                if (value < 1)
                    value = 1;
                _columns = value;
                RelayoutRadioButtons();
            }
        }

        internal void AddRadioButton(string text)
        {
            InsertRadioButton(_radioButtons.Count, text);
        }

        internal void InsertRadioButton(int index, string text)
        {
            RadioButton rb = new RadioButton
            {
                Text = text,
                AutoSize = true
            };

            rb.CheckedChanged += RadioButton_CheckedChanged;

            Controls.Add(rb);
            _radioButtons.Insert(index, rb);
            RelayoutRadioButtons();
        }

        internal void RemoveRadioButtonAt(int index)
        {
            if (index >= 0 && index < _radioButtons.Count)
            {
                RadioButton rb = _radioButtons[index];
                rb.CheckedChanged -= RadioButton_CheckedChanged;
                Controls.Remove(rb);
                _radioButtons.RemoveAt(index);
                RelayoutRadioButtons();
            }
        }

        internal void UpdateRadioButtonText(int index, string text)
        {
            if (index >= 0 && index < _radioButtons.Count)
            {
                _radioButtons[index].Text = text;
            }
        }

        internal void ClearRadioButtons()
        {
            foreach (var rb in _radioButtons)
            {
                rb.CheckedChanged -= RadioButton_CheckedChanged;
                Controls.Remove(rb);
            }
            _radioButtons.Clear();
        }

        private void RelayoutRadioButtons()
        {
            if (_radioButtons.Count == 0 || _columns <= 0)
                return;

            const int padding = 3;

            Rectangle rc = DisplayRectangle;
            int columnWidth = rc.Width / _columns;
            int startX = rc.X + padding;
            int startY = Text == "" ? rc.Y - padding : rc.Y + padding;
            int availableYEnd = rc.Bottom - padding;

            int column = 0;
            int y = startY;

            foreach (RadioButton rb in _radioButtons)
            {
                rb.AutoSize = true;
                rb.Margin = Padding.Empty;
                int height = rb.PreferredSize.Height;

                if (y + height > availableYEnd && column < _columns - 1)
                {
                    column++;
                    y = startY;
                }

                int x = startX + column * columnWidth;
                rb.Location = new Point(x, y);
                y += height + padding;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get => _radioButtons.FindIndex(rb => rb.Checked);
            set
            {
                if (value >= 0 && value < _radioButtons.Count)
                {
                    _suppressClick = true;
                    _radioButtons[value].Checked = true;
                    _suppressClick = false;
                }
            }
        }

        public List<RadioButton> Buttons => _radioButtons;
        public string SelectedText => SelectedIndex >= 0 ? _radioButtons[SelectedIndex].Text : null;

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            RelayoutRadioButtons();
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public new event EventHandler Click;

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressClick)
                return;

            if (sender is RadioButton rb && rb.Checked)
            {
                Click?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            RelayoutRadioButtons();
        }
    }
}
