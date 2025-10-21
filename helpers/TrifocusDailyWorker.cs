using DocumentFormat.OpenXml.ExtendedProperties;
using gestiones_backend.Interfaces;

namespace gestiones_backend.helpers
{
    public class TrifocusDailyWorker : BackgroundService
    {
        private readonly ILogger<TrifocusDailyWorker> _logger;
        private readonly ITrifocusExcelUploader _uploader;

        public TrifocusDailyWorker(
            ILogger<TrifocusDailyWorker> logger,
            ITrifocusExcelUploader uploader)
        {
            _logger = logger;
            _uploader = uploader;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);
                var next = new DateTime(now.Year, now.Month, now.Day, 4, 0, 0, now.Kind);
                if (now > next) next = next.AddDays(1);

                var delay = next - now;
                _logger.LogInformation("Esperando {Delay} para exportar y subir Trifocus...", delay);
                await Task.Delay(delay, stoppingToken);

                try
                {
                    var path = await _uploader.GenerateAndUploadAsync(stoppingToken);
                    _logger.LogInformation("✅ Exportado y subido: {Path}", path);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error en GenerateAndUploadAsync()");
                }
            }
        }
    }
}
