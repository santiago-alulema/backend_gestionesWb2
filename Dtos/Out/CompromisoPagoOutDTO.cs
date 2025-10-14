namespace gestiones_backend.Dtos.Out
{
    public class CompromisoPagoOutDTO
    {
        public Guid IdDeuda { get; set; }
        public Decimal? DeudaCapital { get; set; }
        public Decimal? Interes { get; set; }
        public Decimal? GastosCobranzas { get; set; }
        public Decimal? SaldoDeuda { get; set; }
        public Decimal? DiasMora { get; set; }
        public DateOnly? FechaVenta { get; set; }
        public DateOnly? FechaUltimoPago { get; set; }
        public String CedulaCliente { get; set; } = string.Empty;
        public String NumeroFactura { get; set; } = string.Empty;
        public String NombreCliente { get; set; } = string.Empty;
        public String CompromisoPagoId { get; set; } = string.Empty;
        public string NumeroCouta { get; set; } = string.Empty;
        public Decimal ValorCuota { get; set; } = Decimal.Zero;
        public string TipoTarea { get; set; } = string.Empty;
        public string HoraTarea { get; set; } = string.Empty;
        public string? Empresa { get; set; } = string.Empty;
        public string? MontoCobrar { get; set; } = string.Empty;
        public string? Tramo { get; set; } = string.Empty;
        public string? Gestor { get; set; } = string.Empty;
        public string? ValorCompromisoPago { get; set; } = string.Empty;
    }
}
