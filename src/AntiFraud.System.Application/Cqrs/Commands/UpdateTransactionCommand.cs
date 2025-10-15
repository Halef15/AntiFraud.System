using AntiFraud.System.BuildingBlocks.Responses;
using AntiFraud.System.BulidingBlocks.Cqrs;
using MediatR;
using System.Text.Json.Serialization;
using System.Transactions;
using AntiFraud.System.Domain.Entities;
using System.ComponentModel;


namespace AntiFraud.System.Application.Cqrs.Commands
{
    public class UpdateTransactionCommand : IRequest<Result<Guid>>, ICommand
    {
        [property: JsonIgnore]
        public Guid TransactionId { get; set; }

        [DefaultValue(Domain.Entities.TransactionStatus.Approved)]
        public Domain.Entities.TransactionStatus Status { get; set; } = Domain.Entities.TransactionStatus.Approved;
    }
}