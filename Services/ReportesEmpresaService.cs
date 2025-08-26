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

                // Manejo de fechas con valores por defecto (últimos 30 días si no se especifican)
                DateTime fechaInicio = !string.IsNullOrEmpty(FechaInicio)
                    ? DateTime.Parse(FechaInicio).ToUniversalTime()
                    : DateTime.UtcNow.AddDays(-30).Date;

                DateTime fechaFin = !string.IsNullOrEmpty(FechaFin)
                    ? DateTime.Parse(FechaFin).ToUniversalTime().AddDays(1).AddTicks(-1)
                    : DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);

                var query = _context.Deudas.Include(d => d.IdDeudorNavigation)
                                           .ThenInclude(deudor => deudor.Usuario).AsQueryable();

                if (usuario.Rol.ToLower() != "admin")
                {
                    query = query.Where(d => d.IdDeudorNavigation.IdUsuario == usuario.IdUsuario);
                }

                var reporte = await query
                    .Where(d => !string.IsNullOrEmpty(d.Empresa))
                    .GroupBy(d => d.Empresa)
                    .Select(g => new ReporteEmpresaDto
                    {
                        Empresa = g.Key,
                        CantidadGestiones = g.SelectMany(d => d.Gestiones)
                            .Count(ges => ges.FechaGestion >= fechaInicio &&
                                        ges.FechaGestion <= fechaFin),
                        CantidadCompromisosPago = g.SelectMany(d => d.CompromisosPagos)
                            .Count(c => c.FechaRegistro >= fechaInicio &&
                                      c.FechaRegistro <= fechaFin),
                        CantidadPagos = g.SelectMany(d => d.Pagos)
                            .Count(p => p.FechaRegistro >= fechaInicio &&
                                      p.FechaRegistro <= fechaFin),
                        ValorTotalPagos = g.SelectMany(d => d.Pagos)
                            .Where(p => p.FechaRegistro >= fechaInicio &&
                                      p.FechaRegistro <= fechaFin)
                            .Sum(p => p.MontoPagado),
                        ValorTotalCompromisos = g.SelectMany(d => d.CompromisosPagos)
                            .Where(c => c.FechaRegistro >= fechaInicio &&
                                      c.FechaRegistro <= fechaFin)
                            .Sum(c => c.MontoComprometido )
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