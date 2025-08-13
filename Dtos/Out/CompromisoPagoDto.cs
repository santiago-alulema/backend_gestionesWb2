namespace gestiones_backend.Dtos.Out
{
    public class CompromisoPagoDto
    {
        public string IdCompromiso { get; set; }
        public DateOnly FechaCompromiso { get; set; }
        public string Deudor { get; set; } = string.Empty;
        public decimal MontoComprometido { get; set; }
        public bool? Estado { get; set; }
        public DateOnly? FechaCumplimientoReal { get; set; }
        public string Observaciones { get; set; } = string.Empty;
    }
}
