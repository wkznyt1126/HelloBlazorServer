using Bunit;
using HelloBlazorServer.Components.Shared;

namespace HelloBlazorServer.Tests;

public class CounterButtonTests : BunitContext
{
    [Fact]
    public void 初期表示は0とLabel()
    {
        var cut = Render<CounterButton>(p => p.Add(x => x.Label, "追加"));

        Assert.Equal("0", cut.Find(".count").TextContent);
        Assert.Equal("追加", cut.Find("button").TextContent);
    }

    [Fact]
    public void クリックでStep分増える()
    {
        var cut = Render<CounterButton>(p => p.Add(x => x.Step, 5));

        cut.Find("button").Click();

        Assert.Equal("5", cut.Find(".count").TextContent);
    }

    [Fact]
    public void Maxで止まりボタンが無効になる()
    {
        var cut = Render<CounterButton>(p => p
            .Add(x => x.Step, 3)
            .Add(x => x.Max, 5));

        cut.Find("button").Click(); // 3
        cut.Find("button").Click(); // 6 ではなく 5 で止まる

        Assert.Equal("5", cut.Find(".count").TextContent);
        Assert.True(cut.Find("button").HasAttribute("disabled"));
    }

    [Fact]
    public void クリックでOnCountChangedに新しい値が渡る()
    {
        int? received = null;
        var cut = Render<CounterButton>(p => p
            .Add(x => x.OnCountChanged, (int v) => received = v));

        cut.Find("button").Click();

        Assert.Equal(1, received);
    }
}