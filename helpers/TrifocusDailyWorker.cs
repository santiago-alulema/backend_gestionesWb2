using DocumentFormat.OpenXml.ExtendedProperties;
using gestiones_backend.Interfaces;

namespace gestiones_backend.helpers
{
    public class TrifocusDailyWorker : BackgroundService
    {
        private readonly ILogger<TrifocusDailyWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public TrifocusDailyWorker(
            ILogger<TrifocusDailyWorker> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);
                var next = new DateTime(now.Year, now.Month, now.Day, 23, 40, 0, now.Kind);
                if (now > next) next = next.AddDays(1);

                var delay = next - now;
               
                _logger.LogInformation("Esperando {Delay} para exportar y subir Trifocus...", delay);
                await Task.Delay(delay, stoppingToken);

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var uploader = scope.ServiceProvider.GetRequiredService<ITrifocusExcelUploader>();

                    var path = await uploader.GenerateAndUploadAsync(stoppingToken);
                    _logger.LogInformation("✅ Exportado y subido: {Path}", uploader);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error en GenerateAndUploadAsync()");
                }
            }
        }
    }
}
