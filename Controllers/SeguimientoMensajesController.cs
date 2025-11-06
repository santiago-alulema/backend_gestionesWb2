using gestiones_backend.Context;
using gestiones_backend.Entity;
using gestiones_backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeguimientoMensajesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IAuthenticationService _authService;
        private readonly IConfiguration _configuration;
        private readonly IGestionesService _gestionesService;


        public SeguimientoMensajesController(DataContext context,
                                IAuthenticationService authService,
                                IConfiguration configuration,
                                IGestionesService gestionesService)
        {
            _context = context;
            _authService = authService;
            _configuration = configuration;
            _gestionesService = gestionesService;
        }

        [HttpPost("grabar-Mensaje")]
        public IActionResult actionResultAudi([FromBody] SeguimientoMensajes seguimiento )
        {
            seguimiento.fechaRegistro = DateTime.Now;
            _context.SeguimientoMensajes.Add(seguimiento);
            _context.SaveChanges();
            return Ok("Exito");
        }
    }
}
