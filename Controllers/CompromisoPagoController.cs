using gestiones_backend.Context;
using gestiones_backend.Entity;
using gestiones_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompromisoPagoController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly CompromisosPagoService _serviceCompromisos;
        public CompromisoPagoController(DataContext context,
            CompromisosPagoService serviceCompromisos)
        {
            _context = context;
            _serviceCompromisos = serviceCompromisos;
        }

        [HttpGet("marcar-incumplidos")]
        public async Task<IActionResult> MarcarIncumplidos()
        {
            var afectados = await _serviceCompromisos.MarcarIncumplidosVencidosAsync();
            return Ok(new
            {
                updated = afectados,
                message = $"Se marcaron {afectados} compromisos como incumplidos y se desactivaron."
            });
        }

        [HttpGet("uplodad-compromiso-pago/{compromisoPagoId}")]
        public IActionResult UploadCompromisoPago( string compromisoPagoId) {

            CompromisosPago compromiso = _context.CompromisosPagos.Where(x => x.IdCompromiso == compromisoPagoId).FirstOrDefault();
            compromiso.IncumplioCompromisoPago = true;
            compromiso.Estado = false;
            _context.CompromisosPagos.Update(compromiso);
            _context.SaveChanges();
            return Ok("Se actualizo el incumplimiento de compromiso de pago");
        }

        [HttpGet("desactivar-compromiso-pago/{compromisoPagoId}")]
        public IActionResult DesactivarCompromisoPago(string compromisoPagoId)
        {

            CompromisosPago compromiso = _context.CompromisosPagos.Where(x => x.IdCompromiso == compromisoPagoId).FirstOrDefault();
            compromiso.Estado = false;
            _context.CompromisosPagos.Update(compromiso);
            _context.SaveChanges();
            return Ok("Se Dio de baja el compromiso de pago");
        }
    }
}
