using gestiones_backend.Context;
using gestiones_backend.Dtos.In;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensajesWhatsappController : ControllerBase
    {
        private readonly DataContext _db;

        public MensajesWhatsappController(DataContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Lista todos (sin paginación). Filtros opcionales por texto (q) y tipo.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MensajeWhatsappReadDto>>> GetAll(
            [FromQuery] string? q = null,
            [FromQuery] string? tipo = null)
        {
            var query = _db.MensajesWhatsapp.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var like = q.Trim().ToLower();
                query = query.Where(x =>
                    (x.Mensaje != null && x.Mensaje.ToLower().Contains(like)) ||
                    (x.MensajeCorreo != null && x.MensajeCorreo.ToLower().Contains(like)));
            }

            if (!string.IsNullOrWhiteSpace(tipo))
            {
                var tipoNorm = tipo.Trim().ToLower();
                query = query.Where(x => x.TipoMensaje.ToLower() == tipoNorm);
            }

            var data = await query
                .OrderByDescending(x => x.Id) // cambia por fecha si agregas una
                .Select(x => new MensajeWhatsappReadDto
                {
                    Id = x.Id,
                    Mensaje = x.Mensaje,
                    TipoMensaje = x.TipoMensaje,
                    MensajeCorreo = x.MensajeCorreo
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MensajeWhatsappReadDto>> GetById(string id)
        {
            var entity = await _db.MensajesWhatsapp.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null) return NotFound();

            return Ok(new MensajeWhatsappReadDto
            {
                Id = entity.Id,
                Mensaje = entity.Mensaje,
                TipoMensaje = entity.TipoMensaje,
                MensajeCorreo = entity.MensajeCorreo
            });
        }

        [HttpPost]
        public async Task<ActionResult<MensajeWhatsappReadDto>> Create([FromBody] MensajeWhatsappCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = new MensajeWhatsappUsuario
            {
                Id = Guid.NewGuid().ToString(),
                Mensaje = dto.Mensaje.Trim(),
                TipoMensaje = dto.TipoMensaje.Trim(),
                MensajeCorreo = string.IsNullOrWhiteSpace(dto.MensajeCorreo) ? null : dto.MensajeCorreo.Trim()
            };

            _db.MensajesWhatsapp.Add(entity);
            await _db.SaveChangesAsync();

            var read = new MensajeWhatsappReadDto
            {
                Id = entity.Id,
                Mensaje = entity.Mensaje,
                TipoMensaje = entity.TipoMensaje,
                MensajeCorreo = entity.MensajeCorreo
            };

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, read);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MensajeWhatsappReadDto>> Update(string id, [FromBody] MensajeWhatsappUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = await _db.MensajesWhatsapp.FirstOrDefaultAsync(x => x.Id == id);
            if (entity is null) return NotFound();

            entity.Mensaje = dto.Mensaje.Trim();
            entity.TipoMensaje = dto.TipoMensaje.Trim();
            entity.MensajeCorreo = string.IsNullOrWhiteSpace(dto.MensajeCorreo) ? null : dto.MensajeCorreo.Trim();

            await _db.SaveChangesAsync();

            return Ok(new MensajeWhatsappReadDto
            {
                Id = entity.Id,
                Mensaje = entity.Mensaje,
                TipoMensaje = entity.TipoMensaje,
                MensajeCorreo = entity.MensajeCorreo
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _db.MensajesWhatsapp.FirstOrDefaultAsync(x => x.Id == id);
            if (entity is null) return NotFound();

            _db.MensajesWhatsapp.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}