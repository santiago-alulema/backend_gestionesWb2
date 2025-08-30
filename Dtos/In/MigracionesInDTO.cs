using gestiones_backend.helpers;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace gestiones_backend.Dtos.In
{
    public class MigracionesInDTO
    {
        [JsonPropertyName("__rowNumber")]
        public int? RowNumber { get; set; }   // ya que viene en tu JSON

        [JsonPropertyName("user_name")]
        public string? UserName { get; set; }

        [JsonPropertyName("malla")]
        public string? Malla { get; set; }

        [JsonPropertyName("cedula")]
        public string? Cedula { get; set; }

        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; }

        [JsonPropertyName("ciudad")]
        public string? Ciudad { get; set; }

        [JsonPropertyName("agencia")]
        public string? Agencia { get; set; }

        [JsonPropertyName("direccion")]
        public string? Direccion { get; set; }

        [JsonPropertyName("operacion_cxc")]
        public string? OperacionCxc { get; set; } // ← texto, viene con comilla inicial

        [JsonPropertyName("creditos")]
        public int? Creditos { get; set; }

        [JsonPropertyName("sueldo")]
        public decimal? Sueldo { get; set; }

        [JsonPropertyName("dias_en_mora")]
        public int? DiasEnMora { get; set; }

        [JsonPropertyName("dias_en_mora_actual")]
        public int? DiasEnMoraActual { get; set; }

        [JsonPropertyName("tramo")]
        public string? Tramo { get; set; }

        [JsonPropertyName("valor_vencido")]
        public decimal? ValorVencido { get; set; }

        [JsonPropertyName("interes_gastos_de_cobranza")]
        public decimal? InteresGastosDeCobranza { get; set; }

        [JsonPropertyName("deuda_total")]
        public decimal? DeudaTotal { get; set; }

        // ⇩⇩ CAMBIO: texto (p.ej. "HASTA EL 0,55")
        [JsonPropertyName("descuento")]
        public string? Descuento { get; set; }

        [JsonPropertyName("monto_a_cobrar")]
        public decimal? MontoACobrar { get; set; }

        [JsonPropertyName("valor_cuota")]
        public decimal? ValorCuota { get; set; }

        [JsonPropertyName("cuotas_convenio")]
        public int? CuotasConvenio { get; set; }

        [JsonPropertyName("cestadotarjeta")]
        public string? CestadoTarjeta { get; set; }

        [JsonPropertyName("cestadooperacion")]
        public string? CestadoOperacion { get; set; }

        [JsonPropertyName("calificacion")]
        public string? Calificacion { get; set; }

        [JsonPropertyName("pagos_vencidos")]
        public int? PagosVencidos { get; set; }

        [JsonPropertyName("pago_minimo")]
        public decimal? PagoMinimo { get; set; }

        [JsonPropertyName("valor_vencido_actual")]
        public decimal? ValorVencidoActual { get; set; }

        [JsonPropertyName("capital_provision")]
        public decimal? CapitalProvision { get; set; }

        // ⇩⇩ CAMBIO: texto
        [JsonPropertyName("cupo_utilizado")]
        public string? CupoUtilizado { get; set; }

        // ⇩⇩ CAMBIO: texto
        [JsonPropertyName("cupo_utilizado_actual")]
        public string? CupoUtilizadoActual { get; set; }

        // ⇩⇩ CAMBIO: texto
        [JsonPropertyName("int_rec")]
        public string? InteresRec { get; set; }

        [JsonPropertyName("producto")]
        public string? Producto { get; set; }

        [JsonPropertyName("fecha_tope_pago")]
        [JsonConverter(typeof(NullableFlexibleDateTimeConverter))]
        public DateTime? FechaTopePago { get; set; }

        [JsonPropertyName("fecha_ultimo_pago")]
        [JsonConverter(typeof(NullableFlexibleDateTimeConverter))]
        public DateTime? FechaUltimoPago { get; set; }

        [JsonPropertyName("condonacion_castigada")]
        public decimal? CondonacionCastigada { get; set; }

        [JsonPropertyName("condonacion_vencida")]
        public decimal? CondonacionVencida { get; set; }

        [JsonPropertyName("cao")]
        public string? Cao { get; set; }

        [JsonPropertyName("credito")]
        public string? Credito { get; set; }

        // ⇩⇩ CAMBIO: texto (viene "EC0213...")
        [JsonPropertyName("coactivas")]
        public string? Coactivas { get; set; }

        [JsonPropertyName("ULTIMA-GESTION:")]
        [Column("ULTIMA-GESTION:")]
        public string? UltimaGestion { get; set; }

        [JsonPropertyName("usuario")]
        public string? Usuario { get; set; }

        [JsonPropertyName("resulted")]
        public string? Resulted { get; set; }

        [JsonPropertyName("typeresulted")]
        public string? TypeResulted { get; set; }

        [JsonPropertyName("responsed")]
        public string? ResponseD { get; set; }

        [JsonPropertyName("typecodephone")]
        public string? TypeCodePhone { get; set; }

        [JsonPropertyName("codephone")]
        public string? CodePhone { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("created")]
        [JsonConverter(typeof(NullableFlexibleDateTimeConverter))]
        public DateTime? Created { get; set; }

        [JsonPropertyName("compromiso")]
        public string? Compromiso { get; set; }

        [JsonPropertyName("monto")]
        public decimal? Monto { get; set; }

        [JsonPropertyName("fecha")]
        [JsonConverter(typeof(NullableFlexibleDateTimeConverter))]
        public DateTime? Fecha { get; set; }
    }
}
