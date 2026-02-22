using gestiones_backend.Context;
using gestiones_backend.Dtos.In;
using gestiones_backend.Entity;
using gestiones_backend.helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly JwtTokenService _jwtService;

        public UsuariosController(DataContext context, JwtTokenService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }


        [HttpGet("obtener-lista-usuarios")]
        public async Task<IActionResult> ObtenerUsuarios()
        {
            List<Usuario> usuarios =  _context.Usuarios.Where(x => x.Rol == "user" || x.Rol == "admin").ToList();
            if (usuarios.Count() == 0)
                return BadRequest("No existen usuarios para listar");
            return Ok(usuarios.Select(x => new { id = x.IdUsuario, nombre = x.NombreCompleto }).ToList());
        }
        
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel oLoginCLS)
        {

            try
            {
                var user = await _context.Usuarios
                         .Where(x => x.NombreUsuario == oLoginCLS.Username && x.Contrasena == oLoginCLS.Password && x.EstaActivo)
                         .FirstOrDefaultAsync();

                if (user != null)
                {
                    var token = _jwtService.GenerateToken(user.NombreUsuario, user.Rol, oLoginCLS.RememberMe, user.NombreCompleto, user.Telefono);
                    return Ok(new { token });
                }
                else
                {
                    return BadRequest("Usuario o Contraseña incorrectas");
                }
                    return Unauthorized();

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
