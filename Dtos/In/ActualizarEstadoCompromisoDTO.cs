using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Dtos.In
{
    public class ActualizarEstadoCompromisoDTO
    {
        [Required]
        public string IdCompromiso { get; set; }

        [Required]
        public string NuevoEstado { get; set; }
    }
}
