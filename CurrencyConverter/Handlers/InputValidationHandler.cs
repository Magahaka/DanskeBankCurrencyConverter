using CurrencyConverter.Handlers.Base;
using CurrencyConverter.Interfaces;
using CurrencyConverter.Models;

namespace CurrencyConverter.Handlers;

public class InputValidationHandler(ICurrencyConverterValidator currencyConverterValidator) : AbstractHandler
{
    private const int _commandIndex = 0;
    private const int _currencyPairIndex = 1;
    private const int _amountIndex = 2;
    
    public override InputContext Handle(InputContext input)
    {
        currencyConverterValidator.ValidateInputArgumentCount(input.UserInput);

        var arguments = input.UserInput
            .Split()
            .ToList();

        var command = arguments[_commandIndex];
        currencyConverterValidator.ValidateInputCommand(command);

        var currencyPair = arguments[_currencyPairIndex];
        currencyConverterValidator.ValidateCurrencyPairFormat(currencyPair);

        var amount = arguments[_amountIndex];
        currencyConverterValidator.ValidateAmount(amount);

        return base.Handle(input);
    }
}
