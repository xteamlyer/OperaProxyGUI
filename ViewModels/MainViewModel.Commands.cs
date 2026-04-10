using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using OperaProxyGUI.Services;

namespace OperaProxyGUI.ViewModels
{
    // ── Команды: запуск/остановка прокси ──────────────────────────────────
    public partial class MainViewModel
    {
        private void ToggleProxy()
        {
            if (_runner.IsRunning) DoStop();
            else DoStart();
        }

        private async void DoStart()
        {
            if (IsRunning) return;
            try
            {
                string apiProxy = Settings.ApiProxy;

                // Если включён авто-подбор — сначала находим рабочий прокси
                if (UseProxyList && !string.IsNullOrWhiteSpace(ProxyListUrl))
                {
                    apiProxy = await ResolveApiProxyAsync();
                    if (string.IsNullOrEmpty(apiProxy))
                    {
                        AddLog("[GUI] Рабочий прокси не найден — запуск отменён.");
                        ProxyStatus = "Рабочий прокси не найден — запуск отменён";
                        return;
                    }
                }

                await _runner.StartAsync(Settings, apiProxy);
                IsRunning = true;
            }
            catch (Exception ex)
            {
                AddLog("[ОШИБКА] " + ex.Message);
                MessageBox.Show(ex.Message, "Ошибка запуска", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void DoStop()
        {
            _runner.Stop();
            IsRunning = false;
            CommandManager.InvalidateRequerySuggested();
        }

        // ── Подбор api-proxy ───────────────────────────────────────────────

        private async void TestProxyList()
        {
            if (IsTestingProxy) return;
            try
            {
                var found = await ResolveApiProxyAsync();
                if (found != null)
                    ProxyStatus = $"Найден рабочий прокси: {found}";
                else
                    ProxyStatus = "Рабочий прокси не найден";
            }
            catch (Exception ex)
            {
                ProxyStatus = "Ошибка: " + ex.Message;
                AddLog("[ОШИБКА] " + ex.Message);
            }
            finally
            {
                IsTestingProxy = false;
            }
        }

        private async Task<string> ResolveApiProxyAsync()
        {
            IsTestingProxy = true;
            try
            {
                ProxyStatus = "Скачивание списка прокси…";
                ProxyProgress = string.Empty;
                SelectedProxy = string.Empty;

                var proxies = await _proxyListService.DownloadListAsync(ProxyListUrl, CancellationToken.None);
                AddLog($"[GUI] Список прокси получен: {proxies.Count} подходящих строк " +
                       "(socks4/socks4a не поддерживаются бинарником и отфильтрованы).");

                if (proxies.Count == 0)
                {
                    ProxyProgress = string.Empty;
                    return null;
                }

                ProxyStatus = "Проверка подключений…";
                var progress = new Progress<Tuple<string, int, int>>(p =>
                {
                    ProxyProgress = $"Проверено {p.Item2} из {p.Item3}: {p.Item1}";
                });

                var found = await _proxyListService.FindFirstWorkingAsync(
                    proxies, ProxyTestTimeoutMs, ProxyTestConcurrency, progress, CancellationToken.None);

                if (found != null)
                {
                    SelectedProxy = found.ToString();
                    AddLog($"[GUI] Найден рабочий прокси: {found}");
                    return found.ToString();
                }

                ProxyProgress = string.Empty;
                return null;
            }
            finally
            {
                IsTestingProxy = false;
            }
        }
    }
}
