using gestiones_backend.ConfigurationsMapper;
using gestiones_backend.Context;
using gestiones_backend.Dtos.In;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Entity;
using Mapster;
using gestiones_backend.Interfaces;
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
        private readonly IAuthenticationService _authService;

        public ClienteController(DataContext context, IAuthenticationService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet("deudas-por-cliente/{cedulaCliente}")]
        public IActionResult DeudasPorCliente(string cedulaCliente, [FromQuery] string? empresa, [FromQuery] bool sinGestionar = false)
        {
            IQueryable<Deuda> deudasQuery = _context.Deudas.AsNoTracking().Where(x => x.IdDeudor == cedulaCliente);

            if (!string.IsNullOrEmpty(empresa) && empresa != "TODOS")
            {
                deudasQuery = deudasQuery.Where(d =>
                    d.Empresa != null &&
                    d.Empresa.ToUpper() == empresa.ToUpper());
            }

            if (sinGestionar)
            {
                deudasQuery = deudasQuery.Where(d =>
                    !d.CompromisosPagos.Any() &&
                    !d.Gestiones.Any() &&
                    !d.Pagos.Any());

            }
            List<Deuda> deudas = deudasQuery.ToList();
            List<DeudasClienteOutDTO> deudoresDTO = deudas.Adapt<List<DeudasClienteOutDTO>>();
            return Ok(deudoresDTO);
        }

        [HttpGet("buscar-deuda-por-id/{idDeuda}")]
        public IActionResult DeudasPorId(string idDeuda)
        {
            Deuda deudasQuery = _context.Deudas.AsNoTracking().FirstOrDefault(x => x.IdDeuda == Guid.Parse(idDeuda));
            if (deudasQuery == null)
            {
                return BadRequest("No existe deuda;");
            }

            DeudasClienteOutDTO deudoresDTO = deudasQuery.Adapt<DeudasClienteOutDTO>();
            return Ok(deudoresDTO);
        }

        [HttpGet("buscar-cliente-por-id/{clienteId}")]
        public IActionResult Allclients(string? clienteId)
        {
            Usuario usuario = _authService.GetCurrentUser();

            Deudores clientesQuery = _context.Deudores.Include(x => x.Deuda)
                                    .Include(x => x.Usuario)
                                    .FirstOrDefault(x => x.IdDeudor == clienteId);

            ClientesOutDTO deudoresDTO = new ClientesOutDTO()
            {
                cedula = clientesQuery.IdDeudor,
                nombre = clientesQuery.Nombre,
                telefono = clientesQuery.Telefono,
                direccion = clientesQuery.Direccion,
                descripcion = clientesQuery.Descripcion,
                correo = clientesQuery.Correo,
                numeroDeudas = clientesQuery.Deuda.Count().ToString(),
                tramos = clientesQuery.Deuda != null ?
                        String.Join("", clientesQuery.Deuda.Select((x, index) => $"<strong>{index + 1}</strong>: {x.Tramo} <br>")) :
                        string.Empty,
                gestor = clientesQuery.Usuario.NombreCompleto
            };

            return Ok(deudoresDTO);
        }


        [HttpGet("listar-compromisos-pago/{esHoy}")]
        public IActionResult GestionarCompromisos(bool esHoy)
        {
            Usuario usuario = _authService.GetCurrentUser();
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            IQueryable<CompromisosPago> query = _context.CompromisosPagos
                .Include(x => x.IdDeudaNavigation)
                .ThenInclude(x => x.IdDeudorNavigation)
                .Include(x => x.IdTipoTareaNavigation)
                ;

            if (esHoy)
            {
                query = query.Where(c => c.FechaCompromiso == hoy);
            }

            if (usuario.Rol == "user")
            {
                query = query.Where(x => x.Estado == true && x.IdUsuario == usuario.IdUsuario);
            }

            var compromisos = query
                .Select(c => new CompromisoPagoOutDTO
                {
                    CompromisoPagoId = c.IdCompromiso,
                    IdDeuda = c.IdDeuda != null ? c.IdDeuda.Value : Guid.Empty,
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
                        ? c.IdDeudaNavigation.NumeroCuotas.ToString()
                        : "",
                    ValorCuota = c.IdDeudaNavigation != null ? (c.IdDeudaNavigation.ValorCuota ?? 0m) : 0m,
                    Empresa = c.IdDeudaNavigation != null && c.IdDeudaNavigation.Empresa != null
                        ? c.IdDeudaNavigation.Empresa
                        : "",
                    HoraTarea = c.HoraRecordatorio,
                    TipoTarea = c.IdTipoTareaNavigation.Nombre,
                    ValorCompromisoPago = c.MontoComprometido.ToString() ?? "0",
                    MontoCobrar = c.IdDeudaNavigation.MontoCobrar.ToString(),
                    Tramo = c.IdDeudaNavigation.Tramo
                }).ToList();

            return Ok(compromisos);

        }

        [HttpPost("grabar-telefono-nuevo-cliente")]
        public IActionResult GrabarTelefonoNuevo([FromBody] GrabarTelefonoNuevoInDTO telefonosNuevoDeudores)
        {
            TypeAdapterConfig config = new TypeAdapterConfig();
            ConfigTelefonoDeudor.Register(config);

            DeudorTelefono TelefonosDeudores = telefonosNuevoDeudores.Adapt<DeudorTelefono>(config);
            TelefonosDeudores.Observacion = telefonosNuevoDeudores.origen;
            TelefonosDeudores.Origen = "Desde Web";
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
        public async Task<IActionResult> Allclients([FromQuery] string? empresa, [FromQuery] bool sinGestionar = false)
        {
            Usuario usuario = _authService.GetCurrentUser();

            // Construir query base
            IQueryable<Deudores> clientesQuery = _context.Deudores.AsNoTracking();

            // Filtrar por usuario si es necesario
            if (usuario.Rol == "user")
            {
                clientesQuery = clientesQuery.Where(x => x.IdUsuario == usuario.IdUsuario);
            }

            // Aplicar filtro de empresa
            if (!string.IsNullOrEmpty(empresa) && empresa != "TODOS")
            {
                var empresaUpper = empresa.ToUpper();
                clientesQuery = clientesQuery.Where(c =>
                    c.Deuda.Any(d => d.Empresa != null && d.Empresa.ToUpper() == empresaUpper));
            }

            // Aplicar filtro de sin gestionar
            if (sinGestionar)
            {
                clientesQuery = clientesQuery.Where(c =>
                    c.Deuda.Any(d =>
                        !d.CompromisosPagos.Any() &&
                        !d.Gestiones.Any() &&
                        !d.Pagos.Any()));
            }

            // Proyección en la base de datos (más eficiente)
            var deudoresDTO = await clientesQuery
                .Select(c => new
                {
                    c.IdDeudor,
                    c.Nombre,
                    c.Telefono,
                    c.Direccion,
                    c.Descripcion,
                    c.Correo,
                    NumeroDeudas = c.Deuda.Count(),
                    UsuarioNombre = c.Usuario.NombreCompleto,
                    Deudas = c.Deuda.Select(d => d.Tramo).ToList()
                })
                .ToListAsync();

            // Transformación final en memoria (solo lo necesario)
            var result = deudoresDTO.Select(c => new ClientesOutDTO()
            {
                cedula = c.IdDeudor,
                nombre = c.Nombre,
                telefono = c.Telefono,
                direccion = c.Direccion,
                descripcion = c.Descripcion,
                correo = c.Correo,
                numeroDeudas = c.NumeroDeudas.ToString(),
                tramos = c.Deudas.Any() ?
                        string.Join("", c.Deudas.Select((tramo, index) => $"<strong>{index + 1}</strong>: {tramo} <br>")) :
                        string.Empty,
                gestor = c.UsuarioNombre
            }).ToList();

            return Ok(result);
        }

        [HttpGet("listar-telefonos-activos-cliente/{cedulaCliente}")]
        public IActionResult ListarTelefonosActivosClientes(string cedulaCliente)
        {
            List<DeudorTelefono> deudorTelefonos = _context.DeudorTelefonos.Where(x => x.IdDeudor == cedulaCliente).ToList();
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
        public IActionResult VerificarTelefonoActivoCliente(string Telefono)
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
