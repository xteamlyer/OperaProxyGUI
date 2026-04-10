using System;
using System.Windows;
using OperaProxyGUI.Services;

namespace OperaProxyGUI.ViewModels
{
    // ── Вспомогательные методы главной ViewModel ───────────────────────────
    public partial class MainViewModel
    {
        private void UpdateStatus()
        {
            ToggleText = IsRunning ? "■  Остановить прокси" : "▶  Запустить прокси";
            StatusText = IsRunning ? "● Работает" : "● Остановлен";
            _tray.SetStatus(IsRunning);
        }

        private void ApplyWinAutostart(bool enable)
        {
            try { if (enable) AutostartService.Register(); else AutostartService.Unregister(); }
            catch (Exception ex) { AddLog("[ОШИБКА] " + ex.Message); }
        }

        private void Save() => ConfigService.Save(Settings);

        private void OnLogReceived(string line) => OnUi(() => AddLog(line));

        private void AddLog(string line)
        {
            Log.Add($"[{DateTime.Now:HH:mm:ss}] {line}");
            if (Log.Count > 3000)
            {
                for (int i = 0; i < 300; i++) Log.RemoveAt(0);
            }
        }

        private void OnUi(Action action)
        {
            if (_syncContext != null)
                _syncContext.Post(_ => action(), null);
            else
                action();
        }

        private void ShowMainWindow()
        {
            var w = Application.Current.MainWindow;
            if (w != null)
            {
                w.Show();
                w.WindowState = WindowState.Normal;
                w.Activate();
            }
        }

        private void Exit()
        {
            _forceClose = true;
            DoStop();
            _tray.Dispose();
            Application.Current.Shutdown();
        }

        public void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_forceClose) return;
            e.Cancel = true;
            Application.Current.MainWindow?.Hide();
        }

        public void OnClosed()
        {
            _runner.Dispose();
            _tray.Dispose();
        }

        public void OnLoaded()
        {
            // Тёмный заголовок применяем после создания HWND
            if (Application.Current.MainWindow is Window w)
                DarkTitleBar.Apply(w);

            // Автозапуск прокси — после полной инициализации окна,
            // чтобы UI успел отрисоваться до начала подбора прокси.
            if (Settings.AutoStartProxy && !_autoStartDone)
            {
                _autoStartDone = true;
                DoStart();
            }
        }
    }
}
