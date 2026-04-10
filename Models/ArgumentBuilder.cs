using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperaProxyGUI.Models
{
    /// <summary>
    /// Сборка аргументов командной строки opera-proxy.
    /// </summary>
    public static class ArgumentBuilder
    {
        /// <summary>
        /// Формирует строку аргументов. Если apiProxy указан — он добавляется как -api-proxy.
        /// </summary>
        public static string Build(AppSettings c, string apiProxy)
        {
            var sb = new StringBuilder();
            void App(string s) { sb.Append(s); sb.Append(' '); }

            App($"-bind-address {c.BindAddress}");
            if (c.SocksMode) App("-socks-mode");
            App($"-bootstrap-dns {c.BootstrapDns}");
            App($"-country {c.Country}");
            App($"-verbosity {c.Verbosity}");

            if (c.UseApiUserAgent && !string.IsNullOrWhiteSpace(c.ApiUserAgent))
                App($"-api-user-agent \"{c.ApiUserAgent}\"");
            if (c.UseFakeSni && !string.IsNullOrWhiteSpace(c.FakeSni))
                App($"-fake-SNI {c.FakeSni}");
            if (c.UseInitRetries)
                App($"-init-retries {c.InitRetries}");
            if (c.UseInitRetryInterval && !string.IsNullOrWhiteSpace(c.InitRetryInterval))
                App($"-init-retry-interval {c.InitRetryInterval}");
            if (c.UseServerSelection)
                App($"-server-selection {c.ServerSelectionMode}");
            if (c.UseServerSelectionDlLimit)
                App($"-server-selection-dl-limit {c.ServerSelectionDlLimit}");
            if (c.UseServerSelectionTimeout && !string.IsNullOrWhiteSpace(c.ServerSelectionTimeout))
                App($"-server-selection-timeout {c.ServerSelectionTimeout}");
            if (c.UseServerSelectionTestUrl && !string.IsNullOrWhiteSpace(c.ServerSelectionTestUrl))
                App($"-server-selection-test-url {c.ServerSelectionTestUrl}");
            if (c.UseApiAddress && !string.IsNullOrWhiteSpace(c.ApiAddress))
                App($"-api-address {c.ApiAddress}");
            if (c.UseApiClientType && !string.IsNullOrWhiteSpace(c.ApiClientType))
                App($"-api-client-type {c.ApiClientType}");
            if (c.UseApiClientVersion && !string.IsNullOrWhiteSpace(c.ApiClientVersion))
                App($"-api-client-version \"{c.ApiClientVersion}\"");
            if (c.UseApiLogin && !string.IsNullOrWhiteSpace(c.ApiLogin))
                App($"-api-login {c.ApiLogin}");
            if (c.UseApiPassword && !string.IsNullOrWhiteSpace(c.ApiPassword))
                App($"-api-password {c.ApiPassword}");
            if (!string.IsNullOrWhiteSpace(apiProxy))
                App($"-api-proxy {apiProxy}");
            else if (c.UseApiProxy && !string.IsNullOrWhiteSpace(c.ApiProxy))
                App($"-api-proxy {c.ApiProxy}");
            if (c.UseCafile && !string.IsNullOrWhiteSpace(c.Cafile))
                App($"-cafile \"{c.Cafile}\"");
            if (c.UseProxy && !string.IsNullOrWhiteSpace(c.Proxy))
                App($"-proxy {c.Proxy}");
            if (c.UseOverrideProxyAddress && !string.IsNullOrWhiteSpace(c.OverrideProxyAddress))
                App($"-override-proxy-address {c.OverrideProxyAddress}");
            if (c.UseTimeout && !string.IsNullOrWhiteSpace(c.Timeout))
                App($"-timeout {c.Timeout}");
            if (c.UseRefresh && !string.IsNullOrWhiteSpace(c.Refresh))
                App($"-refresh {c.Refresh}");
            if (c.UseRefreshRetry && !string.IsNullOrWhiteSpace(c.RefreshRetry))
                App($"-refresh-retry {c.RefreshRetry}");

            return sb.ToString().TrimEnd();
        }
    }
}
