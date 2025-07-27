namespace gestiones_backend.Dtos.Out
{
    public class GestionDto
    {
        public string idGestion { get; set; }
        public DateTime fechaGestion { get; set; }
        public string deudor { get; set; } = string.Empty;
        public string tipoGestion { get; set; } = string.Empty;
        public string descripcion { get; set; } = string.Empty;
        public string usuario { get; set; } = string.Empty;
    }
}
