# Tests

Two projects, both built from the root `Cocos2DMono.sln`:

| Project | What it is | Run |
|---|---|---|
| `Cocos2DMono.UnitTests` | Headless xUnit suite (math primitives, `CCRawList`, `CCUserDefault`, …). Runs in CI. | `dotnet test Tests/Cocos2DMono.UnitTests/Cocos2DMono.UnitTests.csproj` |
| `Cocos2DMono.IntegrationTests` | The interactive, multi-scene visual **test app**. A maintainer-run harness, not part of the automated gate. | see below |
| `cocos2d-mono.Tests/` | Not a project — the **shared source** (scenes, `TestController`, `Program.cs`, content) that `Cocos2DMono.IntegrationTests` globs. | — |

## Running the interactive test app

`Cocos2DMono.IntegrationTests` is a single multi-targeted project (it replaced the 13
legacy per-platform test-app projects/solutions during the build consolidation — nothing
was lost, only the launch path changed). Pick the target framework for your platform:

```bash
# Desktop (DesktopGL) — the usual dev target
dotnet run --project Tests/Cocos2DMono.IntegrationTests/Cocos2DMono.IntegrationTests.csproj -f net10.0

# Windows (WindowsDX)
dotnet run --project Tests/Cocos2DMono.IntegrationTests/Cocos2DMono.IntegrationTests.csproj -f net10.0-windows7.0
```

Android (`net10.0-android36.0`) and iOS (`net10.0-ios26.0`) build from the same project but
deploy through their platform tooling rather than `dotnet run`.

A convenience script is provided: [`run-testapp.ps1`](run-testapp.ps1) (defaults to the
DesktopGL target).

## Adding a test scene

Add the `.cs` under `Tests/cocos2d-mono.Tests/` — the project globs the tree, so no csproj
edit is needed — then wire it into `TestController.cs`.
