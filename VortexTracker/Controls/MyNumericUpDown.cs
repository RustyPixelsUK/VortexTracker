using LibVT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace VortexTracker.Controls
{
    public class MyNumericUpDown : NumericUpDown
    {
        public event EventHandler<ValueChangingEventArgs> ValueChanging;

        private bool _suppressEvent = false;
        private int _leadingZeros = 0;
        private bool _validateValue = true;

        public MyNumericUpDown()
            : base()
        {
            UpdateEditText();
        }

        [Category("Behavior")]
        [Description("Specifies whether the control should validate user-entered values.")]
        [DefaultValue(true)]
        public bool ValidateValue
        {
            get => _validateValue;
            set
            {
                _validateValue = value;
                if (_validateValue)
                    UpdateEditText(); // re-sync if toggled on
            }
        }

        [Category("Appearance")]
        [Description("Specifies the minimum number of digits to display by padding with leading zeros.")]
        [DefaultValue(0)]
        public int LeadingZeros
        {
            get => _leadingZeros;
            set
            {
                _leadingZeros = Math.Max(0, value);
                UpdateEditText();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new decimal Value
        {
            get => base.Value;
            set
            {
                if (base.Value != value)
                {
                    decimal oldValue = base.Value;

                    if (!_suppressEvent && _validateValue)
                    {
                        var args = new ValueChangingEventArgs(oldValue, value);
                        OnValueChanging(args);
                        if (args.Cancel)
                            return;
                    }

                    base.Value = value;
                    UpdateEditText();
                }
            }
        }

        protected virtual void OnValueChanging(ValueChangingEventArgs e)
        {
            ValueChanging?.Invoke(this, e);
        }

        public override void UpButton()
        {
            decimal newValue = base.Value + Increment;
            if (newValue <= Maximum)
            {
                var args = new ValueChangingEventArgs(base.Value, newValue);
                OnValueChanging(args);
                if (!args.Cancel)
                    base.UpButton();
            }
        }

        public override void DownButton()
        {
            decimal newValue = base.Value - Increment;
            if (newValue >= Minimum)
            {
                var args = new ValueChangingEventArgs(base.Value, newValue);
                OnValueChanging(args);
                if (!args.Cancel)
                    base.DownButton();
            }
        }

        protected override void UpdateEditText()
        {
            _suppressEvent = true;

            if (!_validateValue)
            {
                _suppressEvent = false;
                return;
            }

            if (LeadingZeros <= 0)
            {
                base.UpdateEditText();
            }
            else
            {
                int intVal = decimal.ToInt32(Math.Truncate(Value));
                if (Hexadecimal)
                    Text = intVal.ToString("X" + LeadingZeros);
                else
                    Text = intVal.ToString("D" + LeadingZeros);
            }

            _suppressEvent = false;
        }

        protected override void ValidateEditText()
        {
            if (!_validateValue)
                return;

            try
            {
                string text = Text.Trim();
                decimal newValue;

                if (Hexadecimal)
                    newValue = Convert.ToDecimal(Convert.ToInt32(text, 16));
                else
                    newValue = Convert.ToDecimal(text);

                base.Value = Math.Min(Math.Max(newValue, Minimum), Maximum);
            }
            catch
            {
                UpdateEditText();
            }
        }
    }

    public class ValueChangingEventArgs : CancelEventArgs
    {
        public decimal OldValue { get; }
        public decimal NewValue { get; }

        public ValueChangingEventArgs(decimal oldValue, decimal newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public enum UpDownDirection
    {
        None,
        Up,
        Down
    }
}
