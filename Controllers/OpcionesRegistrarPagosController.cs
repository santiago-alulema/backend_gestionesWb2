using gestiones_backend.Context;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Entity;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpcionesRegistrarPagosController : ControllerBase
    {
        private readonly DataContext _context;

        public OpcionesRegistrarPagosController(DataContext context)
        {
            _context = context;
        }


        [HttpGet("bancos-activos")]
        public IActionResult ObtenerBancosActivos()
        {
            List<BancosPagos> resultado = _context.BancosPagos.Where(x => x.Activo == true).ToList();
            List<SeleccionGeneralOutDTO> gestionesDto = resultado.Adapt<List<SeleccionGeneralOutDTO>>();
            return Ok(gestionesDto);
        }

        [HttpGet("cuentas-activos")]
        public IActionResult ObtenerCuencasActivas()
        {
            List<TipoCuentaBancaria> resultado = _context.TiposCuentaBancaria.Where(x => x.Activo == true).ToList();
            List<SeleccionGeneralOutDTO> gestionesDto = resultado.Adapt<List<SeleccionGeneralOutDTO>>();
            return Ok(gestionesDto);
        }

        [HttpGet("transacciones-activas")]
        public IActionResult ObtenerTipoTransaccion()
        {
            List<TipoTransaccion> resultado = _context.TiposTransaccion.Where(x => x.Activo == true).ToList();
            List<SeleccionGeneralOutDTO> gestionesDto = resultado.Adapt<List<SeleccionGeneralOutDTO>>();
            return Ok(gestionesDto);
        }

        [HttpGet("abono-liquidacion-activas")]
        public IActionResult ObtenerLiquidacionAbonos()
        {
            List<AbonoLiquidacion> resultado = _context.AbonosLiquidacion.Where(x => x.Activo == true).ToList();
            List<SeleccionGeneralOutDTO> gestionesDto = resultado.Adapt<List<SeleccionGeneralOutDTO>>();
            return Ok(gestionesDto);
        }
    }
}
