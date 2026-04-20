using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockAlert.Application.AlertRule.UseCases;
using StockAlert.Communication.Requests.AlertRule;
using StockAlert.Communication.Responses.AlertRule;

namespace StockAlert.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AlertRulesController : ControllerBase
    {
        private readonly RegisterAlertRuleUseCase _useCase;

        public AlertRulesController(RegisterAlertRuleUseCase useCase)
        {
            _useCase = useCase;
        }

        [HttpPost]
        [ProducesResponseType(typeof(AlertRuleResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Register([FromBody] RegisterAlertRuleRequest request)
        {
            var response = await _useCase.Execute(request);

            return CreatedAtAction(nameof(Register), response);
        }
    }
}
