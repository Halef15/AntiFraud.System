using FluentValidation;

namespace AntiFraud.System.Application.Cqrs.Commands.Validators
{
    public class BlockCardCommandValidator : AbstractValidator<BlockCardCommand>
    {
        public BlockCardCommandValidator()
        {
            RuleFor(c => c.CardNumber)
                .NotEmpty()
                .WithMessage("O número do cartão é obrigatório.")
                .CreditCard()
                .WithMessage("O número do cartão não é válido.");
        }
    }
}