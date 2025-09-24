using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity
{
    public class MensajeWhatsappUsuario
    {
        [Key]
        public string Id { get; set; }
        public string Mensaje { get; set; }
        public string TipoMensaje { get; set; }
        //public string  IdUsuario { get; set; }
        //public Usuario Usuario { get; set; } = new Usuario();

    }
}
