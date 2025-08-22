namespace gestiones_backend.Dtos.Out
{
    public class ClientesOutDTO
    {
        public string cedula { get; set; } = string.Empty;

        public string nombre { get; set; } = string.Empty;
        public string direccion { get; set; } = string.Empty;
        public string telefono { get; set; } = string.Empty;
        public string correo { get; set; } = string.Empty;
        public string descripcion { get; set; } = string.Empty;
        public string numerofactura { get; set; } = string.Empty;
        public string numeroDeudas { get; set; } = string.Empty;
        public string gestor { get; set; } = string.Empty;
        public string tramos { get; set; } = string.Empty;
    }
}
