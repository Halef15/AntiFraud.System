using AntiFraud.System.Application.Models;
using AntiFraud.System.BuildingBlocks.Responses;
using MediatR;

namespace AntiFraud.System.Application.Cqrs.Queries
{
    public class GetAllTransactionQuery : IRequest<Result<IEnumerable<TransactionViewModel>>>
    {
    }
}