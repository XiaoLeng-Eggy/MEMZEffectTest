// 特效模块类 (EffectModule)
//
// 说明：
// 本类的大部分源代码参考了 ExRFy 开发的 EUZE Malware（易语言病毒项目）中的“特效(病毒)模块.ec”。
// 项目链接：https://gitee.com/ExMinecraft158/EUZE_Malware/releases
//
// 在将其从易语言代码转换为 C# 代码的过程中，我们尽量保持了原有的逻辑结构和功能实现，
// 同时注重代码的可读性和结构化，便于理解和后续维护。
// 
// 注意：
// 本类中的所有功能均为视觉或音频特效模拟，不包含任何系统破坏性代码，
// 仅用于学习和研究目的，请勿用于非法或恶意用途。
//
// 技术选择说明：
// - 使用 C# 而非 C++ 的主要原因是快速搭建可视化应用程序框架（如 WinForms）更加方便。
// - 大量直接调用 Windows API（而非 System.Drawing 中的封装方法）是为了：
//     1. 更接近底层图形接口，性能更高；
//     2. 便于将来将代码移植到 C++ 或其他语言时参考；
//     3. 保留与易语言中原有 API 调用方式的一致性。
//
// 函数列表中包含了多种屏幕特效、图标绘制、鼠标控制、声音播放等功能，
// 所有功能均通过 P/Invoke 调用原生 Windows API 实现。

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;
using System.Threading;

