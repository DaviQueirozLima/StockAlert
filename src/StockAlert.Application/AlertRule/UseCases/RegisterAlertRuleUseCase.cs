using FluentValidation;
using StockAlert.Communication.Requests.AlertRule;
using StockAlert.Communication.Responses.AlertRule;
using StockAlert.Domain.Repositories;
using StockAlert.Domain.Security;

namespace StockAlert.Application.AlertRule.UseCases
{
    public class RegisterAlertRuleUseCase
    {
        private readonly IAlertRuleRepository _repository;
        private readonly IValidator<RegisterAlertRuleRequest> _validator;
        private readonly ILoggedUserAccessor _loggedUserAccessor;

        public RegisterAlertRuleUseCase(
            IAlertRuleRepository repository,
            IValidator<RegisterAlertRuleRequest> validator,
            ILoggedUserAccessor loggedUserAccessor)
        {
            _repository = repository;
            _validator = validator;
            _loggedUserAccessor = loggedUserAccessor;
        }
        public async Task<AlertRuleResponse> Execute(RegisterAlertRuleRequest request)
        {
            // 1. Validar a requisição
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new Exception.ValidationException(errors);
            }

            // 2. Obter o ID do usuário logado através do seu novo Accessor
            var userId = _loggedUserAccessor.GetUserId();

            // 3. Mapear DTO para Entidade
            var alertRule = new Domain.Entities.AlertRule
            {
                UserId = userId,
                StockSymbol = request.StockSymbol.ToUpper(),
                TargetPrice = request.TargetPrice,
                PercentageChange = request.PercentageChange,
                // Fazemos o cast do Enum do Communication para o do Domain
                Operator = (Domain.Enums.ComparisonOperator)request.Operator,
                NotifyOnce = request.NotifyOnce,
                IsActive = true
            };

            // 4. Salvar no banco
            await _repository.AddAsync(alertRule);

            // 5. Retornar a resposta
            return new AlertRuleResponse
            {
                Id = alertRule.Id,
                StockSymbol = alertRule.StockSymbol,
                TargetPrice = alertRule.TargetPrice,
                PercentageChange = alertRule.PercentageChange,
                Operator = request.Operator, // Mantemos o do Communication na volta
                IsActive = alertRule.IsActive
            };
        }
    }
}
