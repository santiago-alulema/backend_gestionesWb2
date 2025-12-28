using gestiones_backend.Context;
using gestiones_backend.Entity;
using gestiones_backend.Interfaces;

namespace gestiones_backend.Services
{
    
    public class MetodosCrecosServices : IMetodosCrecos
    {
        private readonly DataContext _dataContext;
        public MetodosCrecosServices(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public string AsignacionAutomaticaDeudasCrecos()
        {
            List<Usuario> usuariosActivos = _dataContext.Usuarios.Where(x => x.EstaActivo && x.Rol == "user").ToList();
            var deudasCrecosActivas = _dataContext.Deudas.Where(x => x.EsActivo == true && 
            x.Empresa.Contains("CRECO") && 
            x.IdUsuario == null).ToList();

            int cantidadUsuariosActivos = usuariosActivos.Count();
            int contador = 0;
            foreach (var item in deudasCrecosActivas)
            {
                item.IdUsuario = usuariosActivos[contador].IdUsuario;
                contador++;
                if (contador == cantidadUsuariosActivos) contador = 0;
            }
            _dataContext.SaveChanges();
            return "Ok";
        }

        public string AsignardeudaNullIdUsuario(string IdUsuario)
        {
            var deudasCrecosActivasUsuario = _dataContext.Deudas.Where(x => x.EsActivo == true &&
            x.Empresa.Contains("CRECO") &&
            x.IdUsuario == IdUsuario).ToList();
            foreach (var item in deudasCrecosActivasUsuario)
            {
                item.IdUsuario = null;
            }
            _dataContext.SaveChanges();
            return "Ok";
        }
    }
}
