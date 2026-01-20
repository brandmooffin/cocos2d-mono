#if IOS || __IOS__
using System;
using System.ComponentModel;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ObjCRuntime;
using OpenGLES;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using OpenTK.Platform.iPhoneOS;
using UIKit;

namespace Cocos2D
{
    /// <summary>
    /// iOS-specific time source for the game view.
    /// </summary>
    class GameViewTimeSource
    {
        TimeSpan _timeout;
        NSTimer _timer;
        CCGameView _view;

        public GameViewTimeSource(CCGameView view, double updatesPerSecond)
        {
            _view = view;

            // Can't use TimeSpan.FromSeconds() as that only has 1ms
            // resolution, and we need better (e.g. 60fps doesn't fit nicely
            // in 1ms resolution, but does in ticks).
            _timeout = new TimeSpan((long)(((1.0 * TimeSpan.TicksPerSecond) / updatesPerSecond) + 0.5));
        }

        public void Suspend()
        {
            if (_timer != null)
            {
                _timer.Invalidate();
                _timer = null;
            }
        }

        public void Resume()
        {
            if (_timeout != new TimeSpan(-1))
            {
                _timer = NSTimer.CreateRepeatingTimer(_timeout, _view.RunIteration);
                NSRunLoop.Main.AddTimer(_timer, NSRunLoopMode.Common);
            }
        }

        public void Invalidate()
        {
            if (_timer != null)
            {
                _timer.Invalidate();
                _timer = null;
            }
        }
    }

    /// <summary>
    /// iOS-specific partial implementation of CCGameView.
    /// Inherits from iPhoneOSGameView to provide OpenGL rendering on iOS.
    /// </summary>
    [Register("CCGameView"), DesignTimeVisible(true)]
    public partial class CCGameView : iPhoneOSGameView
    {
        bool _bufferCreated;
        uint _depthbuffer;

        GameViewTimeSource _timeSource;

        NSObject _backgroundObserver;
        NSObject _foregroundObserver;

        #region Constructors

        /// <summary>
        /// Creates a new CCGameView from a coder (for Interface Builder).
        /// </summary>
        [Export("initWithCoder:")]
        public CCGameView(NSCoder coder)
            : base(coder)
        {
            BeginInitialise();
        }

        /// <summary>
        /// Creates a new CCGameView with the specified frame.
        /// </summary>
        public CCGameView(CGRect frame)
            : base(frame)
        {
            BeginInitialise();
        }

        void BeginInitialise()
        {
            LayerRetainsBacking = true;
            LayerColorFormat = EAGLColorFormat.RGBA8;
            ContextRenderingApi = EAGLRenderingAPI.OpenGLES2;
            ContentScaleFactor = UIScreen.MainScreen.Scale;
        }

        #endregion Constructors

        #region Initialisation

        partial void PlatformInitialise()
        {
            AutoResize = true;
            _backgroundObserver = NSNotificationCenter.DefaultCenter.AddObserver(
                UIApplication.DidEnterBackgroundNotification, (n) => Paused = true);
            _foregroundObserver = NSNotificationCenter.DefaultCenter.AddObserver(
                UIApplication.WillEnterForegroundNotification, (n) => Paused = false);
        }

        partial void PlatformInitialiseGraphicsDevice(ref PresentationParameters presParams)
        {
            // iOS-specific graphics device initialization
        }

        partial void PlatformStartGame()
        {
            if (_timeSource != null)
                _timeSource.Invalidate();

            _timeSource = new GameViewTimeSource(this, 60.0f);

            CreateFrameBuffer();

            _timeSource.Resume();
        }

        /// <summary>
        /// Creates the frame buffer.
        /// </summary>
        protected override void CreateFrameBuffer()
        {
            RemoveExistingView();

            CAEAGLLayer eaglLayer = (CAEAGLLayer)Layer;

            if (_bufferCreated || eaglLayer.Bounds.Size.Width == 0 || eaglLayer.Bounds.Size.Height == 0)
                return;

            base.CreateFrameBuffer();

            MakeCurrent();

            var newSize = new System.Drawing.Size(
                (int)Math.Round(eaglLayer.Bounds.Size.Width * Layer.ContentsScale),
                (int)Math.Round(eaglLayer.Bounds.Size.Height * Layer.ContentsScale));

            GL.GenRenderbuffers(1, out _depthbuffer);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _depthbuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferInternalFormat.DepthComponent16, newSize.Width, newSize.Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferSlot.DepthAttachment, RenderbufferTarget.Renderbuffer, _depthbuffer);

            Size = newSize;

            Initialise();

            // For iOS, MonoGame's GraphicsDevice needs to maintain reference to default framebuffer
            if (_graphicsDevice != null)
            {
                // Note: glFramebuffer property may not be accessible in all MonoGame versions
                // This is a platform-specific detail
            }

            _bufferCreated = true;
        }

