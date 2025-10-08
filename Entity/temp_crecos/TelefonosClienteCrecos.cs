using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestiones_backend.Entity.temp_crecos
{
    public class TelefonosClienteCrecos
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int ISECUENCIA { get; set; }

        [MaxLength(50)]
        [Column("CNUMEROIDENTIFICACION")]
        public string? CNumeroIdentificacion { get; set; }

        [MaxLength(50)]
        [Column("COD_UBICACION")]
        public string? CodUbicacion { get; set; }

        [MaxLength(150)]
        [Column("DESCRIP_UBICACION")]
        public string? DescripUbicacion { get; set; }

        [MaxLength(50)]
        [Column("COD_TIPO_TELEFONO")]
        public string? CodTipoTelefono { get; set; }

        [MaxLength(100)]
        [Column("TIPO_TELEFONO")]
        public string? TipoTelefono { get; set; }

        [MaxLength(50)]
        [Column("CNUMERO")]
        public string? CNumero { get; set; }

        [MaxLength(10)]
        [Column("CPREFIJO")]
        public string? CPrefijo { get; set; }
    }
}
