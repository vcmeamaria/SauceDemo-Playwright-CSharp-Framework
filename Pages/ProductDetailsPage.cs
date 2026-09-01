using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace SauceDemo.Playwright.CSharp.Pages;

public sealed class ProductDetailsPage : BasePage
{
    private ILocator Name => Page.GetByTestId("inventory-item-name");
    private ILocator Price => Page.GetByTestId("inventory-item-price");
    private ILocator AddButton => Page.GetByRole(AriaRole.Button, new() { Name = "Add to cart" });
    private ILocator RemoveButton => Page.GetByRole(AriaRole.Button, new() { Name = "Remove" });
    private ILocator BackButton => Page.GetByTestId("back-to-products");

    public ProductDetailsPage(IPage page) : base(page) { }

    public Task AssertProductAsync(string expectedName) =>
        Expect(Name).ToHaveTextAsync(expectedName);

    public Task<string> PriceTextAsync() => Price.InnerTextAsync();

    public async Task AddToCartAsync() => await ClickAsync(AddButton, "Add product from details");
    public async Task RemoveFromCartAsync() => await ClickAsync(RemoveButton, "Remove product from details");
    public async Task BackToProductsAsync() => await ClickAsync(BackButton, "Back to products");
}
