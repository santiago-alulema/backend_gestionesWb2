namespace gestiones_backend.Dtos.In
{
    public class TelefonosDeudorInDTO
    {
        public string? cedula { get; set; }
        public string? telefono { get; set; }
        public DateTime? fechaAdicion { get; set; } = DateTime.Now;
        public bool? esValido { get; set; } = true;
        public string? origen { get; set; }
    }
}
