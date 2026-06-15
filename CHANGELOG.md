# Changelog

All notable changes to Cocos2D-Mono are recorded here. This file was introduced with
2.5.10; earlier releases are described in their GitHub release notes / git history.
The project follows [Semantic Versioning](https://semver.org/) where practical.

## 2.5.10 - 2026-06-15

A modernization and hardening pass on the current runtime (.NET 9 / MonoGame 3.8.4.1).
It is almost entirely internal cleanup, correctness fixes, and removal of long-dead
platform code — no new public API. The .NET 10 / MonoGame 3.8.5 upgrade continues
separately on the `2.6.0` line and will ship once MonoGame 3.8.5 is stable.

### ⚠️ Breaking
- Removed the custom array pool `Cocos2D.ArrayPool<T>`. `CCRawList<T>` now pools through
  `System.Buffers.ArrayPool<T>.Shared`, and its public `UseArrayPool` option is unchanged.
  Code that referenced `Cocos2D.ArrayPool<T>` directly (uncommon) should switch to
  `System.Buffers.ArrayPool<T>`.

### Fixed
- `CCDirector.SharedDirector` lazy initialization is now thread-safe. Previously a race
  during concurrent first access could expose a partially-initialized director (a node
  built on another thread could cache a null action manager and throw).
- `CCInputState` now receives the correct per-frame delta time. It was passed
  `1 / ElapsedGameTime.Milliseconds` (the 0–999 millisecond component, inverted), which
  produced wrong values and `Infinity` when the component was 0.
- `CCTouchDelegate.DoesScriptHandlerExist` no longer throws `KeyNotFoundException` for an
  unregistered event type; it returns `false`.
- `CCRawList<T>.PackToCount` keeps the pooled buffer correctly owned and never leaves a
  zero-length backing array (which `Add`'s doubling growth could not expand).

### Performance
- `CCRawList<T>` array pooling is now backed by `System.Buffers.ArrayPool<T>` — about 21%
  faster with near-zero allocation on the pooled path.

### Removed (dead code)
- All legacy dead-platform conditional code: `WINDOWS_PHONE`, `XBOX`, `XBOX360`, `PSM`,
  `WP8`, `SILVERLIGHT`, `PCL`, and `NETFX_CORE` / `WINRT` / `WINDOWS_UWP` (UWP support is
  maintained in its own package).
- Dead files `Tuple.cs` and `b2FlagExtensions.cs`.
- Non-generic `System.Collections` usage (`ArrayList` / `Hashtable`) replaced with generics.

### Internal
- Added a unit-test suite (geometry, color, node, actions, scheduler, `CCRawList`) and a
  BenchmarkDotNet project for the perf-sensitive paths.
- Added an `.editorconfig` formatting and C# style baseline.