        /// <summary>
        /// Called when the view is resized.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            ViewSize = new CCSize(Size.Width, Size.Height);
            _viewportDirty = true;
        }

        /// <summary>
        /// Called when subviews need to be laid out.
        /// </summary>
        public override void LayoutSubviews()
        {
            // Called when the dimensions of our view change
            // E.g. When rotating the device and autoresizing
            var newSize = new System.Drawing.Size(
                (int)Math.Round(Layer.Bounds.Size.Width * Layer.ContentsScale),
                (int)Math.Round(Layer.Bounds.Size.Height * Layer.ContentsScale));

            if (newSize.Width == 0 || newSize.Height == 0)
                return;

            CreateFrameBuffer();

            if ((Framebuffer + _depthbuffer + Renderbuffer == 0) || EAGLContext == null)
                return;

            Size = newSize;

            var eaglLayer = Layer as CAEAGLLayer;

            // Do not call base because iPhoneOSGameView:LayoutSubviews
            // destroys our graphics context
            // Instead we will manually rejig our buffer storage

            MakeCurrent();

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, Renderbuffer);
            EAGLContext.RenderBufferStorage((uint)All.Renderbuffer, eaglLayer);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0, RenderbufferTarget.Renderbuffer, Renderbuffer);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _depthbuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferInternalFormat.DepthComponent16, newSize.Width, newSize.Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferSlot.DepthAttachment, RenderbufferTarget.Renderbuffer, _depthbuffer);

            _platformInitialised = true;
            LoadGame();
        }

        partial void InitialiseInputHandling()
        {
            InitialiseMobileInputHandling();
        }

        #endregion Initialisation

        #region Cleaning up

        partial void PlatformDispose(bool disposing)
        {
            MakeCurrent();

            if (disposing)
            {
                if (_timeSource != null)
                {
                    _timeSource.Invalidate();
                    _timeSource = null;
                }
            }

            if (_backgroundObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_backgroundObserver);

            if (_foregroundObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_foregroundObserver);
        }

        partial void PlatformCanDisposeGraphicsDevice(ref bool canDispose)
        {
            try
            {
                MakeCurrent();
                canDispose = true;
            }
            catch (Exception)
            {
                canDispose = false;
            }
        }

        /// <summary>
        /// Destroys the frame buffer.
        /// </summary>
        protected override void DestroyFrameBuffer()
        {
            MakeCurrent();

            GL.DeleteRenderbuffers(1, ref _depthbuffer);
            _depthbuffer = 0;

            base.DestroyFrameBuffer();

            _bufferCreated = false;
        }

        /// <summary>
        /// Called when the view is about to move to a window.
        /// </summary>
        public override void WillMoveToWindow(UIWindow window)
        {
            if (window != null)
                base.WillMoveToWindow(window);
        }

        #endregion Cleaning up

        #region Run loop

        partial void PlatformUpdatePaused()
        {
            if (Paused)
                _timeSource.Suspend();
            else
                _timeSource.Resume();

            MobilePlatformUpdatePaused();
        }

        /// <summary>
        /// Internal method called by the timer to run one iteration of the game loop.
        /// </summary>
        internal void RunIteration(NSTimer timer)
        {
            if (GL.GetErrorCode() != ErrorCode.NoError)
                return;

            OnUpdateFrame(null);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);

            OnRenderFrame(null);
        }

        /// <summary>
        /// Called on each render frame.
        /// </summary>
        protected override void OnRenderFrame(global::OpenTK.FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            if (GraphicsContext == null || GraphicsContext.IsDisposed)
                return;

            if (!GraphicsContext.IsCurrent)
                MakeCurrent();

            Draw();

            Present();
        }

        partial void PlatformPresent()
        {
            if (_graphicsDevice != null)
                _graphicsDevice.Present();

            SwapBuffers();
        }

        /// <summary>
        /// Called on each update frame.
        /// </summary>
        protected override void OnUpdateFrame(global::OpenTK.FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            Tick();
        }

        partial void ProcessInput()
        {
            ProcessMobileInput();
        }

        #endregion Run loop

        #region Touch handling

        partial void PlatformUpdateTouchEnabled()
        {
            UserInteractionEnabled = TouchEnabled;
        }

        /// <summary>
        /// Called when touches begin.
        /// </summary>
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            FillTouchCollection(touches);
        }

        /// <summary>
        /// Called when touches end.
        /// </summary>
        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            FillTouchCollection(touches);
        }

        /// <summary>
        /// Called when touches move.
        /// </summary>
        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);
            FillTouchCollection(touches);
        }

        /// <summary>
        /// Called when touches are cancelled.
        /// </summary>
        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
            FillTouchCollection(touches);
        }

        void FillTouchCollection(NSSet touches)
        {
            if (touches.Count == 0)
                return;

            foreach (UITouch touch in touches)
            {
                var location = touch.LocationInView(touch.View);
                var position = new CCPoint((float)(location.X * Layer.ContentsScale), (float)(location.Y * Layer.ContentsScale));

                var id = touch.Handle.ToInt32();

                switch (touch.Phase)
                {
                    case UITouchPhase.Moved:
                        UpdateIncomingMoveTouch(id, ref position);
                        break;
                    case UITouchPhase.Began:
                        AddIncomingNewTouch(id, ref position);
                        break;
                    case UITouchPhase.Ended:
                    case UITouchPhase.Cancelled:
                        UpdateIncomingReleaseTouch(id);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion Touch handling
    }
}
#endif
