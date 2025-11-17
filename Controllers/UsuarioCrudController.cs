using gestiones_backend.Context;
using gestiones_backend.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioCrudController : ControllerBase
    {
        private readonly DataContext _context;

        public UsuarioCrudController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetUsuariosActivos()
        {
            var usuarios = await _context.Usuarios
                .Where(u => u.EstaActivo)
                .ToListAsync();

            var usuariosDto = usuarios.Select(u => new
            {
                u.IdUsuario,
                u.NombreUsuario,
                u.Rol,
                u.Contrasena,
                u.Email,
                u.Telefono,
                u.NombreCompleto,
                u.CodigoUsuario,
                Estado = u.EstaActivo ? "ACTIVO" : "INACTIVO"
            });

            return Ok(usuariosDto);
        }

        [HttpGet("todos")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetTodosUsuarios()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(string id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> CrearUsuario([FromBody] Usuario usuario)
        {
            usuario.EstaActivo = true;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(string id, [FromBody] Usuario usuarioActualizado)
        {
            if (id != usuarioActualizado.IdUsuario)
                return BadRequest("El Id del usuario no coincide.");

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            usuario.NombreUsuario = usuarioActualizado.NombreUsuario;
            usuario.Rol = usuarioActualizado.Rol;
            usuario.Contrasena = usuarioActualizado.Contrasena;
            usuario.Email = usuarioActualizado.Email;
            usuario.Telefono = usuarioActualizado.Telefono;
            usuario.NombreCompleto = usuarioActualizado.NombreCompleto;
            usuario.CodigoUsuario = usuarioActualizado.CodigoUsuario;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(string id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound("no existe usuario.");

            if (!usuario.EstaActivo)
                return NotFound("esta inactivo");

            usuario.EstaActivo = false;
            await _context.SaveChangesAsync();

            return Ok("Se inactivo correctamente");
        }
    }
}
