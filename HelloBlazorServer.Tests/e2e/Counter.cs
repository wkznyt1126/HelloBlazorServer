using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Threading.Tasks;

namespace PlaywrightTests;

public class CounterE2ETests : PageTest
{

    private string TopPage = "http://localhost:5180";
    
    // テストごとにトレース残すようの処理
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

    // テストごとにトレース残すようの処理その2
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

    // テストケース
    [Test]
    public async Task Counterページへの遷移と加算減算()
    {
        await Page.GotoAsync(TopPage);

        // ページ遷移
        await Page.GetByRole(AriaRole.Navigation).GetByText("Counter").ClickAsync();

        // 初期値の検証
        await Expect(Page.GetByRole(AriaRole.Status)).ToContainTextAsync("Current count: 0");

        // 加算
        await Page.GetByRole(AriaRole.Button, new() { Name = "+1" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Status)).ToContainTextAsync("Current count: 1");

        // 減算
        await Page.GetByRole(AriaRole.Button, new() { Name = "-1" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Status)).ToContainTextAsync("Current count: 0");
    }
}