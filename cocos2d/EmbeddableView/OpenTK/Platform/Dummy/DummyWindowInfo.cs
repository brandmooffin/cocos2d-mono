using System;
using MonoGame.OpenGL;

namespace cocos2d.EmbeddableView.OpenTK.Platform.Dummy
{
    internal class DummyWindowInfo : IWindowInfo
    {
        public void Dispose()
        {
        }

        public IntPtr Handle
        {
            get { return IntPtr.Zero; }
        }
    }
}

