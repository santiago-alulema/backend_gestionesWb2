using gestiones_backend.Dtos.In;

namespace gestiones_backend.Interfaces
{
    public interface IMensajesWhatsapp
    {
        public string MensajeWhatsapp(string tipoMensaje);
        public MensajeNuevoInDto CrearNuevoMensaje(MensajeNuevoInDto nuevoMensaje);
        public MensajeNuevoInDto UpdateNuevoMensaje(string mensajeId, string mensaje);
        public List<MensajeNuevoInDto> MesajesDeUsuarios();


    }
}
