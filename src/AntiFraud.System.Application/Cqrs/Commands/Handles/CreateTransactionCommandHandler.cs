using AntiFraud.System.Application.Repositories;
using AntiFraud.System.BuildingBlocks.Responses;
using AntiFraud.System.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AntiFraud.System.Application.Cqrs.Commands.Handles
{
    public class CreateTransactionCommandHandler :
        IRequestHandler<CreateTransactionCommand, Result<Guid>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        // 1. Adicione um campo para o logger
        private readonly ILogger<CreateTransactionCommandHandler> _logger;

        // 2. Injete o ILogger no construtor
        public CreateTransactionCommandHandler(
            ITransactionRepository transactionRepository,
            IUnitOfWork unitOfWork,
            ILogger<CreateTransactionCommandHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = new Transaction(
                request.Amount,
                request.CardHolder,
                request.CardNumber,
                request.IpAddress,
                request.Location,
                request.TransactionDate);

            // 3. Adicione o bloco try-catch para implementar o fallback
            try
            {
                await _transactionRepository.AddAsync(transaction, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Transaction {TransactionId} created successfully.", transaction.Id);

                return Result<Guid>.Success(transaction.Id);
            }
            catch (Exception ex)
            {
                // Bloco 'catch' simula o fallback
                _logger.LogError(ex, "Error saving transaction {TransactionId} to the database. Initiating fallback.", transaction.Id);

                // Retorna um erro, informando que a operação principal falhou.
                return Result<Guid>.Failure($"An error occurred while processing the transaction. Fallback was initiated. Error: {ex.Message}");
            }
        }
    }
}