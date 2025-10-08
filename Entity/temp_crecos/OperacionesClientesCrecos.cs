using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestiones_backend.Entity.temp_crecos
{
    public class OperacionesClientesCrecos
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [MaxLength(50)]
        public string? ICODIGOOPERACION { get; set; }

        [MaxLength(50)]
        public string? COD_OFICINA { get; set; }

        [MaxLength(150)]
        public string? DESC_OFICINA { get; set; }

        [MaxLength(50)]
        public string? N_IDENTIFICACION { get; set; }

        [MaxLength(50)]
        public string? NUM_FACTURA { get; set; }

        [MaxLength(10)]
        public string? COD_MONEDA { get; set; }

        [MaxLength(50)]
        public string? DESC_MONEDA { get; set; }

        [MaxLength(50)]
        public string? COD_PROD_FINANCIERO { get; set; }

        [MaxLength(150)]
        public string? DES_PROD_FINANCIERO { get; set; }

        [MaxLength(50)]
        public string? ICODIGO_OPERACION_NEGOCIACION { get; set; }

        public int? NUM_CUOTAS { get; set; }

        public decimal? TASA_INTERES { get; set; }        
        public string? FECHA_FACTURA { get; set; }
        public string? FECHA_ULTIMO_VENCIMIENTO { get; set; }
        public string? FECHA_ULTMO_PAGO { get; set; }

        public decimal? MONTO_CREDITO { get; set; }
        public decimal? VALOR_FINANCIAR { get; set; }

        [MaxLength(50)]
        public string? NUMERO_SOLICITUD { get; set; }

        [MaxLength(50)]
        public string? COD_T_OPERACION { get; set; }

        [MaxLength(150)]
        public string? DESC_T_OPERACION { get; set; }

        [MaxLength(50)]
        public string? COD_T_CREDITO { get; set; }

        [MaxLength(150)]
        public string? DESC_T_CREDITO { get; set; }

        [MaxLength(50)]
        public string? COD_ESTADO_OPERACION { get; set; }

        [MaxLength(150)]
        public string? DESC_ESTADO_OPERACION { get; set; }

        public int? SECUENC_CUPO { get; set; }

        [MaxLength(20)]
        public string? ESTADO_REGISTRO { get; set; }

        [MaxLength(100)]
        public string? DES_ESTADO_REGISTRO { get; set; }

        [MaxLength(50)]
        public string? COD_VENDEDOR { get; set; }

        [MaxLength(150)]
        public string? DESC_VENDEDOR { get; set; }

    }
}
