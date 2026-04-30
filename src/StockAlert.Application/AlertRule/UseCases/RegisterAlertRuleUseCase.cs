using FluentValidation;
using StockAlert.Communication.Requests.AlertRule;
using StockAlert.Communication.Responses.AlertRule;
using StockAlert.Domain.Repositories;
using StockAlert.Domain.Security;
using Microsoft.Extensions.Logging;

namespace StockAlert.Application.AlertRule.UseCases
{
    public class RegisterAlertRuleUseCase
    {
        private readonly IAlertRuleRepository _repository;
        private readonly IStockRepository _stockRepository;
        private readonly IValidator<RegisterAlertRuleRequest> _validator;
        private readonly ILoggedUserAccessor _loggedUserAccessor;
        private readonly ILogger<RegisterAlertRuleUseCase> _logger; 

        public RegisterAlertRuleUseCase(
            IAlertRuleRepository repository,
            IStockRepository stockRepository,
            IValidator<RegisterAlertRuleRequest> validator,
            ILoggedUserAccessor loggedUserAccessor,
            ILogger<RegisterAlertRuleUseCase> logger) 
        {
            _repository = repository;
            _stockRepository = stockRepository;
            _validator = validator;
            _loggedUserAccessor = loggedUserAccessor;
            _logger = logger;
        }

        public async Task<AlertRuleResponse> Execute(RegisterAlertRuleRequest request)
        {
            _logger.LogInformation("Attempting to register alert rule for stock {Symbol}", request.StockSymbol);

            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for alert rule on stock {Symbol}", request.StockSymbol);
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new StockAlert.Exception.ValidationException(errors);
            }

            var userId = _loggedUserAccessor.GetUserId();
            var symbol = request.StockSymbol.ToUpper();

            var stock = await _stockRepository.GetBySymbolAndUserIdAsync(symbol, userId);
            if (stock == null)
            {
                _logger.LogWarning("User {UserId} tried to create alert for unregistered stock {Symbol}", userId, symbol);
                throw new StockAlert.Exception.ValidationException(
                    new List<string> { $"The stock {symbol} must be registered before creating an alert rule." });
            }

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

            await _repository.AddAsync(alertRule);

            _logger.LogInformation("Alert rule {AlertId} successfully registered for stock {Symbol} by user {UserId}",
                alertRule.Id, symbol, userId);

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
