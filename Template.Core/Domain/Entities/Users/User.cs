using System.ComponentModel.DataAnnotations.Schema;

namespace Template.Core.Domain.Entities.Users
{
    [Table("users")]
    public class User : BaseEntity
    {
        [Column("full_name")]
        public required string FullName { get; set; }

        [Column("email")]
        public required string Email { get; set; }

        [Column("phone")]
        public required string Phone { get; set; }

        [Column("is_active")]
        public required bool IsActive { get; set; }

        public virtual IList<UserPassword>? UserPasswords { get; set; }

        public virtual IList<UserSession>? UserSessions { get; set; }
    }
}
