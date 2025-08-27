using gestiones_backend.Context;
using gestiones_backend.Dtos.In;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Entity;
using gestiones_backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace gestiones_backend.Services
{
    public class TareasService : ITareasService
    {
        private readonly DataContext _context;
        private readonly IAuthenticationService _authService;
        public TareasService(DataContext context,
            IAuthenticationService authService)
        {
            _context = context;
            _authService = authService;
        }

        public bool DeleteAsync(string idCompromiso)
        {
            var tarea = _context.CompromisosPagos.Find(idCompromiso);
            if (tarea == null)
                return false;

            _context.CompromisosPagos.Remove(tarea);
            _context.SaveChanges();
            return true;
        }

        public List<TareaDto> GetAllAsync()
        {
            Usuario usuario = _authService.GetCurrentUser();

            IQueryable<CompromisosPago> query = _context.CompromisosPagos
                .Include(x => x.IdDeudaNavigation)
                    .ThenInclude(x => x.IdDeudorNavigation)
                .Include(x => x.IdTipoTareaNavigation);

            if (usuario.Rol != "admin")
            {
                query = query.Where(x => x.IdUsuario == usuario.IdUsuario);
            }

            return query.Select(t => new TareaDto
            {
                IdCompromiso = t.IdCompromiso,
                IdDeuda = t.IdDeuda,
                FechaCompromiso = t.FechaCompromiso,
                FechaRegistro = t.FechaRegistro,
                MontoComprometido = t.MontoComprometido,
                Estado = t.Estado,
                IncumplioCompromisoPago = t.IncumplioCompromisoPago,
                FechaCumplimientoReal = t.FechaCumplimientoReal,
                Observaciones = t.Observaciones,
                IdUsuario = t.IdUsuario,
                IdTipoTarea = t.IdTipoTarea,
                TipoTarea = t.IdTipoTareaNavigation.Nombre,
                HoraRecordatorio = t.HoraRecordatorio,
                Cedula = t.IdDeudaNavigation.IdDeudorNavigation.IdDeudor,
                Nombre = t.IdDeudaNavigation.IdDeudorNavigation.Nombre
            }).OrderByDescending(x => x.FechaRegistro).ToList();
        }

        public TareaDto? UpdateAsync(string idCompromiso, UpdateTareaDto dto)
        {
            var tarea =  _context.CompromisosPagos.Find(idCompromiso);
            if (tarea == null)
                return null;

            tarea.FechaCompromiso = dto.FechaCompromiso;
            tarea.MontoComprometido = dto.MontoComprometido;
            tarea.Observaciones = dto.Observaciones;
            tarea.HoraRecordatorio = dto.HoraRecordatorio;

            _context.SaveChanges();
            return new TareaDto
            {
                IdCompromiso = tarea.IdCompromiso,
                IdDeuda = tarea.IdDeuda,
                FechaCompromiso = tarea.FechaCompromiso,
                FechaRegistro = tarea.FechaRegistro,
                MontoComprometido = tarea.MontoComprometido,
                Estado = tarea.Estado,
                IncumplioCompromisoPago = tarea.IncumplioCompromisoPago,
                FechaCumplimientoReal = tarea.FechaCumplimientoReal,
                Observaciones = tarea.Observaciones,
                IdUsuario = tarea.IdUsuario,
                IdTipoTarea = tarea.IdTipoTarea,
                HoraRecordatorio = tarea.HoraRecordatorio
            };
        }
    }
}
