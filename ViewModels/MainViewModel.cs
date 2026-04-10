using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using OperaProxyGUI.Models;
using OperaProxyGUI.Services;

namespace OperaProxyGUI.ViewModels
{
    /// <summary>
    /// Главная ViewModel: связывает настройки, запуск прокси и подбор api-proxy.
    /// </summary>
    public partial class MainViewModel : ViewModelBase
    {
        private readonly ProxyRunnerService _runner = new ProxyRunnerService();
        private readonly ProxyListService _proxyListService = new ProxyListService();
        private readonly TrayIconService _tray;
        private readonly SynchronizationContext _syncContext;

        private bool _isRunning;
        private bool _isTestingProxy;
        private string _proxyStatus;
        private string _selectedProxy;
        private string _proxyProgress;
        private string _statusText = "● Остановлен";
        private string _toggleText = "▶  Запустить прокси";
        private bool _forceClose;
        private bool _autoStartDone;

        public MainViewModel()
        {
            _syncContext = SynchronizationContext.Current;
            Settings = ConfigService.LoadOrFallback();
            Log = new ObservableCollection<string>();

            ToggleProxyCommand = new RelayCommand(ToggleProxy, () => !IsTestingProxy);
            StopProxyCommand = new RelayCommand(DoStop, () => IsRunning && !IsTestingProxy);
            ExitCommand = new RelayCommand(Exit);
            TestProxyCommand = new RelayCommand(TestProxyList, () => !IsTestingProxy && !string.IsNullOrWhiteSpace(ProxyListUrl));

            _tray = new TrayIconService(ShowMainWindow,
                () => { if (!_runner.IsRunning) DoStart(); },
                () => { if (_runner.IsRunning) DoStop(); },
                Exit);
            _tray.Init();

            _runner.LogReceived += OnLogReceived;
            _runner.ProcessExited += () => OnUi(() =>
            {
                IsRunning = false;
                AddLog("[GUI] Процесс завершился.");
            });

            if (Settings.AutoStartProxy)
                AddLog("[GUI] Автозапуск прокси при старте программы.");

            // Синхронизируем индексы выпадающих списков со значениями из конфига
            _countryIndex = (int)Settings.Country;
            _serverSelectionIndex = (int)Settings.ServerSelectionMode;
            OnPropertyChanged(nameof(CountryIndex));
            OnPropertyChanged(nameof(ServerSelectionIndex));

            // Сохраняем конфиг при изменении любого поля
            Settings.PropertyChanged += OnSettingsChanged;
        }

        private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppSettings.Country)) return;
            if (e.PropertyName == nameof(AppSettings.ServerSelectionMode)) return;
            Save();
        }

        public AppSettings Settings { get; }
        public ObservableCollection<string> Log { get; }

        public bool IsRunning
        {
            get => _isRunning;
            set { if (Set(ref _isRunning, value)) UpdateStatus(); }
        }

        public bool IsTestingProxy
        {
            get => _isTestingProxy;
            set
            {
                if (Set(ref _isTestingProxy, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        public string ProxyStatus { get => _proxyStatus; set => Set(ref _proxyStatus, value); }
        public string SelectedProxy { get => _selectedProxy; set => Set(ref _selectedProxy, value); }
        public string ProxyProgress { get => _proxyProgress; set => Set(ref _proxyProgress, value); }
        public string StatusText { get => _statusText; set => Set(ref _statusText, value); }
        public string ToggleText { get => _toggleText; set => Set(ref _toggleText, value); }

        public string ProxyListUrl
        {
            get => Settings.ProxyListUrl;
            set { Settings.ProxyListUrl = value; Save(); }
        }

        public bool UseProxyList
        {
            get => Settings.UseProxyList;
            set { Settings.UseProxyList = value; Save(); }
        }

        public int ProxyTestTimeoutMs
        {
            get => Settings.ProxyTestTimeoutMs;
            set { Settings.ProxyTestTimeoutMs = value; Save(); }
        }

        public int ProxyTestConcurrency
        {
            get => Settings.ProxyTestConcurrency;
            set { Settings.ProxyTestConcurrency = value; Save(); }
        }

        public bool AutoStartProxy
        {
            get => Settings.AutoStartProxy;
            set { Settings.AutoStartProxy = value; Save(); }
        }

        public bool AutoStartWithWindows
        {
            get => Settings.AutoStartWithWindows;
            set
            {
                Settings.AutoStartWithWindows = value;
                ApplyWinAutostart(value);
                Save();
            }
        }

        public RelayCommand ToggleProxyCommand { get; }
        public RelayCommand StopProxyCommand { get; }
        public RelayCommand ExitCommand { get; }
        public RelayCommand TestProxyCommand { get; }

        /// <summary>Список регионов для выпадающего списка.</summary>
        public string[] Countries { get; } = { "EU", "AS", "AM" };

        private int _countryIndex;
        /// <summary>Индекс выбранного региона (связан с Settings.Country).</summary>
        public int CountryIndex
        {
            get => _countryIndex;
            set
            {
                if (Set(ref _countryIndex, value))
                {
                    Settings.Country = (CountryRegion)value;
                    Save();
                }
            }
        }

        /// <summary>Список политик выбора сервера.</summary>
        public string[] ServerSelections { get; } = { "first", "random", "fastest" };

        private int _serverSelectionIndex;
        /// <summary>Индекс выбранной политики (связан с Settings.ServerSelectionMode).</summary>
        public int ServerSelectionIndex
        {
            get => _serverSelectionIndex;
            set
            {
                if (Set(ref _serverSelectionIndex, value))
                {
                    Settings.ServerSelectionMode = (ServerSelectionPolicy)value;
                    Save();
                }
            }
        }
    }
}
