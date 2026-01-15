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
                var delay = GetDelayUntilNextRun(tz, hour: 4, minute: 0);

                // ⛳️ Solo para pruebas locales:
                //var ahoraLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
                //var delay = GetDelayUntilNextRun(tz, ahoraLocal.Hour, (ahoraLocal.Minute + 1) % 60);
                _logger.LogInformation("Job 'MarcarIncumplidos' esperando {Delay} hasta la próxima ejecución.", delay);

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (OperationCanceledException) { break; }

                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var folderService = scope.ServiceProvider.GetRequiredService<FolderCleanService>();
                    var sftpService = scope.ServiceProvider.GetRequiredService<SftpDownloadService>();
                    var zipService = scope.ServiceProvider.GetRequiredService<ZipExtractService>();
                    var importSvc = scope.ServiceProvider.GetRequiredService<DeudoresImportService>();
                    var compromisos = scope.ServiceProvider.GetRequiredService<CompromisosPagoService>();

                    _logger.LogInformation("Iniciando proceso de actualización diaria de deudores...");

                    folderService.LimpiarCarpeta();
                    _logger.LogInformation("Carpeta limpiada.");

                    sftpService.DescargarZipsUltimoDia();
                    _logger.LogInformation("Zips descargados.");

                    zipService.DescomprimirZipsUltimo();
                    _logger.LogInformation("Zips descomprimidos.");

                    await importSvc.ImportarDeudoresCompletoAsync();
                    await importSvc.ImportarTelefonosBasicoAsync();
                    importSvc.GrabarTablas();
                    importSvc.ImportarDeudas();
                    importSvc.importarPagos();
                    //_logger.LogInformation("Datos importados correctamente.");

                    // var updated = await compromisos.MarcarIncumplidosVencidosAsync();
                    //  _logger.LogInformation("Job 'MarcarIncumplidos' ejecutado. Filas actualizadas: {Updated}", updated);
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
            if (nowLocal >= next) next = next.AddDays(1);
            var delay = next - nowLocal;
            return delay < TimeSpan.Zero ? TimeSpan.Zero : delay;
        }

        private static TimeZoneInfo GetEcuadorTimeZone()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil"); } catch { }
            try { return TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"); } catch { }
            return TimeZoneInfo.Utc;
        }
    }
}