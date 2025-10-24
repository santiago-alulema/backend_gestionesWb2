using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Dtos.In
{
    public class MensajeWhatsappUpdateDto
    {
        public string Mensaje { get; set; } = default!;
        public string TipoMensaje { get; set; } = default!;
        public string? MensajeCorreo { get; set; }
    }
}
