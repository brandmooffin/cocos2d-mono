# Contributing to Cocos2D-Mono

Thanks so much for your interest in cocos2d-mono and wanting to contribute to the project!

Here's a guide to help you get started.


## How To Contribute

cocos2d-mono has a `master` branch for stable releases and uses `feature` branches for new features and `bugfix` branches for fixes and resolving bugs.

In order to contribute to the project you should learn and be familiar with how to [use Git](https://help.github.com/articles/set-up-git/), how to [create a fork of Cocos2D-Mono](https://help.github.com/articles/fork-a-repo/), and how to [submit a Pull Request](https://help.github.com/articles/using-pull-requests/).

After you submit a PR, your changes will be reviewed and provided with any constructive feedback to improve your submission.

Once your changes are good for cocos2d-mono, your PR will be merged.


## Quick Guidelines

Here are a few simple rules and suggestions to remember when contributing to Cocos2D-Mono.

* :bangbang: **NEVER** commit code that you didn't personally write.
* :bangbang: **NEVER** use decompiler tools to steal code and submit it as your own work.
* :bangbang: **NEVER** decompile XNA assemblies and steal Microsoft's copyrighted code.
* **PLEASE** try to keep your PRs focused on a single topic and of a reasonable size or you may be asked to break it up.
* **PLEASE** be sure to write simple and descriptive commit messages.
* **DO NOT** surprise us with new APIs or big new features. Open an issue to discuss your ideas first.
* **DO NOT** reorder type members as it makes it difficult to compare code changes in a PR.
* **DO** try to follow our [coding style](CODESTYLE.md) for new code.
* **DO** give priority to the existing style of the file you're changing.
* **DO** try to add to the [tests](Tests) when adding new features or fixing bugs.
* **DO NOT** send PRs for code style changes or make code changes just for the sake of style.
* **PLEASE** keep a civil and respectful tone when discussing and reviewing contributions.
* **PLEASE** tell others about Cocos2D-Mono and your contributions via social media.


## Decompiler Tools

We prohibit the use of tools like dotPeek, ILSpy, JustDecompiler, or .NET Reflector which convert compiled assemblies into readable code.

There has been confusion on this point in the past, so we want to make this clear.  It is **NEVER ACCEPTABLE** to decompile copyrighted assemblies and submit that code to the MonoGame project.

* It **DOES NOT** matter how much you change the code.
* It **DOES NOT** matter what country you live in or what your local laws say.  
* It **DOES NOT** matter that XNA is discontinued.  
* It **DOES NOT** matter how small the bit of code you have stolen is.  
* It **DOES NOT** matter what your opinion of stealing code is.

If you did not write the code, you do not have ownership of the code and you shouldn't submit it to MonoGame.

If we find a contribution to be in violation of copyright, it will be immediately removed.  We will bar that contributor from the MonoGame project.

## Code guidelines

Due to limitations on private target platforms, MonoGame enforces the use of C# 5.0 features.

It is however allowed to use the latest class library, but if contributions make use of classes that are not present in .NET 4.5, it will be required from the contribution to implement backward-compatible switches.

These limitations should be lifted at some point.

## Licensing

The MonoGame project is under the [Microsoft Public License](https://opensource.org/licenses/MS-PL) except for a few portions of the code.  See the [LICENSE.txt](LICENSE.txt) file for more details.  Third-party libraries used by MonoGame are under their own licenses.  Please refer to those libraries for details on the license they use.

We accept contributions in "good faith" that it isn't bound to a conflicting license.  By submitting a PR you agree to distribute your work under the MonoGame license and copyright.

To this end, when submitting new files, include the following in the header if appropriate:
```csharp
// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
```

## Need More Help?

If you need help, please ask questions on our [community forums](http://community.monogame.net/) or come [chat on Gitter](https://gitter.im/mono/MonoGame).


Thanks for reading this guide and helping make MonoGame great!

 :heart: The MonoGame Team