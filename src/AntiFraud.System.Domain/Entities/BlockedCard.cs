using AntiFraud.System.BuildingBlocks.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AntiFraud.System.Domain.Entities
{
    [Table("blocked_cards")]
    public sealed class BlockedCard : EntityBase
    {
        [Required]
        [Column("card_number", TypeName = "varchar(20)")]
        public string CardNumber { get; private set; }

        [Column("reason", TypeName = "varchar(255)")]
        public string? Reason { get; private set; } // Motivo do bloqueio (opcional)

        private BlockedCard() { } // Construtor para o EF

        public BlockedCard(string cardNumber, string? reason)
        {
            CardNumber = cardNumber;
            Reason = reason;
        }
    }
}