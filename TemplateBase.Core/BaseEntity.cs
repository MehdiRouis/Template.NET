using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TemplateBase.Core
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    public abstract class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        [Column("updated_at")]
        public DateTime UpdatedAtOnUtc { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        [Column("force_delete")]
        public bool ForceDelete { get; set; }

    }
}
