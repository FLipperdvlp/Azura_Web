using Bank_Convert_API.Models;
using Bank_Convert_API.Services;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly CurrencyRateService currencyRateService;

    public HomeController(CurrencyRateService currencyRateService)
    {
        this.currencyRateService = currencyRateService;
    }

    public IActionResult Index()
    {
        var currencyInfo = currencyRateService.GetCurrencyInfo();
        currencyInfo.From.Add("UAH");
        currencyInfo.To.Add("UAH");

        return View(currencyInfo);
    }

    public IActionResult Convert(ConvertViewModel convertViewModel)
    {
        var convertRes = currencyRateService.ConvertAsync(
            convertViewModel.From,
            convertViewModel.To,
            convertViewModel.Amount
        ).Result;

        if (convertRes.HasValue)
        {
            convertRes = Math.Round(convertRes.Value, 2);
        }

        var currencyInfo = currencyRateService.GetCurrencyInfo();
        currencyInfo.From.Add("UAH");
        currencyInfo.To.Add("UAH");

        ViewBag.Result = convertRes;
        return View("Index", currencyInfo);
    }

}