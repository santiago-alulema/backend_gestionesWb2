namespace gestiones_backend.Entity.temp_crecos
{
    public class CuotaOperacionCrecos
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string CodOperacion { get; set; } = default!;
        public string CodCuota { get; set; } = default!;
        public int NumeroCuota { get; set; }

        public DateTime? FechaVencimiento { get; set; }
        public DateTime? FechaCorte { get; set; }
        public DateTime? FechaUltimoPago { get; set; }
        public DateTime? DFechaPostergacion { get; set; }

        public string? CodEstadoCuota { get; set; }
        public string? DescEstadoOperacion { get; set; }
        public decimal? TasaMora { get; set; }
        public string? CodEstadoRegistro { get; set; }
        public string? DesEstadoRegistro { get; set; }

        public decimal? IValorTotalCuota { get; set; }
        public decimal? IValorCuota { get; set; }
        public decimal? ValorCapitalInteres { get; set; }
        public decimal? ValorCargos { get; set; }
        public decimal? ValorOtrosCargos { get; set; }
    }
}
