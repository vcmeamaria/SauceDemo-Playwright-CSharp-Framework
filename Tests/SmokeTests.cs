using NUnit.Framework;
using SauceDemo.Playwright.CSharp.Models;
using SauceDemo.Playwright.CSharp.Pages;

namespace SauceDemo.Playwright.CSharp.Tests;

[TestFixture]
[Category("Smoke")]
public sealed class SmokeTests : BaseTest
{
    private const string Backpack = "Sauce Labs Backpack";

    [Test, Category("TC-LOGIN-001")]
    public async Task LoginPageLoads()
    {
        await LoginPage.OpenAsync(Settings.BaseUrl);
        await LoginPage.AssertLoadedAsync();
    }

    [Test, Category("TC-LOGIN-002")]
    public async Task StandardUserCanLogin()
    {
        await LoginAsAsync();
    }

    [Test, Category("TC-INV-001")]
    public async Task InventoryPageLoads()
    {
        await LoginAsAsync();
        Assert.That(await InventoryPage.ProductCountAsync(), Is.EqualTo(6));
    }

    [Test, Category("TC-INV-007")]
    public async Task ProductCanBeAddedToCart()
    {
        await LoginAsAsync();
        await InventoryPage.AddProductAsync(Backpack);
        await InventoryPage.AssertCartCountAsync(1);
    }

    [Test, Category("TC-CART-002")]
    public async Task CartDisplaysCorrectProduct()
    {
        await LoginAsAsync();
        await AddAndOpenCartAsync(Backpack);
        await CartPage.AssertContainsAsync(Backpack);
    }

    [Test, Category("TC-CHK-001")]
    public async Task ValidCheckoutInformationIsAccepted()
    {
        await LoginAsAsync();
        await AddAndOpenCartAsync(Backpack);
        await CartPage.CheckoutAsync();
        await CheckoutInformationPage.ContinueAsync(CheckoutCustomer.ValidUkCustomer);
        await CheckoutOverviewPage.AssertContainsAsync(Backpack);
    }

    [Test, Category("TC-OVR-003")]
    public async Task ItemSubtotalIsCorrect()
    {
        await ReachOverviewAsync(Backpack);
        var subtotal = CheckoutOverviewPage.ParseMoney(await CheckoutOverviewPage.ItemTotalTextAsync());
        Assert.That(subtotal, Is.EqualTo(29.99m));
    }

    [Test, Category("TC-OVR-005")]
    public async Task FinalTotalEqualsSubtotalPlusTax()
    {
        await ReachOverviewAsync(Backpack);
        var subtotal = CheckoutOverviewPage.ParseMoney(await CheckoutOverviewPage.ItemTotalTextAsync());
        var tax = CheckoutOverviewPage.ParseMoney(await CheckoutOverviewPage.TaxTextAsync());
        var total = CheckoutOverviewPage.ParseMoney(await CheckoutOverviewPage.TotalTextAsync());
        Assert.That(total, Is.EqualTo(subtotal + tax));
    }

    [Test, Category("TC-OVR-010")]
    public async Task OrderCanBeCompleted()
    {
        await ReachOverviewAsync(Backpack);
        await CheckoutOverviewPage.FinishAsync();
        await CheckoutCompletePage.AssertOrderCompleteAsync();
    }

    [Test, Category("TC-NAV-006")]
    public async Task UserCanLogout()
    {
        await LoginAsAsync();
        await InventoryPage.OpenMenuAsync();
        await Menu.LogoutAsync();
        await LoginPage.AssertLoadedAsync();
    }
}
