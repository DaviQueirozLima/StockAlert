using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockAlert.Application.AlertRule.UseCases;
using StockAlert.Communication.Requests.AlertRule;

namespace StockAlert.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AlertRulesController : ControllerBase
    {
        private readonly RegisterAlertRuleUseCase _useCase;
        private readonly UpdateAlertRuleUseCase _updateUseCase;
        private readonly DeleteAlertRuleUseCase _deleteUseCase;

        public AlertRulesController(RegisterAlertRuleUseCase useCase, UpdateAlertRuleUseCase updateUseCase, DeleteAlertRuleUseCase deleteUseCase)
        {
            _useCase = useCase;
            _updateUseCase = updateUseCase;
            _deleteUseCase = deleteUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterAlertRuleRequest request)
        {
            var response = await _useCase.Execute(request);
            return CreatedAtAction(nameof(Register), response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] RegisterAlertRuleRequest request)
        {
            await _updateUseCase.Execute(id, request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _deleteUseCase.Execute(id);
            return NoContent();
        }

    }
}
