namespace gestiones_backend.Interfaces
{
    public interface IUserContext
    {
        bool IsAuthenticated { get; }
        string? UserName { get; }
        string? Token { get; }
    }
}
