using gestiones_backend.Context;
using gestiones_backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialClienteController : ControllerBase
    {
        private readonly IGestionarImagenes _imageService;
        private readonly DataContext _dataContext;
        private readonly ILogger<ImagesController> _logger;
        private readonly string _imagorBaseUrl;

        public HistorialClienteController(
                                DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("obtener-clientes/{buscador}")]
        public IActionResult obtenerDeudaCliente(string buscador)
        {
            var resultadoClientes = _dataContext.Deudores.Where(x => x.IdDeudor.ToUpper().Contains(buscador.ToUpper()) ||
                                                                      x.Nombre.ToUpper().Contains(buscador.ToUpper()));

            var deudores = resultadoClientes.Select(x => new { id = x.IdDeudor, name = x.Nombre }).ToList();
            return Ok(deudores);
        }


    }
}
