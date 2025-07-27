using gestiones_backend.Dtos.In;
using gestiones_backend.Entity;
using Mapster;

namespace gestiones_backend.ConfigurationsMapper
{
    public class ConfigTelefonoDeudor
    {
        public static void Register(TypeAdapterConfig config)
        {
            config.NewConfig<TelefonosDeudorInDTO, DeudorTelefono>()
                .Ignore(dest => dest.IdDeudorTelefonos)
                .Ignore(dest => dest.IdDeudorNavigation)
                .Map(dest => dest.FechaAdicion, src => DateTime.Now)
                .Map(dest => dest.IdDeudor, src => src.cedula)
                .Map(dest => dest.Origen, src => src.origen)
                .Map(dest => dest.EsValido, src => true)
                .Map(dest => dest.Telefono, src => src.telefono);

            config.NewConfig<GrabarTelefonoNuevoInDTO, DeudorTelefono>()
                .Ignore(dest => dest.IdDeudorTelefonos)
                .Ignore(dest => dest.IdDeudorNavigation)
                .Map(dest => dest.FechaAdicion, src => DateTime.Now)
                .Map(dest => dest.IdDeudor, src => src.cedula)
                .Map(dest => dest.Origen, src => src.origen)
                .Map(dest => dest.EsValido, src => true)
                .Map(dest => dest.Telefono, src => src.telefono);
        }
    }
}
