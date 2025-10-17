using AntiFraud.System.Application.Repositories;
using AntiFraud.System.Domain.Entities;
using AntiFraud.System.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AntiFraud.System.Infrastructure.Repositories
{
    public class BlockedCardRepository : IBlockedCardRepository
    {
        private readonly ApplicationDbContext _context;

        public BlockedCardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BlockedCard blockedCard, CancellationToken cancellationToken)
        {
            await _context.BlockedCards.AddAsync(blockedCard, cancellationToken);
        }

        public async Task<bool> IsCardBlockedAsync(string cardNumber)
        {
            return await _context.BlockedCards.AnyAsync(c => c.CardNumber == cardNumber);
        }
    }
}