namespace gestiones_backend.Dtos.In
{
    public class GestionInDTO
    {
        public Guid idDeuda { get; set; }
        public string IdTipoGestion { get; set; }
        public string Descripcion { get; set; }
        public string IdTipoContactoDeudor { get; set; }
        public string IdRespuesta { get; set; }
        public string Email { get; set; }

    }
}
