using gestiones_backend.Context;
using gestiones_backend.Dtos.In;
using gestiones_backend.Entity;
using gestiones_backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MigracionController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IAuthenticationService _authService;

        public MigracionController(DataContext context, IAuthenticationService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost("migrar-gestiones")]
        public IActionResult Post(List<MigracionesInDTO> migraciones) {
            List<Gestione> gestiones = new();
            List<CompromisosPago> compromisosPago = new();
            List<Deuda> deudas = _context.Deudas.ToList();
            List<TipoResultado> tipoResultado = _context.TiposResultado.Include(x => x.TiposConstactosNavigation)
                                                                 .ThenInclude(x => x.TiposRespuestaNavigation).ToList();
            List<Usuario> usuarios = _context.Usuarios.ToList();
            List<TipoTarea> tiposTareas = _context.TiposTareas.ToList();
            foreach (MigracionesInDTO mig in migraciones)
            {
                Deuda deuda = deudas.FirstOrDefault(x => x.NumeroFactura == mig.OperacionCxc.Replace("'", ""));
                TipoResultado resultado = tipoResultado.FirstOrDefault(x => x.Nombre == mig.Resulted.Replace("'", ""));
                TipoContactoResultado tipoContacto = resultado.TiposConstactosNavigation.FirstOrDefault(x => x.Nombre == mig.TypeResulted);
                RespuestaTipoContacto respuesta = tipoContacto.TiposRespuestaNavigation.FirstOrDefault(x => x.Nombre == mig.ResponseD.Replace("'", ""));
                List<string> listaCompromisosPago = new List<string>() { "PLAN DE PAGO", "ABONO", "LIQUIDACION", "VOLVER A LLAMAR", "COMPROMISO DE PAGO", "REALIZARA ABONO" };
                var palabras = mig.Usuario.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                Usuario usuario = usuarios.Where(u => palabras.All(p => u.NombreCompleto.ToLower().Contains(p.ToLower()))).FirstOrDefault();
                if (deuda == null)
                {
                    string ss = "";
                    continue;
                }

                if (!listaCompromisosPago.Contains(mig.ResponseD.Trim()) )
                {
                    if (respuesta != null)
                    {
                        Gestione gestionNueva = new()
                        {
                            IdGestion = Guid.NewGuid().ToString(),
                            IdDeuda = deuda?.IdDeuda ?? Guid.Empty,
                            FechaGestion = mig?.Created != null
                                            ? DateTime.SpecifyKind(DateTime.Parse(mig.Created.ToString() ?? DateTime.Now.ToString()), DateTimeKind.Utc)
                                            : DateTime.UtcNow,
                            Descripcion = (mig?.Comment ?? "") + " [MIGRACION]",
                            Email = mig?.Email ?? "",
                            IdUsuarioGestiona = usuario.IdUsuario,
                            Telefono = mig?.Phone ?? "",
                            IdTipoContactoResultado = tipoContacto?.Id ?? null,
                            IdTipoResultado = resultado?.Id ?? null,
                            IdRespuestaTipoContacto = respuesta?.Id ?? null
                        };
                        gestiones.Add(gestionNueva);
                    }
                }
                else
                {
                    CompromisosPago compromiso = new()
                    {
                        IdCompromiso = Guid.NewGuid().ToString(),
                        IdDeuda = deuda?.IdDeuda,
                        FechaCompromiso = DateOnly.FromDateTime(mig?.Fecha ?? DateTime.UtcNow),
                        FechaRegistro = mig?.Created != null
                        ? DateTime.SpecifyKind(mig.Created.Value, DateTimeKind.Utc)
                        : DateTime.UtcNow,
                        MontoComprometido = mig?.Monto ?? 0m,
                        Telefono = mig?.Phone ?? "S/N",
                        Estado = true,
                        IncumplioCompromisoPago = false,
                        FechaCumplimientoReal = null,
                        Observaciones = mig?.Comment + " [MIGRACION]",
                        IdUsuario = usuario.IdUsuario,
                        IdTipoTarea = tiposTareas.FirstOrDefault(x => x.Nombre == mig.ResponseD).Id,
                        HoraRecordatorio = (mig?.Fecha is DateTime dt)
                                             ? dt.ToString("HH:00", CultureInfo.InvariantCulture)
                                             : DateTime.UtcNow.ToString("HH:00", CultureInfo.InvariantCulture)
                    };
                    compromisosPago.Add(compromiso);
                }
            }
            _context.Gestiones.AddRange(gestiones);
            _context.CompromisosPagos.AddRange(compromisosPago);
            _context.SaveChanges();
            return Ok("migraciones realizadas correctamente");
        }

        [HttpPost("migrar-pagos")]
        public IActionResult PostMigrarPagos(List<RegistroPagoInDto> migraciones)
        {
            List<BancosPagos> bancos = _context.BancosPagos.ToList();
            List<TipoCuentaBancaria> tipoCuentaBancaria = _context.TiposCuentaBancaria.ToList();
            List<TipoTransaccion> tipoTransaccion = _context.TiposTransaccion.ToList();
            List<AbonoLiquidacion> abonoLiquidacions = _context.AbonosLiquidacion.ToList();
            List<Deuda> deudas = _context.Deudas.ToList();
            List<Usuario> usuarios = _context.Usuarios.ToList();
            List<Pago> pagosGrabar = new();
            foreach (RegistroPagoInDto mig in migraciones)
            {
                Deuda deuda = deudas.FirstOrDefault(x => x.NumeroFactura == mig.CXC.Replace("'", ""));
                var palabras = mig.Usuario.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                Usuario usuario = usuarios.Where(u => palabras.All(p => u.NombreCompleto.ToLower().Contains(p.ToLower()))).FirstOrDefault();
                string bandoId = bancos.FirstOrDefault(x => x.Nombre == mig.Banco)?.Id;
                string abonoLiquidacion = abonoLiquidacions.FirstOrDefault(x => x.Nombre == mig.AbonoLiquidacion).Id;
                if (abonoLiquidacion.IsNullOrEmpty())
                {
                    string ss = "";
                }
                Pago pagoNuevo = new()
                {
                    IdPago = Guid.NewGuid(),
                    IdDeuda = deuda.IdDeuda,
                    FechaPago = mig.FechaPago.HasValue
                                ? DateOnly.FromDateTime(mig.FechaPago.Value)
                                : DateOnly.FromDateTime(DateTime.UtcNow),
                    FechaRegistro = mig.FechaPago.HasValue
                                    ? DateTime.SpecifyKind(mig.FechaPago.Value, DateTimeKind.Utc)
                                    : DateTime.UtcNow,
                    MontoPagado = mig.Monto ?? 0m,
                    Telefono = "",
                    MedioPago = null,
                    NumeroDocumenro = mig.NroDocumento,
                    Observaciones = "[MIGRACION]",
                    IdUsuario = usuario.IdUsuario,
                    IdBancosPago = bandoId,
                    IdTipoCuentaBancaria = null,
                    IdTipoTransaccion = null,
                    IdAbonoLiquidacion = abonoLiquidacion
                };

                pagosGrabar.Add(pagoNuevo);
            }

            _context.Pagos.AddRange(pagosGrabar);
            _context.SaveChanges();
           
            return Ok("migraciones realizadas correctamente");
        }
    }
}
