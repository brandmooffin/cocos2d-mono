# Task 4.2: macOS Platform Integration - Detailed Breakdown

## Overview
Integrate the Metal backend with macOS platform-specific components, ensuring proper window management, input handling, and macOS-specific optimizations work correctly within the cocos2d-mono framework.

---

## Subtask 4.2.1: Create macOS Metal View Integration
**Time Estimate**: 2 hours  
**Dependencies**: Task 4.1 complete  
**Assignee**: macOS developer with Metal experience

### Steps:

1. **Create macOS Metal view controller**
   ```csharp
   // File: cocos2d/platform/macOS/CCMetalViewController.cs
   #if MACOS && METAL
   using System;
   using Foundation;
   using AppKit;
   using Metal;
   using MetalKit;
   using CoreGraphics;
   using CoreAnimation;
   using Cocos2D.Platform.Metal;
   using Cocos2D.Platform.Factory;
   
   namespace Cocos2D.Platform.macOS
   {
       /// <summary>
       /// macOS view controller that manages Metal rendering for cocos2d-mono
       /// </summary>
       public class CCMetalViewController : NSViewController, IMTKViewDelegate
       {
           private MTKView _metalView;
           private IMTLDevice _device;
           private MetalRenderer _renderer;
           private CVDisplayLink _displayLink;
           
           // Game integration
           private Game _game;
           private bool _isGameRunning = false;
           
           // Window management
           private NSWindow _window;
           private bool _isFullScreen = false;
           
           // Performance tracking
           private DateTime _lastFrameTime = DateTime.Now;
           private int _frameCount = 0;
           private double _fps = 60.0;
           
           public MTKView MetalView => _metalView;
           public MetalRenderer Renderer => _renderer;
           public double CurrentFPS => _fps;
           public NSWindow Window => _window;
           
           public override void ViewDidLoad()
           {
               base.ViewDidLoad();
               
               InitializeMetal();
               SetupView();
               CreateRenderer();
               StartDisplayLink();
               SetupEventHandlers();
               
               CCLog.Log("macOS Metal view controller loaded");
           }
           
           private void InitializeMetal()
           {
               try
               {
                   // Get Metal device
                   _device = MTLDevice.SystemDefault;
                   if (_device == null)
                   {
                       throw new InvalidOperationException("Metal is not available on this Mac");
                   }
                   
                   // Create Metal view
                   _metalView = new MTKView(View.Bounds, _device)
                   {
                       Delegate = this,
                       ColorPixelFormat = MTLPixelFormat.BGRA8Unorm,
                       DepthStencilPixelFormat = MTLPixelFormat.Depth24Unorm_Stencil8,
                       SampleCount = 1,
                       ClearColor = new MTLClearColor(0.0, 0.0, 0.0, 1.0)
                   };
                   
                   _metalView.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
                   
                   // Configure for Retina displays
                   _metalView.Layer.ContentsScale = NSScreen.MainScreen.BackingScaleFactor;
                   
                   CCLog.Log($"Metal initialized with device: {_device.Name}");
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Failed to initialize Metal: {ex.Message}");
                   throw;
               }
           }
           
           private void SetupView()
           {
               try
               {
                   // Add Metal view to controller
                   View.AddSubview(_metalView);
                   
                   // Configure for game rendering
                   _metalView.FramebufferOnly = false;
                   _metalView.DrawableSize = new CGSize(
                       View.Bounds.Width * NSScreen.MainScreen.BackingScaleFactor,
                       View.Bounds.Height * NSScreen.MainScreen.BackingScaleFactor
                   );
                   
                   // Enable tracking areas for mouse events
                   SetupTrackingAreas();
                   
                   CCLog.Log($"Metal view configured: {_metalView.DrawableSize.Width}x{_metalView.DrawableSize.Height}");
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error setting up Metal view: {ex.Message}");
                   throw;
               }
           }
           
           private void SetupTrackingAreas()
           {
               var trackingArea = new NSTrackingArea(
                   _metalView.Bounds,
                   NSTrackingAreaOptions.ActiveInKeyWindow | 
                   NSTrackingAreaOptions.MouseEnteredAndExited |
                   NSTrackingAreaOptions.MouseMoved |
                   NSTrackingAreaOptions.EnabledDuringMouseDrag,
                   _metalView,
                   null
               );
               
               _metalView.AddTrackingArea(trackingArea);
           }
           
           private void CreateRenderer()
           {
               try
               {
                   // Create Metal renderer
                   _renderer = new MetalRenderer(_metalView);
                   
                   // Configure graphics factory to use Metal
                   CCGraphicsFactory.ForceBackend(GraphicsBackend.Metal);
                   
                   // Initialize cocos2d-mono with Metal backend
                   CCDrawManager.InitializeWithFactory();
                   
                   CCLog.Log("Metal renderer created and integrated");
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error creating Metal renderer: {ex.Message}");
                   throw;
               }
           }
           
           private void StartDisplayLink()
           {
               try
               {
                   // macOS uses CVDisplayLink instead of CADisplayLink
                   var displayLinkOutputCallback = new CVDisplayLinkOutputCallback(OnDisplayLinkUpdate);
                   CVDisplayLink.TryCreateWithActiveCGDisplays(out _displayLink);
                   
                   _displayLink.SetOutputCallback(displayLinkOutputCallback);
                   _displayLink.Start();
                   
                   CCLog.Log("Display link started");
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error starting display link: {ex.Message}");
               }
           }
           
           private CVReturn OnDisplayLinkUpdate(CVDisplayLink displayLink, ref CVTimeStamp inNow, 
                                             ref CVTimeStamp inOutputTime, CVOptionFlags flagsIn, 
                                             ref CVOptionFlags flagsOut)
           {
               if (_isGameRunning && _game != null)
               {
                   try
                   {
                       // Dispatch to main thread for UI updates
                       DispatchQueue.MainQueue.DispatchAsync(() =>
                       {
                           try
                           {
                               // Update game
                               _game.Update();
                               
                               // Request Metal view redraw
                               _metalView.NeedsDisplay = true;
                               
                               // Update FPS
                               UpdateFPS();
                           }
                           catch (Exception ex)
                           {
                               CCLog.Log($"Error in main thread update: {ex.Message}");
                           }
                       });
                   }
                   catch (Exception ex)
                   {
                       CCLog.Log($"Error in display link update: {ex.Message}");
                   }
               }
               
               return CVReturn.Success;
           }
           
           private void UpdateFPS()
           {
               _frameCount++;
               var currentTime = DateTime.Now;
               var deltaTime = (currentTime - _lastFrameTime).TotalSeconds;
               
               if (deltaTime >= 1.0) // Update FPS every second
               {
                   _fps = _frameCount / deltaTime;
                   _frameCount = 0;
                   _lastFrameTime = currentTime;
               }
           }
           
           private void SetupEventHandlers()
           {
               // Window notifications
               NSNotificationCenter.DefaultCenter.AddObserver(
                   NSWindow.DidResizeNotification,
                   OnWindowResized,
                   View.Window
               );
               
               NSNotificationCenter.DefaultCenter.AddObserver(
                   NSWindow.DidMiniaturizeNotification,
                   OnWindowMinimized,
                   View.Window
               );
               
               NSNotificationCenter.DefaultCenter.AddObserver(
                   NSWindow.DidDeminiaturizeNotification,
                   OnWindowDeminiaturized,
                   View.Window
               );
               
               // Screen change notifications
               NSNotificationCenter.DefaultCenter.AddObserver(
                   NSApplication.DidChangeScreenParametersNotification,
                   OnScreenParametersChanged
               );
           }
           
           // IMTKViewDelegate Implementation
           public void DrawableSizeWillChange(MTKView view, CGSize size)
           {
               try
               {
                   CCLog.Log($"Metal drawable size changed: {size.Width}x{size.Height}");
                   
                   // Update viewport in graphics device
                   if (_renderer?.GetGraphicsDevice() is MetalDevice metalDevice)
                   {
                       metalDevice.Viewport = new Microsoft.Xna.Framework.Graphics.Viewport(
                           0, 0, (int)size.Width, (int)size.Height);
                   }
                   
                   // Notify game of size change
                   _game?.OnResize((int)size.Width, (int)size.Height);
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error handling drawable size change: {ex.Message}");
               }
           }
           
           public void Draw(MTKView view)
           {
               if (!_isGameRunning || _game == null) return;
               
               try
               {
                   // Draw game content
                   _game.Draw();
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error in Metal draw: {ex.Message}");
               }
           }
           
           // Window event handlers
           private void OnWindowResized(NSNotification notification)
           {
               try
               {
                   var window = notification.Object as NSWindow;
                   if (window != null)
                   {
                       var frame = window.Frame;
                       CCLog.Log($"Window resized: {frame.Width}x{frame.Height}");
                       
                       // Update Metal view drawable size
                       _metalView.DrawableSize = new CGSize(
                           frame.Width * NSScreen.MainScreen.BackingScaleFactor,
                           frame.Height * NSScreen.MainScreen.BackingScaleFactor
                       );
                   }
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error handling window resize: {ex.Message}");
               }
           }
           
           private void OnWindowMinimized(NSNotification notification)
           {
               try
               {
                   CCLog.Log("Window minimized");
                   PauseGame();
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error handling window minimize: {ex.Message}");
               }
           }
           
           private void OnWindowDeminiaturized(NSNotification notification)
           {
               try
               {
                   CCLog.Log("Window deminimized");
                   ResumeGame();
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error handling window deminimize: {ex.Message}");
               }
           }
           
           private void OnScreenParametersChanged(NSNotification notification)
           {
               try
               {
                   CCLog.Log("Screen parameters changed");
                   
                   // Update for potential resolution or display changes
                   var scaleFactor = NSScreen.MainScreen.BackingScaleFactor;
                   _metalView.Layer.ContentsScale = scaleFactor;
                   
                   // Update drawable size
                   _metalView.DrawableSize = new CGSize(
                       _metalView.Bounds.Width * scaleFactor,
                       _metalView.Bounds.Height * scaleFactor
                   );
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error handling screen parameter change: {ex.Message}");
               }
           }
           
           // Game Integration
           public void SetGame(Game game)
           {
               _game = game;
               
               if (_game != null)
               {
                   // Initialize game with Metal graphics device
                   _game.GraphicsDevice = _renderer.GetGraphicsDevice() as Microsoft.Xna.Framework.Graphics.GraphicsDevice;
                   _game.Initialize();
               }
               
               CCLog.Log($"Game set: {game?.GetType().Name}");
           }
           
           public void StartGame()
           {
               if (_game != null)
               {
                   _isGameRunning = true;
                   _game.Start();
                   CCLog.Log("Game started");
               }
           }
           
           public void StopGame()
           {
               if (_game != null)
               {
                   _isGameRunning = false;
                   _game.Stop();
                   CCLog.Log("Game stopped");
               }
           }
           
           public void PauseGame()
           {
               if (_game != null)
               {
                   _isGameRunning = false;
                   _game.Pause();
                   CCLog.Log("Game paused");
               }
           }
           
           public void ResumeGame()
           {
               if (_game != null)
               {
                   _isGameRunning = true;
                   _game.Resume();
                   CCLog.Log("Game resumed");
               }
           }
           
           // Full screen management
           public void ToggleFullScreen()
           {
               try
               {
                   if (_window == null)
                   {
                       _window = View.Window;
                   }
                   
                   if (_window != null)
                   {
                       _window.ToggleFullScreen(this);
                       _isFullScreen = !_isFullScreen;
                       
                       CCLog.Log($"Full screen toggled: {_isFullScreen}");
                   }
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error toggling full screen: {ex.Message}");
               }
           }
           
           protected override void Dispose(bool disposing)
           {
               if (disposing)
               {
                   StopGame();
                   
                   _displayLink?.Stop();
                   _displayLink?.Dispose();
                   _displayLink = null;
                   
                   _renderer?.Dispose();
                   _renderer = null;
                   
                   _metalView?.RemoveFromSuperview();
                   _metalView?.Dispose();
                   _metalView = null;
                   
                   // Remove notification observers
                   NSNotificationCenter.DefaultCenter.RemoveObserver(this);
                   
                   CCLog.Log("macOS Metal view controller disposed");
               }
               
               base.Dispose(disposing);
           }
       }
   }
   #endif
   ```

