using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using OperaProxyGUI.Models;

namespace OperaProxyGUI.Services
{
    /// <summary>
    /// Сохраняет/загружает конфиг в JSON рядом с exe.
    /// Использует System.Text.Json (.NET 8).
    /// </summary>
    public static class ConfigService
    {
        private static readonly string ConfigPath = Path.Combine(App.AppDirectory, "opera-proxy-gui.json");

        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public static void Save(AppSettings settings)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings, Options);
                File.WriteAllText(ConfigPath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ConfigService.Save error: " + ex.Message);
            }
        }

        public static AppSettings Load() => LoadOrFallback();

        public static AppSettings LoadOrFallback()
        {
            try
            {
                if (!File.Exists(ConfigPath)) return new AppSettings();
                var json = File.ReadAllText(ConfigPath, Encoding.UTF8);
                return JsonSerializer.Deserialize<AppSettings>(json, Options) ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }
    }
}
