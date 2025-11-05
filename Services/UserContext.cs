using gestiones_backend.Interfaces;
using System.Security.Claims;

namespace gestiones_backend.Services
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _http;
        public UserContext(IHttpContextAccessor http) => _http = http;

        public bool IsAuthenticated => (_http.HttpContext?.User?.Identity?.IsAuthenticated) == true;

        public string? UserName =>
            _http.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value
            ?? _http.HttpContext?.User.FindFirst("name")?.Value
            ?? _http.HttpContext?.User.FindFirst("unique_name")?.Value
            ?? _http.HttpContext?.User.FindFirst("preferred_username")?.Value;

        public string? Token
        {
            get
            {
                var h = _http.HttpContext?.Request.Headers["Authorization"].ToString();
                return (!string.IsNullOrWhiteSpace(h) && h.StartsWith("Bearer ")) ? h["Bearer ".Length..].Trim() : null;
            }
        }
    }    }