using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace gestiones_backend.Dtos.In
{
    public class RegistroPagoInDto
    {
        [JsonPropertyName("usuario")]
        public string Usuario { get; set; }

        [JsonPropertyName("cedente")]
        public string Cedente { get; set; }

        [JsonPropertyName("archivo")]
        public string Archivo { get; set; }

        [JsonPropertyName("calificacion")]
        public string Calificacion { get; set; }

        [JsonPropertyName("malla")]
        public string Malla { get; set; }

        [JsonPropertyName("tramo")]
        public string Tramo { get; set; }

        [JsonPropertyName("cxc")]
        public string CXC { get; set; }

        [JsonPropertyName("nombreDeudor")]
        public string NombreDeudor { get; set; }

        [JsonPropertyName("cedulaDeudor")]
        public string CedulaDeudor { get; set; }

        [JsonPropertyName("fecha_Pago")]
        public DateTime? FechaPago { get; set; }

        [JsonPropertyName("monto")]
        public decimal? Monto { get; set; }

        [JsonPropertyName("nroControl")]
        public string NroControl { get; set; }

        [JsonPropertyName("nro_Documento")]
        public string NroDocumento { get; set; }

        [JsonPropertyName("banco")]
        public string Banco { get; set; }

        [JsonPropertyName("abonoLiquidacion")]
        public string AbonoLiquidacion { get; set; }

        [JsonPropertyName("verificado")]
        public bool? Verificado { get; set; }
    }
}
