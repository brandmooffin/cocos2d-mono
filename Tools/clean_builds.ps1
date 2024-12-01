$BuildPaths = "..\cocos2d-mono\cocos2d\cocos2d.Android\bin", "..\cocos2d-mono\cocos2d\cocos2d.Android\obj", 
    "..\cocos2d-mono\cocos2d\cocos2d.Core.Android\bin", "..\cocos2d-mono\cocos2d\cocos2d.Core.Android\obj", 
    "..\cocos2d-mono\cocos2d\cocos2d.iOS\bin", "..\cocos2d-mono\cocos2d\cocos2d.iOS\obj", 
    "..\cocos2d-mono\cocos2d\cocos2d.Core.iOS\bin", "..\cocos2d-mono\cocos2d\cocos2d.Core.iOS\obj", 
    "..\cocos2d-mono\cocos2d\cocos2d.DesktopGL\bin", "..\cocos2d-mono\cocos2d\cocos2d.DesktopGL\obj", 
    "..\cocos2d-mono\cocos2d\cocos2d.Core.DesktopGL\bin", "..\cocos2d-mono\cocos2d\cocos2d.Core.DesktopGL\obj", 
    "..\cocos2d-mono\cocos2d\cocos2d.Windows\bin", "..\cocos2d-mono\cocos2d\cocos2d.Windows\obj", 
    "..\cocos2d-mono\cocos2d\cocos2d.Core.Windows\bin", "..\cocos2d-mono\cocos2d\cocos2d.Core.Windows\obj",
    "..\cocos2d-mono\box2d\box2d.Android\bin", "..\cocos2d-mono\box2d\box2d.Android\obj", 
    "..\cocos2d-mono\box2d\box2d.iOS\bin", "..\cocos2d-mono\box2d\box2d.iOS\obj", 
    "..\cocos2d-mono\box2d\box2d.DesktopGL\bin", "..\cocos2d-mono\box2d\box2d.DesktopGL\obj", 
    "..\cocos2d-mono\box2d\box2d.Windows\bin", "..\cocos2d-mono\box2d\box2d.Windows\obj"
foreach($BuildPath in $BuildPaths)
{
    if (Test-Path $BuildPath) {
        Remove-Item $BuildPath -Recurse -Force
    }
}