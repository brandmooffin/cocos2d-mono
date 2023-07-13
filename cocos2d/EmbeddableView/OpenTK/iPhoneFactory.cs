using System;
using System.Collections.Generic;
using System.Text;
using MonoGame.OpenGL;
using OpenGLES;

using OpenTK.Graphics;
using OpenTK.Windowing.Common;

namespace cocos2d.EmbeddableView.OpenTK
{
    internal class iPhoneFactory : PlatformFactoryBase
    {
        public override IGraphicsContext CreateGLContext(GraphicsMode mode, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags)
        {
            return new iPhoneOS.iPhoneOSGraphicsContext(mode, window, shareContext, major, minor, flags);
        }

        public override IGraphicsContext CreateGLContext(ContextHandle handle, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags)
        {
            return new iPhoneOS.iPhoneOSGraphicsContext(handle, window, shareContext, major, minor, flags);
        }

        public override GraphicsContext.GetCurrentContextDelegate CreateGetCurrentGraphicsContext()
        {
            return () => {
                EAGLContext c = EAGLContext.CurrentContext;
                IntPtr h = IntPtr.Zero;
                if (c != null)
                {
                    h = c.Handle;
                }
                return new ContextHandle(h);
            };
        }

        public override INativeWindow CreateNativeWindow(int x, int y, int width, int height, string title, GraphicsMode mode, GameWindowFlags options, DisplayDevice device)
        {
            throw new NotImplementedException();
        }

        public override IDisplayDeviceDriver CreateDisplayDeviceDriver()
        {
            return new AndroidDisplayDeviceDriver();
        }

        public override OpenTK.Input.IKeyboardDriver2 CreateKeyboardDriver()
        {
            throw new NotImplementedException();
        }

        public override OpenTK.Input.IMouseDriver2 CreateMouseDriver()
        {
            throw new NotImplementedException();
        }

        public override OpenTK.Input.IJoystickDriver2 CreateJoystickDriver()
        {
            throw new NotImplementedException();
        }
    }
}

