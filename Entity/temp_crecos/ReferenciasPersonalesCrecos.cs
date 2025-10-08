using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestiones_backend.Entity.temp_crecos
{
    public class ReferenciasPersonalesCrecos
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [MaxLength(50)]
        [Column("NUM_IDENTIFICACION")]
        public string? NumIdentificacion { get; set; }

        [MaxLength(200)]
        [Column("CNOMBRECOMPLETO")]
        public string? NombreCompleto { get; set; }

        [MaxLength(50)]
        [Column("COD_TIPO_IDENT_REF")]
        public string? CodTipoIdentRef { get; set; }

        [MaxLength(150)]
        [Column("DESCRIPC_TIPO_IDENTIFIC")]
        public string? DescripTipoIdentific { get; set; }

        [MaxLength(50)]
        [Column("NUM_IDENTIFIC_REF")]
        public string? NumIdentificRef { get; set; }

        [MaxLength(200)]
        [Column("NOMBRE_REFERENCIA")]
        public string? NombreReferencia { get; set; }

        [MaxLength(50)]
        [Column("COD_PAIS_REF")]
        public string? CodPaisRef { get; set; }

        [MaxLength(150)]
        [Column("DESCRIP_PAIS")]
        public string? DescripPais { get; set; }

        [MaxLength(50)]
        [Column("COD_PROVINCIA_REF")]
        public string? CodProvinciaRef { get; set; }

        [MaxLength(150)]
        [Column("DESCRIP_PROVINCIA")]
        public string? DescripProvincia { get; set; }

        [MaxLength(50)]
        [Column("COD_CANTON_REF")]
        public string? CodCantonRef { get; set; }

        [MaxLength(150)]
        [Column("DESCRIP_CANTON")]
        public string? DescripCanton { get; set; }

        [MaxLength(50)]
        [Column("COD_TIPO_VINCULO_REF")]
        public string? CodTipoVinculoRef { get; set; }

        [MaxLength(150)]
        [Column("DESCRIP_VINCULO")]
        public string? DescripVinculo { get; set; }

        [MaxLength(300)]
        [Column("DIRECCION_REF")]
        public string? DireccionRef { get; set; }

        [MaxLength(50)]
        [Column("NUMERO_REFERENCIA")]
        public string? NumeroReferencia { get; set; }
    }
}
