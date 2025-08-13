using gestiones_backend.Context;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Entity;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoGestionController : ControllerBase
    {
        private readonly DataContext _context;

        public TipoGestionController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("tipo-gestion-padre")]
        public IActionResult TipoGestionPadre() {
            List<TiposGestion> gestiones = _context.TiposGestion.Where( x => x.IdPadre == null &&
                                                                             x.Estado == true).ToList();
            List<TipoGestionOutDTO> gestionesDto = gestiones.Adapt<List<TipoGestionOutDTO>>();
            return Ok(gestionesDto);
        }

        [HttpGet("tipo-gestion-por-padre/{CodigoPadre}")]
        public IActionResult TipoGestionPorPadre(string CodigoPadre)
        {
            List<TiposGestion> gestiones = _context.TiposGestion.Where(x => x.Estado == true && 
                                                                             x.IdPadre == CodigoPadre).ToList();
            List<TipoGestionOutDTO> gestionesDto = gestiones.Adapt<List<TipoGestionOutDTO>>();
            return Ok(gestionesDto);
        }
    }
}
