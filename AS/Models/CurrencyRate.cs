namespace Bank_Convert_API.Models;

public class CurrencyRate
{
    public string Ccy { get; set; } = "";
    public string? Base_ccy { get; set; }
    public string Buy { get; set; } = "";
    public string Sale { get; set; } = "";
}