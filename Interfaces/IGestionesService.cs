using gestiones_backend.Dtos.In;
using gestiones_backend.Dtos.Out;

namespace gestiones_backend.Interfaces
{
    public interface IGestionesService
    {
       List<GestionOutDto> GetAllAsync();
        GestionOutDto? UpdateAsync(string idGestion, UpdateGestionDto dto);
        bool DeleteAsync(string idGestion);
        string  UltimoGestorGestionaDeuda (string idDeuda);

    }
}
