#if DESKTOPGL || WINDOWS || WINDOWSGL || MACOS || LINUX
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Cocos2D
{
    /// <summary>
    /// Desktop-specific partial implementation of CCGameView.
    /// Provides a game view that can be embedded in desktop applications.
    /// Note: On desktop platforms, this typically wraps an existing Game instance.
    /// </summary>
    public partial class CCGameView
    {
        Game _game;
        GraphicsDeviceManager _graphicsDeviceManager;
        bool _mouseEnabled = true;
        int _lastMouseId;
        MouseState _lastMouseState;
        MouseState _prevMouseState;

        #region Desktop Constructors

        /// <summary>
        /// Creates a new CCGameView from an existing MonoGame Game instance.
        /// </summary>
        /// <param name="game">The MonoGame Game instance.</param>
        /// <param name="graphics">The graphics device manager.</param>
        public CCGameView(Game game, GraphicsDeviceManager graphics)
        {
            _game = game;
            _graphicsDeviceManager = graphics;
            _graphicsDevice = game.GraphicsDevice;
            ViewSize = new CCSize(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        #endregion Desktop Constructors

        #region Properties

        /// <summary>
        /// Gets or sets whether mouse input is enabled.
        /// </summary>
        public bool MouseEnabled
        {
            get { return _mouseEnabled; }
            set { _mouseEnabled = value; }
        }

        #endregion Properties

        #region Initialisation

        partial void PlatformInitialise()
        {
            // Desktop-specific initialization
            _platformInitialised = true;
        }

        partial void PlatformInitialiseGraphicsDevice(ref PresentationParameters presParams)
        {
            // For desktop, we use the existing graphics device from the Game
            presParams.BackBufferWidth = (int)ViewSize.Width;
            presParams.BackBufferHeight = (int)ViewSize.Height;
        }

        partial void PlatformStartGame()
        {
            // Desktop platforms typically use the MonoGame game loop directly
        }

        partial void InitialiseInputHandling()
        {
            // Initialize desktop input handling (keyboard, mouse)
        }

        #endregion Initialisation

        #region Run loop

        partial void PlatformUpdatePaused()
        {
            DesktopPlatformUpdatePaused();
        }

        partial void ProcessInput()
        {
            ProcessDesktopInput();
        }

        void ProcessDesktopInput()
        {
            if (_mouseEnabled)
            {
                ProcessMouse();
            }
        }

        void ProcessMouse()
        {
            _prevMouseState = _lastMouseState;
            _lastMouseState = Mouse.GetState();

            var touchDispatcher = Director.TouchDispatcher;
            if (touchDispatcher == null || !touchDispatcher.IsDispatchEvents)
                return;

            var newTouches = new System.Collections.Generic.List<CCTouch>();
            var movedTouches = new System.Collections.Generic.List<CCTouch>();
            var endedTouches = new System.Collections.Generic.List<CCTouch>();

            if (_prevMouseState.LeftButton == ButtonState.Released && _lastMouseState.LeftButton == ButtonState.Pressed)
            {
                var pos = CCDrawManager.ScreenToWorld(_lastMouseState.X, _lastMouseState.Y);
                _lastMouseId++;
                var touch = new CCTouch(_lastMouseId, pos.X, pos.Y);
                newTouches.Add(touch);
            }
            else if (_prevMouseState.LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton == ButtonState.Pressed)
            {
                if (_prevMouseState.X != _lastMouseState.X || _prevMouseState.Y != _lastMouseState.Y)
                {
                    var pos = CCDrawManager.ScreenToWorld(_lastMouseState.X, _lastMouseState.Y);
                    var touch = new CCTouch(_lastMouseId, pos.X, pos.Y);
                    movedTouches.Add(touch);
                }
            }
            else if (_prevMouseState.LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton == ButtonState.Released)
            {
                var pos = CCDrawManager.ScreenToWorld(_lastMouseState.X, _lastMouseState.Y);
                var touch = new CCTouch(_lastMouseId, pos.X, pos.Y);
                endedTouches.Add(touch);
            }

            if (newTouches.Count > 0)
            {
                touchDispatcher.TouchesBegan(newTouches);
            }

            if (movedTouches.Count > 0)
            {
                touchDispatcher.TouchesMoved(movedTouches);
            }

            if (endedTouches.Count > 0)
            {
                touchDispatcher.TouchesEnded(endedTouches);
            }
        }

        #endregion Run loop

        #region Rendering

        partial void PlatformPresent()
        {
            // Desktop presentation is handled by the MonoGame game loop
        }

        #endregion Rendering

        #region Cleanup

        partial void PlatformDispose(bool disposing)
        {
            // Desktop-specific cleanup
        }

        partial void PlatformCanDisposeGraphicsDevice(ref bool canDispose)
        {
            // On desktop, the graphics device is typically managed by the Game
            canDispose = false;
        }

        #endregion Cleanup
    }
}
#endif
