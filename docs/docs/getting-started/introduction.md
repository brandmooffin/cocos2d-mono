---
sidebar_position: 1
---

# Introduction

![Cocos2D-Mono](https://raw.githubusercontent.com/brandmooffin/cocos2d-mono/master/Logos/logo-full-200.png)

Cocos2D-Mono is the premier 2D game development engine based upon the wildly popular and successful Cocos2D-X engine and picking up where Cocos2D-XNA left off. With Cocos2D-Mono, the game developer can create fantastic games with rich user experiences without the tremendous cost of a proprietary game library. MIT licensed, and open source hosted on GitHub, this framework gives developers total power and control over every aspect of their game. Cocos2D-XNA has been used to deploy games to nearly every type of device in use today using XNA from Microsoft or MonoGame, Cocos2D-Mono hopes to continue that journey. Refreshing the Cocos2D-XNA project to support latest MonoGame versions and bringing support to more platforms, the power of XNA and the depth of Cocos2d are at every game developers reach -again-, taking their creative genius to over 95% of the computing devices on the planet.

Cocos2D-Mono focuses more on the MonoGame Framework and removes the limitations from proper XNA which held the original Cocos2D-XNA project back.

# Build Status

[![DesktopGL (Windows/Linux/macOS)](https://github.com/brandmooffin/cocos2d-mono/actions/workflows/desktopgl_build.yml/badge.svg)](https://github.com/brandmooffin/cocos2d-mono/actions/workflows/desktopgl_build.yml)
[![Windows](https://github.com/brandmooffin/cocos2d-mono/actions/workflows/windows_build.yml/badge.svg)](https://github.com/brandmooffin/cocos2d-mono/actions/workflows/windows_build.yml)
[![Android](https://github.com/brandmooffin/cocos2d-mono/actions/workflows/android_build.yml/badge.svg)](https://github.com/brandmooffin/cocos2d-mono/actions/workflows/android_build.yml)
[![iOS](https://github.com/brandmooffin/cocos2d-mono/actions/workflows/ios_build.yml/badge.svg)](https://github.com/brandmooffin/cocos2d-mono/actions/workflows/ios_build.yml)

# Supported Platforms

We support a growing list of platforms across the desktop, mobile, and console space. If there is a platform we don't support, please [make a request](https://github.com/brandmooffin/cocos2d-mono/issues).

- Desktop PCs
  - Windows (OpenGL & DirectX)
  - Linux (OpenGL)
  - macOS (OpenGL)
- Mobile/Tablet Devices
  - Android (OpenGL)
  - iOS (OpenGL)
- Coming Soon
  - iOS (Metal)
  - tvOS (Metal)
  - macOS (Metal)
  - Xbox (XDK)
  - Nintendo Switch
  - PlayStation 4
  - PlayStation 5

# Support & Contributing

If you think you have found a bug or have a feature request, use the [issue tracker](https://github.com/brandmooffin/Cocos2D-Mono/issues). Before opening a new issue, please search to see if your problem has already been reported. Try to be as detailed as possible in your issue reports.

If you are interested in contributing fixes or features to Cocos2D-Mono, please read our [contributors guide](/docs/contributing/getting-involved.md) first.

## Tests

We have created solutions for all the supported platforms that serves as our Test Bed for each platform.

You can find those in the [Tests directory](https://github.com/brandmooffin/cocos2d-mono/tree/master/Tests "Tests")

- cocos2d-mono.Tests.Android

- cocos2d-mono.Tests.Windows

- cocos2d-mono.Tests.DesktopGL

- cocos2d-mono.Tests.iOS

**LINUX SETUP NOTE:** There are some fonts used within the Test Bed not natively found on Linux, please run the following command to add the missing fonts:

> sudo apt-get install ttf-mscorefonts-installer

More tests coming soon!

## Linux & macOS (OpenGL)

For Linux & macOS projects use DesktopGL (cross-platform with Windows support).
