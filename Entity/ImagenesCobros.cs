using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity
{
    public class ImagenesCobros
    {
        [Key]
        public string Id { get; set; }
        public string Url { get; set; }
        public string UrlRelativo { get; set; }
        public string NombreArchivo { get; set; }
        public string Tamanio { get; set; }
        public DateTime? FechaRegistro { get; set; } = DateTime.UtcNow;
        public Guid PagoId { get; set; }
        public virtual Pago Pago { get; set; }
    }
}