2. **Create macOS window integration**
   ```csharp
   // File: cocos2d/platform/macOS/CCMetalWindow.cs
   #if MACOS && METAL
   using System;
   using Foundation;
   using AppKit;
   using CoreGraphics;
   
   namespace Cocos2D.Platform.macOS
   {
       /// <summary>
       /// macOS window that hosts cocos2d-mono Metal rendering
       /// </summary>
       public class CCMetalWindow : NSWindow
       {
           private CCMetalViewController _viewController;
           
           public CCMetalViewController ViewController => _viewController;
           
           public CCMetalWindow(CGRect contentRect, NSWindowStyle styleMask, NSBackingStore backingType, bool defer)
               : base(contentRect, styleMask, backingType, defer)
           {
               Initialize();
           }
           
           public CCMetalWindow() : base()
           {
               Initialize();
           }
           
           private void Initialize()
           {
               try
               {
                   // Configure window for optimal Metal rendering
                   Title = "Cocos2D Metal Application";
                   AcceptsMouseMovedEvents = true;
                   IsOpaque = true;
                   
                   // Create and set view controller
                   _viewController = new CCMetalViewController();
                   ContentViewController = _viewController;
                   
                   // Configure window behavior
                   SetupWindowBehavior();
                   
                   CCLog.Log("Metal window initialized");
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error initializing Metal window: {ex.Message}");
                   throw;
               }
           }
           
           private void SetupWindowBehavior()
           {
               // Center window on screen
               Center();
               
               // Set minimum size
               MinSize = new CGSize(320, 240);
               
               // Configure for games
               CollectionBehavior = NSWindowCollectionBehavior.FullScreenPrimary;
               
               // Make key and front
               MakeKeyAndOrderFront(null);
           }
           
           public override bool AcceptsFirstResponder()
           {
               return true;
           }
           
           public override bool CanBecomeKeyWindow()
           {
               return true;
           }
           
           public override bool CanBecomeMainWindow()
           {
               return true;
           }
           
           // Key event handling
           public override void KeyDown(NSEvent theEvent)
           {
               try
               {
                   // Convert NSEvent to cocos2d key event
                   var keyCode = theEvent.KeyCode;
                   var characters = theEvent.Characters;
                   
                   CCLog.Log($"Key down: {keyCode} ({characters})");
                   
                   // Forward to input handler
                   HandleKeyDown(keyCode, characters);
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error handling key down: {ex.Message}");
               }
           }
           
           public override void KeyUp(NSEvent theEvent)
           {
               try
               {
                   var keyCode = theEvent.KeyCode;
                   var characters = theEvent.Characters;
                   
                   CCLog.Log($"Key up: {keyCode} ({characters})");
                   
                   // Forward to input handler
                   HandleKeyUp(keyCode, characters);
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error handling key up: {ex.Message}");
               }
           }
           
           public override void FlagsChanged(NSEvent theEvent)
           {
               try
               {
                   var modifierFlags = theEvent.ModifierFlags;
                   CCLog.Log($"Modifier flags changed: {modifierFlags}");
                   
                   // Handle modifier key changes
                   HandleModifierFlagsChanged(modifierFlags);
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error handling flags changed: {ex.Message}");
               }
           }
           
           // Window lifecycle
           public override void OrderOut(NSObject sender)
           {
               try
               {
                   _viewController?.PauseGame();
                   base.OrderOut(sender);
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error ordering out window: {ex.Message}");
               }
           }
           
           public override void OrderFront(NSObject sender)
           {
               try
               {
                   base.OrderFront(sender);
                   _viewController?.ResumeGame();
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error ordering front window: {ex.Message}");
               }
           }
           
           // Game integration
           public void SetGame(Game game)
           {
               _viewController?.SetGame(game);
           }
           
           public void StartGame()
           {
               _viewController?.StartGame();
           }
           
           public void StopGame()
           {
               _viewController?.StopGame();
           }
           
           // Input handling methods (to be implemented based on input system requirements)
           private void HandleKeyDown(ushort keyCode, string characters)
           {
               // Forward to cocos2d input system
               // Implementation depends on input handling architecture
           }
           
           private void HandleKeyUp(ushort keyCode, string characters)
           {
               // Forward to cocos2d input system
           }
           
           private void HandleModifierFlagsChanged(NSEventModifierMask modifierFlags)
           {
               // Handle modifier key state changes
           }
           
           protected override void Dispose(bool disposing)
           {
               if (disposing)
               {
                   _viewController?.Dispose();
                   _viewController = null;
                   
                   CCLog.Log("Metal window disposed");
               }
               
               base.Dispose(disposing);
           }
       }
   }
   #endif
   ```

