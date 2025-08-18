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

       
    }
}
