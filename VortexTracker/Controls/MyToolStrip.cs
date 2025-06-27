using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexTracker.Controls
{
    public class MyToolStrip : ToolStrip
    {
        const int WM_MOUSEACTIVATE = 0x21;
        const int MA_ACTIVATE = 1;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_MOUSEACTIVATE)
            {
                m.Result = MA_ACTIVATE;   // activate & keep the click
                return;
            }

            base.WndProc(ref m);
        }
    }
}
