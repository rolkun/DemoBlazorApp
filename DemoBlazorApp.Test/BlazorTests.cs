namespace DemoBlazorAppTest.Tests;

using DemoBlazorApp.Test.Pages;
using Microsoft.Playwright;
using NUnit.Framework;

[TestFixture]
public class BlazorTests : BaseTest
{
    [Test]
    public async Task HomePage_Shows_HelloWorld()
    {
        var home = new HomePage(Page);

        await home.NavigateAsync("http://localhost:5000");
        await home.AssertHeadingContainsAsync("Hello");
    }

    [Test]
    public async Task Counter_Increments()
    {
        var home = new HomePage(Page);
        var counter = new CounterPage(Page);

        await home.NavigateAsync("http://localhost:5000");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle); 
        await home.GoToCounterAsync();

        await counter.AssertCountAsync(0);
        await counter.ClickIncrementAsync();
        await counter.AssertCountAsync(1);
    }
}
