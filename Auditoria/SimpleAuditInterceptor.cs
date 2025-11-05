using gestiones_backend.Class; // AuditableEntity
using gestiones_backend.Entity; // AuditLog
using gestiones_backend.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace gestiones_backend.Auditoria
{
    public class SimpleAuditInterceptor : SaveChangesInterceptor
    {
        private readonly IUserContext _user;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public SimpleAuditInterceptor(IUserContext user) => _user = user;

        // === SÍNCRONO (cubre SaveChanges()) ===
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            var ctx = eventData.Context;
            if (ctx is null) return base.SavingChanges(eventData, result);

            ctx.ChangeTracker.DetectChanges();
            BuildAndAttachAuditLogsSync(ctx, _user.UserName ?? "anonymous");

            return base.SavingChanges(eventData, result);
        }

        // === ASÍNCRONO (cubre SaveChangesAsync()) ===
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken ct = default)
        {
            var ctx = eventData.Context;
            if (ctx is null) return await base.SavingChangesAsync(eventData, result, ct);

            ctx.ChangeTracker.DetectChanges();
            await BuildAndAttachAuditLogsAsync(ctx, _user.UserName ?? "anonymous", ct);

            return await base.SavingChangesAsync(eventData, result, ct);
        }

        // --------- SYNC ----------
        private void BuildAndAttachAuditLogsSync(DbContext ctx, string userName)
        {
            var now = DateTime.UtcNow;

            // Setear Created/Modified
            foreach (var entry in ctx.ChangeTracker.Entries().Where(e =>
                         e.Entity is AuditableEntity &&
                         (e.State == EntityState.Added || e.State == EntityState.Modified)))
            {
                var aud = (AuditableEntity)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    aud.CreatedAtUtc = now;
                    aud.CreatedBy = userName;
                }
                aud.ModifiedAtUtc = now;
                aud.ModifiedBy = userName;
            }

            var entries = ctx.ChangeTracker.Entries()
                .Where(e => e.Entity is not AuditLog &&
                            (e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
                .ToList();

            if (entries.Count == 0) return;

            var logs = new List<AuditLog>(entries.Count);

            foreach (var e in entries)
            {
                Dictionary<string, object?>? dbBefore = null;
                if (e.State == EntityState.Modified)
                {
                    // SYNC: usar GetDatabaseValues()
                    var dbValues = e.GetDatabaseValues();
                    if (dbValues != null)
                    {
                        dbBefore = new Dictionary<string, object?>();
                        foreach (var p in e.Properties.Where(p => !p.Metadata.IsPrimaryKey()))
                        {
                            dbBefore[p.Metadata.Name] = dbValues[p.Metadata.Name];
                        }
                    }
                }

                var (action, before, after) = BuildDiffWithDbFallback(e, dbBefore);

                if (action == "Update" &&
                    (before == null || before.Count == 0) &&
                    (after == null || after.Count == 0))
                {
                    continue;
                }

                var key = ExtractKey(e);

                logs.Add(new AuditLog
                {
                    Id = Guid.NewGuid(),
                    EventDateUtc = now,
                    UserName = userName,
                    Action = action,
                    Table = e.Metadata.GetTableName() ?? e.Entity.GetType().Name,
                    KeyJson = key is null ? null : JsonSerializer.Serialize(key, _json),
                    BeforeJson = before is null ? null : JsonSerializer.Serialize(before, _json),
                    AfterJson = after is null ? null : JsonSerializer.Serialize(after, _json)
                });
            }

            if (logs.Count > 0)
                ctx.Set<AuditLog>().AddRange(logs);
        }

        // --------- ASYNC ----------
        private async Task BuildAndAttachAuditLogsAsync(DbContext ctx, string userName, CancellationToken ct)
        {
            var now = DateTime.UtcNow;

            // Setear Created/Modified
            foreach (var entry in ctx.ChangeTracker.Entries().Where(e =>
                         e.Entity is AuditableEntity &&
                         (e.State == EntityState.Added || e.State == EntityState.Modified)))
            {
                var aud = (AuditableEntity)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    aud.CreatedAtUtc = now;
                    aud.CreatedBy = userName;
                }
                aud.ModifiedAtUtc = now;
                aud.ModifiedBy = userName;
            }

            var entries = ctx.ChangeTracker.Entries()
                .Where(e => e.Entity is not AuditLog &&
                            (e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
                .ToList();

            if (entries.Count == 0) return;

            var logs = new List<AuditLog>(entries.Count);

            foreach (var e in entries)
            {
                Dictionary<string, object?>? dbBefore = null;
                if (e.State == EntityState.Modified)
                {
                    // ASYNC: usar GetDatabaseValuesAsync()
                    var dbValues = await e.GetDatabaseValuesAsync(ct);
                    if (dbValues != null)
                    {
                        dbBefore = new Dictionary<string, object?>();
                        foreach (var p in e.Properties.Where(p => !p.Metadata.IsPrimaryKey()))
                        {
                            dbBefore[p.Metadata.Name] = dbValues[p.Metadata.Name];
                        }
                    }
                }

                var (action, before, after) = BuildDiffWithDbFallback(e, dbBefore);

                if (action == "Update" &&
                    (before == null || before.Count == 0) &&
                    (after == null || after.Count == 0))
                {
                    continue;
                }

                var key = ExtractKey(e);

                logs.Add(new AuditLog
                {
                    Id = Guid.NewGuid(),
                    EventDateUtc = now,
                    UserName = userName,
                    Action = action,
                    Table = e.Metadata.GetTableName() ?? e.Entity.GetType().Name,
                    KeyJson = key is null ? null : JsonSerializer.Serialize(key, _json),
                    BeforeJson = before is null ? null : JsonSerializer.Serialize(before, _json),
                    AfterJson = after is null ? null : JsonSerializer.Serialize(after, _json)
                });
            }

            if (logs.Count > 0)
                ctx.Set<AuditLog>().AddRange(logs);
        }

        // ---------- helpers compartidos ----------
        private static (string action, Dictionary<string, object?>? before, Dictionary<string, object?>? after)
    BuildDiffWithDbFallback(EntityEntry entry, Dictionary<string, object?>? dbBefore)
        {
            var before = new Dictionary<string, object?>();
            var after = new Dictionary<string, object?>();

            switch (entry.State)
            {
                case EntityState.Added:
                    foreach (var p in entry.Properties.Where(p => !p.Metadata.IsPrimaryKey()))
                        after[p.Metadata.Name] = p.CurrentValue;
                    return ("Insert", null, after);

                case EntityState.Deleted:
                    foreach (var p in entry.Properties.Where(p => !p.Metadata.IsPrimaryKey()))
                        before[p.Metadata.Name] = p.OriginalValue;
                    return ("Delete", before, null);

                case EntityState.Modified:
                    foreach (var p in entry.Properties.Where(p => !p.Metadata.IsPrimaryKey()))
                    {
                        var name = p.Metadata.Name;

                        // 👇 PRIORIDAD: valores de la BD si los tenemos; si no, OriginalValue.
                        var beforeVal = (dbBefore != null && dbBefore.TryGetValue(name, out var v))
                                        ? v
                                        : p.OriginalValue;

                        var afterVal = p.CurrentValue;

                        if (!Equals(beforeVal, afterVal))
                        {
                            before[name] = beforeVal;
                            after[name] = afterVal;
                        }
                    }

                    if (before.Count == 0 && after.Count == 0)
                        return ("Update", null, null);

                    return ("Update", before, after);

                default:
                    return ("None", null, null);
            }
        }

        private static Dictionary<string, object?>? ExtractKey(EntityEntry entry)
        {
            var pk = entry.Metadata.FindPrimaryKey();
            if (pk is null || pk.Properties.Count == 0) return null;

            var dict = new Dictionary<string, object?>(pk.Properties.Count);
            foreach (var p in pk.Properties)
            {
                var prop = entry.Property(p.Name);
                dict[p.Name] = prop.CurrentValue ?? prop.OriginalValue;
            }
            return dict;
        }
    }
}
