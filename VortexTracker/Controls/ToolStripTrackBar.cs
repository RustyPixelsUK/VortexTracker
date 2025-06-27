using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace VortexTracker.Controls
{
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip)]
    public class ToolStripTrackBar : ToolStripControlHost
    {
        public ToolStripTrackBar() : base(new TrackBar())
        {
            TrackBar.AutoSize = false;
            TrackBar.Width = 100;
            TrackBar.Height = 22;
            TrackBar.TickFrequency = 4;
        }

        public TrackBar TrackBar => Control as TrackBar;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Left
        {
            get => TrackBar.Left;
            set => TrackBar.Left = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Minimum
        {
            get => TrackBar.Minimum;
            set => TrackBar.Minimum = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Maximum
        {
            get => TrackBar.Maximum;
            set => TrackBar.Maximum = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Value
        {
            get => TrackBar.Value;
            set => TrackBar.Value = value;
        }

        public event EventHandler ValueChanged
        {
            add => TrackBar.ValueChanged += value;
            remove => TrackBar.ValueChanged -= value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Orientation Orientation
        {
            get => TrackBar.Orientation;
            set => TrackBar.Orientation = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int TickFrequency
        {
            get => TrackBar.TickFrequency;
            set => TrackBar.TickFrequency = value;
        }
    }
}
