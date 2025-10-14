using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AntiFraud.System.BuildingBlocks.Entities
{
    public abstract class EntityBase
    {
        [Key]
        [Required]
        [Column("id")]
        public Guid Id { get; protected set; }

        [Required]
        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; private set; }

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; protected set; }

        protected EntityBase()
        {
            Id = Guid.NewGuid(); // Usaremos o NewGuid() padrão por enquanto.
            CreatedAt = DateTimeOffset.UtcNow;
            UpdatedAt = null;
        }
    }
}