using Microsoft.AspNetCore.Mvc;
using StockAlert.Application.Stock.UseCases;
using StockAlert.Communication.Requests.Stock;
using StockAlert.Communication.Responses.Stock;

namespace StockAlert.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly RegisterStockUseCase _useCase;

        public StocksController(RegisterStockUseCase useCase)
        {
            _useCase = useCase;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(StockResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterStockRequest request)
        {
            var response = await _useCase.Execute(request);

            return CreatedAtAction(nameof(Register), response);
        }
    }
}
