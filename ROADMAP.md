# Cocos2D-Mono Roadmap

Cocos2D-Mono is an actively maintained 2D game framework for .NET, built on MonoGame. This document outlines the direction of the project at a high level. Priorities and timing may shift, and feedback is welcome through the [issue tracker](https://github.com/Cocos2D-Mono/cocos2d-mono/issues).

## Guiding principles

- **Incremental and stable.** Improvements land in small, reviewable steps. We avoid large, disruptive rewrites.
- **Compatibility first.** Where practical, public API changes go through `[Obsolete]` deprecation cycles. Breaking changes are signaled by a major version bump and documented with migration notes.
- **Current platform.** Track current .NET and MonoGame releases.

## Themes

### Build & packaging
Simplify the project and packaging layout — fewer, clearer NuGet packages with centralized, consistent dependency versions — so the framework is easier to consume and maintain. This has landed: the build is consolidated into multi-targeted projects, and the next release ships as three packages (`Cocos2D-Mono`, `Cocos2D-Mono.Core`, `Cocos2D-Mono.Box2D`) replacing the per-platform package line (final per-platform release: 2.5.10). Migration notes live in the README.

### Quality & testing
Grow automated test coverage (math primitives, actions, scheduling, serialization) and adopt code analyzers, so changes stay safe and regressions are caught early. A unit-test suite now gates changes in CI and keeps growing alongside the work; a benchmark project supports performance decisions on demand.

### Modern C#
Adopt current language features — nullable reference types for null-safety, up-to-date syntax, and value-type discipline for the math types — improving correctness and readability.

### API & architecture *(longer-term, exploratory)*
Evolve toward a more composable, testable design — composition over deep inheritance, optional dependency injection, and async content loading — while preserving familiar entry points. This work is exploratory and will be staged carefully.

### Platforms
Continue supporting desktop (Windows, macOS, Linux / DesktopGL), Android, and iOS. UWP / Xbox-UWP support is maintained separately in the [Cocos2D-Mono.UWP](https://github.com/Cocos2D-Mono/Cocos2D-Mono.UWP) repository.

## Status at a glance

| Theme | Status |
|---|---|
| Quality & testing | In place and growing |
| Build & packaging | Landed — ships with the next release |
| Modern C# | Next up |
| API & architecture | Exploratory |
| Platforms | Maintained |

---

*This roadmap is a living document, not a commitment to specific features or dates.*
