<p><img src="Graphics/logo.png" width="500"></p>

![](https://img.shields.io/badge/unity-2022.3+-000.svg)
[![Releases](https://img.shields.io/github/release/hoshinolabs/Sardinject.svg)](https://github.com/hoshinolabs/Sardinject/releases)

Sardinject is a simple DI (Dependency Injection) library for <a href="https://udonsharp.docs.vrchat.com/">VRChat Udon#</a>.  
  
Sardinject は <a href="https://udonsharp.docs.vrchat.com/">VRChat Udon#</a> 用のシンプルなDI(依存性注入)ライブラリ。

## Features

- IDパラメータ付きの注入

## Documentation

~~View on [GitHub Pages](https://sardinject.github.io)~~

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
vpm add com.hoshinolabs.vrchat.sardinject
```

### Install manually (using .unitypackage)

1. Download the .unitypackage from [releases](https://github.com/hoshinolabs-vrchat/Sardinject/releases) page.
2. Open .unitypackage

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

依存関係を記述したエディタスクリプトを作成します。

```csharp
public class Builder : IProcessSceneWithReport {
  public int callbackOrder => 0;

  public void OnProcessScene(Scene scene, BuildReport report) {
    var context = new Context();
    context.Enqueue(builder => {
      builder.AddOnNewGameObject<Sardine>(Lifetime.Cached);
      builder.AddOnNewGameObject<StartupGreeting>(Lifetime.Cached);

      builder.AddEntryPoint<StartupGreeting>();
    });
    context.Build();
  }
}
```

この例では、ゲームオブジェクトが自動的に作成され、コンポーネントが AddComponent され、 `StartupGreeting` クラスの sardine フィールドに `Sardine` クラスのインスタンスが自動的に設定されます。  
これにより、インスペクタから複雑な設定を手動で触る必要がなくなります。  
また、複雑なエディタ拡張を自分で作る必要もなくなります。

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
  IContainer container;

  private void Start() {
    var sardine = (Sardine)container.Resolve(GetUdonTypeName<Sardine>());
    sardine.Hello();
  }
}
```

エディタースクリプトで、以下のように特別なコンテキストに登録しておきます。

```csharp
public class Builder : IProcessSceneWithReport {
  public int callbackOrder => 0;

  public void OnProcessScene(Scene scene, BuildReport report) {
    var context = new Context();
    context.Enqueue(builder => {
      builder.AddOnNewGameObject<Sardine>(Lifetime.Cached);
      builder.AddOnNewGameObject<StartupGreeting>(Lifetime.Cached);

      builder.AddEntryPoint<StartupGreeting>();
    });
    context.Build();
  }
}
```

## Advanced Usage (loose coupling between packages)

以下のコンポーネントを提供する `SomeonePackage` が存在するとします。

```csharp
namespace SomeonePackage {
  public class SomeoneSardine : UdonSharpBehaviour {
    public Hello() {
      Debug.Log($"Hello. What sardines are you?");
    }
  }

#if UNITY_EDITOR
  public class Builder : IProcessSceneWithReport {
    public int callbackOrder => 0;

    public void OnProcessScene(Scene scene, BuildReport report) {
      ProjectContext.Enqueue(builder => {
        builder.AddInHierarchy<SomeoneSardine>();
      });
    }
  }
#endif
}
```

あなたのパッケージから `SomeonePackage` のコンポーネントを使いたいとします。

```csharp
namespace MyPackage {
  public class MySardine : UdonSharpBehaviour {
    [Inject, SerializeField, HideInInspector]
    SomeoneSardine sardine;

    private void Start() {
      sardine.Hello();
    }
  }

#if UNITY_EDITOR
  public class Builder : IProcessSceneWithReport {
    public int callbackOrder => 0;

    public void OnProcessScene(Scene scene, BuildReport report) {
      var context = ProjectContext.New();
      context.Enqueue(builder => {
        builder.AddInHierarchy<MySardine>();
      });
      context.Build();
    }
  }
#endif
}
```

この例では、自動的にあなたの `MySardine` コンポーネントの sardine フィールドに相手の `SomeoneSardine` が設定されます。  
つまり、どのオブジェクトに必要なコンポーネントがアタッチされているかをエディタ画面で探すことなく、簡単に依存関係を設定することができます。  
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

#if UNITY_EDITOR
  [Inject]
  void CalledAtBuildTime() {
    builddate = DateTime.Now.ToString();
  }
#endif

#if UNITY_EDITOR
  public class Builder : IProcessSceneWithReport {
    public int callbackOrder => 0;

    public void OnProcessScene(Scene scene, BuildReport report) {
      var context = new Context();
      context.Enqueue(builder => {
        builder.AddInHierarchy<BuildDateKeeper>();
      });
      context.Build();
    }
  }
#endif
}
```

この例では、 `BuildDateKeeper` の builddate フィールドににビルド日時が埋め込まれます。  
埋め込みに使用される `CalledAtBuildTime()` メソッドはアップロードされたワールドデータには含まれないため、無駄なアップロードサイズや実行時の負荷はありません。

## Credits

Sardinject is inspired by:

- [VContainer](https://github.com/hadashiA/VContainer)

## Author

[@ikuko](https://twitter.com/magi_ikuko)

## License

MIT
