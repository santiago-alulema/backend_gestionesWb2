namespace gestiones_backend.Dtos.In
{
    public class CompromisoPagoInDTO
    {
        public Guid? IdDeuda { get; set; }
        public DateOnly FechaCompromiso { get; set; }
        public decimal MontoComprometido { get; set; }
        public string? Estado { get; set; }
        public string? Observaciones { get; set; }
    }
}
