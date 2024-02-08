# Sardinject

![](https://img.shields.io/badge/unity-2022.3+-000.svg)
[![Releases](https://img.shields.io/github/release/hoshinolabs-vrchat/Sardinject.svg)](https://github.com/hoshinolabs-vrchat/Sardinject/releases)

The simple DI (Dependency Injection) for VRChat.

- **No CPU time required at runtime:**: No CPU time is needed at runtime as most processing is resolved in the editor.
- **Runs on Udon Runtime:**: Limited operation at Udon runtime if needed.

## Features

- Flexible scoping
  - Scoping by Hierarchy or Code
- Inject with ID parameter
- Multiple instance Resolve

## Documentation

View on [GitHub Pages](https://sardinject.github.io)

## Installation

*Requires Unity 2022.3+*

### Install via VCC

1. Open the next page and press "Add to VCC  
  [HoshinoLabs VPM Repository](https://vpm.hoshinolabs.com/)
2. Press "Manage Project" in VCC
3. Press the + button next to `Sardinject`

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

Suppose we have the following Udon.

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

Create an editor script with written dependencies.

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

In this example, the game object is automatically created, the component is AddComponent, and then an instance of the Sardine class is automatically set in the sardine field of the StartupGreeting class.  
This eliminates the need to manually touch complex settings from the inspector.  
It also eliminates the need to create complex editor extensions yourself.

## Advanced Usage (dynamic resolve)

You can dynamically resolve instances from types by doing the following.

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

Editor scripts should be registered in a special context as follows.

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

Suppose there exists an SomeonePackage that provides the following components.

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

Suppose you want to use someone else's component from your package.

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

In this example, your MySardine component is automatically set to your partner's SomeoneSardine.
This means that you can effortlessly set up a relationship without having to search the editor screen to find which object has the desired component attached.  
Separate packages do not require users to manually configure or use complex editor extensions.

## Advanced Usage (call udon process at build time)

Suppose we have the following Udon.

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

Create an editor script with written dependencies.

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

This will embed the build date and time in BuildDateKeeper.  
The CalledAtBuildTime() method used for embedding is excluded from the uploaded world data, so there is no load at run-time.

## Credits

Sardinject is inspired by:

- [VContainer](https://github.com/hadashiA/VContainer)

## Author

[@ikuko](https://twitter.com/magi_ikuko)

## License

MIT
