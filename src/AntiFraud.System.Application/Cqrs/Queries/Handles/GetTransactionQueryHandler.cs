using AntiFraud.System.Application.Models;
using AntiFraud.System.Application.Repositories;
using AntiFraud.System.BuildingBlocks.Responses;
using MediatR;

namespace AntiFraud.System.Application.Cqrs.Queries.Handles
{
    public class GetTransactionQueryHandler : 
        IRequestHandler<GetTransactionQuery, Result<TransactionViewModel?>>
    {
        private readonly ITransactionQueryRepository _queryRepository;

        public GetTransactionQueryHandler(ITransactionQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<Result<TransactionViewModel?>> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
        {
            var transaction = await _queryRepository.GetByIdAsync(request.TransactionId, cancellationToken);

            if (transaction is null)
            {
                return Result<TransactionViewModel?>.Failure("Transação não encontrada.");
            }

            return Result<TransactionViewModel?>.Success(transaction);
        }
    }
}