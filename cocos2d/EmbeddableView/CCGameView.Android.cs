#if ANDROID
using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenTK.Graphics;
using OpenTK.Platform.Android;

namespace Cocos2D
{
    /// <summary>
    /// Android-specific partial implementation of CCGameView.
    /// Inherits from AndroidGameView to provide OpenGL rendering on Android.
    /// </summary>
    public partial class CCGameView : AndroidGameView, View.IOnTouchListener, ISurfaceHolderCallback
    {
        bool _startedRunning;
        CCAndroidScreenReceiver _screenLockHandler;
        object _androidViewLock = new object();

        #region Android screen lock handling inner class

        class CCAndroidScreenReceiver : BroadcastReceiver
        {
            bool _previousPausedState;
            CCGameView _gameView;

            public CCAndroidScreenReceiver(CCGameView view)
            {
                _gameView = view;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                if (intent.Action == Intent.ActionScreenOff)
                    OnLocked();
                else if (intent.Action == Intent.ActionScreenOn)
                {
                    KeyguardManager keyguard = (KeyguardManager)context.GetSystemService(Context.KeyguardService);
                    if (!keyguard.InKeyguardRestrictedInputMode())
                        OnUnlocked();
                }
                else if (intent.Action == Intent.ActionUserPresent)
                    OnUnlocked();
            }

            void OnLocked()
            {
                _previousPausedState = _gameView.Paused;
                _gameView.Paused = true;
            }

            void OnUnlocked()
            {
                _gameView.Paused = _previousPausedState;
            }
        }

        #endregion Android screen lock handling inner class

        #region Constructors

        /// <summary>
        /// Creates a new CCGameView with the specified context.
        /// </summary>
        public CCGameView(Context context)
            : base(context)
        {
            ViewInit();
        }

        /// <summary>
        /// Creates a new CCGameView with the specified context and attributes.
        /// </summary>
        public CCGameView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            ViewInit();
        }

        void ViewInit()
        {
            RenderOnUIThread = false;
            AutoSetContextOnRenderFrame = true;
            RenderThreadRestartRetries = 100;
            FocusableInTouchMode = true;
            ContextRenderingApi = GLVersion.ES2;
        }

        #endregion Constructors

        #region Initialisation

        partial void PlatformInitialise()
        {
            var context = Android.App.Application.Context;

            IntentFilter filter = new IntentFilter();
            filter.AddAction(Intent.ActionScreenOff);
            filter.AddAction(Intent.ActionScreenOn);
            filter.AddAction(Intent.ActionUserPresent);

            _screenLockHandler = new CCAndroidScreenReceiver(this);
            context.RegisterReceiver(_screenLockHandler, filter);
        }

        partial void PlatformInitialiseGraphicsDevice(ref PresentationParameters presParams)
        {
            // Android-specific graphics device initialization
        }

        partial void PlatformStartGame()
        {
            lock (_androidViewLock)
            {
                Resume();
            }
        }

        /// <summary>
        /// Called when the OpenGL context is set.
        /// </summary>
        protected override void OnContextSet(EventArgs e)
        {
            lock (_androidViewLock)
            {
                base.OnContextSet(e);

                Initialise();

                _platformInitialised = true;
                LoadGame();
            }
        }

        /// <summary>
        /// Creates the frame buffer.
        /// </summary>
        protected override void CreateFrameBuffer()
        {
            lock (_androidViewLock)
            {
                try
                {
                    base.CreateFrameBuffer();
                    // Kick start the render loop
                    // In particular, graphics context is lazily created, so we need to start this up 
                    // here so that the view is initialised correctly
                    if (!_startedRunning)
                    {
                        Run();
                        _startedRunning = true;
                    }
                    return;
                }
                catch (Exception ex)
                {
                    CCLog.Log("CCGameView: Error creating frame buffer. StartedRunning={0}, GraphicsContext={1}. Error: {2}",
                        _startedRunning, GraphicsContext != null ? "available" : "null", ex.Message);
                }
            }
        }

