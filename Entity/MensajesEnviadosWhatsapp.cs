namespace gestiones_backend.Entity
{
    public class MensajesEnviadosWhatsapp
    {
        public string? Id { get; set; } = Guid.NewGuid().ToString();
        public string? IdCliente { get; set; } = null!;
        public string? IdUsuario { get; set; } = null!;
        public string? TelefonoEnviado { get; set; } = null!;
        public DateTime? FechaRegistro { get; set; } = DateTime.Now;
    }
}
