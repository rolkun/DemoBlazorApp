using Microsoft.Playwright;
using System.Text.RegularExpressions;

namespace DemoBlazorApp.Test.Pages;

public class HomePage : BasePage
{
    public HomePage(IPage page) : base(page) { }

    private ILocator Heading => Page.GetByRole(AriaRole.Heading);

    public async Task AssertHeadingContainsAsync(string text)
    {
        await Assertions
            .Expect(Heading)
            .ToHaveTextAsync(new Regex(text, RegexOptions.IgnoreCase));
    }

    public async Task GoToCounterAsync()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "Counter" }).ClickAsync();
    }
}
