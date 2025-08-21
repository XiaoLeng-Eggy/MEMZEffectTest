using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;

namespace MEMZEffectTest
{
    public partial class EffectModule
    {
        // Windows API函数声明
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        private static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);

        [DllImport("gdi32.dll")]
        private static extern bool StretchBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int nWidthSrc, int nHeightSrc, uint dwRop);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern int ShowCursor(bool bShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateSolidBrush(int crColor);

        [DllImport("gdi32.dll")]
        private static extern bool PatBlt(IntPtr hdc, int nXLeft, int nYLeft, int nWidth, int nHeight, uint dwRop);

        [DllImport("gdi32.dll")]
        private static extern bool PlgBlt(IntPtr hdcDest, POINT[] lpPoint, IntPtr hdcSrc, int nXSrc, int nYSrc, int nWidth, int nHeight, IntPtr hbmMask, int xMask, int yMask);

        [DllImport("gdi32.dll")]
        private static extern bool SetPixel(IntPtr hdc, int x, int y, int crColor);

        [DllImport("winmm.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool PlaySound(string pszSound, IntPtr hmod, uint fdwSound);

        // 常量定义
        private const uint SRCCOPY = 0x00CC0020;
        private const uint NOTSRCCOPY = 0x00330008;
        private const uint PATCOPY = 0x00F00021;
        private const uint CAPTUREBLT = 0x40000000;
        private const uint WHITENESS = 0x00FF0062;

        private const uint GW_CHILD = 5;
        private const uint GW_HWNDNEXT = 2;
        private const int IDI_ERROR = 32513;
        private const int IDI_WARNING = 32515;
        private const int IDI_INFORMATION = 32516;
        private const int IDI_QUESTION = 32514;
        private const int IDI_SHIELD = 32518;
        private const int IDI_APPLICATION = 32512;
        private const int IDI_WINLOGO = 32517;
        private const int IDI_HAND = 32513;
        private const int IDI_ASTERISK = 32516;

        // 播放声音的常量定义
        private const uint SND_FILENAME = 0x00020000;
        private const uint SND_ALIAS = 0x00010000;
        private const uint SND_ASYNC = 0x00000001;
        private const uint SND_NODEFAULT = 0x00000002;

        // 系统声音别名
        private static readonly string[] SystemSoundAliases =
        {
            "SystemAsterisk",     // 信息提示音
            "SystemExclamation",  // 警告提示音
            "SystemHand",         // 错误提示音
            "SystemQuestion",     // 问题提示音
            "SystemStart",        // 启动提示音
            "SystemExit",         // 退出提示音
            "SystemMenuCommand",  // 菜单命令提示音
            "SystemMenuPopup",    // 菜单弹出提示音
            "SystemDefault"
        };

        // 结构定义
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        // 类变量
        private static IntPtr desktopWindow;
        private static IntPtr desktopDC;
        private static RECT desktopRect;
        private static Random random = new Random();
    }
}
