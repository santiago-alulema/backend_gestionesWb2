using gestiones_backend.Context;
using gestiones_backend.Dtos.In;
using gestiones_backend.helpers;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel oLoginCLS)
        {

            try
            {
                var user = await _context.Usuarios
                         .Where(x => x.NombreUsuario == oLoginCLS.Username && x.Contrasena == oLoginCLS.Password)
                         .FirstOrDefaultAsync();

                if (user != null)
                {
                    var token = _jwtService.GenerateToken(user.NombreUsuario, user.Rol, oLoginCLS.RememberMe);
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
