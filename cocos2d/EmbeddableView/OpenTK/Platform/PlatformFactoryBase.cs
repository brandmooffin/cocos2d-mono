using System;
using cocos2d.EmbeddableView.OpenTK.Graphics;
using cocos2d.EmbeddableView.OpenTK.Input;
using MonoGame.OpenGL;
using System.Collections.Generic;
using System.Diagnostics;
using GraphicsContext = cocos2d.EmbeddableView.OpenTK.Graphics.GraphicsContext;

namespace cocos2d.EmbeddableView.OpenTK.Platform
{
    /// \internal
    /// <summary>
    /// Implements IPlatformFactory functionality that is common
    /// for all platform backends. IPlatformFactory implementations
    /// should inherit from this class.
    /// </summary>
    internal abstract class PlatformFactoryBase : IPlatformFactory
    {
        private static readonly object sync = new object();
        private readonly List<IDisposable> Resources = new List<IDisposable>();

        protected bool IsDisposed;

        public PlatformFactoryBase()
        {
        }

        public abstract INativeWindow CreateNativeWindow(int x, int y, int width, int height, string title, GraphicsMode mode, GameWindowFlags options, DisplayDevice device);

        public abstract IDisplayDeviceDriver CreateDisplayDeviceDriver();

        public abstract IGraphicsContext CreateGLContext(GraphicsMode mode, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags);

        public virtual IGraphicsContext CreateGLContext(ContextHandle handle, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags)
        {
            throw new NotImplementedException();
        }

        public abstract GraphicsContext.GetCurrentContextDelegate CreateGetCurrentGraphicsContext();

        public abstract IKeyboardDriver2 CreateKeyboardDriver();

        public abstract IMouseDriver2 CreateMouseDriver();

        public virtual IGamePadDriver CreateGamePadDriver()
        {
            return new MappedGamePadDriver();
        }

        public abstract IJoystickDriver2 CreateJoystickDriver();

        public void RegisterResource(IDisposable resource)
        {
            lock (sync)
            {
                Resources.Add(resource);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool manual)
        {
            if (!IsDisposed)
            {
                if (manual)
                {
                    lock (sync)
                    {
                        foreach (var resource in Resources)
                        {
                            resource.Dispose();
                        }
                        Resources.Clear();
                    }
                }
                else
                {
                    Debug.Print("[OpenTK] {0} leaked with {1} live resources, did you forget to call Dispose()?",
                        GetType().FullName, Resources.Count);
                }
                IsDisposed = true;
            }
        }

        ~PlatformFactoryBase()
        {
            Dispose(false);
        }
    }
}

