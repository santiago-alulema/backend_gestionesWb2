using gestiones_backend.Context;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Entity;
using gestiones_backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace gestiones_backend.Services
{
    public class ReportesEmpresaService : IReportesEmpresaService
    {
        private readonly DataContext _context;
        private readonly ILogger<ReportesEmpresaService> _logger;
        private readonly IAuthenticationService _authService;

        public ReportesEmpresaService(DataContext context, 
                                      ILogger<ReportesEmpresaService> logger,
                                      IAuthenticationService authService)
        {
            _context = context;
            _logger = logger;
            _authService = authService;
        }
        public async Task<IEnumerable<ReporteEmpresaDto>> ObtenerReportePorEmpresaMesActual(string FechaInicio, string FechaFin)
        {
            try
            {
                Usuario usuario = _authService.GetCurrentUser();

                // Rango base en UTC (inicio de día y fin de día)
                DateTime fechaInicio = !string.IsNullOrWhiteSpace(FechaInicio)
                    ? DateTime.SpecifyKind(DateTime.Parse(FechaInicio).Date, DateTimeKind.Utc)                              // 00:00
                    : DateTime.UtcNow.Date.AddDays(-30);

                DateTime fechaFin = !string.IsNullOrWhiteSpace(FechaFin)
                    ? DateTime.SpecifyKind(DateTime.Parse(FechaFin).Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc)         // 23:59:59.9999999
                    : DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);

                // Derivados para columnas DateOnly (como FechaPago)
                var fechaInicioOnly = DateOnly.FromDateTime(fechaInicio);
                var fechaFinOnly = DateOnly.FromDateTime(fechaFin);

                IQueryable<Deuda> query = _context.Deudas
                    .Include(d => d.IdDeudorNavigation)
                        .ThenInclude(deudor => deudor.Usuario)
                    .Include(d => d.Pagos)
                    .Include(d => d.CompromisosPagos)
                    .Include(d => d.Gestiones);

                if (!string.Equals(usuario.Rol, "admin", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(d => d.IdDeudorNavigation.IdUsuario == usuario.IdUsuario);
                }

                var reporte = await query
                    .Where(d => !string.IsNullOrEmpty(d.Empresa))
                    .GroupBy(d => d.Empresa)
                    .Select(g => new ReporteEmpresaDto
                    {
                        Empresa = g.Key,

                        // DateTime vs DateTime
                        CantidadGestiones = g.SelectMany(d => d.Gestiones)
                            .Count(ges => ges.FechaGestion >= fechaInicio && ges.FechaGestion <= fechaFin),

                        // DateTime vs DateTime
                        CantidadCompromisosPago = g.SelectMany(d => d.CompromisosPagos)
                            .Count(c => c.FechaRegistro >= fechaInicio && c.FechaRegistro <= fechaFin),

                        // DateOnly vs DateOnly (NO uses Parse/ToString)
                        CantidadPagos = g.SelectMany(d => d.Pagos)
                            .Count(p => p.FechaPago >= fechaInicioOnly && p.FechaPago <= fechaFinOnly),

                        // Usa tam-bién FechaPago para coherencia con CantidadPagos
                        ValorTotalPagos = g.SelectMany(d => d.Pagos)
                            .Where(p => p.FechaPago >= fechaInicioOnly && p.FechaPago <= fechaFinOnly)
                            .Sum(p => (decimal?)p.MontoPagado) ?? 0m,

                        // DateTime vs DateTime
                        ValorTotalCompromisos = g.SelectMany(d => d.CompromisosPagos)
                            .Where(c => c.FechaCompromiso >= fechaInicioOnly && c.FechaCompromiso <= fechaFinOnly)
                            .Sum(c => (decimal?)c.MontoComprometido) ?? 0m
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