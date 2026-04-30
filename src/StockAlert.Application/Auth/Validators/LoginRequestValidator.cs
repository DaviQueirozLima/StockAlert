using FluentValidation;
using StockAlert.Communication.Requests.Auth;

namespace StockAlert.Application.Auth.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator() 
        {
         RuleFor(x => x.IdToken)
        .NotEmpty()
        .WithMessage("IdToken is required.")
        .Must(token => !string.IsNullOrWhiteSpace(token))
        .WithMessage("IdToken cannot be empty.");
        }

    }
}
