using gestiones_backend.Context;
using gestiones_backend.Entity.temp_crecos;
using gestiones_backend.helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql.Bulk;
using System.IO.Compression;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrecosMetodosController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _env;
        private readonly SftpDownloadService _sftpDownloadService;
        private readonly FolderCleanService _limpiarCarpeta;


        public CrecosMetodosController(
            DataContext dataContext,
            IWebHostEnvironment env,
            SftpDownloadService sftpDownloadService,
            FolderCleanService limpiarCarpeta
        )
        {
            _dataContext = dataContext;
            _env = env;
            _sftpDownloadService = sftpDownloadService;
            _limpiarCarpeta = limpiarCarpeta;
        }

        [HttpPost("grabar-campania")]
        public IActionResult GrabarCampaniaCrecos(List<TrifocusCrecosPartes> crecosPartes, [FromQuery] Boolean borrarTodo =  false)
        {
            if (borrarTodo)
            {
                _dataContext.Database.ExecuteSqlRaw(@"
                    TRUNCATE TABLE temp_crecos.trifocuscrecospartes 
                    RESTART IDENTITY CASCADE;
                ");
            }

            var bulk = new NpgsqlBulkUploader(_dataContext);
            bulk.Insert(crecosPartes);
            return Ok("Las liquidaciones de Crecos por parte se insertaron correctamente");
        }

        [HttpGet("descargar-trifocus-zip-ultimo-dia")]
        public IActionResult DescargarTrifocusZipUltimoDia()
        {
            try
            {
                _limpiarCarpeta.LimpiarCarpeta();
                _sftpDownloadService.DescargarZipsUltimoDia();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error descargando desde SFTP: {ex.Message}");
            }
            return Ok("Se realizo correctamente");
        }

        [HttpGet("descargar-trifocus-zip")]
        public IActionResult DescargarTrifocusZip()
        {
            try
            {
                _limpiarCarpeta.LimpiarCarpeta();
                _sftpDownloadService.DescargarZips();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error descargando desde SFTP: {ex.Message}");
            }

            var carpetaArchivos = Path.Combine(_env.ContentRootPath, "ArchivosExternos");

            if (!Directory.Exists(carpetaArchivos))
                return NotFound("No existe la carpeta de archivos externos.");

            var tempFolder = Path.GetTempPath();
            var nombreZip = $"Trifocus_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
            var rutaZip = Path.Combine(tempFolder, nombreZip);

            if (System.IO.File.Exists(rutaZip))
                System.IO.File.Delete(rutaZip);

            ZipFile.CreateFromDirectory(carpetaArchivos, rutaZip, CompressionLevel.Fastest, false);

            var bytes = System.IO.File.ReadAllBytes(rutaZip);

            System.IO.File.Delete(rutaZip);

            return File(bytes, "application/zip", nombreZip);
        }
    }
}
