using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity.temp_crecos
{
    public class DatosClienteCrecos
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [MaxLength(50)]
        public string? CCODIGOTIPOPERSONA { get; set; }

        [MaxLength(100)]
        public string? TIPO_PERSONA { get; set; }

        [MaxLength(50)]
        public string? COD_TIPO_IDENTIF { get; set; }

        [MaxLength(100)]
        public string? DESCRIP_TIPO_IDENTIF { get; set; }

        [MaxLength(50)]
        public string? CNUMEROIDENTIFICACION { get; set; }

        [MaxLength(200)]
        public string? CNOMBRECOMPLETO { get; set; }

        [MaxLength(10)]
        public string? CEXENTOIMPUESTO { get; set; }

        [MaxLength(100)]
        public string? CPRIMERNOMBRE { get; set; }

        [MaxLength(100)]
        public string? CSEGUNDONOMBRE { get; set; }

        [MaxLength(100)]
        public string? CAPELLIDOPATERNO { get; set; }

        [MaxLength(100)]
        public string? CAPELLIDOMATERNO { get; set; }

        [MaxLength(50)]
        public string? CCODIGOESTADOCIVIL { get; set; }

        [MaxLength(100)]
        public string? DESCRIP_ESTADO_CIVIL { get; set; }

        [MaxLength(50)]
        public string? CCODIGOSEXO { get; set; }

        [MaxLength(100)]
        public string? DESCRIP_SEXO { get; set; }

        [MaxLength(50)]
        public string? CCODIGOPAIS { get; set; }

        [MaxLength(100)]
        public string? DESCRIP_PAIS { get; set; }

        [MaxLength(50)]
        public string? CCODIGOCIUDADNACIMIENTO { get; set; }

        [MaxLength(150)]
        public string? DESCRIP_CIUDAD_NACIMIENTO { get; set; }

        [MaxLength(50)]
        public string? CCODIGONACIONALIDAD { get; set; }

        [MaxLength(100)]
        public string? DESCRIP_NACIONALIDAD { get; set; }

        public string? DFECHANACIMIENTO { get; set; }

        public int? INUMEROCARGA { get; set; }

        [MaxLength(10)]
        public string? CSEPARACIONBIEN { get; set; }

        [MaxLength(50)]
        public string? CCODIGONIVELEDUCACION { get; set; }

        [MaxLength(150)]
        public string? DESCRIP_NIVEL_EDUC { get; set; }

        [MaxLength(50)]
        public string? CCODIGOTITULO { get; set; }

        [MaxLength(150)]
        public string? DESCRIP_TITULO { get; set; }

        [MaxLength(200)]
        public string? CRAZONCOMERCIAL { get; set; }

        [MaxLength(200)]
        public string? CNOMBREEMPRESA { get; set; }

        [MaxLength(50)]
        public string? CCODIGOCARGO { get; set; }

        [MaxLength(150)]
        public string? DESCRIP_CARGO { get; set; }

        public string? DFECHAINGRESOLAB { get; set; }

        public decimal? IINGRESO { get; set; }

        public decimal? IEGRESO { get; set; }

        [MaxLength(50)]
        public string? ICODIGOCLIENTE { get; set; }

        public int? ISCORE { get; set; }
    }
}
