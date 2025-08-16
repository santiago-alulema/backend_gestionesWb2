using gestiones_backend.Context;
using gestiones_backend.DbConn;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.ComponentModel;
using System.Data;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReporteGeneralGestionesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IReportesEmpresaService _reportesEmpresaService;
        private readonly IConfiguration Configuration;

        public ReporteGeneralGestionesController(
       DataContext context,
       IReportesEmpresaService reportesEmpresaService,
       IConfiguration config)
        {
            _context = context;
            _reportesEmpresaService = reportesEmpresaService;
            Configuration = config;
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
                        .Sum(g => g.IdDeudaNavigation.SaldoDeulda)
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


        [HttpGet("reporte-general-gestiones/{fechaInicio}/{fechaFin}/{tipoReporte}/{cliente}")]
        public IActionResult GetReporteGeneral(string fechaInicio, string fechaFin, string tipoReporte, string cliente = null)
        {
            string consulta = "";
            string filtroCliente = cliente == "-SP-" ? "": @$"d2.""Nombre"" like '%{cliente}%' or d2.""IdDeudor"" like '%{cliente}%' or";
            if (tipoReporte == "pagos")
            {
                consulta = @$"select  d2.""IdDeudor"" cedula, d2.""Nombre"" nombres,
                            p.""Observaciones"", fp.""Nombre"" formaPago , bp.""Nombre"" banco, tcb.""Nombre"" cuentaBancaria, tt.""Nombre"" ""Tipos Transaccion"", al.""Nombre"" AbonoLiquidacion,
                            d.*
                            from ""Pagos"" p 
                            join ""FormasPago"" fp  ON p.""FormaPagoId""  = fp.""FormaPagoId""  
                            join ""BancosPagos"" bp ON p.""IdBancosPago""  = bp.""Id""  
                            join ""TiposCuentaBancaria"" tcb  ON p.""IdTipoCuentaBancaria""  = tcb.""Id""
                            join ""TiposTransaccion"" tt on tt.""Id""  = p.""IdTipoTransaccion"" 
                            join ""AbonosLiquidacion"" al on al.""Id"" = p.""IdAbonoLiquidacion"" 
                            join ""Deudas"" d ON p.""IdDeuda""  = d.""IdDeuda"" 
                            join ""Deudores"" d2 on d2.""IdDeudor"" = d.""IdDeudor"" 
                            where {filtroCliente} (p.""FechaPago"" >= '{fechaInicio}' and p.""FechaPago"" <= '{fechaFin}' ) ";
            }

            if (tipoReporte == "gestiones")
            {
                consulta = @$"select  d2.""IdDeudor"" cedula, d2.""Nombre"" nombres,
                                g.*,
                                d.*
                                from ""Gestiones"" g 
                                join ""Deudas"" d ON g.""IdDeuda""  = d.""IdDeuda"" 
                                join ""Deudores"" d2 on d2.""IdDeudor"" = d.""IdDeudor"" 
                                where {filtroCliente} (g.""FechaGestion"" >= '{fechaInicio}' and g.""FechaGestion"" <= '{fechaFin}' ) ";
            }

            if (tipoReporte == "compromisos")
            {
                consulta = @$"select  d2.""IdDeudor"" cedula, d2.""Nombre"" nombres,
                                cp.*,
                                d.*
                                from ""CompromisosPagos"" cp 
                                join ""Deudas"" d ON cp.""IdDeuda""  = d.""IdDeuda"" 
                                join ""Deudores"" d2 on d2.""IdDeudor"" = d.""IdDeudor""
                                where {filtroCliente} (cp.""FechaCompromiso""  >= '{fechaInicio}' and cp.""FechaCompromiso""  <= '{fechaFin}' )";
            }

            PgConn conn = new PgConn();
            conn.cadenaConnect = Configuration.GetConnectionString("DefaultConnection");

            DataTable dataTable = conn.ejecutarconsulta_dt(consulta);
            ExcelPackage.License.SetNonCommercialPersonal("Santiago");
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells.LoadFromDataTable(dataTable, true);
                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{tipoReporte}_{DateTime.Now}.xlsx");
            }
        }

    }
}
