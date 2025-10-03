using System.ComponentModel.DataAnnotations;

namespace gestiones_backend.Entity
{
    public class EmailSmtpConfig
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string Key { get; set; } = "default";

        [Required, MaxLength(200)]
        public string SmtpHost { get; set; } = default!;

        public int SmtpPort { get; set; } = 587;

        public bool UseSsl { get; set; } = false;

        public bool UseStartTls { get; set; } = true;

        [Required, MaxLength(200)]
        public string Username { get; set; } = default!;

        [Required, MaxLength(500)]
        public string Password { get; set; } = default!;

        public bool IsActive { get; set; } = true;

        public int TimeoutSeconds { get; set; } = 30;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
