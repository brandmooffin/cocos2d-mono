---
slug: 2.4.8-release
title: Cocos2D-Mono 2.4.8 is now out!
authors: [brandmooffin]
tags: [update, release, 2.4.8]
---

Cocos2D-Mono 2.4.8 is out now! Go check out the [release notes](https://github.com/brandmooffin/cocos2d-mono/releases/tag/2.4.8) to see what's change...or just read below.

## What's Changed
    - Support for CCVector2
    - Replace GetActionByTag with GetAction
    - Replace StopActionByTag with StopAction
    - Replace AlignItemsVerticallyWithPadding with AlignItemsVertically
    - Replace AlignItemsHorizontallyWithPadding with AlignItemsHorizontally
    - Add support for pause all sound effects
    - Add support for resume all sound effects
    - Fix out of index error for CCSpriteBatchNode
    - Fix for recreating static texture for the atlas in CCLabel
    - Increase Texture Size for CCLabel
    - Static context improvements for CCLabel
    - Vertical alignment adjustment for Android Labels
    - Fix Font Size not changing immediately
    - Lower iOS target version to 16.1
    - Improvements for CCLabel textures
    - Improvements for CCLabel character spacing
    - Reduce character spacing for JA language
    - Improve CCLabel Texture Caching
    - Upgrade to .NET 7
    - Fix vertical spacing for CCLabel
    - Fix spacing between CJK characters
    - Improved support for CJK characters
    - Resolved character spacing issues for CCLabel
    - iOS improvements
    - Better handling for CCTexture caching
    - Improvements for font definitions
    - Minor adjustment for font spacing
    - Improve texture cache for CCLabel textures
    - Add null checks for CCScheduler
    - Improve reliability for CCLabel textures
    - Improvements reusing TextureAtlas
    - Fixes for Create Charater Bitmap Fonts
    - Fixes for Creating Fonts
    - Fixes for Round Line Cap
    - SystemFontSpacing for character spacing
    - CCLabel improvements for Android and iOS  

**Full Changelog**: https://github.com/brandmooffin/cocos2d-mono/compare/2.4.7...2.4.8

NuGet Packages:

[Cocos2D-Mono.Android](https://www.nuget.org/packages/Cocos2D-Mono.Android/)
[Cocos2D-Mono.DesktopGL](https://www.nuget.org/packages/Cocos2D-Mono.DesktopGL/)
[Cocos2D-Mono.iOS](https://www.nuget.org/packages/Cocos2D-Mono.iOS/)
[Cocos2D-Mono.Uwp](https://www.nuget.org/packages/Cocos2D-Mono.Uwp/)
[Cocos2D-Mono.Windows](https://www.nuget.org/packages/Cocos2D-Mono.Windows/)

[Cocos2D-Mono.Core.Android](https://www.nuget.org/packages/Cocos2D-Mono.Core.Android/)
[Cocos2D-Mono.Core.DesktopGL](https://www.nuget.org/packages/Cocos2D-Mono.Core.DesktopGL/)
[Cocos2D-Mono.Core.iOS](https://www.nuget.org/packages/Cocos2D-Mono.Core.iOS/)
[Cocos2D-Mono.Core.Uwp](https://www.nuget.org/packages/Cocos2D-Mono.Core.Uwp/)
[Cocos2D-Mono.Core.Windows](https://www.nuget.org/packages/Cocos2D-Mono.Core.Windows/)

[Visual Studio Project Template Extension](https://marketplace.visualstudio.com/items?itemName=Cocos2D-MonoTeamBrokenWallsStudios.cocos2dmonoprojecttemplates)

## Breaking Changes
There are a couple of changes that may cause some minor refactoring. As part of this release the following changes are to be kept in mind:

  - Replace GetActionByTag with GetAction
  - Replace StopActionByTag with StopAction
  - Replace AlignItemsVerticallyWithPadding with AlignItemsVertically
  - Replace AlignItemsHorizontallyWithPadding with AlignItemsHorizontally

```
GetActionByTag -> GetAction
StopActionByTag -> StopAction

AlignItemsVerticallyWithPadding -> AlignItemsVertically
AlignItemsHorizontallyWithPadding -> AlignItemsHorizontally
```

You will need to replace these usages but should be very minimal other than function name changes.



Check it out & stay tuned for more to come!