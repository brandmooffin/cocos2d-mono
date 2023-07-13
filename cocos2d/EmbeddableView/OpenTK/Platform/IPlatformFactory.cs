using System;
using GameController;
using MonoGame.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace cocos2d.EmbeddableView.OpenTK.Platform
{
    internal interface IPlatformFactory : IDisposable
    {
        INativeWindow CreateNativeWindow(int x, int y, int width, int height, string title, GraphicsMode mode, GameWindowFlags options, DisplayDevice device);

        IDisplayDeviceDriver CreateDisplayDeviceDriver();

        IGraphicsContext CreateGLContext(GraphicsMode mode, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags);

        IGraphicsContext CreateGLContext(ContextHandle handle, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags);

        OpenTK.GraphicsContext.GetCurrentContextDelegate CreateGetCurrentGraphicsContext();

        OpenTK.Input.IKeyboardDriver2 CreateKeyboardDriver();

        OpenTK.Input.IMouseDriver2 CreateMouseDriver();

        OpenTK.Input.IGamePadDriver CreateGamePadDriver();

        OpenTK.Input.IJoystickDriver2 CreateJoystickDriver();

        void RegisterResource(IDisposable resource);
    }
}

