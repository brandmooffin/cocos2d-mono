---
sidebar_position: 4
---

# Migrating to 2.4.0

Migrating from 2.3.X should be straightforward for most platforms.

The major difference is that 2.4.0 now requires .NET 6 and Visual Studio 2022. You can follow the [environment setup tutorial](/docs/getting-started/environment-setup.md) to make sure that you are not missing any components.

It is recommend that you use the updated [Cocos2D-Mono Visual Studio 2022 Extension](https://marketplace.visualstudio.com/items?itemName=Cocos2D-MonoTeamBrokenWallsStudios.cocos2dmonoprojecttemplates) which contains the new project templates that will help with the migration process.

## WindowsDX, DesktopGL, and UWP
Upgrading from 2.3.X should be as straightforward as upgrading your `TargetFramework`, Cocos2d-Mono version and MonoGame version.

Edit your csproj file to change your `TargetFramework`:

    <TargetFramework>net6.0</TargetFramework>

Then edit your Cocos2D-Mono PackageReference to point to 2.4.0 & MonoGame PackageReference to point to 3.8.1:

    <PackageReference Include="Cocos2D-Mono.{Platform}" Version="2.4.0" />
    <PackageReference Include="MonoGame.Framework.{Platform}" Version="3.8.2.1105" />
    <PackageReference Include="MonoGame.Content.Builder.Task" Version="3.8.2.1105" />

## iOS/iPadOS, and Android
.NET 6 introduced breaking changes in how csproj are defined for iOS/iPadOS and Android. We recommand that you create new projects using the [Cocos2D-Mono 2.4.0 Project Templates](https://marketplace.visualstudio.com/items?itemName=Cocos2D-MonoTeamBrokenWallsStudios.cocos2dmonoprojecttemplates) and that you copy over your project files there.
