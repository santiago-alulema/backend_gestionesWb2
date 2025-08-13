namespace gestiones_backend.Entity
{
    public class TipoContactoResultado
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public Boolean Activo { get; set; }
        public string TipoResultadoId { get; set; }

        public TipoResultado TipoResultadoNavigation { get; set; }
        public List<RespuestaTipoContacto> TiposRespuestaNavigation { get; set; }
    }
}
