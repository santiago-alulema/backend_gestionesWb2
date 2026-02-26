using gestiones_backend.Context;
using gestiones_backend.DbConn;
using gestiones_backend.Entity.temp_crecos;
using gestiones_backend.helpers;
using gestiones_backend.Interfaces;
using gestiones_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql.Bulk;
using OfficeOpenXml;
using System.Data;
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
        private readonly DeudoresImportService deudoresImportService; 
        private readonly ZipExtractService _zipExtractService;
        private readonly IConfiguration _configuration;
        private readonly ITrifocusExcelUploader _trifocusExcelUploader;

        public CrecosMetodosController(
            DataContext dataContext,
            IWebHostEnvironment env,
            SftpDownloadService sftpDownloadService,
            FolderCleanService limpiarCarpeta,
            DeudoresImportService _deudoresImportService,
            ZipExtractService zipExtractService,
            IConfiguration configuration,
            ITrifocusExcelUploader trifocusExcelUploader
        )
        {
            _dataContext = dataContext;
            _env = env;
            _sftpDownloadService = sftpDownloadService;
            _limpiarCarpeta = limpiarCarpeta;
            deudoresImportService = _deudoresImportService;
            _zipExtractService = zipExtractService;
            _configuration = configuration;
            _trifocusExcelUploader = trifocusExcelUploader;
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

        [HttpGet("cantidad-inconsistencia-cartera-saldo")]
        public IActionResult CantidadInconsistenciaCarteraSaldo()
        {
            string cadena = @$"select count(*) from temp_crecos.""CarteraAsignadaCrecosSinSaldoCliente"";";
            PgConn conn = new PgConn();
            conn.cadenaConnect = _configuration.GetConnectionString("DefaultConnection");
            DataTable resultado = conn.ejecutarconsulta_dt(cadena);
            return Ok(resultado.Rows[0][0]);
        }

        [HttpGet("descargar-excel-inconsistencias-cartera-saldo")]
        public IActionResult DescargarExcelInconsistenciasCarteraSaldo()
        {
            string cadena = @$"select * from temp_crecos.""CarteraAsignadaCrecosSinSaldoCliente"";";
            PgConn conn = new PgConn();
            conn.cadenaConnect = _configuration.GetConnectionString("DefaultConnection");
            DataTable resultado = conn.ejecutarconsulta_dt(cadena);
            ExcelPackage.License.SetNonCommercialPersonal("Santiago");
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells.LoadFromDataTable(resultado, true);

                var fileBytes = package.GetAsByteArray();
                var fileName = $"CARTERA_CRECOS_INCONSISTENCIAS_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(
                    fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
        }

        [HttpGet("sincronizar-ultimos-archivos-crecos")]
        [AllowAnonymous]
        public async Task<IActionResult> DescargarUltimosArchivosCrecos()
        {
            try
            {
                _limpiarCarpeta.LimpiarCarpeta();

                _sftpDownloadService.DescargarZipsUltimoDia();

                _zipExtractService.DescomprimirZipsUltimo();
                await deudoresImportService.ImportarDeudoresCompletoAsync();
                await deudoresImportService.ImportarTelefonosBasicoAsync();
                deudoresImportService.GrabarTablas();
              //  deudoresImportService.ImportarDeudas();
                deudoresImportService.importarPagos();
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

            ZipFile.CreateFromDirectory(carpetaArchivos, rutaZip, System.IO.Compression.CompressionLevel.Fastest, false);

            var bytes = System.IO.File.ReadAllBytes(rutaZip);

            System.IO.File.Delete(rutaZip);

            return File(bytes, "application/zip", nombreZip);
        }
    }
}
