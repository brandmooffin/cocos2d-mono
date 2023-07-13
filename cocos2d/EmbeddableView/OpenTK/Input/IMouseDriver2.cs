using System;
using Microsoft.Xna.Framework.Input;

namespace cocos2d.EmbeddableView.OpenTK.Input
{
    internal interface IMouseDriver2
    {
        MouseState GetState();
        MouseState GetState(int index);
        void SetPosition(double x, double y);
        MouseState GetCursorState();
    }
}