### Verification:
- macOS Metal view controller initializes correctly
- Window management functions properly
- Display link integration works correctly
- Event handling system responds appropriately

---

## Subtask 4.2.2: Implement macOS Input System Integration
**Time Estimate**: 2 hours  
**Dependencies**: Subtask 4.2.1  
**Assignee**: macOS developer

### Steps:

1. **Create macOS mouse input handler**
   ```csharp
   // File: cocos2d/platform/macOS/CCmacOSMouseHandler.cs
   #if MACOS && METAL
   using System;
   using System.Collections.Generic;
   using Foundation;
   using AppKit;
   using CoreGraphics;
   using Microsoft.Xna.Framework;
   
   namespace Cocos2D.Platform.macOS
   {
       /// <summary>
       /// Handles macOS mouse input for Metal-rendered cocos2d-mono games
       /// </summary>
       public class CCmacOSMouseHandler
       {
           private readonly NSView _view;
           private readonly Dictionary<int, CCMouseButton> _buttonStates;
           
           // Mouse event delegates
           public event Action<CCMouseEvent> MouseDown;
           public event Action<CCMouseEvent> MouseUp;
           public event Action<CCMouseEvent> MouseMoved;
           public event Action<CCMouseEvent> MouseDragged;
           public event Action<CCMouseEvent> MouseEntered;
           public event Action<CCMouseEvent> MouseExited;
           public event Action<CCScrollEvent> ScrollWheel;
           
           // Configuration
           public bool InvertYAxis { get; set; } = true; // Cocos2D coordinate system
           public float Sensitivity { get; set; } = 1.0f;
           
           public CCmacOSMouseHandler(NSView view)
           {
               _view = view ?? throw new ArgumentNullException(nameof(view));
               _buttonStates = new Dictionary<int, CCMouseButton>();
               
               CCLog.Log("macOS mouse handler initialized");
           }
           
           public void HandleMouseDown(NSEvent mouseEvent)
           {
               try
               {
                   var button = ConvertMouseButton(mouseEvent);
                   var location = ConvertLocation(mouseEvent.LocationInWindow);
                   var modifiers = ConvertModifiers(mouseEvent.ModifierFlags);
                   
                   _buttonStates[(int)button] = button;
                   
                   var ccEvent = new CCMouseEvent
                   {
                       Button = button,
                       Location = location,
                       Modifiers = modifiers,
                       ClickCount = (int)mouseEvent.ClickCount,
                       Timestamp = mouseEvent.Timestamp,
                       EventType = CCMouseEventType.Down
                   };
                   
                   MouseDown?.Invoke(ccEvent);
                   
                   CCLog.Log($"Mouse down: {button} at {location}");
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error handling mouse down: {ex.Message}");
               }
           }
           
           public void HandleMouseUp(NSEvent mouseEvent)
           {
               try
               {
                   var button = ConvertMouseButton(mouseEvent);
                   var location = ConvertLocation(mouseEvent.LocationInWindow);
                   var modifiers = ConvertModifiers(mouseEvent.ModifierFlags);
                   
                   _buttonStates.Remove((int)button);
                   
                   var ccEvent = new CCMouseEvent
                   {
                       Button = button,
                       Location = location,
                       Modifiers = modifiers,
                       ClickCount = (int)mouseEvent.ClickCount,
                       Timestamp = mouseEvent.Timestamp,
                       EventType = CCMouseEventType.Up
                   };
                   
                   MouseUp?.Invoke(ccEvent);
                   
                   CCLog.Log($"Mouse up: {button} at {location}");
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error handling mouse up: {ex.Message}");
               }
           }
           
           public void HandleMouseMoved(NSEvent mouseEvent)
           {
               try
               {
                   var location = ConvertLocation(mouseEvent.LocationInWindow);
                   var delta = new Vector2((float)mouseEvent.DeltaX, (float)mouseEvent.DeltaY) * Sensitivity;
                   var modifiers = ConvertModifiers(mouseEvent.ModifierFlags);
                   
                   var ccEvent = new CCMouseEvent
                   {
                       Button = CCMouseButton.None,
                       Location = location,
                       Delta = delta,
                       Modifiers = modifiers,
                       Timestamp = mouseEvent.Timestamp,
                       EventType = _buttonStates.Count > 0 ? CCMouseEventType.Dragged : CCMouseEventType.Moved
                   };
                   
                   if (ccEvent.EventType == CCMouseEventType.Dragged)
                   {
                       MouseDragged?.Invoke(ccEvent);
                   }
                   else
                   {
                       MouseMoved?.Invoke(ccEvent);
                   }
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error handling mouse moved: {ex.Message}");
               }
           }
           
           public void HandleMouseEntered(NSEvent mouseEvent)
           {
               try
               {
                   var location = ConvertLocation(mouseEvent.LocationInWindow);
                   
                   var ccEvent = new CCMouseEvent
                   {
                       Button = CCMouseButton.None,
                       Location = location,
                       Timestamp = mouseEvent.Timestamp,
                       EventType = CCMouseEventType.Entered
                   };
                   
                   MouseEntered?.Invoke(ccEvent);
                   
                   CCLog.Log($"Mouse entered at {location}");
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error handling mouse entered: {ex.Message}");
               }
           }
           
           public void HandleMouseExited(NSEvent mouseEvent)
           {
               try
               {
                   var location = ConvertLocation(mouseEvent.LocationInWindow);
                   
                   var ccEvent = new CCMouseEvent
                   {
                       Button = CCMouseButton.None,
                       Location = location,
                       Timestamp = mouseEvent.Timestamp,
                       EventType = CCMouseEventType.Exited
                   };
                   
                   MouseExited?.Invoke(ccEvent);
                   
                   CCLog.Log($"Mouse exited at {location}");
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error handling mouse exited: {ex.Message}");
               }
           }
           
           public void HandleScrollWheel(NSEvent scrollEvent)
           {
               try
               {
                   var location = ConvertLocation(scrollEvent.LocationInWindow);
                   var scrollDelta = new Vector2(
                       (float)scrollEvent.ScrollingDeltaX,
                       (float)scrollEvent.ScrollingDeltaY
                   ) * Sensitivity;
                   
                   // Handle precise vs line-based scrolling
                   if (scrollEvent.HasPreciseScrollingDeltas)
                   {
                       scrollDelta *= 0.1f; // Scale down precise scrolling
                   }
                   
                   var ccEvent = new CCScrollEvent
                   {
                       Location = location,
                       Delta = scrollDelta,
                       IsPrecise = scrollEvent.HasPreciseScrollingDeltas,
                       Timestamp = scrollEvent.Timestamp
                   };
                   
                   ScrollWheel?.Invoke(ccEvent);
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error handling scroll wheel: {ex.Message}");
               }
           }
           
           private CCMouseButton ConvertMouseButton(NSEvent mouseEvent)
           {
               switch (mouseEvent.Type)
               {
                   case NSEventType.LeftMouseDown:
                   case NSEventType.LeftMouseUp:
                   case NSEventType.LeftMouseDragged:
                       return CCMouseButton.Left;
                       
                   case NSEventType.RightMouseDown:
                   case NSEventType.RightMouseUp:
                   case NSEventType.RightMouseDragged:
                       return CCMouseButton.Right;
                       
                   case NSEventType.OtherMouseDown:
                   case NSEventType.OtherMouseUp:
                   case NSEventType.OtherMouseDragged:
                       return (CCMouseButton)(mouseEvent.ButtonNumber + 1);
                       
                   default:
                       return CCMouseButton.None;
               }
           }
           
           private Vector2 ConvertLocation(CGPoint location)
           {
               // Convert from NSView coordinates to cocos2d coordinates
               var viewHeight = _view.Bounds.Height;
               
               if (InvertYAxis)
               {
                   return new Vector2((float)location.X, (float)(viewHeight - location.Y));
               }
               else
               {
                   return new Vector2((float)location.X, (float)location.Y);
               }
           }
           
           private CCInputModifiers ConvertModifiers(NSEventModifierMask modifierFlags)
           {
               var modifiers = CCInputModifiers.None;
               
               if (modifierFlags.HasFlag(NSEventModifierMask.ShiftKeyMask))
                   modifiers |= CCInputModifiers.Shift;
               
               if (modifierFlags.HasFlag(NSEventModifierMask.ControlKeyMask))
                   modifiers |= CCInputModifiers.Control;
               
               if (modifierFlags.HasFlag(NSEventModifierMask.AlternateKeyMask))
                   modifiers |= CCInputModifiers.Alt;
               
               if (modifierFlags.HasFlag(NSEventModifierMask.CommandKeyMask))
                   modifiers |= CCInputModifiers.Command;
               
               return modifiers;
           }
           
           /// <summary>
           /// Gets the current state of all mouse buttons
           /// </summary>
           public CCMouseButton[] GetPressedButtons()
           {
               var buttons = new CCMouseButton[_buttonStates.Count];
               _buttonStates.Values.CopyTo(buttons, 0);
               return buttons;
           }
           
           /// <summary>
           /// Checks if a specific mouse button is currently pressed
           /// </summary>
           public bool IsButtonPressed(CCMouseButton button)
           {
               return _buttonStates.ContainsKey((int)button);
           }
       }
       
       // Event data structures
       public class CCMouseEvent
       {
           public CCMouseButton Button { get; set; }
           public Vector2 Location { get; set; }
           public Vector2 Delta { get; set; }
           public CCInputModifiers Modifiers { get; set; }
           public int ClickCount { get; set; }
           public double Timestamp { get; set; }
           public CCMouseEventType EventType { get; set; }
       }
       
       public class CCScrollEvent
       {
           public Vector2 Location { get; set; }
           public Vector2 Delta { get; set; }
           public bool IsPrecise { get; set; }
           public double Timestamp { get; set; }
       }
       
       public enum CCMouseButton
       {
           None = 0,
           Left = 1,
           Right = 2,
           Middle = 3,
           Button4 = 4,
           Button5 = 5
       }
       
       public enum CCMouseEventType
       {
           Down,
           Up,
           Moved,
           Dragged,
           Entered,
           Exited
       }
       
       [Flags]
       public enum CCInputModifiers
       {
           None = 0,
           Shift = 1 << 0,
           Control = 1 << 1,
           Alt = 1 << 2,
           Command = 1 << 3
       }
   }
   #endif
   ```

