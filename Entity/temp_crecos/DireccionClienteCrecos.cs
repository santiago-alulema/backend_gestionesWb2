using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity.temp_crecos
{
    public class DireccionClienteCrecos
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? CNUMEROIDENTIFICACION { get; set; }

        public string? CODIGO_UBICACION { get; set; }

        public string? DESCRIP_UBICACION { get; set; }

        public string? COD_PAIS { get; set; }

        public string? DESCRIP_PAIS { get; set; }

        public string? COD_PROVINCIA { get; set; }

        public string? DESCRIP_PROVINCIA { get; set; }

        [MaxLength(50)]
        public string? COD_CANTON { get; set; }

        [MaxLength(150)]
        public string? DESCRIP_CANTON { get; set; }

        [MaxLength(50)]
        public string? COD_PARROQUIA { get; set; }

        [MaxLength(150)]
        public string? DESCRIP_PARROQUIA { get; set; }

        [MaxLength(50)]
        public string? COD_BARRIO { get; set; }

        [MaxLength(150)]
        public string? DESCRIP_BARRIO { get; set; }

        [MaxLength(250)]
        public string? CDIRECCIONCARGA { get; set; }

        [MaxLength(250)]
        public string? COBSERVACION { get; set; }

        [MaxLength(300)]
        public string? CDIRECCIONCOMPLETA { get; set; }

        [MaxLength(50)]
        public string? CCASILLA { get; set; }

        [MaxLength(150)]
        public string? CCORREOELECTRONICO { get; set; }

        [MaxLength(50)]
        public string? COD_TIPO_ESPACIO { get; set; }

        [MaxLength(150)]
        public string? DESCRIP_TIPO_ESPACIO { get; set; }

        public int? INUMEROESPACIO { get; set; }

        public int? INUMEROSOLAR { get; set; }

        [MaxLength(200)]
        public string? CCALLEPRINCIPAL { get; set; }

        [MaxLength(50)]
        public string? CNUMEROCALLE { get; set; }

        [MaxLength(200)]
        public string? CALLE_SECUND { get; set; }

        [MaxLength(200)]
        public string? CALLE_SECUND_2 { get; set; }

        [MaxLength(50)]
        public string? CNUMERO_SOLAR { get; set; }

        [MaxLength(50)]
        public string? COD_TIPO_NUMERACION { get; set; }

        [MaxLength(150)]
        public string? DESCRIP_TIPO_NUMERACION { get; set; }

        [MaxLength(50)]
        public string? COD_INDICADOR_POSICION { get; set; }

        [MaxLength(150)]
        public string? DESCRIP_IND_POSICION { get; set; }

        [MaxLength(150)]
        public string? NOMBRE_EDIFICIO { get; set; }

        [MaxLength(20)]
        public string? CNUMEROPISO { get; set; }

        [MaxLength(50)]
        public string? PISO_BLOQUE { get; set; }

        [MaxLength(100)]
        public string? COFICINA_DEPARTAMENTO { get; set; }

        [MaxLength(10)]
        public string? INDICADOR_PRINCIPAL { get; set; }

        [MaxLength(50)]
        public string? COD_T_PROPIEDAD { get; set; }

        [MaxLength(150)]
        public string? DESCRIP_TIPO_PROPIEDAD { get; set; }

        public int? AÑO_ANTIGUEDAD { get; set; }

        public int? MES_ANTIGUEDAD { get; set; }

        public int? DIAS_ANTIGUEDAD { get; set; }
    }
}
