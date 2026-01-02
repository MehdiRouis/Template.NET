using System.ComponentModel.DataAnnotations.Schema;

namespace Template.Core.Domain.Entities.Users
{
    [Table("users_passwords")]
    public class UserPassword : BaseEntity
    {
        [Column("user_id")]
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        public virtual User? User { get; set; }
    }
}
