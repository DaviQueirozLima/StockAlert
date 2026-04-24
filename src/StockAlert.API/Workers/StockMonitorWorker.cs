// Local: src/StockAlert.API/Workers/StockMonitorWorker.cs

using StockAlert.Domain.Repositories;
using StockAlert.Domain.Services;
using StockAlert.Domain.Enums;

namespace StockAlert.API.Workers;

public class StockMonitorWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StockMonitorWorker> _logger;

    public StockMonitorWorker(IServiceProvider serviceProvider, ILogger<StockMonitorWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Stock Monitor Worker is starting...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Criamos um escopo para poder usar os repositórios (Scoped) dentro do Worker (Singleton)
                using (var scope = _serviceProvider.CreateScope())
                {
                    // Injeção de dependência manual dentro do escopo
                    var alertRepo = scope.ServiceProvider.GetRequiredService<IAlertRuleRepository>();
                    var brapiService = scope.ServiceProvider.GetRequiredService<IBrapiService>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var historyRepo = scope.ServiceProvider.GetRequiredService<INotificationHistoryRepository>();

                    // 1. Busca todas as regras de alerta ativas (com os dados do usuário inclusos)
                    var activeRules = await alertRepo.GetAllActiveAsync();

                    foreach (var rule in activeRules)
                    {
                        // 2. Busca o preço atual na API da Brapi
                        var quote = await brapiService.GetStockQuoteAsync(rule.StockSymbol);

                        if (quote != null)
                        {
                            // 3. Verifica se a condição da regra foi atingida (Ex: Preço > Alvo)
                            if (CheckCondition(quote.Price, rule))
                            {
                                // 4. Verifica o Cooldown (Evita mandar e-mail repetido em curto intervalo)
                                if (CanSendNotification(rule))
                                {
                                    _logger.LogInformation($"Condition met for {rule.StockSymbol} (User: {rule.User?.Email})");

                                    // 5. Dispara a notificação (E-mail Fake no console)
                                    await emailService.SendAlertEmailAsync(
                                         rule.User!.Email,
                                         $"Stock Alert: {rule.StockSymbol} reached target!",
                                         $"The stock {rule.StockSymbol} is now {quote.Price:C2}. Your target was {rule.TargetPrice:C2}."
                                     );

                                    // 6. Registra o histórico na tabela NotificationHistories para o PgAdmin
                                    var history = new StockAlert.Domain.Entities.NotificationHistory
                                    {
                                        AlertRuleId = rule.Id,
                                        UserId = rule.UserId,
                                        SentAt = DateTime.UtcNow,
                                        Channel = StockAlert.Domain.Enums.NotificationChannel.Email,
                                        Success = true,
                                        Status = "Sent",
                                        Message = $"Stock {rule.StockSymbol} reached {quote.Price:C2}",
                                        Recipient = rule.User!.Email
                                    };

                                    await historyRepo.AddAsync(history);

                                    // 7. Atualiza o banco com o horário do disparo e desativa se for NotifyOnce
                                    rule.LastTriggeredAt = DateTime.UtcNow;
                                    if (rule.NotifyOnce) rule.IsActive = false;

                                    await alertRepo.UpdateAsync(rule);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Stock Monitor Worker.");
            }

            // Espera 10 segundos para o teste (Depois você pode voltar para 5 minutos)
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private bool CheckCondition(decimal currentPrice, StockAlert.Domain.Entities.AlertRule rule)
    {
        if (rule.TargetPrice == null) return false;

        return rule.Operator switch
        {
            StockAlert.Domain.Enums.ComparisonOperator.GreaterThan => currentPrice > rule.TargetPrice,
            StockAlert.Domain.Enums.ComparisonOperator.LessThan => currentPrice < rule.TargetPrice,
            StockAlert.Domain.Enums.ComparisonOperator.Equal => currentPrice == rule.TargetPrice,
            StockAlert.Domain.Enums.ComparisonOperator.GreaterThanOrEqual => currentPrice >= rule.TargetPrice,
            StockAlert.Domain.Enums.ComparisonOperator.LessThanOrEqual => currentPrice <= rule.TargetPrice,
            _ => false
        };
    }

    private bool CanSendNotification(StockAlert.Domain.Entities.AlertRule rule)
    {
        if (rule.LastTriggeredAt == null) return true;

        var cooldown = rule.CooldownMinutes ?? 15;
        // Compara o tempo atual com o último disparo + tempo de espera (cooldown)
        return DateTime.UtcNow >= rule.LastTriggeredAt.Value.AddMinutes(cooldown);
    }
}
