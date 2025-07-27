namespace gestiones_backend.Dtos.Out
{
    public class VerificarTelefonoClienteOutDTO
    {
        public string Telefono { get; set; } = null!;
        public string Estado { get; set; } = null!; // NoExiste - Existe - Inactivo
        public string Observacion { get; set; } = null!;
    }
}
