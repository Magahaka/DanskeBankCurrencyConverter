using CurrencyConverter.CurrencyProviders;
using CurrencyConverter.Handlers;
using CurrencyConverter.Models;
using FluentAssertions;

namespace UnitTests.Handlers;

public class CurrencyConverterHandlerTests
{
    private readonly CurrencyConverterHandler _handler;
    private readonly InputContext _inputContext;

    public CurrencyConverterHandlerTests()
    {
        _handler = new();

        _inputContext = new InputContext();
    }

    [Fact]
    public void Handle_ShouldConvertAmountCorrectly()
    {
        _inputContext.SetOriginalAmount(1M);
        _inputContext.SetMainCurrency(new Euro() { ConversionRate = 2M });
        _inputContext.SetMoneyCurrency(new UnitedStatesDollar() { ConversionRate = 4M });

        var result = _handler.Handle(_inputContext);

        result.ConvertedAmount.Should().Be(0.5M);
    }

    [Fact]
    public void Handle_ShouldReturnSameAmount_WhenBothRatesAreEqual()
    {
        _inputContext.SetOriginalAmount(1M);
        _inputContext.SetMainCurrency(new Euro() { ConversionRate = 1M });
        _inputContext.SetMoneyCurrency(new UnitedStatesDollar() { ConversionRate = 1M });

        var result = _handler.Handle(_inputContext);

        result.ConvertedAmount.Should().Be(1M);
    }

    [Fact]
    public void Handle_ShouldThrowDivideByZeroException_WhenMonryCurrencyRateIsZero()
    {
        _inputContext.SetOriginalAmount(1M);
        _inputContext.SetMainCurrency(new Euro() { ConversionRate = 1M });
        _inputContext.SetMoneyCurrency(new UnitedStatesDollar() { ConversionRate = 0M });

        _handler
            .Invoking(x => x.Handle(_inputContext))
            .Should()
            .Throw<DivideByZeroException>()
            .WithMessage("The conversion rate of the money currency cannot be zero.");
    }

    [Fact]
    public void Handle_ShouldHandleNegativeAmounts()
    {
        _inputContext.SetOriginalAmount(-1M);
        _inputContext.SetMainCurrency(new Euro() { ConversionRate = 4M });
        _inputContext.SetMoneyCurrency(new UnitedStatesDollar() { ConversionRate = 2M });

        var result = _handler.Handle(_inputContext);

        result.ConvertedAmount.Should().Be(-2M);
    }

    [Fact]
    public void Handle_ShouldPreserveDecimalPrecision()
    {
        _inputContext.SetOriginalAmount(1M);
        _inputContext.SetMainCurrency(new Euro() { ConversionRate = 1.3333M });
        _inputContext.SetMoneyCurrency(new UnitedStatesDollar() { ConversionRate = 2M });

        var result = _handler.Handle(_inputContext);

        result.ConvertedAmount.Should().Be(0.66665M);
    }

    [Fact]
    public void Handle_ShouldReturnZero_WhenOriginalAmountIsZero()
    {
        _inputContext.SetOriginalAmount(0M);
        _inputContext.SetMainCurrency(new Euro() { ConversionRate = 1M });
        _inputContext.SetMoneyCurrency(new UnitedStatesDollar() { ConversionRate = 1M });

        var result = _handler.Handle(_inputContext);

        result.ConvertedAmount.Should().Be(0M);
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenConvertedAmpuntIsMorrThanMaxValue()
    {
        _inputContext.SetOriginalAmount(decimal.MaxValue);
        _inputContext.SetMainCurrency(new Euro() { ConversionRate = 2M });
        _inputContext.SetMoneyCurrency(new UnitedStatesDollar() { ConversionRate = 1M });

        _handler
            .Invoking(x => x.Handle(_inputContext))
            .Should()
            .Throw<OverflowException>()
            .WithMessage("Value was either too large or too small for a Decimal.");
    }
}
