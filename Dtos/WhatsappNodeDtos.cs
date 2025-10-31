namespace gestiones_backend.Dtos
{
    public record LoginResponse(bool ok, string token);

    // GET /api/session/:user/ensure
    public record EnsureResponse(
        string user,
        bool ready,
        string? qrDataUrl,
        string? reason,
        bool exists
    );

 
    // GET /api/session/:user/status
    public record StatusResponse(
        string user,
        bool ready,
        bool hasQr,
        bool exists
    );
}
