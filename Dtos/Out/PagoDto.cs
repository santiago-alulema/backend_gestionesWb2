namespace gestiones_backend.Dtos.Out
{
    public class PagoDto
    {
        public string IdPago { get; set; }
        public Guid IdDeuda { get; set; }
        public DateOnly? FechaPago { get; set; }
        public decimal MontoPagado { get; set; }
        public string? MedioPago { get; set; }
        public string? NumeroDocumenro { get; set; }
        public string? Observaciones { get; set; }
        public string? Cedula { get; set; }
        public string? Nombre { get; set; }
        public string? Gestor { get; set; }

        public string? IdBanco { get; set; }
        public string? Banco { get; set; }
        public string? IdCuenta { get; set; }
        public string? Cuenta { get; set; }
        public string? IdTipoTransaccion { get; set; }

        public string? TipoTransaccion { get; set; }
        public string? IdAbonoLiquidacion { get; set; }

        public string? AbonoLiquidacion { get; set; }
        public string? NumeroDocumento { get; set; }
    }
}
