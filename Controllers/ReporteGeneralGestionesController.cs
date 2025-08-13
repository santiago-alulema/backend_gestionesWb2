using gestiones_backend.Context;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReporteGeneralGestionesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IReportesEmpresaService _reportesEmpresaService;
        public ReporteGeneralGestionesController(
       DataContext context,
       IReportesEmpresaService reportesEmpresaService)
        {
            _context = context;
            _reportesEmpresaService = reportesEmpresaService;
        }

        [HttpGet("gestiones-por-usuario")]
        public async Task<ActionResult<IEnumerable<object>>> GetGestionesPorUsuario()
        {
            // Crear fechas con Kind=Utc
            var fechaInicio = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var fechaFin = new DateTime(fechaInicio.Year, fechaInicio.Month,
                                      DateTime.DaysInMonth(fechaInicio.Year, fechaInicio.Month),
                                      23, 59, 59, DateTimeKind.Utc);

            var resultado = await _context.Usuarios
                .Select(u => new
                {
                    nombreUsuario = u.NombreUsuario,
                    CantidadGestiones = u.Gestiones
                        .Count(g => g.FechaGestion >= fechaInicio && g.FechaGestion <= fechaFin),
                    valorTotal = u.Gestiones
                        .Where(g => g.FechaGestion >= fechaInicio && g.FechaGestion <= fechaFin)
                        .Sum(g => g.IdDeudaNavigation.MontoOriginal)
                })
                .Where(r => r.CantidadGestiones > 0)
                .OrderByDescending(r => r.CantidadGestiones)
                .ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("gestiones-pago-por-usuario")]
        public async Task<ActionResult<IEnumerable<object>>> GetGestionesPagoPorUsuario()
        {
            var fechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            var resultado = await _context.Usuarios
                .Select(u => new
                {
                    nombreUsuario = u.NombreUsuario,
                    Pagos = u.Deudores
                        .SelectMany(d => d.Deuda)
                        .SelectMany(d => d.Pagos)
                        .Where(p => p.FechaPago != null &&
                                   DateOnly.FromDateTime(p.FechaPago.Value.ToDateTime(TimeOnly.MinValue)) >= DateOnly.FromDateTime(fechaInicio) &&
                                   DateOnly.FromDateTime(p.FechaPago.Value.ToDateTime(TimeOnly.MinValue)) <= DateOnly.FromDateTime(fechaFin))
                        .ToList()
                })
                .Select(r => new
                {
                    r.nombreUsuario,
                    CantidadPagos = r.Pagos.Count,
                    valorTotal = r.Pagos.Sum(p => p.MontoPagado)
                })
                .Where(r => r.CantidadPagos > 0)
                .OrderByDescending(r => r.CantidadPagos)
                .ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("compromisos-pago-por-usuario")]
        public async Task<ActionResult<IEnumerable<object>>> GetCompromisosPagoPorUsuario()
        {
            var fechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            var resultado = await _context.Usuarios
                .Select(u => new
                {
                    nombreUsuario = u.NombreUsuario,
                    Compromisos = u.CompromisosPagos
                        .Where(c => c.FechaCompromiso >= DateOnly.FromDateTime(fechaInicio) &&
                                   c.FechaCompromiso <= DateOnly.FromDateTime(fechaFin))
                        .ToList()
                })
                .Select(r => new
                {
                    r.nombreUsuario,
                    CantidadCompromisos = r.Compromisos.Count,
                    valorTotal = r.Compromisos.Sum(c => c.MontoComprometido)
                })
                .Where(r => r.CantidadCompromisos > 0)
                .OrderByDescending(r => r.CantidadCompromisos)
                .ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("reporte-por-empresa-mes-actual")]
        public async Task<ActionResult<IEnumerable<ReporteEmpresaDto>>> GetReportePorEmpresaMesActual()
        {
                var reporte = await _reportesEmpresaService.ObtenerReportePorEmpresaMesActual();
                return Ok(reporte);
        }

    }
}
