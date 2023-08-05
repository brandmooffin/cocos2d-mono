---
slug: 2.4.3-release
title: Cocos2D-Mono 2.4.3 is now out!
authors: [brandmooffin]
tags: [update, release, 2.4.3]
---

Cocos2D-Mono 2.4.3 is out now! Go check out the [release notes](https://github.com/brandmooffin/cocos2d-mono/releases/tag/2.4.3) to see what's change...or just read below.

## What's Changed
    - Support for AddedToScene for CCLayer
    - New CCTapNode for supporting TouchOneByOne TouchMode
    - Support Dispose function for CCNode, CCSprite, CCLayer
    - New CCActionState
    - Support Math Operators for CCColor4B
    - Support for initializing CCScale9Sprite from existing CCSprite
    - Added ContentSize getter for CCSpriteFrame
        - Note: OriginalSize will be deprecated in a future release
    - Added TextureRectInPixels and SpriteFrame Properties for CCSprite
    - Added IsSpriteFrameDisplayed function for CCSprite
        - Note: IsFrameDisplayed will be deprecated in a future release
    - New IsNear function for CCPoint
    - Added LengthSquared and DistanceSquared functions for CCPoint
        - Note: DistanceSQ, LengthSQ and LengthSquare will be deprecated in a future release
    - New CCActions: CCColorBlendAnimation, CCMoveFrom, CCRotateAnimation, CCSwapAction, CCTargetedAction, CCTimerAction
    - Support for UnscheduleAll for CCNode
    - Added SystemFont and SystemFontSize property for CCLabel
        - Note: FontName and FontSize will be deprecated in a future release
    - Added ScaledContentSize and BoundingBoxTransformedToWorld properties for CCNode
    - Migrated CCNodeRGBA into CCNode
        - Note: CCNodeRGBA will be deprecated in a future release
    - Added Lerp and Clamp functions to CCMathHelper
    - New Pi properties for CCMathHelper


**Full Changelog**: https://github.com/brandmooffin/cocos2d-mono/compare/2.4.2...2.4.3

NuGet Packages:

[Cocos2D-Mono.Android](https://www.nuget.org/packages/Cocos2D-Mono.Android/)
[Cocos2D-Mono.DesktopGL](https://www.nuget.org/packages/Cocos2D-Mono.DesktopGL/)
[Cocos2D-Mono.iOS](https://www.nuget.org/packages/Cocos2D-Mono.iOS/)
[Cocos2D-Mono.Uwp](https://www.nuget.org/packages/Cocos2D-Mono.Uwp/)
[Cocos2D-Mono.Windows](https://www.nuget.org/packages/Cocos2D-Mono.Windows/)

[Visual Studio Project Template Extension](https://marketplace.visualstudio.com/items?itemName=Cocos2D-MonoTeamBrokenWallsStudios.cocos2dmonoprojecttemplates)

This release sees some great improvements to the library! 

Check it out & stay tuned for more to come!