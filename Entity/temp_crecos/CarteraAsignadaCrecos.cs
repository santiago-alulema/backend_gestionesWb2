using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity.temp_crecos
{
    public class CarteraAsignadaCrecos
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [MaxLength(50)] public string? COD_EMPRESA { get; set; }
        [MaxLength(200)] public string? EMPRESA { get; set; }

        [MaxLength(50)] public string? COD_UNIDAD_NEGOCIO { get; set; }
        [MaxLength(200)] public string? UNIDAD_NEGOCIO { get; set; }

        [MaxLength(50)] public string? COD_TIPO_CARTERA { get; set; }
        [MaxLength(200)] public string? TIPO_CARTERA { get; set; }

        public int? IMES { get; set; }
        public int? IANO { get; set; }

        [MaxLength(50)] public string? CNUMEROIDENTIFICACION { get; set; }
        [MaxLength(200)] public string? CNOMBRECOMPLETO { get; set; }

        [MaxLength(50)] public string? COD_TIPO_GESTOR { get; set; }
        [MaxLength(200)] public string? CDESCRIPCION { get; set; }

        // S/N, Y/N o 0/1 según fuente
        [MaxLength(10)] public string? BCUOTAIMPAGA { get; set; }

        public int? DIAS_MORA { get; set; }
        public string? DFECHAVENCIMIENTO { get; set; }

        public decimal? IVALORDEUDATOTAL { get; set; }
        public int? ICICLOCORTE { get; set; }

        [MaxLength(50)] public string? COD_PAIS { get; set; }
        [MaxLength(150)] public string? PAIS { get; set; }

        [MaxLength(50)] public string? COD_PROVINCIA { get; set; }
        [MaxLength(150)] public string? PROVINCIA { get; set; }

        [MaxLength(50)] public string? COD_CANTON { get; set; }
        [MaxLength(150)] public string? CANTON { get; set; }

        [MaxLength(50)] public string? COD_ZONA { get; set; }
        [MaxLength(150)] public string? ZONA { get; set; }

        [MaxLength(50)] public string? COD_BARRIO { get; set; }
        [MaxLength(150)] public string? BARRIO { get; set; }

        [MaxLength(50)] public string? COD_GESTOR { get; set; }
        [MaxLength(150)] public string? GESTOR { get; set; }

        [MaxLength(50)] public string? CODIGOCLIENTE { get; set; }
    }
}
