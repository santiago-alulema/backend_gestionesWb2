namespace gestiones_backend.Dtos.In
{
    public class DeudasCrecosMasivoInDto
    {
        public string? IdDeudor { get; set; }
        public decimal? SaldoDeuda { get; set; }
        public int? DiasMora { get; set; }
        public string? Tramo { get; set; }
        public Decimal? UltimoPago { get; set; }
        public string? Empresa { get; set; } = "CRECOSCORP";
        public string? Clasificacion { get; set; }
        public int? Creditos { get; set; }
        public int? Descuento { get; set; }
        public DateTime? FechaUltimoPago { get; set; } = null;
        public decimal? MontoCobrar { get; set; }
        public string? TipoDocumento { get; set; }
        public string? Agencia { get; set; }
        public string? Ciudad { get; set; }
        public bool? EsActivo { get; set; } = true;
        public string? CodigoEmpresa { get; set; } = "000001";
        public string? CodigoOperacion { get; set; }
        public decimal? MontoCobrarPartes { get; set; }
        public decimal? MontoPonteAlDia { get; set; }
        public string? Gestor { get; set; }
    }
}
