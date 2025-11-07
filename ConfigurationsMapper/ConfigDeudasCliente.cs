using gestiones_backend.Dtos.Out;
using gestiones_backend.Entity;
using Mapster;

namespace gestiones_backend.ConfigurationsMapper
{
    public class ConfigDeudasCliente
    {
        public static void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Deuda, DeudasClienteOutDTO>()
            // 1:1 directos
            .Map(d => d.IdDeuda, s => s.IdDeuda)
            .Map(d => d.IdDeudor, s => s.IdDeudor)
            .Map(d => d.DeudaCapital, s => s.DeudaCapital)
            .Map(d => d.Interes, s => s.Interes)
            .Map(d => d.GastosCobranzas, s => s.GastosCobranzas)
            .Map(d => d.SaldoDeuda, s => s.SaldoDeuda)
            .Map(d => d.Descuento, s => s.Descuento)
            .Map(d => d.MontoCobrar, s => s.MontoCobrar)
            .Map(d => d.FechaVenta, s => s.FechaVenta)
            .Map(d => d.FechaUltimoPago, s => s.FechaUltimoPago)
            .Map(d => d.Estado, s => s.Estado)
            .Map(d => d.DiasMora, s => s.DiasMora)
            .Map(d => d.NumeroFactura, s => s.NumeroFactura)
            .Map(d => d.Clasificacion, s => s.Clasificacion)
            .Map(d => d.Creditos, s => s.Creditos)
            .Map(d => d.MontoCobrarPartes, s => s.MontoCobrarPartes)
            .Map(d => d.NumeroCuotas, s => s.NumeroCuotas)
            .Map(d => d.TipoDocumento, s => s.TipoDocumento)
            .Map(d => d.ValorCuota, s => s.ValorCuota)
            .Map(d => d.Tramo, s => s.Tramo)
            .Map(d => d.UltimoPago, s => s.UltimoPago)
            .Map(d => d.Empresa, s => s.Empresa)
            .Map(d => d.ProductoDescripcion, s => s.ProductoDescripcion)
            .Map(d => d.Agencia, s => s.Agencia)
            .Map(d => d.Ciudad, s => s.Ciudad)
            .Map(d => d.Nombre, s => s.IdDeudorNavigation.Nombre)
            // Campo con typo en el DTO (si existe)
            .Map(d => d.SaldoDeulda, s => s.SaldoDeuda)

            // Navegaciones null-safe
            .Map(d => d.Nombre, s => s.IdDeudorNavigation != null ? s.IdDeudorNavigation.Nombre : null)
            .Map(d => d.Telefono, s => s.IdDeudorNavigation != null && s.IdDeudorNavigation.Telefono != null ? s.IdDeudorNavigation.Telefono : string.Empty)
            .Map(d => d.Email, s => s.IdDeudorNavigation != null && s.IdDeudorNavigation.Correo != null ? s.IdDeudorNavigation.Correo : string.Empty)

            // Usuario puede no estar cargado -> null-safe
            .Map(d => d.NombreCompleto, s => s.Usuario != null ? s.Usuario.NombreCompleto : null);
        }
    }
}
