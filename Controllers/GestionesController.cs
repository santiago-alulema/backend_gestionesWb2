using gestiones_backend.Context;
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

        public GestionesController(IAuthenticationService authService, DataContext context, GestioneService service, CompromisosPagoService serviceCompromisos)
        {
            _context = context;
            _service = service;
            _serviceCompromisos = serviceCompromisos;
            _authService = authService;
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
            gestione.IdTipoGestion = nuevaGestion.IdTipoGestion;
            gestione.IdUsuarioGestiona = usuario.IdUsuario;
            gestione.IdTipoContactoGestion = nuevaGestion.IdTipoContactoDeudor;
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

            deuda.SaldoActual -= pagoDto.MontoPagado;
            deuda.UltimoPago = pagoDto.MontoPagado;

            _context.Pagos.Add(pago);
            //_context.Deudas.Update(deuda);
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
    }
}
