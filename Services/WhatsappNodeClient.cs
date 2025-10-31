using gestiones_backend.Dtos;
using gestiones_backend.Entity;
using gestiones_backend.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace gestiones_backend.Services
{
    public class WhatsappNodeClient : IWhatsappNodeClient
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);
        private readonly IAuthenticationService _authService;
        public WhatsappNodeClient(HttpClient http, IConfiguration cfg, IAuthenticationService authService   )
        {
            _http = http;
            var baseUrl = cfg.GetValue<string>("WhatsappNode:BaseUrl");
            baseUrl = baseUrl.TrimEnd('/') + "/";
            _http.BaseAddress = new Uri(baseUrl);
            _authService = authService;
        }

        public async Task<string> LoginAsync(string user, CancellationToken ct = default)
        {
            var res = await _http.PostAsJsonAsync("/api/login", new { user }, ct);
            res.EnsureSuccessStatusCode();
            var payload = await res.Content.ReadFromJsonAsync<LoginResponse>(_json, ct);
            return payload?.token ?? throw new InvalidOperationException("Token vacío");
        }

        private void UseToken(string token)
            => _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        public async Task<EnsureResponse> EnsureAsync(string user, CancellationToken ct = default)
        {
            //var token = await LoginAsync(user, ct);
            //Usuario usuario = _authService.GetCurrentUser();
            UseToken(_authService.GetCurrentToken());
            var res = await _http.GetAsync($"api/session/{Uri.EscapeDataString(user)}/ensure", ct);
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadFromJsonAsync<EnsureResponse>(_json, ct)
                   ?? throw new InvalidOperationException("Respuesta inválida (ensure)");
        }

        public async Task<StatusResponse> StatusAsync(string user, CancellationToken ct = default)
        {
            //var token = await LoginAsync(user, ct);
            UseToken(_authService.GetCurrentToken());
            var res = await _http.GetAsync($"api/session/{Uri.EscapeDataString(user)}/status", ct);
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadFromJsonAsync<StatusResponse>(_json, ct)
                   ?? throw new InvalidOperationException("Respuesta inválida (status)");
        }

        public async Task LogoutAsync(string user, CancellationToken ct = default)
        {
            //var token = await LoginAsync(user, ct);
            UseToken(_authService.GetCurrentToken());
            var res = await _http.PostAsync($"api/session/{Uri.EscapeDataString(user)}/logout", content: null, ct);
            res.EnsureSuccessStatusCode();
        }
    }
}