using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VortexTracker
{
    public partial class Win32
    {
        public const int MAX_PATH = 260;
        public const int SC_SIZE = 0xF000;
        public const int SC_SIZE_SIDELEFT = SC_SIZE + 1;
        public const int SC_SIZE_SIDERIGHT = SC_SIZE + 2;
        public const int SC_SIZE_TOPMIDDLE = SC_SIZE + 3;
        public const int SC_SIZE_TOPLEFT = SC_SIZE + 4;
        public const int SC_SIZE_TOPRIGHT = SC_SIZE + 5;
        public const int SC_SIZE_BOTTOMMIDDLE = SC_SIZE + 6;
        public const int SC_SIZE_BOTTOMLEFT = SC_SIZE + 7;
        public const int SC_SIZE_BOTTOMRIGHT = SC_SIZE + 8;
        public const int SC_MOVE = 0xF010;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_CLOSE = 0xF060;
        public const int SC_RESTORE = 0xF120;

        //public const int SWP_NOSIZE = 0x1;
        //public const int SWP_NOMOVE = 0x2;
        //public const int SWP_NOACTIVATE = 0x0010;

        public const int SB_LINELEFT = 0;
        public const int SB_LINERIGHT = 1;
        public const int SB_LEFT = 6;
        public const int SB_RIGHT = 7;

        public const int WM_SETREDRAW = 0xB;
        public const int WM_PAINT = 0xF;
        public const int WM_GETDLGCODE = 0x87;
        public const int WM_CHAR = 0x0102;
        public const int WM_FONTCHANGE = 0x001D;

        public const int WM_NOTIFY = 0x004E;

        public const int WM_SETFOCUS = 0x0007;
        public const int WM_KILLFOCUS = 0x0008;
        public const int WM_HSCROLL = 0x0114;
        //public const int WM_USER = 0x0400;

        public const int WM_SYSCHAR = 0x0106;
        public const int WM_ERASEBKGND = 0x0014;

        public const int WM_WINDOWPOSCHANGING = 0x0046;
        public const int WM_WINDOWPOSCHANGED = 0x0047;

        public const int WM_USER = 0x0400;
        /* public const int UM_REDRAWTRACKS = WM_USER + 1;
        public const int UM_PLAYINGOFF = WM_USER + 2;
        public const int UM_FINALIZEWO = WM_USER + 3;
        public const int EM_GETTEXTRANGE = WM_USER + 52;
        public const int EM_GETEVENTMASK = WM_USER + 59;
        public const int EM_SETEVENTMASK = WM_USER + 69;
        public const int EM_AUTOURLDETECT = WM_USER + 91; */

        public const int WM_SYSCOLORCHANGE = 0x0015;
        public const int WM_REFLECT = 0x2000;

        public const int WM_SYSCOMMAND = 0x0112;

        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;
        public const int WM_MOUSEWHEEL = 0x20A;

        public const int SW_SHOWNORMAL = 1;
        public const int EN_LINK = 0x070C;

        public const uint HWND_BROADCAST = 0xFFFF;

        public const uint DLGC_WANTTAB = 0x0002;

        public const uint STILL_ACTIVE = 0x103;
        public const int ENM_LINK = 0x04000000;

        public const int RDW_INVALIDATE = 0x0001;
        public const int RDW_INTERNALPAINT = 0x0002;
        public const int RDW_ERASE = 0x0004;

        public const int RDW_VALIDATE = 0x0008;
        public const int RDW_NOINTERNALPAINT = 0x0010;
        public const int RDW_NOERASE = 0x0020;

        public const int RDW_NOCHILDREN = 0x0040;
        public const int RDW_ALLCHILDREN = 0x0080;

        public const int RDW_UPDATENOW = 0x0100;
        public const int RDW_ERASENOW = 0x0200;

        public const int RDW_FRAME = 0x0400;
        public const int RDW_NOFRAME = 0x0800;

        public const uint RT_RCDATA = 0x0000000a;

        public const uint PM_REMOVE = 0x0001;

        public const byte FIXED_PITCH = 1;

        public const uint CF_PRIVATEFIRST = 0x0200;
        public const uint GMEM_MOVEABLE = 0x0002;

        public const uint SHCNE_ASSOCCHANGED = 0x08000000;
        public const uint SHCNF_IDLIST = 0x0000;

        public const uint CF_TEXT = 1;

        [DllImport("kernel32.dll")]
        public static extern void OutputDebugString(string lpOutputString);

        [DllImport("USER32.dll")]
        public static extern short GetKeyState(System.Windows.Forms.Keys nVirtKey);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hWndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMHDR
        {
            public IntPtr hwndFrom;
            public IntPtr idFrom;
            public uint code;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CHARRANGE
        {
            public int cpMin;
            public int cpMax;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ENLINK
        {
            public NMHDR nmhdr;
            public uint msg;         // The mouse message (e.g., WM_LBUTTONDOWN)
            public IntPtr wParam;
            public IntPtr lParam;
            public CHARRANGE chrg;   // Character range where the event occurred
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TEXTRANGE
        {
            public CHARRANGE chrg;
            public IntPtr lpstrText; // Pointer to a buffer that receives the text
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, ref TEXTRANGE lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

        [DllImport("user32.dll")]
        public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, int flags);

        [DllImport("user32.dll")]
        public static extern bool RedrawWindow(IntPtr hWnd, ref RECT lprcUpdate, IntPtr hrgnUpdate, int flags);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        [DllImport("kernel32.dll")]
        public static extern bool SetEvent(IntPtr hEvent);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeThread(IntPtr hThread, out uint lpExitCode);

        public delegate int ThreadStartDelegate(IntPtr a);

        /* [DllImport("kernel32.dll")]
        public static extern IntPtr CreateThread(IntPtr SecurityAttributes, uint StackSize, ThreadStartDelegate StartFunction, IntPtr ThreadParameter, uint CreationFlags, out uint ThreadId);

        public const uint ABOVE_NORMAL_PRIORITY_CLASS = 0x00008000;
        public const uint BELOW_NORMAL_PRIORITY_CLASS = 0x00004000;
        public const uint HIGH_PRIORITY_CLASS = 0x00000080;
        public const uint IDLE_PRIORITY_CLASS = 0x00000040;
        public const uint NORMAL_PRIORITY_CLASS = 0x00000020;
        public const uint REALTIME_PRIORITY_CLASS = 0x00000100; */

        [DllImport("kernel32")]
        public static extern long SetThreadPriority(IntPtr hThread, uint nPriority);

        //[DllImport("kernel32.dll", SetLastError = true)]
        //public static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("kernel32.dll")]
        public static extern bool ReleaseMutex(IntPtr hMutex);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
            public int cx;
            public int cy;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public IntPtr handle;
            public uint msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public POINT p;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            public RECT rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public byte[] rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int Length;
            public int Flags;
            public int ShowCmd;
            public POINT MinPosition;
            public POINT MaxPosition;
            public RECT NormalPosition;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        public static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);
        
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        public const int GWL_STYLE = -16;
        public const int SWP_NOMOVE = 0x0001;
        public const int SWP_NOSIZE = 0x0002;
        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_NOACTIVATE = 0x0010;
        public const int SWP_FRAMECHANGED = 0x0020;

        public const uint WS_THICKFRAME = 0x00040000;
        public const uint WS_MAXIMIZEBOX = 0x00010000;
        public const uint WS_MINIMIZEBOX = 0x00020000;

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        //[DllImport("user32.dll")]
        //private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        /// <summary>Give a child window a sizable (true) or fixed-single (false) border without recreating the handle.</summary>
        public static void SetSizable(Form form, bool sizable)
        {
            IntPtr h = form.Handle;
            uint style = (uint)GetWindowLong32(h, GWL_STYLE);

            bool alreadySizable = (style & WS_THICKFRAME) != 0;

            if (sizable == alreadySizable) return;   // nothing to do

            if (sizable)
                style |= WS_THICKFRAME | WS_MAXIMIZEBOX | WS_MINIMIZEBOX;
            else
                style &= ~(WS_THICKFRAME | WS_MAXIMIZEBOX | WS_MINIMIZEBOX);

            SetWindowLong32(h, GWL_STYLE, (int)style);

            // Tell Windows the frame bits changed; keep position and size
            SetWindowPos(h, IntPtr.Zero, 0, 0, 0, 0,
                         SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        public static extern uint SetTextColor(IntPtr hdc, int crColor);

        [DllImport("gdi32.dll")]
        public static extern uint SetBkColor(IntPtr hdc, int crColor);

        [DllImport("gdi32.dll")]
        public static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString, int cbString, out SIZE lpSize);

        public enum COLOR : int
        {
            SCROLLBAR = 0,
            BACKGROUND = 1,
            DESKTOP = 1,
            ACTIVECAPTION = 2,
            INACTIVECAPTION = 3,
            MENU = 4,
            WINDOW = 5,
            WINDOWFRAME = 6,
            MENUTEXT = 7,
            WINDOWTEXT = 8,
            CAPTIONTEXT = 9,
            ACTIVEBORDER = 10,
            INACTIVEBORDER = 11,
            APPWORKSPACE = 12,
            HIGHLIGHT = 13,
            HIGHLIGHTTEXT = 14,
            BTNFACE = 15,
            THREEDFACE = 15,
            BTNSHADOW = 16,
            THREEDSHADOW = 16,
            GRAYTEXT = 17,
            BTNTEXT = 18,
            INACTIVECAPTIONTEXT = 19,
            BTNHIGHLIGHT = 20,
            TREEDHIGHLIGHT = 20,
            THREEDHILIGHT = 20,
            BTNHILIGHT = 20,
            THREEDDKSHADOW = 21,
            THREEDLIGHT = 22,
            INFOTEXT = 23,
            INFOBK = 24,
            HOTLIGHT = 26,
            MENUHILIGHT = 29,
            MENUBAR = 30,
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern uint GetSysColor(COLOR nIndex);

        [DllImport("user32.dll")]
        public static extern bool SetSysColors(int cElements, int[] lpaElements, int[] lpaRgbValues);

        /* [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString); */

        /* [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC); */

        [DllImport("user32.dll")]
        public static extern bool CreateCaret(IntPtr hWnd, int hBitmap, int nWidth, int nHeight);

        [DllImport("user32.dll")]
        public static extern bool SetCaretPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern bool DestroyCaret();

        [DllImport("user32.dll")]
        public static extern bool ShowCaret(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool HideCaret(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int ScrollWindow(IntPtr hwnd, int XAmount, int YAmount, ref RECT lpRect, ref RECT lpClipRect);

        [DllImport("user32.dll")]
        public static extern int ScrollWindow(IntPtr hwnd, int XAmount, int YAmount, IntPtr lpRect, IntPtr lpClipRect);

        public enum RasterOperations : uint
        {
            SRCCOPY = 0x00CC0020,
            SRCPAINT = 0x00EE0086,
            SRCAND = 0x008800C6,
            SRCINVERT = 0x00660046,
            SRCERASE = 0x00440328,
            NOTSRCCOPY = 0x00330008,
            NOTSRCERASE = 0x001100A6,
            MERGECOPY = 0x00C000CA,
            MERGEPAINT = 0x00BB0226,
            PATCOPY = 0x00F00021,
            PATPAINT = 0x00FB0A09,
            PATINVERT = 0x005A0049,
            DSTINVERT = 0x00550009,
            BLACKNESS = 0x00000042,
            WHITENESS = 0x00FF0062,
            CAPTUREBLT = 0x40000000
        }

        /* [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, RasterOperations dwRop);
        */
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        /*
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr FindResource(IntPtr hModule, string lpName, string lpType);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LockResource(IntPtr hResData);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);


        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr AddFontMemResourceEx(byte[] pbFont, int cbFont, IntPtr pdv, out uint pcFonts);

        [DllImport("gdi32.dll")]
        public static extern int AddFontResourceEx(string lpszFilename, uint fl, IntPtr pdv);

        [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
        public static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)] string lpFileName); */

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern uint DragQueryFile(IntPtr hDrop, uint iFile, StringBuilder lpszFile, uint cch);

        [DllImport("shell32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DragQueryPoint(IntPtr hDrop, out POINT lppt);

        [DllImport("shell32.dll")]
        public static extern void DragAcceptFiles(IntPtr hDrop, bool fAccept);

        [DllImport("shell32.dll")]
        public static extern void DragFinish(IntPtr hDrop);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint ExpandEnvironmentStrings(string lpSrc, StringBuilder lpDst, uint nSize);

        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);

       /*  [DllImport("user32.dll")]
        public static extern int FillRect(IntPtr hDC, [In] ref RECT lprc, IntPtr hbr);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateSolidBrush(uint crColor); */

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        /* [DllImport("user32.dll", SetLastError = true)]
        public static extern uint RegisterClipboardFormat(string lpszFormat);

        [DllImport("user32.dll")]
        public static extern bool IsClipboardFormatAvailable(uint format);

        [DllImport("user32.dll")]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool EmptyClipboard();

        [DllImport("user32.dll")]
        public static extern IntPtr GetClipboardData(uint uFormat);


        [DllImport("user32.dll")]
        public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

        [DllImport("user32.dll")]
        public static extern bool CloseClipboard();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        public static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GlobalSize(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GlobalFree(IntPtr hMem); */

        [DllImport("shell32.dll")]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        public static readonly IntPtr HWND_TOP = new IntPtr(0);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int AddFontResourceEx(string lpszFilename, uint fl, IntPtr pdv);

        public const uint FR_PRIVATE = 0x10;
        public const uint FR_NOT_ENUM = 0x20;
    }
}
