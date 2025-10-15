using gestiones_backend.Dtos.Out;
using gestiones_backend.Entity;
using Mapster;

namespace gestiones_backend.ConfigurationsMapper
{
    public class ConfigDeudasCliente
    {
        public static void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Deuda, DeudasClienteOutDTO>()
                .Map(dest => dest.Nombre, src => src.IdDeudorNavigation != null ? src.IdDeudorNavigation.Nombre : null)
                .Map(dest => dest.NombreCompleto, src => src.Usuario != null ? src.Usuario.NombreCompleto : null);
        }
    }
}
