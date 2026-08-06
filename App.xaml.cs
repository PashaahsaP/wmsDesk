using Newtonsoft.Json;
using Serilog;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace WmsDesk
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var jsonIp = File.ReadAllText("config.json");
            var setting = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonIp);
            var ip = setting["Ip"];
            base.OnStartup(e);

            // Настройка Serilog для .NET Framework
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                // Автоматически берет имя ПК из Windows (Environment.MachineName)
                .Enrich.WithMachineName()
                // Отправка POST-запросов на ваш Node.js сервер
                .WriteTo.Http(
                    requestUri: $"http://{ip}:3000/api/logs", // адрес вашего Node сервера
                    queueLimitBytes: 5 * 1024 * 1024 // 5 МБ буфер на диске, если сервер недоступен
                )
                .CreateLogger();

        }

        protected override void OnExit(ExitEventArgs e)
        {
            // КРИТИЧЕСКИ ВАЖНО для .NET Framework: 
            // Сбрасывает все недоотправленные HTTP-запросы из памяти на сервер перед выходом
            Log.CloseAndFlush();

            base.OnExit(e);
        }
    }

}