2. **Create macOS keyboard input handler**
   ```csharp
   // File: cocos2d/platform/macOS/CCmacOSKeyboardHandler.cs
   #if MACOS && METAL
   using System;
   using System.Collections.Generic;
   using Foundation;
   using AppKit;
   using Microsoft.Xna.Framework.Input;
   
   namespace Cocos2D.Platform.macOS
   {
       /// <summary>
       /// Handles macOS keyboard input for Metal-rendered cocos2d-mono games
       /// </summary>
       public class CCmacOSKeyboardHandler
       {
           private readonly HashSet<ushort> _pressedKeys;
           private readonly Dictionary<ushort, Keys> _keyMapping;
           
           // Keyboard event delegates
           public event Action<CCKeyEvent> KeyDown;
           public event Action<CCKeyEvent> KeyUp;
           public event Action<string> TextInput;
           
           // Configuration
           public bool EnableTextInput { get; set; } = true;
           
           public CCmacOSKeyboardHandler()
           {
               _pressedKeys = new HashSet<ushort>();
               _keyMapping = new Dictionary<ushort, Keys>();
               
               InitializeKeyMapping();
               
               CCLog.Log("macOS keyboard handler initialized");
           }
           
           private void InitializeKeyMapping()
           {
               // Map macOS virtual key codes to Microsoft.Xna.Framework.Input.Keys
               _keyMapping[0] = Keys.A;
               _keyMapping[11] = Keys.B;
               _keyMapping[8] = Keys.C;
               _keyMapping[2] = Keys.D;
               _keyMapping[14] = Keys.E;
               _keyMapping[3] = Keys.F;
               _keyMapping[5] = Keys.G;
               _keyMapping[4] = Keys.H;
               _keyMapping[34] = Keys.I;
               _keyMapping[38] = Keys.J;
               _keyMapping[40] = Keys.K;
               _keyMapping[37] = Keys.L;
               _keyMapping[46] = Keys.M;
               _keyMapping[45] = Keys.N;
               _keyMapping[31] = Keys.O;
               _keyMapping[35] = Keys.P;
               _keyMapping[12] = Keys.Q;
               _keyMapping[15] = Keys.R;
               _keyMapping[1] = Keys.S;
               _keyMapping[17] = Keys.T;
               _keyMapping[32] = Keys.U;
               _keyMapping[9] = Keys.V;
               _keyMapping[13] = Keys.W;
               _keyMapping[7] = Keys.X;
               _keyMapping[16] = Keys.Y;
               _keyMapping[6] = Keys.Z;
               
               // Numbers
               _keyMapping[29] = Keys.D0;
               _keyMapping[18] = Keys.D1;
               _keyMapping[19] = Keys.D2;
               _keyMapping[20] = Keys.D3;
               _keyMapping[21] = Keys.D4;
               _keyMapping[23] = Keys.D5;
               _keyMapping[22] = Keys.D6;
               _keyMapping[26] = Keys.D7;
               _keyMapping[28] = Keys.D8;
               _keyMapping[25] = Keys.D9;
               
               // Function keys
               _keyMapping[122] = Keys.F1;
               _keyMapping[120] = Keys.F2;
               _keyMapping[99] = Keys.F3;
               _keyMapping[118] = Keys.F4;
               _keyMapping[96] = Keys.F5;
               _keyMapping[97] = Keys.F6;
               _keyMapping[98] = Keys.F7;
               _keyMapping[100] = Keys.F8;
               _keyMapping[101] = Keys.F9;
               _keyMapping[109] = Keys.F10;
               _keyMapping[103] = Keys.F11;
               _keyMapping[111] = Keys.F12;
               
               // Special keys
               _keyMapping[36] = Keys.Enter;
               _keyMapping[53] = Keys.Escape;
               _keyMapping[49] = Keys.Space;
               _keyMapping[51] = Keys.Back;
               _keyMapping[48] = Keys.Tab;
               _keyMapping[56] = Keys.LeftShift;
               _keyMapping[60] = Keys.RightShift;
               _keyMapping[59] = Keys.LeftControl;
               _keyMapping[62] = Keys.RightControl;
               _keyMapping[58] = Keys.LeftAlt;
               _keyMapping[61] = Keys.RightAlt;
               _keyMapping[55] = Keys.LeftWindows; // Command key
               _keyMapping[54] = Keys.RightWindows; // Command key
               
               // Arrow keys
               _keyMapping[126] = Keys.Up;
               _keyMapping[125] = Keys.Down;
               _keyMapping[123] = Keys.Left;
               _keyMapping[124] = Keys.Right;
               
               // Additional keys can be added as needed
           }
           
           public void HandleKeyDown(NSEvent keyEvent)
           {
               try
               {
                   var keyCode = keyEvent.KeyCode;
                   var characters = keyEvent.Characters;
                   var modifiers = ConvertModifiers(keyEvent.ModifierFlags);
                   
                   _pressedKeys.Add(keyCode);
                   
                   var key = _keyMapping.ContainsKey(keyCode) ? _keyMapping[keyCode] : Keys.None;
                   
                   var ccEvent = new CCKeyEvent
                   {
                       Key = key,
                       KeyCode = keyCode,
                       Characters = characters,
                       Modifiers = ConvertModifiers(keyEvent.ModifierFlags),
                       IsRepeat = keyEvent.IsARepeat,
                       Timestamp = keyEvent.Timestamp
                   };
                   
                   KeyDown?.Invoke(ccEvent);
                   
                   // Handle text input separately
                   if (EnableTextInput && !string.IsNullOrEmpty(characters) && !IsModifierKey(key))
                   {
                       TextInput?.Invoke(characters);
                   }
                   
                   CCLog.Log($"Key down: {key} ({keyCode}) - {characters}");
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error handling key down: {ex.Message}");
               }
           }
           
           public void HandleKeyUp(NSEvent keyEvent)
           {
               try
               {
                   var keyCode = keyEvent.KeyCode;
                   var characters = keyEvent.Characters;
                   
                   _pressedKeys.Remove(keyCode);
                   
                   var key = _keyMapping.ContainsKey(keyCode) ? _keyMapping[keyCode] : Keys.None;
                   
                   var ccEvent = new CCKeyEvent
                   {
                       Key = key,
                       KeyCode = keyCode,
                       Characters = characters,
                       Modifiers = ConvertModifiers(keyEvent.ModifierFlags),
                       IsRepeat = false,
                       Timestamp = keyEvent.Timestamp
                   };
                   
                   KeyUp?.Invoke(ccEvent);
                   
                   CCLog.Log($"Key up: {key} ({keyCode})");
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error handling key up: {ex.Message}");
               }
           }
           
           public void HandleFlagsChanged(NSEvent flagsEvent)
           {
               try
               {
                   var modifierFlags = flagsEvent.ModifierFlags;
                   var keyCode = flagsEvent.KeyCode;
                   
                   // Determine if modifier key was pressed or released
                   bool isPressed = _pressedKeys.Contains(keyCode);
                   
                   if (!isPressed)
                   {
                       // Modifier key pressed
                       HandleKeyDown(flagsEvent);
                   }
                   else
                   {
                       // Modifier key released
                       HandleKeyUp(flagsEvent);
                   }
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error handling flags changed: {ex.Message}");
               }
           }
           
           private CCInputModifiers ConvertModifiers(NSEventModifierMask modifierFlags)
           {
               var modifiers = CCInputModifiers.None;
               
               if (modifierFlags.HasFlag(NSEventModifierMask.ShiftKeyMask))
                   modifiers |= CCInputModifiers.Shift;
               
               if (modifierFlags.HasFlag(NSEventModifierMask.ControlKeyMask))
                   modifiers |= CCInputModifiers.Control;
               
               if (modifierFlags.HasFlag(NSEventModifierMask.AlternateKeyMask))
                   modifiers |= CCInputModifiers.Alt;
               
               if (modifierFlags.HasFlag(NSEventModifierMask.CommandKeyMask))
                   modifiers |= CCInputModifiers.Command;
               
               return modifiers;
           }
           
           private bool IsModifierKey(Keys key)
           {
               switch (key)
               {
                   case Keys.LeftShift:
                   case Keys.RightShift:
                   case Keys.LeftControl:
                   case Keys.RightControl:
                   case Keys.LeftAlt:
                   case Keys.RightAlt:
                   case Keys.LeftWindows:
                   case Keys.RightWindows:
                       return true;
                   default:
                       return false;
               }
           }
           
           /// <summary>
           /// Gets all currently pressed keys
           /// </summary>
           public Keys[] GetPressedKeys()
           {
               var keys = new List<Keys>();
               
               foreach (var keyCode in _pressedKeys)
               {
                   if (_keyMapping.ContainsKey(keyCode))
                   {
                       keys.Add(_keyMapping[keyCode]);
                   }
               }
               
               return keys.ToArray();
           }
           
           /// <summary>
           /// Checks if a specific key is currently pressed
           /// </summary>
           public bool IsKeyPressed(Keys key)
           {
               foreach (var kvp in _keyMapping)
               {
                   if (kvp.Value == key && _pressedKeys.Contains(kvp.Key))
                   {
                       return true;
                   }
               }
               
               return false;
           }
           
           /// <summary>
           /// Clears all pressed key states
           /// </summary>
           public void ClearPressedKeys()
           {
               _pressedKeys.Clear();
               CCLog.Log("Pressed keys cleared");
           }
       }
       
       public class CCKeyEvent
       {
           public Keys Key { get; set; }
           public ushort KeyCode { get; set; }
           public string Characters { get; set; }
           public CCInputModifiers Modifiers { get; set; }
           public bool IsRepeat { get; set; }
           public double Timestamp { get; set; }
       }
   }
   #endif
   ```

### Verification:
- Mouse input is correctly captured and converted
- Keyboard input mapping works for standard keys
- Input coordinates are properly transformed to cocos2d space
- Modifier keys are handled correctly

---

## Summary and Timeline

### Total Estimated Time: ~4 hours (half to full day for one developer)

### Optimal Task Assignment (Single macOS developer required):

**macOS/Metal Developer:**
- Subtask 4.2.1: Metal View Integration (2h)
- Subtask 4.2.2: Input System Integration (2h)
- **Total: 4h**

### Dependencies:
```
4.2.1 (Metal View) ──> 4.2.2 (Input System)
```

This macOS platform integration provides:
- Complete macOS Metal view controller with proper window management
- Full screen support with proper transitions
- Comprehensive mouse and keyboard input handling
- macOS-specific optimizations for Retina displays
- Proper macOS app lifecycle integration
- Platform-specific event handling for window state changes

The macOS integration ensures that Metal rendering works seamlessly within macOS applications while maintaining all platform-specific features and optimizations, including proper input handling and window management.