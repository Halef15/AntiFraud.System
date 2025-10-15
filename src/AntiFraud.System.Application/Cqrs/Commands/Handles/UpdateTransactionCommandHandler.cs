using AntiFraud.System.Application.Repositories;
using AntiFraud.System.BuildingBlocks.Responses;
using AntiFraud.System.Domain.Entities; 
using MediatR;

namespace AntiFraud.System.Application.Cqrs.Commands.Handles
{
    public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, Result<Guid>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTransactionCommandHandler(ITransactionRepository transactionRepository, IUnitOfWork unitOfWork)
        {
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
        {
            // Use TransactionViewModel for query, but you need Transaction entity for update
            var transactionViewModel = await _transactionRepository.GetByIdAsync(request.TransactionId, cancellationToken);

            if (transactionViewModel is null)
            {
                return Result<Guid>.Failure("Transação não encontrada.");
            }

            // Fetch the Transaction entity for update
            var transaction = await _transactionRepository.GetByIdForUpdateAsync(request.TransactionId, cancellationToken);

            if (transaction is null)
            {
                return Result<Guid>.Failure("Transação não encontrada para atualização.");
            }

            // Converte o status do comando para o enum
            var targetStatus = (TransactionStatus)request.Status;

            // Atualiza o status diretamente na entidade Transaction
            switch (targetStatus)
            {
                case TransactionStatus.Approved:
                    transaction.Approve();
                    break;
                case TransactionStatus.Rejected:
                    transaction.Reject();
                    break;
                case TransactionStatus.Review:
                    transaction.SendToReview();
                    break;
                default:
                    // Retorna um erro se a transição de estado não for válida (ex: tentar voltar para 'Pending')
                    return Result<Guid>.Failure($"A atualização para o status '{targetStatus}' não é permitida.");
            }

            transaction.UpdatedAt = DateTimeOffset.UtcNow;

            _transactionRepository.Update(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(request.TransactionId);
        }
    }
}