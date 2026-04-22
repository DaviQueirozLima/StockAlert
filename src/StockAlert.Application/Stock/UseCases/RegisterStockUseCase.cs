// Local: src/StockAlert.Application/Stock/UseCases/RegisterStockUseCase.cs

namespace StockAlert.Application.Stock.UseCases;

public class RegisterStockUseCase
{
    private readonly StockAlert.Domain.Repositories.IStockRepository _stockRepository;
    private readonly FluentValidation.IValidator<StockAlert.Communication.Requests.Stock.RegisterStockRequest> _validator;
    private readonly StockAlert.Domain.Security.ILoggedUserAccessor _loggedUserAccessor;
    private readonly StockAlert.Domain.Services.IBrapiService _brapiService;

    public RegisterStockUseCase(
        StockAlert.Domain.Repositories.IStockRepository stockRepository,
        FluentValidation.IValidator<StockAlert.Communication.Requests.Stock.RegisterStockRequest> validator,
        StockAlert.Domain.Security.ILoggedUserAccessor loggedUserAccessor,
        StockAlert.Domain.Services.IBrapiService brapiService)
    {
        _stockRepository = stockRepository;
        _validator = validator;
        _loggedUserAccessor = loggedUserAccessor;
        _brapiService = brapiService;
    }

    public async Task<StockAlert.Communication.Responses.Stock.StockResponse> Execute(StockAlert.Communication.Requests.Stock.RegisterStockRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            // USANDO O CAMINHO COMPLETO PARA A EXCEÇÃO:        
            throw new StockAlert.Exception.ValidationException(errors);
        }

        var userId = _loggedUserAccessor.GetUserId();
        var symbol = request.Symbol.ToUpper();

        // USANDO O MÉTODO ATUALIZADO NO REPOSITÓRIO:
        var existingStock = await _stockRepository.GetBySymbolAndUserIdAsync(symbol, userId);

        if (existingStock != null)
        {
            return new StockAlert.Communication.Responses.Stock.StockResponse
            {
                Symbol = existingStock.Symbol,
                CurrentPrice = existingStock.CurrentPrice,
                LastUpdated = existingStock.LastUpdated
            };
        }

        var quote = await _brapiService.GetStockQuoteAsync(symbol);
        if (quote == null)
        {           
            throw new StockAlert.Exception.ValidationException(new List<string> { $"Ação {symbol} não encontrada." });
        }

        // USANDO O CAMINHO COMPLETO PARA A ENTIDADE:
        var newStock = new StockAlert.Domain.Entities.Stock
        {
            Symbol = symbol,
            CurrentPrice = quote.Price,
            LastUpdated = DateTime.UtcNow,
            Source = "Brapi Integration",
            UserId = userId
        };

        await _stockRepository.AddAsync(newStock);

        return new StockAlert.Communication.Responses.Stock.StockResponse
        {
            Symbol = newStock.Symbol,
            CurrentPrice = newStock.CurrentPrice,
            LastUpdated = newStock.LastUpdated
        };
    }
}
