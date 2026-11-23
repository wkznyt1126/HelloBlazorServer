using Microsoft.AspNetCore.Components;

namespace HelloBlazorServer.Components.Shared;

/// <summary>
/// クリックするたびに Step ずつ増えるカウンター。Max に達するとボタンが無効になる。
/// </summary>
public partial class CounterButton : ComponentBase
{
    /// <summary>ボタンに表示する文字。</summary>
    [Parameter] public string Label { get; set; } = "+1";

    /// <summary>1 回のクリックで増える量。</summary>
    [Parameter] public int Step { get; set; } = 1;

    /// <summary>上限。null なら上限なし。</summary>
    [Parameter] public int? Max { get; set; }

    /// <summary>値が変わったときに新しい値を通知する。</summary>
    [Parameter] public EventCallback<int> OnCountChanged { get; set; }

    public int Count { get; private set; }

    private bool IsMax => Max is int max && Count >= max;

    private async Task IncrementAsync()
    {
        if (IsMax)
        {
            return;
        }

        Count = Max is int max ? Math.Min(Count + Step, max) : Count + Step;
        await OnCountChanged.InvokeAsync(Count);
    }
}