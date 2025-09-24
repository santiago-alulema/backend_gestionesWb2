using gestiones_backend.Context;
using gestiones_backend.Dtos.In;
using gestiones_backend.Entity;
using gestiones_backend.Interfaces;
using Mapster;

namespace gestiones_backend.Services
{
    public class MensajesWhatsappServices : IMensajesWhatsapp
    {
        private readonly DataContext _context;
        private readonly IAuthenticationService _authService;
        public MensajesWhatsappServices(DataContext context,
            IAuthenticationService authService)
        {
            _context = context;
            _authService = authService;
        }

        public MensajeNuevoInDto CrearNuevoMensaje(MensajeNuevoInDto nuevoMensajeDto)
        {
            Usuario usuario = _authService.GetCurrentUser();

            MensajeWhatsappUsuario nuevoMensaje = new MensajeWhatsappUsuario() { Id = Guid.NewGuid().ToString(),
            //IdUsuario = usuario.IdUsuario,
            Mensaje = nuevoMensajeDto?.Mensaje ?? "",
            TipoMensaje = nuevoMensajeDto.TipoMensaje
            };
            _context.MensajesWhatsapp.Add(nuevoMensaje);
            return nuevoMensajeDto;
        }

        public string MensajeWhatsapp(string tipoMensaje)
        {
            Usuario usuario = _authService.GetCurrentUser();
            string mensaje = _context.MensajesWhatsapp.Where(x =>  x.TipoMensaje == tipoMensaje).Select(x => x.Mensaje).FirstOrDefault() ?? "";
            return mensaje;
        }

        public List<MensajeNuevoInDto> MesajesDeUsuarios()
        {
            List<MensajeNuevoInDto> listaMensajes = new();
            Usuario usuario = _authService.GetCurrentUser();
            List<MensajeWhatsappUsuario> mensajes = _context.MensajesWhatsapp.ToList();
            return mensajes.Adapt<List<MensajeNuevoInDto>>();
        }

        public MensajeNuevoInDto UpdateNuevoMensaje(string mensajeId, string mensaje)
        {
            throw new NotImplementedException();
        }
    }
}
