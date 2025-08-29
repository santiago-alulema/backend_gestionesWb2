using gestiones_backend.Context;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Entity;
using Microsoft.EntityFrameworkCore;

namespace gestiones_backend.Services
{
    public class GestioneService
    {
        private readonly DataContext _context;

        public GestioneService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<GestionDto>> GetGestionesByFiltersAsync(
                                                                        DateTime? startDate = null,
                                                                        DateTime? endDate = null,
                                                                        string? deudorName = null)
        {
            var query = _context.Gestiones
                .Include(g => g.IdDeudaNavigation)
                    .ThenInclude(d => d.IdDeudorNavigation)
                .Include(g => g.IdUsuarioGestionaNavigation)
                .AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(g => g.FechaGestion >= startDate && g.FechaGestion <= endDate);
            }

            if (!string.IsNullOrEmpty(deudorName))
            {
                query = query.Where(g =>
                    g.IdDeudaNavigation.IdDeudorNavigation.Nombre.Contains(deudorName));
            }

            return await query.Select(g => new GestionDto
            {
                idGestion = g.IdGestion,
                fechaGestion = g.FechaGestion,
                deudor = g.IdDeudaNavigation.IdDeudorNavigation.Nombre ?? "S/N",
                descripcion = g.Descripcion,
                usuario = g.IdUsuarioGestionaNavigation.NombreUsuario
            }).ToListAsync();
        }

    }
}
