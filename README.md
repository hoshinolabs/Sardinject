<p><img src="Graphics/logo.png" width="500"></p>

![](https://img.shields.io/badge/unity-2022.3+-000.svg)
[![Releases](https://img.shields.io/github/release/ikuko/Sardinject.svg)](https://github.com/ikuko/Sardinject/releases)

Sardinject is a simple DI (Dependency Injection) library for <a href="https://unity.com/">Unity C#</a>, <a href="https://udonsharp.docs.vrchat.com/">VRChat Udon(U#)</a>.  
  
Sardinject は <a href="https://unity.com/">Unity C#</a>、<a href="https://udonsharp.docs.vrchat.com/">VRChat Udon(U#)</a> 用のシンプルなDI(依存性注入)ライブラリです。

## Features

## Documentation

View on [GitHub Pages](https://ikuko.github.io/Sardinject/)

## Installation

*Unity 2022.3+ が必要です*

### VCC を利用したインストール

1. 次のページを開き、「Add to VCC」を押します。  
  [HoshinoLabs VPM Repository](https://vpm.hoshinolabs.com/)
2. VCCの「Manage Project」を押す。
3. `Sardinject` の横の「+」ボタンを押す。

### Install commandline (using VPM CLI)

```bash
vpm add repo https://vpm.hoshinolabs.com/vpm.json
cd /your-unity-project
vpm add com.hoshinolabs.sardinject
```

### Install manually (using .unitypackage)

1. Download the .unitypackage from [releases](https://github.com/ikuko/Sardinject/releases) page.
2. Open .unitypackage

### Install manually (UPM)

以下を UPM でインストールします。

```
https://github.com/ikuko/Sardinject.git?path=Packages/com.hoshinolabs.sardinject
```

Sardinject はリリースタグを使用するので以下のようにバージョンを指定できます。

```
https://github.com/ikuko/Sardinject.git?path=Packages/com.hoshinolabs.sardinject#1.0.0
```

## Basic Usage

次のような Udon があるとする。

```csharp
public class Sardine : UdonSharpBehaviour {
  public void Hello() {
    Debug.Log($"Hello. Do you like sardines?");
  }
}
```

```csharp
public class StartupGreeting : UdonSharpBehaviour {
  [Inject, SerializeField, HideInInspector]
  Sardine sardine;

  private void Start() {
    sardine.Hello();
  }
}
```

依存関係を記述したスクリプトを作成します。  
シーン上にオブジェクトを作成し `SceneScope` コンポーネントと `CustomInstaller` を追加します。

```csharp
public class CustomInstaller : MonoBehaviour, IInstaller {
  public void Install(ContainerBuilder builder) {
    builder.RegisterComponentOnNewGameObject<Sardine>(Lifetime.Cached);
    builder.RegisterEntryPoint<StartupGreeting>(Lifetime.Cached);
  }
}
```

この例では、`StartupGreeting` コンポーネントの sardine フィールドに `Sardine` コンポーネントが自動的に設定されます。  
`StartupGreeting` コンポーネントと `Sardine` コンポーネントは自動的にシーン上に追加されます。

## Advanced Usage (dynamic resolve)

以下のようにして、型からインスタンスを動的に解決することができます。

```csharp
public class Sardine : UdonSharpBehaviour {
  public void Hello() {
    Debug.Log($"Hello. Do you like sardines?");
  }
}
```

```csharp
public class StartupGreeting : UdonSharpBehaviour {
  [Inject, SerializeField, HideInInspector]
  Container container;

  private void Start() {
    var sardine = container.Resolve<Sardine>();
    sardine.Hello();
  }
}
```

依存関係を記述したスクリプトを作成します。  
シーン上にオブジェクトを作成し `SceneScope` コンポーネントと `CustomInstaller` を追加します。

```csharp
public class CustomInstaller : MonoBehaviour, IInstaller {
  public void Install(ContainerBuilder builder) {
    builder.RegisterComponentOnNewGameObject<Sardine>(Lifetime.Cached);
    builder.RegisterEntryPoint<StartupGreeting>(Lifetime.Cached);
  }
}
```

実行が開始されると `StartupGreeting` コンポーネントは動的に `Sardine` コンポーネントを取得します。  
この際、 `Sardine` コンポーネントが自動的に生成され返却されます。  
必要になるまで対象のオブジェクトを生成したくない場合に便利です。

## Advanced Usage (loose coupling between packages)

以下のコンポーネントを提供する `SomeonePackage` が存在するとします。

```csharp
namespace SomeonePackage {
  public class SomeoneSardine : UdonSharpBehaviour {
    public Hello() {
      Debug.Log($"Hello. What sardines are you?");
    }
  }
}
```

合わせて以下のスクリプトを作成します。
オブジェクトを作成し `ProjectScope` コンポーネントと作成したスクリプトを追加し、プレハブとして保存します。

```csharp
namespace SomeonePackage {
  public class CustomInstaller : MonoBehaviour, IInstaller {
    public void Install(ContainerBuilder builder) {
      builder.RegisterComponentOnNewGameObject<SomeoneSardine>(Lifetime.Transient);
    }
  }
}
```

あなたのパッケージから `SomeonePackage` の `SomeoneSardine` コンポーネントを使いたいとします。  

```csharp
namespace MyPackage {
  public class MySardine : UdonSharpBehaviour {
    [Inject, SerializeField, HideInInspector]
    SomeoneSardine sardine;

    private void Start() {
      sardine.Hello();
    }
  }
}
```

依存関係を記述したスクリプトを作成します。
`MySardine` コンポーネントと同じ場所に `SceneScope` コンポーネントと作成したスクリプトを追加します。

```csharp
namespace MyPackage {
  public class CustomInstaller : MonoBehaviour, IInstaller {
    public void Install(ContainerBuilder builder) {
      builder.RegisterComponentInHierarchy<MySardine>();
    }
  }
}
```

この例では、ビルド時にあなたの `MySardine` コンポーネントの sardine フィールドに新規オブジェクトとして生成された `SomeoneSardine` コンポーネントが設定されます。  
つまり、必要なプレハブをシーン上に配置したり、どのオブジェクトに必要なコンポーネントがアタッチされているかをエディタ画面で探すことなく、スクリプタブルに依存関係を設定することができます。  
ユーザーが手動で依存関係をエディタから設定したり、複雑なエディタ拡張機能を作成する必要はありません。

## Advanced Usage (call udon process at build time)

次のような Udon があるとする。

```csharp
public class BuildDateKeeper : UdonSharpBehaviour {
  [SerializeField, HideInInspector]
  string builddate;

  private void Start() {
    Debug.Log($"Build was made `{builddate}`.")
  }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
  [Inject]
  void CalledAtBuildTimeOnly() {
    builddate = DateTime.Now.ToString();
  }
#endif
}
```

合わせて以下のスクリプトを作成し `BuildDateKeeper` コンポーネントと同じ場所に `SceneScope` コンポーネントと作成したスクリプトを追加します。

```csharp
namespace MyPackage {
  public class CustomInstaller : MonoBehaviour, IInstaller {
    public void Install(ContainerBuilder builder) {
      builder.RegisterComponentInHierarchy<BuildDateKeeper>();
    }
  }
}
```

この例では `BuildDateKeeper` の builddate フィールドににビルド日時が埋め込まれます。  
埋め込みに使用される `CalledAtBuildTimeOnly()` メソッドはアップロードされたワールドデータには含まれないため、無駄なアップロードサイズや実行時の負荷はありません。

## Credits

- [VContainer](https://github.com/hadashiA/VContainer)  
システム全体の設計を参考にさせて頂きました。

## Author

[@ikuko](https://twitter.com/magi_ikuko)

## License

MIT
