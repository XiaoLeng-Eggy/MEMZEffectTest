using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MEMZEffectTest
{
    internal static class Program
    {
        // 导入Windows API以设置DPI感知
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [DllImport("shcore.dll")]
        private static extern int SetProcessDpiAwareness(int PROCESS_DPI_AWARENESS);

        private enum DpiAwareness
        {
            None = 0,
            SystemAware = 1,
            PerMonitorAware = 2
        }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // 设置DPI感知
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    if (Environment.OSVersion.Version.Build >= 14393) // Windows 10 Anniversary Update及以上
                    {
                        SetProcessDpiAwareness((int)DpiAwareness.PerMonitorAware);
                    }
                    else // Windows Vista到Windows 8.1
                    {
                        SetProcessDPIAware();
                    }
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"应用程序启动时发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
