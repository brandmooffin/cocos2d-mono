using System;
namespace cocos2d.EmbeddableView.OpenTK.Platform
{
    internal interface IDisplayDeviceDriver
    {
        bool TryChangeResolution(DisplayDevice device, DisplayResolution resolution);
        bool TryRestoreResolution(DisplayDevice device);
        DisplayDevice GetDisplay(DisplayIndex displayIndex);
    }
}

