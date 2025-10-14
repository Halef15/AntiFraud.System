namespace AntiFraud.System.Application.Models
{
    public class TransactionViewModel
    {
        public Guid TransactionId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}