using gestiones_backend.helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolderController : ControllerBase
    {
        private readonly FolderCleanService _folderService;

        public FolderController(FolderCleanService folderService)
        {
            _folderService = folderService;
        }

        [HttpDelete("limpiar")]
        public IActionResult Limpiar()
        {
            _folderService.LimpiarCarpeta();
            return Ok("El contenido de la carpeta ArchivosExternos fue eliminado correctamente.");
        }
    }
}
