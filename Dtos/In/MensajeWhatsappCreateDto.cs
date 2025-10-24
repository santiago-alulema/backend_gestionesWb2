using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Dtos.In
{
    public class MensajeWhatsappCreateDto
    {
        [Required, MaxLength(1000)]
        public string Mensaje { get; set; } = default!;

        [Required, MaxLength(50)]
        public string TipoMensaje { get; set; } = default!;

        [MaxLength(2000)]
        public string? MensajeCorreo { get; set; }
    }
}
