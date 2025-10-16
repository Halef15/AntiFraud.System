using AntiFraud.System.Domain.Entities;
using MediatR;

namespace AntiFraud.System.Application.Cqrs.Notifications
{
    public sealed class AntiFraudRegisteredNotification : INotification
    {
        #region Public Properties
        public required Guid TransactionId { get; init; }
        public required TransactionStatus Status{ get; init; }
        public required DateTimeOffset CreatedAt { get; init; }
        public required DateTimeOffset UpdatedAt { get; init; }

        #endregion
    }
}