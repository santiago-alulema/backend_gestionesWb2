namespace gestiones_backend.Dtos.In
{
    public class CompromisoPagoInDTO
    {
        public Guid? IdDeuda { get; set; }
        public DateOnly FechaCompromiso { get; set; }
        public decimal MontoComprometido { get; set; }
        public string TipoTarea { get; set; }
        public string HoraRecordatorio { get; set; }
        public bool? Estado { get; set; }
        public string? Observaciones { get; set; }
        public string? Telefono { get; set; }

    }
}
