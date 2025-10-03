using gestiones_backend.Context;
using gestiones_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportacionController : ControllerBase
    {
        private readonly DeudoresImportService svc;
        public ImportacionController(DeudoresImportService _svc)
        {
            svc= _svc;
        }

        [HttpPost("deudores/completo")]
        public async Task<IActionResult> ImportarDeudoresCompleto()
        {
            var deudas = await svc.ImportarDeudasBasicoAsync();

            var afectados = await svc.ImportarDeudoresCompletoAsync();
            var telefonos = await svc.ImportarTelefonosBasicoAsync();

            return Ok(new { registrosAfectados = afectados, telefonosAgregados = telefonos });
        }
    }
}
