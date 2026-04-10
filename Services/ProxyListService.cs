using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using OperaProxyGUI.Models;

namespace OperaProxyGUI.Services
{
    /// <summary>
    /// Скачивает список прокси и проверяет их на успешное подключение.
    /// </summary>
    public class ProxyListService
    {
        private static readonly ProxyEntry[] NoProxies = Array.Empty<ProxyEntry>();

        /// <summary>Скачивает текстовый файл со списком прокси и парсит строки.</summary>
        public async Task<IReadOnlyList<ProxyEntry>> DownloadListAsync(string url, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL списка прокси не указан.");

            string text;
            using (var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            })
            using (var client = new HttpClient(handler))
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("OperaProxyGUI/2.0");
                using (var resp = await client.GetAsync(url, HttpCompletionOption.ResponseContentRead, ct))
                {
                    resp.EnsureSuccessStatusCode();
                    text = await resp.Content.ReadAsStringAsync(ct);
                }
            }

            return ParseLines(text);
        }

        /// <summary>Парсит текст списка прокси (поддержка \n, \r\n).</summary>
        public static IReadOnlyList<ProxyEntry> ParseLines(string text)
        {
            if (string.IsNullOrEmpty(text)) return NoProxies;
            return text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                       .Select(l => l.Trim())
                       .Where(l => l.Length > 0 && !l.StartsWith("#"))
                       .Select(ProxyEntry.Parse)
                       .Where(p => p.Valid && p.SupportedForApiProxy)
                       .ToArray();
        }

        /// <summary>
        /// Проверяет прокси на успешное подключение и возвращает первый рабочий.
        /// </summary>
        public async Task<ProxyEntry> FindFirstWorkingAsync(
            IReadOnlyList<ProxyEntry> proxies,
            int timeoutMs,
            int concurrency,
            IProgress<Tuple<string, int, int>> progress = null,
            CancellationToken ct = default)
        {
            if (proxies == null || proxies.Count == 0) return null;
            if (concurrency < 1) concurrency = 8;

            var done = 0;
            var total = proxies.Count;
            var result = new TaskCompletionSource<ProxyEntry>(TaskCreationOptions.RunContinuationsAsynchronously);
            var gate = new SemaphoreSlim(concurrency);

            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(ct))
            {
                var token = cts.Token;

                var tasks = proxies.Select(p => Task.Run(async () =>
                {
                    if (result.Task.IsCompleted) return;
                    await gate.WaitAsync(token).ConfigureAwait(false);
                    try
                    {
                        if (result.Task.IsCompleted) return;
                        bool ok;
                        try
                        {
                            ok = await TestConnectionAsync(p, timeoutMs, token).ConfigureAwait(false);
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }
                        var d = Interlocked.Increment(ref done);
                        progress?.Report(Tuple.Create(p.Raw, d, total));
                        if (ok)
                        {
                            result.TrySetResult(p);
                            cts.Cancel();
                        }
                    }
                    finally
                    {
                        gate.Release();
                    }
                }, token)).ToArray();

                try
                {
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // ожидаемо: отменяем оставшиеся проверки после успеха
                }

                return result.Task.IsCompleted ? await result.Task.ConfigureAwait(false) : null;
            }
        }

        /// <summary>Проверка одного прокси: TCP-подключение к host:port.</summary>
        public static async Task<bool> TestConnectionAsync(ProxyEntry proxy, int timeoutMs, CancellationToken ct)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    await client.ConnectAsync(proxy.Host, proxy.Port).WaitAsync(TimeSpan.FromMilliseconds(timeoutMs), ct);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
