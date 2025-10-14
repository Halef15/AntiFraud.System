using AntiFraud.System.Application.Models;

namespace AntiFraud.System.Application.Repositories
{
    public interface ITransactionQueryRepository
    {
        Task<IEnumerable<TransactionViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
        
        Task<TransactionViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}