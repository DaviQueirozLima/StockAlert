using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using StockAlert.Domain.Services;
using StockAlert.Domain.Services.Dtos;
using StockAlert.Infrastructure.ExternalServices.Brapi.Models;

namespace StockAlert.Infrastructure.ExternalServices.Brapi
{
    public class BrapiService : IBrapiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _brapiBaseUrl;
        private readonly string _brapiToken; 

        public BrapiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _brapiBaseUrl = configuration["Brapi:BaseUrl"]
                            ?? throw new ArgumentNullException("Brapi:BaseUrl configuration is missing.");
            _brapiToken = configuration["Brapi:Token"]
                          ?? throw new ArgumentNullException("Brapi:Token configuration is missing."); 
        }

        public async Task<StockQuoteDto?> GetStockQuoteAsync(string stockSymbol)
        {
            // Usando o token lido da configuração
            var requestUrl = $"{_brapiBaseUrl}/quote/{stockSymbol}?token={_brapiToken}";

            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            // Adicionando verificação para o resultado da desserialização
            var brapiResponse = JsonSerializer.Deserialize<BrapiResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Verificando se brapiResponse é nulo e se Results é nulo ou vazio
            if (brapiResponse?.Results == null || !brapiResponse.Results.Any())
            {
                return null;
            }

            var stockData = brapiResponse.Results.FirstOrDefault();

            // Verificando se stockData é nulo ou se suas propriedades essenciais são nulas
            if (stockData == null || stockData.Symbol == null || stockData.RegularMarketPrice == 0 || stockData.RegularMarketTime == null)
            {
                return null;
            }
            DateTime lastRefreshDateTime;
            if (!DateTime.TryParse(stockData.RegularMarketTime, out lastRefreshDateTime))
            {
                // Se a conversão da string de data falhar, podemos logar e retornar null
                return null;
            }

            return new StockQuoteDto
            {
                Symbol = stockData.Symbol,
                Price = stockData.RegularMarketPrice,
                LastRefresh = lastRefreshDateTime 
            };
        }
    }
}
