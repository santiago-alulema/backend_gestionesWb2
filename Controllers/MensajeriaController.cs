using gestiones_backend.Context;
using gestiones_backend.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensajeriaController : ControllerBase
    {
        public readonly DataContext _dataContext;
        public MensajeriaController(DataContext dataContext)
        {
            _dataContext =  dataContext;
        }

        [HttpGet("obtener-mensajes-whatsapp-gestion")]
        public IActionResult ObtenerMensajeWhatsappGestion()
        {
            MensajeWhatsappUsuario mensaje = _dataContext.MensajesWhatsapp.FirstOrDefault(x => x.TipoMensaje == "RECORDATORIO DE GESTION");
            if (mensaje == null)
                return BadRequest("No existe mensaje de <strong>RECORDATORIO DE GESTION</strong>");
            return Ok(mensaje);
        }

        [HttpGet("obtener-mensajes-whatsapp-tareas")]
        public IActionResult ObtenerMensajeWhatsappTareas()
        {
            List<MensajeWhatsappUsuario> mensajes = _dataContext.MensajesWhatsapp.Where(x => x.TipoMensaje != "RECORDATORIO DE GESTION").ToList();
            if (mensajes.Count == 0)
                return BadRequest("No existe mensaje de <strong>RECORDATORIO DE GESTION</strong>");
            return Ok(mensajes);
        }
    }
}
