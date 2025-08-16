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
            List<DeudasClienteOutDTO> deudoresDTO = deudas.Adapt<List<DeudasClienteOutDTO>>();
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

                var compromisos = query
                    .Where(x => x.Estado == true)
                    .Select(c => new CompromisoPagoOutDTO
                    {
                        CompromisoPagoId = c.IdCompromiso,
                        DeudaId = c.IdDeuda != null ? c.IdDeuda.Value : Guid.Empty,
                        DeudaCapital = c.IdDeudaNavigation != null ? (c.IdDeudaNavigation.DeudaCapital ?? 0m) : 0m,
                        Interes = c.IdDeudaNavigation != null ? (c.IdDeudaNavigation.Interes ?? 0m) : 0m,
                        GastosCobranzas = c.IdDeudaNavigation != null ? (c.IdDeudaNavigation.GastosCobranzas ?? 0m) : 0m,
                        SaldoDeuda = c.IdDeudaNavigation != null ? (c.IdDeudaNavigation.SaldoDeuda ?? 0m) : 0m,
                        DiasMora = c.IdDeudaNavigation != null ? (c.IdDeudaNavigation.DiasMora ?? 0) : 0,
                        FechaVenta = c.IdDeudaNavigation != null ? c.IdDeudaNavigation.FechaVenta : null,
                        FechaUltimoPago = c.IdDeudaNavigation != null ? c.IdDeudaNavigation.FechaUltimoPago : null,
                        CedulaCliente = c.IdDeudaNavigation != null && c.IdDeudaNavigation.IdDeudorNavigation != null
                            ? c.IdDeudaNavigation.IdDeudorNavigation.IdDeudor
                            : "",
                        NumeroFactura = c.IdDeudaNavigation != null ? c.IdDeudaNavigation.NumeroFactura : "",
                        NombreCliente = c.IdDeudaNavigation != null && c.IdDeudaNavigation.IdDeudorNavigation != null
                            ? c.IdDeudaNavigation.IdDeudorNavigation.Nombre
                            : "",
                        NumeroCouta = c.IdDeudaNavigation != null
                            ? (c.IdDeudaNavigation.Creditos.ToString() + "/" + c.IdDeudaNavigation.NumeroCuotas.ToString())
                            : "",
                        ValorCuota = c.IdDeudaNavigation != null ? (c.IdDeudaNavigation.ValorCuota ?? 0m) : 0m,
                        Empresa = c.IdDeudaNavigation != null && c.IdDeudaNavigation.Empresa != null
                            ? c.IdDeudaNavigation.Empresa
                            : "",
                    })
                    .ToList();

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
            List<string> deudoresExistentes = _context.Deudores.Select(x => x.IdDeudor).ToList();
            var telefonosValidos = telefonosDeudores
                .Where(t => deudoresExistentes.Contains(t.cedula))  
                .ToList();

            if (!telefonosValidos.Any())
            {
                return BadRequest("Ninguno de los deudores proporcionados existe en la base de datos");
            }

            TypeAdapterConfig config = new TypeAdapterConfig();
            ConfigTelefonoDeudor.Register(config);

            List<DeudorTelefono> telefonosParaGuardar = telefonosValidos.Adapt<List<DeudorTelefono>>(config);
            _context.DeudorTelefonos.AddRange(telefonosParaGuardar);
            _context.SaveChanges();

            int omitidos = telefonosDeudores.Count - telefonosValidos.Count;
            string mensaje = $"Se grabaron {telefonosValidos.Count} teléfonos exitosamente";

            if (omitidos > 0)
            {
                mensaje += $". Se omitieron {omitidos} teléfonos porque sus deudores no existen";
            }

            return Ok(mensaje);
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

            List<Deudores> clientes = usuario.Rol == "user"
                                     ? _context.Deudores.Where(x => x.IdUsuario == usuario.IdUsuario).ToList()
                                     : _context.Deudores.ToList();

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