        partial void InitialiseInputHandling()
        {
            InitialiseMobileInputHandling();
        }

        #endregion Initialisation

        #region Cleaning up

        /// <summary>
        /// Called when the OpenGL context is lost.
        /// </summary>
        protected override void OnContextLost(EventArgs e)
        {
            lock (_androidViewLock)
            {
                base.OnContextLost(e);

                if (_graphicsDevice != null)
                    _graphicsDevice.Dispose();
            }
        }

        partial void PlatformDispose(bool disposing)
        {
            var context = Android.App.Application.Context;
            if (_screenLockHandler != null)
            {
                context.UnregisterReceiver(_screenLockHandler);
                _screenLockHandler = null;
            }
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

        #endregion Cleaning up

        #region Run loop

        partial void PlatformUpdatePaused()
        {
            if (Paused)
            {
                Pause();
                ClearFocus();
            }
            else
            {
                Resume();

                if (!IsFocused)
                    RequestFocus();
            }

            MobilePlatformUpdatePaused();
        }

        /// <summary>
        /// Called on each render frame.
        /// </summary>
        protected override void OnRenderFrame(global::OpenTK.FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            if (Paused || GraphicsContext == null || GraphicsContext.IsDisposed)
                return;

            Draw();

            PlatformPresentInternal();
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

        #region Rendering

        partial void PlatformPresent()
        {
            PlatformPresentInternal();
        }

        void PlatformPresentInternal()
        {
            if (Paused)
                return;

            try
            {
                if (_graphicsDevice != null)
                    _graphicsDevice.Present();

                SwapBuffers();
            }
            catch (Exception ex)
            {
                CCLog.Log("CCGameView: Error in swap buffers. Paused={0}, GraphicsDevice={1}, GraphicsContext={2}. Error: {3}",
                    Paused, _graphicsDevice != null ? "available" : "null",
                    GraphicsContext != null && !GraphicsContext.IsDisposed ? "valid" : "invalid",
                    ex.Message);
            }
        }

        void ISurfaceHolderCallback.SurfaceDestroyed(ISurfaceHolder holder)
        {
            lock (_androidViewLock)
            {
                Paused = true;
                SurfaceDestroyed(holder);
            }
        }

        void ISurfaceHolderCallback.SurfaceCreated(ISurfaceHolder holder)
        {
            lock (_androidViewLock)
            {
                SurfaceCreated(holder);
                Paused = false;
            }
        }

        void ISurfaceHolderCallback.SurfaceChanged(ISurfaceHolder holder, Android.Graphics.Format format, int width, int height)
        {
            lock (_androidViewLock)
            {
                SurfaceChanged(holder, format, width, height);
                ViewSize = new CCSize(width, height);
                _viewportDirty = true;
            }
        }

        #endregion Rendering

        #region Touch handling

        partial void PlatformUpdateTouchEnabled()
        {
            SetOnTouchListener(_touchEnabled ? this : null);
        }

        bool IOnTouchListener.OnTouch(View v, MotionEvent e)
        {
            if (!TouchEnabled || Paused)
                return true;

            CCPoint position = new CCPoint(e.GetX(e.ActionIndex), e.GetY(e.ActionIndex));
            int id = e.GetPointerId(e.ActionIndex);
            switch (e.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    AddIncomingNewTouch(id, ref position);
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                    UpdateIncomingReleaseTouch(id);
                    break;
                case MotionEventActions.Move:
                    for (int i = 0; i < e.PointerCount; i++)
                    {
                        id = e.GetPointerId(i);
                        position.X = e.GetX(i);
                        position.Y = e.GetY(i);
                        UpdateIncomingMoveTouch(id, ref position);
                    }
                    break;
                case MotionEventActions.Cancel:
                case MotionEventActions.Outside:
                    for (int i = 0; i < e.PointerCount; i++)
                    {
                        id = e.GetPointerId(i);
                        UpdateIncomingReleaseTouch(id);
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        #endregion Touch handling
    }
}
#endif
