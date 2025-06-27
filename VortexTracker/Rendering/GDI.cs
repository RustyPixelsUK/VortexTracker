using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexTracker.Rendering
{
    public class GDI
    {
        public static void DrawText(Graphics g, Font font, int x, int y, string str, Color textColor)
        {
            TextRenderer.DrawText(g, str, font, new Point(x, y), textColor, TextFormatFlags.NoPadding);
        }

        public static void DrawText(Graphics g, Font font, int x, int y, string str, Color textColor, Color bgColor)
        {
            TextRenderer.DrawText(g, str, font, new Point(x, y), textColor, bgColor, TextFormatFlags.NoPadding);
        }

        public static void FillRectangle(Graphics g, Rectangle rect, Color bgColor)
        {
            using (SolidBrush brush = new SolidBrush(bgColor))
                g.FillRectangle(brush, rect);
        }

        public static void DrawTriangleUp(Graphics g, Font font, int x, int y, Color textColor, Color bgColor)
        {
            TextRenderer.DrawText(g, "0", font, new Point(x, y), textColor, bgColor, TextFormatFlags.NoPadding);
        }

        public static void DrawTriangleDown(Graphics g, Font font, int x, int y, Color textColor, Color bgColor)
        {
            TextRenderer.DrawText(g, "1", font, new Point(x, y), textColor, bgColor, TextFormatFlags.NoPadding);
        }
    }
}
