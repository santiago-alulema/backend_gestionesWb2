using gestiones_backend.helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZipController : ControllerBase
    {
        private readonly ZipExtractService _zipService;

        public ZipController(ZipExtractService zipService)
        {
            _zipService = zipService;
        }
        
        [HttpGet("descomprimir")]
        public IActionResult Descomprimir()
        {
            _zipService.DescomprimirZips();
            return Ok("Todos los .zip fueron descomprimidos en ArchivosExternos.");
        }

        [HttpGet("descomprimir-ultimo")]
        public IActionResult DescomprimirUltimo()
        {
            _zipService.DescomprimirZipsUltimo();
            return Ok("Todos los .zip fueron descomprimidos en ArchivosExternos.");
        }
    }
}
