using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Dtos.In
{
    public class PagoGrabarDTO
    {
        public Guid IdDeuda { get; set; }

        public DateOnly FechaPago { get; set; }

        public decimal MontoPagado { get; set; }

        public string? MedioPago { get; set; }
        public string? Telefono { get; set; }

        public string? Observaciones { get; set; }

        public string NumeroDocumento { get; set; }

        public string BancoId { get; set; }

        public string CuentaId { get; set; }

        public string TipoTransaccionId { get; set; }

        public string AbonoLiquidacionId { get; set; }

    }
}
