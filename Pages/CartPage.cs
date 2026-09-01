using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace SauceDemo.Playwright.CSharp.Pages;

public sealed class CartPage : BasePage
{
    private ILocator CartList => Page.GetByTestId("cart-list");
    private ILocator CartItems => Page.GetByTestId("inventory-item");
    private ILocator CheckoutButton => Page.GetByTestId("checkout");
    private ILocator ContinueShoppingButton => Page.GetByTestId("continue-shopping");

    public CartPage(IPage page) : base(page) { }

    public Task AssertLoadedAsync() => Expect(CartList).ToBeVisibleAsync();

    public Task<int> ItemCountAsync() => CartItems.CountAsync();

    public Task AssertContainsAsync(string productName) =>
        Expect(CartItems.Filter(new() { HasText = productName })).ToHaveCountAsync(1);

    public async Task RemoveAsync(string productName)
    {
        var item = CartItems.Filter(new() { HasText = productName });
        await ClickAsync(item.GetByRole(AriaRole.Button, new() { Name = "Remove" }), $"Remove {productName} from cart");
    }

    public async Task CheckoutAsync() => await ClickAsync(CheckoutButton, "Checkout");
    public async Task ContinueShoppingAsync() => await ClickAsync(ContinueShoppingButton, "Continue shopping");
}
