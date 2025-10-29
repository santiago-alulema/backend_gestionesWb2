using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestiones_backend.Entity.temp_crecos
{
    public class ReciboFormaPagoCrecos
    {

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Column("COD_RECIBO_FORMAPAGO")]
        [StringLength(100)]
        public string CodReciboFormaPago { get; set; } = Guid.NewGuid().ToString();

        [Column("COD_RECIBO")]
        [StringLength(100)]
        public string? CodRecibo { get; set; }

        [Column("COD_FORMA_PAGO")]
        [StringLength(50)]
        public string? CodFormaPago { get; set; }

        public string? NombreArchivo { get; set; }


        [Column("DESCRIPC_FPAGO")]
        [StringLength(200)]
        public string? DescripcFPago { get; set; }

        [Column("COD_INS_FINANCIERA")]
        [StringLength(50)]
        public string? CodInsFinanciera { get; set; }

        [Column("CDESCRIPCION_INSTITUCION_FINANCIERA")]
        [StringLength(200)]
        public string? CDescripcionInstitucionFinanciera { get; set; }

        [Column("NUM_CUENTA")]
        [StringLength(100)]
        public string? NumCuenta { get; set; }

        [Column("NUM_DOCUMENTO")]
        [StringLength(100)]
        public string? NumDocumento { get; set; }

        [Column("CNOMBRECUENTACORRENTISTA")]
        [StringLength(200)]
        public string? CNombreCuentaCorrentista { get; set; }

        [Column("CCEDULACUENTACORRENTISTA")]
        [StringLength(50)]
        public string? CCedulaCuentaCorrentista { get; set; }

        [Column("DFECHACOBRODOCUMENTO")]
        public DateTime? DFechaCobroDocumento { get; set; }

        [Column("COD_MONEDA")]
        [StringLength(10)]
        public string? CodMoneda { get; set; }

        [Column("DESCRIPC_MONEDA")]
        [StringLength(100)]
        public string? DescripcMoneda { get; set; }

        [Column("COD_MOTIVO")]
        [StringLength(50)]
        public string? CodMotivo { get; set; }

        [Column("DESCRIPC_MOTIVO")]
        [StringLength(200)]
        public string? DescripcMotivo { get; set; }

        [Column("IVALOR")]
        [Precision(18, 2)]
        public decimal? IValor { get; set; }

    }
}
