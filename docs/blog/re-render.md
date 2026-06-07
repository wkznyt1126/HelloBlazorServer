# Blazor Server で再描画が走る4つのタイミング

## はじめに

Blazor のレンダリングを調べると「ライフサイクル」の話が多く出てくるが、ここで整理したいのはそれとは少し違う。

**初回描画ではなく、すでに表示されているコンポーネントが再描画（re-render）されるタイミング**の話。

4つある。

-----

## 基本：親が再描画されると子も再描画されるかもしれない

まずこれを頭に入れておく。

親コンポーネントが再描画されると、その Razor マークアップが再評価される。その過程で子コンポーネントのパラメータも再評価され、子も再描画される可能性がある。

なので「なぜ子が再描画されたのか」を追うときは、まず親が再描画されていないかを確認するのが先になる。

-----

## 再描画が走る4つのトリガー

### 1. `StateHasChanged()` を呼ぶ

最も直接的なトリガー。

```csharp
private async Task DoSomething()
{
    // 何らかの処理
    await Task.Delay(1000);
    
    StateHasChanged(); // 明示的に再描画を要求
}
```

非同期処理の途中で UI を更新したいときによく使う。

なお、イベントハンドラの完了時は Blazor が自動で `StateHasChanged()` を呼ぶので、通常は明示的に書く必要はない。書く必要があるのは非同期処理の「途中」で UI を更新したい場合など。

-----

### 2. `@on` ディレクティブで C# の世界に入るやつ

```razor
<button @onclick="HandleClick">クリック</button>

@code {
    private void HandleClick()
    {
        // C# の世界に入った
        // このメソッドが完了したら Blazor が StateHasChanged() を自動で呼ぶ
    }
}
```

`@onclick`、`@oninput` などの `@on` ディレクティブでイベントハンドラを紐付けると、ブラウザのイベント発生時に C# のメソッドが実行される。

メソッドの完了後、Blazor が自動で再描画を走らせる。

-----

### 3. 子が `EventCallback` 経由で親のメソッドを呼ぶ

上の2番と似ているが、**誰が誰のイベントを起こすか**が違う。

```razor
<!-- 親コンポーネント -->
<ChildComponent OnChildAction="HandleChildAction" />

@code {
    private void HandleChildAction()
    {
        // 子から呼ばれる
        // 完了後に親が再描画される
    }
}
```

```razor
<!-- 子コンポーネント -->
@code {
    [Parameter]
    public EventCallback OnChildAction { get; set; }

    private async Task NotifyParent()
    {
        await OnChildAction.InvokeAsync(); // 親のメソッドを呼ぶ
    }
}
```

`EventCallback` は呼び出し元（親）の `StateHasChanged()` を自動で呼んでくれる。子が `InvokeAsync()` した結果として**親が**再描画される、という流れ。

2番との違いを一言でいうと：

- **2番**：HTML イベント → C# メソッド → 同じコンポーネントが再描画
- **3番**：子コンポーネント → 親の `EventCallback` → **親**が再描画

-----

## まとめ

|トリガー                                |誰が再描画されるか     |
|------------------------------------|--------------|
|親が再描画される                            |子も再描画されるかもしれない|
|`StateHasChanged()` を呼ぶ             |そのコンポーネント     |
|`@on` ディレクティブのイベントハンドラ完了            |そのコンポーネント     |
|子が `EventCallback.InvokeAsync()` を呼ぶ|親コンポーネント      |

「なぜ再描画されたのか分からない」というときは、この4つを起点に追っていくと原因が見つかりやすい。
