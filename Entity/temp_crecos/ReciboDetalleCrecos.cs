using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestiones_backend.Entity.temp_crecos
{
    public class ReciboDetalleCrecos
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Column("IRECIBODETALLE")]
        [StringLength(100)]
        public string IReciboDetalle { get; set; } = string.Empty;

        [Column("COD_RECIBO")]
        [StringLength(100)]
        public string? CodRecibo { get; set; }

        [Column("ICODIGOOPERACION")]
        [StringLength(100)]
        public string? ICodigoOperacion { get; set; }

        [Column("NUM_CUOTA")]
        public int? NumCuota { get; set; }

        public string? NombreArchivo { get; set; }


        [Column("COD_RUBRO")]
        [StringLength(50)]
        public string? CodRubro { get; set; }

        [Column("CDESCRIPCION_RUBRO")]
        [StringLength(200)]
        public string? CDescripcionRubro { get; set; }

        [Column("VALOR_RECIBO")]
        [Precision(18, 2)]
        public decimal? ValorRecibo { get; set; }

    }
}
