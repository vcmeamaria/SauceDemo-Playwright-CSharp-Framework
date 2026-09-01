using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace SauceDemo.Playwright.CSharp.Pages;

public sealed class LoginPage : BasePage
{
    private ILocator Username => Page.GetByTestId("username");
    private ILocator Password => Page.GetByTestId("password");
    private ILocator LoginButton => Page.GetByTestId("login-button");
    private ILocator Error => Page.GetByTestId("error");

    public LoginPage(IPage page) : base(page) { }

    public async Task OpenAsync(string baseUrl)
    {
        await Page.GotoAsync(baseUrl, new() { WaitUntil = WaitUntilState.DOMContentLoaded });
        await Expect(LoginButton).ToBeVisibleAsync();
    }

    public async Task LoginAsync(string username, string password)
    {
        await FillAsync(Username, username, "username");
        await FillAsync(Password, password, "password", sensitive: true);
        await ClickAsync(LoginButton, "Login button");
    }

    public Task<string> ErrorTextAsync() => Error.InnerTextAsync();

    public Task AssertLoadedAsync() => Expect(LoginButton).ToBeVisibleAsync();

    public Task AssertErrorContainsAsync(string text) =>
        Expect(Error).ToContainTextAsync(text);

    public Task AssertPasswordMaskedAsync() =>
        Expect(Password).ToHaveAttributeAsync("type", "password");
}
