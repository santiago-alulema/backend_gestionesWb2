namespace gestiones_backend.Dtos.Out
{
    public class TelefonosActivosDeudorOutDTO
    {
        public String IdDeudorTelefonos { get; set; }
        public string Telefono { get; set; } = null!;
        public string Propietario { get; set; } = null!;
        public bool EsValido { get; set; } = false!;


    }
}
