using FluentValidation;
using StockAlert.Communication.Requests.Stock;
using StockAlert.Communication.Responses.Stock;
using StockAlert.Domain.Repositories;

namespace StockAlert.Application.Stock.UseCases;

public class RegisterStockUseCase
{
    private readonly IStockRepository _stockRepository;
    private readonly IValidator<RegisterStockRequest> _validator;

    public RegisterStockUseCase(
     IStockRepository stockRepository,
     IValidator<StockAlert.Communication.Requests.Stock.RegisterStockRequest> validator) 
    {
        _stockRepository = stockRepository;
        _validator = validator;
    }

    public async Task<StockResponse> Execute(RegisterStockRequest request)
    {
        // 1. Validar a requisição usando o FluentValidation que você criou
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new Exception.ValidationException(errors); // Sua exceção customizada
        }

        // 2. Verificar se a ação já está cadastrada
        var existingStock = await _stockRepository.GetBySymbolAsync(request.Symbol);

        if (existingStock != null)
        {
            // Se já existe, apenas retornamos os dados atuais
            return new StockResponse
            {
                Symbol = existingStock.Symbol,
                CurrentPrice = existingStock.CurrentPrice,
                LastUpdated = existingStock.LastUpdated
            };
        }

        // 3. Criar a nova entidade de Stock
        var newStock = new Domain.Entities.Stock
        {
            Symbol = request.Symbol.ToUpper(),
            CurrentPrice = 0, // Será atualizado pelo serviço externo no futuro
            LastUpdated = DateTime.UtcNow,
            Source = "Manual Registration"
        };

        // 4. Salvar no banco
        await _stockRepository.AddAsync(newStock);

        // 5. Retornar a resposta padronizada
        return new StockResponse
        {
            Symbol = newStock.Symbol,
            CurrentPrice = newStock.CurrentPrice,
            LastUpdated = newStock.LastUpdated
        };
    }
}
