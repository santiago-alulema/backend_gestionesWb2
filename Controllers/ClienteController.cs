using gestiones_backend.ConfigurationsMapper;
using gestiones_backend.Context;
using gestiones_backend.Dtos.In;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Entity;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly DataContext _context;

        public ClienteController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("deudas-por-cliente/{cedulaCliente}")]
        public IActionResult DeudasPorCliente(string cedulaCliente)
        {
            List<Deuda> deudas = _context.Deudas.Where(x => x.IdDeudor == cedulaCliente).ToList();
            List<DeudasClienteOutDTO> deudoresDTO = new();
            for (int i = 0; i < deudas.Count; i++)
            {
                deudoresDTO.Add(new DeudasClienteOutDTO()
                {
                    montoOriginal = deudas[i].MontoOriginal,
                    saldoActual = deudas[i].SaldoActual,
                    fechaVencimiento = deudas[i].FechaVencimiento,
                    fechaAsignacion = deudas[i].FechaAsignacion,
                    cedulaCliente = !string.IsNullOrEmpty(deudas[i].IdDeudor)
                                    ? deudas[i].IdDeudor
                                    : string.Empty,
                    descripcion = deudas[i].Descripcion,
                    deudaId = deudas[i].IdDeuda,
                    numeroFactura = deudas[i].NumeroFactura,
                    numeroCouta = deudas[i].CuotaActual ?? 0,
                    totalCuotas = deudas[i].NumeroCuotas ?? 0,
                    valorCuotas = deudas[i].ValorCuota ?? 0,
                    empresa = deudas[i].Empresa
                });
            }
            return Ok(deudoresDTO);
        }

        [HttpGet("listar-compromisos-pago/{esHoy}")]
        public IActionResult GestionarCompromisos(bool esHoy)
        {
                var hoy = DateOnly.FromDateTime(DateTime.Today);
                IQueryable<CompromisosPago> query = _context.CompromisosPagos
                    .Include(x => x.IdDeudaNavigation)
                    .ThenInclude(x => x.IdDeudorNavigation);

                if (esHoy)
                {
                    query = query.Where(c => c.FechaCompromiso == hoy);
                }

                var compromisos = query.Where(x => x.Estado== true).Select(c => new CompromisoPagoOutDTO
                {
                    compromisoPagoId = c.IdCompromiso,
                    deudaId = c.IdDeuda != null ? c.IdDeuda.Value : Guid.Empty,
                    montoOriginal = c.IdDeudaNavigation != null ? c.IdDeudaNavigation.MontoOriginal : 0,
                    saldoActual = c.IdDeudaNavigation != null ? c.IdDeudaNavigation.SaldoActual : 0,
                    fechaVencimiento = c.IdDeudaNavigation != null ? c.IdDeudaNavigation.FechaVencimiento : DateOnly.MinValue,
                    fechaAsignacion = c.IdDeudaNavigation != null ? c.IdDeudaNavigation.FechaAsignacion : null,
                    descripcion = c.IdDeudaNavigation != null ? c.IdDeudaNavigation.Descripcion : string.Empty,
                    cedulaCliente = c.IdDeudaNavigation != null && c.IdDeudaNavigation.IdDeudorNavigation != null
                        ? c.IdDeudaNavigation.IdDeudorNavigation.IdDeudor
                        : string.Empty,
                    numeroFactura = c.IdDeudaNavigation != null ? c.IdDeudaNavigation.NumeroFactura : string.Empty,
                    numeroCouta = c.IdDeudaNavigation != null ? c.IdDeudaNavigation.CuotaActual ?? 0 : 0,
                    totalCuotas = c.IdDeudaNavigation != null ? c.IdDeudaNavigation.NumeroCuotas ?? 0 : 0,
                    valorCuotas = c.IdDeudaNavigation != null ? c.IdDeudaNavigation.ValorCuota ?? 0 : 0,
                    nombreCliente = c.IdDeudaNavigation.IdDeudorNavigation.Nombre,
                    fechaCompromiso = c.FechaCompromiso,
                    montoCompromiso = c.MontoComprometido
                }).ToList();

                return Ok(compromisos);
            
        }

        [HttpPost("grabar-telefono-nuevo-cliente")]
        public IActionResult GrabarTelefonoNuevo([FromBody] GrabarTelefonoNuevoInDTO telefonosNuevoDeudores)
        {
            TypeAdapterConfig config = new TypeAdapterConfig();
            ConfigTelefonoDeudor.Register(config);

            DeudorTelefono TelefonosDeudores = telefonosNuevoDeudores.Adapt<DeudorTelefono>(config);

            _context.DeudorTelefonos.Add(TelefonosDeudores);
            _context.SaveChanges();

            return Ok("Se grabo los telefonos exitosamente");
        }

        [HttpPost("grabar-telefonos-cliente")]
        public IActionResult SavePhonesClients([FromBody] List<TelefonosDeudorInDTO> telefonosDeudores)
        {
            TypeAdapterConfig config = new TypeAdapterConfig();
            ConfigTelefonoDeudor.Register(config);

            List<DeudorTelefono> TelefonosDeudores = telefonosDeudores.Adapt<List<DeudorTelefono>>(config);

            _context.DeudorTelefonos.AddRange(TelefonosDeudores);
            _context.SaveChanges();

            return Ok("Se grabo los telefonos exitosamente");
        }

        [HttpGet("listar-clientes")]
        public IActionResult Allclients()
        {

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var claims = identity.Claims;
            string name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            Usuario usuario = _context.Usuarios.Where(x => x.NombreUsuario == name).FirstOrDefault();

            if (usuario == null)
            {
                throw new Exception("Token incorrecto");
            }

            List<Deudores> clientes = _context.Deudores.Where(x => x.IdUsuario == usuario.IdUsuario).ToList();
            List<ClientesOutDTO> deudoresDTO = new();
            for (int i = 0; i < clientes.Count; i++)
            {
                deudoresDTO.Add(new ClientesOutDTO()
                {
                    cedula = clientes[i].IdDeudor,
                    nombre = clientes[i].Nombre,
                    telefono = clientes[i].Telefono,
                    direccion = clientes[i].Direccion,
                    descripcion = clientes[i].Descripcion,
                    correo = clientes[i].Correo
                });
            }
            return Ok(deudoresDTO);
        }

        [HttpGet("listar-telefonos-activos-cliente/{cedulaCliente}")]
        public IActionResult ListarTelefonosActivosClientes(string cedulaCliente)
        {
            List<DeudorTelefono> deudorTelefonos = _context.DeudorTelefonos.Where( x => x.EsValido == true && x.IdDeudor == cedulaCliente).ToList();
            List<TelefonosActivosDeudorOutDTO> TelefonosDeudoresDTO = deudorTelefonos.Adapt<List<TelefonosActivosDeudorOutDTO>>();
            return Ok(TelefonosDeudoresDTO);
        }

        [HttpDelete("desactivar-telefono")]
        public IActionResult DesactivarTelefono(DesactivarTelefonoInDTO desactivarTelefono)
        {
            DeudorTelefono telefonoBuscado = _context.DeudorTelefonos.Where(x => x.IdDeudorTelefonos == desactivarTelefono.idTelefono).FirstOrDefault();
            if (telefonoBuscado == null)
            {
                throw new Exception("No existe un registro asociado con ese id");
            }
            telefonoBuscado.EsValido = false;
            telefonoBuscado.Observacion = desactivarTelefono.observacion;
            _context.DeudorTelefonos.Update(telefonoBuscado);
            _context.SaveChanges();
            return Ok("Se actualizo correctamente");
        }

        [HttpGet("verificar-telefono-activos-cliente/{Telefono}")]
        public IActionResult  VerificarTelefonoActivoCliente(string Telefono)
        {
            DeudorTelefono deudorTelefono = _context.DeudorTelefonos.Where(x => x.Telefono == Telefono).FirstOrDefault();
            VerificarTelefonoClienteOutDTO TelefonoEstadoDto = new();
            if (deudorTelefono == null)
            {
                TelefonoEstadoDto.Telefono = "";
                TelefonoEstadoDto.Estado = "NoExiste";
                return Ok(TelefonoEstadoDto);
            }
            if ((bool)!deudorTelefono.EsValido)
            {
                TelefonoEstadoDto.Telefono = deudorTelefono.Telefono;
                TelefonoEstadoDto.Estado = "Inactivo";
                TelefonoEstadoDto.Observacion = deudorTelefono.Observacion;
                return Ok(TelefonoEstadoDto);
            }
            TelefonoEstadoDto.Telefono = deudorTelefono.Telefono;
            TelefonoEstadoDto.Estado = "Existe";
            TelefonoEstadoDto.Observacion = deudorTelefono.Observacion;
            return Ok(TelefonoEstadoDto);
        }


        [HttpPut("actualizar-estado-compromiso")]
        public async Task<IActionResult> ActualizarEstadoCompromiso([FromBody] ActualizarEstadoCompromisoDTO dto)
        {
            var compromiso = await _context.CompromisosPagos.FindAsync(dto.IdCompromiso);
            if (compromiso == null) return NotFound();

            compromiso.Estado = dto.NuevoEstado;

            await _context.SaveChangesAsync();
            return Ok(compromiso);
        }
    }
}
