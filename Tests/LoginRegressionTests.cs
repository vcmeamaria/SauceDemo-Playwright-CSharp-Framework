using NUnit.Framework;

namespace SauceDemo.Playwright.CSharp.Tests;

[TestFixture]
[Category("Regression")]
[Category("Login")]
public sealed class LoginRegressionTests : BaseTest
{
    [Test, Category("TC-LOGIN-003")]
    public async Task InvalidUsernameIsRejected()
    {
        await LoginPage.OpenAsync(Settings.BaseUrl);
        await LoginPage.LoginAsync("invalid_user", "secret_sauce");
        await LoginPage.AssertErrorContainsAsync("Username and password do not match");
    }

    [Test, Category("TC-LOGIN-004")]
    public async Task InvalidPasswordIsRejected()
    {
        await LoginPage.OpenAsync(Settings.BaseUrl);
        await LoginPage.LoginAsync("standard_user", "invalid_password");
        await LoginPage.AssertErrorContainsAsync("Username and password do not match");
    }

    [TestCase("", "", "Username is required", TestName = "Empty credentials show username error")]
    [TestCase("standard_user", "", "Password is required", TestName = "Empty password shows password error")]
    [Category("TC-LOGIN-006")]
    [Category("TC-LOGIN-008")]
    public async Task RequiredFieldValidation(string username, string password, string expected)
    {
        await LoginPage.OpenAsync(Settings.BaseUrl);
        await LoginPage.LoginAsync(username, password);
        await LoginPage.AssertErrorContainsAsync(expected);
    }

    [Test, Category("TC-LOGIN-009")]
    public async Task LockedOutUserIsRejected()
    {
        var user = Settings.User("locked");
        await LoginPage.OpenAsync(Settings.BaseUrl);
        await LoginPage.LoginAsync(user.Username, user.Password);
        await LoginPage.AssertErrorContainsAsync("locked out");
    }

    [Test, Category("TC-LOGIN-010")]
    public async Task PasswordIsMasked()
    {
        await LoginPage.OpenAsync(Settings.BaseUrl);
        await LoginPage.AssertPasswordMaskedAsync();
    }

    [TestCase("' OR 1=1 --")]
    [TestCase("<script>alert(1)</script>")]
    [TestCase("!@#$%^&*")]
    [Category("TC-LOGIN-016")]
    [Category("TC-LOGIN-017")]
    [Category("TC-LOGIN-018")]
    public async Task MaliciousOrSpecialInputDoesNotAuthenticate(string input)
    {
        await LoginPage.OpenAsync(Settings.BaseUrl);
        await LoginPage.LoginAsync(input, input);
        await LoginPage.AssertErrorContainsAsync("Username and password do not match");
    }

    [Test, Category("TC-LOGIN-022")]
    public async Task InventoryCannotBeAccessedWithoutAuthentication()
    {
        await Page.GotoAsync(Settings.BaseUrl + "inventory.html");
        await LoginPage.AssertErrorContainsAsync("You can only access '/inventory.html' when you are logged in.");
    }
}
