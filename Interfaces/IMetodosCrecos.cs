using gestiones_backend.Dtos.In;

namespace gestiones_backend.Interfaces
{
    public interface IMetodosCrecos
    {
        string AsignacionAutomaticaDeudasCrecos();

        string AsignardeudaNullIdUsuario(string IdUsuario);
        Task<bool> SubirDeudasCrecosMasivomanual(List<DeudasCrecosMasivoInDto> deudas, CancellationToken ct);

    }
}
