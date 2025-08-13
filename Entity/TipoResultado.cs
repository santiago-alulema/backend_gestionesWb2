namespace gestiones_backend.Entity
{
    public class TipoResultado
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public Boolean Activo { get; set; }
        public List<TipoContactoResultado> TiposConstactosNavigation { get; set; }

    }
}
