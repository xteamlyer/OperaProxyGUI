using System;
using System.IO;
using System.Windows;
using OperaProxyGUI.Services;
using OperaProxyGUI.ViewModels;

namespace OperaProxyGUI
{
    public partial class App : Application
    {
        public static string AppDirectory { get; } =
            Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var window = new MainWindow { DataContext = new MainViewModel() };

            // Тёмный заголовок: применяем после получения HWND
            DarkTitleBar.ApplyOnLoaded(window);

            window.Show();
        }
    }
}
