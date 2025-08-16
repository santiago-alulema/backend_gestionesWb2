namespace gestiones_backend.Dtos.Out
{
    public class DeudasClienteOutDTO
    {
        public Guid IdDeuda { get; set; } = Guid.Empty;
        public string? IdDeudor { get; set; }
        public Decimal? DeudaCapital { get; set; }
        public Decimal? Interes { get; set; }
        public Decimal? GastosCobranzas { get; set; }
        public Decimal? SaldoDeuda { get; set; }
        public int? Descuento { get; set; }
        public Decimal? MontoCobrar { get; set; }
        public DateOnly? FechaVenta { get; set; }
        public DateOnly? FechaUltimoPago { get; set; }
        public string? Estado { get; set; }
        public int? DiasMora { get; set; }
        public string? NumeroFactura { get; set; }
        public string? Clasificacion { get; set; }
        public int? Creditos { get; set; }
        public Decimal? SaldoDeulda { get; set; }
        public int? NumeroCuotas { get; set; }
        public string? TipoDocumento { get; set; }
        public Decimal? ValorCuota { get; set; }
        public string? Tramo { get; set; }
        public Decimal? UltimoPago { get; set; }
        public string? Empresa { get; set; }


    }
}
