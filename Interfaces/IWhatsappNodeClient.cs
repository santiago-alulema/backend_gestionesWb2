
using gestiones_backend.Dtos;

namespace gestiones_backend.Interfaces
{
    public interface IWhatsappNodeClient
    {
        Task<string> LoginAsync(string user, CancellationToken ct = default);
        Task<EnsureResponse> EnsureAsync(string user, CancellationToken ct = default);
        Task<StatusResponse> StatusAsync(string user, CancellationToken ct = default);
        Task LogoutAsync(string user, CancellationToken ct = default);
    }
}
