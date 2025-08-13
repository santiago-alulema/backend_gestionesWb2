namespace gestiones_backend.Entity
{
    public class RespuestaTipoContacto
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public Boolean Activo { get; set; }
        public string IdTipoContactoResultado { get; set; }
        public TipoContactoResultado TipoContactoNavigatorNavigation { get; set; }
    }
}
