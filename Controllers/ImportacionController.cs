using gestiones_backend.Context;
using gestiones_backend.Entity;
using gestiones_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportacionController : ControllerBase
    {
        private readonly DeudoresImportService svc;
        private readonly DataContext _dataContext;
        private readonly DeudoresImportService _deudoresImportService;

        public ImportacionController(DeudoresImportService _svc, 
            DataContext datacontex,
            DeudoresImportService deudoresImportService )
        {
            svc = _svc;
            _dataContext = datacontex;
            _deudoresImportService = deudoresImportService;
        }


        [HttpPost("deudores/insertar-deudas-crecos")]
        public async Task<IActionResult> ImportarDeudas()
        {
            // _deudoresImportService.RedimencionarDeudasNoGestionadas();
            _deudoresImportService.importarPagos();
            return Ok("Se insertó y actualizó correctamente");
        }

        [HttpPost("deudores/completo")]
        public async Task<IActionResult> ImportarDeudoresCompleto()
        {
            //var deudas = await svc.ImportarDeudasBasicoAsync();
            await svc.ImportarDeudoresCompletoAsync();
            //await svc.ImportarTelefonosBasicoAsync();
            //svc.GrabarTablas();
          // svc.ImportarDeudas();

            return Ok(new { registrosAfectados = 2, telefonosAgregados = 2 });
        }

        
        [HttpGet("actualizar-deudas-crecos")]
        public async Task<IActionResult> ActualizarDeudasCrecos()
        {
            svc.ImportarDeudas();
            return Ok(new { registrosAfectados = 2, telefonosAgregados = 2 });
        }

        [HttpGet("actualizar-deudas-crecos-sin-campania")]
        public async Task<IActionResult> ActualizarDeudasCrecosSinCampania()
        {
            svc.ImportarDeudasSinCampania();
            return Ok(new { registrosAfectados = 2, telefonosAgregados = 2 });
        }

        [HttpGet("grabar-tablas-crecos")]
        public async Task<IActionResult> GrabarTablaCrecos()
        {
            svc.GrabarTablas();
            return Ok(new { registrosAfectados = 2, telefonosAgregados = 2 });
        }


        [HttpGet("importar-pagos-crecos")]
        public async Task<IActionResult> ImportarPagosCrecos()
        {
            svc.importarPagos();
            return Ok(new { registrosAfectados = 2, telefonosAgregados = 2 });
        }






    }
}
