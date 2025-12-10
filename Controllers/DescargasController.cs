using gestiones_backend.helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DescargasController : ControllerBase
    {
        private readonly SftpDownloadService _sftpService;

        public DescargasController(SftpDownloadService sftpService)
        {
            _sftpService = sftpService;
        }

        [HttpGet("descargar-zip-especifico")]
        public IActionResult DescargarZip([FromQuery] string nombreEspecifico)
        {
            if (string.IsNullOrWhiteSpace(nombreEspecifico))
                return BadRequest("El parámetro 'nombreEspecifico' es obligatorio.");

            var result = _sftpService.DescargarArchivoEspecifico(nombreEspecifico);

            if (result is null)
                return NotFound($"No se encontró ningún archivo .zip que contenga: {nombreEspecifico}");

            var (fileBytes, fileName) = result.Value;

            Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition");

            return File(fileBytes, "application/zip", fileName);
        }

        [HttpGet("obtener-nombres-archivos")]
        public IActionResult ObtenerNombresarchivosCrecos()
        {
            return Ok(_sftpService.ObtenerNombresArchivos());
        }
    }
}