namespace MEMZEffectTest
{
    public class EffectModule
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
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);

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

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int MessageBoxTimeout(IntPtr hWnd, string text, string caption, uint type, uint wLanguageId, uint dwMilliseconds);

        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        // 光栅操作码常量定义
        private const uint SRCCOPY = 0x00CC0020;
        private const uint NOTSRCCOPY = 0x00330008;
        private const uint PATCOPY = 0x00F00021;
        private const uint CAPTUREBLT = 0x40000000;
        private const uint WHITENESS = 0x00FF0062;

        // 图标资源ID常量定义
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

        // 消息框样式常量定义
        private const uint MB_OK = 0x00000000;
        private const uint MB_ICONINFORMATION = 0x00000040;
        private const uint MB_ICONWARNING = 0x00000030;
        private const uint MB_ICONERROR = 0x00000010;
        private const uint MB_ICONQUESTION = 0x00000020;
        private const uint MB_SYSTEMMODAL = 0x00001000;

        // SetWindowPos函数常量定义
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;

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

        // RECT 结构体定义
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        // POINT 结构体定义
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


        // 初始化函数
        static EffectModule()
        {
            desktopWindow = GetDesktopWindow();
            desktopDC = GetWindowDC(desktopWindow);
            GetWindowRect(desktopWindow, out desktopRect);
        }

        // MAKEINTRESOURCE 函数的 C# 实现（将整数转换为 IntPtr）
        private static IntPtr MAKEINTRESOURCE(int resourceId)
        {
            return new IntPtr(resourceId);
        }

        // 获取屏幕宽度
        private static int GetScreenWidth()
        {
            return Screen.PrimaryScreen.Bounds.Width;
        }

        // 获取屏幕高度
        private static int GetScreenHeight()
        {
            return Screen.PrimaryScreen.Bounds.Height;
        }

        // 获取鼠标水平位置
        private static int GetMouseX()
        {
            return Cursor.Position.X;
        }

        // 获取鼠标垂直位置
        private static int GetMouseY()
        {
            return Cursor.Position.Y;
        }

        // 获取随机数
        private static int GetRandom(int min, int max)
        {
            return random.Next(min, max + 1);
        }

        // 设置随机数种子
        private static void SetRandomSeed()
        {
            random = new Random();
        }

        // 获取颜色值
        private static int GetColorValue(int red, int green, int blue)
        {
            // 确保RGB值在有效范围内
            red = Math.Max(0, Math.Min(255, red));
            green = Math.Max(0, Math.Min(255, green));
            blue = Math.Max(0, Math.Min(255, blue));
            return Color.FromArgb(red, green, blue).ToArgb();
        }

        // 屏幕反色
        public static void ScreenInvert()
        {
            int width = desktopRect.Right - desktopRect.Left;
            int height = desktopRect.Bottom - desktopRect.Top;
            BitBlt(desktopDC, 0, 0, width, height, desktopDC, 0, 0, NOTSRCCOPY);
        }

        // 鼠标画error图标
        public static void DrawErrorIconAtMouse()
        {
            IntPtr hIcon = LoadIcon(IntPtr.Zero, MAKEINTRESOURCE(IDI_ERROR));
            IntPtr dc = GetDC(IntPtr.Zero);
            DrawIcon(dc, GetMouseX(), GetMouseY(), hIcon);
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 随机位置画图标
        public static void DrawIconsAtRandomPositions()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();
        
            // 定义图标ID数组
            int[] iconIds = { IDI_ERROR, IDI_APPLICATION, IDI_HAND, IDI_WARNING, 
                              IDI_SHIELD, IDI_INFORMATION, IDI_WINLOGO, IDI_QUESTION };
        
            // 随机选择一个图标ID
            int randomIconId = iconIds[GetRandom(0, iconIds.Length - 1)];
        
            // 绘制随机选中的图标到随机位置
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), 
                     LoadIcon(IntPtr.Zero, MAKEINTRESOURCE(randomIconId)));
        
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 鼠标和随机位置疯狂画图标
        public static void DrawIconsCrazy()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();
            int mouseX = GetMouseX();
            int mouseY = GetMouseY();
        
            // 定义图标ID数组
            int[] iconIds = { IDI_ERROR, IDI_APPLICATION, IDI_HAND, IDI_WARNING, 
                              IDI_SHIELD, IDI_INFORMATION, IDI_WINLOGO, IDI_QUESTION };
        
            // 随机选择一个图标ID
            int randomIconId = iconIds[GetRandom(0, iconIds.Length - 1)];
            
            // 加载一次图标，避免重复加载
            IntPtr hIcon = LoadIcon(IntPtr.Zero, MAKEINTRESOURCE(randomIconId));
            
            // 使用循环替代重复代码，绘制4组图标（在鼠标位置和随机位置）
            for (int i = 0; i < 4; i++)
            {
                DrawIcon(dc, mouseX, mouseY, hIcon);
                DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), hIcon);
            }
        
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 鼠标画随机图标
        public static void DrawIconsAtMouse()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int mouseX = GetMouseX();
            int mouseY = GetMouseY();
        
            // 定义图标ID数组
            int[] iconIds = { IDI_ERROR, IDI_APPLICATION, IDI_HAND, IDI_WARNING, 
                              IDI_SHIELD, IDI_INFORMATION, IDI_WINLOGO, IDI_QUESTION };
        
            // 随机选择一个图标ID
            int randomIconId = iconIds[GetRandom(0, iconIds.Length - 1)];
        
            // 绘制随机选中的图标
            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, MAKEINTRESOURCE(randomIconId)));
        
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 鼠标画未知程序图标
        public static void DrawUnknownIconsAtMouse()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int mouseX = GetMouseX();
            int mouseY = GetMouseY();

            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, MAKEINTRESOURCE(IDI_APPLICATION)));
            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, MAKEINTRESOURCE(IDI_WINLOGO)));

            ReleaseDC(IntPtr.Zero, dc);
        }

        // 鼠标画警告图标
        public static void DrawWarningIconAtMouse()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            DrawIcon(dc, GetMouseX(), GetMouseY(), LoadIcon(IntPtr.Zero, MAKEINTRESOURCE(IDI_WARNING)));
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 鼠标画信息图标
        public static void DrawInfoIconAtMouse()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            DrawIcon(dc, GetMouseX(), GetMouseY(), LoadIcon(IntPtr.Zero, MAKEINTRESOURCE(IDI_INFORMATION)));
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 鼠标画问号图标
        public static void DrawQuestionIconAtMouse()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            DrawIcon(dc, GetMouseX(), GetMouseY(), LoadIcon(IntPtr.Zero, MAKEINTRESOURCE(IDI_QUESTION)));
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 鼠标画Windows双色黄蓝色防火墙护盾图标
        public static void DrawShieldIconAtMouse()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            DrawIcon(dc, GetMouseX(), GetMouseY(), LoadIcon(IntPtr.Zero, MAKEINTRESOURCE(IDI_SHIELD)));
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 屏幕上下翻转
        public static void ScreenFlipVertical()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int width = GetScreenWidth();
            int height = GetScreenHeight();
            StretchBlt(dc, 0, height, width, -height, dc, 0, 0, width, height, SRCCOPY | CAPTUREBLT);
            ReleaseDC(GetDesktopWindow(), dc);
        }

        // 屏幕融化
        public static void ScreenMelt()
        {
            SetRandomSeed();
            IntPtr dc = GetDC(IntPtr.Zero);
            int width = GetScreenWidth();
            int height = GetScreenHeight();
            int x = GetRandom(-width / 4, width * 5 / 4);
            int y = GetRandom(-height / 4, height * 5 / 4);
            int w = GetRandom(-width / 2, width / 2);
            int h = GetRandom(-height / 2, height / 2);

            BitBlt(dc, x + GetRandom(-1, 3), y + GetRandom(-4, 4), w, h, dc, x, y, SRCCOPY);
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 彩虹猫中隧道
        public static void NyanCatMiddleTunnel()
        {
            int width = GetScreenWidth();
            IntPtr dc = GetDC(IntPtr.Zero);
            StretchBlt(dc, 50, 50, width - 100, width - 200, dc, 0, 0, width, width, SRCCOPY);
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 彩虹猫下隧道
        public static void NyanCatBottomTunnel()
        {
            int width = GetScreenWidth();
            IntPtr dc = GetDC(IntPtr.Zero);
            StretchBlt(dc, 50, 50, width - 100, width - 100, dc, 0, 0, width, width, SRCCOPY);
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 彩虹猫上隧道
        public static void NyanCatTopTunnel()
        {
            int width = GetScreenWidth();
            IntPtr dc = GetDC(IntPtr.Zero);
            StretchBlt(dc, 50, 50, width - 100, width - 300, dc, 0, 0, width, width, SRCCOPY);
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 彩虹猫左隧道
        public static void NyanCatLeftTunnel()
        {
            int width = GetScreenWidth();
            IntPtr dc = GetDC(IntPtr.Zero);
            StretchBlt(dc, 50, 50, width - 200, width - 200, dc, 0, 0, width, width, SRCCOPY);
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 彩虹猫右隧道
        public static void NyanCatRightTunnel()
        {
            int width = GetScreenWidth();
            IntPtr dc = GetDC(IntPtr.Zero);
            StretchBlt(dc, 50, 50, width, width - 200, dc, 0, 0, width, width, SRCCOPY);
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 随机屏幕乱码
        public static void RandomScreenMess()
        {
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();
            int destX = GetRandom(-screenWidth, screenWidth);
            int destY = GetRandom(-screenHeight, screenHeight);
            int width = GetRandom(-screenWidth, screenWidth);
            int height = GetRandom(-screenHeight, screenHeight);
            int srcX = GetRandom(-screenWidth, screenWidth);
            int srcY = GetRandom(-screenHeight, screenHeight);

            BitBlt(desktopDC, destX, destY, width, height, desktopDC, srcX, srcY, NOTSRCCOPY);
        }

        // 斜屏幕转圈圈 - 注意：PlgBlt在某些系统上可能无法正常工作
        public static void ScreenSkewRotate()
        {
            int width = GetScreenWidth();
            int height = GetScreenHeight();
            IntPtr dc = GetDC(IntPtr.Zero);
            IntPtr memDC = CreateCompatibleDC(dc);
            IntPtr memBitmap = CreateCompatibleBitmap(dc, width, height);
            IntPtr oldBitmap = SelectObject(memDC, memBitmap);
            
            // 将屏幕内容复制到内存位图
            BitBlt(memDC, 0, 0, width, height, dc, 0, 0, SRCCOPY);
        
            // 创建有效的变换点 - 形成一个平行四边形
            POINT[] points = new POINT[3];
            int offset = GetRandom(10, 30); // 随机偏移量
            points[0] = new POINT(0, offset);
            points[1] = new POINT(width, -offset);
            points[2] = new POINT(offset, height);
        
            // 应用变换
            PlgBlt(memDC, points, memDC, 0, 0, width, height, IntPtr.Zero, 0, 0);
        
            // 将处理后的内容绘制回屏幕
            BitBlt(dc, 0, 0, width, height, memDC, 0, 0, SRCCOPY);
        
            // 清理资源
            SelectObject(memDC, oldBitmap); // 恢复原来的位图
            DeleteObject(memBitmap);
            DeleteDC(memDC);
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 屏幕位移
        public static void ScreenShift(int offset)
        {
            // 实现屏幕位移效果
            IntPtr dc = GetDC(IntPtr.Zero);
            int width = GetScreenWidth();
            int height = GetScreenHeight();
            
            // 创建一个兼容的DC和位图来存储屏幕内容
            IntPtr memDC = CreateCompatibleDC(dc);
            IntPtr memBitmap = CreateCompatibleBitmap(dc, width, height);
            SelectObject(memDC, memBitmap);
            
            // 复制当前屏幕到内存位图
            BitBlt(memDC, 0, 0, width, height, dc, 0, 0, SRCCOPY);
            
            // 从内存位图复制回屏幕，但有偏移
            BitBlt(dc, offset, 0, width - Math.Abs(offset), height, memDC, offset > 0 ? 0 : -offset, 0, SRCCOPY);
            
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 屏幕花屏位移
        public static void ScreenMessyShift(int offset)
        {
            // 实现屏幕花屏位移效果
            IntPtr dc = GetDC(IntPtr.Zero);
            int width = GetScreenWidth();
            int height = GetScreenHeight();
            
            // 创建一个兼容的DC和位图来存储屏幕内容
            IntPtr memDC = CreateCompatibleDC(dc);
            IntPtr memBitmap = CreateCompatibleBitmap(dc, width, height);
            SelectObject(memDC, memBitmap);
            
            // 复制当前屏幕到内存位图
            BitBlt(memDC, 0, 0, width, height, dc, 0, 0, SRCCOPY);
            
            // 添加一些随机噪点效果
            for (int i = 0; i < 50; i++)
            {
                int randX = GetRandom(0, width);
                int randY = GetRandom(0, height);
                int randW = GetRandom(10, 50);
                int randH = GetRandom(10, 50);
                
                BitBlt(dc, randX, randY, randW, randH, memDC, randX + GetRandom(-20, 20), randY + GetRandom(-20, 20), SRCCOPY);
            }
            
            // 应用位移
            BitBlt(dc, offset, 0, width - Math.Abs(offset), height, memDC, offset > 0 ? 0 : -offset, 0, SRCCOPY);
            
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 屏幕位移kill
        public static void ScreenShiftKill()
        {
            ScreenShift(GetRandom(-250, 250));
        }

        // 屏幕花屏位移kill
        public static void ScreenMessyShiftKill()
        {
            ScreenMessyShift(GetRandom(-250, 250));
        }

        // 鼠标抖动
        public static void MouseShake()
        {
            int mouseX = GetMouseX();
            int mouseY = GetMouseY();
            int shakeAmount1 = GetRandom(5, GetRandom(7, 12));
            int shakeAmount2 = GetRandom(5, GetRandom(7, 12));
            int shakeAmount3 = GetRandom(5, GetRandom(7, 12));
            int shakeAmount4 = GetRandom(5, GetRandom(7, 12));

            SetCursorPos(mouseX + GetRandom(-shakeAmount1, shakeAmount2), mouseY + GetRandom(-shakeAmount3, shakeAmount4));
            SetCursorPos(mouseX - GetRandom(-shakeAmount1, shakeAmount2), mouseY - GetRandom(-shakeAmount3, shakeAmount4));
        }

        // 随机画error图标
        public static void DrawErrorIconRandom()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, MAKEINTRESOURCE(IDI_ERROR)));
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 随机画警告图标
        public static void DrawWarningIconRandom()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, MAKEINTRESOURCE(IDI_WARNING)));
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 随机画未知程序图标
        public static void DrawUnknownIconRandom()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, MAKEINTRESOURCE(IDI_APPLICATION)));
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, MAKEINTRESOURCE(IDI_WINLOGO)));
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 随机画Windows双色黄蓝色防火墙护盾图标
        public static void DrawShieldIconRandom()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, MAKEINTRESOURCE(IDI_SHIELD)));
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 随机画问号图标
        public static void DrawQuestionIconRandom()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, MAKEINTRESOURCE(IDI_QUESTION)));
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 随机画信息图标
        public static void DrawInfoIconRandom()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, MAKEINTRESOURCE(IDI_INFORMATION)));
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 隐藏鼠标
        public static void HideMouse()
        {
            ShowCursor(false);
        }

        // 显示鼠标
        public static void ShowMouse()
        {
            ShowCursor(true);
        }

        // 窗口文字翻转
        public static void WindowTextFlip()
        {
            IntPtr hWnd = GetDesktopWindow();
            hWnd = GetWindow(hWnd, GW_CHILD);
            StringBuilder title = new StringBuilder(200);

            while (hWnd != IntPtr.Zero)
            {
                title.Clear();
                GetWindowText(hWnd, title, 200);
                hWnd = GetWindow(hWnd, GW_HWNDNEXT);
            }
            // 注意：这段代码只是获取窗口标题，没有实际翻转它们
        }

        // 鼠标乱动
        public static void MouseRandomMove()
        {
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();
            SetCursorPos(GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight));
        }

        // ===== 新增效果（与原易语言模块相较） =====
        
        // 屏幕水平翻转
        public static void ScreenFlipHorizontal()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int width = GetScreenWidth();
            int height = GetScreenHeight();
            StretchBlt(dc, width, 0, -width, height, dc, 0, 0, width, height, (uint)CopyPixelOperation.SourceCopy | CAPTUREBLT);
            ReleaseDC(GetDesktopWindow(), dc);
        }
        
        // 屏幕网格效果
        public static void ScreenGridEffect()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int width = GetScreenWidth();
            int height = GetScreenHeight();
            int gridSize = GetRandom(20, 100);
            
            // 创建内存DC和位图
            IntPtr memDC = CreateCompatibleDC(dc);
            IntPtr memBitmap = CreateCompatibleBitmap(dc, width, height);
            SelectObject(memDC, memBitmap);
            
            // 复制屏幕到内存
            BitBlt(memDC, 0, 0, width, height, dc, 0, 0, SRCCOPY);
            
            // 应用网格效果
            for (int x = 0; x < width; x += gridSize)
            {
                for (int y = 0; y < height; y += gridSize)
                {
                    BitBlt(dc, x, y, gridSize, gridSize, memDC, x + GetRandom(-gridSize/4, gridSize/4), y + GetRandom(-gridSize/4, gridSize/4), SRCCOPY);
                }
            }
            
            ReleaseDC(IntPtr.Zero, dc);
        }
        
        // 彩虹效果
        public static void RainbowEffect()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int width = GetScreenWidth();
            int height = GetScreenHeight();
            
            // 画彩虹条纹
            for (int i = 0; i < 10; i++)
            {
                int y = GetRandom(0, height);
                int stripeHeight = GetRandom(5, 20);
                
                // 确保条纹不会超出屏幕范围
                int endY = Math.Min(y + stripeHeight, height);
                
                // 绘制完整的彩虹条纹，而不仅仅是单行
                for (int currentY = y; currentY < endY; currentY++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // 创建彩虹色
                        int r = (int)(Math.Sin(x * 0.02 + i) * 127 + 128);
                        int g = (int)(Math.Sin(x * 0.02 + i + 2) * 127 + 128);
                        int b = (int)(Math.Sin(x * 0.02 + i + 4) * 127 + 128);
                        
                        // 转换Color到COLORREF格式并绘制像素
                        int colorRef = Color.FromArgb(r, g, b).ToArgb() & 0x00FFFFFF; // 去掉alpha通道
                        SetPixel(dc, x, currentY, colorRef);
                    }
                }
            }
            
            ReleaseDC(IntPtr.Zero, dc);
        }
        
        // 闪烁效果
        public static void ScreenFlash()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int width = GetScreenWidth();
            int height = GetScreenHeight();
            
            // 创建白色画刷
            IntPtr whiteBrush = CreateSolidBrush(Color.White.ToArgb());
            SelectObject(dc, whiteBrush);
            
            // 绘制全屏白色
            PatBlt(dc, 0, 0, width, height, PATCOPY);
            
            // 短暂延迟
            Thread.Sleep(50);
            
            // 刷新屏幕（通过反色再反色来恢复）
            BitBlt(desktopDC, 0, 0, width, height, desktopDC, 0, 0, NOTSRCCOPY);
            BitBlt(desktopDC, 0, 0, width, height, desktopDC, 0, 0, NOTSRCCOPY);
            
            ReleaseDC(IntPtr.Zero, dc);
        }
        
        // 波浪效果
        public static void ScreenWaveEffect()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int width = GetScreenWidth();
            int height = GetScreenHeight();
            
            // 创建内存DC和位图
            IntPtr memDC = CreateCompatibleDC(dc);
            IntPtr memBitmap = CreateCompatibleBitmap(dc, width, height);
            SelectObject(memDC, memBitmap);
            
            // 复制屏幕到内存
            BitBlt(memDC, 0, 0, width, height, dc, 0, 0, SRCCOPY);
            
            // 应用波浪效果
            for (int y = 0; y < height; y += 4)
            {
                int offset = (int)(Math.Sin(y * 0.02) * 20);
                BitBlt(dc, offset, y, width - Math.Abs(offset), 4, memDC, 0, y, SRCCOPY);
            }
            
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 随机播放系统声音
        public static void RandomPlaySystemSound()
        {
            Random random = new Random();
            int index = random.Next(0, SystemSoundAliases.Length);
            string soundAlias = SystemSoundAliases[index];

            // 播放选定的系统声音
            // 使用SND_ALIAS标志指定播放系统预定义声音
            // 使用SND_ASYNC标志允许声音异步播放，不阻塞程序
            // 使用SND_NODEFAULT标志防止在声音无法播放时播放默认声音
            PlaySound(soundAlias, IntPtr.Zero, SND_ALIAS | SND_ASYNC | SND_NODEFAULT);
        }

        // 随机位置显示消息框（不阻塞主UI线程）
        public static void ShowRandomMessageBox()
        {
            // 创建新线程来显示消息框，避免阻塞主UI线程
            Thread messageBoxThread = new Thread(() =>
            {
                try
                {
                    int screenWidth = GetScreenWidth();
                    int screenHeight = GetScreenHeight();
                    string[] messages =
                    {
                        "系统检测到异常活动！",
                        "注意：您的计算机可能受到感染！",
                        "警告：请立即关闭所有程序！",
                        "错误：无法继续执行操作！",
                        "信息：系统正在执行自检..."
                    };
                    string[] titles =
                    {
                        "系统警告",
                        "安全提示",
                        "错误信息",
                        "重要通知",
                        "系统消息"
                    };
                    uint[] icons = { MB_ICONWARNING, MB_ICONERROR, MB_ICONINFORMATION, MB_ICONQUESTION };

                    // 随机选择消息、标题和图标
                    string message = messages[GetRandom(0, messages.Length - 1)];
                    string title = titles[GetRandom(0, titles.Length - 1)];
                    uint icon = icons[GetRandom(0, icons.Length - 1)];
                    uint messageBoxType = MB_OK | icon | MB_SYSTEMMODAL;

                    // 启动一个线程来查找并移动消息框
                    Thread moverThread = new Thread(() =>
                    {
                        // 等待消息框创建
                        Thread.Sleep(10);

                        // 查找消息框窗口
                        IntPtr hWnd = IntPtr.Zero;
                        int attempts = 0;
                        while (hWnd == IntPtr.Zero && attempts < 10)
                        {
                            hWnd = FindWindow(null, title);
                            Thread.Sleep(10);
                            attempts++;
                        }

                        if (hWnd != IntPtr.Zero)
                        {
                            // 获取消息框的实际大小，而不是使用固定值
                            GetWindowRect(hWnd, out RECT rect);
                            int msgBoxWidth = rect.Right - rect.Left;
                            int msgBoxHeight = rect.Bottom - rect.Top;

                            // 随机计算位置，确保消息框完全在屏幕内
                            int x = GetRandom(0, screenWidth - msgBoxWidth);
                            int y = GetRandom(0, screenHeight - msgBoxHeight);

                            // 使用SetWindowPos代替MoveWindow，更可靠地移动窗口
                            SetWindowPos(hWnd, IntPtr.Zero, x, y, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
                        }
                    }) { IsBackground = true }; // 后台线程，防止阻塞主UI线程
                    moverThread.Start();

                    // 显示消息框，设置超时时间（可选）
                    MessageBoxTimeout(IntPtr.Zero, message, title, messageBoxType, 0, 10000); // 10秒后自动关闭
                }
                catch { }
            }) { IsBackground = true }; // 后台线程，防止阻塞主UI线程
        
            messageBoxThread.Start();
        }

        // 批量显示多个随机消息框
        public static void ShowMultipleRandomMessageBoxes(int count)
        {
            for (int i = 0; i < count; i++)
            {
                // 每个消息框延迟随机时间显示，避免同时出现
                Thread.Sleep(GetRandom(100, 500));
                ShowRandomMessageBox();
            }
        }
    }
}