using gestiones_backend.Context;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Entity;
using gestiones_backend.Services;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpcionesGestionController : ControllerBase
    {
        private readonly DataContext _context;

        public OpcionesGestionController( DataContext context)
        {
            _context = context;
        }

        [HttpGet("obtener-respuesta-gestion")]
        public IActionResult ObtenerrespuestaGestion() { 
            List<TipoResultado> resultado = _context.TiposResultado.Where(x => x.Activo == true).ToList();
            List<SeleccionGeneralOutDTO> gestionesDto = resultado.Adapt<List<SeleccionGeneralOutDTO>>();
            return Ok(gestionesDto);
        }

        [HttpGet("obtener-tipo-contacto/{TipoResultadoId}")]
        public IActionResult ObtenerRespuestaGestion(string TipoResultadoId)
        {
            List<TipoContactoResultado> resultado = _context.TiposContactoResultado.Where(x => x.Activo == true && x.TipoResultadoId == TipoResultadoId).ToList();
            List<SeleccionGeneralOutDTO> gestionesDto = resultado.Adapt<List<SeleccionGeneralOutDTO>>();
            return Ok(gestionesDto);
        }

        [HttpGet("obtener-tipos-tareas")]
        public IActionResult TiposTareas()
        {
            List<TipoTarea> resultado = _context.TiposTareas.Where(x => x.Estado == true ).ToList();
            List<SeleccionGeneralOutDTO> gestionesDto = resultado.Adapt<List<SeleccionGeneralOutDTO>>();
            return Ok(gestionesDto);
        }

        [HttpGet("obtener-todas-empresas")]
        public IActionResult ListaEmpresas()
        {
            var empresas = _context.Deudas
                .Where(d => !string.IsNullOrEmpty(d.Empresa))
                .Select(d => d.Empresa.Trim().ToUpper())
                .Distinct()
                .OrderBy(e => e)  
                .Select(e => new SeleccionGeneralOutDTO
                {
                    Id = e,
                    Nombre = e
                })
                .ToList();

            return Ok(empresas);
        }


        [HttpGet("obtener-respuesta-tipo-contacto/{TipoContactoResultadoId}")]
        public IActionResult ObtenerTipoContactoResultadoId(string TipoContactoResultadoId)
        {
            List<RespuestaTipoContacto> resultado = _context.RespuestasTipoContacto.Where(x => x.Activo == true && x.IdTipoContactoResultado == TipoContactoResultadoId).ToList();
            List<SeleccionGeneralOutDTO> gestionesDto = resultado.Adapt<List<SeleccionGeneralOutDTO>>();
            return Ok(gestionesDto);
        }

    }
}
