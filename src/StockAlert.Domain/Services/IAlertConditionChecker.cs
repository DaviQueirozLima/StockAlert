using AlertRuleEntity = StockAlert.Domain.Entities.AlertRule;

namespace StockAlert.Domain.Services
{
    public interface IAlertConditionChecker
    {
        bool IsConditionMet(decimal currentPrice, decimal? previousPrice, AlertRuleEntity rule);
    }
}
