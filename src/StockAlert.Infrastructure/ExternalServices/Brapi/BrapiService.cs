using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StockAlert.Domain.Services;
using StockAlert.Domain.Services.Dtos;
using StockAlert.Infrastructure.ExternalServices.Brapi.Models;
using System.Net.Http.Json;

namespace StockAlert.Infrastructure.ExternalServices.Brapi;

public class BrapiService : IBrapiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BrapiService> _logger;
    private readonly string _token;

    public BrapiService(HttpClient httpClient, IConfiguration configuration, ILogger<BrapiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _token = configuration["Brapi:Token"] ?? throw new ArgumentNullException("Brapi:Token is missing.");

        if (httpClient.BaseAddress == null)
        {
            var baseUrl = configuration["Brapi:BaseUrl"] ?? throw new ArgumentNullException("Brapi:BaseUrl is missing.");
            _httpClient.BaseAddress = new Uri(baseUrl);
        }
    }

    public async Task<StockQuoteDto?> GetStockQuoteAsync(string stockSymbol)
    {
        try
        {
            // GetFromJsonAsync simplifica muito o código e já lida com o JSON internamente
            var response = await _httpClient.GetFromJsonAsync<BrapiResponse>($"quote/{stockSymbol}?token={_token}");
            var stockData = response?.Results?.FirstOrDefault();

            if (stockData == null || stockData.RegularMarketPrice == 0)
            {
                _logger.LogInformation("No valid data found for symbol: {Symbol}", stockSymbol);
                return null;
            }

            // Tentativa simplificada de parse de data
            if (!DateTime.TryParse(stockData.RegularMarketTime, out var lastRefresh))
            {
                _logger.LogWarning("Could not parse market time for {Symbol}: {Time}", stockSymbol, stockData.RegularMarketTime);
                return null;
            }

            return new StockQuoteDto
            {
                Symbol = stockData.Symbol ?? "UNKNOWN",
                Price = stockData.RegularMarketPrice,
                LastRefresh = lastRefresh
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching stock quote for {Symbol}", stockSymbol);
            return null;
        }
    }
}
