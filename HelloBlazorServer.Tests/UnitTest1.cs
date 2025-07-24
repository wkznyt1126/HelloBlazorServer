using Bunit;
using HelloBlazorServer.Components.Pages; // Counter.razorのnamespaceに合わせて調整
using Xunit;

namespace HelloBlazorServer.Tests;

public class UnitTest1 : TestContext
{
    [Fact]
    public void Counter初期値は0で表示される()
    {
        // Counterコンポーネントをレンダリング
        var cut = RenderComponent<Counter>();

        // 表示内容を検証
        Assert.Contains("Current count: 0", cut.Markup);
    }
}