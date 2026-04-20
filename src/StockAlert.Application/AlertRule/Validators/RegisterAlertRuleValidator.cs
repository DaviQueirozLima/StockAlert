using FluentValidation;
using StockAlert.Communication.Requests.AlertRule;

namespace StockAlert.Application.AlertRule.Validator
{
    public class RegisterAlertRuleValidator : AbstractValidator<RegisterAlertRuleRequest>
    {
        public RegisterAlertRuleValidator()
        {
            RuleFor(x => x.StockSymbol)
               .NotEmpty().WithMessage("The stock symbol is required.")
               .MaximumLength(10).WithMessage("The stock symbol must not exceed 10 characters.");

            // Regra customizada: Deve ter ou TargetPrice ou PercentageChange
            RuleFor(x => x)
                .Must(x => x.TargetPrice.HasValue || x.PercentageChange.HasValue)
                .WithMessage("Either a target price or a percentage change must be provided.");

            RuleFor(x => x.TargetPrice)
                .GreaterThan(0).When(x => x.TargetPrice.HasValue)
                .WithMessage("The target price must be greater than zero.");

            RuleFor(x => x.PercentageChange)
                .ExclusiveBetween(-100, 1000).When(x => x.PercentageChange.HasValue)
                .WithMessage("The percentage change must be between -100 and 1000.");

            RuleFor(x => x.Operator)
                .IsInEnum().WithMessage("A valid comparison operator must be selected.");
        }
    }
}
