using AntiFraud.System.Domain.Entities;

namespace AntiFraud.System.Application.Repositories
{
    public interface IBlockedCardRepository
    {
        Task<bool> IsCardBlockedAsync(string cardNumber);
        Task AddAsync(BlockedCard blockedCard, CancellationToken cancellationToken); // Adicionar este método
    }
}