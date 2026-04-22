using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockAlert.Domain.Services;

namespace StockAlert.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestBrapiController : ControllerBase
    {
        private readonly IBrapiService _brapiService;

        public TestBrapiController(IBrapiService brapiService)
        {
            _brapiService = brapiService;
        }

        [HttpGet("stock-quote/{symbol}")]
        public async Task<IActionResult> GetStockQuote(string symbol)
        {
            var quote = await _brapiService.GetStockQuoteAsync(symbol);

            if (quote == null)
            {
                return NotFound($"Stock quote for {symbol} not found.");
            }

            return Ok(quote);
        }
    }
}
