using CurrencyConverter.Handlers.Base;
using CurrencyConverter.Interfaces.Broker;
using CurrencyConverter.Models;

namespace CurrencyConverter.Handlers;

public class InputParserHandler(ICurrencyBroker currencyBroker) : AbstractHandler
{
    private const int _commandIndex = 0;
    private const int _currencyPairIndex = 1;
    private const int _amountIndex = 2;
    
    public override InputContext Handle(InputContext context)
    {
        var arguments = context.UserInput
            .Split()
            .ToList();

        (var mainCurrencyIsoCode, var moneyCurrencyIsoCode, var amount) = ConvertInputsToRequiredTypes(arguments);

        var mainCurrency = currencyBroker.GetCurrencyByIsoCode(mainCurrencyIsoCode);
        var moneyCurrency = currencyBroker.GetCurrencyByIsoCode(moneyCurrencyIsoCode);

        context.SetMainCurrency(mainCurrency);
        context.SetMoneyCurrency(moneyCurrency);
        context.SetOriginalAmount(amount);

        return base.Handle(context);
    }

    private static (string mainCurrency, string moneyCurrency, decimal amount) ConvertInputsToRequiredTypes(
        List<string> arguments)
    {
        var currencyPair = arguments[_currencyPairIndex].Split('/');

        var mainCurrency = currencyPair[_commandIndex];
        var moneyCurrency = currencyPair[_currencyPairIndex];

        var amount = decimal.Parse(arguments[_amountIndex]);

        return (mainCurrency, moneyCurrency, amount);
    }
}
