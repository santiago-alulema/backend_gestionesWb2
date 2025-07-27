namespace gestiones_backend.Dtos.In
{
    public class GrabarTelefonoNuevoInDTO
    {
        public string cedula { get; set; }
        public string telefono { get; set; }
        public Boolean esValido { get; set; }
        public string origen { get; set; }

    }
}
