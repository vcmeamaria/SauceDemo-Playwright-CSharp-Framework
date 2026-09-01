using NUnit.Framework;
using SauceDemo.Playwright.CSharp.Models;
using SauceDemo.Playwright.CSharp.Pages;

namespace SauceDemo.Playwright.CSharp.Tests;

[TestFixture]
[Category("Regression")]
[Category("CartCheckout")]
public sealed class CartCheckoutRegressionTests : BaseTest
{
    private const string Backpack = "Sauce Labs Backpack";
    private const string BikeLight = "Sauce Labs Bike Light";

    [Test, Category("TC-CART-003")]
    public async Task MultipleCartItemsAreDisplayed()
    {
        await LoginAsAsync();
        await AddAndOpenCartAsync(Backpack, BikeLight);
        Assert.That(await CartPage.ItemCountAsync(), Is.EqualTo(2));
    }

    [Test, Category("TC-CART-005")]
    public async Task ItemCanBeRemovedFromCart()
    {
        await LoginAsAsync();
        await AddAndOpenCartAsync(Backpack, BikeLight);
        await CartPage.RemoveAsync(Backpack);
        Assert.That(await CartPage.ItemCountAsync(), Is.EqualTo(1));
        await CartPage.AssertContainsAsync(BikeLight);
    }

    [Test, Category("TC-CART-007")]
    public async Task ContinueShoppingReturnsToInventory()
    {
        await LoginAsAsync();
        await AddAndOpenCartAsync(Backpack);
        await CartPage.ContinueShoppingAsync();
        await InventoryPage.AssertLoadedAsync();
        await InventoryPage.AssertCartCountAsync(1);
    }

    [TestCase("", "Smith", "SW1A 1AA", "First Name is required")]
    [TestCase("John", "", "SW1A 1AA", "Last Name is required")]
    [TestCase("John", "Smith", "", "Postal Code is required")]
    [Category("TC-CHK-003")]
    [Category("TC-CHK-004")]
    [Category("TC-CHK-005")]
    public async Task CheckoutRequiredFieldsAreValidated(string first, string last, string postal, string expected)
    {
        await LoginAsAsync();
        await AddAndOpenCartAsync(Backpack);
        await CartPage.CheckoutAsync();
        await CheckoutInformationPage.SubmitRawAsync(first, last, postal);
        await CheckoutInformationPage.AssertErrorContainsAsync(expected);
    }

    [Test, Category("TC-CHK-017")]
    public async Task CheckoutCanBeCancelled()
    {
        await LoginAsAsync();
        await AddAndOpenCartAsync(Backpack);
        await CartPage.CheckoutAsync();
        await CheckoutInformationPage.CancelAsync();
        await CartPage.AssertContainsAsync(Backpack);
    }

    [Test, Category("TC-OVR-001")]
    public async Task OverviewContainsAllSelectedItems()
    {
        await ReachOverviewAsync(Backpack, BikeLight);
        await CheckoutOverviewPage.AssertContainsAsync(Backpack);
        await CheckoutOverviewPage.AssertContainsAsync(BikeLight);
    }

    [Test, Category("TC-OVR-005")]
    public async Task TotalsAreInternallyConsistent()
    {
        await ReachOverviewAsync(Backpack, BikeLight);
        var subtotal = CheckoutOverviewPage.ParseMoney(await CheckoutOverviewPage.ItemTotalTextAsync());
        var tax = CheckoutOverviewPage.ParseMoney(await CheckoutOverviewPage.TaxTextAsync());
        var total = CheckoutOverviewPage.ParseMoney(await CheckoutOverviewPage.TotalTextAsync());
        Assert.That(total, Is.EqualTo(subtotal + tax));
    }

    [Test, Category("TC-ORD-004")]
    public async Task CartIsClearedAfterCompletedOrder()
    {
        await ReachOverviewAsync(Backpack);
        await CheckoutOverviewPage.FinishAsync();
        await CheckoutCompletePage.BackHomeAsync();
        await InventoryPage.AssertCartBadgeAbsentAsync();
    }

    [Test, Category("TC-NAV-005")]
    public async Task ResetAppStateClearsCart()
    {
        await LoginAsAsync();
        await InventoryPage.AddProductAsync(Backpack);
        await InventoryPage.OpenMenuAsync();
        await Menu.ResetAppStateAsync();
        await InventoryPage.AssertCartBadgeAbsentAsync();
    }

    [Test, Category("TC-NAV-007")]
    public async Task ProtectedPageCannotBeUsedAfterLogout()
    {
        await LoginAsAsync();
        await InventoryPage.OpenMenuAsync();
        await Menu.LogoutAsync();
        await Page.GotoAsync(Settings.BaseUrl + "inventory.html");
        await LoginPage.AssertErrorContainsAsync("You can only access '/inventory.html' when you are logged in.");
    }
}
