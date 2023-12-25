---
slug: 2.4.6-release
title: Cocos2D-Mono 2.4.6 is now out!
authors: [brandmooffin]
tags: [update, release, 2.4.6]
---

Cocos2D-Mono 2.4.6 is out now! Go check out the [release notes](https://github.com/brandmooffin/cocos2d-mono/releases/tag/2.4.6) to see what's change...or just read below.

## What's Changed
    - Implemented CCTextField for all platforms
    - Fixed issue with CCNode Pause and Resume
    - Fixed visibility issue with CCSprite
    - Enhancements for DrawSegment
    - Add DrawLine function
    - Additional colors like CCColor3B.DarkGray
    - Font improvements for Android and iOS
    - Fixes for CCSprite ContentSize setter
    - Fixes for CCParallel
    - Reset vertex for CCDrawNode on Clear
    - Maintain CCSprite Scale on TextureRect changes

**Full Changelog**: https://github.com/brandmooffin/cocos2d-mono/compare/2.4.5...2.4.6

NuGet Packages:

[Cocos2D-Mono.Android](https://www.nuget.org/packages/Cocos2D-Mono.Android/)
[Cocos2D-Mono.DesktopGL](https://www.nuget.org/packages/Cocos2D-Mono.DesktopGL/)
[Cocos2D-Mono.iOS](https://www.nuget.org/packages/Cocos2D-Mono.iOS/)
[Cocos2D-Mono.Uwp](https://www.nuget.org/packages/Cocos2D-Mono.Uwp/)
[Cocos2D-Mono.Windows](https://www.nuget.org/packages/Cocos2D-Mono.Windows/)

[Visual Studio Project Template Extension](https://marketplace.visualstudio.com/items?itemName=Cocos2D-MonoTeamBrokenWallsStudios.cocos2dmonoprojecttemplates)

This release sees some great improvements to the library! 

Please note that this will be the final update under .NET 6. Updates going forward will be under .NET 7 as .NET 6 is end of life and this will allow better support with those working with .NET 8. .NET 7 allows the library to be on the latest version possible while still being able to support MonoGame.

You can expect the move to .NET 7 will occur as part of the first release for 2024. Following that update, there will also be a roadmap released to show the direction of the library and some of the amazing features that are coming up. 2024 should be a big year for cocos2d-mono and I can't wait to show off why at the start of the year!

That said, once MonoGame makes the move to .NET 8, plans to move to .NET 8 will also be made.

Check it out & stay tuned for more to come!