using gestiones_backend.Dtos.In;
using gestiones_backend.Dtos.Out;

namespace gestiones_backend.Interfaces
{
    public interface ITareasService
    {
        List<TareaDto> GetAllAsync();
        TareaDto? UpdateAsync(string idCompromiso, UpdateTareaDto dto);
        bool DeleteAsync(string idCompromiso);
    }
}
