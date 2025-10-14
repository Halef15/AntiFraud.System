using AntiFraud.System.BuildingBlocks.Responses;
using AntiFraud.System.BulidingBlocks.Cqrs;
using MediatR;

namespace AntiFraud.System.Application.Cqrs.Commands
{
    public class CreateTransactionCommand : IRequest<Result<Guid>>, ICommand
    {
        public decimal Amount { get; set; }
        public string CardHolder { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTimeOffset TransactionDate { get; set; }
    }
}