using System;
using System.Diagnostics;
using MonoGame.OpenGL;

namespace cocos2d.EmbeddableView.OpenTK.Graphics
{
    // Provides the foundation for all IGraphicsContext implementations.
    internal abstract class GraphicsContextBase : IGraphicsContext, IGraphicsContextInternal, IEquatable<IGraphicsContextInternal>
    {
        protected ContextHandle Handle;
        protected GraphicsMode Mode;

        public abstract void SwapBuffers();

        public abstract void MakeCurrent(IWindowInfo window);

        public abstract bool IsCurrent { get; }

        public bool IsDisposed { get; protected set; }

        public bool VSync
        {
            get { return SwapInterval > 0; }
            set
            {
                if (value && SwapInterval <= 0)
                {
                    SwapInterval = 1;
                }
                else if (!value && SwapInterval > 0)
                {
                    SwapInterval = 0;
                }
            }
        }

        public abstract int SwapInterval { get; set; }

        public virtual void Update(IWindowInfo window) { }

        public GraphicsMode GraphicsMode { get { return Mode; } }

        public bool ErrorChecking
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IGraphicsContext Implementation { get { return this; } }

        public abstract void LoadAll();

        public ContextHandle Context { get { return Handle; } }

        // This function is no longer used.
        // The GraphicsContext facade will
        // always call the IntPtr overload.
        public IntPtr GetAddress(string function)
        {
            throw new NotImplementedException();
        }

        public abstract IntPtr GetAddress(IntPtr function);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);

#if DEBUG
        ~GraphicsContextBase()
        {
            Dispose(false);
            Debug.Print("[Warning] {0}:{1} leaked. Did you forget to call Dispose()?",
                GetType().FullName, Handle);
        }
#endif

        public bool Equals(IGraphicsContextInternal other)
        {
            return Context.Equals(other.Context);
        }

        public override string ToString()
        {
            return string.Format("[{0}: IsCurrent={1}, IsDisposed={2}, VSync={3}, SwapInterval={4}, GraphicsMode={5}, Context={6}]",
                GetType().Name, IsCurrent, IsDisposed, VSync, SwapInterval, GraphicsMode, Context);
        }

        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return
                obj is IGraphicsContextInternal &&
                Equals((IGraphicsContextInternal)obj);
        }
    }
}

