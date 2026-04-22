using Microsoft.Playwright;
using System.Text.RegularExpressions;

namespace DemoBlazorApp.Test.Pages;

public class CounterPage : BasePage
{
    public CounterPage(IPage page) : base(page) { }

    private ILocator Status => Page.GetByRole(AriaRole.Status);
    private ILocator Button => Page.GetByRole(AriaRole.Button, new() { Name = "Click me" });

    public async Task AssertCountAsync(int expected)
    {
        await Assertions
            .Expect(Status)
            .ToHaveTextAsync(new Regex($"Current count: {expected}"));
    }

    public async Task ClickIncrementAsync()
    {
        await Button.ClickAsync();
    }
}
