namespace gestiones_backend.Dtos.Out
{
    public class GestionOutDto
    {
        public string IdGestion { get; set; }
        public Guid IdDeuda { get; set; }
        public string FechaGestion { get; set; }
        public string Descripcion { get; set; }
        public string Email { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string IdUsuarioGestiona { get; set; }
        public string UsuarioGestiona { get; set; }
        public string IdTipoContactoResultado { get; set; }
        public string TipoContactoResultado { get; set; }
        public string IdTipoResultado { get; set; }
        public string TipoResultado { get; set; }
        public string IdRespuestaTipoContacto { get; set; }
        public string RespuestaTipoContacto { get; set; }

    }
}
