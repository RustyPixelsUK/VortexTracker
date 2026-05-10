using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace VortexTracker.Controls
{
    public class MyDataGridView : DataGridView
    {
        private bool _suppressCurrentCellChanged;

        public MyDataGridView()
            : base()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.Opaque |
                ControlStyles.StandardClick |
                ControlStyles.UseTextForAccessibility |
                ControlStyles.StandardDoubleClick |
                ControlStyles.FixedHeight |
                ControlStyles.Selectable,
                true);
            UpdateStyles();
            DoubleBuffered = true;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int LeftCol
        {
            get => FirstDisplayedScrollingColumnIndex;
            set
            {
                if (value >= 0 && value < ColumnCount)
                    FirstDisplayedScrollingColumnIndex = value;
            }
        }

        public int VisibleColumnCount
        {
            get
            {
                int visibleCount = 0;
                foreach (DataGridViewColumn col in Columns)
                {
                    if (col.Visible)
                        visibleCount++;
                }
                return visibleCount;
            }
        }

        protected override void OnCurrentCellChanged(EventArgs e)
        {
            if (_suppressCurrentCellChanged)
                return;

            base.OnCurrentCellChanged(e);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle Selection
        {
            get
            {
                if (SelectedCells.Count == 0)
                    return Rectangle.Empty;

                int minRow = int.MaxValue, maxRow = int.MinValue;
                int minCol = int.MaxValue, maxCol = int.MinValue;

                foreach (DataGridViewCell cell in SelectedCells)
                {
                    if (cell.RowIndex < minRow) minRow = cell.RowIndex;
                    if (cell.RowIndex > maxRow) maxRow = cell.RowIndex;
                    if (cell.ColumnIndex < minCol) minCol = cell.ColumnIndex;
                    if (cell.ColumnIndex > maxCol) maxCol = cell.ColumnIndex;
                }

                return new Rectangle(minCol, minRow, maxCol - minCol, maxRow - minRow);
            }

            set
            {
                _suppressCurrentCellChanged = true;

                try
                {
                    ClearSelection();

                    for (int row = value.Top; row < value.Top + value.Height + 1; row++)
                    {
                        for (int col = value.Left; col < value.Left + value.Width + 1; col++)
                        {
                            if (row < RowCount && col < ColumnCount)
                            {
                                this[col, row].Selected = true;
                            }
                        }
                    }

                    // Optional scroll centering
                    int visibleCols = 0;
                    int totalWidth = 0;
                    for (int i = 0; i < Columns.Count; i++)
                    {
                        if (Columns[i].Visible)
                        {
                            totalWidth += Columns[i].Width;
                            visibleCols++;
                        }
                    }

                    int approxColWidth = visibleCols > 0 ? totalWidth / visibleCols : 1;
                    int colsThatFit = ClientSize.Width / approxColWidth;
                    int offset = colsThatFit / 2;

                    int targetScrollCol = Math.Max(0, value.Left - offset);
                    FirstDisplayedScrollingColumnIndex = targetScrollCol;
                }
                finally
                {
                    _suppressCurrentCellChanged = false;
                }

                Invalidate();
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new event MouseEventHandler MouseWheel
        {
            add { base.MouseWheel += value; }
            remove { base.MouseWheel -= value; }
        }
    }
}
