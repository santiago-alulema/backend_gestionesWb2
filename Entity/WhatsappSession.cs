using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestiones_backend.Entity
{
    [Table("WhatsappSessions")]
    public class WhatsappSession
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(120)]
        public string User { get; set; } = default!;

        public bool Ready { get; set; }
        public bool ExistsOnNode { get; set; }

        [MaxLength(300)]
        public string? LastReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
