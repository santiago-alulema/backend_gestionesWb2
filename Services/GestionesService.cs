using gestiones_backend.Context;
using gestiones_backend.Dtos.In;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Entity;
using gestiones_backend.Interfaces;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;

namespace gestiones_backend.Services
{
    public class GestionesService : IGestionesService
    {
        private readonly DataContext _context;
        private readonly IAuthenticationService _authService;
        public GestionesService(DataContext context,
            IAuthenticationService authService)
        {
            _context = context;
            _authService = authService;
        }

        public List<GestionOutDto> GetAllAsync()
        {
            Usuario usuario = _authService.GetCurrentUser();

            IQueryable<Gestione> query = _context.Gestiones
                .Include(x => x.IdDeudaNavigation)
                    .ThenInclude(x => x.IdDeudorNavigation)
                .Include(x => x.IdTipoResultadoNavigation)
                .Include(x => x.RespuestaTipoContactoNavigation)
                .Include(x => x.IdTipoContactoResultadoNavigation)
                    .ThenInclude(x => x.TipoResultadoNavigation)
                .Include(x => x.IdUsuarioGestionaNavigation);

            if (usuario.Rol != "admin")
            {
                query = query.Where(x => x.IdUsuarioGestiona == usuario.IdUsuario);
            }

            return query.Select(g => new GestionOutDto
            {
                IdGestion = g.IdGestion,
                IdDeuda = g.IdDeuda,
                FechaGestion = g.FechaGestion.ToString("yyyy/MM/dd"),
                Descripcion = g.Descripcion,
                Email = g.Email,
                IdUsuarioGestiona = g.IdUsuarioGestiona,
                UsuarioGestiona = g.IdUsuarioGestionaNavigation.NombreCompleto,
                IdTipoContactoResultado = g.IdTipoContactoResultado,
                TipoContactoResultado = g.IdTipoContactoResultadoNavigation.Nombre,
                IdTipoResultado = g.IdTipoResultado,
                TipoResultado = g.IdTipoContactoResultadoNavigation.TipoResultadoNavigation.Nombre,
                IdRespuestaTipoContacto = g.IdRespuestaTipoContacto,
                RespuestaTipoContacto = g.RespuestaTipoContactoNavigation.Nombre,
                Cedula = g.IdDeudaNavigation.IdDeudorNavigation.IdDeudor,
                Nombre = g.IdDeudaNavigation.IdDeudorNavigation.Nombre
            }).ToList();
        }
        public GestionOutDto? UpdateAsync(string idGestion, UpdateGestionDto dto)
        {
            var gestion = _context.Gestiones.Find(idGestion);
            if (gestion == null)
                return null;

            gestion.Descripcion = dto.Descripcion;
            gestion.Email = dto.Email;
            gestion.IdTipoContactoResultado = dto.IdTipoContactoResultado;
            gestion.IdTipoResultado = dto.IdTipoResultado;
            gestion.IdRespuestaTipoContacto = dto.IdRespuestaTipoContacto;

            _context.SaveChanges();

            return new GestionOutDto
            {
                IdGestion = gestion.IdGestion,
                IdDeuda = gestion.IdDeuda,
                FechaGestion = gestion.FechaGestion.ToString("yyyy/MM/dd"),
                Descripcion = gestion.Descripcion,
                Email = gestion.Email,
                IdUsuarioGestiona = gestion.IdUsuarioGestiona,
                IdTipoContactoResultado = gestion.IdTipoContactoResultado,
                IdTipoResultado = gestion.IdTipoResultado,
                IdRespuestaTipoContacto = gestion.IdRespuestaTipoContacto
            };
        }

        public bool DeleteAsync(string idGestion)
        {
            var gestion = _context.Gestiones.Find(idGestion);
            if (gestion == null)
                return false;

            _context.Gestiones.Remove(gestion);
            _context.SaveChanges();
            return true;
        }

        public string UltimoGestorGestionaDeuda(string idDeuda)
        {
            Pago? pago = _context.Pagos
                       .Where(x => x.IdDeuda.ToString() == idDeuda)
                       .Include(x => x.IdUsuarioNavigation)
                       .OrderByDescending(y => y.FechaRegistro)
                       .FirstOrDefault();

            CompromisosPago? compromiso = _context.CompromisosPagos
                                        .Where(x => x.IdDeuda.ToString() == idDeuda)
                                        .Include(x => x.IdUsuarioNavigation)
                                        .OrderByDescending(y => y.FechaRegistro)
                                        .FirstOrDefault();

            Gestione? gestion = _context.Gestiones
                                .Where(x => x.IdDeuda.ToString() == idDeuda)
                                .Include(x => x.IdUsuarioGestionaNavigation)
                                .OrderByDescending(y => y.FechaGestion)
                                .FirstOrDefault();

            var fechas = new List<(string Tipo, DateTime? Fecha)>
                                    {
                                        ("Pago", pago?.FechaRegistro),
                                        ("CompromisoPago", compromiso?.FechaRegistro),
                                        ("Gestion", gestion?.FechaGestion)
                                    };

            var masReciente = fechas
                .Where(f => f.Fecha.HasValue)
                .OrderByDescending(f => f.Fecha)
                .FirstOrDefault();

            DateTime fechaPago = pago?.FechaRegistro ?? DateTime.MinValue;
            DateTime fechaCompromiso = compromiso?.FechaRegistro ?? DateTime.MinValue;
            DateTime fechaGestion = gestion?.FechaGestion ?? DateTime.MinValue;

            if (fechaPago >= fechaCompromiso && fechaPago >= fechaGestion)
            {
                return (pago.IdUsuarioNavigation.NombreCompleto);
            }
            else if (fechaCompromiso >= fechaPago && fechaCompromiso >= fechaGestion)
            {
                return (compromiso.IdUsuarioNavigation.NombreCompleto);
            }
            else
            {
                return (gestion.IdUsuarioGestionaNavigation.NombreCompleto); ;
            }

            return ("No se encontro ultimo Gestor");
        }
    }
}
