using gestiones_backend.Dtos.In;
using gestiones_backend.Dtos.Out;

namespace gestiones_backend.Interfaces
{
    public interface IPagosService
    {
        List<PagoDto> GetAllAsync();
        PagoDto? UpdateAsync(string idPago, UpdatePagoDto dto);
        bool DeleteAsync(string idPago);
    }
}
