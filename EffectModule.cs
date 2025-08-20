using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;

namespace MEMZEffect
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

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr LoadIcon(IntPtr hInstance, string lpIconName);

        [DllImport("gdi32.dll")]
        private static extern bool StretchBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int nWidthSrc, int nHeightSrc, uint dwRop);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern int ShowCursor(bool bShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

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

        // 初始化函数
        static EffectModule()
        {
            desktopWindow = GetDesktopWindow();
            desktopDC = GetWindowDC(desktopWindow);
            GetWindowRect(desktopWindow, out desktopRect);
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
            IntPtr hIcon = LoadIcon(IntPtr.Zero, IDI_ERROR.ToString());
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

            // 绘制各种图标
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_ERROR.ToString()));
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_APPLICATION.ToString()));
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_HAND.ToString()));
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_WARNING.ToString()));
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_SHIELD.ToString()));
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_INFORMATION.ToString()));
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_WINLOGO.ToString()));
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_QUESTION.ToString()));

            ReleaseDC(IntPtr.Zero, dc);
        }

        // 疯狂画图标
        public static void DrawIconsCrazy()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();
            int mouseX = GetMouseX();
            int mouseY = GetMouseY();

            // 在鼠标位置和随机位置绘制各种图标
            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, IDI_ERROR.ToString()));
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_ERROR.ToString()));
            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, IDI_APPLICATION.ToString()));
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_APPLICATION.ToString()));
            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, IDI_HAND.ToString()));
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_HAND.ToString()));
            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, IDI_WARNING.ToString()));
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_WARNING.ToString()));
            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, IDI_SHIELD.ToString()));
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_SHIELD.ToString()));
            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, IDI_INFORMATION.ToString()));
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_INFORMATION.ToString()));
            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, IDI_WINLOGO.ToString()));
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_WINLOGO.ToString()));
            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, IDI_QUESTION.ToString()));
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_QUESTION.ToString()));

            ReleaseDC(IntPtr.Zero, dc);
        }

        // 鼠标画一大堆图标
        public static void DrawIconsAtMouse()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int mouseX = GetMouseX();
            int mouseY = GetMouseY();

            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, IDI_ERROR.ToString()));
            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, IDI_APPLICATION.ToString()));
            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, IDI_HAND.ToString()));
            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, IDI_WARNING.ToString()));
            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, IDI_SHIELD.ToString()));
            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, IDI_INFORMATION.ToString()));
            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, IDI_WINLOGO.ToString()));
            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, IDI_QUESTION.ToString()));

            ReleaseDC(IntPtr.Zero, dc);
        }

        // 鼠标画未知程序图标
        public static void DrawUnknownIconsAtMouse()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int mouseX = GetMouseX();
            int mouseY = GetMouseY();

            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, IDI_APPLICATION.ToString()));
            DrawIcon(dc, mouseX, mouseY, LoadIcon(IntPtr.Zero, IDI_WINLOGO.ToString()));

            ReleaseDC(IntPtr.Zero, dc);
        }

        // 鼠标画警告图标
        public static void DrawWarningIconAtMouse()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            DrawIcon(dc, GetMouseX(), GetMouseY(), LoadIcon(IntPtr.Zero, IDI_WARNING.ToString()));
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 鼠标画信息图标
        public static void DrawInfoIconAtMouse()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            DrawIcon(dc, GetMouseX(), GetMouseY(), LoadIcon(IntPtr.Zero, IDI_INFORMATION.ToString()));
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 鼠标画问号图标
        public static void DrawQuestionIconAtMouse()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            DrawIcon(dc, GetMouseX(), GetMouseY(), LoadIcon(IntPtr.Zero, IDI_QUESTION.ToString()));
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 鼠标画Windows双色黄蓝色防火墙护盾图标
        public static void DrawShieldIconAtMouse()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            DrawIcon(dc, GetMouseX(), GetMouseY(), LoadIcon(IntPtr.Zero, IDI_SHIELD.ToString()));
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 屏幕上下翻转
        public static void ScreenFlipVertical()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int width = GetScreenWidth();
            int height = GetScreenHeight();
            StretchBlt(dc, 0, height, width, -height, dc, 0, 0, width, height, (uint)CopyPixelOperation.SourceCopy | CAPTUREBLT);
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
            SelectObject(memDC, memBitmap);
            BitBlt(memDC, 0, 0, width, height, dc, 0, 0, SRCCOPY);

            // 创建变换点
            POINT[] points = new POINT[3];
            points[0] = new POINT(width, -1);
            points[1] = new POINT(width, 0);
            points[2] = new POINT(width, 0);

            // 应用变换
            PlgBlt(memDC, points, memDC, 0, 0, width, height, IntPtr.Zero, 0, 0);

            // 绘制随机颜色
            IntPtr brush = CreateSolidBrush(GetColorValue(GetRandom(-256, 256), GetRandom(-256, 256), GetRandom(-256, 256)));
            SelectObject(memDC, brush);
            PatBlt(memDC, 0, 0, width, height, PATCOPY);

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
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_ERROR.ToString()));
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 随机画警告图标
        public static void DrawWarningIconRandom()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_WARNING.ToString()));
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 随机画未知程序图标
        public static void DrawUnknownIconRandom()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_APPLICATION.ToString()));
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_WINLOGO.ToString()));
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 随机画Windows双色黄蓝色防火墙护盾图标
        public static void DrawShieldIconRandom()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_SHIELD.ToString()));
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 随机画问号图标
        public static void DrawQuestionIconRandom()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_QUESTION.ToString()));
            ReleaseDC(IntPtr.Zero, dc);
        }

        // 随机画信息图标
        public static void DrawInfoIconRandom()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            int screenWidth = GetScreenWidth();
            int screenHeight = GetScreenHeight();
            DrawIcon(dc, GetRandom(-screenWidth, screenWidth), GetRandom(-screenHeight, screenHeight), LoadIcon(IntPtr.Zero, IDI_INFORMATION.ToString()));
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

        // ===== 新增效果 =====
        
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
                
                for (int x = 0; x < width; x++)
                {
                    // 创建彩虹色
                    int r = (int)(Math.Sin(x * 0.02 + i) * 127 + 128);
                    int g = (int)(Math.Sin(x * 0.02 + i + 2) * 127 + 128);
                    int b = (int)(Math.Sin(x * 0.02 + i + 4) * 127 + 128);
                    
                    // 绘制像素点
                    SetPixel(dc, x, y, Color.FromArgb(r, g, b));
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
            System.Threading.Thread.Sleep(50);
            
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
        
        // 辅助函数：设置像素颜色（需要添加DllImport）
        [DllImport("gdi32.dll")]
        private static extern bool SetPixel(IntPtr hdc, int X, int Y, Color crColor);
    }
}