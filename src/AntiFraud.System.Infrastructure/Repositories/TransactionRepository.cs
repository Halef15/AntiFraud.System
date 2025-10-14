using AntiFraud.System.Application.Models;
using AntiFraud.System.Application.Repositories;
using AntiFraud.System.Domain.Entities;
using AntiFraud.System.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AntiFraud.System.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
        {
            await _context.Transactions.AddAsync(transaction, cancellationToken);
        }

        public void Update(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
        }

        public async Task<Transaction?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Transactions.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IEnumerable<TransactionViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Transactions
                .AsNoTracking()
                .Select(t => new TransactionViewModel
                {
                    TransactionId = t.Id,
                    Status = t.Status.ToString(),
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<TransactionViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Transactions
                .AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => new TransactionViewModel
                {
                    TransactionId = t.Id,
                    Status = t.Status.ToString(),
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}