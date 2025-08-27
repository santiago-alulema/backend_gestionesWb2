using gestiones_backend.Context;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Entity;
using Microsoft.EntityFrameworkCore;

namespace gestiones_backend.Services
{
    public class CompromisosPagoService
    {

        private readonly DataContext _context;

        public CompromisosPagoService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<CompromisoPagoDto>> GetCompromisosByFiltersAsync(
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            string? deudorName = null)
        {
            var query = _context.CompromisosPagos
                .Include(c => c.IdDeudaNavigation)
                    .ThenInclude(d => d.IdDeudorNavigation)
                .AsQueryable();

            if (startDate != null && endDate != null)
            {
                query = query.Where(c =>
                    c.FechaCompromiso >= startDate && c.FechaCompromiso <= endDate);
            }

            if (!string.IsNullOrEmpty(deudorName))
            {
                query = query.Where(c =>
                    c.IdDeudaNavigation.IdDeudorNavigation.Nombre.Contains(deudorName));
            }

            return await query.OrderByDescending(x => x.FechaRegistro)
                .Select(c => new CompromisoPagoDto
                {
                    IdCompromiso = c.IdCompromiso ?? "",
                    FechaCompromiso = c.FechaCompromiso,
                    Deudor = c.IdDeudaNavigation.IdDeudorNavigation.Nombre,
                    MontoComprometido = c.MontoComprometido,
                    Estado = c.Estado,
                    FechaCumplimientoReal = c.FechaCumplimientoReal,
                    Observaciones = c.Observaciones
                })
                .ToListAsync();
        }
    }
}
