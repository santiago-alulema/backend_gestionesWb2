using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity
{
    public class MensajeWhatsappUsuario
    {
        [Key]
        public string Id { get; set; }
        public string Mensaje { get; set; }
        public string TipoMensaje { get; set; }
        public string MensajeCorreo { get; set; }
    }
}
