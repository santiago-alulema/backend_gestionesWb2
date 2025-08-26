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
        public async Task<ActionResult<IEnumerable<object>>> GetGestionesPorUsuario(
       [FromQuery] string FechaInicio = null,
       [FromQuery] string FechaFin = null)
        {
            Usuario usuario = _authService.GetCurrentUser();

            DateOnly fechaInicio = !string.IsNullOrEmpty(FechaInicio)
                ? DateOnly.FromDateTime(DateTime.Parse(FechaInicio))
                : DateOnly.FromDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));

            DateOnly fechaFin = !string.IsNullOrEmpty(FechaFin)
                ? DateOnly.FromDateTime(DateTime.Parse(FechaFin))
                : DateOnly.FromDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
                                            .AddMonths(1)
                                            .AddDays(-1));

            var inicio = fechaInicio.ToDateTime(TimeOnly.MinValue).ToUniversalTime();
            var fin = fechaFin.ToDateTime(TimeOnly.MaxValue).ToUniversalTime();

            var query = _context.Gestiones
                .Where(g => g.FechaGestion >= inicio &&
                g.FechaGestion <= fin);

            if (usuario.Rol.ToLower() != "admin")
            {
                query = query.Where(g => g.IdUsuarioGestiona == usuario.IdUsuario);
            }

            var resultado = await query
                .GroupBy(g => new { g.IdUsuarioGestiona, g.IdUsuarioGestionaNavigation.NombreUsuario })
                .Select(gr => new
                {
                    idUsuario = gr.Key.IdUsuarioGestiona,
                    nombreUsuario = gr.Key.NombreUsuario,
                    cantidadGestiones = gr.Count(),
                    resultados = gr
                        .GroupBy(g => g.IdTipoResultadoNavigation.Nombre)
                        .Select(rg => new
                        {
                            TipoResultado = rg.Key,
                            Cantidad = rg.Count(),
                            valorTotal = 0
                        })
                        .ToList()
                })
                .Where(r => r.cantidadGestiones > 0)
                .OrderByDescending(r => r.cantidadGestiones)
                .ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("gestiones-pago-por-usuario")]
        public async Task<ActionResult<IEnumerable<object>>> GetUsuariosConPagos(
                                                                                    [FromQuery] string FechaInicio,
                                                                                    [FromQuery] string FechaFin)
        {
            Usuario usuario = _authService.GetCurrentUser();

            DateOnly fechaInicio = !string.IsNullOrEmpty(FechaInicio)
                ? DateOnly.FromDateTime(DateTime.Parse(FechaInicio))
                : DateOnly.FromDateTime(DateTime.Today);

            DateOnly fechaFin = !string.IsNullOrEmpty(FechaFin)
                ? DateOnly.FromDateTime(DateTime.Parse(FechaFin))
                : DateOnly.FromDateTime(DateTime.Today);

            var query = _context.Pagos
                .Where(p => p.FechaPago != null &&
                            p.FechaPago.Value >= fechaInicio &&
                            p.FechaPago.Value <= fechaFin);

            if (usuario.Rol.ToLower() != "admin")
            {
                query = query.Where(p => p.IdUsuario == usuario.IdUsuario);
            }

            var resultado = await query
                .GroupBy(p => new { p.IdUsuario, p.IdUsuarioNavigation.NombreUsuario })
                .Select(g => new
                {
                    idUsuario = g.Key.IdUsuario,
                    nombreUsuario = g.Key.NombreUsuario == null ? "Sin Gestor" : g.Key.NombreUsuario,
                    cantidadPagos = g.Count(),
                    valorTotal = g.Sum(p => p.MontoPagado)
                })
                .OrderByDescending(r => r.cantidadPagos)
                .ToListAsync();

            return Ok(resultado);
        }


        [HttpGet("compromisos-pago-por-usuario")]
        public async Task<ActionResult<IEnumerable<object>>> GetCompromisosPagoPorUsuario(
     [FromQuery] string FechaInicio = null,
     [FromQuery] string FechaFin = null)
        {
            Usuario usuario = _authService.GetCurrentUser();

            DateOnly fechaInicio = !string.IsNullOrEmpty(FechaInicio)
                ? DateOnly.FromDateTime(DateTime.Parse(FechaInicio))
                : DateOnly.FromDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));

            DateOnly fechaFin = !string.IsNullOrEmpty(FechaFin)
                ? DateOnly.FromDateTime(DateTime.Parse(FechaFin))
                : DateOnly.FromDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
                                            .AddMonths(1)
                                            .AddDays(-1));

            var query = _context.CompromisosPagos
                .Where(c => c.FechaCompromiso >= fechaInicio &&
                            c.FechaCompromiso <= fechaFin);

            // Si no es admin, filtrar solo compromisos del usuario actual
            if (usuario.Rol.ToLower() != "admin")
            {
                query = query.Where(c => c.IdUsuario == usuario.IdUsuario);
            }

            var resultado = await query
                .GroupBy(c => new { c.IdUsuario, c.IdUsuarioNavigation.NombreUsuario })
                .Select(gr => new
                {
                    idUsuario = gr.Key.IdUsuario,
                    nombreUsuario = gr.Key.NombreUsuario,
                    cantidadCompromisos = gr.Count(),
                    valorTotal = gr.Sum(c => c.MontoComprometido),
                    incumplidos = gr.Count(c => c.IncumplioCompromisoPago == true),
                    cumplidos = gr.Count(c => c.IncumplioCompromisoPago == false && c.FechaCumplimientoReal != null)
                })
                .Where(r => r.cantidadCompromisos > 0)
                .OrderByDescending(r => r.cantidadCompromisos)
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
                consulta = @$"select 
                                d2.""IdDeudor"" cedula, 
                     		    d2.""Nombre"" nombres,
                                p.""Observaciones"", 
                                bp.""Nombre"" banco, 
                                tcb.""Nombre"" cuentaBancaria, 
                                tt.""Nombre"" ""Tipos Transaccion"", 
                                al.""Nombre"" AbonoLiquidacion,
                                p.""MontoPagado"",
                                d.""Empresa"" 
                            from ""Pagos"" p 
                            join ""BancosPagos"" bp ON p.""IdBancosPago""  = bp.""Id""  
                            join ""TiposCuentaBancaria"" tcb  ON p.""IdTipoCuentaBancaria""  = tcb.""Id""
                            join ""TiposTransaccion"" tt on tt.""Id""  = p.""IdTipoTransaccion"" 
                            join ""AbonosLiquidacion"" al on al.""Id"" = p.""IdAbonoLiquidacion"" 
                            join ""Deudas"" d ON p.""IdDeuda""  = d.""IdDeuda"" 
                            join ""Deudores"" d2 on d2.""IdDeudor"" = d.""IdDeudor"" 
                            where {filtroCliente} (Date(p.""FechaRegistro"") >= '{fechaInicio}' and Date(p.""FechaRegistro"") <= '{fechaFin}' ) ";
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
                consulta = @$"select    d2.""IdDeudor"" cedula, 
	                                    d2.""Nombre"" nombres,
       	                                cp.""MontoComprometido"",
       	                                tt.""Nombre"",
       	                                d.""Empresa"",
       	                                d.""NumeroFactura"",
       	                                d.""DiasMora"",
       	                                d.""ValorCuota"",
       	                                d.""Tramo"",
       	                                d.""Descuento"" 
                                from ""CompromisosPagos"" cp 
                                join ""Deudas"" d ON cp.""IdDeuda""  = d.""IdDeuda"" 
                                join ""Deudores"" d2 on d2.""IdDeudor"" = d.""IdDeudor""
                                join ""TiposTareas"" tt ON tt.""Id""  = cp.""IdTipoTarea"" 
                                where {filtroCliente} (Date(cp.""FechaRegistro"")  >= '{fechaInicio}' and Date(cp.""FechaRegistro"")  <= '{fechaFin}' )";
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
