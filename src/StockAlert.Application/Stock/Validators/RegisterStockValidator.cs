using FluentValidation;

namespace StockAlert.Application.Stock.Validators
{
    public class RegisterStockValidator : AbstractValidator<StockAlert.Communication.Requests.Stock.RegisterStockRequest>
    {
        public RegisterStockValidator() 
        { 
            RuleFor(x => x.Symbol)
                .NotEmpty().WithMessage("The stock symbol is required.")
                .MinimumLength(3).WithMessage("The stock symbol must be at least 3 characters long.")
                .MaximumLength(10).WithMessage("The stock symbol must be at most 10 characters long.")
                .Must(symbol => !string.IsNullOrWhiteSpace(symbol)).WithMessage("Stock symbol cannot be empty.")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("The symbol must contain only letters and numbers.");

        }
    }
}
