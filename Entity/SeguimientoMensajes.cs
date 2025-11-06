namespace gestiones_backend.Entity
{
    public class SeguimientoMensajes
    {
        public string id { get; set; }
        public string? tipo { get; set; }
        public string? numeroDestino { get; set; }
        public string? usuario { get; set; }
        public string? usuarioWhatsapp { get; set; }
        public string? mensaje { get; set; }
        public DateTime? fechaRegistro { get; set; }

    }
}
