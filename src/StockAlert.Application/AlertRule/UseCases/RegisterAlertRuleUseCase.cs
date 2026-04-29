// Local: src/StockAlert.Application/AlertRule/UseCases/RegisterAlertRuleUseCase.cs

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
        private readonly IStockRepository _stockRepository; 
        private readonly IValidator<RegisterAlertRuleRequest> _validator;
        private readonly ILoggedUserAccessor _loggedUserAccessor;

        public RegisterAlertRuleUseCase(
            IAlertRuleRepository repository,
            IStockRepository stockRepository,
            IValidator<RegisterAlertRuleRequest> validator,
            ILoggedUserAccessor loggedUserAccessor)
        {
            _repository = repository;
            _stockRepository = stockRepository;
            _validator = validator;
            _loggedUserAccessor = loggedUserAccessor;
        }

        public async Task<AlertRuleResponse> Execute(RegisterAlertRuleRequest request)
        {
            // 1. Validação da requisição usando FluentValidation
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new StockAlert.Exception.ValidationException(errors);
            }

            // 2. Obtém o ID do usuário logado
            var userId = _loggedUserAccessor.GetUserId();
            var symbol = request.StockSymbol.ToUpper();

            // 3. Validação de Regra de Negócio: A ação deve estar cadastrada para o usuário
            // Isso garante que a regra de alerta esteja conectada a um monitoramento real
            var stock = await _stockRepository.GetBySymbolAndUserIdAsync(symbol, userId);
            if (stock == null)
            {
                throw new StockAlert.Exception.ValidationException(
                    new List<string> { $"The stock {symbol} must be registered before creating an alert rule." });
            }

            // 4. Mapeamento da Entidade de Domínio
            var alertRule = new Domain.Entities.AlertRule
            {
                UserId = userId,
                StockSymbol = symbol,
                TargetPrice = request.TargetPrice,
                PercentageChange = request.PercentageChange,
                Operator = (Domain.Enums.ComparisonOperator)request.Operator,
                NotifyOnce = request.NotifyOnce,
                IsActive = true
            };

            // 5. Persistência da regra no banco de dados
            await _repository.AddAsync(alertRule);

            // 6. Mapeamento e retorno da resposta
            return new AlertRuleResponse
            {
                Id = alertRule.Id,
                StockSymbol = alertRule.StockSymbol,
                TargetPrice = alertRule.TargetPrice,
                PercentageChange = alertRule.PercentageChange,
                Operator = request.Operator,
                IsActive = alertRule.IsActive
            };
        }
    }
}
