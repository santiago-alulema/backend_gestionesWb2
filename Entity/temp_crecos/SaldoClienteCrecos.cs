using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity.temp_crecos
{
    public class SaldoClienteCrecos
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [MaxLength(50)] public string? COD_EMPRESA { get; set; }
        [MaxLength(200)] public string? DESCP_EMPRESA { get; set; }
        [MaxLength(50)] public string? COD_U_NEGOCIO { get; set; }
        [MaxLength(200)] public string? DESC_U_NEGOCIO { get; set; }
        [MaxLength(50)] public string? COD_CARTERA { get; set; }
        [MaxLength(200)] public string? DESCRIP_CARTERA { get; set; }
        [MaxLength(50)] public string? COD_GESTOR { get; set; }
        [MaxLength(200)] public string? DESC_GESTOR { get; set; }

        public int? IMES { get; set; }                 // Mes (1-12)
        public int? IANO { get; set; }                 // Año (yyyy)

        [MaxLength(50)] public string? COD_OFICINA { get; set; }
        [MaxLength(200)] public string? CDESCRIPCION_OFICINA { get; set; }
        [MaxLength(50)] public string? COD_TCREDITO { get; set; }
        [MaxLength(200)] public string? DESCRIP_TCREDITO { get; set; }

        [MaxLength(50)] public string? CNUMEROIDENTIFICACION { get; set; }
        [MaxLength(50)] public string? COD_OPERACION { get; set; }
        [MaxLength(50)] public string? CNUMEROTARJETA { get; set; }

        [MaxLength(50)] public string? CICLO_CORTE { get; set; }
        [MaxLength(150)] public string? DESC_CICLOCORTE { get; set; }

        public int? DIAS_VENCIDOS { get; set; }
        public int? ITRAMO { get; set; }
        [MaxLength(200)] public string? CDESCRIPCIONTRAMO { get; set; }

        public string? FECHA_MAX_PAGO { get; set; }

        public decimal? VALOR_DEUDA { get; set; }
        public decimal? VALOR_PAGO_MINIMO { get; set; }
        public decimal? VALOR_CORRIENTE { get; set; }
        public decimal? VALOR_VENCIDO { get; set; }
        public decimal? VALOR_POR_VENCER { get; set; }
        public decimal? VALOR_MORA { get; set; }
        public decimal? VALOR_GESTION { get; set; }
        public decimal? VALOR_VENCIDO_CORTEANTERIOR { get; set; }

        public string? PRIMERA_CUOTA_VENCIDA { get; set; }   // Si viene como fecha

        [MaxLength(10)] public string? NEGOCIACION_ACTIVA { get; set; } // “S/N”, “Y/N”

        public string? DFECHAEJECUCION { get; set; }
        public string? FECHA_INGRESO { get; set; }

        [MaxLength(50)] public string? CALIFICACION_CLIENTE { get; set; }

        public string? F_ULTIMO_CORTE { get; set; }
        public string? FECHA_ULT_PAGO { get; set; }

        public decimal? VAL_ULT_PAGO { get; set; }
        public decimal? VALOR_PAGO_MINIMO_ACTUALIZADO { get; set; }
        public string? NOMBRE_ARCHIVO { get; set; }

        [MaxLength(50)] public string? CODIGOCLIENTE { get; set; }
    }
}
