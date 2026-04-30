using FluentAssertions;
using FluentValidation.TestHelper;
using StockAlert.Application.AlertRule.Validator;
using StockAlert.Communication.Requests.AlertRule;

namespace StockAlert.Tests;

public class RegisterAlertRuleValidatorTests
{
    private readonly RegisterAlertRuleValidator _validator;

    public RegisterAlertRuleValidatorTests()
    {
        _validator = new RegisterAlertRuleValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Symbol_Is_Empty()
    {
        var request = new RegisterAlertRuleRequest { StockSymbol = "" };
        var result = _validator.TestValidate(request);

        // Forma alternativa que funciona em qualquer versão:
        result.ShouldHaveValidationErrorFor(x => x.StockSymbol);
    }

    [Fact]
    public void Should_Have_Error_When_Neither_TargetPrice_Nor_PercentageChange_Is_Provided()
    {
        var request = new RegisterAlertRuleRequest
        {
            StockSymbol = "AAPL",
            TargetPrice = null,
            PercentageChange = null
        };
        var result = _validator.TestValidate(request);

        // Se o ShouldHaveAnyValidationError não funciona, usamos o IsValid:
        result.IsValid.Should().BeFalse("because at least one target (price or percentage) must be provided");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid()
    {
        var request = new RegisterAlertRuleRequest
        {
            StockSymbol = "PETR4",
            TargetPrice = 35.00m,
            Operator = (StockAlert.Communication.Enums.ComparisonOperator)1
        };
        var result = _validator.TestValidate(request);

        // Se o ShouldNotHaveAnyValidationErrors não funciona, usamos o IsValid:
        result.IsValid.Should().BeTrue("because the request contains all required valid fields");
    }
}
