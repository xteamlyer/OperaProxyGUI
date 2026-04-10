using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace OperaProxyGUI.Services
{
    /// <summary>
    /// Иконка в системном трее с контекстным меню.
    /// </summary>
    public sealed class TrayIconService : IDisposable
    {
        private NotifyIcon _notifyIcon;
        private readonly Action _showMainWindow;
        private readonly Action _startProxy;
        private readonly Action _stopProxy;
        private readonly Action _exitApplication;

        public TrayIconService(Action showMainWindow, Action startProxy, Action stopProxy, Action exitApplication)
        {
            _showMainWindow = showMainWindow;
            _startProxy = startProxy;
            _stopProxy = stopProxy;
            _exitApplication = exitApplication;
        }

        public void Init()
        {
            _notifyIcon = new NotifyIcon
            {
                Text = "Opera Proxy GUI",
                Icon = LoadAppIcon(),
                Visible = true,
            };

            var menu = new ContextMenuStrip();
            menu.Items.Add("Показать окно", null, (s, e) => _showMainWindow?.Invoke());
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Запустить прокси", null, (s, e) => _startProxy?.Invoke());
            menu.Items.Add("Остановить прокси", null, (s, e) => _stopProxy?.Invoke());
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Выход", null, (s, e) => _exitApplication?.Invoke());
            _notifyIcon.ContextMenuStrip = menu;
            _notifyIcon.DoubleClick += (s, e) => _showMainWindow?.Invoke();
        }

        public void SetStatus(bool running)
        {
            if (_notifyIcon == null) return;
            _notifyIcon.Text = running ? "Opera Proxy GUI — Работает" : "Opera Proxy GUI — Остановлен";
        }

        public void ShowBalloon(string title, string text)
        {
            try { _notifyIcon?.ShowBalloonTip(2000, title, text, ToolTipIcon.Info); }
            catch { }
        }

        private static Icon LoadAppIcon()
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                using (var s = asm.GetManifestResourceStream("OperaProxyGUI.Resources.app.ico"))
                    if (s != null) return new Icon(s);
            }
            catch { }
            return SystemIcons.Application;
        }

        public void Dispose()
        {
            var icon = _notifyIcon;
            _notifyIcon = null;
            if (icon != null)
            {
                icon.Visible = false;
                icon.Dispose();
            }
        }
    }
}
