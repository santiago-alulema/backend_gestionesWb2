using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestiones_backend.Entity.temp_crecos
{
    public class ReciboPagosCrecos
    {
        // Clave primaria: asumo que COD_RECIBO identifica el recibo
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Column("COD_RECIBO")]
        [StringLength(100)]
        public string CodRecibo { get; set; } = string.Empty;

        [Column("CESTADO_REGISTRO")]
        [StringLength(50)]
        public string? CestadoRegistro { get; set; }

        [Column("COD_EMPRESA")]
        [StringLength(50)]
        public string? CodEmpresa { get; set; }

        [Column("DESCRIPC_EMPRESA")]
        [StringLength(200)]
        public string? DescripcEmpresa { get; set; }

        [Column("COD_UNEGOCIO")]
        [StringLength(50)]
        public string? CodUNegocio { get; set; }

        [Column("DESCRIPC_UNEGOCIO")]
        [StringLength(200)]
        public string? DescripcUNegocio { get; set; }

        [Column("COD_TCARTERA")]
        [StringLength(50)]
        public string? CodTCartera { get; set; }

        [Column("DESCRIPC_TCARTERA")]
        [StringLength(200)]
        public string? DescripcTCartera { get; set; }

        [Column("COD_OFICINA")]
        [StringLength(50)]
        public string? CodOficina { get; set; }

        [Column("CDESCRIPCION_OFICINA")]
        [StringLength(200)]
        public string? CDescripcionOficina { get; set; }

        [Column("NUM_IDENTIFICACION")]
        [StringLength(50)]
        public string? NumIdentificacion { get; set; }

        [Column("COD_PAGO_REFERENCIAL")]
        [StringLength(100)]
        public string? CodPagoReferencial { get; set; }

        [Column("COD_MONEDA")]
        [StringLength(10)]
        public string? CodMoneda { get; set; }

        [Column("DESCRIPC_MONEDA")]
        [StringLength(100)]
        public string? DescripcMoneda { get; set; }

        [Column("COD_TPAGO")]
        [StringLength(50)]
        public string? CodTPago { get; set; }

        [Column("DESCRIPC_TPAGO")]
        [StringLength(200)]
        public string? DescripcTPago { get; set; }

        [Column("COD_CAJA")]
        [StringLength(50)]
        public string? CodCaja { get; set; }

        [Column("DESCRIPC_CAJA")]
        [StringLength(200)]
        public string? DescripcCaja { get; set; }

        [Column("COD_GESTOR")]
        [StringLength(100)]
        public string? CodGestor { get; set; }

        [Column("DESCRIPC_GESTOR")]
        [StringLength(200)]
        public string? DescripcGestor { get; set; }

        [Column("COD_TRECIBO")]
        [StringLength(50)]
        public string? CodTRecibo { get; set; }
        
        public string? NombreArchivo { get; set; }

        [Column("DESCRIPC_TRECIBO")]
        [StringLength(200)]
        public string? DescripcTRecibo { get; set; }

        // Fechas (nullable por si vienen vacías). Recomiendo guardar en UTC.
        [Column("FECHA_PAGO")]
        public DateTime? FechaPago { get; set; }

        [Column("DFECHAREVERSO")]
        public DateTime? DFechaReverso { get; set; }

        [Column("MONTO")]
        [Precision(18, 2)]
        public decimal? Monto { get; set; }

        // Tipo de cambio: más precisión
        [Column("CAMBIO")]
        [Precision(18, 6)]
        public decimal? Cambio { get; set; }
    }
}
