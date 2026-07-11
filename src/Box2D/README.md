<div align="center">

![Cocos2D-Mono](https://raw.githubusercontent.com/Cocos2D-Mono/cocos2d-mono/master/Logos/logo-full-200.png)

### MonoGame powered built the cocos2d way!

[Check out the docs!](https://cocos2d-mono.dev)

</div>

# Cocos2D-Mono.Box2D

The **Box2D** physics engine — a managed C# port — used by
[Cocos2D-Mono](https://www.nuget.org/packages/Cocos2D-Mono/).

This package multi-targets every supported platform, so a single reference works everywhere:

| Target framework | Platform |
|---|---|
| `net9.0` | DesktopGL (Windows, Linux, macOS) |
| `net9.0-windows7.0` | WindowsDX |
| `net9.0-android35.0` | Android |
| `net9.0-ios18.0` | iOS |

## Do I need to reference this directly?

**Usually not.** `Cocos2D-Mono` (and `Cocos2D-Mono.Core`) already depend on this package, so
Box2D flows in transitively and `Box2D.dll` lands in your output automatically.

Reference it explicitly only if you want the physics port **without** the Cocos2D-Mono engine, or
you want to pin its version directly:

```xml
<PackageReference Include="Cocos2D-Mono.Box2D" Version="2.5.11" />
```

## Usage

The types live in the `Box2D.*` namespaces:

```csharp
using Box2D.Collision.Shapes;
using Box2D.Common;
using Box2D.Dynamics;

var world = new b2World(new b2Vec2(0f, -10f));
```

Cocos2D-Mono also ships helpers that bridge Box2D with the scene graph — see the
[Box2D sample](https://github.com/Cocos2D-Mono/cocos2d-mono-samples) for a worked example.

## Links

- [Documentation](https://cocos2d-mono.dev)
- [Source](https://github.com/Cocos2D-Mono/cocos2d-mono)
- [Samples](https://github.com/Cocos2D-Mono/cocos2d-mono-samples)
- [Issue tracker](https://github.com/Cocos2D-Mono/cocos2d-mono/issues)

## License

Cocos2D-Mono is licensed under **AGPL-3.0**, with a commercial license available for use in
closed-source or proprietary applications. See the
[LICENSE](https://github.com/Cocos2D-Mono/cocos2d-mono/blob/master/LICENSE) for full terms,
including how to obtain a commercial license.
