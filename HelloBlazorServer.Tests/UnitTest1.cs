using Bunit;
using HelloBlazorServer.Components.Pages; // Counter.razorのnamespaceに合わせて調整
using Xunit;

namespace HelloBlazorServer.Tests;

public class UnitTest1 : TestContext
{
    [Fact]
    public void Counter初期値は0で表示される()
    {
        // Arrage
        var cut = RenderComponent<Counter>();

        // Assert
        Assert.Contains("Current count: 0", cut.Markup);
    }

    [Fact]
    public void ボタンクリックでカウントが1増える()
    {
        // Arrange
        var cut = RenderComponent<Counter>();

        // Act
        cut.Find("button").Click();

        // Assert
        Assert.Contains("Current count: 1", cut.Markup);
    }
}