using AntiFraud.System.Application.Repositories;
using AntiFraud.System.BuildingBlocks.Responses;
using AntiFraud.System.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AntiFraud.System.Application.Cqrs.Commands.Handles
{
    public class BlockCardCommandHandler : IRequestHandler<BlockCardCommand, Result<Guid>>
    {
        private readonly IBlockedCardRepository _blockedCardRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BlockCardCommandHandler> _logger;

        public BlockCardCommandHandler(
            IBlockedCardRepository blockedCardRepository,
            IUnitOfWork unitOfWork,
            ILogger<BlockCardCommandHandler> logger)
        {
            _blockedCardRepository = blockedCardRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(BlockCardCommand request, CancellationToken cancellationToken)
        {
            var cardAlreadyExists = await _blockedCardRepository.IsCardBlockedAsync(request.CardNumber);
            if (cardAlreadyExists)
            {
                return Result<Guid>.Failure("Este cartão já está bloqueado.");
            }

            var blockedCard = new BlockedCard(request.CardNumber, request.Reason);

            await _blockedCardRepository.AddAsync(blockedCard, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Card {CardNumber} has been blocked with Id {Id}", blockedCard.CardNumber, blockedCard.Id);

            return Result<Guid>.Success(blockedCard.Id);
        }
    }
}