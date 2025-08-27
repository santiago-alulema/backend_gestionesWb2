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
        private readonly IGestionesService _gestionesService;

        public GestionesController(IAuthenticationService authService, 
            DataContext context, 
            GestioneService service,
            CompromisosPagoService serviceCompromisos,
            IConfiguration config,
            IGestionesService gestionesService)
        {
            _context = context;
            _service = service;
            _serviceCompromisos = serviceCompromisos;
            _authService = authService;
            Configuration = config;
            _gestionesService = gestionesService;
        }

        [HttpGet]
        public ActionResult<List<GestionDto>> GetAll()
        {
            var gestiones = _gestionesService.GetAllAsync();
            return Ok(gestiones);
        }

        [HttpPut("{id}")]
        public ActionResult<GestionDto> Update(string id, [FromBody] UpdateGestionDto dto)
        {
            var gestion = _gestionesService.UpdateAsync(id, dto);
            if (gestion == null)
                return NotFound(new { message = "Gestión no encontrada" });

            return Ok(gestion);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var eliminado = _gestionesService.DeleteAsync(id);
            if (!eliminado)
                return NotFound(new { message = "Gestión no encontrada" });

            return NoContent();
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
            gestione.Telefono = nuevaGestion.Telefono;
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
            compromisoPago.Telefono = compromisoPagoNuevo.Telefono;
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
                Telefono = pagoDto.Telefono,
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
            string consulta = @$"SELECT tipo, 
                              TO_CHAR(fecha, 'YYYY-MM-DD') as fecha_formateada,
                              observaciones,
                              tracking
                              FROM (
                      SELECT 'Pago' AS tipo,
                              p.""FechaRegistro"" AS fecha,
                              p.""Observaciones"" observaciones, 
                              ('<strong>Banco: </strong> '  || COALESCE(bp.""Nombre"", 'N/A') || 
                             '<br><strong>Cuenta: </strong>' || COALESCE(tcb.""Nombre"", 'N/A') || 
                             '<br><strong>Tipo Transaccion: </strong>' || COALESCE(tt.""Nombre"", 'N/A') || 
                             '<br><strong>Abono/Liquidacion: </strong>' || COALESCE(al.""Nombre"", 'N/A') || 
                             '<br><strong>Numero Doc.: </strong>' || COALESCE(p.""NumeroDocumenro"", 'N/A') || 
                             '<br><strong>Fecha Pago: </strong>' || COALESCE(TO_CHAR(p.""FechaPago"", 'YYYY-MM-DD HH24:MI:SS'), 'N/A') || 
                             '<br><strong>Valor: </strong>' || COALESCE(p.""MontoPagado""::text, 'N/A') ||
                             '<br><strong>Telefono: </strong>' || COALESCE(p.""Telefono""::text, 'N/A')  ) AS tracking
                      FROM ""Pagos"" p 
                      LEFT JOIN ""Deudas"" d ON p.""IdDeuda"" = d.""IdDeuda"" 
                      LEFT JOIN ""BancosPagos"" bp ON p.""IdBancosPago"" = bp.""Id""  
                      LEFT JOIN ""TiposCuentaBancaria"" tcb ON p.""IdTipoCuentaBancaria"" = tcb.""Id""
                      LEFT JOIN ""TiposTransaccion"" tt ON tt.""Id"" = p.""IdTipoTransaccion"" 
                      LEFT JOIN ""AbonosLiquidacion"" al ON al.""Id"" = p.""IdAbonoLiquidacion"" 
                      WHERE d.""IdDeuda"" = '{idDeuda}'

                      UNION ALL

                      SELECT 'Gestion' AS tipo, 
                              g.""FechaGestion"" AS fecha, 
                              g.""Descripcion"" observaciones,
                              ('<strong>RESULTADO:</strong> '  || tr.""Nombre"" || 
                               '<br><strong>Tipo Contacto Cliente</strong>' || tcr.""Nombre"" || 
                               '<br><strong>Respuesta: </strong>' || rtc.""Nombre"" ||
                               '<br><strong>Telefono: </strong>' || g.""Telefono"") as tracking
                      FROM ""Gestiones"" g 
                      JOIN ""Deudas"" d ON g.""IdDeuda"" = d.""IdDeuda""
                      JOIN ""TiposResultado"" tr ON tr.""Id"" = g.""IdTipoResultado""
                      JOIN ""TiposContactoResultado"" tcr ON tcr.""Id"" = g.""IdTipoContactoResultado""
                      JOIN ""RespuestasTipoContacto"" rtc ON rtc.""Id"" = g.""IdRespuestaTipoContacto""
                      WHERE d.""IdDeuda"" = '{idDeuda}'

                      UNION ALL

                      SELECT 'Tarea' AS tipo, 
                              cp.""FechaRegistro"" AS fecha,
                              cp.""Observaciones"" observaciones,
                              ('<strong>Fecha recordatorio:</strong> '  || TO_CHAR(cp.""FechaCompromiso"", 'YYYY-MM-DD HH24:MI:SS') || 
                         	    '<br><strong>Hora recordatorio</strong>' || cp.""HoraRecordatorio"" || 
                         	    '<br><strong>Valor: </strong>' || cp.""MontoComprometido"" || 
                         	    '<br><strong>Tipo tarea: </strong>' || tr.""Nombre""|| 
                         	    '<br><strong>Observaciones: </strong>' || cp.""Observaciones"" ||
                                '<br><strong>Telefono: </strong>' || cp.""Telefono"") as tracking
                      FROM ""CompromisosPagos"" cp
                      JOIN ""Deudas"" d ON cp.""IdDeuda"" = d.""IdDeuda""
                      JOIN ""TiposTareas"" tr ON tr.""Id"" = cp.""IdTipoTarea""
                      WHERE d.""IdDeuda"" = '{idDeuda}'
                  ) AS combined_results
                  ORDER BY fecha DESC;";
            PgConn conn = new PgConn();
            conn.cadenaConnect = Configuration.GetConnectionString("DefaultConnection");

            DataTable dataTable = conn.ejecutarconsulta_dt(consulta);
            string JSONString = string.Empty;
            JSONString = Newtonsoft.Json.JsonConvert.SerializeObject(dataTable);

            return Content(JSONString, "application/json");
        }
    }
}
