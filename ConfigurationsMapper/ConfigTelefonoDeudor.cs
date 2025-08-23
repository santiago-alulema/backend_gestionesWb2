using gestiones_backend.Dtos.In;
using gestiones_backend.Entity;
using Mapster;
using System.Linq;

namespace gestiones_backend.ConfigurationsMapper
{
    public class ConfigTelefonoDeudor
    {
        public static void Register(TypeAdapterConfig config)
        {
            config.NewConfig<TelefonosDeudorInDTO, DeudorTelefono>()
                   .Ignore(dest => dest.IdDeudorNavigation)
                   .Map(dest => dest.FechaAdicion, src => DateTime.UtcNow)
                   .Map(dest => dest.IdDeudorTelefonos, src => Guid.NewGuid().ToString())
                   .Map(dest => dest.IdDeudor, src => src.cedula)
                   .Map(dest => dest.Origen, src => src.origen)
                   .Map(dest => dest.Propietario, src => src.propietario)
                   .Map(dest => dest.EsValido, src => src.esValido)
                   .Map(dest => dest.Telefono, src => src.telefono);

            config.NewConfig<GrabarTelefonoNuevoInDTO, DeudorTelefono>()
                .Ignore(dest => dest.IdDeudorNavigation)
                .Map(dest => dest.FechaAdicion, src => DateTime.UtcNow)
                .Map(dest => dest.IdDeudorTelefonos, src => Guid.NewGuid().ToString())
                .Map(dest => dest.IdDeudor, src => src.cedula)
                .Map(dest => dest.Origen, src => src.origen)
                .Map(dest => dest.EsValido, src => src.esValido)
                .Map(dest => dest.Telefono, src => src.telefono)
                .Map(dest => dest.Propietario, src => src.propietario);

        }
    }
}
