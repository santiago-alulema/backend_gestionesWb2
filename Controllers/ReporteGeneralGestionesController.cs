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
                    valorTotal = 0
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
            var usuario = _authService.GetCurrentUser();

            DateOnly fi = !string.IsNullOrWhiteSpace(FechaInicio)
                ? DateOnly.FromDateTime(DateTime.Parse(FechaInicio))
                : new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1);

            DateOnly ff = !string.IsNullOrWhiteSpace(FechaFin)
                ? DateOnly.FromDateTime(DateTime.Parse(FechaFin))
                : new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);

            var query = _context.Pagos
                .Where(p => p.FechaPago != null &&
                            p.FechaPago.Value >= fi &&
                            p.FechaPago.Value <= ff);

            if (usuario.Rol?.ToLower() != "admin")
                query = query.Where(p => p.IdUsuario == usuario.IdUsuario);

            var resultado = await query
                .GroupBy(p => new { p.IdUsuario, p.IdUsuarioNavigation.NombreUsuario })
                .Select(g => new
                {
                    idUsuario = g.Key.IdUsuario,
                    nombreUsuario = g.Key.NombreUsuario ?? "Sin Gestor",
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

            DateTime fechaInicio = !string.IsNullOrEmpty(FechaInicio)
                                   ? DateTime.SpecifyKind(DateTime.Parse(FechaInicio).Date, DateTimeKind.Utc)
                                   : DateTime.SpecifyKind(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTimeKind.Utc);

            DateTime fechaFin = !string.IsNullOrEmpty(FechaFin)
                                ? DateTime.SpecifyKind(DateTime.Parse(FechaFin).Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc)
                                : DateTime.SpecifyKind(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
                                    .AddMonths(1)
                                    .AddTicks(-1), DateTimeKind.Utc);

            var fechaInicioOnly = DateOnly.FromDateTime(fechaInicio);
            var fechaFinOnly = DateOnly.FromDateTime(fechaFin);

            var query = _context.CompromisosPagos
                .Where(c => c.FechaCompromiso >= fechaInicioOnly &&
                            c.FechaCompromiso <= fechaFinOnly);



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



        [HttpGet("bajar-reporte-deudas-subidas")]
        public IActionResult BajarReporteDeudas([FromQuery] string fechaInicio, [FromQuery] string fechaFin)
        {
            string consulta = @$"SELECT
                                    deu.""Nombre""                      AS ""Deudor"",
                                    deu.""Direccion""                   AS ""Direccion"",
                                    deu.""Telefono""                    AS ""Telefono"",
                                    deu.""Correo""                      AS ""Correo"",
                                    dz.""Empresa""                      AS ""Empresa"",
                                    dz.""Agencia""                      AS ""Agencia"",
                                    dz.""Ciudad""                       AS ""Ciudad"",
                                    dz.""TipoDocumento""                AS ""Tipo Documento"",
                                    dz.""NumeroFactura""                AS ""Nro Factura"",
                                    dz.""ProductoDescripcion""          AS ""Producto"",
                                    dz.""FechaVenta""                   AS ""Fecha Venta"",
                                    dz.""FechaUltimoPago""              AS ""Fecha Último Pago"",
                                    dz.""DeudaCapital""                 AS ""Deuda Capital"",
                                    dz.""Interes""                      AS ""Interés"",
                                    dz.""GastosCobranzas""              AS ""Gastos Cobranzas"",
                                    dz.""MontoCobrar""                  AS ""Monto a Cobrar"",
                                    COALESCE(dz.""SaldoDeuda"", dz.""SaldoDeulda"", 0) AS ""Saldo Pendiente"",
                                    dz.""NumeroCuotas""                 AS ""Cuotas"",
                                    dz.""ValorCuota""                   AS ""Valor Cuota"",
                                    dz.""DiasMora""                     AS ""Días de Mora"",
                                    dz.""UltimoPago""                   AS ""Último Pago (valor)"",
                                    dz.""Clasificacion""                AS ""Clasificación"",
                                    dz.""Tramo""                        AS ""Tramo"",
                                    dz.""Estado""                       AS ""Estado Deuda"",
                                    CASE WHEN dz.""EsActivo"" = TRUE
                                         THEN 'ACTIVA'
                                         ELSE 'INACTIVA'
                                    END                                 AS ""Registro Activo""
                                FROM public.""Deudas"" dz
                                LEFT JOIN public.""Deudores"" deu
                                  ON deu.""IdDeudor"" = dz.""IdDeudor""
                                ORDER BY dz.""DiasMora"" DESC NULLS LAST, dz.""FechaVenta"" ASC;";

            PgConn conn = new PgConn();
            conn.cadenaConnect = Configuration.GetConnectionString("DefaultConnection");

            DataTable dataTable = conn.ejecutarconsulta_dt(consulta);
            ExcelPackage.License.SetNonCommercialPersonal("Santiago");
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells.LoadFromDataTable(dataTable, true);
                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"DEUDAS_SUBIDAS_{DateTime.Now}.xlsx");
            }
        }
        [HttpGet("reporte-general-gestiones/{fechaInicio}/{fechaFin}/{tipoReporte}/{cliente}")]
        public IActionResult GetReporteGeneral(string fechaInicio, string fechaFin, string tipoReporte, string cliente = null)
        {
            string consulta = "";
            string filtroCliente = cliente == "-SP-" ? "" : @$"d2.""Nombre"" like '%{cliente}%' or d2.""IdDeudor"" like '%{cliente}%' or";
            if (tipoReporte == "pagos")
            {
                consulta = @$"select 
                                 d2.""IdDeudor"" AS cedula, 
                                d2.""Nombre"" AS nombres,
                                p.""Observaciones"", 
                                bp.""Nombre"" AS banco, 
                                tcb.""Nombre"" AS cuenta_bancaria, 
                                tt.""Nombre"" AS tipos_transaccion, 
                                al.""Nombre"" AS abono_liquidacion,
                                p.""MontoPagado"",
                                p.""FechaPago"" fechaPago,
                                p.""NumeroDocumenro"",
                                p.""Observaciones"" ,
                                COALESCE(NULLIF(d.""Empresa"", ''), p.""ArchivoMigracion"") AS ""Empresa"",
                                d.""FechaVenta"",
                                d.""Estado"",
                                d.""NumeroFactura"",
                                d.""SaldoDeuda"",
                                d.""NumeroCuotas"",
                                d.""DiasMora"",
                                d.""ValorCuota"",
                                d.""Tramo"",
                                d.""UltimoPago"",
                                d.""Clasificacion"",
                                d.""Creditos"",
                                d.""Descuento"",
                                d.""DeudaCapital"",
                                d.""FechaUltimoPago"",
                                d.""GastosCobranzas"",
                                d.""Interes"",
                                d.""MontoCobrar"",
                                d.""TipoDocumento"",
                                d.""Agencia"",
                                d.""Ciudad"",
                                d.""ProductoDescripcion"",
                                u.""NombreUsuario"",
                                u.""NombreCompleto"" gestor
                            from ""Pagos"" p 
                            left join ""BancosPagos"" bp ON p.""IdBancosPago""  = bp.""Id""  
                            left join ""TiposCuentaBancaria"" tcb  ON p.""IdTipoCuentaBancaria""  = tcb.""Id""
                            left join ""TiposTransaccion"" tt on tt.""Id""  = p.""IdTipoTransaccion"" 
                            left join ""AbonosLiquidacion"" al on al.""Id"" = p.""IdAbonoLiquidacion"" 
                            left join ""Deudas"" d ON p.""IdDeuda""  = d.""IdDeuda"" 
                            left join ""Deudores"" d2 on d2.""IdDeudor"" = d.""IdDeudor"" 
                            left join ""Usuarios"" u on u.""IdUsuario"" = p.""IdUsuario"" 
                            where {filtroCliente} (Date(p.""FechaPago"") >= '{fechaInicio}' and Date(p.""FechaPago"") <= '{fechaFin}' ) ";
            }

            if (tipoReporte == "gestiones")
            {
                consulta = @$" select       g.""Descripcion"",
	                                        g.""Email"",
	                                        g.""FechaGestion"",
                                            d2.""IdDeudor"" cedula, 
	                               		    d2.""Nombre"" nombres,
	                               		    tr.""Nombre"" tipoResultado,
	                               		    rtc.""Nombre"" tipoContacto,
	                               		    tcr.""Nombre"" resultado,
                                            d.""Empresa"",
                                            d.""FechaVenta"",
                                            d.""Estado"",
                                            d.""NumeroFactura"",
                                            d.""SaldoDeuda"",
                                            d.""NumeroCuotas"",
                                            d.""DiasMora"",
                                            d.""ValorCuota"",
                                            d.""Tramo"",
                                            d.""UltimoPago"",
                                            d.""Clasificacion"",
                                            d.""Creditos"",
                                            d.""Descuento"",
                                            d.""DeudaCapital"",
                                            d.""FechaUltimoPago"",
                                            d.""GastosCobranzas"",
                                            d.""Interes"",
                                            d.""MontoCobrar"",
                                            d.""TipoDocumento"",
                                            d.""Agencia"",
                                            d.""Ciudad"",
                                            d.""ProductoDescripcion"",
                                            u.""NombreUsuario"",
                                            u.""NombreCompleto"" gestor
	                                from ""Gestiones"" g 
	                                join ""Deudas"" d ON g.""IdDeuda""  = d.""IdDeuda"" 
	                                join ""Deudores"" d2 on d2.""IdDeudor"" = d.""IdDeudor"" 
	                                Left join ""TiposResultado"" tr on tr.""Id"" = g.""IdTipoResultado"" 
	                                Left join ""RespuestasTipoContacto"" rtc on rtc.""Id"" = g.""IdRespuestaTipoContacto"" 
	                                Left join ""TiposContactoResultado"" tcr on tcr.""Id"" = g.""IdTipoContactoResultado""
                                    join ""Usuarios"" u on u.""IdUsuario"" = g.""IdUsuarioGestiona"" 
                                where {filtroCliente} (Date(g.""FechaGestion"") >= '{fechaInicio}' and Date(g.""FechaGestion"") <= '{fechaFin}' ) ";
            }

            if (tipoReporte == "compromisos")
            {
                consulta = @$"select    
                                        cp.""FechaCompromiso"", 
                                        cp.""FechaRegistro"", 
                                        cp.""MontoComprometido"", 
                                        cp.""Observaciones"", 
                                        cp.""IncumplioCompromisoPago"", 
                                        cp.""HoraRecordatorio"",
                                        d2.""IdDeudor"" cedula, 
	                                    d2.""Nombre"" nombres,
       	                                cp.""MontoComprometido"",
       	                                tt.""Nombre"",
       	                                d.""Empresa"",
                                        d.""FechaVenta"",
                                        d.""Estado"",
                                        d.""NumeroFactura"",
                                        d.""SaldoDeuda"",
                                        d.""NumeroCuotas"",
                                        d.""DiasMora"",
                                        d.""ValorCuota"",
                                        d.""Tramo"",
                                        d.""UltimoPago"",
                                        d.""Clasificacion"",
                                        d.""Creditos"",
                                        d.""Descuento"",
                                        d.""DeudaCapital"",
                                        d.""FechaUltimoPago"",
                                        d.""GastosCobranzas"",
                                        d.""Interes"",
                                        d.""MontoCobrar"",
                                        d.""TipoDocumento"",
                                        d.""Agencia"",
                                        d.""Ciudad"",
                                        d.""ProductoDescripcion"",
                                        u.""NombreUsuario"",
                                        u.""NombreCompleto"" gestor
                                from ""CompromisosPagos"" cp 
                                join ""Deudas"" d ON cp.""IdDeuda""  = d.""IdDeuda"" 
                                join ""Deudores"" d2 on d2.""IdDeudor"" = d.""IdDeudor""
                                Left join ""TiposTareas"" tt ON tt.""Id""  = cp.""IdTipoTarea"" 
                                join ""Usuarios"" u on u.""IdUsuario"" = cp.""IdUsuario"" 
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
