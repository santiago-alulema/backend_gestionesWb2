using gestiones_backend.Context;
using gestiones_backend.Dtos;
using gestiones_backend.Entity;
using gestiones_backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/whatsapp")]
public class WhatsappSessionsController : ControllerBase
{
    private readonly DataContext _ctx;
    private readonly IWhatsappNodeClient _wa;
    private readonly IAuthenticationService _authService;
    public WhatsappSessionsController(DataContext ctx,
                                      IWhatsappNodeClient wa,
                                      IAuthenticationService authService)
    {
        _ctx = ctx;
        _wa = wa;
        _authService = authService;
    }

    public class EnsureDto { public string User { get; set; } = default!; }
    public class UpsertSessionDto
    {
        public string User { get; set; } = default!;
        public bool Ready { get; set; }
        public bool ExistsOnNode { get; set; }
        public string? LastReason { get; set; }
    }

    [HttpGet("sessions")]
    public async Task<IActionResult> List()
    {
        Usuario usuario = _authService.GetCurrentUser();
        IQueryable<WhatsappSession> query = _ctx.WhatsappSessions;
        if (usuario.Rol != "admin")
        {
            query = query.Where(x => x.IdUsuario == usuario.IdUsuario);
        }
        var items = await query
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync();
        return Ok(items);
    }

    //[HttpPost("sessions/ensure")]
    //public async Task<IActionResult> Ensure([FromBody] EnsureDto body, CancellationToken ct)
    //{
    //    Usuario usuario = _authService.GetCurrentUser();

    //    if (string.IsNullOrWhiteSpace(body.User)) return BadRequest("User requerido");
    //    var user = body.User.Trim().ToLower();
    //    WhatsappSession? sessionExiste = _ctx.WhatsappSessions.FirstOrDefault(x => x.User == user  );
    //   if (sessionExiste != null && sessionExiste.IdUsuario != usuario.IdUsuario) 
    //        return BadRequest($"Usuario <strong>{user}</strong> de whatsapp ya existe");
    //    var ensure = await _wa.EnsureAsync(user, ct);

    //    await UpsertInternal(new UpsertSessionDto
    //    {
    //        User = user,
    //        Ready = ensure.ready,
    //        ExistsOnNode = ensure.exists,
    //        LastReason = ensure.reason
    //    }, ct);

    //    return Ok(new
    //    {
    //        ok = true,
    //        user = ensure.user,
    //        ready = ensure.ready,
    //        qrDataUrl = ensure.qrDataUrl,
    //        reason = ensure.reason
    //    });
    //}


