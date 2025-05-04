---
sidebar_position: 1
---

# Common Issues

### Missing Arial Font (Linux)

`System.IO.FileNotFoundException: Could not find "Arial" font file.`

This error comes up if you are trying to use "Arial" font but are missing the font on your system.

Simply install it via the following command in terminal:

```console
sudo apt-get install ttf-mscorefonts-installer
```

### Unable to Load freetype6
`System.DllNotFoundException: Unable to load shared library 'freetype6' or one of its dependencies.`

#### Windows

If you are running into issues related `Unable to load DLL 'freetype6.dll'` or something similar, you may need to install [VC++ Runtime for 2012](https://www.microsoft.com/en-us/download/details.aspx?id=30679).

#### Linux

Run the following in terminal:
```console
sudo apt-get install freetype*
```

Reference: https://stackoverflow.com/questions/21216129/install-gd-library-and-freetype-on-linux

You may need to also copy the lib file to the .dotnet directory so MonoGame can discover it.

- Locate `libfreetype.so.6.20.1` in the following directory `/usr/lib/aarch64-linux-gnu`

- Copy `libfreetype.so.6.20.1` into `~/.dotnet/shared/Microsoft.NETCore.App/8.0.13` (or whichever version of donet installed)

- Rename `libfreetype.so.6.20.1` to `freetype6.so`

#### macOS

Run the following in terminal:
```console
brew install freetype
```

Reference: https://formulae.brew.sh/formula/freetype#default

You may need to also copy the lib file to the .dotnet directory so MonoGame can discover it.

- Locate `libfreetype.6.dylib` in the following directory `/opt/homebrew/Cellar/freetype/2.13.3/lib`

- Copy `libfreetype.6.dylib` into `~/.dotnet/shared/Microsoft.NETCore.App/8.0.13` (or whichever version of donet installed)

- Rename `libfreetype.6.dylib` to `libfreetype6.dylib` 


### Unable to Load freeimage
`System.DllNotFoundException: Unable to load shared library 'FreeImage' or one of its dependencies.`

#### Linux

Run the following in terminal:
```console
sudo apt-get install libfreeimage3 libfreeimage-dev
```

Reference: https://codeyarns.com/tech/2014-02-11-how-to-install-and-use-freeimage.html#gsc.tab=0

You may need to also copy the lib file to the .dotnet directory so MonoGame can discover it.

- Locate `libfreeimage-3.18.0.so` in the following directory `/usr/lib/aarch64-linux-gnu`

- Copy `libfreeimage-3.18.0.so` into `~/.dotnet/shared/Microsoft.NETCore.App/8.0.13` (or whichever version of donet installed)

- Rename `libfreeimage-3.18.0.so` to `FreeImage.so` **(casing matters)**

#### macOS

Run the following in terminal:
```console
brew install freeimage
```

Reference: https://formulae.brew.sh/formula/freeimage#default

You may need to also copy the lib file to the .dotnet directory so MonoGame can discover it.

- Locate `libfreeimage.3.18.0.dylib` in the following directory `/opt/homebrew/Cellar/freeimage/3.18.0/lib`

- Copy `libfreeimage.3.18.0.dylib` into `~/.dotnet/shared/Microsoft.NETCore.App/8.0.13` (or whichever version of donet installed)

- Rename `libfreeimage.3.18.0.dylib` to `libfreeimage.dylib` 