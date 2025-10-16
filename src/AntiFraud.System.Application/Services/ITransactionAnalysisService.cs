using AntiFraud.System.Domain.Entities;

namespace AntiFraud.System.Application.Services
{
    public interface ITransactionAnalysisService
    {
        Task AnalyzeTransaction(Transaction transaction);
    }
}