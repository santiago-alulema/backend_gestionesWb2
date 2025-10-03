using gestiones_backend.Services;

namespace gestiones_backend.helpers
{
    public class MarcarIncumplidosDailyWorker : BackgroundService
    {
        private readonly ILogger<MarcarIncumplidosDailyWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public MarcarIncumplidosDailyWorker(
            ILogger<MarcarIncumplidosDailyWorker> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tz = GetEcuadorTimeZone();

            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = GetDelayUntilNextRun(tz, hour: 2, minute: 0); // 02:00
                _logger.LogInformation("Job 'MarcarIncumplidos' esperando {Delay} hasta la próxima ejecución.", delay);

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<CompromisosPagoService>();

                    var updated = await service.MarcarIncumplidosVencidosAsync();
                    _logger.LogInformation("Job 'MarcarIncumplidos' ejecutado. Filas actualizadas: {Updated}", updated);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error ejecutando job 'MarcarIncumplidos'.");
                }
            }
        }

        private static TimeSpan GetDelayUntilNextRun(TimeZoneInfo tz, int hour, int minute)
        {
            var nowUtc = DateTime.UtcNow;
            var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, tz);
            var next = new DateTime(nowLocal.Year, nowLocal.Month, nowLocal.Day, hour, minute, 0);

            if (nowLocal >= next)
                next = next.AddDays(1);

            var delay = next - nowLocal;
            return delay < TimeSpan.Zero ? TimeSpan.Zero : delay;
        }

        private static TimeZoneInfo GetEcuadorTimeZone()
        {
            // Linux / contenedores
            try { return TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil"); } catch { }
            // Windows Server / IIS
            try { return TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"); } catch { }
            // Fallback: UTC (no ideal, pero evita romper)
            return TimeZoneInfo.Utc;
        }
    }
}