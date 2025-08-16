namespace gestiones_backend.Dtos.In
{
    public class UploadDeudoresInDTO
    {
        public string Cedula { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public string? Direccion { get; set; }

        public string? Telefono { get; set; } = null!;

        public string? Correo { get; set; }

        public string? Descripcion { get; set; }
        public string? Usuario { get; set; }

    }
}
