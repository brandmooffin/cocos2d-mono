<div align="center">

![Cocos2D-Mono](https://raw.githubusercontent.com/Cocos2D-Mono/cocos2d-mono/master/Logos/logo-full-200.png)

### MonoGame powered built the cocos2d way!

[Check out the docs!](https://cocos2d-mono.dev)

</div>

# Cocos2D-Mono.Core

**Cocos2D-Mono without the MonoGame content-pipeline (MGCB) dependency.**

This is the same engine as [`Cocos2D-Mono`](https://www.nuget.org/packages/Cocos2D-Mono/) — the
runtime assembly (`Cocos2DMono.dll`) is identical. The only difference is that this package does
not bring in `MonoGame.Content.Builder.Task`, so nothing is added to your build for the content
pipeline.

**Use this package if** you don't use the MGCB content pipeline (for example, you load assets
yourself at runtime, or you build content by another route).

**Use [`Cocos2D-Mono`](https://www.nuget.org/packages/Cocos2D-Mono/) instead if** you do use the
content pipeline — that's the common case.

```xml
<PackageReference Include="Cocos2D-Mono.Core" Version="2.5.11" />
```

## One package, every platform

A single reference covers every supported platform; the right build is selected automatically
from your project's target framework:

| Target framework | Platform |
|---|---|
| `net9.0` | DesktopGL (Windows, Linux, macOS) |
| `net9.0-windows7.0` | WindowsDX |
| `net9.0-android35.0` | Android |
| `net9.0-ios18.0` | iOS |

This replaces the retired per-platform packages (`Cocos2D-Mono.Core.DesktopGL`,
`Cocos2D-Mono.Core.Windows`, and so on). Coming from those? See the
[migration guide](https://cocos2d-mono.dev/docs/guides/developing/migrating-to-2.5.11).

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
