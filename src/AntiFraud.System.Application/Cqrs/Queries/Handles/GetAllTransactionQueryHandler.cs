using AntiFraud.System.Application.Models;
using AntiFraud.System.Application.Repositories;
using AntiFraud.System.BuildingBlocks.Responses;
using MediatR;

namespace AntiFraud.System.Application.Cqrs.Queries.Handles
{
    public class GetAllTransactionQueryHandler : IRequestHandler<GetAllTransactionQuery, Result<IEnumerable<TransactionViewModel>>>
    {
        private readonly ITransactionQueryRepository _queryRepository;

        public GetAllTransactionQueryHandler(ITransactionQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        // O tipo do 'request' aqui também foi simplificado
        public async Task<Result<IEnumerable<TransactionViewModel>>> Handle(GetAllTransactionQuery request, CancellationToken cancellationToken)
        {
            var transactions = await _queryRepository.GetAllAsync(cancellationToken);
            return Result<IEnumerable<TransactionViewModel>>.Success(transactions);
        }
    }
}