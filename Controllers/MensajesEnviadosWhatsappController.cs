using gestiones_backend.Context;
using gestiones_backend.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensajesEnviadosWhatsappController : ControllerBase
    {
        private readonly DataContext _context;

        public MensajesEnviadosWhatsappController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] MensajesEnviadosWhatsapp dto)
        {
            if (dto == null)
                return BadRequest("Datos inválidos.");

            dto.FechaRegistro = DateTime.UtcNow;

            _context.MensajesEnviadosWhatsapp.Add(dto);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                mensaje = "Mensaje registrado correctamente",
                id = dto.Id
            });
        }
    }
}
