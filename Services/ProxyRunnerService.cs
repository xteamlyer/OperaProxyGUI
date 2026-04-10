using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OperaProxyGUI.Models;

namespace OperaProxyGUI.Services
{
    /// <summary>
    /// Запуск/остановка процесса opera-proxy, чтение stdout/stderr.
    /// Бинарник извлекается из embedded resources во временный файл.
    /// </summary>
    public sealed class ProxyRunnerService : IDisposable
    {
        private const string BinaryName = "opera-proxy.windows-amd64.exe";
        private const string ResourceName = "OperaProxyGUI.Resources.opera-proxy.windows-amd64.exe";

        private Process _process;
        private string _tempExePath;

        public bool IsRunning => _process != null && !_process.HasExited;

        public event Action<string> LogReceived;
        public event Action ProcessExited;

        /// <summary>Возвращает путь к бинарнику: embedded resource → папка bin → рядом с exe.</summary>
        private string EnsureBinary()
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream(ResourceName))
            {
                if (stream != null)
                {
                    _tempExePath = Path.Combine(Path.GetTempPath(), BinaryName);
                    using (var fs = new FileStream(_tempExePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        stream.CopyTo(fs);
                    return _tempExePath;
                }
            }

            var local = Path.Combine(App.AppDirectory, BinaryName);
            if (File.Exists(local)) return local;

            var bin = Path.Combine(App.AppDirectory, "bin", BinaryName);
            if (File.Exists(bin)) return bin;

            throw new FileNotFoundException(
                $"Бинарник не найден ни как embedded resource, ни в папке bin, ни рядом с exe.\n" +
                $"Добавьте файл {BinaryName} в проект как Embedded Resource.");
        }

        public Task StartAsync(AppSettings config, string apiProxy)
        {
            if (IsRunning)
                throw new InvalidOperationException("Процесс уже запущен.");

            var exePath = EnsureBinary();
            var args = ArgumentBuilder.Build(config, apiProxy);

            var psi = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            };

            _process = new Process { StartInfo = psi, EnableRaisingEvents = true };
            _process.OutputDataReceived += (s, e) => { if (e.Data != null) LogReceived?.Invoke(e.Data); };
            _process.ErrorDataReceived += (s, e) => { if (e.Data != null) LogReceived?.Invoke("[ERR] " + e.Data); };
            _process.Exited += (s, e) => ProcessExited?.Invoke();

            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            LogReceived?.Invoke($"[GUI] Запущен: {Path.GetFileName(exePath)} {args}");
            return Task.CompletedTask;
        }

        public void Stop()
        {
            if (_process != null && !_process.HasExited)
            {
                try { _process.Kill(); _process.WaitForExit(3000); }
                catch { }
            }
            _process?.Dispose();
            _process = null;

            if (_tempExePath != null && File.Exists(_tempExePath))
            {
                try { File.Delete(_tempExePath); } catch { }
                _tempExePath = null;
            }

            LogReceived?.Invoke("[GUI] Процесс остановлен.");
        }

        public void Dispose() => Stop();
    }
}
