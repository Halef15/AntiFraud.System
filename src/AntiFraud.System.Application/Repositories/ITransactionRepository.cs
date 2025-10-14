using AntiFraud.System.Domain.Entities;

namespace AntiFraud.System.Application.Repositories
{
    public interface ITransactionRepository : ITransactionQueryRepository
    {
        Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default);
        void Update(Transaction transaction);
        Task<Transaction?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
    }
}