using gestiones_backend.Entity;

namespace gestiones_backend.Interfaces
{
    public interface IAuthenticationService
    {
        public Usuario GetCurrentUser();
        public Task<Usuario> GetCurrentUserAsync();
        string? GetCurrentToken();
    }
}
