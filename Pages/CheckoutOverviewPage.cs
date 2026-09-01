using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace SauceDemo.Playwright.CSharp.Pages;

public sealed class CheckoutOverviewPage : BasePage
{
    private ILocator Items => Page.GetByTestId("inventory-item");
    private ILocator ItemTotal => Page.GetByTestId("subtotal-label");
    private ILocator Tax => Page.GetByTestId("tax-label");
    private ILocator Total => Page.GetByTestId("total-label");
    private ILocator FinishButton => Page.GetByTestId("finish");
    private ILocator CancelButton => Page.GetByTestId("cancel");

    public CheckoutOverviewPage(IPage page) : base(page) { }

    public Task AssertContainsAsync(string productName) =>
        Expect(Items.Filter(new() { HasText = productName })).ToHaveCountAsync(1);

    public Task<string> ItemTotalTextAsync() => ItemTotal.InnerTextAsync();
    public Task<string> TaxTextAsync() => Tax.InnerTextAsync();
    public Task<string> TotalTextAsync() => Total.InnerTextAsync();

    public async Task FinishAsync() => await ClickAsync(FinishButton, "Finish order");
    public async Task CancelAsync() => await ClickAsync(CancelButton, "Cancel order");

    public static decimal ParseMoney(string text) =>
        decimal.Parse(
            System.Text.RegularExpressions.Regex.Match(text, @"\d+\.\d{2}").Value,
            System.Globalization.CultureInfo.InvariantCulture);
}
