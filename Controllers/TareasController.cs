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
    public class TareasController : ControllerBase
    {
        private readonly ITareasService _tareasService;

        public TareasController(ITareasService tareasService)
        {
            _tareasService = tareasService;
        }

        [HttpGet]
        public  ActionResult<List<TareaDto>> GetAll()
        {
            var tareas =  _tareasService.GetAllAsync();
            return Ok(tareas);
        }

        [HttpPut("{id}")]
        public  ActionResult<TareaDto> Update(string id, [FromBody] UpdateTareaDto dto)
        {
            var tarea =  _tareasService.UpdateAsync(id, dto);
            if (tarea == null)
                return NotFound(new { message = "Tarea no encontrada" });

            return Ok(tarea);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var eliminado =  _tareasService.DeleteAsync(id);
            if (!eliminado)
                return NotFound(new { message = "Tarea no encontrada" });

            return NoContent();
        }
    }
}
