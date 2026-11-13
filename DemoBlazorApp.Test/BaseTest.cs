namespace DemoBlazorAppTest.Tests;

using DemoBlazorApp.Test;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Diagnostics;

public abstract class BaseTest
{
    private static Executable? blazorApp;
    protected IPlaywright Playwright = null!;
    protected IBrowser Browser = null!;
    protected IBrowserContext Context = null!;
    protected IPage Page = null!;

    private string artifactsDir = Path.Combine("artifacts", DateTime.Now.ToString("yyyyMMdd_HHmmss"));

    [OneTimeSetUp]
    public async Task StartBlazorApp()
    {
        var appDir = UserCodeDirectory.FullDemoBlazorAppPath.Value;
        var exePath = Path.Combine(appDir, "DemoBlazorApp.exe");

        // The app runs from its build output (not a publish folder). Static web assets
        // (wwwroot, bootstrap, scoped CSS) are only resolved from the build manifest
        // in the Development environment.
        var environment = new Dictionary<string, string> { ["ASPNETCORE_ENVIRONMENT"] = "Development" };

        if (File.Exists(exePath))
        {
            // Start the apphost exe directly if present
            blazorApp = new Executable(exePath, appDir);
            await blazorApp.StartAsync("--urls=http://localhost:5000", environment);
        }
        else
        {
            // Fall back to starting the DLL via `dotnet`
            blazorApp = new Executable("dotnet", appDir);
            await blazorApp.StartAsync("DemoBlazorApp.dll --urls=http://localhost:5000", environment);
        }

        await WaitForBlazorToBeReady("http://localhost:5000");
    }

    [SetUp]
    public async Task Setup()
    {
        Directory.CreateDirectory(artifactsDir);

        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            RecordVideoDir = Path.Combine(artifactsDir, "videos"),
            RecordVideoSize = new() { Width = 1280, Height = 720 }
        });

        await Context.Tracing.StartAsync(new()
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });

        Page = await Context.NewPageAsync();
    }

    [OneTimeTearDown]
    public async Task StopBlazorAppAsync()
    {
        if (blazorApp != null)
            await blazorApp.StopAsync();
    }

    [TearDown]
    public async Task Cleanup()
    {
        var testName = TestContext.CurrentContext.Test.Name;
        var testFailed = TestContext.CurrentContext.Result.FailCount > 0;

        if (testFailed)
        {
            var screenshotPath = Path.Combine(artifactsDir, $"{testName}_screenshot.png");
            await Page.ScreenshotAsync(new() { Path = screenshotPath });

            var tracePath = Path.Combine(artifactsDir, $"{testName}_trace.zip");
            await Context.Tracing.StopAsync(new() { Path = tracePath });
        }
        else
        {
            await Context.Tracing.StopAsync();
        }

        await Browser.CloseAsync();
        Playwright.Dispose();
    }

    private async Task WaitForBlazorToBeReady(string url)
    {
        using var client = new HttpClient();

        for (int i = 0; i < 40; i++)
        {
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                    return;
            }
            catch { }

            await Task.Delay(500);
        }

        throw new Exception("Blazor app did not start in time.");
    }
}
