using gestiones_backend.Context;
using gestiones_backend.Entity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace gestiones_backend.Services
{

    public class AuthenticationService : Interfaces.IAuthenticationService // Implementa la interfaz
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _context;

        public AuthenticationService(IHttpContextAccessor httpContextAccessor, DataContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public Usuario GetCurrentUser()
        {
            var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                throw new UnauthorizedAccessException("No se encontró información de autenticación");
            }

            var claims = identity.Claims;
            string username = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                throw new UnauthorizedAccessException("Token no contiene el claim Name");
            }

            Usuario usuario = _context.Usuarios
                .FirstOrDefault(x => x.NombreUsuario == username);

            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Usuario no encontrado en la base de datos");
            }

            return usuario;
        }

        public async Task<Usuario> GetCurrentUserAsync()
        {
            var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                throw new UnauthorizedAccessException("No se encontró información de autenticación");
            }

            var claims = identity.Claims;
            string username = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                throw new UnauthorizedAccessException("Token no contiene el claim Name");
            }

            Usuario usuario = await _context.Usuarios
                .FirstOrDefaultAsync(x => x.NombreUsuario == username);

            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Usuario no encontrado en la base de datos");
            }

            return usuario;
        }
    }
}