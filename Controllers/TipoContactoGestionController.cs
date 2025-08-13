using gestiones_backend.Context;
using gestiones_backend.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoContactoGestionController : ControllerBase
    {
        private readonly DataContext _context;

        public TipoContactoGestionController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoContactoGestion>>> GetTiposContactoGestion()
        {
            return await _context.TiposContactoGestion.Where(x => x.Estado == true).ToListAsync();
        }
    }
}
