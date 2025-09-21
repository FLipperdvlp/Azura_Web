using System.Globalization;
using Bank_Convert_API.Models;

namespace Bank_Convert_API.Services;

public class CurrencyRateService
{
    private readonly HttpClient _httpClient;

    public CurrencyRateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private async Task<List<CurrencyRate>?> GetRatesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<CurrencyRate>>(
            "https://api.privatbank.ua/p24api/pubinfo?json&exchange&coursid=5" );
    }

    public async Task<CurrencyInfo> GetCurrencyInfoAsync()
    {
        var rates = await GetRatesAsync();
        var currencyInfo = new CurrencyInfo();

        if (rates != null)
        {
            foreach (var rate in rates)
            {
                currencyInfo.From.Add(rate.Ccy);
                currencyInfo.To.Add(rate.Ccy);
            }
        }
        return currencyInfo;
    }

    public async Task<List<CurrencyRate>?> GetCurrencyRatesAsync()
    {
        return await GetRatesAsync();
    }

    public async Task<decimal?> ConvertAsync(string from, string to, decimal amount)
    {
        var rates = await GetRatesAsync();
        if (rates == null) { return null; }

        var fromRate = rates.FirstOrDefault(r => r.Ccy == from.ToUpper());
        var toRate = rates.FirstOrDefault(r => r.Ccy == to.ToUpper());

        decimal amountInUah;
        if (from.ToUpper() == "UAH")
        {
            amountInUah = amount;
        }
        else if (fromRate != null)
        {
            amountInUah = amount * decimal.Parse(fromRate.Sale, CultureInfo.InvariantCulture);
        }
        else
        {
            return null;
        }

        if (to.ToUpper() == "UAH")
        {
            return amountInUah;
        }
        else if (toRate != null)
        {
            return amountInUah / decimal.Parse(toRate.Sale, CultureInfo.InvariantCulture);
        }

        return null;
    }


    public CurrencyInfo GetCurrencyInfo()
    {
        var rates = GetRatesAsync().Result;
        var currencyInfo = new CurrencyInfo();

        foreach (var rate in rates)
        {
            currencyInfo.From.Add(rate.Ccy);
            currencyInfo.To.Add(rate.Ccy);
        }

        return currencyInfo;
    }
}