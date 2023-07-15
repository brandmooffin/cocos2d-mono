using System;
using System.Collections.Generic;
using System.Text;
using cocos2d.EmbeddableView.OpenTK;
using cocos2d.EmbeddableView.OpenTK.Platform;
using OpenTK.Platform;

namespace cocos2d.EmbeddableView
{
    internal class iPhoneDisplayDeviceDriver : IDisplayDeviceDriver
    {
        private static DisplayDevice dev;
        static iPhoneDisplayDeviceDriver()
        {
            dev = new DisplayDevice();
            dev.IsPrimary = true;
            dev.BitsPerPixel = 16;
        }

        public DisplayDevice GetDisplay(DisplayIndex displayIndex)
        {
            return (displayIndex == DisplayIndex.First || displayIndex == DisplayIndex.Primary) ? dev : null;
        }


        public bool TryChangeResolution(DisplayDevice device, DisplayResolution resolution)
        {
            return false;
        }

        public bool TryRestoreResolution(DisplayDevice device)
        {
            return false;
        }
    }
}

