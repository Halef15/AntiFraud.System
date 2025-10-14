using AntiFraud.System.Application.Models;
using AntiFraud.System.BuildingBlocks.Responses; 
using MediatR;

namespace AntiFraud.System.Application.Cqrs.Queries
{
    public class GetTransactionQuery : IRequest<Result<TransactionViewModel?>>
    {
        public Guid TransactionId { get; set; }

        public GetTransactionQuery(Guid transactionId)
        {
            TransactionId = transactionId;
        }
    }
}