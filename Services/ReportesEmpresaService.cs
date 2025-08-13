using gestiones_backend.Context;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace gestiones_backend.Services
{
    public class ReportesEmpresaService : IReportesEmpresaService
    {
        private readonly DataContext _context;
        private readonly ILogger<ReportesEmpresaService> _logger;

        public ReportesEmpresaService(DataContext context, ILogger<ReportesEmpresaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ReporteEmpresaDto>> ObtenerReportePorEmpresaMesActual()
        {
            try
            {
                // Obtener fechas en UTC
                var primerDiaMes = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var ultimoDiaMes = new DateTime(primerDiaMes.Year, primerDiaMes.Month,
                                              DateTime.DaysInMonth(primerDiaMes.Year, primerDiaMes.Month),
                                              23, 59, 59, DateTimeKind.Utc);

                // Convertir a DateOnly para comparaciones que lo requieran
                var fechaInicioDateOnly = DateOnly.FromDateTime(primerDiaMes.ToLocalTime());
                var fechaFinDateOnly = DateOnly.FromDateTime(ultimoDiaMes.ToLocalTime());

                var reporte = await _context.Deudas
                    .Where(d => !string.IsNullOrEmpty(d.Empresa))
                    .GroupBy(d => d.Empresa)
                    .Select(g => new ReporteEmpresaDto
                    {
                        Empresa = g.Key,
                        CantidadGestiones = g.SelectMany(d => d.Gestiones)
                            .Count(ges => ges.FechaGestion >= primerDiaMes &&
                                        ges.FechaGestion <= ultimoDiaMes),
                        CantidadCompromisosPago = g.SelectMany(d => d.CompromisosPagos)
                            .Count(c => c.FechaCompromiso >= fechaInicioDateOnly &&
                                      c.FechaCompromiso <= fechaFinDateOnly),
                        CantidadPagos = g.SelectMany(d => d.Pagos)
                            .Count(p => p.FechaPago.HasValue &&
                                      DateOnly.FromDateTime(p.FechaPago.Value.ToDateTime(TimeOnly.MinValue)) >= fechaInicioDateOnly &&
                                      DateOnly.FromDateTime(p.FechaPago.Value.ToDateTime(TimeOnly.MinValue)) <= fechaFinDateOnly),
                        ValorTotalPagos = g.SelectMany(d => d.Pagos)
                            .Where(p => p.FechaPago.HasValue &&
                                      DateOnly.FromDateTime(p.FechaPago.Value.ToDateTime(TimeOnly.MinValue)) >= fechaInicioDateOnly &&
                                      DateOnly.FromDateTime(p.FechaPago.Value.ToDateTime(TimeOnly.MinValue)) <= fechaFinDateOnly)
                            .Sum(p => p.MontoPagado),
                        ValorTotalCompromisos = g.SelectMany(d => d.CompromisosPagos)
                            .Where(c => c.FechaCompromiso >= fechaInicioDateOnly &&
                                      c.FechaCompromiso <= fechaFinDateOnly)
                            .Sum(c => c.MontoComprometido)
                    })
                    .OrderByDescending(r => r.CantidadGestiones)
                    .ToListAsync();

                return reporte;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte por empresa del mes actual");
                throw;
            }
        }

    }
}