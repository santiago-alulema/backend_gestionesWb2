using gestiones_backend.Context;
using gestiones_backend.DbConn;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Entity;
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
        private readonly IAuthenticationService _authService;

        public ReporteGeneralGestionesController(
                                                   DataContext context,
                                                   IReportesEmpresaService reportesEmpresaService,
                                                   IConfiguration config,
                                                   IAuthenticationService authService)
        {
            _context = context;
            _reportesEmpresaService = reportesEmpresaService;
            Configuration = config;
            _authService = authService;
        }

        [HttpGet("gestiones-por-usuario")]
        public async Task<ActionResult<IEnumerable<object>>> GetGestionesPorUsuario([FromQuery] string FechaInicio,
                                                                                    [FromQuery] string FechaFin)
        {
            Usuario usuario = _authService.GetCurrentUser();

            DateTime fechaInicio = !string.IsNullOrEmpty(FechaInicio)
                                 ? DateTime.Parse(FechaInicio).ToUniversalTime()
                                 : DateTime.UtcNow.Date;

            DateTime fechaFin = !string.IsNullOrEmpty(FechaFin)
                ? DateTime.Parse(FechaFin).ToUniversalTime().AddDays(1).AddTicks(-1)
                : DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);


            var query = _context.Usuarios.AsQueryable();

            if (usuario.Rol.ToLower() != "admin") 
            {
                query = query.Where(u => u.IdUsuario == usuario.IdUsuario);
            }

            var resultado = await query
                .Select(u => new
                {
                    nombreUsuario = u.NombreUsuario,
                    CantidadGestiones = u.Gestiones
                        .Count(g => g.FechaGestion >= fechaInicio && g.FechaGestion <= fechaFin),
                    valorTotal = u.Gestiones
                        .Where(g => g.FechaGestion >= fechaInicio && g.FechaGestion <= fechaFin)
                        .Sum(g => g.IdDeudaNavigation.SaldoDeuda)
                })
                .Where(r => r.CantidadGestiones > 0) 
                .OrderByDescending(r => r.CantidadGestiones)
                .ToListAsync();

            return Ok(resultado);
        }


        [HttpGet("gestiones-pago-por-usuario")]
        public async Task<ActionResult<IEnumerable<object>>> GetGestionesPagoPorUsuario([FromQuery] string FechaInicio,
                                                                                        [FromQuery] string FechaFin)
        {
            Usuario usuario = _authService.GetCurrentUser();

            DateOnly fechaInicio = !string.IsNullOrEmpty(FechaInicio)
        ? DateOnly.FromDateTime(DateTime.Parse(FechaInicio))
        : DateOnly.FromDateTime(DateTime.Today);

            DateOnly fechaFin = !string.IsNullOrEmpty(FechaFin)
                ? DateOnly.FromDateTime(DateTime.Parse(FechaFin))
                : DateOnly.FromDateTime(DateTime.Today);

            var query = _context.Usuarios.AsQueryable();

            if (usuario.Rol.ToLower() != "admin")
            {
                query = query.Where(u => u.IdUsuario == usuario.IdUsuario);
            }

            var resultado = await query
                .Select(u => new
                {
                    nombreUsuario = u.NombreUsuario,
                    Pagos = u.Deudores
                        .SelectMany(d => d.Deuda)
                        .SelectMany(d => d.Pagos)
                        .Where(p => p.FechaPago != null &&
                                   p.FechaPago.Value >= fechaInicio &&
                                   p.FechaPago.Value <= fechaFin)
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
        public async Task<ActionResult<IEnumerable<object>>> GetCompromisosPagoPorUsuario([FromQuery] string FechaInicio = null,
                                                                                          [FromQuery] string FechaFin = null)
        {
            Usuario usuario = _authService.GetCurrentUser();

            DateOnly fechaInicio = !string.IsNullOrEmpty(FechaInicio)
                ? DateOnly.FromDateTime(DateTime.Parse(FechaInicio))
                : DateOnly.FromDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));

            DateOnly fechaFin = !string.IsNullOrEmpty(FechaFin)
                ? DateOnly.FromDateTime(DateTime.Parse(FechaFin))
                : DateOnly.FromDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1));

            var query = _context.Usuarios.AsQueryable();

            if (usuario.Rol.ToLower() != "admin")
            {
                query = query.Where(u => u.IdUsuario == usuario.IdUsuario);
            }

            var resultado = await query
                .Select(u => new
                {
                    nombreUsuario = u.NombreUsuario,
                    Compromisos = u.CompromisosPagos
                        .Where(c => c.FechaCompromiso >= fechaInicio &&
                                   c.FechaCompromiso <= fechaFin)
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
        public async Task<ActionResult<IEnumerable<ReporteEmpresaDto>>> GetReportePorEmpresaMesActual([FromQuery] string FechaInicio = null,
                                                                                                      [FromQuery] string FechaFin = null)
        {
                var reporte = await _reportesEmpresaService.ObtenerReportePorEmpresaMesActual(FechaInicio, FechaFin);
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
