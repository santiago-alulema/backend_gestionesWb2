using gestiones_backend.helpers;
using gestiones_backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SftpController : ControllerBase
    {
        private readonly SftpDownloadService _sftpService;
        private readonly ITrifocusExcelUploader _trifocusExcelUploader;


        public SftpController(SftpDownloadService sftpService,ITrifocusExcelUploader trifocusExcelUploader)
        {
            _sftpService = sftpService;
            _trifocusExcelUploader= trifocusExcelUploader;
        }

        [HttpGet("descargar")]
        public IActionResult DescargarArchivos()
        {
            _sftpService.DescargarZips();
            return Ok("Archivos .zip descargados en la carpeta ArchivosExternos.");
        }

        [HttpGet("subir-archivo-trifocus")]
        public IActionResult SubirExcelTrifocus()
        {
           // _sftpService.DescargarZips();
            _trifocusExcelUploader.GenerateAndUploadAsync();
            return Ok("Archivos .zip descargados en la carpeta ArchivosExternos.");
        }
    }
}