namespace SauceDemo.Playwright.CSharp.Models;

public sealed record CheckoutCustomer(string FirstName, string LastName, string PostalCode)
{
    public static CheckoutCustomer ValidUkCustomer =>
        new("John", "Smith", "SW1A 1AA");
}
