using System;
using System.ComponentModel;
using System.Windows;
using OperaProxyGUI.ViewModels;

namespace OperaProxyGUI
{
    /// <summary>
    /// Логика взаимодействия главного окна.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (s, e) => ViewModel?.OnLoaded();
        }

        private MainViewModel ViewModel => (MainViewModel)DataContext;

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            ViewModel?.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ViewModel?.OnClosed();
        }
    }
}
