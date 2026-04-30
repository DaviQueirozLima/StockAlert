using StockAlert.Domain.Repositories;
using StockAlert.Domain.Security;

namespace StockAlert.Application.AlertRule.UseCases;

public class DeleteAlertRuleUseCase
{
    private readonly IAlertRuleRepository _repository;
    private readonly ILoggedUserAccessor _loggedUserAccessor;

    public DeleteAlertRuleUseCase(IAlertRuleRepository repository, ILoggedUserAccessor loggedUserAccessor)
    {
        _repository = repository;
        _loggedUserAccessor = loggedUserAccessor;
    }

    public async Task Execute(Guid ruleId)
    {
        // ID do usuário logado 
        var userId = _loggedUserAccessor.GetUserId();

        var rule = await _repository.GetByIdAsync(ruleId);

        //  ID da regra com o ID que seu Accessor retornou
        if (rule == null || rule.UserId != userId)
        {
            throw new StockAlert.Exception.NotFoundException("Alerta não encontrado.");
        }

        await _repository.DeleteAsync(ruleId);
    }
}
