using System.Collections.ObjectModel;

namespace gestiones_backend.Entity
{
    public class TipoTarea
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public bool Estado { get; set; }
        public Collection<CompromisosPago> CompromisosPagos { get; set; }

    }
}
