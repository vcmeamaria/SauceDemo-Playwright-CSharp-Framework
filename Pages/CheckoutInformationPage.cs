using Microsoft.Playwright;
using SauceDemo.Playwright.CSharp.Models;
using static Microsoft.Playwright.Assertions;

namespace SauceDemo.Playwright.CSharp.Pages;

public sealed class CheckoutInformationPage : BasePage
{
    private ILocator FirstName => Page.GetByTestId("firstName");
    private ILocator LastName => Page.GetByTestId("lastName");
    private ILocator PostalCode => Page.GetByTestId("postalCode");
    private ILocator ContinueButton => Page.GetByTestId("continue");
    private ILocator CancelButton => Page.GetByTestId("cancel");
    private ILocator Error => Page.GetByTestId("error");

    public CheckoutInformationPage(IPage page) : base(page) { }

    public async Task ContinueAsync(CheckoutCustomer customer)
    {
        await FillAsync(FirstName, customer.FirstName, "first name");
        await FillAsync(LastName, customer.LastName, "last name");
        await FillAsync(PostalCode, customer.PostalCode, "postal code");
        await ClickAsync(ContinueButton, "Continue checkout");
    }

    public async Task SubmitRawAsync(string first, string last, string postal)
    {
        await FillAsync(FirstName, first, "first name");
        await FillAsync(LastName, last, "last name");
        await FillAsync(PostalCode, postal, "postal code");
        await ClickAsync(ContinueButton, "Continue checkout");
    }

    public Task AssertErrorContainsAsync(string text) => Expect(Error).ToContainTextAsync(text);
    public async Task CancelAsync() => await ClickAsync(CancelButton, "Cancel checkout");
}
