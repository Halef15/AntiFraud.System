using AntiFraud.System.BuildingBlocks.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AntiFraud.System.Domain.Entities
{
    [Table("transactions")]
    public sealed class Transaction : EntityBase
    {
        [Required]
        [Column("amount")]
        public decimal Amount { get; private set; }

        [Required]
        [Column("card_holder", TypeName = "varchar(200)")]
        public string CardHolder { get; private set; }

        [Required]
        [Column("card_number", TypeName = "varchar(20)")]
        public string CardNumber { get; private set; }

        [Required]
        [Column("ip_address", TypeName = "varchar(50)")]
        public string IpAddress { get; private set; }

        [Required]
        [Column("location", TypeName = "varchar(10)")]
        public string Location { get; private set; }

        [Required]
        [Column("transaction_date")]
        public DateTimeOffset TransactionDate { get; private set; }

        [Required]
        [Column("status")]
        public TransactionStatus Status { get; private set; }

        // Construtor para o EF Core
        private Transaction() { }

        public Transaction(decimal amount, string cardHolder, string cardNumber, string ipAddress, string location, DateTimeOffset transactionDate)
        {
            Amount = amount;
            CardHolder = cardHolder;
            CardNumber = cardNumber;
            IpAddress = ipAddress;
            Location = location;
            TransactionDate = transactionDate;
            Status = TransactionStatus.Pending; // Toda nova transação começa como pendente
        }

        // Métodos que representam o comportamento do negócio
        public void Approve()
        {
            if (Status == TransactionStatus.Pending || Status == TransactionStatus.Review)
            {
                Status = TransactionStatus.Approved;
                UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

        public void Reject()
        {
            if (Status == TransactionStatus.Pending || Status == TransactionStatus.Review)
            {
                Status = TransactionStatus.Rejected;
                UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

        public void SendToReview()
        {
            if (Status == TransactionStatus.Pending)
            {
                Status = TransactionStatus.Review;
                UpdatedAt = DateTimeOffset.UtcNow;
            }
        }
    }
}