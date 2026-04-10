using System;
using Microsoft.Win32;

namespace OperaProxyGUI.Services
{
    /// <summary>
    /// Автозапуск через реестр HKCU\Run.
    /// </summary>
    public static class AutostartService
    {
        private const string RegKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "OperaProxyGUI";

        public static bool IsRegistered()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegKey, false))
                    return key?.GetValue(AppName) != null;
            }
            catch { return false; }
        }

        public static void Register()
        {
            try
            {
                var exePath = Environment.ProcessPath;
                using (var key = Registry.CurrentUser.CreateSubKey(RegKey, writable: true))
                    key?.SetValue(AppName, $"\"{exePath}\"");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Не удалось добавить в автозапуск: " + ex.Message, ex);
            }
        }

        public static void Unregister()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(RegKey, writable: true))
                    key?.DeleteValue(AppName, throwOnMissingValue: false);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Не удалось убрать из автозапуска: " + ex.Message, ex);
            }
        }
    }
}
