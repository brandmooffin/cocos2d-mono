using System;
using Microsoft.Xna.Framework.Input;

namespace cocos2d.EmbeddableView.OpenTK.Input
{
    internal interface IJoystickDriver2
    {
        JoystickState GetState(int index);
        JoystickCapabilities GetCapabilities(int index);
        Guid GetGuid(int index);
    }
}

