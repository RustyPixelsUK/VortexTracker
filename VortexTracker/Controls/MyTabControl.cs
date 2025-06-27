using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexTracker.Controls
{
    public class MyTabControl : TabControl
    {
        protected override void WndProc(ref Message m)
        {
            const int WM_ERASEBKGND = 0x0014;

            if (m.Msg == WM_ERASEBKGND)
            {
                using (Graphics g = Graphics.FromHdc(m.WParam))
                {
                    Rectangle tabHeaderRect = new Rectangle(0, 0, Width, ItemSize.Height + 4);

                    using (Brush brush = new SolidBrush(BackColor))
                        g.FillRectangle(brush, tabHeaderRect);
                }

                // Let default erase continue (optional)
                return; // Uncomment to skip base erase
            }

            base.WndProc(ref m);
        }
    }
}
