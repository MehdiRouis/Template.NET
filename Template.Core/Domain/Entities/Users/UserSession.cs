using System.ComponentModel.DataAnnotations.Schema;

namespace Template.Core.Domain.Entities.Users
{
    [Table("users_sessions")]
    public class UserSession : BaseEntity
    {
        [Column("user_id")]
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [Column("session_token")]
        public string SessionToken { get; set; } = string.Empty;

        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        public virtual User? User { get; set; }
    }
}
