using StockAlert.Communication.Enums;
using StockAlert.Communication.Requests.AlertRule;
using StockAlert.Domain.Repositories;
using StockAlert.Domain.Security;
using StockAlert.Exception;

namespace StockAlert.Application.AlertRule.UseCases
{
    public class UpdateAlertRuleUseCase
    {
        private readonly IAlertRuleRepository _repository;
        private readonly ILoggedUserAccessor _loggedUserAccessor; 

        public UpdateAlertRuleUseCase(IAlertRuleRepository repository, ILoggedUserAccessor loggedUserAccessor)
        {
            _repository = repository;
            _loggedUserAccessor = loggedUserAccessor;
        }

        public async Task Execute(Guid ruleId, RegisterAlertRuleRequest request)
        {
            var userId = _loggedUserAccessor.GetUserId();
            var rule = await _repository.GetByIdAsync(ruleId);

            if (rule == null || rule.UserId != userId)
                throw new StockAlert.Exception.NotFoundException("Alert rule not found.");

            rule.TargetPrice = request.TargetPrice ?? rule.TargetPrice;

            rule.Operator = (StockAlert.Domain.Enums.ComparisonOperator)request.Operator;

            rule.LastTriggeredAt = null;

            await _repository.UpdateAsync(rule);
        }


    }
}
