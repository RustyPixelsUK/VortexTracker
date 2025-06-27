using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexTracker.Controls
{
    public class MyListView : ListView
    {
        private bool _checkFromDoubleClick = false;

        protected override void OnItemCheck(ItemCheckEventArgs ice)
        {
            if (_checkFromDoubleClick)
            {
                ice.NewValue = ice.CurrentValue;
                _checkFromDoubleClick = false;
            }
            else
                base.OnItemCheck(ice);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Is this a double-click?
            if (e.Button == MouseButtons.Left && e.Clicks > 1)
                _checkFromDoubleClick = true;

            base.OnMouseDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            _checkFromDoubleClick = false;
            base.OnKeyDown(e);
        }
    }
}
