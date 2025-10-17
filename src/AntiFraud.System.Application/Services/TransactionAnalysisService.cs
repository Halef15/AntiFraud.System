using AntiFraud.System.Application.Repositories;
using AntiFraud.System.Domain.Entities;
// Remova o using para o DbContext se ele ainda estiver aqui

namespace AntiFraud.System.Application.Services
{
    public class TransactionAnalysisService : ITransactionAnalysisService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBlockedCardRepository _blockedCardRepository; 
        private static readonly HashSet<string> HighRiskCountries = new HashSet<string> { "AF", "IR", "KP" };

        public TransactionAnalysisService(
            ITransactionRepository transactionRepository,
            IBlockedCardRepository blockedCardRepository) // Receba a interface
        {
            _transactionRepository = transactionRepository;
            _blockedCardRepository = blockedCardRepository;
        }

        public async Task AnalyzeTransaction(Transaction transaction)
        {
            if (await _blockedCardRepository.IsCardBlockedAsync(transaction.CardNumber))
            {
                transaction.Reject();
                return;
            }

            // ... resto do método permanece o mesmo ...
            bool needsReview = false;

            if (IsAmountTooHigh(transaction.Amount))
            {
                needsReview = true;
            }

            if (await HasTooManyTransactionsFromIp(transaction.IpAddress))
            {
                needsReview = true;
            }

            if (IsInHighRiskCountry(transaction.Location))
            {
                needsReview = true;
            }

            if (needsReview)
            {
                transaction.SendToReview();
            }
            else
            {
                transaction.Approve();
            }
        }

        private bool IsAmountTooHigh(decimal amount)
        {
            return amount > 5000.00m;
        }

        private async Task<bool> HasTooManyTransactionsFromIp(string ipAddress)
        {
            var oneHourAgo = DateTimeOffset.UtcNow.AddHours(-1);
            var recentTransactions = await _transactionRepository.GetTransactionsByIpAddressSince(ipAddress, oneHourAgo);
            return recentTransactions.Count() >= 3;
        }

        private bool IsInHighRiskCountry(string location)
        {
            return HighRiskCountries.Contains(location);
        }
    }
}