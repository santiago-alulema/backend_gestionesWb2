namespace gestiones_backend.Dtos.Out
{
    public class CompromisoPagoOutDTO
    {
        public Guid deudaId { get; set; }
        public Decimal montoOriginal { get; set; }
        public Decimal saldoActual { get; set; }
        public DateOnly fechaVencimiento { get; set; }
        public DateOnly? fechaAsignacion { get; set; }
        public DateOnly? fechaCompromiso { get; set; }
        public String descripcion { get; set; }
        public String cedulaCliente { get; set; } = string.Empty;
        public String numeroFactura { get; set; } = string.Empty;
        public String nombreCliente { get; set; } = string.Empty;
        public String compromisoPagoId { get; set; } = string.Empty;

        public int numeroCouta { get; set; } = 0;
        public int totalCuotas { get; set; } = 0;
        public Decimal valorCuotas { get; set; } = 0;
        public Decimal montoCompromiso { get; set; } = 0;



    }
}
