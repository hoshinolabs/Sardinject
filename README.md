# Sardinject

![](https://img.shields.io/badge/unity-2022.3+-000.svg)
[![Releases](https://img.shields.io/github/release/hoshinolabs-vrchat/Sardinject.svg)](https://github.com/hoshinolabs-vrchat/Sardinject/releases)

The simple DI (Dependency Injection) for VRChat.

- **実行時の処理負荷無し:** ほとんどの処理はビルド時に解決されるため実行時に処理負荷がありません。
- **Udon上で実行可能:** 必要に応じてUdonランタイムで限定的に動作します。

## Features

- 柔軟なスコーピング設定
  - ヒエラルキ/コードによるスコープ
- IDパラメータ付きの注入
- 複数インスタンスの解決

## Documentation

View on [GitHub Pages](https://sardinject.github.io)

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
2. Open com.hoshinolabs.vrchat.sardinject-vx.x.x.unitypackage

## Basic Usage

次のような Udon があるとする。

```csharp
public class Sardine : UdonSharpBehaviour {
  public Hello() {
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
    var builder = new ContainerBuilder();
    builder.AddOnNewGameObject<Sardine>();
    builder.AddOnNewGameObject<StartupGreeting>();
    builder.Build();
  }
}
```

この例では、ゲームオブジェクトが自動的に作成され、コンポーネントが AddComponent され、 `StartupGreeting` クラスの sardine フィールドに `Sardine `クラスのインスタンスが自動的に設定されます。  
これにより、インスペクタから複雑な設定を手動で触る必要がなくなります。  
また、複雑なエディタ拡張を自分で作る必要もなくなります。

## Advanced Usage (dynamic resolve)

以下のようにして、型からインスタンスを動的に解決することができます。

```csharp
public class StartupGreeting : UdonSharpBehaviour {
  [Inject, SerializeField, HideInInspector]
  Container container;

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
    BuildContext.Push(builder => {
      builder.AddInHierarchy<Sardine>();
    });
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
      BuildContext.Push(builder => {
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
      BuildContext.Push(builder => {
        builder.AddInHierarchy<MySardine>();
      });
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
}
```

依存関係を記述したエディタスクリプトを作成します。

```csharp
public class Builder : IProcessSceneWithReport {
  public int callbackOrder => 0;

  public void OnProcessScene(Scene scene, BuildReport report) {
    var builder = new ContainerBuilder();
    builder.AddOnNewGameObject<BuildDateKeeper>();
    builder.Build();
  }
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
