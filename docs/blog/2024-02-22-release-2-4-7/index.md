---
slug: 2.4.7-release
title: Cocos2D-Mono 2.4.7 is now out!
authors: [brandmooffin]
tags: [update, release, 2.4.7]
---

Cocos2D-Mono 2.4.7 is out now! Go check out the [release notes](https://github.com/brandmooffin/cocos2d-mono/releases/tag/2.4.7) to see what's change...or just read below.

## What's Changed
    - Fix issue with TextureAtlas Data not purging correctly
    - Core libraries!! 
    - These libraries only contain the MonoGame framework only, the Content Builder Task is not included
    - Fix for CCLabel Crashing
    - iOS Font Texture Improvements
    - Fix for DisplayStats on iOS
    - Fix for CCTexture being disposed too early
    - Implement Line Break Modes for CCLabelBMFont and CCLabel
    - Added support for more delimiters in Labels
    - Streamlined usages for RemoveFromParent, RemoveChild and RemoveAllChildren.
    - Adjust position to reduce line artifacts on fonts
    - Improvements with cleanup and dispose objects

**Full Changelog**: https://github.com/brandmooffin/cocos2d-mono/compare/2.4.6...2.4.7

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

## Core Library
This will only have the MonoGame Framework dependency and not the MonoGame Content Builder Task. 

The benefits for these Core libraries are speed improvements to compilation and resolves an existing vulnerability that exists within `MonoGame.Content.Builder.Task`
![image](https://github.com/brandmooffin/cocos2d-mono/assets/1774581/6d869242-8d2a-4343-90ec-0372f4293b72)

The downside to this is assets will need to be built manually prior to compilation, either via command line using the [dotnet tool](https://monogame.net/articles/tools/mgcb.html) or the [MGCB Editor](https://monogame.net/articles/tools/mgcb_editor.html).

## CCTextLineBreakMode

Introducing new LIne Break Modes for `CCLabelBMFont` & `CCLabel`:
```
  public enum CCTextLineBreakMode
  {
      SmartBreak,
      WordBreak,
      CharacterBreak,
      NoBreak
  }
```

### Word Break
Break string lines by words

### Character Break
Break string lines by characters

### Smart Break (Default)
Combination of Word and Character breaks. Makes best determination for line break.

### No Break
Line has no breaks. Just one line continuous string.


## Note
`LineBreakWithoutSpace` will be deprecated in a future release.

> This release introduces some changes around usages of **RemoveFromParent**, **RemoveChild** and **RemoveAllChildren**.
> 
> First **RemoveFromParentAndCleanup** and **RemoveAllChildrenWithCleanup** have been removed in favor of simply passing in a **cleanup** parameter, which is now `true` by default. This mirrors the same behavior as before but without needing to explicitly call **RemoveFromParentAndCleanup** and **RemoveAllChildrenWithCleanup**.
> 
> For example, **m_plabel.RemoveFromParentAndCleanup(true);** would now look like **m_plabel.RemoveFromParent();**.
> 
> You can see that this helps to streamline the usage and cleanup will always be performed unless explicitly necessary by passing in `false` e.g. **m_plabel.RemoveFromParent(false);**.
> 
> This also reduces any confusion on whether **RemoveFromParent** or **RemoveFromParentAndCleanup** should be called. It is simply just **RemoveFromParent** in every case and just specify if cleanup is needed.

Check it out & stay tuned for more to come!