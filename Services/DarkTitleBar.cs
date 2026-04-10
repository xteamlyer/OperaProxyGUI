using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace OperaProxyGUI.Services
{
    /// <summary>
    /// Тёмная рамка / Title Bar через DWM (Windows 10 20H1+ / Windows 11).
    /// </summary>
    public static class DarkTitleBar
    {
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        public static void Apply(Window window)
        {
            if (window == null) return;
            try
            {
                var hwnd = new WindowInteropHelper(window).Handle;
                if (hwnd == IntPtr.Zero) return;
                int dark = 1;
                if (DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref dark, sizeof(int)) != 0)
                    DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, ref dark, sizeof(int));
            }
            catch { /* не критично — останется светлая рамка */ }
        }

        public static void ApplyOnLoaded(Window window)
        {
            if (window == null) return;
            void Apply()
            {
                try
                {
                    var hwnd = new WindowInteropHelper(window).Handle;
                    if (hwnd == IntPtr.Zero) return;
                    int dark = 1;
                    if (DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref dark, sizeof(int)) != 0)
                        DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, ref dark, sizeof(int));
                }
                catch { }
            }

            if (window.IsLoaded) Apply();
            else window.SourceInitialized += (s, e) => Apply();
        }

        public static void ApplyToAll()
        {
            try
            {
                foreach (Window w in Application.Current.Windows) ApplyOnLoaded(w);
            }
            catch { }
        }
    }
}
