using gestiones_backend.Context;
using gestiones_backend.Entity;
using gestiones_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormaPagoController : ControllerBase
    {
        private readonly DataContext _context;

        public FormaPagoController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FormaPago>>> GetFormasPago()
        {
            return await _context.FormasPago.Where(x => x.Estado == true).ToListAsync();
        }

    }
}
