namespace gestiones_backend.Dtos.In
{
    public class UpdatePagoDto
    {
        public DateOnly? FechaPago { get; set; }
        public decimal MontoPagado { get; set; }
        public string? MedioPago { get; set; }
        public string? NumeroDocumenro { get; set; }
        public string? Observaciones { get; set; }
        public string? IdBanco { get; set; }
        public string? IdCuenta { get; set; }
        public string? IdTipoTransaccion { get; set; }
        public string? IdAbonoLiquidacion { get; set; }

    }
}
