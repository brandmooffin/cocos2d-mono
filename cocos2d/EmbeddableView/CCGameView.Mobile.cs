using System;
using System.Collections.Generic;

namespace Cocos2D
{
    /// <summary>
    /// Mobile-specific partial implementation of CCGameView.
    /// Contains shared touch handling code for iOS and Android.
    /// </summary>
    public partial class CCGameView
    {
        static readonly TimeSpan TouchTimeLimit = TimeSpan.FromMilliseconds(1000);

        bool _touchEnabled;

        Dictionary<int, CCTouch> _touchMap;
        List<CCTouch> _incomingNewTouches;
        List<CCTouch> _incomingMoveTouches;
        List<CCTouch> _incomingReleaseTouches;

        object _touchLock = new object();

        #region Properties

        /// <summary>
        /// Gets or sets whether touch input is enabled.
        /// </summary>
        public bool TouchEnabled
        {
            get { return _touchEnabled; }
            set
            {
                _touchEnabled = value;
                PlatformUpdateTouchEnabled();
            }
        }

        /// <summary>
        /// Gets the accelerometer for this view.
        /// </summary>
        public CCAccelerometer Accelerometer { get { return CCDirector.SharedDirector.Accelerometer; } }

        #endregion Properties

        #region Initialisation

        void InitialiseMobileInputHandling()
        {
            _touchMap = new Dictionary<int, CCTouch>();
            _incomingNewTouches = new List<CCTouch>();
            _incomingMoveTouches = new List<CCTouch>();
            _incomingReleaseTouches = new List<CCTouch>();

            TouchEnabled = true;
        }

        #endregion Initialisation

        #region Pause handling

        void MobilePlatformUpdatePaused()
        {
            // Handle accelerometer state when pausing/resuming
            // The CCAccelerometer in cocos2d-mono handles this differently
        }

        void DesktopPlatformUpdatePaused()
        {
            // Desktop-specific pause handling if needed
        }

        #endregion Pause handling

        #region Touch handling

        /// <summary>
        /// Adds a new touch to the incoming touch queue.
        /// </summary>
        protected void AddIncomingNewTouch(int touchId, ref CCPoint position)
        {
            lock (_touchLock)
            {
                if (!_touchMap.ContainsKey(touchId))
                {
                    var touch = new CCTouch(touchId, position.X, position.Y);
                    _touchMap.Add(touchId, touch);
                    _incomingNewTouches.Add(touch);
                }
            }
        }

        /// <summary>
        /// Updates an existing touch with move information.
        /// </summary>
        protected void UpdateIncomingMoveTouch(int touchId, ref CCPoint position)
        {
            lock (_touchLock)
            {
                CCTouch existingTouch;
                if (_touchMap.TryGetValue(touchId, out existingTouch))
                {
                    var delta = existingTouch.LocationInView - position;
                    if (delta.LengthSquared > 1.0f)
                    {
                        _incomingMoveTouches.Add(existingTouch);
                        existingTouch.SetTouchInfo(touchId, position.X, position.Y);
                    }
                }
            }
        }

        /// <summary>
        /// Updates an existing touch to indicate it has been released.
        /// </summary>
        protected void UpdateIncomingReleaseTouch(int touchId)
        {
            lock (_touchLock)
            {
                CCTouch existingTouch;
                if (_touchMap.TryGetValue(touchId, out existingTouch))
                {
                    _incomingReleaseTouches.Add(existingTouch);
                    _touchMap.Remove(touchId);
                }
            }
        }

        void ProcessMobileInput()
        {
            lock (_touchLock)
            {
                var touchDispatcher = Director.TouchDispatcher;
                if (touchDispatcher != null && touchDispatcher.IsDispatchEvents)
                {
                    RemoveOldTouches();

                    if (_incomingNewTouches.Count > 0)
                    {
                        touchDispatcher.TouchesBegan(_incomingNewTouches);
                    }

                    if (_incomingMoveTouches.Count > 0)
                    {
                        touchDispatcher.TouchesMoved(_incomingMoveTouches);
                    }

                    if (_incomingReleaseTouches.Count > 0)
                    {
                        touchDispatcher.TouchesEnded(_incomingReleaseTouches);
                    }

                    _incomingNewTouches.Clear();
                    _incomingMoveTouches.Clear();
                    _incomingReleaseTouches.Clear();
                }
            }

            // Update accelerometer if available and enabled
#if !NETFX_CORE
            if (Accelerometer != null)
            {
                Accelerometer.Update();
            }
#endif
        }

        // Prevent memory leaks by removing stale touches
        // In particular, in the case of the game entering the background
        // a release touch event may not have been triggered within the view
        void RemoveOldTouches()
        {
            // Note: In cocos2d-mono, touch timeout handling is different
            // The original CocosSharp implementation tracked timestamp on touches
            // We'll keep this simplified for now
        }

        /// <summary>
        /// Platform-specific touch enabled state update.
        /// </summary>
        partial void PlatformUpdateTouchEnabled();

        #endregion Touch handling
    }
}
