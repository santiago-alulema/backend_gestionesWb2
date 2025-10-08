using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity.temp_crecos
{
    public class ArticuloOperacionCrecos
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public int? ISECUENCIAL { get; set; }

        public string? COD_PRODUCTO { get; set; }

        public string? COD_OPERACION { get; set; }

        public string? DESC_PRODUCTO { get; set; }

        public int? CANTIDAD { get; set; }

        public string? OBSERVACION { get; set; }
    }
}
