using StockAlert.Domain.Enums;
using StockAlert.Domain.Services;
using AlertRuleEntity = StockAlert.Domain.Entities.AlertRule;

namespace StockAlert.Application.Services
{
    public class AlertConditionChecker : IAlertConditionChecker
    {
        public bool IsConditionMet(decimal currentPrice, decimal? previousPrice, AlertRuleEntity rule)
        {
            if (rule.TargetPrice.HasValue)
                return Compare(currentPrice, rule.TargetPrice.Value, rule.Operator);

            if (rule.PercentageChange.HasValue && previousPrice.HasValue && previousPrice.Value > 0)
            {
                var changePercent = ((currentPrice - previousPrice.Value) / previousPrice.Value) * 100;

                return Compare(changePercent, rule.PercentageChange.Value, rule.Operator);
            }

            return false;
        }

        private static bool Compare(decimal currentValue, decimal targetValue, ComparisonOperator comparisonOperator)
        {
            return comparisonOperator switch
            {
                ComparisonOperator.GreaterThan => currentValue > targetValue,
                ComparisonOperator.LessThan => currentValue < targetValue,
                ComparisonOperator.Equal => currentValue == targetValue,
                ComparisonOperator.GreaterThanOrEqual => currentValue >= targetValue,
                ComparisonOperator.LessThanOrEqual => currentValue <= targetValue,
                _ => false
            };
        }
    }
}