    [HttpPost("sessions/ensure")]
    public async Task<IActionResult> Ensure([FromBody] EnsureDto body, CancellationToken ct)
    {
        try
        {
            Usuario usuario = _authService.GetCurrentUser();

            if (body == null || string.IsNullOrWhiteSpace(body.User))
                return BadRequest(new { ok = false, error = "User requerido" });

            var user = body.User.Trim().ToLower();

            // 1) Validación de pertenencia / unicidad del user
            var sessionExiste = await _ctx.WhatsappSessions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.User == user, ct);

            if (sessionExiste != null && sessionExiste.IdUsuario != usuario.IdUsuario)
            {
                return BadRequest(new
                {
                    ok = false,
                    error = $"El usuario de WhatsApp '{user}' ya está asociado a otro usuario del sistema."
                });
            }

            // 2) Polling hacia Node para levantar sesión o traer QR
            var maxWait = TimeSpan.FromSeconds(65);      // tiempo total máximo de espera
            var delay = TimeSpan.FromMilliseconds(600);  // backoff inicial
            var delayMax = TimeSpan.FromSeconds(3);      // tope de backoff
            var start = DateTime.UtcNow;

            EnsureResponse ensure = null!;

            while (true)
            {
                ct.ThrowIfCancellationRequested();

                ensure = await _wa.EnsureAsync(user, ct);   // llama a Node

                if (ensure == null)
                {
                    return StatusCode(502, new
                    {
                        ok = false,
                        error = "Node no respondió correctamente en EnsureAsync."
                    });
                }

                // Si ya está autenticado / listo -> salimos (no habrá QR)
                if (ensure.ready)
                    break;

                // Si Node ya devolvió un QR (dataURL), salimos para que el frontend lo muestre
                if (!string.IsNullOrWhiteSpace(ensure.qrDataUrl))
                    break;

                // Timeout global
                if (DateTime.UtcNow - start > maxWait)
                {
                    return StatusCode(504, new
                    {
                        ok = false,
                        error = "Timeout esperando estado/QR desde Node (polling)."
                    });
                }

                // Backoff suave
                await Task.Delay(delay, ct);
                var nextMs = Math.Min(delay.TotalMilliseconds * 1.5, delayMax.TotalMilliseconds);
                delay = TimeSpan.FromMilliseconds(nextMs);
            }

            // 3) Upsert del estado en tu tabla
            await UpsertInternal(new UpsertSessionDto
            {
                User = user,
                Ready = ensure.ready,
                ExistsOnNode = ensure.exists,
                LastReason = ensure.reason
            }, ct);

            // 4) Respuesta final hacia el frontend
            var hasQr = !string.IsNullOrWhiteSpace(ensure.qrDataUrl);

            return Ok(new
            {
                ok = true,
                user = ensure.user,
                ready = ensure.ready,
                exists = ensure.exists,
                hasQr,
                qrDataUrl = ensure.qrDataUrl,
                reason = ensure.reason
            });
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { ok = false, error = "Cancelado por el cliente." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { ok = false, error = ex.Message });
        }
    }


    [HttpGet("sessions/status")]
    public async Task<IActionResult> Status([FromQuery] string user, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(user))
            return BadRequest(new { ok = false, error = "User requerido" });

        user = user.Trim().ToLower();

        try
        {
            StatusResponse st = await _wa.StatusAsync(user, ct);

            if (st == null)
            {
                return StatusCode(502, new { ok = false, error = "Node no respondió en StatusAsync." });
            }

            // Guardamos estado en BD (AQUÍ ya no existe reason)
            await UpsertInternal(new UpsertSessionDto
            {
                User = user,
                Ready = st.ready,
                ExistsOnNode = st.exists,
                LastReason = null // NO HAY reason en StatusResponse
            }, ct);

            return Ok(new
            {
                ok = true,
                user = st.user,
                ready = st.ready,
                exists = st.exists,
                hasQr = st.hasQr
            });
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, new { ok = false, error = "Cancelado por el cliente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { ok = false, error = ex.Message });
        }
    }


    [HttpPost("sessions/logout")]
    public async Task<IActionResult> Logout([FromBody] EnsureDto body, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body.User)) return BadRequest("User requerido");
        var user = body.User.Trim().ToLower();

        await _wa.LogoutAsync(user, ct);

        await UpsertInternal(new UpsertSessionDto
        {
            User = user,
            Ready = false,
            ExistsOnNode = false,
            LastReason = "logout"
        }, ct);

        return Ok(new { ok = true });
    }

    [HttpPost("upsert")]
    public async Task<IActionResult> Upsert([FromBody] UpsertSessionDto body, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body.User)) return BadRequest("User requerido");
        await UpsertInternal(body, ct);
        return Ok(new { ok = true });
    }

    private async Task UpsertInternal(UpsertSessionDto body, CancellationToken ct)
    {
        Usuario usuario = _authService.GetCurrentUser();
        var user = body.User.Trim().ToLower();
        var entity = await _ctx.WhatsappSessions.FirstOrDefaultAsync(x => x.User == user, ct);
        if (entity is null)
        {
            entity = new WhatsappSession
            {
                User = user,
                Ready = body.Ready,
                ExistsOnNode = body.ExistsOnNode,
                LastReason = body.LastReason,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IdUsuario = usuario.IdUsuario
            };
            _ctx.WhatsappSessions.Add(entity);
        }
        else
        {
            entity.Ready = body.Ready;
            entity.ExistsOnNode = body.ExistsOnNode;
            entity.LastReason = body.LastReason;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        await _ctx.SaveChangesAsync(ct);
    }
}
