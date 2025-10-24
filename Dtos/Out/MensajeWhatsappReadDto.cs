namespace gestiones_backend.Dtos.Out
{
    public class MensajeWhatsappReadDto
    {
        public string Id { get; set; } = default!;
        public string Mensaje { get; set; } = default!;
        public string TipoMensaje { get; set; } = default!;
        public string? MensajeCorreo { get; set; }
    }
}
