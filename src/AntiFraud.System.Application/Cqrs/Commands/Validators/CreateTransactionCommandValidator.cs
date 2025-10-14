using FluentValidation;

namespace AntiFraud.System.Application.Cqrs.Commands.Validators
{
    public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
    {
        public CreateTransactionCommandValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("O valor da transação deve ser maior que zero.");

            RuleFor(x => x.CardHolder)
                .NotEmpty().WithMessage("O nome do titular do cartão é obrigatório.")
                .MaximumLength(200).WithMessage("O nome do titular do cartão não pode exceder 200 caracteres.");

            RuleFor(x => x.CardNumber)
                .NotEmpty().WithMessage("O número do cartão é obrigatório.")
                .CreditCard().WithMessage("O número do cartão é inválido.");

            RuleFor(x => x.IpAddress)
                .NotEmpty().WithMessage("O endereço IP é obrigatório.")
                .MaximumLength(50).WithMessage("O endereço IP não pode exceder 50 caracteres.");
        }
    }
}