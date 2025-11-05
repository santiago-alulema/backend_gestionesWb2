namespace gestiones_backend.Entity
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public DateTime EventDateUtc { get; set; }
        public string? UserName { get; set; }
        public string Action { get; set; } = default!;     // Insert | Update | Delete
        public string Table { get; set; } = default!;
        public string? KeyJson { get; set; }               // { "Id": 123 }
        public string? BeforeJson { get; set; }            // valores originales (solo props cambiadas)
        public string? AfterJson { get; set; }
    }
}
