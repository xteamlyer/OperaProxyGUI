namespace OperaProxyGUI.Models
{
    /// <summary>
    /// Значения по умолчанию для параметров opera-proxy.
    /// </summary>
    public static class Defaults
    {
        public const string UserAgent =
            "Mozilla/5.0 (Linux; Android 13; Pixel 7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Mobile Safari/537.36";

        public const string TestUrl = "https://translate.yandex.ru/icons/favicon.ico";
        public const string ApiPassword = "SILrMEPBmJuhomxWkfm3JalqHX2Eheg1YhlEZiMh8II";

        public const string BootstrapDns =
            "https://1.1.1.3/dns-query,https://8.8.4.4/dns-query,tls://9.9.9.9:853," +
            "https://security.cloudflare-dns.com/dns-query,https://wikimedia-dns.org/dns-query," +
            "https://dns.adguard-dns.com/dns-query,https://dns.quad9.net/dns-query," +
            "https://dns.comss.one/dns-query,https://router.comss.one/dns-query";
    }
}
