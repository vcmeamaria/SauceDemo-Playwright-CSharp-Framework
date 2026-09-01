using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace SauceDemo.Playwright.CSharp.Pages;

public sealed class InventoryPage : BasePage
{
    public const string Route = "inventory.html";

    private ILocator InventoryContainer => Page.GetByTestId("inventory-container");
    private ILocator Items => Page.GetByTestId("inventory-item");
    private ILocator CartLink => Page.GetByTestId("shopping-cart-link");
    private ILocator CartBadge => Page.GetByTestId("shopping-cart-badge");
    private ILocator Sort => Page.GetByTestId("product-sort-container");
    private ILocator MenuButton => Page.GetByRole(AriaRole.Button, new() { Name = "Open Menu" });

    public InventoryPage(IPage page) : base(page) { }

    public Task AssertLoadedAsync() =>
        Expect(InventoryContainer).ToBeVisibleAsync();

    public Task<int> ProductCountAsync() => Items.CountAsync();

    public async Task AddProductAsync(string productName)
    {
        var item = Items.Filter(new() { HasText = productName });
        await ClickAsync(item.GetByRole(AriaRole.Button, new() { Name = "Add to cart" }), $"Add {productName}");
    }

    public async Task RemoveProductAsync(string productName)
    {
        var item = Items.Filter(new() { HasText = productName });
        await ClickAsync(item.GetByRole(AriaRole.Button, new() { Name = "Remove" }), $"Remove {productName}");
    }

    public Task AssertCartCountAsync(int count) =>
        Expect(CartBadge).ToHaveTextAsync(count.ToString());

    public Task AssertCartBadgeAbsentAsync() =>
        Expect(CartBadge).ToHaveCountAsync(0);

    public async Task OpenCartAsync() =>
        await ClickAsync(CartLink, "Shopping cart");

    public async Task OpenProductAsync(string productName)
    {
        var item = Items.Filter(new() { HasText = productName });
        await ClickAsync(item.GetByRole(AriaRole.Link, new() { Name = productName }), $"Product {productName}");
    }

    public async Task SortAsync(string value)
    {
        await Sort.SelectOptionAsync(value);
    }

    public async Task<IReadOnlyList<string>> ProductNamesAsync() =>
        await Items.GetByTestId("inventory-item-name").AllInnerTextsAsync();

    public async Task<IReadOnlyList<decimal>> ProductPricesAsync()
    {
        var texts = await Items.GetByTestId("inventory-item-price").AllInnerTextsAsync();
        return texts.Select(t => decimal.Parse(t.Replace("$", ""), System.Globalization.CultureInfo.InvariantCulture)).ToList();
    }

    public async Task OpenMenuAsync() =>
        await ClickAsync(MenuButton, "Open menu");
}
