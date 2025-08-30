namespace gestiones_backend.Dtos.Out
{
    public class TareaDto
    {
        public string IdCompromiso { get; set; }
        public Guid? IdDeuda { get; set; }
        public DateOnly FechaCompromiso { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string FechaRegistroString { get; set; }
        public decimal MontoComprometido { get; set; }
        public string MontoComprometidoString { get; set; }
        public bool? Estado { get; set; }
        public bool? IncumplioCompromisoPago { get; set; }
        public DateOnly? FechaCumplimientoReal { get; set; }
        public string? Observaciones { get; set; }
        public string IdUsuario { get; set; }
        public string Gestor { get; set; }
        public string IdTipoTarea { get; set; }
        public string HoraRecordatorio { get; set; }
        public string TipoTarea { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
    }
}
