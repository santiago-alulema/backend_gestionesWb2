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
                var usuario = _authService.GetCurrentUser();
                bool isAdmin = string.Equals(usuario.Rol, "admin", StringComparison.OrdinalIgnoreCase);

                var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil");

                DateTime startLocal = !string.IsNullOrWhiteSpace(FechaInicio)
                    ? DateTime.SpecifyKind(DateTime.Parse(FechaInicio), DateTimeKind.Unspecified)
                    : DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc); // solo para derivar la fecha de "hoy"
                                                                               // Tomamos solo la fecha
                var startLocalDate = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(startLocal.Kind == DateTimeKind.Utc ? startLocal : TimeZoneInfo.ConvertTimeToUtc(startLocal, tz), tz));
                if (string.IsNullOrWhiteSpace(FechaInicio))
                    startLocalDate = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz)).AddDays(-30);

                DateTime endLocal = !string.IsNullOrWhiteSpace(FechaFin)
                    ? DateTime.SpecifyKind(DateTime.Parse(FechaFin), DateTimeKind.Unspecified)
                    : DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                var endLocalDate = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(endLocal.Kind == DateTimeKind.Utc ? endLocal : TimeZoneInfo.ConvertTimeToUtc(endLocal, tz), tz));

                var reporte = await _context.Deudas
                    .Where(d => !string.IsNullOrEmpty(d.Empresa))
                    .GroupBy(d => d.Empresa)
                    .Select(g => new ReporteEmpresaDto
                    {
                        Empresa = g.Key,

                        // Gestiones: asumo FechaGestion es DateTime (UTC). Si es date, cámbialo a DateOnly igual que abajo.
                        CantidadGestiones = g.SelectMany(d => d.Gestiones)
                            .Count(ges =>
                                // si es DateTime UTC en BD, compara por rango de fechas locales (convirtiendo a DateOnly)
                                DateOnly.FromDateTime(ges.FechaGestion) >= startLocalDate &&
                                DateOnly.FromDateTime(ges.FechaGestion) <= endLocalDate &&
                                (isAdmin || ges.IdUsuarioGestiona == usuario.IdUsuario)
                            ),

                        // Compromisos: DateOnly en BD
                        CantidadCompromisosPago = g.SelectMany(d => d.CompromisosPagos)
                            .Count(c =>
                                c.FechaCompromiso >= startLocalDate &&
                                c.FechaCompromiso <= endLocalDate &&
                                (isAdmin || c.IdUsuario == usuario.IdUsuario)
                            ),

                        // Pagos: DateOnly en BD
                        CantidadPagos = g.SelectMany(d => d.Pagos)
                            .Count(p =>
                                p.FechaPago >= startLocalDate &&
                                p.FechaPago <= endLocalDate &&
                                (isAdmin || p.IdUsuario == usuario.IdUsuario)
                            ),

                        ValorTotalPagos = g.SelectMany(d => d.Pagos)
                            .Where(p =>
                                p.FechaPago >= startLocalDate &&
                                p.FechaPago <= endLocalDate &&
                                (isAdmin || p.IdUsuario == usuario.IdUsuario)
                            )
                            .Sum(p => (decimal?)p.MontoPagado) ?? 0m,

                        ValorTotalCompromisos = g.SelectMany(d => d.CompromisosPagos)
                            .Where(c =>
                                c.FechaCompromiso >= startLocalDate &&
                                c.FechaCompromiso <= endLocalDate &&
                                (isAdmin || c.IdUsuario == usuario.IdUsuario)
                            )
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