using StockAlert.Communication.Enums;

namespace StockAlert.Communication.Responses.AlertRule
{
    public class AlertRuleResponse
    {
        public Guid Id { get; set; }
        public string StockSymbol { get; set; } = string.Empty;
        public decimal? TargetPrice { get; set; }
        public decimal? PercentageChange { get; set; }
        public ComparisonOperator Operator { get; set; }
        public bool IsActive { get; set; }
    }
}
