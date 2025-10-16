using AntiFraud.System.Application.Repositories;
using AntiFraud.System.Application.Services; 
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
        private readonly ILogger<CreateTransactionCommandHandler> _logger;
        private readonly ITransactionAnalysisService _transactionAnalysisService; 

        public CreateTransactionCommandHandler(
            ITransactionRepository transactionRepository,
            IUnitOfWork unitOfWork,
            ILogger<CreateTransactionCommandHandler> logger,
            ITransactionAnalysisService transactionAnalysisService) 
        {
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _transactionAnalysisService = transactionAnalysisService; 
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

            try
            {
                // 1. Analisa a transação para definir o status
                await _transactionAnalysisService.AnalyzeTransaction(transaction);

                // 2. Salva a transação com o status já definido
                await _transactionRepository.AddAsync(transaction, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Transaction {TransactionId} created with status {Status}.", transaction.Id, transaction.Status);

                return Result<Guid>.Success(transaction.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving transaction {TransactionId} to the database. Initiating fallback.", transaction.Id);
                return Result<Guid>.Failure($"An error occurred while processing the transaction. Fallback was initiated. Error: {ex.Message}");
            }
        }
    }
}