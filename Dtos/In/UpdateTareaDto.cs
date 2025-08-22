namespace gestiones_backend.Dtos.In
{
    public class UpdateTareaDto
    {
        public DateOnly FechaCompromiso { get; set; }
        public decimal MontoComprometido { get; set; }
        public string? Observaciones { get; set; }
        public string HoraRecordatorio { get; set; }
    }
}
