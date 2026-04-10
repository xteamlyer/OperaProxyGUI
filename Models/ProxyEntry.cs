using System;

namespace OperaProxyGUI.Models
{
    /// <summary>
    /// Одна строка из списка прокси: socks4://host:port, socks5://host:port, http(s)://host:port
    /// </summary>
    public class ProxyEntry
    {
        public string Raw { get; }
        public string Scheme { get; }
        public string Host { get; }
        public int Port { get; }
        public bool Valid { get; }

        /// <summary>
        /// Подходит ли прокси для аргумента -api-proxy.
        /// Бинарник opera-proxy не поддерживает socks4/socks4a — отфильтровываем их.
        /// </summary>
        public bool SupportedForApiProxy { get; }

        private ProxyEntry(string raw, string scheme, string host, int port, bool valid, bool supported)
        {
            Raw = raw; Scheme = scheme; Host = host; Port = port;
            Valid = valid; SupportedForApiProxy = supported;
        }

        public static ProxyEntry Parse(string line)
        {
            var raw = (line ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(raw))
                return new ProxyEntry(raw, null, null, 0, valid: false, supported: false);

            // Поддержка «host:port» без схемы — трактуем как socks5
            var input = raw;
            var scheme = "socks5";
            foreach (var s in new[] { "socks5://", "socks4://", "socks4a://", "https://", "http://" })
            {
                if (input.StartsWith(s, StringComparison.OrdinalIgnoreCase))
                {
                    input = input.Substring(s.Length);
                    scheme = s.Substring(0, s.Length - 3).ToLowerInvariant();
                    break;
                }
            }

            // Отрезаем user:pass@ и завершающий путь
            var at = input.IndexOf('@');
            if (at >= 0) input = input.Substring(at + 1);
            var slash = input.IndexOf('/');
            if (slash >= 0) input = input.Substring(0, slash);

            var colon = input.LastIndexOf(':');
            if (colon < 0 || !int.TryParse(input.Substring(colon + 1), out int port) || port <= 0 || port > 65535)
                return new ProxyEntry(raw, null, null, 0, valid: false, supported: false);

            var host = input.Substring(0, colon).Trim('[', ']', ' ');
            if (string.IsNullOrEmpty(host))
                return new ProxyEntry(raw, null, null, 0, valid: false, supported: false);

            // opera-proxy не поддерживает socks4/socks4a для -api-proxy
            var supported = scheme != "socks4" && scheme != "socks4a";
            return new ProxyEntry(raw, scheme, host, port, valid: true, supported: supported);
        }

        /// <summary>Строка в формате, подходящем для аргумента -api-proxy.</summary>
        public override string ToString() => Valid ? $"{Scheme}://{Host}:{Port}" : Raw;
    }
}
