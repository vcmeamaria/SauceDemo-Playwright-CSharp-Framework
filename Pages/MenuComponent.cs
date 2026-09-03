using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace SauceDemo.Playwright.CSharp.Pages;

public sealed class MenuComponent : BasePage
{
    private ILocator Menu =>
        Page.Locator(".bm-menu");

    private ILocator AllItems =>
        ByDataTest("inventory-sidebar-link");

    private ILocator Reset =>
        ByDataTest("reset-sidebar-link");

    private ILocator Logout =>
        ByDataTest("logout-sidebar-link");

    private ILocator Close =>
        Page.GetByRole(
            AriaRole.Button,
            new() { Name = "Close Menu" });

    public MenuComponent(IPage page) : base(page) { }

    public Task AssertOpenAsync() =>
        Expect(Menu)
            .ToBeVisibleAsync();

    public Task AssertClosedAsync() =>
        Expect(Menu)
            .ToBeHiddenAsync();

    public async Task ResetAppStateAsync() =>
        await ClickAsync(
            Reset,
            "Reset app state");

    public async Task LogoutAsync() =>
        await ClickAsync(
            Logout,
            "Logout");

    public async Task AllItemsAsync() =>
        await ClickAsync(
            AllItems,
            "All items");

    public async Task CloseAsync() =>
        await ClickAsync(
            Close,
            "Close menu");
}