using Moq;
using FluentAssertions;
using StockAlert.Application.AlertRule.UseCases;
using StockAlert.Domain.Repositories;
using StockAlert.Domain.Security;
using StockAlert.Communication.Requests.AlertRule;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace StockAlert.Tests;

public class RegisterAlertRuleUseCaseTests
{
    [Fact]
    public async Task Execute_Should_Throw_Exception_When_Stock_Not_Registered()
    {
        // Arrange (Configuração)
        var alertRepoMock = new Mock<IAlertRuleRepository>();
        var stockRepoMock = new Mock<IStockRepository>();
        var validatorMock = new Mock<IValidator<RegisterAlertRuleRequest>>();
        var userAccessorMock = new Mock<ILoggedUserAccessor>();
        var loggerMock = new Mock<ILogger<RegisterAlertRuleUseCase>>();

        // Simula que o validador passou
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<RegisterAlertRuleRequest>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // Simula que o usuário logado é o ID 1
        userAccessorMock.Setup(u => u.GetUserId()).Returns(Guid.NewGuid());

        // Simula que a ação NÃO existe no banco (retorna null)
        stockRepoMock.Setup(s => s.GetBySymbolAndUserIdAsync(It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync((StockAlert.Domain.Entities.Stock?)null);

        var useCase = new RegisterAlertRuleUseCase(
            alertRepoMock.Object,
            stockRepoMock.Object,
            validatorMock.Object,
            userAccessorMock.Object,
            loggerMock.Object);

        var request = new RegisterAlertRuleRequest { StockSymbol = "INVALID" };

        // Act (Ação)
        var act = async () => await useCase.Execute(request);

        // Assert (Verificação)
        var exception = await act.Should().ThrowAsync<StockAlert.Exception.ValidationException>();

        // Verificamos se dentro da lista de erros da exceção existe a mensagem esperada
        exception.Which.Errors.Should().Contain(e => e.Contains("must be registered before creating an alert rule"));
    }
}
