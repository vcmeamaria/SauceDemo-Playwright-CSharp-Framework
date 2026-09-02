using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace SauceDemo.Playwright.CSharp.Pages;

public sealed class CheckoutOverviewPage : BasePage
{
    private ILocator Items =>
        ByDataTest("inventory-item");

    private ILocator ItemTotal =>
        ByDataTest("subtotal-label");

    private ILocator Tax =>
        ByDataTest("tax-label");

    private ILocator Total =>
        ByDataTest("total-label");

    private ILocator FinishButton =>
        ByDataTest("finish");

    private ILocator CancelButton =>
        ByDataTest("cancel");

    public CheckoutOverviewPage(IPage page)
        : base(page) { }

    public Task AssertContainsAsync(
        string productName) =>
        Expect(
            Items.Filter(new()
            {
                HasText = productName
            }))
        .ToHaveCountAsync(1);

    public Task<string> ItemTotalTextAsync() =>
        ItemTotal.InnerTextAsync();

    public Task<string> TaxTextAsync() =>
        Tax.InnerTextAsync();

    public Task<string> TotalTextAsync() =>
        Total.InnerTextAsync();

    public async Task FinishAsync() =>
        await ClickAsync(
            FinishButton,
            "Finish order");

    public async Task CancelAsync() =>
        await ClickAsync(
            CancelButton,
            "Cancel order");

    public static decimal ParseMoney(
        string text) =>
        decimal.Parse(
            System.Text.RegularExpressions.Regex
                .Match(
                    text,
                    @"\d+\.\d{2}")
                .Value,
            System.Globalization.CultureInfo
                .InvariantCulture);
}