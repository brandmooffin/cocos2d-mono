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

namespace Cocos2D
{
    /// <summary>
    /// Android-specific partial implementation of CCGameView.
    /// Uses Android SurfaceView for rendering with MonoGame graphics integration.
    /// </summary>
    public partial class CCGameView : SurfaceView, ISurfaceHolderCallback, View.IOnTouchListener
    {
        bool _running;
        Thread _renderThread;
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
            FocusableInTouchMode = true;
            Holder.AddCallback(this);
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
            presParams.DeviceWindowHandle = Holder.Surface.Handle;
        }

        partial void PlatformStartGame()
        {
            lock (_androidViewLock)
            {
                if (!_running)
                {
                    _running = true;
                    _renderThread = new Thread(RenderLoop);
                    _renderThread.Start();
                }
            }
        }

        partial void InitialiseInputHandling()
        {
            InitialiseMobileInputHandling();
        }

        #endregion Initialisation

        #region Cleaning up

        partial void PlatformDispose(bool disposing)
        {
            _running = false;
            _renderThread?.Join();

            var context = Android.App.Application.Context;
            if (_screenLockHandler != null)
            {
                context.UnregisterReceiver(_screenLockHandler);
                _screenLockHandler = null;
            }
        }

        partial void PlatformCanDisposeGraphicsDevice(ref bool canDispose)
        {
            canDispose = true;
        }

        #endregion Cleaning up

        #region Run loop

        partial void PlatformUpdatePaused()
        {
            if (Paused)
            {
                ClearFocus();
            }
            else
            {
                if (!IsFocused)
                    RequestFocus();
            }

            MobilePlatformUpdatePaused();
        }

        void RenderLoop()
        {
            Initialise();
            _platformInitialised = true;
            LoadGame();

            while (_running)
            {
                if (!Paused)
                {
                    Tick();
                    Draw();
                    PlatformPresentInternal();
                }
                else
                {
                    Thread.Sleep(16);
                }
            }
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
            }
            catch (Exception ex)
            {
                CCLog.Log("CCGameView: Error in present. Paused={0}, GraphicsDevice={1}. Error: {2}",
                    Paused, _graphicsDevice != null ? "available" : "null", ex.Message);
            }
        }

        #endregion Rendering

        #region ISurfaceHolderCallback

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            lock (_androidViewLock)
            {
                Paused = true;
            }
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            lock (_androidViewLock)
            {
                Paused = false;
            }
        }

        public void SurfaceChanged(ISurfaceHolder holder, Android.Graphics.Format format, int width, int height)
        {
            lock (_androidViewLock)
            {
                ViewSize = new CCSize(width, height);
                _viewportDirty = true;
            }
        }

        #endregion ISurfaceHolderCallback

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
