using Microsoft.Playwright;
using SauceDemo.Playwright.CSharp.Models;
using static Microsoft.Playwright.Assertions;

namespace SauceDemo.Playwright.CSharp.Pages;

public sealed class CheckoutInformationPage : BasePage
{
    private ILocator FirstName =>
        ByDataTest("firstName");

    private ILocator LastName =>
        ByDataTest("lastName");

    private ILocator PostalCode =>
        ByDataTest("postalCode");

    private ILocator ContinueButton =>
        ByDataTest("continue");

    private ILocator CancelButton =>
        ByDataTest("cancel");

    private ILocator Error =>
        ByDataTest("error");

    public CheckoutInformationPage(IPage page)
        : base(page) { }

    public async Task ContinueAsync(
        CheckoutCustomer customer)
    {
        await FillAsync(
            FirstName,
            customer.FirstName,
            "first name");

        await FillAsync(
            LastName,
            customer.LastName,
            "last name");

        await FillAsync(
            PostalCode,
            customer.PostalCode,
            "postal code");

        await ClickAsync(
            ContinueButton,
            "Continue checkout");
    }

    public async Task SubmitRawAsync(
        string first,
        string last,
        string postal)
    {
        await FillAsync(
            FirstName,
            first,
            "first name");

        await FillAsync(
            LastName,
            last,
            "last name");

        await FillAsync(
            PostalCode,
            postal,
            "postal code");

        await ClickAsync(
            ContinueButton,
            "Continue checkout");
    }

    public Task AssertErrorContainsAsync(
        string text) =>
        Expect(Error)
            .ToContainTextAsync(text);

    public async Task CancelAsync() =>
        await ClickAsync(
            CancelButton,
            "Cancel checkout");
}