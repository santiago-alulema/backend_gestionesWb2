using gestiones_backend.Context;
using gestiones_backend.Dtos.In;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Entity;
using gestiones_backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace gestiones_backend.Services
{
    public class PagosService : IPagosService
    {
        private readonly DataContext _context;
        private readonly IAuthenticationService _authService;
        public PagosService(DataContext context,
            IAuthenticationService authService)
        {
            _context = context;
            _authService = authService;
        }

        public bool DeleteAsync(string idPago)
        {
            var pago = _context.Pagos.Find(Guid.Parse(idPago));
            if (pago == null)
                return false;

            _context.Pagos.Remove(pago);
            _context.SaveChanges();
            return true;
        }

        public List<PagoDto> GetAllAsync()
        {
            Usuario usuario = _authService.GetCurrentUser();

            IQueryable<Pago> query = _context.Pagos
                .Include(x => x.IdUsuarioNavigation)
                .Include(x => x.BancosNavigation)
                .Include(x => x.TipoCuentaBancariaNavigation)
                .Include(x => x.TipoTransaccionNavigation)
                .Include(x => x.AbonoLiquidacionNavigation)
                .Include(x => x.ImagenesCobrosNavigation)
                .Include(x => x.IdDeudaNavigation)
                    .ThenInclude(x => x.IdDeudorNavigation)
                .OrderByDescending(x => x.FechaRegistro);

            if (usuario.Rol != "admin")
            {
                query = query.Where(x => x.IdUsuario == usuario.IdUsuario);
            }

            return query.OrderByDescending(x => x.FechaPago).Select(p => new PagoDto
            {
                IdPago = p.IdPago.ToString(),
                IdDeuda = p.IdDeuda,
                FechaPago = p.FechaPago,
                MontoPagado = p.MontoPagado,
                MedioPago = p.MedioPago,
                NumeroDocumenro = p.NumeroDocumenro,
                Observaciones = p.Observaciones,
                Cedula = p.IdDeudaNavigation.IdDeudorNavigation.IdDeudor,
                Nombre = p.IdDeudaNavigation.IdDeudorNavigation.Nombre,
                IdBanco = p.BancosNavigation.Id,
                Banco = p.BancosNavigation.Nombre,
                IdCuenta = p.TipoCuentaBancariaNavigation.Id,
                Cuenta = p.TipoCuentaBancariaNavigation.Nombre,
                IdTipoTransaccion = p.TipoTransaccionNavigation.Id,
                TipoTransaccion = p.TipoTransaccionNavigation.Nombre,
                IdAbonoLiquidacion = p.AbonoLiquidacionNavigation.Id,
                AbonoLiquidacion = p.AbonoLiquidacionNavigation.Nombre,
                Gestor = p.IdUsuarioNavigation.NombreCompleto,
                ImagenUrl = p.ImagenesCobrosNavigation.Count() > 0 ? p.ImagenesCobrosNavigation.FirstOrDefault().Url : ""
            }).ToList();
        }
        public PagoDto? UpdateAsync(string idPago, UpdatePagoDto dto)
        {
            var pago = _context.Pagos.Find(Guid.Parse(idPago));
            if (pago == null)
                return null;

            pago.FechaPago = dto.FechaPago;
            pago.MontoPagado = dto.MontoPagado;
            pago.MedioPago = dto.MedioPago;
            pago.NumeroDocumenro = dto.NumeroDocumenro;
            pago.Observaciones = dto.Observaciones;

            pago.IdBancosPago = dto.IdBanco;
            pago.IdTipoCuentaBancaria = dto.IdCuenta;
            pago.IdTipoTransaccion = dto.IdTipoTransaccion;
            pago.IdAbonoLiquidacion = dto.IdAbonoLiquidacion;


            _context.SaveChanges();

            return new PagoDto
            {
                IdPago = pago.IdPago.ToString(),
                IdDeuda = pago.IdDeuda,
                FechaPago = pago.FechaPago,
                MontoPagado = pago.MontoPagado,
                MedioPago = pago.MedioPago,
                NumeroDocumenro = pago.NumeroDocumenro,
                Observaciones = pago.Observaciones
            };
        }

    }
}
