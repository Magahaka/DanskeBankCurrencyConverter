using CurrencyConverter.CurrencyProviders;
using CurrencyConverter.Handlers;
using CurrencyConverter.Interfaces;
using CurrencyConverter.Models;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace UnitTests.Handlers;

public class InputValidationHandlerTests
{
    private readonly ICurrencyConverterValidator _validator = Substitute.For<ICurrencyConverterValidator>();

    private readonly InputValidationHandler _handler;
    private readonly InputContext _inputContext;  

    public InputValidationHandlerTests()
    {
        _handler = new InputValidationHandler(_validator);

        _inputContext = new InputContext() { UserInput = "Exchange EUR/USD 1" };
    }

    [Fact]
    public void Handle_ShouldCallValidators()
    {
        _handler.Handle(_inputContext);

        _validator
            .Received(1)
            .ValidateInputArgumentCount("Exchange EUR/USD 1");

        _validator
            .Received(1)
            .ValidateInputCommand("Exchange");

        _validator
            .Received(1)
            .ValidateCurrencyPairFormat("EUR/USD");

        _validator
            .Received(1)
            .ValidateAmount("1");
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenInputArgumentIsNotValid()
    {
        var exception = new Exception("error");

        _validator
            .When(x => x.ValidateInputArgumentCount(_inputContext.UserInput))
            .Throw<Exception>();

        _handler
            .Invoking(x => x.Handle(_inputContext))
            .Should()
            .Throw<Exception>()
            .WithMessage("error");
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenCurrencyPairArgumentIdNotValid()
    {
        var exception = new Exception("error");

        _validator
            .When(x => x.ValidateCurrencyPairFormat(Arg.Any<string>()))
            .Throw<Exception>();

        _handler
            .Invoking(x => x.Handle(_inputContext))
            .Should()
            .Throw(exception)
            .WithMessage("error");
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenAmountArgumentIsNotValid()
    {
        var exception = new Exception("error");

        _validator
            .When(x => x.ValidateAmount(Arg.Any<string>()))
            .Throw(exception);

        _handler
            .Invoking(x => x.Handle(_inputContext))
            .Should()
            .Throw<Exception>()
            .WithMessage("error");
    }
}

