using System.ComponentModel.DataAnnotations.Schema;

namespace Template.Core.Domain.Entities.Users
{
    [Table("users_sessions")]
    public class UserSession : BaseEntity
    {
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("refresh_token_hash")]
        public string RefreshTokenHash { get; set; } = string.Empty;

        [Column("access_expires_at")]
        public DateTime AccessExpiresAt { get; set; }

        [Column("refresh_expires_at")]
        public DateTime RefreshExpiresAt { get; set; }

        [Column("revoked_at")]
        public DateTime? RevokedAt { get; set; }

        public virtual User? User { get; set; }
    }
}
