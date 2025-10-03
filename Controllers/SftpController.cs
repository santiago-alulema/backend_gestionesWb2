using gestiones_backend.helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SftpController : ControllerBase
    {
        private readonly SftpDownloadService _sftpService;

        public SftpController(SftpDownloadService sftpService)
        {
            _sftpService = sftpService;
        }

        [HttpGet("descargar")]
        public IActionResult DescargarArchivos()
        {
            _sftpService.DescargarZips();
            return Ok("Archivos .zip descargados en la carpeta ArchivosExternos.");
        }
    }
}