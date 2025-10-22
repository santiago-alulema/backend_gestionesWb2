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
            var pago = _context.Pagos
                .Where(x => x.IdDeuda.ToString() == idDeuda)
                .Include(x => x.IdUsuarioNavigation)
                .OrderByDescending(y => y.FechaRegistro)
                .FirstOrDefault();

            var compromiso = _context.CompromisosPagos
                .Where(x => x.IdDeuda.ToString() == idDeuda)
                .Include(x => x.IdUsuarioNavigation)
                .OrderByDescending(y => y.FechaRegistro)
                .FirstOrDefault();

            var gestion = _context.Gestiones
                .Where(x => x.IdDeuda.ToString() == idDeuda)
                .Include(x => x.IdUsuarioGestionaNavigation)
                .OrderByDescending(y => y.FechaGestion)
                .FirstOrDefault();

            if (pago == null && compromiso == null && gestion == null)
                return "No se encontró ningún registro relacionado con la deuda.";

            DateTime fechaPago = pago?.FechaRegistro ?? DateTime.MinValue;
            DateTime fechaCompromiso = compromiso?.FechaRegistro ?? DateTime.MinValue;
            DateTime fechaGestion = gestion?.FechaGestion ?? DateTime.MinValue;

            if (fechaPago >= fechaCompromiso && fechaPago >= fechaGestion)
            {
                return pago?.IdUsuarioNavigation?.NombreCompleto ?? "Gestor desconocido (Pago)";
            }
            else if (fechaCompromiso >= fechaPago && fechaCompromiso >= fechaGestion)
            {
                return compromiso?.IdUsuarioNavigation?.NombreCompleto ?? "Gestor desconocido (Compromiso)";
            }
            else if (fechaGestion != DateTime.MinValue)
            {
                return gestion?.IdUsuarioGestionaNavigation?.NombreCompleto ?? "Gestor desconocido (Gestión)";
            }

            return "No se encontró último gestor.";
        }
    }
}
