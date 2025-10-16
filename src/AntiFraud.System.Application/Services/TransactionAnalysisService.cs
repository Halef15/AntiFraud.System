using AntiFraud.System.Application.Repositories;
using AntiFraud.System.Domain.Entities;

namespace AntiFraud.System.Application.Services
{
    public class TransactionAnalysisService : ITransactionAnalysisService
    {
        private readonly ITransactionRepository _transactionRepository;
        private static readonly HashSet<string> HighRiskCountries = new HashSet<string> { "AF", "IR", "KP" };
        private static readonly HashSet<string> CardNumberBlocklist = new HashSet<string> { "CARD_BLOCKED_NUMBER_1", "CARD_BLOCKED_NUMBER_2" };

        public TransactionAnalysisService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task AnalyzeTransaction(Transaction transaction)
        {
            if (IsCardNumberInBlocklist(transaction.CardNumber))
            {
                transaction.Reject();
                return;
            }

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

        private bool IsCardNumberInBlocklist(string cardNumber)
        {
            return CardNumberBlocklist.Contains(cardNumber);
        }

        private bool IsAmountTooHigh(decimal amount)
        {
            return amount > 5000.00m;
        }

        private async Task<bool> HasTooManyTransactionsFromIp(string ipAddress)
        {
            var oneHourAgo = DateTimeOffset.UtcNow.AddHours(-1);
            var recentTransactions = await _transactionRepository.GetTransactionsByIpAddressSince(ipAddress, oneHourAgo);
            return recentTransactions.Count() > 3;
        }

        private bool IsInHighRiskCountry(string location)
        {
            return HighRiskCountries.Contains(location);
        }
    }
}