using Microsoft.Playwright;

namespace DemoBlazorApp.Test.Pages;

public abstract class BasePage
{
    protected readonly IPage Page;

    protected BasePage(IPage page)
    {
        Page = page;
    }

    public async Task NavigateAsync(string url)
    {
        await Page.GotoAsync(url);
    }
}
