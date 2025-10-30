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

    public WhatsappSessionsController(DataContext ctx, IWhatsappNodeClient wa)
    {
        _ctx = ctx;
        _wa = wa;
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
        var items = await _ctx.WhatsappSessions
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost("sessions/ensure")]
    public async Task<IActionResult> Ensure([FromBody] EnsureDto body, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body.User)) return BadRequest("User requerido");
        var user = body.User.Trim().ToLower();

        var ensure = await _wa.EnsureAsync(user, ct);

        await UpsertInternal(new UpsertSessionDto
        {
            User = user,
            Ready = ensure.ready,
            ExistsOnNode = ensure.exists,
            LastReason = ensure.reason
        }, ct);

        return Ok(new
        {
            ok = true,
            user = ensure.user,
            ready = ensure.ready,
            qrDataUrl = ensure.qrDataUrl,
            reason = ensure.reason
        });
    }

    [HttpGet("sessions/status")]
    public async Task<IActionResult> Status([FromQuery] string user, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(user)) return BadRequest("User requerido");
        user = user.Trim().ToLower();

        var st = await _wa.StatusAsync(user, ct);

        await UpsertInternal(new UpsertSessionDto
        {
            User = user,
            Ready = st.ready,
            ExistsOnNode = st.exists,
            LastReason = null
        }, ct);

        return Ok(new { ok = true, user = st.user, ready = st.ready, hasQr = st.hasQr, exists = st.exists });
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
                UpdatedAt = DateTime.UtcNow
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
