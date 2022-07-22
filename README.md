# Cocos2D-Mono
![Cocos2D-Mono](https://raw.githubusercontent.com/brandmooffin/cocos2d-mono/master/Logos/logo-full-200.png)

Cocos2D-Mono is the premier 2D game development engine based upon the wildly popular and successful Cocos2D-X engine and picking up where Cocos2D-XNA left off. With Cocos2D-Mono, the game developer can create fantastic games with rich user experiences without the tremendous cost of a proprietary game library. MIT licensed, and open source hosted on GitHub, this framework gives developers total power and control over every aspect of their game. Cocos2D-XNA has been used to deploy games to nearly every type of device in use today using XNA from Microsoft or MonoGame, Cocos2D-Mono hopes to continue that journey. Refreshing the Cocos2D-XNA project to support latest MonoGame versions and bringing support to more platforms, the power of XNA and the depth of Cocos2d are at every game developers reach -again-, taking their creative genius to over 95% of the computing devices on the planet.

Cocos2D-Mono focuses more on the MonoGame Framework and removes the limitations from proper XNA which held the original Cocos2D-XNA project back.

# Build Status

[![DesktopGL](https://github.com/brandmooffin/cocos2d-mono/actions/workflows/desktopgl_build.yml/badge.svg)](https://github.com/brandmooffin/cocos2d-mono/actions/workflows/desktopgl_build.yml)

[![Windows](https://github.com/brandmooffin/cocos2d-mono/actions/workflows/windows_build.yml/badge.svg)](https://github.com/brandmooffin/cocos2d-mono/actions/workflows/windows_build.yml)

# Supported Platforms

We support a growing list of platforms across the desktop, mobile, and console space.  If there is a platform we don't support, please [make a request](https://github.com/brandmooffin/cocos2d-mono/issues).

 * Desktop PCs
   * Windows 10 Store Apps (UWP)
   * Windows Win32 (OpenGL & DirectX)
   * Linux (OpenGL)
   * macOS (OpenGL)
 * Mobile/Tablet Devices
   * Android (OpenGL)
   * iOS (OpenGL)
   * Windows Phone 10 (UWP)
 * Consoles 
   * Xbox One (UWP)
 * Coming Soon
   * iOS (Metal)
   * tvOS (Metal)
   * macOS (Metal)
   * Xbox One (XDK)
   * Nintendo Switch
   * PlayStation Vita
   * PlayStation 4
   

# Download and Run

To obtain the code you will need a git client.  Either command line or graphical.

Using the git command line you will need to clone the git repository.

> $ git clone https://github.com/brandmooffin/cocos2d-mono.git

Wait until the clone has finished.

You should see something similar to the following:

	Cloning into 'cocos2d-mono'...
	remote: Counting objects: 20553, done.
	remote: Compressing objects: 100% (7677/7677), done.
	remote: Total 20553 (delta 14127), reused 18870 (delta 12446)
	Receiving objects: 100% (20553/20553), 100.83 MiB | 634 KiB/s, done.
	Resolving deltas: 100% (14127/14127), done.
	Checking out files: 100% (4130/4130), done.

You now have everything you need to start start developing with Cocos2D-Mono

NOTE: Cocos2D-Mono is currently built with MonoGame Framework 3.8 and included as a nuget package, so no need to pull the MonoGame source code! (Unless you want to anyways, then go for it)

# Support & Contributing

If you think you have found a bug or have a feature request, use the [issue tracker](https://github.com/brandmooffin/Cocos2D-Mono/issues). Before opening a new issue, please search to see if your problem has already been reported.  Try to be as detailed as possible in your issue reports.

If you are interested in contributing fixes or features to Cocos2D-Mono, please read our [contributors guide](CONTRIBUTING.md) first.

# Templates for Visual Studio


To make things as easy as possible templates for Visual Studio are provided and can be found [here](https://github.com/brandmooffin/cocos2d-mono/tree/master/ProjectTemplates).

There are currently templates available as an extension for Visual Studio 2017 & 2019 [here](https://marketplace.visualstudio.com/items?itemName=Cocos2D-MonoTeamBrokenWallsStudios.cocos2dmonoprojecttemplates). Additional IDEs will be supported soon!


# Getting Started

## Samples

Samples can be found [here](https://github.com/brandmooffin/cocos2d-mono/tree/master/Samples)

## NuGet Packages

Cocos2D-Mono is also available as a [NuGet package](https://www.nuget.org/packages?q=cocos2d-mono)

## Tests

We have created solutions for all the supported platforms that serves as our Test Bed for each platform.

You can find those in the [Tests directory](https://github.com/brandmooffin/cocos2d-mono/tree/master/Tests "Tests")

  * cocos2d-mono.Tests.Android
  
  * cocos2d-mono.Tests.Windows

  * cocos2d-mono.Tests.Uwp

  * cocos2d-mono.Tests.DesktopGL
  
  * cocos2d-mono.Tests.iOS

**LINUX SETUP NOTE:** There are some fonts used within the Test Bed not natively found on Linux, please run the following command to add the missing fonts:

 > sudo apt-get install ttf-mscorefonts-installer

More tests coming soon!

## Linux & macOS (OpenGL)

For Linux & macOS projects use DesktopGL (cross-platform with Windows support).

## Troubleshooting
If you are running into issues related `Unable to load DLL 'freetype6.dll'` or something similar, you may need to install [VC++ Runtime for 2012](https://www.microsoft.com/en-us/download/details.aspx?id=30679).
