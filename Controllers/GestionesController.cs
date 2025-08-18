using gestiones_backend.Context;
using gestiones_backend.DbConn;
using gestiones_backend.Dtos.In;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Entity;
using gestiones_backend.Interfaces;
using gestiones_backend.Services;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GestionesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly GestioneService _service;
        private readonly CompromisosPagoService _serviceCompromisos;
        private readonly IAuthenticationService _authService;
        private readonly IConfiguration Configuration;

        public GestionesController(IAuthenticationService authService, 
            DataContext context, 
            GestioneService service,
            CompromisosPagoService serviceCompromisos,
            IConfiguration config)
        {
            _context = context;
            _service = service;
            _serviceCompromisos = serviceCompromisos;
            _authService = authService;
            Configuration = config;
        }


        [Authorize]
        [HttpPost("grabar-gestion")]
        public IActionResult GrabarGestiones([FromBody] GestionInDTO nuevaGestion)
        {
            var usuario = _authService.GetCurrentUser();

            Gestione gestione = new Gestione();
            gestione.IdGestion = Guid.NewGuid().ToString();
            gestione.Descripcion = nuevaGestion.Descripcion;
            gestione.IdDeuda = nuevaGestion.idDeuda;
            gestione.IdUsuarioGestiona = usuario.IdUsuario;
            gestione.IdTipoResultado = nuevaGestion.IdResultado;
            gestione.IdTipoContactoResultado = nuevaGestion.idTipoContactoCliente;
            gestione.IdRespuestaTipoContacto = nuevaGestion.IdRespuesta;
            gestione.Email = nuevaGestion.Email;
            _context.Gestiones.Add(gestione);
            _context.SaveChanges();
            return Ok("Se grabo exitosamente");
        }

        [Authorize]
        [HttpPost("grabar-compromiso-pago")]
        public IActionResult GrabarCompromisosPago(CompromisoPagoInDTO compromisoPagoNuevo)
        {
            var usuario = _authService.GetCurrentUser();

            CompromisosPago compromisoPago = compromisoPagoNuevo.Adapt<CompromisosPago>();
            compromisoPago.IdTipoTarea = compromisoPagoNuevo.TipoTarea;
            compromisoPago.HoraRecordatorio = compromisoPagoNuevo.HoraRecordatorio;
            compromisoPago.IdCompromiso = Guid.NewGuid().ToString();
            compromisoPago.IdUsuario = usuario.IdUsuario;
            _context.CompromisosPagos.Add(compromisoPago);
            _context.SaveChanges();
            return Ok("Se grabo exitosamente");
        }

        [HttpPost("grabar-pago")]
        public async Task<ActionResult<Pago>> RegistrarPago(PagoGrabarDTO pagoDto)
        {
            var usuario = _authService.GetCurrentUser();
            var deuda = await _context.Deudas.FindAsync(pagoDto.IdDeuda);

            if (deuda == null)
            {
                return NotFound(new { Error = "La deuda especificada no existe" });
            }

            var pago = new Pago
            {
                IdDeuda = pagoDto.IdDeuda,
                FechaPago = pagoDto.FechaPago,
                MontoPagado = pagoDto.MontoPagado,
                MedioPago = pagoDto.MedioPago,
                Observaciones = pagoDto.Observaciones,
                IdUsuario = usuario.IdUsuario,
                IdBancosPago = pagoDto.BancoId,
                NumeroDocumenro = pagoDto.NumeroDocumento,
                IdTipoCuentaBancaria = pagoDto.CuentaId,
                IdTipoTransaccion = pagoDto.TipoTransaccionId,
                IdAbonoLiquidacion = pagoDto.AbonoLiquidacionId,
                
            };
            
            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            return Ok("Se grabo exitosamente");
        }

        [HttpGet("gestiones-reporte")]
        public async Task<IActionResult> GetByFilters(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string? deudorName = null)
        {
            List<GestionDto> gestiones = await _service.GetGestionesByFiltersAsync(
                startDate, endDate, deudorName);

            return Ok(gestiones);
        }

        [HttpGet("compromiso-pagos-hoy")]
        public async Task<IActionResult> GetByFiltersGestiones()
        {
            var usuario = _authService.GetCurrentUser();
            DateOnly hoy = DateOnly.FromDateTime(DateTime.Today);
            List<CompromisosPago> conpromisosPagos = _context.CompromisosPagos.Include(x => x.IdDeudaNavigation).Where(x => x.FechaCompromiso == hoy).ToList();
            return Ok("");
        }

        [HttpGet("compromiso-reporte")]
        public async Task<IActionResult> GetByFiltersGestiones(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        [FromQuery] string? deudorName = null)
        {
            List<CompromisoPagoDto> compromisos = await _serviceCompromisos.GetCompromisosByFiltersAsync(
                startDate, endDate, deudorName);

            return Ok(compromisos);
        }

        [HttpGet("movimiento-deuda/{idDeuda}")]
        public async Task<IActionResult> GetMovimientoDeuda(string idDeuda)
        {
            string consulta = @$"SELECT * FROM (
                                SELECT 'Pago' AS tipo,
                                        TO_CHAR( p.""FechaPago""::DATE, 'YYYY-MM-DD') AS fecha,
                                        p.""Observaciones"" observaciones, 
                                        ('<strong>Banco: </strong> '  || bp.""Nombre"" || 
                                	   	'<br><strong>Cuenta: </strong>' || tcb.""Nombre"" || 
                                	   	'<br><strong>Tipo Transaccion: </strong>' || tt.""Nombre""|| 
                                	   	'<br><strong>Abono/Liquidacion: </strong>' || al.""Nombre""|| 
                                	   	'<br><strong>Numero Doc.: </strong>' || p.""NumeroDocumenro"" || 
                                	   	'<br><strong>Fecha Pago: </strong>' || p.""FechaPago"" || 
                                	   	'<br><strong>Valor: </strong>' || p.""MontoPagado"") as tracking
                                FROM ""Deudas"" d
                                JOIN ""Pagos"" p ON p.""IdDeuda"" = d.""IdDeuda"" 
                                JOIN ""BancosPagos"" bp ON p.""IdBancosPago"" = bp.""Id""  
                                JOIN ""TiposCuentaBancaria"" tcb ON p.""IdTipoCuentaBancaria"" = tcb.""Id""
                                JOIN ""TiposTransaccion"" tt ON tt.""Id"" = p.""IdTipoTransaccion"" 
                                JOIN ""AbonosLiquidacion"" al ON al.""Id"" = p.""IdAbonoLiquidacion"" 
                                WHERE d.""IdDeuda"" = '{idDeuda}'
    
                                UNION ALL
    
                                SELECT 'Gestion' AS tipo, 
                                        TO_CHAR(""FechaGestion""::DATE, 'YYYY-MM-DD') AS fecha, 
                                        g.""Descripcion"" observaciones,
                                        ('<strong>RESULTADO:</strong> '  || tr.""Nombre"" || 
                                         '<br><strong>Tipo Constacto Cliente</strong>' || tcr.""Nombre"" || 
                                         '<br><strong>Respuesta: </strong>' || rtc.""Nombre"") as tracking
                                FROM ""Deudas"" d
                                JOIN ""Gestiones"" g ON g.""IdDeuda"" = d.""IdDeuda""
                                join  ""TiposResultado"" tr on tr.""Id"" = g.""IdTipoResultado""
                                join ""TiposContactoResultado"" tcr on tcr.""Id"" = g.""IdTipoContactoResultado""
                                join  ""RespuestasTipoContacto"" rtc on rtc.""Id"" = g.""IdRespuestaTipoContacto""
                                WHERE d.""IdDeuda"" = '{idDeuda}'
    
                                UNION ALL
    
                                SELECT 'Compromiso Pago' AS tipo, 
                                        TO_CHAR( cp.""FechaCompromiso""::DATE, 'YYYY-MM-DD') AS fecha,
                                        cp.""Observaciones"" observaciones,
                                        ('<strong>Fecha recordatorio:</strong> '  || cp.""FechaCompromiso"" || 
                                	   	'<br><strong>Hora recordatorio</strong>' || cp.""HoraRecordatorio"" || 
                                	   	'<br><strong>Valor: </strong>' || cp.""MontoComprometido"" || 
                                	   	'<br><strong>Tipo tarea: </strong>' || tr.""Nombre""|| 
                                	   	'<br><strong>Observaciones: </strong>' || cp.""Observaciones"") as tracking
                                FROM ""Deudas"" d
                                JOIN ""CompromisosPagos"" cp ON cp.""IdDeuda"" = d.""IdDeuda""
                                JOIN ""TiposTareas"" tr ON tr.""Id"" = cp.""IdTipoTarea""
                                WHERE d.""IdDeuda"" = '{idDeuda}'
                            ) AS combined_results
                            ORDER BY fecha ASC;";
            PgConn conn = new PgConn();
            conn.cadenaConnect = Configuration.GetConnectionString("DefaultConnection");

            DataTable dataTable = conn.ejecutarconsulta_dt(consulta);
            string JSONString = string.Empty;
            JSONString = Newtonsoft.Json.JsonConvert.SerializeObject(dataTable);

            return Content(JSONString, "application/json");
        }
    }
}
