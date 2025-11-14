using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity.temp_crecos
{
    public class TrifocusCrecos
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? UsuarioGenera { get; set; }
        public string? GestorAsignado { get; set; }
        public string? Ciudad { get; set; }
        public string? CodigoCliente { get; set; }
        public string? IdentificacionCliente { get; set; }
        public string? NombreCliente { get; set; }
        public string? EstadoCivil { get; set; }
        public string? TipoCredito { get; set; }
        public string? TramoActual { get; set; }
        public string? Formalidad { get; set; }
        public string? Calificacion { get; set; }
        public int? NoCorte { get; set; }
        public string? Semaforo { get; set; }
        public string? Operacion { get; set; }
        public bool? DeudaRefinanciada { get; set; }  
        public DateOnly? FechaCompromisodePago { get; set; }
        public int? DiasVencidosIniciodeMes { get; set; }
        public string? TRAMO { get; set; }
        public int? DiasVencidosActuales { get; set; }
        public decimal? DeudaTotal { get; set; }
        public decimal? PagoMinimo { get; set; }
        public decimal? SaldoVencido { get; set; }
        public DateOnly? FechaUltimoPago { get; set; }
        public decimal? MontoUltimoPagado { get; set; }

        public string? TelefonoDomicilio { get; set; }
        public string? CelularDomicilio { get; set; }
        public string? DireccionDomicilio { get; set; }

        public string? TelefonoTrabajo { get; set; }
        public string? CelularTrabajo { get; set; }
        public string? Cargo { get; set; }
        public string? NombreTrabajo { get; set; }
        public string? DireccionTrabajo { get; set; }

        public string? TelefonoNegocio { get; set; }
        public string? CelularNegocio { get; set; }
        public string? DireccionNegocio { get; set; }

        public string? Referencia1 { get; set; }
        public string? TelefonoReferencia1 { get; set; }
        public string? Referencia2 { get; set; }
        public string? TelefonoReferencia2 { get; set; }

        public decimal? MontoCarteraAsignada { get; set; }
        public decimal? MontoCobrado { get; set; }
        public bool? PoseeVehiculo { get; set; }      // (SI/NO -> bool?)
        public DateOnly? UltimaGestionTerrena { get; set; }
        public DateOnly? GestionTerrenaAnterior { get; set; }
        public DateOnly? UltimaGestionTelefonica { get; set; }
        public DateOnly? GestionTelefonicaAnterior { get; set; }
        public int? NoGestiones { get; set; }
        public int? NoCuotasPagadas { get; set; }
        public decimal? ValLiquidacion { get; set; }
        public decimal? ValLiquidacionPartes { get; set; }
    }
}
