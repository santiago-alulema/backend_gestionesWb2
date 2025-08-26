namespace gestiones_backend.Dtos.In
{
    public class UploadDeudasInDTO
    {
        public string CedulaDeudor { get; set; }
        public decimal? DeudaCapital { get; set; }
        public decimal? Interes { get; set; }
        public decimal? GastosCobranza { get; set; }
        public decimal? SaldoDeuda { get; set; }
        public string? Descuento { get; set; }
        public decimal? MontoCobrar { get; set; }
        public string? FechaVenta { get; set; }
        public string? FechaUltimoPago { get; set; }
        public string? Estado { get; set; }
        public int? DiasMora { get; set; }
        public string? NumeroFactura { get; set; }
        public string? Calificacion { get; set; }
        public int? Creditos { get; set; }
        public int? NumeroCuotas { get; set; }
        public string? TipoDeDocumento { get; set; }
        public decimal? ValorCuota { get; set; }
        public string? Tramo { get; set; }
        public decimal? UltimoPago { get; set; }
        public string? Empresa { get; set; }
        public string? ProductoDescripcion { get; set; }
        public string? Agencia { get; set; }
        public string? Ciudad { get; set; }

    }
}
