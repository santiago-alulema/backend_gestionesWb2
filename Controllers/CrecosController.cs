using gestiones_backend.Context;
using gestiones_backend.Dtos.In;
using gestiones_backend.Entity.temp_crecos;
using gestiones_backend.helpers;
using gestiones_backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrecosController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMetodosCrecos _metodosCrecos;

        public CrecosController(DataContext context,
                                IMetodosCrecos metodosCrecos)
        {
            _context = context;
            _metodosCrecos = metodosCrecos;
        }

       
        [HttpGet("cartera")]
        public  ActionResult GetCartera()
        {
            var data =  _context.CarteraAsignadaCrecos
                        .AsNoTracking()
                        .ToList();

            return Ok(data);
        }

        [HttpPost("subir-cartera-manual")]
        public async Task<ActionResult> SubirCarteraManual([FromBody]List<DeudasCrecosMasivoInDto> deudas, CancellationToken ct)
        {
            await _metodosCrecos.SubirDeudasCrecosMasivomanual(deudas, ct);
            return Ok("Se actualizo las deudas creos subidas exitosamente");
        }


        [HttpGet("saldos")]
        public ActionResult GetSaldos()
        {
            var data =  _context.SaldoClienteCrecos
                            .AsNoTracking()
                            .ToList();

            return Ok(data);
        }

   
        [HttpGet("recibos")]
        public ActionResult GetRecibos()
        {
            var data =  _context.ReciboPagosCrecos
                        .AsNoTracking()
                        .ToList();

            return Ok(data);
        }

    
        [HttpGet("recibos-detalle")]
        public ActionResult GetRecibosDetalle()
        {
            var data =  _context.ReciboDetalleCrecos
                        .AsNoTracking()
                        .ToList();

            return Ok(data);
        }

        [HttpGet("cuotas")]
        public ActionResult GetCuotas()
        {
            var data =  _context.CuotasOperacionCrecos
                        .AsNoTracking().Take(1000)
                        .ToList();

            return Ok(data);
        }

        [HttpGet("reasignar-cartera-usuario-vacio")]
        public ActionResult ReassignarCarteraCrecosUsuarioNull()
        {
            var data = _metodosCrecos.AsignacionAutomaticaDeudasCrecos();
            return Ok(data);
        }

        [HttpGet("remover-usuarios-deuda-crecos/{IdUsuario}")]
        public ActionResult RemoverUsuarioDeudaCrecos(string IdUsuario)
        {
            var data = _metodosCrecos.AsignardeudaNullIdUsuario(IdUsuario);
            return Ok(data);
        }

        [HttpGet("cartera-excel")]
        public IActionResult GetCarteraExcel()
        {
            var data = _context.CarteraAsignadaCrecos
                .AsNoTracking()
                .ToList();

            var excelBytes = ExcelHelper.ListToExcel(data, "Cartera");

            return File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"cartera_crecos_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            );
        }

        [HttpGet("saldos-excel")]
        public IActionResult GetSaldosExcel()
        {
            var data = _context.SaldoClienteCrecos
                .AsNoTracking()
                .ToList();

            var excelBytes = ExcelHelper.ListToExcel(data, "Saldos");

            return File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"saldos_crecos_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            );
        }

        [HttpGet("recibos-excel")]
        public IActionResult GetRecibosExcel()
        {
            var data = _context.ReciboPagosCrecos
                .AsNoTracking()
                .ToList();

            var excelBytes = ExcelHelper.ListToExcel(data, "Recibos");

            return File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"recibos_crecos_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            );
        }

        [HttpGet("recibos-detalle-excel")]
        public IActionResult GetRecibosDetalleExcel()
        {
            var data = _context.ReciboDetalleCrecos
                .AsNoTracking()
                .ToList();

            var excelBytes = ExcelHelper.ListToExcel(data, "Detalle Recibos");

            return File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"detalle_recibos_crecos_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            );
        }

        [HttpGet("cuotas-excel")]
        public IActionResult GetCuotasExcel()
        {
            var data = _context.CuotasOperacionCrecos
                .AsNoTracking()
                .ToList();

            var excelBytes = ExcelHelper.ListToExcel(data, "Cuotas");

            return File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"cuotas_crecos_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            );
        }

    }
}
