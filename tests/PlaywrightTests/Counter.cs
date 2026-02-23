using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Threading.Tasks;

namespace PlaywrightTests;

public class CounterE2ETests : PageTest
{

    [SetUp]
    public async Task Setup()
    {
        await Context.Tracing.StartAsync(new()
        {
            Title = $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}",
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    [TearDown]
    public async Task TearDown()
    {
        await Context.Tracing.StopAsync(new()
        {
            Path = Path.Combine(
                TestContext.CurrentContext.WorkDirectory,
                "playwright-traces",
                $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.zip"
            )
        });
    }

    [Test]
    public async Task Counter_Increments_And_Decrements()
    {
        // アプリが http://localhost:5180 で動作している前提
        await Page.GotoAsync("http://localhost:5180");

        // メニューから Counter をクリックして遷移
        await Page.ClickAsync("nav >> text=Counter");
        await Page.WaitForSelectorAsync("p[role='status']");

        // 初期値確認
        var status = await Page.InnerTextAsync("p[role='status']");
        Assert.That(status, Does.Contain("Current count: 0"));

        // インクリメント
        await Page.ClickAsync("button:has-text(\"Click me\")");
        await Page.WaitForFunctionAsync("() => document.querySelector('p[role=\\'status\\']')?.textContent.includes('Current count: 1')");
        status = await Page.InnerTextAsync("p[role='status']");
        Assert.That(status, Does.Contain("Current count: 1"));

        // デクリメント
        await Page.ClickAsync("button:has-text(\"引く(JS)\")");
        await Page.WaitForFunctionAsync("() => document.querySelector('p[role=\\'status\\']')?.textContent.includes('Current count: 0')");
        status = await Page.InnerTextAsync("p[role='status']");
        Assert.That(status, Does.Contain("Current count: 0"));
    }
}