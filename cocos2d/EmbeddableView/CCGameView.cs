using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace Cocos2D
{
    /// <summary>
    /// CCGameView is an embeddable view that hosts a cocos2d-mono game.
    /// This is a partial class with platform-specific implementations.
    /// </summary>
    public partial class CCGameView : IDisposable
    {
        // (10 mill ticks per second / 60 fps) (rounded up)
        const int NumOfTicksPerUpdate = 166667;
        const int MaxUpdateTimeMilliseconds = 500;

        static readonly CCRect ExactFitViewportRatio = new CCRect(0, 0, 1, 1);

        // Currently, we have a limitation that at most one view instance can be active at any point in time
        static WeakReference s_currentViewInstance;

        /// <summary>
        /// Internal graphics device service implementation.
        /// </summary>
        class CCGraphicsDeviceService : IGraphicsDeviceService
        {
            public GraphicsDevice GraphicsDevice { get; private set; }

            public CCGraphicsDeviceService(GraphicsDevice graphicsDevice)
            {
                GraphicsDevice = graphicsDevice;
            }

            public event EventHandler<EventArgs> DeviceCreated;
            public event EventHandler<EventArgs> DeviceDisposing;
            public event EventHandler<EventArgs> DeviceReset;
            public event EventHandler<EventArgs> DeviceResetting;

            // Suppress warnings about unused events
            internal void RaiseDeviceCreated() => DeviceCreated?.Invoke(this, EventArgs.Empty);
            internal void RaiseDeviceDisposing() => DeviceDisposing?.Invoke(this, EventArgs.Empty);
            internal void RaiseDeviceReset() => DeviceReset?.Invoke(this, EventArgs.Empty);
            internal void RaiseDeviceResetting() => DeviceResetting?.Invoke(this, EventArgs.Empty);
        }

        internal delegate void ViewportChangedEventHandler(CCGameView sender);
        internal event ViewportChangedEventHandler ViewportChanged;

        EventHandler<EventArgs> _viewCreated;

        bool _disposed;
        bool _paused;
        bool _viewInitialised;
        /// <summary>
        /// Indicates whether platform-specific initialization is complete.
        /// Set by platform-specific implementations after initialization.
        /// </summary>
        protected bool _platformInitialised;
        bool _gameLoaded;
        bool _gameStarted;
        bool _viewportDirty;

        CCViewResolutionPolicy _resolutionPolicy = CCViewResolutionPolicy.ShowAll;
        CCRect _viewportRatio = ExactFitViewportRatio;
        CCSize _designResolution = new CCSize(640, 480);

        Matrix _defaultViewMatrix, _defaultProjMatrix;
        Viewport _defaultViewport;
        Viewport _viewport;

        GraphicsDevice _graphicsDevice;
        CCGraphicsDeviceService _graphicsDeviceService;

        GameTime _gameTime;
        TimeSpan _accumulatedElapsedTime;
        TimeSpan _targetElapsedTime;
        TimeSpan _maxElapsedTime;
        Stopwatch _gameTimer;
        long _previousTicks;

        #region Properties

        /// <summary>
        /// Event raised when the view is created and ready for use.
        /// When you subscribe to this event, the game will be loaded.
        /// </summary>
        public event EventHandler<EventArgs> ViewCreated
        {
            add { _viewCreated += value; LoadGame(); }
            remove { _viewCreated -= value; }
        }

        /// <summary>
        /// Gets the director that manages scenes.
        /// </summary>
        public CCDirector Director { get { return CCDirector.SharedDirector; } }

        /// <summary>
        /// Gets the scheduler that manages scheduled callbacks.
        /// </summary>
        public CCScheduler Scheduler { get { return CCDirector.SharedDirector.Scheduler; } }

        /// <summary>
        /// Gets the action manager.
        /// </summary>
        public CCActionManager ActionManager { get { return CCDirector.SharedDirector.ActionManager; } }

        /// <summary>
        /// Gets or sets whether the view is paused.
        /// When paused, the game loop will not update or render.
        /// </summary>
        public bool Paused
        {
            get { return _paused; }
            set
            {
                if (_gameStarted && _paused != value)
                {
                    _paused = value;
                    _previousTicks = _gameTimer.Elapsed.Ticks;
                    PlatformUpdatePaused();
                }
            }
        }

        /// <summary>
        /// Gets or sets the resolution policy that determines how the design resolution
        /// is mapped to the actual view size.
        /// </summary>
        public CCViewResolutionPolicy ResolutionPolicy
        {
            get { return _resolutionPolicy; }
            set
            {
                _resolutionPolicy = value;

                // Reset ratio if using custom resolution policy
                if (_resolutionPolicy == CCViewResolutionPolicy.Custom)
                    _viewportRatio = ExactFitViewportRatio;
                _viewportDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the design resolution size.
        /// This is the logical resolution that your game is designed for.
        /// </summary>
        public CCSize DesignResolution
        {
            get { return _designResolution; }
            set
            {
                _designResolution = value;
                _viewportDirty = true;
            }
        }

        /// <summary>
        /// Gets the actual size of the view in pixels.
        /// </summary>
        public CCSize ViewSize { get; private set; }

        /// <summary>
        /// Gets or sets the custom viewport ratio.
        /// Setting this will automatically change the resolution policy to Custom.
        /// </summary>
        public CCRect ViewportRectRatio
        {
            get { return _viewportRatio; }
            set
            {
                _viewportRatio = value;
                _resolutionPolicy = CCViewResolutionPolicy.Custom;
                _viewportDirty = true;
            }
        }

        /// <summary>
        /// Gets the graphics device used for rendering.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get { return _graphicsDevice; } }

        internal Viewport Viewport
        {
            get
            {
                if (_viewportDirty)
                    UpdateViewport();
                return _viewport;
            }
            private set
            {
                _viewport = value;
                ViewportChanged?.Invoke(this);
            }
        }

        #endregion Properties

        #region Initialisation

        /// <summary>
        /// Starts the game loop.
        /// </summary>
        public void StartGame()
        {
            if (!_gameStarted)
            {
                PlatformStartGame();
                _gameStarted = true;
            }
        }

        /// <summary>
        /// Starts the game and runs the specified scene.
        /// </summary>
        /// <param name="scene">The scene to run.</param>
        public void RunWithScene(CCScene scene)
        {
            StartGame();
            Director.RunWithScene(scene);
        }

        void Initialise()
        {
            if (_viewInitialised)
                return;

            PlatformInitialise();

            // Initialize director subsystems if needed
            if (CCDirector.SharedDirector.NeedsInit)
            {
                CCDirector.SharedDirector.Init();
            }

            InitialiseGraphicsDevice();
            InitialiseRunLoop();
            InitialiseInputHandling();

            _viewInitialised = true;
            s_currentViewInstance = new WeakReference(this);
        }

        void InitialiseGraphicsDevice()
        {
            var presParams = new PresentationParameters();
            presParams.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            presParams.DepthStencilFormat = DepthFormat.Depth24Stencil8;
            presParams.BackBufferFormat = SurfaceFormat.Color;
            PlatformInitialiseGraphicsDevice(ref presParams);

            // Try to create graphics device with hi-def profile
            try
            {
                _graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, presParams);
            }
            // Otherwise, if unsupported defer to using the low-def profile
            catch (NotSupportedException)
            {
                _graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.Reach, presParams);
            }

            CCDrawManager.Init(_graphicsDeviceService = new CCGraphicsDeviceService(_graphicsDevice));
        }

        void InitialiseRunLoop()
        {
            _gameTimer = Stopwatch.StartNew();
            _gameTime = new GameTime();

            _accumulatedElapsedTime = TimeSpan.Zero;
            _targetElapsedTime = TimeSpan.FromTicks(NumOfTicksPerUpdate);
            _maxElapsedTime = TimeSpan.FromMilliseconds(MaxUpdateTimeMilliseconds);
            _previousTicks = 0;
        }

        void LoadGame()
        {
            if (_viewInitialised && _platformInitialised && !_gameLoaded && _viewCreated != null)
            {
                _viewCreated(this, null);
                _gameLoaded = true;
            }
        }

        #endregion Initialisation

        #region Cleaning up

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~CCGameView()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the view and releases all resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the view and releases resources.
        /// </summary>
        /// <param name="disposing">True if called from Dispose(), false if called from finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            PlatformDispose(disposing);

            bool canDisposeGraphics = PlatformCanDisposeGraphicsDevice();

            if (disposing)
            {
                if (_graphicsDevice != null && canDisposeGraphics)
                {
                    _graphicsDevice.Dispose();
                    _graphicsDevice = null;
                }
            }

            s_currentViewInstance = null;
            _disposed = true;
        }

        // MonoGame maintains static references to the underlying context, and more generally, 
        // there's no guarantee that a previous instance of CCGameView will have been disposed by the GC.
        // However, given the limitation imposed by MonoGame to only have one active instance, we need
        // to ensure that this indeed the case and explicitly Dispose of any old game views.
        void RemoveExistingView()
        {
            if (s_currentViewInstance != null && s_currentViewInstance.Target != this)
            {
                var prevGameView = s_currentViewInstance.Target as CCGameView;

                if (prevGameView != null)
                {
                    prevGameView.Paused = true;
                    prevGameView.Dispose();
                }
            }
        }

        #endregion Cleaning up

        #region Drawing

        void UpdateViewport()
        {
            int width = (int)ViewSize.Width;
            int height = (int)ViewSize.Height;

            // The GraphicsDevice BackBuffer dimensions are used by MonoGame when laying out the viewport
            // so make sure they're updated
            _graphicsDevice.PresentationParameters.BackBufferWidth = width;
            _graphicsDevice.PresentationParameters.BackBufferHeight = height;

            if (_resolutionPolicy != CCViewResolutionPolicy.Custom)
            {
                float resolutionScaleX = width / _designResolution.Width;
                float resolutionScaleY = height / _designResolution.Height;

                switch (_resolutionPolicy)
                {
                    case CCViewResolutionPolicy.NoBorder:
                        resolutionScaleX = resolutionScaleY = Math.Max(resolutionScaleX, resolutionScaleY);
                        break;
                    case CCViewResolutionPolicy.ShowAll:
                        resolutionScaleX = resolutionScaleY = Math.Min(resolutionScaleX, resolutionScaleY);
                        break;
                    case CCViewResolutionPolicy.FixedHeight:
                        resolutionScaleX = resolutionScaleY;
                        _designResolution = new CCSize((float)Math.Ceiling(width / resolutionScaleX), _designResolution.Height);
                        break;
                    case CCViewResolutionPolicy.FixedWidth:
                        resolutionScaleY = resolutionScaleX;
                        _designResolution = new CCSize(_designResolution.Width, (float)Math.Ceiling(height / resolutionScaleY));
                        break;
                    default:
                        break;
                }

                float viewPortW = _designResolution.Width * resolutionScaleX;
                float viewPortH = _designResolution.Height * resolutionScaleY;

                CCRect viewPortRect = new CCRect((width - viewPortW) / 2, (height - viewPortH) / 2,
                    viewPortW, viewPortH);

                _viewportRatio = new CCRect(
                    viewPortRect.Origin.X / width,
                    viewPortRect.Origin.Y / height,
                    viewPortRect.Size.Width / width,
                    viewPortRect.Size.Height / height
                );
            }

            Viewport = new Viewport((int)(width * _viewportRatio.Origin.X), (int)(height * _viewportRatio.Origin.Y),
                (int)(width * _viewportRatio.Size.Width), (int)(height * _viewportRatio.Size.Height));

            CCPoint center = new CCPoint(ViewSize.Width / 2.0f, ViewSize.Height / 2.0f);
            _defaultViewMatrix = Matrix.CreateLookAt(new Vector3(center.X, center.Y, 300.0f), new Vector3(center.X, center.Y, 0.0f), Vector3.Up);
            _defaultProjMatrix = Matrix.CreateOrthographic(ViewSize.Width, ViewSize.Height, 1024f, -1024);
            _defaultViewport = new Viewport(0, 0, (int)ViewSize.Width, (int)ViewSize.Height);

            // Update the draw manager with the new resolution
            CCDrawManager.SetDesignResolutionSize(_designResolution.Width, _designResolution.Height, 
                ConvertResolutionPolicy(_resolutionPolicy));

            _viewportDirty = false;
        }

        CCResolutionPolicy ConvertResolutionPolicy(CCViewResolutionPolicy policy)
        {
            switch (policy)
            {
                case CCViewResolutionPolicy.ExactFit:
                    return CCResolutionPolicy.ExactFit;
                case CCViewResolutionPolicy.NoBorder:
                    return CCResolutionPolicy.NoBorder;
                case CCViewResolutionPolicy.ShowAll:
                    return CCResolutionPolicy.ShowAll;
                case CCViewResolutionPolicy.FixedHeight:
                    return CCResolutionPolicy.FixedHeight;
                case CCViewResolutionPolicy.FixedWidth:
                    return CCResolutionPolicy.FixedWidth;
                default:
                    return CCResolutionPolicy.ShowAll;
            }
        }

        void Draw()
        {
            if (CCDrawManager.BeginDraw())
            {
                CCScene runningScene = Director.RunningScene;

                if (runningScene != null)
                {
                    Director.MainLoop(_gameTime);
                }

                CCDrawManager.EndDraw();
            }
        }

        #endregion Drawing

        #region Run loop

        void Tick()
        {
            while (true)
            {
                var currentTicks = _gameTimer.Elapsed.Ticks;
                _accumulatedElapsedTime += TimeSpan.FromTicks(currentTicks - _previousTicks);
                _previousTicks = currentTicks;

                if (_accumulatedElapsedTime >= _targetElapsedTime)
                    break;

                var sleepTime = (int)(_targetElapsedTime - _accumulatedElapsedTime).TotalMilliseconds;
                if (sleepTime > 0)
                {
                    System.Threading.Thread.Sleep(sleepTime);
                }
            }

            if (_accumulatedElapsedTime > _maxElapsedTime)
                _accumulatedElapsedTime = _maxElapsedTime;

            _gameTime.ElapsedGameTime = _targetElapsedTime;
            var stepCount = 0;

            while (_accumulatedElapsedTime >= _targetElapsedTime)
            {
                _gameTime.TotalGameTime += _targetElapsedTime;
                _accumulatedElapsedTime -= _targetElapsedTime;
                ++stepCount;

                Update(_gameTime);
            }

            _gameTime.ElapsedGameTime = TimeSpan.FromTicks(_targetElapsedTime.Ticks * stepCount);
        }

        void Update(GameTime time)
        {
            float deltaTime = (float)_gameTime.ElapsedGameTime.TotalSeconds;

            if (Director.RunningScene != null)
            {
                Scheduler.update(deltaTime);
            }

            ProcessInput();
        }

        #endregion Run loop

        #region Platform-specific partial methods

        /// <summary>
        /// Platform-specific initialization.
        /// </summary>
        partial void PlatformInitialise();

        /// <summary>
        /// Platform-specific graphics device initialization.
        /// </summary>
        partial void PlatformInitialiseGraphicsDevice(ref PresentationParameters presParams);

        /// <summary>
        /// Platform-specific game start.
        /// </summary>
        partial void PlatformStartGame();

        /// <summary>
        /// Platform-specific input handling initialization.
        /// </summary>
        partial void InitialiseInputHandling();

        /// <summary>
        /// Platform-specific input processing.
        /// </summary>
        partial void ProcessInput();

        /// <summary>
        /// Platform-specific pause state update.
        /// </summary>
        partial void PlatformUpdatePaused();

        /// <summary>
        /// Platform-specific presentation/buffer swap.
        /// </summary>
        partial void PlatformPresent();

        /// <summary>
        /// Platform-specific disposal.
        /// </summary>
        partial void PlatformDispose(bool disposing);

        /// <summary>
        /// Platform-specific check if graphics device can be safely disposed.
        /// </summary>
        partial void PlatformCanDisposeGraphicsDevice(ref bool canDispose);

        bool PlatformCanDisposeGraphicsDevice()
        {
            bool canDispose = true;
            PlatformCanDisposeGraphicsDevice(ref canDispose);
            return canDispose;
        }

        #endregion Platform-specific partial methods
    }
}
