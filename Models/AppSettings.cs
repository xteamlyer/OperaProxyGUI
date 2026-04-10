using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OperaProxyGUI.Models
{
    /// <summary>
    /// Настройки приложения. Сериализуются в JSON рядом с exe.
    /// </summary>
    public class AppSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(name);
            return true;
        }
        public string BindAddress
        {
            get => _bindAddress;
            set => Set(ref _bindAddress, value);
        }
        private string _bindAddress = "127.0.0.1:18080";

        public bool SocksMode
        {
            get => _socksMode;
            set => Set(ref _socksMode, value);
        }
        private bool _socksMode = true;

        public string BootstrapDns
        {
            get => _bootstrapDns;
            set => Set(ref _bootstrapDns, value);
        }
        private string _bootstrapDns = Defaults.BootstrapDns;

        public CountryRegion Country
        {
            get => _country;
            set => Set(ref _country, value);
        }
        private CountryRegion _country = CountryRegion.EU;

        public int Verbosity
        {
            get => _verbosity;
            set => Set(ref _verbosity, value);
        }
        private int _verbosity = 50;

        // ── Необязательные параметры (передаются только при Use* = true) ──
        public bool UseApiUserAgent { get => _useApiUserAgent; set => Set(ref _useApiUserAgent, value); }
        private bool _useApiUserAgent;
        public string ApiUserAgent { get => _apiUserAgent; set => Set(ref _apiUserAgent, value); }
        private string _apiUserAgent = Defaults.UserAgent;

        public bool UseFakeSni { get => _useFakeSni; set => Set(ref _useFakeSni, value); }
        private bool _useFakeSni;
        public string FakeSni { get => _fakeSni; set => Set(ref _fakeSni, value); }
        private string _fakeSni = "m.vk.ru";

        public bool UseInitRetries { get => _useInitRetries; set => Set(ref _useInitRetries, value); }
        private bool _useInitRetries;
        public int InitRetries { get => _initRetries; set => Set(ref _initRetries, value); }
        private int _initRetries = 15;

        public bool UseInitRetryInterval { get => _useInitRetryInterval; set => Set(ref _useInitRetryInterval, value); }
        private bool _useInitRetryInterval;
        public string InitRetryInterval { get => _initRetryInterval; set => Set(ref _initRetryInterval, value); }
        private string _initRetryInterval = "5s";

        public bool UseServerSelection { get => _useServerSelection; set => Set(ref _useServerSelection, value); }
        private bool _useServerSelection;
        public ServerSelectionPolicy ServerSelectionMode { get => _serverSelectionMode; set => Set(ref _serverSelectionMode, value); }
        private ServerSelectionPolicy _serverSelectionMode = ServerSelectionPolicy.fastest;

        public bool UseServerSelectionDlLimit { get => _useServerSelectionDlLimit; set => Set(ref _useServerSelectionDlLimit, value); }
        private bool _useServerSelectionDlLimit;
        public int ServerSelectionDlLimit { get => _serverSelectionDlLimit; set => Set(ref _serverSelectionDlLimit, value); }
        private int _serverSelectionDlLimit = 204800;

        public bool UseServerSelectionTimeout { get => _useServerSelectionTimeout; set => Set(ref _useServerSelectionTimeout, value); }
        private bool _useServerSelectionTimeout;
        public string ServerSelectionTimeout { get => _serverSelectionTimeout; set => Set(ref _serverSelectionTimeout, value); }
        private string _serverSelectionTimeout = "60s";

        public bool UseServerSelectionTestUrl { get => _useServerSelectionTestUrl; set => Set(ref _useServerSelectionTestUrl, value); }
        private bool _useServerSelectionTestUrl;
        public string ServerSelectionTestUrl { get => _serverSelectionTestUrl; set => Set(ref _serverSelectionTestUrl, value); }
        private string _serverSelectionTestUrl = Defaults.TestUrl;

        public bool UseApiAddress { get => _useApiAddress; set => Set(ref _useApiAddress, value); }
        private bool _useApiAddress;
        public string ApiAddress { get => _apiAddress; set => Set(ref _apiAddress, value); }
        private string _apiAddress = "";

        public bool UseApiClientType { get => _useApiClientType; set => Set(ref _useApiClientType, value); }
        private bool _useApiClientType;
        public string ApiClientType { get => _apiClientType; set => Set(ref _apiClientType, value); }
        private string _apiClientType = "se0316";

        public bool UseApiClientVersion { get => _useApiClientVersion; set => Set(ref _useApiClientVersion, value); }
        private bool _useApiClientVersion;
        public string ApiClientVersion { get => _apiClientVersion; set => Set(ref _apiClientVersion, value); }
        private string _apiClientVersion = "Stable 114.0.5282.21";

        public bool UseApiLogin { get => _useApiLogin; set => Set(ref _useApiLogin, value); }
        private bool _useApiLogin;
        public string ApiLogin { get => _apiLogin; set => Set(ref _apiLogin, value); }
        private string _apiLogin = "se0316";

        public bool UseApiPassword { get => _useApiPassword; set => Set(ref _useApiPassword, value); }
        private bool _useApiPassword;
        public string ApiPassword { get => _apiPassword; set => Set(ref _apiPassword, value); }
        private string _apiPassword = Defaults.ApiPassword;

        public bool UseApiProxy { get => _useApiProxy; set => Set(ref _useApiProxy, value); }
        private bool _useApiProxy;
        public string ApiProxy { get => _apiProxy; set => Set(ref _apiProxy, value); }
        private string _apiProxy = "";

        public bool UseCafile { get => _useCafile; set => Set(ref _useCafile, value); }
        private bool _useCafile;
        public string Cafile { get => _cafile; set => Set(ref _cafile, value); }
        private string _cafile = "";

        public bool UseProxy { get => _useProxy; set => Set(ref _useProxy, value); }
        private bool _useProxy;
        public string Proxy { get => _proxy; set => Set(ref _proxy, value); }
        private string _proxy = "";

        public bool UseOverrideProxyAddress { get => _useOverrideProxyAddress; set => Set(ref _useOverrideProxyAddress, value); }
        private bool _useOverrideProxyAddress;
        public string OverrideProxyAddress { get => _overrideProxyAddress; set => Set(ref _overrideProxyAddress, value); }
        private string _overrideProxyAddress = "";

        public bool UseTimeout { get => _useTimeout; set => Set(ref _useTimeout, value); }
        private bool _useTimeout;
        public string Timeout { get => _timeout; set => Set(ref _timeout, value); }
        private string _timeout = "10s";

        public bool UseRefresh { get => _useRefresh; set => Set(ref _useRefresh, value); }
        private bool _useRefresh;
        public string Refresh { get => _refresh; set => Set(ref _refresh, value); }
        private string _refresh = "4h0m0s";

        public bool UseRefreshRetry { get => _useRefreshRetry; set => Set(ref _useRefreshRetry, value); }
        private bool _useRefreshRetry;
        public string RefreshRetry { get => _refreshRetry; set => Set(ref _refreshRetry, value); }
        private string _refreshRetry = "5s";

        // ── Настройки приложения ──────────────────────────────────────────
        public bool AutoStartProxy { get; set; }
        public bool AutoStartWithWindows { get; set; }

        // ── Авто-подбор api-proxy ─────────────────────────────────────────
        /// <summary>URL текстового файла со списком прокси (одна строка — один прокси).</summary>
        public string ProxyListUrl { get; set; } = "";
        /// <summary>Включить авто-подбор прокси для -api-proxy.</summary>
        public bool UseProxyList { get; set; }
        /// <summary>Таймаут проверки одного прокси (мс).</summary>
        public int ProxyTestTimeoutMs { get; set; } = 5000;
        /// <summary>Максимум одновременных проверок.</summary>
        public int ProxyTestConcurrency { get; set; } = 32;
    }
}
