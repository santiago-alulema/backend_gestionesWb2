namespace gestiones_backend.Class
{
    public abstract class AuditableEntity
    {
        public DateTime CreatedAtUtc { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedAtUtc { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
