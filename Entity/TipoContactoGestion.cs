namespace gestiones_backend.Entity
{
    public class TipoContactoGestion
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
        public List<RespuestaTipoContacto> RespuestaTipoContactos { get; set; }
    }
}
