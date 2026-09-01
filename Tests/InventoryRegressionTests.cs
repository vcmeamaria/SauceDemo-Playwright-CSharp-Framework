using NUnit.Framework;

namespace SauceDemo.Playwright.CSharp.Tests;

[TestFixture]
[Category("Regression")]
[Category("Inventory")]
public sealed class InventoryRegressionTests : BaseTest
{
    private const string Backpack = "Sauce Labs Backpack";
    private const string BikeLight = "Sauce Labs Bike Light";

    [SetUp]
    public async Task Login() => await LoginAsAsync();

    [Test, Category("TC-INV-008")]
    public async Task MultipleProductsUpdateCartCount()
    {
        await InventoryPage.AddProductAsync(Backpack);
        await InventoryPage.AddProductAsync(BikeLight);
        await InventoryPage.AssertCartCountAsync(2);
    }

    [Test, Category("TC-INV-009")]
    public async Task ProductCanBeRemovedFromInventory()
    {
        await InventoryPage.AddProductAsync(Backpack);
        await InventoryPage.RemoveProductAsync(Backpack);
        await InventoryPage.AssertCartBadgeAbsentAsync();
    }

    [TestCase("az", true, TestName = "Sort names A to Z")]
    [TestCase("za", false, TestName = "Sort names Z to A")]
    [Category("TC-INV-011")]
    [Category("TC-INV-012")]
    public async Task NameSortingWorks(string option, bool ascending)
    {
        await InventoryPage.SortAsync(option);
        var actual = (await InventoryPage.ProductNamesAsync()).ToList();
        var expected = ascending
            ? actual.OrderBy(x => x, StringComparer.Ordinal).ToList()
            : actual.OrderByDescending(x => x, StringComparer.Ordinal).ToList();
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase("lohi", true, TestName = "Sort price low to high")]
    [TestCase("hilo", false, TestName = "Sort price high to low")]
    [Category("TC-INV-013")]
    [Category("TC-INV-014")]
    public async Task PriceSortingWorks(string option, bool ascending)
    {
        await InventoryPage.SortAsync(option);
        var actual = (await InventoryPage.ProductPricesAsync()).ToList();
        var expected = ascending ? actual.Order().ToList() : actual.OrderDescending().ToList();
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test, Category("TC-INV-015")]
    public async Task SortingPreservesCart()
    {
        await InventoryPage.AddProductAsync(Backpack);
        await InventoryPage.SortAsync("hilo");
        await InventoryPage.AssertCartCountAsync(1);
    }

    [Test, Category("TC-PDP-001")]
    public async Task ProductDetailsDisplayCorrectProduct()
    {
        await InventoryPage.OpenProductAsync(Backpack);
        await ProductDetailsPage.AssertProductAsync(Backpack);
    }
}
