$BuildPaths = "..\cocos2d-mono\cocos2d\cocos2d.Android\bin", "..\cocos2d-mono\cocos2d\cocos2d.Android\obj", 
    "..\cocos2d-mono\cocos2d\cocos2d.Core.Android\bin", "..\cocos2d-mono\cocos2d\cocos2d.Core.Android\obj", 
    "..\cocos2d-mono\cocos2d\cocos2d.iOS\bin", "..\cocos2d-mono\cocos2d\cocos2d.iOS\obj", 
    "..\cocos2d-mono\cocos2d\cocos2d.Core.iOS\bin", "..\cocos2d-mono\cocos2d\cocos2d.Core.iOS\obj", 
    "..\cocos2d-mono\cocos2d\cocos2d.DesktopGL\bin", "..\cocos2d-mono\cocos2d\cocos2d.DesktopGL\obj", 
    "..\cocos2d-mono\cocos2d\cocos2d.Core.DesktopGL\bin", "..\cocos2d-mono\cocos2d\cocos2d.Core.DesktopGL\obj", 
    "..\cocos2d-mono\cocos2d\cocos2d.UWP\bin", "..\cocos2d-mono\cocos2d\cocos2d.UWP\obj", 
    "..\cocos2d-mono\cocos2d\cocos2d.Core.UWP\bin", "..\cocos2d-mono\cocos2d\cocos2d.Core.UWP\obj", 
    "..\cocos2d-mono\cocos2d\cocos2d.Windows\bin", "..\cocos2d-mono\cocos2d\cocos2d.Windows\obj", 
    "..\cocos2d-mono\cocos2d\cocos2d.Core.Windows\bin", "..\cocos2d-mono\cocos2d\cocos2d.Core.Windows\obj"
foreach($BuildPath in $BuildPaths)
{
    if (Test-Path $BuildPath) {
        Remove-Item $BuildPath -Recurse -Force
    }
}