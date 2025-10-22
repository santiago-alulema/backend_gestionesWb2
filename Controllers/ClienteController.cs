using gestiones_backend.ConfigurationsMapper;
using gestiones_backend.Context;
using gestiones_backend.DbConn;
using gestiones_backend.Dtos.In;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Entity;
using gestiones_backend.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IAuthenticationService _authService;
        private readonly IConfiguration _configuration;
        private readonly IGestionesService _gestionesService;


        public ClienteController(DataContext context, 
                                IAuthenticationService authService, 
                                IConfiguration configuration,
                                IGestionesService gestionesService)
        {
            _context = context;
            _authService = authService;
            _configuration = configuration;
            _gestionesService = gestionesService;
        }


        [HttpGet("referencias-peronales/{cedulaCliente}")]
        public IActionResult ReferenciasCliente(string cedulaCliente)
        {
            string cadena = $@" select  rpc.""NOMBRE_REFERENCIA"" nombre,
		                                rpc.""DESCRIP_PROVINCIA"" provincia,
		                                rpc.""DESCRIP_CANTON"" canton,
		                                rpc.""DESCRIP_VINCULO"" vinculo,
		                                rpc.""NUMERO_REFERENCIA"" telefono 
                            from ""Deudores"" d 
                            join temp_crecos.""ReferenciasPersonalesCrecos"" rpc 
                                 on rpc.""NUM_IDENTIFICACION"" = d.""IdDeudor"" 
                            where d.""IdDeudor"" = '{cedulaCliente}';";

            PgConn conn = new PgConn();
            conn.cadenaConnect = _configuration.GetConnectionString("DefaultConnection");


            DataTable dataTable = conn.ejecutarconsulta_dt(cadena);
            string JSONString = string.Empty;
            JSONString = Newtonsoft.Json.JsonConvert.SerializeObject(dataTable);

            return Content(JSONString, "application/json");
        }

        [HttpGet("deudas-por-cliente/{cedulaCliente}")]
        public IActionResult DeudasPorCliente(string cedulaCliente, [FromQuery] string? empresa, [FromQuery] string opcionFiltro = "")
        {
            Usuario usuario = _authService.GetCurrentUser();

            IQueryable<Deuda> deudasQuery = _context.Deudas.AsNoTracking().Include(x => x.IdDeudorNavigation)
                                                            .Include(x => x.Usuario).Where(x => x.IdDeudor == cedulaCliente && 
                                                                                      x.EsActivo == true );

            if (usuario.Rol == "user")
            {
                deudasQuery = deudasQuery.Where(x => x.IdUsuario == usuario.IdUsuario);
            }

            if (!string.IsNullOrEmpty(empresa) && empresa != "TODOS")
            {
                deudasQuery = deudasQuery.Where(d =>
                    d.Empresa != null &&
                    d.Empresa.ToUpper() == empresa.ToUpper());
            }

            if (opcionFiltro == "SG")
            {
                deudasQuery = deudasQuery.Where(d =>
                    !d.CompromisosPagos.Any() &&
                    !d.Gestiones.Any() &&
                    !d.Pagos.Any());
            }

            if (opcionFiltro == "G")
            {
                deudasQuery = deudasQuery.Where(d =>
                    d.CompromisosPagos.Any() ||
                    d.Gestiones.Any() ||
                    d.Pagos.Any());
            }

            if (opcionFiltro == "IN")
            {
                deudasQuery = deudasQuery
                                .Where(d => d.CompromisosPagos.All(cp => cp.IncumplioCompromisoPago == true));
            }


            List<Deuda> deudas = deudasQuery.ToList();
            List<DeudasClienteOutDTO> deudoresDTO = deudas.Adapt<List<DeudasClienteOutDTO>>();
            foreach ( var item in deudoresDTO)
            {
                item.GestorUltimaGestion =_gestionesService.UltimoGestorGestionaDeuda(item.IdDeuda.ToString());
            }
            return Ok(deudoresDTO);
        }


        [HttpGet("deudas-por-cliente-global/{cedulaCliente}")]
        public IActionResult DeudasPorClienteGlobal(string cedulaCliente, [FromQuery] string? empresa)
        {
            Usuario usuario = _authService.GetCurrentUser();

            IQueryable<Deuda> deudasQuery = _context.Deudas.AsNoTracking()
                                                            .Include(x => x.IdDeudorNavigation)
                                                            .Include(x => x.Usuario)

                                                            .Where(x => (x.IdDeudor == cedulaCliente ||
                                                                                      x.IdDeudorNavigation.Nombre.ToUpper().Contains(cedulaCliente.ToUpper())) &&
                                                                                      x.Empresa == empresa && x.EsActivo == true);


            List<Deuda> deudas = deudasQuery.ToList();
            List<DeudasClienteOutDTO> deudoresDTO = deudas.Adapt<List<DeudasClienteOutDTO>>();
            return Ok(deudoresDTO);
        }


        [HttpGet("buscar-deuda-por-id/{idDeuda}")]
        public IActionResult DeudasPorId(string idDeuda)
        {
            Deuda deudasQuery = _context.Deudas.AsNoTracking().FirstOrDefault(x => x.IdDeuda == Guid.Parse(idDeuda) && x.EsActivo == true);
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
            if (usuario == null)
            {
                return Ok("no existe usuario");
            }
            IQueryable<CompromisosPago> query = _context.CompromisosPagos
                .Include(x => x.IdDeudaNavigation)
                    .ThenInclude(x => x.IdDeudorNavigation)
                .Include(x => x.IdDeudaNavigation)
                    .ThenInclude(x => x.Usuario)
                .Include(x => x.IdTipoTareaNavigation)
                ;

            var hoy = DateOnly.FromDateTime(TimeZoneInfo
                              .ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil")));

            if (esHoy)
            {
                query = query.Where(c => c.FechaCompromiso == hoy);
            }
            query = query.Where(x => x.Estado == true);



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
                    Tramo = c.IdDeudaNavigation.Tramo,
                    Gestor = c.IdUsuarioNavigation.NombreCompleto
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

        [HttpPut("cambiar-gestor-deuda/{idDeuda}")]
        public IActionResult GrabarTelefonoNuevo(string idDeuda, [FromQuery] string? nuevoGestor)
        {
            Deuda deuda = _context.Deudas.AsNoTracking().Where(x => x.IdDeuda.ToString() == idDeuda).FirstOrDefault();

            if (idDeuda == null)
                return BadRequest("No existe deuda");
            deuda.IdUsuario = nuevoGestor;

            _context.Deudas.Update(deuda);
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
        public async Task<IActionResult> Allclients([FromQuery] string? empresa, [FromQuery] string tipoFiltro = "")
        {
            // Usuario actual
            Usuario usuario = _authService.GetCurrentUser();
            bool isAdmin = usuario.Rol == "admin";

            var empresaUpper = (empresa ?? "").Trim().ToUpper();
            bool filtraEmpresa = !string.IsNullOrEmpty(empresaUpper) && empresaUpper != "TODOS";

            bool filtraSG = tipoFiltro == "SG"; // Sin gestión: sin compromisos, sin gestiones y sin pagos
            bool filtraG = tipoFiltro == "G";  // Con gestión: alguno de los 3 existe
            bool filtraIN = tipoFiltro == "IN"; // Incumplió compromiso

            // Base query (incluye deudas y su usuario para poder mostrar gestor por deuda)
            IQueryable<Deudores> clientesQuery = _context.Deudores
                .Include(c => c.Deuda)
                    .ThenInclude(d => d.Usuario)
                .AsNoTracking();

            if (!isAdmin)
            {
                clientesQuery = clientesQuery.Where(c => c.Deuda.Any(d => d.IdUsuario == usuario.IdUsuario));
            }

            Expression<Func<Deudores, bool>> filtroDeudor = c => c.Deuda.Any(d =>
                d.EsActivo == true &&
                (isAdmin || d.IdUsuario == usuario.IdUsuario) &&
                (!filtraEmpresa || (d.Empresa != null && d.Empresa.ToUpper() == empresaUpper)) &&
                (!filtraSG || (!d.CompromisosPagos.Any() && !d.Gestiones.Any() && !d.Pagos.Any())) &&
                (!filtraG || (d.CompromisosPagos.Any() || d.Gestiones.Any() || d.Pagos.Any())) &&
                (!filtraIN || d.CompromisosPagos.Any(cp => cp.IncumplioCompromisoPago == true))
            );

            clientesQuery = clientesQuery.Where(filtroDeudor);

            var deudoresDTO = await clientesQuery.OrderByDescending(x => x.FechaRegistro)
                .Select(c => new
                {
                    c.IdDeudor,
                    c.Nombre,
                    c.Telefono,
                    c.Direccion,
                    c.Descripcion,
                    c.Correo,

                    DeudasDet = c.Deuda
                        .Where(d =>
                            d.EsActivo == true &&
                            (isAdmin || d.IdUsuario == usuario.IdUsuario) &&
                            (!filtraEmpresa || (d.Empresa != null && d.Empresa.ToUpper() == empresaUpper)) &&
                            (!filtraSG || (!d.CompromisosPagos.Any() && !d.Gestiones.Any() && !d.Pagos.Any())) &&
                            (!filtraG || (d.CompromisosPagos.Any() || d.Gestiones.Any() || d.Pagos.Any())) &&
                            (!filtraIN || d.CompromisosPagos.Any(cp => cp.IncumplioCompromisoPago == true))
                        )
                        .Select(d => new
                        {
                            d.Tramo,
                            GestorNombre = d.Usuario.NombreCompleto, // gestor real de la deuda
                            d.IdUsuario
                        })
                        .ToList(),

                    NumeroDeudas = c.Deuda.Count(d =>
                        d.EsActivo == true &&
                        (isAdmin || d.IdUsuario == usuario.IdUsuario) &&
                        (!filtraEmpresa || (d.Empresa != null && d.Empresa.ToUpper() == empresaUpper)) &&
                        (!filtraSG || (!d.CompromisosPagos.Any() && !d.Gestiones.Any() && !d.Pagos.Any())) &&
                        (!filtraG || (d.CompromisosPagos.Any() || d.Gestiones.Any() || d.Pagos.Any())) &&
                        (!filtraIN || d.CompromisosPagos.Any(cp => cp.IncumplioCompromisoPago == true))
                    )
                })
                .ToListAsync();

            var result = deudoresDTO
                .Select(c => new ClientesOutDTO
                {
                    cedula = c.IdDeudor,
                    nombre = c.Nombre,
                    telefono = c.Telefono,
                    direccion = c.Direccion,
                    descripcion = c.Descripcion,
                    correo = c.Correo,
                    numeroDeudas = c.NumeroDeudas.ToString(),

                    tramos = c.DeudasDet.Any()
                        ? string.Join("",
                            c.DeudasDet.Select((d, i) =>
                                $"<strong>{i + 1}</strong>: {d.Tramo} <br>")
                          )
                        : string.Empty,

                    gestor = string.Join(", ",
                        c.DeudasDet.Select(d => d.GestorNombre).Distinct())
                })

                .Where(x => int.Parse(x.numeroDeudas) > 0)
                .ToList();

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
