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
        private readonly GetAlertRulesUseCase _getUseCase;

        public AlertRulesController(RegisterAlertRuleUseCase useCase, UpdateAlertRuleUseCase updateUseCase, DeleteAlertRuleUseCase deleteUseCase, GetAlertRulesUseCase getUseCase)
        {
            _useCase = useCase;
            _updateUseCase = updateUseCase;
            _deleteUseCase = deleteUseCase;
            _getUseCase = getUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _getUseCase.Execute();
            return Ok(response);
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
