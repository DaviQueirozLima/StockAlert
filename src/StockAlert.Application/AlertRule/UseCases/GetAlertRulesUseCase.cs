using StockAlert.Communication.Responses.AlertRule;
using StockAlert.Domain.Repositories;
using StockAlert.Domain.Security;

namespace StockAlert.Application.AlertRule.UseCases
{
    public class GetAlertRulesUseCase
    {
        private readonly IAlertRuleRepository _repository;
        private readonly ILoggedUserAccessor _loggedUserAccessor;

        public GetAlertRulesUseCase(
            IAlertRuleRepository repository,
            ILoggedUserAccessor loggedUserAccessor)
        {
            _repository = repository;
            _loggedUserAccessor = loggedUserAccessor;
        }

        public async Task<IEnumerable<AlertRuleResponse>> Execute()
        {
            var userId = _loggedUserAccessor.GetUserId();

            var alerts = await _repository.GetByUserIdAsync(userId);

            return alerts.Select(alert => new AlertRuleResponse
            {
                Id = alert.Id,
                StockSymbol = alert.StockSymbol,
                TargetPrice = alert.TargetPrice,
                PercentageChange = alert.PercentageChange,
                Operator = (StockAlert.Communication.Enums.ComparisonOperator)alert.Operator,
                IsActive = alert.IsActive
            });
        }
    }
}
