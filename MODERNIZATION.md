# cocos2d-mono Modernization Plan

A reference document describing how the framework will be modernized over a series of phases. Phases are ordered by risk-adjusted return: the lowest-risk infrastructure work first, the highest-payoff (and most breaking) architecture work last.

---

## Table of contents

1. [Codebase snapshot](#codebase-snapshot)
2. [Phase 1 — Project structure consolidation](#phase-1--project-structure-consolidation)
3. [Phase 1.5 — Fork `Cocos2D-Mono.UWP` as the source-of-truth for `Cocos2D-Mono.Uwp`](#phase-15--fork-cocos2d-monouwp-as-the-source-of-truth-for-cocos2d-monouwp)
4. [Phase 2 — Dead code, dead conditionals, BCL replacements](#phase-2--dead-code-dead-conditionals-bcl-replacements)
5. [Phase 3 — Language-level modernization](#phase-3--language-level-modernization)
6. [Phase 4 — Architectural refactor](#phase-4--architectural-refactor)
7. [Phase 5 — Quality infrastructure](#phase-5--quality-infrastructure)
8. [Phase 6 — Forward-looking platform decisions](#phase-6--forward-looking-platform-decisions)
9. [Suggested ordering and effort](#suggested-ordering-and-effort)

---

## Codebase snapshot

Findings as of the start of the modernization effort.

| Area | Observation |
|---|---|
| Runtime / SDK | .NET 9 already, MonoGame 3.8.4.1 centralized in `Directory.Build.props` |
| Source size | ~460 `.cs` files, ~44k LOC under `cocos2d/` |
| Project sprawl | 12 platform-specific csprojs in `cocos2d/`, 5 in `box2d/`, 12 in `Tests/` (≈ 30 csprojs total) all sharing one `cocos2d.projitems` |
| Naming style | ~4,400 hits for `m_p…/m_b…/m_f…/m_n…/m_s…` C++-style fields |
| Dead conditionals | `WINDOWS_PHONE`, `XBOX`, `PSM`, `WP8`, `SILVERLIGHT` `#if` blocks in 13 files |
| Reinvented BCL | `Tuple.cs`, `CCRawList<T>`, `CCObjectPool`, `CCArrayPool`, custom positional text serializer in `support/Serialization/` |
| Non-generic collections | `System.Collections` (`ArrayList`/`Hashtable`) usage in 10+ files including `CCNode`, `CCDirector`, `CCActionManager` |
| Static state | `CCDirector.SharedDirector`, `CCApplication.SharedApplication`, `CCContentManager.SharedContentManager`, `CCFontManager` (static class), singleton `CCScheduler` / `CCActionManager` / `CCTouchDispatcher` |
| Inheritance | `CCNode` implements 7 interfaces, holds `m_pChildren` + manually-synced `m_pChildrenByTag` index |
| Public mutable fields | `CCNode.m_sTransform`, `CCRawList<T>.Elements` |
| CI | 13 GitHub Actions workflow files, one per platform × Core/non-Core variant |
| Preview branch | `release/2.6.0-preview` exists on upstream. Structurally identical to `dev` (same 30 csprojs, same `.projitems`); only changes are `MonoGameVersion 3.8.4.1 → 3.8.5-preview.2`, every TFM `net9.0* → net10.0*`, workflows `dotnet-version 9.0.x → 10.0.x`, and `global.json` deletion. Periodically refreshed via `Merge dev → release/2.6.0-preview`. |
| UWP / Xbox surface | The UWP `csproj` was removed from `dev` some time ago. However, 59 `#if NETFX_CORE` blocks across 23 files still survive as dormant code (top: `CCApplication.cs` 7, `cocoa/CCGeometry.cs` 6, `support/CCUserDefault.cs` 6, `platform/PList/PlistDocument.cs` 6, `CCDirector.cs` 5, `platform/CCAccelerometer.cs` 5). Nothing currently defines `NETFX_CORE` in any csproj, so these paths compile to nothing today. |

The `Core` vs non-`Core` packaging split appears to be purely about whether `MonoGame.Content.Builder.Task` (and a couple of native deps) are baked in. The shared code is identical via `.projitems`.

---

## Phase 1 — Project structure consolidation

> **Goal:** Reduce ~30 csprojs to ~3, eliminate `.projitems`/`.shproj`, centralize package versions, halve the CI matrix while preserving all output packages.
>
> **Risk:** Low. No code changes; only build/packaging plumbing.
>
> **Estimated effort:** 1–2 weeks.
>
> **Breaking?** No, if NuGet package IDs stay the same.

### Rollout strategy: 1a → 1b → 1c → 1d

Phase 1 ships as four commits to keep blast radius bounded and the legacy build available as a safety net:

| Sub-phase | What changes | Reversible? |
|---|---|---|
| **1a** *(this PR)* | **Add** new files under `src/`, new `Cocos2DMono.sln`, new matrix `.github/workflows/build.yml`. Legacy build is untouched. New build runs **side-by-side** with legacy. TFMs are parameterized via `Cocos2DBaseTfm`/`Cocos2DWindowsTfm`/`Cocos2DAndroidTfm`/`Cocos2DIosTfm` properties so a future .NET / MonoGame bump is a four-property edit. | Yes — delete `src/`, the new `.sln`, the new workflow. Legacy untouched. |
| **1b** | Consolidate `Tests/` into `tests/Cocos2DMono.IntegrationTests/`. Includes iOS/Android resource moves and a new iOS `AppDelegate.cs` to replace the `#if IPHONE` branch of `Tests/cocos2d-mono.Tests/Program.cs`. | Mostly — `git mv` history is preserved. |
| **1c** | **Delete** legacy: 12 cocos2d csprojs, 4 box2d csprojs, 13 tests csprojs, 6 projitems/shproj, 4 AssemblyInfo files, 13 legacy CI workflows, the vestigial `cocos2d/external lib/ICSharpCodeSource/ICSharpCode.SharpZLib.WP8.csproj`. Promote `src/Directory.Packages.props` and `src/Directory.Build.props` to the repo root. | After this commit `cocos2d-mono.All.sln` is deleted; the new `Cocos2DMono.sln` is canonical. |
| **1d** | **Reconcile `release/2.6.0-preview`** with the consolidated layout. Tiny PR against `release/2.6.0-preview`: bump the four `Cocos2D*Tfm` properties in `Directory.Build.props` from `net9.0*` → `net10.0*`, bump `MonoGameVersion` to `3.8.5-preview.*`, bump CI matrix `dotnet-version` to `10.0.x`. No csproj edits required because of the TFM parameterization in 1a. Replaces the old "preview-branch maintenance" pattern of editing 30 csprojs per .NET bump. | Yes — revert the props edit. |

### 1a — Status

Completed on branch `modernization/phase-1-project-consolidation`. Files added:

| File | Purpose |
|---|---|
| `src/Directory.Packages.props` | Central Package Management for new csprojs. Reconciles version inconsistencies (`SharpZipLib` 1.3.3→1.4.2, `System.Drawing.Common` 5.0.3/8.0.8→8.0.11). Scoped to `src/` so legacy projects are unaffected. |
| `src/Directory.Build.props` | Shared metadata: `LangVersion=latest`, deterministic builds, NuGet metadata, source-link/symbol packages. Inherits `MonoGameVersion` from the repo-root `Directory.Build.props`. Defines the parameterized TFM dials (`Cocos2DBaseTfm`, `Cocos2DWindowsTfm`, `Cocos2DAndroidTfm`, `Cocos2DIosTfm`, and `Cocos2DTargetFrameworks` derived from them). |
| `src/Cocos2DMono/Cocos2DMono.csproj` | Multi-targeted (`net9.0;net9.0-windows7.0;net9.0-android35.0;net9.0-ios18.0`). Replaces 12 legacy `cocos2d.{Platform}` + `cocos2d.Core.{Platform}` csprojs. Globs `..\..\cocos2d\**\*.cs` and applies platform exclusions for the small set of `-Platform.cs` files. |
| `src/Box2D/Box2D.csproj` | Same TFM set. Replaces 4 legacy `box2d.{Platform}` csprojs. |
| `Cocos2DMono.sln` | New solution referencing only `src/*`. Coexists with the legacy `cocos2d-mono.All.sln`. |
| `.github/workflows/build.yml` | Single matrix workflow covering all 4 TFMs × `[Debug, Release]`. Runs alongside the 13 legacy `*_build.yml` workflows during the transition. |

**Local verification on Windows host**:

| TFM | `dotnet restore` | `dotnet build` | `dotnet pack` |
|---|---|---|---|
| `net9.0` | ✅ | ✅ 0 errors, 62 pre-existing warnings | ✅ `Cocos2D-Mono.Box2D.3.0.0-alpha.1.nupkg` produced |
| `net9.0-windows7.0` | ✅ | ✅ 0 errors, 46 pre-existing warnings | — |
| `net9.0-android35.0` | ⏭ requires Android workload — defer to CI | | |
| `net9.0-ios18.0` | ⏭ requires macOS host + iOS workload — defer to CI | | |

The 46–62 build warnings are all pre-existing code-style issues (Hungarian-style field shadowing, Skia API deprecations, obsolete MonoGame methods) carried over from the legacy build verbatim. Phase 3 will clear them as part of the language modernization pass.

### 1a — Corrections to the initial design

Three corrections were applied based on adversarial review and verification:

1. **`SharpZipLib` NuGet kept** *(not dropped as the draft spec proposed)*. `cocos2d/particle_nodes/CCParticleSystem.cs:7` imports `ICSharpCode.SharpZipLib.Zip` from the NuGet. The vendored copy under `cocos2d/external lib/ICSharpCodeSource/` was **never compiled** by the legacy build (`cocos2d.projitems` has no Include for it). Dropping the NuGet would have required compiling the WP8-era vendored source on net9.0/Android/iOS — unvalidated. Resolution: keep the NuGet, and exclude `external lib/**` from the new csproj's compile glob.
2. **`net9.0-windows7.0` `DefineConstants` matches legacy exactly: `WINDOWS;XNA` (no `MONOGAME`, no `WINDOWSDX`)**. The legacy `cocos2d.Windows.csproj` defined only `WINDOWS;XNA`, treating the Windows build as the "XNA-compatible" path. `CCContentManager.ReloadGraphicsAssets` is `protected` under `#if MONOGAME` and `public` under `#else`, so defining `MONOGAME` here breaks the call in `CCDrawManager.cs:588`. Phase 2 will reconcile this divergence properly.
3. **Mac Catalyst dropped from Phase 1**. MonoGame 3.8.4.1 has no `maccatalyst` asset. macOS desktop is served via the `net9.0` DesktopGL TFM as it was in legacy. Reintroducing Mac Catalyst is a Phase 6 question (render-backend bet).

### 1a — Acceptance criteria (this PR)

- [x] `src/Directory.Packages.props` and `src/Directory.Build.props` authored.
- [x] `src/Cocos2DMono/Cocos2DMono.csproj` authored and builds clean on `net9.0` + `net9.0-windows7.0`.
- [x] `src/Box2D/Box2D.csproj` authored and builds clean on `net9.0` + `net9.0-windows7.0`.
- [x] `Cocos2DMono.sln` referencing only `src/*` projects.
- [x] Matrix CI workflow at `.github/workflows/build.yml`.
- [x] Local `dotnet pack` produces a versioned `.nupkg` + `.snupkg`.
- [x] Legacy build untouched and reversible.
- [ ] Mobile TFMs (`net9.0-android35.0`, `net9.0-ios18.0`) verified in CI.

### 1.1 Why the current layout exists

The `*.projitems` + per-platform-csproj pattern dates from the pre-SDK-style era when:
- Different platforms required different MSBuild SDKs.
- Multi-targeting wasn't ergonomic (`<TargetFrameworks>` matrix evaluation in conditionals was buggy).
- Per-platform `<DefineConstants>` was easier to manage by having one project per platform.

None of those constraints hold in .NET 9. SDK-style projects support multi-TFM, conditional `<DefineConstants>`, and conditional `<PackageReference>` cleanly.

### 1.2 Target layout

```
/
├── Directory.Build.props          (MonoGame version; widened to common metadata)
├── Directory.Build.targets        (NEW — common build/pack steps)
├── Directory.Packages.props       (NEW — central package management)
├── global.json                    (existing)
├── Cocos2DMono.sln                (NEW — single solution; replaces cocos2d-mono.All.sln)
│
├── src/
│   ├── Cocos2DMono/
│   │   ├── Cocos2DMono.csproj     (multi-targeted: net9.0; net9.0-android; net9.0-ios; net9.0-maccatalyst; net9.0-windows)
│   │   ├── (all current cocos2d/*.cs source moved here, folder structure preserved)
│   │   └── Properties/AssemblyInfo.cs (auto-generated)
│   │
│   └── Box2D/
│       ├── Box2D.csproj           (multi-targeted)
│       └── (current box2d source)
│
├── tests/
│   ├── Cocos2DMono.IntegrationTests/        (the existing visual test runner)
│   │   └── Cocos2DMono.IntegrationTests.csproj  (multi-targeted)
│   └── Cocos2DMono.UnitTests/               (NEW — added in Phase 5)
│       └── Cocos2DMono.UnitTests.csproj
│
├── samples/                       (future home for templates / demos)
├── docs/                          (existing docfx)
└── .github/workflows/
    ├── build.yml                  (matrix-driven; replaces 13 per-platform yml files)
    └── publish.yml                (NuGet publish on tag)
```

### 1.3 Single multi-targeted csproj — sketch

The shape of `src/Cocos2DMono/Cocos2DMono.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net9.0;net9.0-windows7.0;net9.0-android35.0;net9.0-ios18.0;net9.0-maccatalyst</TargetFrameworks>
    <RootNamespace>Cocos2D</RootNamespace>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <Nullable>enable</Nullable>           <!-- after Phase 3 -->
    <LangVersion>latest</LangVersion>
    <Deterministic>true</Deterministic>
    <GeneratePackageOnBuild>false</GeneratePackageOnBuild>
    <PackageId>Cocos2D-Mono</PackageId>   <!-- single canonical package -->
    <Version>3.0.0</Version>              <!-- bumped to signal consolidation -->
  </PropertyGroup>

  <!-- Per-TFM define constants -->
  <PropertyGroup Condition="'$(TargetFramework)' == 'net9.0'">
    <DefineConstants>$(DefineConstants);DESKTOPGL;OPENGL;MONOGAME</DefineConstants>
  </PropertyGroup>
  <PropertyGroup Condition="'$(TargetFramework)' == 'net9.0-windows7.0'">
    <DefineConstants>$(DefineConstants);WINDOWS;DIRECTX;MONOGAME</DefineConstants>
    <UseWindowsForms>true</UseWindowsForms>
  </PropertyGroup>
  <PropertyGroup Condition="'$(TargetFramework)' == 'net9.0-android35.0'">
    <DefineConstants>$(DefineConstants);ANDROID;OPENGL;GLES;MONOGAME</DefineConstants>
    <SupportedOSPlatformVersion>21.0</SupportedOSPlatformVersion>
  </PropertyGroup>
  <PropertyGroup Condition="'$(TargetFramework)' == 'net9.0-ios18.0'">
    <DefineConstants>$(DefineConstants);IOS;__IOS__;__MOBILE__;__UNIFIED__;GLES;OPENGL;MONOGAME</DefineConstants>
    <SupportedOSPlatformVersion>11.0</SupportedOSPlatformVersion>
  </PropertyGroup>
  <PropertyGroup Condition="'$(TargetFramework)' == 'net9.0-maccatalyst'">
    <DefineConstants>$(DefineConstants);MACOS;OPENGL;MONOGAME</DefineConstants>
  </PropertyGroup>

  <!-- Common deps -->
  <ItemGroup>
    <PackageReference Include="MonoGame.Content.Builder.Task" />
    <PackageReference Include="SharpZipLib" />
    <ProjectReference Include="..\Box2D\Box2D.csproj" />
  </ItemGroup>

  <!-- TFM-specific MonoGame variant -->
  <ItemGroup Condition="'$(TargetFramework)' == 'net9.0'">
    <PackageReference Include="MonoGame.Framework.DesktopGL" />
    <PackageReference Include="OpenTK" />
    <PackageReference Include="SkiaSharp" />
    <PackageReference Include="SkiaSharp.NativeAssets.Linux" />
    <PackageReference Include="SkiaSharp.NativeAssets.macOS" />
    <PackageReference Include="System.Drawing.Common" />
  </ItemGroup>
  <ItemGroup Condition="'$(TargetFramework)' == 'net9.0-windows7.0'">
    <PackageReference Include="MonoGame.Framework.WindowsDX" />
    <PackageReference Include="BitMiracle.LibTiff.NET" />
    <PackageReference Include="System.Drawing.Common" />
  </ItemGroup>
  <ItemGroup Condition="'$(TargetFramework)' == 'net9.0-android35.0'">
    <PackageReference Include="MonoGame.Framework.Android" />
    <PackageReference Include="OpenTK" />
  </ItemGroup>
  <ItemGroup Condition="'$(TargetFramework)' == 'net9.0-ios18.0'">
    <PackageReference Include="MonoGame.Framework.iOS" />
    <PackageReference Include="OpenTK" />
  </ItemGroup>

  <!-- Per-platform source exclusion (replaces the implicit per-csproj exclusions today) -->
  <ItemGroup Condition="'$(TargetFramework)' != 'net9.0-android35.0'">
    <Compile Remove="**\*-Android.cs" />
    <Compile Remove="**\Android\**\*.cs" />
  </ItemGroup>
  <ItemGroup Condition="'$(TargetFramework)' != 'net9.0-ios18.0'">
    <Compile Remove="**\*-iOS.cs" />
  </ItemGroup>
  <!-- ...similar for macOS / Windows-only files... -->
</Project>
```

### 1.4 Central package management

Create `Directory.Packages.props` at the repo root:

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    <CentralPackageTransitivePinningEnabled>true</CentralPackageTransitivePinningEnabled>
  </PropertyGroup>

  <ItemGroup>
    <PackageVersion Include="MonoGame.Framework.DesktopGL" Version="$(MonoGameVersion)" />
    <PackageVersion Include="MonoGame.Framework.WindowsDX" Version="$(MonoGameVersion)" />
    <PackageVersion Include="MonoGame.Framework.Android" Version="$(MonoGameVersion)" />
    <PackageVersion Include="MonoGame.Framework.iOS" Version="$(MonoGameVersion)" />
    <PackageVersion Include="MonoGame.Content.Builder.Task" Version="$(MonoGameVersion)" />
    <PackageVersion Include="OpenTK" Version="4.9.4" />
    <PackageVersion Include="SharpZipLib" Version="1.4.2" />
    <PackageVersion Include="SkiaSharp" Version="3.116.1" />
    <PackageVersion Include="SkiaSharp.NativeAssets.Linux" Version="3.116.1" />
    <PackageVersion Include="SkiaSharp.NativeAssets.macOS" Version="3.116.1" />
    <PackageVersion Include="System.Drawing.Common" Version="8.0.11" />
    <PackageVersion Include="BitMiracle.LibTiff.NET" Version="2.4.639" />
  </ItemGroup>
</Project>
```

This eliminates the need to track versions in 30 csprojs and resolves the current inconsistencies (e.g. `SharpZipLib` ranges from 1.3.3 to 1.4.2 across projects; `System.Drawing.Common` from 5.0.3 to 8.0.11).

### 1.5 The Core vs non-Core split

The two SKUs differ only in:
- Whether `MonoGame.Content.Builder.Task` is referenced (non-Core has it; Core does not).
- Some Windows-only auxiliary deps.

Recommendation: **collapse to one library and one package.** Move the content-pipeline tooling into a separate optional package `Cocos2D-Mono.ContentPipeline` (or just document that consumers bring their own MGCB workload). Eliminates 6 csprojs and 6 CI workflows immediately.

If consumer feedback shows the Core variant is genuinely depended on, keep it as a metapackage that excludes the MGCB transitive — but don't duplicate the source project.

### 1.6 CI matrix consolidation

Replace 13 workflow files with a single matrix:

```yaml
# .github/workflows/build.yml
strategy:
  matrix:
    include:
      - { os: windows-latest, tfm: net9.0-windows7.0 }
      - { os: windows-latest, tfm: net9.0-android35.0 }
      - { os: ubuntu-latest,  tfm: net9.0 }              # DesktopGL / Linux
      - { os: macos-latest,   tfm: net9.0-ios18.0 }
      - { os: macos-latest,   tfm: net9.0-maccatalyst }
steps:
  - uses: actions/checkout@v4
  - uses: actions/setup-dotnet@v4
    with: { dotnet-version: 9.0.x }
  - run: dotnet build src/Cocos2DMono/Cocos2DMono.csproj -f ${{ matrix.tfm }} -c Release
  - run: dotnet pack src/Cocos2DMono/Cocos2DMono.csproj -c Release -o artifacts/
```

### 1.7 Migration steps

1. **Snapshot baseline**: tag current `master` as `pre-modernization-v2.5.x`, and produce a NuGet pack of every existing variant for reference.
2. **Add `Directory.Packages.props`** with current versions; convert one existing csproj to consume it as a smoke test.
3. **Create `src/Cocos2DMono/Cocos2DMono.csproj`** with the multi-target shape above. Move `cocos2d/*.cs` into `src/Cocos2DMono/` *physically* — the source structure inside the project stays the same, only the project descriptor changes.
4. **Delete `cocos2d.projitems` and `cocos2d.shproj`** once the multi-target csproj compiles cleanly on every TFM.
5. **Repeat for Box2D and Tests.**
6. **Update solution file**: one `Cocos2DMono.sln` with `src/`, `tests/`, `samples/` folders.
7. **Rewrite CI** as the single matrix workflow.
8. **Verify NuGet output**: produce package with same `PackageId` (`Cocos2D-Mono`) — note this means the multi-targeted package replaces the previous `Cocos2D-Mono.DesktopGL`, `Cocos2D-Mono.Windows`, etc. SKUs. **This is the only consumer-visible change in Phase 1** and it warrants a major version bump (3.0.0). Document the migration in `CHANGELOG.md`.
9. **Delete the old `cocos2d/cocos2d.{Platform}/` and `cocos2d/cocos2d.Core.{Platform}/` directories** after all artifacts verified.

### 1.8 Acceptance criteria for Phase 1 (full)

- [x] `dotnet restore Cocos2DMono.sln` succeeds (1a).
- [x] `dotnet build src/Cocos2DMono/Cocos2DMono.csproj -f net9.0` succeeds on Windows host (1a).
- [x] `dotnet build src/Cocos2DMono/Cocos2DMono.csproj -f net9.0-windows7.0` succeeds (1a).
- [ ] `dotnet build` succeeds on `net9.0-android35.0` (CI verification, 1a).
- [ ] `dotnet build` succeeds on `net9.0-ios18.0` (CI verification, 1a).
- [ ] `dotnet pack` produces a single multi-targeted `Cocos2D-Mono.{version}.nupkg` containing all platform builds (1c, after legacy deletion lets us pack the full TFM set).
- [ ] CI matrix workflow green on every TFM.
- [ ] Tests project consolidated to `tests/Cocos2DMono.IntegrationTests/` and runs identically on every platform (1b).
- [ ] `cocos2d.projitems`, `cocos2d.shproj`, all `cocos2d.{Platform}` and `cocos2d.Core.{Platform}` folders deleted (1c).
- [ ] `release/2.6.0-preview` rebased onto post-1c dev: TFM dials bumped to net10.0, MonoGameVersion → 3.8.5-preview, CI dotnet-version → 10.0.x (1d).
- [x] `MODERNIZATION.md` updated with the actual final shape vs the plan (1a).

---

## Phase 1.5 — Fork `Cocos2D-Mono.UWP` as the source-of-truth for `Cocos2D-Mono.Uwp`

> **Goal:** Move the UWP / Xbox UWP build into its own repository so mainline can free itself of dormant `#if NETFX_CORE` code in Phase 2. **No mainline code changes** in this phase — it's a one-time repo creation, plus a single mainline tag.
>
> **Risk:** Low. Mainline is unaffected. The new repo is the build host for the existing `Cocos2D-Mono.Uwp` NuGet package (latest pre-split: [2.4.8.3](https://www.nuget.org/packages/Cocos2D-Mono.Uwp/2.4.8.3), Jan 14 2025).
>
> **Estimated effort:** 1 day (repo authoring complete; deferred items below).
>
> **Breaking?** No.

### Context

The legacy `cocos2d.Uwp/` and `box2d.Uwp/` csprojs were removed from mainline `dev` on 2024-12-01 in commit [`ea6a2d76`](https://github.com/Cocos2D-Mono/cocos2d-mono/commit/ea6a2d76). The 59 surviving `#if NETFX_CORE` blocks across 23 files have no csproj that defines `NETFX_CORE` — they compile to nothing on every current build target. MonoGame upstream dropped UWP after 3.8.2; no newer MonoGame release supports UWP. Keeping the `NETFX_CORE` guards in `dev` is pure carrying cost.

However, `Cocos2D-Mono.Uwp` continues to be published to NuGet (the latest version 2.4.8.3 shipped over a month *after* the mainline removal). The package needs a permanent build home.

### Approach: separate repo, mainline source via pinned submodule

`Cocos2D-Mono.UWP` is a **new repo that produces the `Cocos2D-Mono.Uwp` and `Cocos2D-Mono.Box2D.Uwp` NuGet packages** (the repo name uses all-caps `UWP`; the package IDs keep the existing mixed-case `Uwp` for version-line continuity). It contains only the UWP-specific csprojs, AssemblyInfo, nuspecs, and CI; mainline cocos2d-mono source is consumed via a **git submodule pinned to commit `c30f5ec3`** (the parent of the UWP-removal commit — the last commit with full `#if NETFX_CORE` source intact).

```
Cocos2D-Mono.UWP/                                       (NEW repo)
├── Directory.Build.props                               (pinned MonoGame.WindowsUniversal etc.)
├── Cocos2D-Mono.Uwp.sln                                (Cocos2D.Uwp + Box2D.Uwp)
├── external/
│   └── cocos2d-mono/                                   (submodule pinned to c30f5ec3)
├── src/
│   ├── Cocos2D.Uwp/Cocos2D.Uwp.csproj                  (pre-SDK; TargetPlatformIdentifier=UAP)
│   ├── Cocos2D.Uwp/Properties/AssemblyInfo.cs
│   ├── Box2D.Uwp/Box2D.Uwp.csproj                      (pre-SDK; TargetPlatformIdentifier=UAP)
│   └── Box2D.Uwp/Properties/AssemblyInfo.cs
├── nuget/
│   ├── Cocos2D-Mono.Uwp.nuspec                         (v2.4.8.4 — continues from 2.4.8.3)
│   └── Cocos2D-Mono.Box2D.Uwp.nuspec                   (companion split)
└── .github/workflows/build.yml                         (windows-latest matrix: x64, ARM)
```

The csprojs use **pre-SDK MSBuild format** because UWP class libraries require `TargetPlatformIdentifier=UAP`, which the modern SDK-style csproj format does not support cleanly. Each csproj has the per-architecture (`AnyCPU` / `x86` / `x64` / `ARM`) `PropertyGroup` set required for UWP `.NET Native` builds.

Source is consumed via `<Compile Include="..\..\external\cocos2d-mono\cocos2d\**\*.cs" Exclude="...">` — the historical `<Import Project="..\cocos2d.projitems" />` pattern is replaced with explicit globbing. Exclusions: mainline per-platform csproj folders, `external lib/**` (vendored ICSharpCode), `Properties/AssemblyInfo*.cs` (supplied locally), and `platform/Tuple.cs` (dead WP/Xbox360 code).

### Pinned versions

| Dependency | Version | Rationale |
|---|---|---|
| `MonoGame.Framework.WindowsUniversal` | `3.8.1.303` | Last MonoGame UWP package actually published to nuget.org. (Mainline csprojs/nuspecs reference `3.8.2.1105` but that version was never published — see `Cocos2D-Mono.UWP` repo for the resolution.) |
| `Microsoft.NETCore.UniversalWindowsPlatform` | `6.2.11` | Final stable UWP runtime. |
| `SharpDX.Mathematics` | `4.0.1` | UWP-compatible. |
| `SharpZipLib` | `1.3.3` | Last UWP-compatible version. |
| mainline `cocos2d-mono` (submodule) | commit `c30f5ec3` | Parent of UWP-removal commit; full `#if NETFX_CORE` source intact. |
| `Cocos2D-Mono.Uwp` package | starts at `2.4.8.4` | Continues the NuGet version line from 2.4.8.3 (last pre-split release). |

### Migration steps

1. **Author the new repo locally** (`c:/Projects/cocos2d-mono-uwp/` — completed). All scaffolding files committed at `c0eb596`.
2. **Create `Cocos2D-Mono/Cocos2D-Mono.UWP` empty repo on GitHub** (manual step — requires org owner permissions).
3. **Push initial commit**: `git remote add origin git@github.com:Cocos2D-Mono/Cocos2D-Mono.UWP.git && git push -u origin main`.
4. **Verify the build runs** via the new repo's CI workflow on `windows-2022` with the `Universal Windows Platform development` VS workload. (`windows-latest` is unusable: it redirects to VS 2026 which dropped the UAP SDK; `windows-2022` retains the VS 2022 UWP workload with UAP 10.0.19041.) Reference build target: `msbuild Cocos2D-Mono.Uwp.sln /p:Configuration=Release /p:Platform=AnyCPU /restore`.
5. **Publish `Cocos2D-Mono.Uwp 2.4.8.4`** and **`Cocos2D-Mono.Box2D.Uwp 2.4.8.4`** to NuGet.org from this repo's `pack` workflow.
6. **Update mainline `README.md`** to point UWP/Xbox users at the new repo.
7. **Mark mainline `#if NETFX_CORE` blocks as removable** — Phase 2 cleanup is now unblocked.

### Decision: license

The new repo is `MIT`, matching the `<PackageLicenseExpression>MIT</PackageLicenseExpression>` declared in every published `Cocos2D-Mono.Uwp.nuspec`. Mainline's top-level `LICENSE` is `AGPL-3.0`, which is a pre-existing inconsistency in mainline — flagged in the new repo's README so the discrepancy is visible.

### Acceptance criteria

- [x] Local `Cocos2D-Mono.UWP` repo authored with Cocos2D.Uwp + Box2D.Uwp pre-SDK csprojs, nuspecs, sln, README, LICENSE, CI workflow.
- [x] Submodule `external/cocos2d-mono` pinned to mainline commit `c30f5ec3`.
- [x] `Cocos2D-Mono/Cocos2D-Mono.UWP` repo created on GitHub and initial commit pushed.
- [x] CI builds green on `windows-2022` for `x64` and `ARM` in both `Debug` and `Release`. (First end-to-end success: run [26908466543](https://github.com/Cocos2D-Mono/Cocos2D-Mono.UWP/actions/runs/26908466543), commit `5c0ffe8`.)
- [ ] `Cocos2D-Mono.Uwp 2.4.8.4` published to NuGet.org from this repo.
- [x] Mainline `README.md` updated to reference the new UWP repo.
- [ ] After all the above, Phase 2 can freely delete `#if NETFX_CORE` blocks from mainline.

---

## Phase 2 — Dead code, dead conditionals, BCL replacements

> **Goal:** Reduce LOC, remove obsolete platform support, replace home-grown utilities with BCL equivalents.
>
> **Risk:** Low. Internal-only changes if the BCL replacements are wrapped behind existing public types.
>
> **Estimated effort:** 2–3 weeks.

### 2.1 Dead-platform conditional removal

> **Prerequisite:** Phase 1.5 (UWP repo) must exist before `NETFX_CORE` deletion. The other constants below are unambiguously dead and can be removed without coordination.

Remove every `#if WINDOWS_PHONE / XBOX / PSM / WP8 / SILVERLIGHT / NETFX_CORE / WINDOWS_UWP / WINRT` block. These platforms have been EOL or are no longer supported on mainline; their guarded code paths are unreachable. Files known to contain these blocks:

**`WINDOWS_PHONE / XBOX / PSM / WP8 / SILVERLIGHT`** (13 files):
- `cocos2d/denshion/CCMusicPlayer.cs`
- `cocos2d/extentions/Box2D/CCDraw.cs`
- `cocos2d/label_nodes/CCLabelBMFont.cs`
- `cocos2d/particle_nodes/CCParticleSystem.cs`
- `cocos2d/platform/CCArrayPool.cs`
- `cocos2d/platform/CCDrawManager.cs`
- `cocos2d/platform/CCRawList.cs`
- `cocos2d/platform/CCTask.cs`
- `cocos2d/platform/PList/PlistDocument.cs`
- `cocos2d/platform/Tuple.cs`
- `cocos2d/support/zip_support/ZipUtils.cs`
- `cocos2d/textures/CCTexture2D.cs`
- `cocos2d/touch_dispatcher/CCTouchDispatcher.cs`

**`NETFX_CORE / WINDOWS_UWP / WINRT`** (23 files, 59 occurrences — UWP support migrated to [[Phase 1.5 UWP repo]]):
- Concentrated in `cocos2d/platform/CCApplication.cs` (7), `cocos2d/cocoa/CCGeometry.cs` (6), `cocos2d/support/CCUserDefault.cs` (6), `cocos2d/platform/PList/PlistDocument.cs` (6), `cocos2d/CCDirector.cs` (5), `cocos2d/platform/CCAccelerometer.cs` (5).
- Remaining hits distributed across `CCDrawManager.cs`, `CCTexture2D.cs`, `CCInputState.cs`, `CCPrimitiveBatch.cs`, `CCMusicPlayer.cs`, `CCLayer.cs`, `CCTouchDelegate.cs`, `CCTextFieldTTF.cs`, `CCConfiguration.cs`, `CCGameView.Mobile.cs`, `support/CCUtils.cs`, `support/Converters/*Converter.cs` (3), `support/Compression/ZlibBaseStream.cs`, `platform/Zlib/ZInputStream.cs`, `platform/Zlib/ZOutputStream.cs`.

### 2.2 BCL replacements

| Custom type | Replacement | Notes |
|---|---|---|
| `Tuple.cs` | `System.ValueTuple` / `(T1, T2)` | Public API surface — keep `Tuple<T1,T2>` as a `[Obsolete]` shim for one minor release. |
| `CCArrayPool` | `System.Buffers.ArrayPool<T>.Shared` | Direct replacement; the BCL pool is per-thread shard backed and faster. |
| `CCRawList<T>` | `List<T>` + `CollectionsMarshal.AsSpan` for hot paths | **Benchmark before swapping.** `CCRawList.Elements` direct access is used in render-inner-loops; `CollectionsMarshal.AsSpan(list)` gives equivalent performance. |
| `CCObjectPool` | `Microsoft.Extensions.ObjectPool` *or* delete | Audit each usage — many pools aren't justified by allocation rates. |
| `CCSerialization` (custom positional text) | `System.Text.Json` with source generators *or* `MemoryPack` | Versioned and forward-compatible by construction. Migrating breaks save format — deliberate. See Phase 4. |
| Non-generic `System.Collections` (ArrayList/Hashtable) | `List<T>` / `Dictionary<TKey,TValue>` | 10+ files. Mechanical. |

### 2.3 Acceptance criteria for Phase 2

- [ ] Zero `#if WINDOWS_PHONE|XBOX|PSM|WP8|SILVERLIGHT` matches in source.
- [ ] Zero references to `System.Collections.ArrayList` or `System.Collections.Hashtable`.
- [ ] `BenchmarkDotNet` results show `CCRawList<T>` replacement is within ±5% on the existing render-loop benchmarks.

---

## Phase 3 — Language-level modernization

> **Goal:** Bring code style up to current C# idioms (file-scoped namespaces, NRTs, records, primary constructors, collection expressions). Driven by analyzers + `dotnet format`.
>
> **Risk:** Medium. The field rename is mechanically large (4400+ sites) and any callers outside this repo break.
>
> **Estimated effort:** 4–6 weeks.

### 3.1 Field renaming pass

Replace Hungarian-style field naming:

| Pattern | New convention |
|---|---|
| `m_pFoo` (pointer-prefix) | `_foo` (private), `Foo` (public auto-property) |
| `m_bIsRunning` | `_isRunning` / `IsRunning` |
| `m_fScaleX` | `_scaleX` / `ScaleX` |
| `m_nTag` | `_tag` / `Tag` |
| `m_sTransform` | `_transform` / `Transform` |

Approach:
1. Convert public mutable fields to auto-properties first (changes binary signature; warrants the major version bump anyway).
2. Run a Roslyn-based codefix to rename per directory in batches.
3. Add `[Obsolete]` aliases on any field that was actually `public` so external consumers get a deprecation cycle, not a hard break.

### 3.2 Nullable reference types

Enable `<Nullable>enable</Nullable>` project-wide. Expected fallout:
- A few hundred warnings concentrated in `CCNode` (lots of nullable references for parents/children).
- Address by directory in PRs of ~20 files each.
- Add `[NotNullWhen(true)]` / `[MemberNotNull]` annotations where invariants hold.

This is the single largest correctness win available — null-deref bugs in the action manager and touch dispatcher have been long-running pain points.

### 3.3 Modern C# syntax adoption

- `namespace Cocos2D;` (file-scoped namespaces) — `dotnet format` converts mechanically.
- Collection expressions `[…]` for static initializers.
- `record class` / `record struct` for math types where equality semantics fit.
- `readonly struct` discipline for `CCPoint`, `CCSize`, `CCRect`, `CCColor3B`, `CCColor4B`.
- Primary constructors where they reduce boilerplate.
- `required` on builders.
- Pattern matching (`is { … }`, `switch` expressions) replacing `if/else` cascades in `CCAction` evaluation paths.

### 3.4 Acceptance criteria for Phase 3

- [ ] Zero `m_[a-z]` field matches outside `[Obsolete]` shims.
- [ ] `<Nullable>enable</Nullable>` set; warnings-as-errors clean.
- [ ] `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` set in CI.
- [ ] All math types are `readonly struct` or `record struct`.

---

## Phase 4 — Architectural refactor

> **Goal:** Replace singletons with injected services, replace deep inheritance with composition (components on `CCNode`), make `CCNode` lightweight, async-ify I/O.
>
> **Risk:** High. Public-API breaking. Requires major version 3.0.0 (or 4.0.0 if Phase 1 took 3.0.0).
>
> **Estimated effort:** 8–12+ weeks.

### 4.1 Kill the singletons

Today's globals → tomorrow's services on a `CCGameContext`:

| Today (static) | Future (service) |
|---|---|
| `CCDirector.SharedDirector` | `gameContext.Director` |
| `CCApplication.SharedApplication` | `gameContext.Application` |
| `CCContentManager.SharedContentManager` | `gameContext.Content` |
| `CCFontManager` (static) | `gameContext.Fonts` |
| `CCTextureCache.SharedTextureCache` | `gameContext.Textures` |
| `CCScheduler` | `gameContext.Scheduler` |
| `CCActionManager` | `gameContext.Actions` |
| `CCTouchDispatcher` | `gameContext.Touch` |

Compatibility plan:
- Existing static `Shared*` accessors stay as `[Obsolete]` shims that return the default (most-recent-set) `CCGameContext`.
- New code paths take an injected `CCGameContext`.
- Tests can finally instantiate the framework without a real graphics device by providing fakes.

### 4.2 Componentize CCNode

Today, `CCNode` implements 7 interfaces and hardcodes transform + render + input + scheduling responsibilities. Move toward components:

```csharp
public sealed class CCNode {
    public Transform Transform { get; }
    public ComponentCollection Components { get; }
    public NodeChildren Children { get; }
    // …no more 7-interface god class
}

// Optional, attached as needed:
public sealed class SpriteRenderer : CCComponent { … }
public sealed class TouchHandler  : CCComponent { … }
public sealed class KeyboardHandler : CCComponent { … }
```

`CCSprite`, `CCLabel`, etc. become factory helpers that return a `CCNode` with the right components attached, *or* remain thin subclasses for backward source compatibility while the underlying machinery is composition-based.

### 4.3 Drop manual child indices

Replace the parallel `m_pChildren` (raw list) + `m_pChildrenByTag` (`Dictionary<int, List<CCNode>>`) with a single canonical store:

```csharp
public sealed class NodeChildren : IReadOnlyList<CCNode> {
    private readonly List<CCNode> _ordered = new();
    private Dictionary<int, CCNode>? _byTag;   // built lazily on first GetByTag

    public CCNode? GetByTag(int tag) { … }
    public void Add(CCNode child) { … }
    public void Remove(CCNode child) { … }
    // index always consistent with ordered list
}
```

Eliminates the entire class of "tag index out of sync" bugs.

### 4.4 Async-first I/O

Add `ValueTask<T>`-returning APIs on:
- `CCContentManager.LoadAsync<T>(string asset, …)`
- `CCFileUtils.ReadAllBytesAsync(string path, …)`
- `CCTextureCache.AddImageAsync(string file, …)`

Sync versions remain (with `[Obsolete]` for ones that should be replaced) — game loops still want blocking loads at boot; background streaming is the new use case.

### 4.5 Platform abstraction via DI

Replace `#if ANDROID/IOS/DESKTOPGL` blocks in shared code with platform interfaces:

```csharp
public interface ICCPlatformDevice {
    CCSize ScreenSize { get; }
    string DeviceModel { get; }
    void Vibrate(TimeSpan duration);
}

// Platform-specific impls live in -Android.cs / -iOS.cs partial files,
// registered into CCGameContext during Game.Initialize.
```

Most current `#if`-heavy files (`CCAccelerometer`, `CCDevice`, `CCApplication`) collapse to 1–2 partials each.

### 4.6 Acceptance criteria for Phase 4

- [ ] `CCGameContext` exists; all framework services accessible from it.
- [ ] All `Shared*` static accessors marked `[Obsolete]`.
- [ ] `CCNode` no longer implements `ICCSelectorProtocol`/`ICCFocusable`/etc. directly (delegated to components).
- [ ] `m_pChildrenByTag` removed; tests for tag operations remain green.
- [ ] At least one async load API added with full test coverage.
- [ ] Zero `#if ANDROID|IOS|DESKTOPGL` in `cocos2d/CCDirector.cs`, `CCApplication.cs`, `CCAccelerometer.cs`, `CCDevice.cs`.

---

## Phase 5 — Quality infrastructure

> **Goal:** Tooling that prevents regressions during Phases 2–4.
>
> **Risk:** None. Additive.
>
> **Estimated effort:** Parallel with other phases; ongoing.

### 5.1 EditorConfig + analyzers

- `.editorconfig` with naming rules (post-Phase 3 conventions), formatting, severity-to-error for key analyzers.
- `Microsoft.CodeAnalysis.NetAnalyzers` package for FxCop-equivalent rules.
- `StyleCop.Analyzers` with a curated rule set.
- `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` in CI builds (not local, to keep dev iteration fast).

### 5.2 Real test projects

The current `Tests/cocos2d-mono.Tests/` is a runnable visual test app — useful for integration but not for CI assertions.

Add `tests/Cocos2DMono.UnitTests/` (xUnit) covering:
- Math primitives (`CCPoint`, `CCRect`, `CCAffineTransform`).
- Action evaluation given mocked time (`CCActionInterval`, ease functions, sequences).
- Serialization round-trips.
- `CCNode` parent/child invariants.
- `CCScheduler` timing.

Keep the visual test runner; rename to `Cocos2DMono.IntegrationTests`.

### 5.3 Benchmark project

`tests/Cocos2DMono.Benchmarks/` using BenchmarkDotNet, covering:
- Render-loop hot paths (`CCSpriteBatchNode.Visit`, `CCDrawManager` vertex emission).
- `CCActionManager.Update` scaling with action count.
- `CCNode` traversal at varying tree depths.

Used to defend Phase 2 BCL replacements with numbers.

### 5.4 Source-link / symbol packages

```xml
<PropertyGroup>
  <PublishRepositoryUrl>true</PublishRepositoryUrl>
  <EmbedUntrackedSources>true</EmbedUntrackedSources>
  <IncludeSymbols>true</IncludeSymbols>
  <SymbolPackageFormat>snupkg</SymbolPackageFormat>
</PropertyGroup>
```

Lets consumers step into framework source from their debugger.

### 5.5 Acceptance criteria for Phase 5

- [ ] `tests/Cocos2DMono.UnitTests/` exists with > 100 unit tests covering math, actions, serialization.
- [ ] CI gates on test pass + analyzer warnings.
- [ ] Benchmark suite reproducible via `dotnet run -c Release --project tests/Cocos2DMono.Benchmarks`.
- [ ] Published packages include `.snupkg` symbols on NuGet.org.

---

## Phase 6 — Forward-looking platform decisions

> **Goal:** Strategic positioning for the next 3–5 years.
>
> **Risk:** High. Defer until Phases 1–4 are stable.
>
> **Estimated effort:** TBD; depends on direction chosen.

### 6.1 MonoGame future *(downgraded to maintenance — decision already made)*

The existence of `release/2.6.0-preview` on MonoGame `3.8.5-preview.2` + .NET 10 makes this an *implicit decision already in flight*: **Option A — stay on upstream MonoGame**. The 30-csproj-per-bump pattern that the preview branch needed for this version churn is exactly what Phase 1 eliminates (see Phase 1d).

Forward action items:
- **Track upstream MonoGame cadence**: keep `release/<next-version>-preview` branches alive as small TFM/version-property bumps after Phase 1.
- **Keep an internal rendering abstraction option open**: during the Phase 4 componentization, route rendering through a `Renderer` component rather than direct MonoGame calls in `CCSprite`/etc. That preserves Option B (swap renderer) without committing to it now.
- **KNI / FNA migration (Option C)**: explicitly off the table. Reconsider only if upstream MonoGame stalls for >12 months.

### 6.2 Native AOT viability *(potentially advanced to Phase 4)*

Most code is reflection-light, but three things block AOT today:
- `CCSerialization`'s `Type.GetType(typeName)` + `Activator.CreateInstance` (see [CCNode.cs:362-363](cocos2d/base_nodes/CCNode.cs#L362-L363)).
- The PList parser uses reflection for type lookup.
- MonoGame's `ContentTypeReader<T>` discovery (mitigated today by `TrimmerRootAssembly` in the new csprojs).

The `release/2.6.0-preview` move to **.NET 10** materially improves AOT support — trim warnings are more accurate, AOT diagnostics are better, and `System.Text.Json`'s source generator covers more scenarios. This pulls AOT viability earlier in the timeline:

- **Phase 2 serializer redesign MUST target AOT compatibility from day one** (`System.Text.Json` source generators, type-registry pattern instead of `Type.GetType`).
- **Phase 4 componentization** should use static-DI patterns (`IServiceProvider` with explicit registrations) rather than reflection-based discovery.
- **Phase 6.2's role downgrades to "verify AOT works end-to-end and add a CI matrix slot for `PublishAot=true`"** — the prep work happens in earlier phases.

### 6.3 New hosting surfaces

The `cocos2d/EmbeddableView/` directory hints at hosting cocos2d inside non-game shells. Worth investing in:
- **MAUI host sample** — a `Cocos2DView` MAUI control.
- **Avalonia host sample** — same idea cross-platform desktop.
- **WinUI3 / Uno** — if Windows-app hosting matters.

Each sample lives in `samples/` and demonstrates embedding the engine in a larger app.

### 6.4 Acceptance criteria for Phase 6

Defined when the phase is scheduled — depends on which strategic options are chosen.

---

## Suggested ordering and effort

| Phase | Risk | Effort | Breaks public API? | Can run in parallel with… |
|---|---|---|---|---|
| 1. Project consolidation (1a → 1b → 1c → 1d) | Low | 1–2 weeks | NuGet IDs change in 1c (3.0 release); 1d is the `release/2.6.0-preview` reconciliation | Phase 5 setup |
| 1.5. Fork `Cocos2D-Mono.UWP` | Low | 1 day | No (mainline untouched) | Standalone — runs between 1c and 2 |
| 2. Dead code / BCL replacement | Low | 2–3 weeks | No | Phase 5, Phase 3 setup |
| 3. Language modernization | Medium | 4–6 weeks | Field renames; mitigated by `[Obsolete]` shims | Phase 5 |
| 4. Architectural refactor | High | 8–12+ weeks | Yes (4.0 release) | Late Phase 3 |
| 5. Quality infrastructure | Low | Ongoing | No | All other phases |
| 6. Strategic | Lower than before (Option A locked in) | TBD | Depends | After Phase 4 |

### Recommended starting point

Begin with **Phase 1 + Phase 5 setup in parallel**:
- Phase 1 unblocks every subsequent phase by making the build sane.
- Phase 5 catches regressions while Phases 2/3/4 move fast.

Phase 4 should not start until Phase 3's NRT pass is complete — designing components and DI on top of a non-null-aware codebase wastes effort.

---

## Open questions

- Public API consumers: who uses `Cocos2D-Mono.Core.{Platform}` versus `Cocos2D-Mono.{Platform}`? Answer determines whether Phase 1 collapses to one package or keeps a Core variant.
- Save-file compatibility: are there shipped games with `CCSerialization`-format save files that need to load on the new serializer? If yes, Phase 2 needs a one-time legacy reader.
- ~~Render backend bet~~: **Resolved** — Option A (stay on upstream MonoGame). The `release/2.6.0-preview` branch's move to MonoGame 3.8.5 confirms this. Phase 4 still routes rendering through a component boundary so the door stays open.
- Cadence: is this modernization a continuous effort, or driven by specific releases? Affects whether `[Obsolete]` shims live for one minor or three.
- ~~UWP repo bootstrapping~~: **Resolved** — the UWP repo is a *source-of-truth*, not a NuGet consumer. It uses a git submodule pinned to mainline commit `c30f5ec3` to vendor cocos2d-mono source. See Phase 1.5 for details. The `Cocos2D-Mono.Uwp` NuGet package family continues with version 2.4.8.4 from the new repo.

---

*Document last updated alongside the modernization effort kickoff. Update each phase's section as work completes — the “Acceptance criteria” checkboxes are the source of truth for completion.*
