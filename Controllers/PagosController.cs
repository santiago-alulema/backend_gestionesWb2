using gestiones_backend.Context;
using gestiones_backend.Dtos.In;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Interfaces;
using gestiones_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
        private readonly IPagosService _pagosService;

        public PagosController(IPagosService pagosService)
        {
            _pagosService = pagosService;
        }

        [HttpGet]
        public ActionResult<List<PagoDto>> GetAll()
        {
            var pagos = _pagosService.GetAllAsync();
            return Ok(pagos);
        }

        [HttpPut("editar/{id}")]
        public  ActionResult<PagoDto> Update(string id, [FromBody] UpdatePagoDto dto)
        {
            var pago =  _pagosService.UpdateAsync(id, dto);
            if (pago == null)
                return NotFound(new { message = "Pago no encontrado" });
            return Ok(pago);
        }

        [HttpDelete("eliminar/{id}")]
        public  ActionResult Delete(string id)
        {
            var eliminado =  _pagosService.DeleteAsync(id);
            if (!eliminado)
                return NotFound(new { message = "Pago no encontrado" });

            return NoContent();
        }

    }
}
