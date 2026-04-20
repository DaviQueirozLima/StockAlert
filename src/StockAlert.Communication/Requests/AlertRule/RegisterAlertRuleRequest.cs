using StockAlert.Communication.Enums;

namespace StockAlert.Communication.Requests.AlertRule
{
    public class RegisterAlertRuleRequest
    {
        public string StockSymbol { get; set; } = string.Empty;
        public decimal? TargetPrice { get; set; }
        public decimal? PercentageChange { get; set; }
        public ComparisonOperator Operator { get; set; }
        public NotificationChannel? PreferredChannel { get; set; }
        public bool NotifyOnce { get; set; }
    }
}
