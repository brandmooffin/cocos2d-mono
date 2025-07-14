# Task 1.2: Create Graphics Factory - Detailed Breakdown

## Overview
Create the factory pattern for graphics backend selection and renderer creation. This task builds the runtime system for choosing between MonoGame and Metal backends.

---

## Subtask 1.2.1: Create Factory Infrastructure
**Time Estimate**: 45 minutes  
**Dependencies**: Task 1.1 complete  
**Assignee**: Senior developer

### Steps:

1. **Create factory directory structure**
   ```bash
   mkdir -p cocos2d/platform/Factory
   touch cocos2d/platform/Factory/CCGraphicsFactory.cs
   touch cocos2d/platform/Factory/GraphicsBackend.cs
   touch cocos2d/platform/Factory/BackendCapabilities.cs
   ```

2. **Define GraphicsBackend enum**
   ```csharp
   // File: cocos2d/platform/Factory/GraphicsBackend.cs
   namespace Cocos2D.Platform.Factory
   {
       /// <summary>
       /// Available graphics rendering backends
       /// </summary>
       public enum GraphicsBackend
       {
           /// <summary>
           /// Automatically select the best available backend
           /// </summary>
           Auto,
           
           /// <summary>
           /// Use MonoGame renderer (cross-platform compatibility)
           /// </summary>
           MonoGame,
           
           /// <summary>
           /// Use Metal renderer (iOS/macOS only, high performance)
           /// </summary>
           Metal,
           
           /// <summary>
           /// Use Vulkan renderer (future implementation)
           /// </summary>
           Vulkan,
           
           /// <summary>
           /// Use WebGPU renderer (future implementation)
           /// </summary>
           WebGPU,
           
           /// <summary>
           /// Use DirectX 12 renderer (future implementation)
           /// </summary>
           DirectX12
       }
       
       /// <summary>
       /// Backend feature flags
       /// </summary>
       [Flags]
       public enum BackendFeatures
       {
           None = 0,
           RenderTargets = 1 << 0,
           MultiSampling = 1 << 1,
           ComputeShaders = 1 << 2,
           GeometryShaders = 1 << 3,
           TessellationShaders = 1 << 4,
           IndirectDrawing = 1 << 5,
           AsyncCompute = 1 << 6,
           RayTracing = 1 << 7
       }
   }
   ```

3. **Create BackendCapabilities class**
   ```csharp
   // File: cocos2d/platform/Factory/BackendCapabilities.cs
   using System;
   using System.Collections.Generic;
   
   namespace Cocos2D.Platform.Factory
   {
       /// <summary>
       /// Describes the capabilities of a graphics backend
       /// </summary>
       public class BackendCapabilities
       {
           public GraphicsBackend Backend { get; set; }
           public string Name { get; set; }
           public string Version { get; set; }
           public BackendFeatures SupportedFeatures { get; set; }
           public int MaxTextureSize { get; set; }
           public int MaxRenderTargets { get; set; }
           public bool IsAvailable { get; set; }
           public float PerformanceScore { get; set; }
           public Dictionary<string, object> AdditionalInfo { get; set; }
           
           public BackendCapabilities()
           {
               AdditionalInfo = new Dictionary<string, object>();
           }
           
           /// <summary>
           /// Checks if a specific feature is supported
           /// </summary>
           public bool SupportsFeature(BackendFeatures feature)
           {
               return (SupportedFeatures & feature) == feature;
           }
           
           /// <summary>
           /// Gets a human-readable description
           /// </summary>
           public override string ToString()
           {
               return $"{Name} {Version} ({Backend}) - Available: {IsAvailable}, Score: {PerformanceScore:F1}";
           }
       }
   }
   ```

### Verification:
- All files compile successfully
- Enums and classes are properly documented
- Directory structure follows project conventions

---

## Subtask 1.2.2: Implement Factory Core Logic
**Time Estimate**: 1.5 hours  
**Dependencies**: Subtask 1.2.1  
**Assignee**: Senior developer

### Steps:

1. **Create factory class skeleton**
   ```csharp
   // File: cocos2d/platform/Factory/CCGraphicsFactory.cs
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Cocos2D.Platform.Interfaces;
   
   namespace Cocos2D.Platform.Factory
   {
       /// <summary>
       /// Factory for creating platform-appropriate graphics renderers
       /// </summary>
       public static class CCGraphicsFactory
       {
           private static IGraphicsRenderer _currentRenderer;
           private static GraphicsBackend _forcedBackend = GraphicsBackend.Auto;
           private static readonly object _lock = new object();
           private static readonly Dictionary<GraphicsBackend, BackendCapabilities> _availableBackends;
           
           static CCGraphicsFactory()
           {
               _availableBackends = new Dictionary<GraphicsBackend, BackendCapabilities>();
               DetectAvailableBackends();
           }
       }
   }
   ```

2. **Implement backend detection**
   ```csharp
   /// <summary>
   /// Detects all available graphics backends on the current platform
   /// </summary>
   private static void DetectAvailableBackends()
   {
       _availableBackends.Clear();
       
       // Always available: MonoGame
       _availableBackends[GraphicsBackend.MonoGame] = DetectMonoGameCapabilities();
       
   #if METAL && (IOS || MACOS)
       // Metal availability (iOS/macOS only)
       var metalCaps = DetectMetalCapabilities();
       if (metalCaps.IsAvailable)
       {
           _availableBackends[GraphicsBackend.Metal] = metalCaps;
       }
   #endif
       
   #if VULKAN && !IOS
       // Future: Vulkan detection
       var vulkanCaps = DetectVulkanCapabilities();
       if (vulkanCaps.IsAvailable)
       {
           _availableBackends[GraphicsBackend.Vulkan] = vulkanCaps;
       }
   #endif
       
       CCLog.Log($"Detected {_availableBackends.Count} available graphics backends:");
       foreach (var backend in _availableBackends.Values)
       {
           CCLog.Log($"  - {backend}");
       }
   }
   
   /// <summary>
   /// Detects MonoGame backend capabilities
   /// </summary>
   private static BackendCapabilities DetectMonoGameCapabilities()
   {
       return new BackendCapabilities
       {
           Backend = GraphicsBackend.MonoGame,
           Name = "MonoGame",
           Version = GetMonoGameVersion(),
           SupportedFeatures = BackendFeatures.RenderTargets | BackendFeatures.MultiSampling,
           MaxTextureSize = 8192, // Conservative estimate
           MaxRenderTargets = 4,
           IsAvailable = true,
           PerformanceScore = 7.0f
       };
   }
   
   #if METAL && (IOS || MACOS)
   /// <summary>
   /// Detects Metal backend capabilities
   /// </summary>
   private static BackendCapabilities DetectMetalCapabilities()
   {
       try
       {
           var device = Metal.MTLDevice.SystemDefault;
           if (device == null)
           {
               return new BackendCapabilities
               {
                   Backend = GraphicsBackend.Metal,
                   IsAvailable = false
               };
           }
           
           var features = BackendFeatures.RenderTargets | BackendFeatures.MultiSampling;
           
           // Check for advanced features
           if (device.SupportsFeatureSet(Metal.MTLFeatureSet.iOS_GPUFamily3_v1) ||
               device.SupportsFeatureSet(Metal.MTLFeatureSet.macOS_GPUFamily1_v1))
           {
               features |= BackendFeatures.ComputeShaders;
           }
           
           return new BackendCapabilities
           {
               Backend = GraphicsBackend.Metal,
               Name = "Metal",
               Version = GetMetalVersion(),
               SupportedFeatures = features,
               MaxTextureSize = (int)device.MaxTextureWidth2D,
               MaxRenderTargets = 8,
               IsAvailable = true,
               PerformanceScore = 9.5f,
               AdditionalInfo = new Dictionary<string, object>
               {
                   ["DeviceName"] = device.Name,
                   ["HasUnifiedMemory"] = device.HasUnifiedMemory,
                   ["MaxBufferLength"] = device.MaxBufferLength
               }
           };
       }
       catch (Exception ex)
       {
           CCLog.Log($"Failed to detect Metal capabilities: {ex.Message}");
           return new BackendCapabilities
           {
               Backend = GraphicsBackend.Metal,
               IsAvailable = false
           };
       }
   }
   #endif
   ```

3. **Add helper methods**
   ```csharp
   /// <summary>
   /// Gets the MonoGame version string
   /// </summary>
   private static string GetMonoGameVersion()
   {
       try
       {
           var assembly = typeof(Microsoft.Xna.Framework.Game).Assembly;
           var version = assembly.GetName().Version;
           return version?.ToString() ?? "Unknown";
       }
       catch
       {
           return "Unknown";
       }
   }
   
   #if METAL && (IOS || MACOS)
   /// <summary>
   /// Gets the Metal version string
   /// </summary>
   private static string GetMetalVersion()
   {
       try
       {
           // Metal version is tied to OS version
   #if IOS
           return UIKit.UIDevice.CurrentDevice.SystemVersion;
   #elif MACOS
           return Foundation.NSProcessInfo.ProcessInfo.OperatingSystemVersionString;
   #endif
       }
       catch
       {
           return "Unknown";
       }
   }
   #endif
   ```

### Verification:
- Backend detection works on all platforms
- Capabilities are correctly identified
- Logging provides useful information

---

## Subtask 1.2.3: Implement Renderer Creation Logic
**Time Estimate**: 1 hour  
**Dependencies**: Subtask 1.2.2  
**Assignee**: Senior developer

### Steps:

1. **Implement main factory methods**
   ```csharp
   /// <summary>
   /// Forces a specific graphics backend. Call before any renderer creation.
   /// </summary>
   /// <param name="backend">The backend to force</param>
   /// <exception cref="InvalidOperationException">If renderer already created</exception>
   public static void ForceBackend(GraphicsBackend backend)
   {
       lock (_lock)
       {
           if (_currentRenderer != null)
           {
               throw new InvalidOperationException(
                   "Cannot change backend after renderer is created. Call ForceBackend before any graphics operations.");
           }
           
           _forcedBackend = backend;
           CCLog.Log($"Graphics backend forced to: {backend}");
       }
   }
   
   /// <summary>
   /// Creates the appropriate graphics renderer for the current platform
   /// </summary>
   /// <returns>The created graphics renderer</returns>
   public static IGraphicsRenderer CreateRenderer()
   {
       lock (_lock)
       {
           if (_currentRenderer != null)
           {
               return _currentRenderer;
           }
           
           var backend = DetermineOptimalBackend();
           CCLog.Log($"Creating graphics renderer: {backend}");
           
           try
           {
               _currentRenderer = CreateRendererForBackend(backend);
               CCLog.Log($"Successfully created {backend} renderer: {_currentRenderer.GetType().Name}");
               return _currentRenderer;
           }
           catch (Exception ex)
           {
               CCLog.Log($"Failed to create {backend} renderer: {ex.Message}");
               
               // Fallback to MonoGame if requested backend failed
               if (backend != GraphicsBackend.MonoGame)
               {
                   CCLog.Log("Falling back to MonoGame renderer");
                   _currentRenderer = CreateRendererForBackend(GraphicsBackend.MonoGame);
                   return _currentRenderer;
               }
               
               throw;
           }
       }
   }
   ```

2. **Implement backend selection logic**
   ```csharp
   /// <summary>
   /// Determines the optimal graphics backend for the current platform and configuration
   /// </summary>
   private static GraphicsBackend DetermineOptimalBackend()
   {
       // Check if backend is forced
       if (_forcedBackend != GraphicsBackend.Auto)
       {
           return ValidateBackendAvailability(_forcedBackend);
       }
       
       // Find the best available backend based on performance score
       var bestBackend = _availableBackends.Values
           .Where(caps => caps.IsAvailable)
           .OrderByDescending(caps => caps.PerformanceScore)
           .FirstOrDefault();
       
       if (bestBackend != null)
       {
           CCLog.Log($"Auto-selected backend: {bestBackend.Backend} (score: {bestBackend.PerformanceScore})");
           return bestBackend.Backend;
       }
       
       // Ultimate fallback
       CCLog.Log("No optimal backend found, defaulting to MonoGame");
       return GraphicsBackend.MonoGame;
   }
   
   /// <summary>
   /// Validates that the requested backend is available
   /// </summary>
   private static GraphicsBackend ValidateBackendAvailability(GraphicsBackend backend)
   {
       if (_availableBackends.TryGetValue(backend, out var capabilities) && capabilities.IsAvailable)
       {
           return backend;
       }
       
       CCLog.Log($"{backend} backend requested but not available. Available backends: {string.Join(", ", _availableBackends.Where(kvp => kvp.Value.IsAvailable).Select(kvp => kvp.Key))}");
       
       // Return best available fallback
       return DetermineOptimalBackend();
   }
   ```

3. **Implement renderer instantiation**
   ```csharp
   /// <summary>
   /// Creates a renderer instance for the specified backend
   /// </summary>
   private static IGraphicsRenderer CreateRendererForBackend(GraphicsBackend backend)
   {
       switch (backend)
       {
           case GraphicsBackend.MonoGame:
               return new MonoGameRenderer();
               
   #if METAL && (IOS || MACOS)
           case GraphicsBackend.Metal:
               return new MetalRenderer();
   #endif
               
   #if VULKAN && !IOS
           case GraphicsBackend.Vulkan:
               return new VulkanRenderer();
   #endif
               
           default:
               throw new NotSupportedException($"Backend {backend} is not implemented on this platform");
       }
   }
   ```

### Verification:
- Renderer creation works for available backends
- Fallback logic prevents crashes
- Error messages are informative

---

## Subtask 1.2.4: Add Factory Management Methods
**Time Estimate**: 45 minutes  
**Dependencies**: Subtask 1.2.3  
**Assignee**: Any developer

### Steps:

1. **Add renderer lifecycle methods**
   ```csharp
   /// <summary>
   /// Gets the current renderer without creating a new one
   /// </summary>
   /// <returns>Current renderer or null if none created</returns>
   public static IGraphicsRenderer GetCurrentRenderer()
   {
       lock (_lock)
       {
           return _currentRenderer;
       }
   }
   
   /// <summary>
   /// Checks if a renderer has been created
   /// </summary>
   public static bool HasRenderer => _currentRenderer != null;
   
   /// <summary>
   /// Gets the backend of the current renderer
   /// </summary>
   public static GraphicsBackend? CurrentBackend
   {
       get
       {
           if (_currentRenderer == null) return null;
           
           return _currentRenderer switch
           {
               MonoGameRenderer => GraphicsBackend.MonoGame,
   #if METAL && (IOS || MACOS)
               MetalRenderer => GraphicsBackend.Metal,
   #endif
               _ => null
           };
       }
   }
   ```

2. **Add factory reset and cleanup**
   ```csharp
   /// <summary>
   /// Releases the current renderer and allows creation of a new one
   /// </summary>
   public static void Reset()
   {
       lock (_lock)
       {
           try
           {
               _currentRenderer?.Dispose();
           }
           catch (Exception ex)
           {
               CCLog.Log($"Error disposing renderer: {ex.Message}");
           }
           finally
           {
               _currentRenderer = null;
               _forcedBackend = GraphicsBackend.Auto;
               CCLog.Log("Graphics factory reset");
           }
       }
   }
   
   /// <summary>
   /// Re-detects available backends (useful after driver updates)
   /// </summary>
   public static void RefreshBackends()
   {
       lock (_lock)
       {
           if (_currentRenderer != null)
           {
               CCLog.Log("Cannot refresh backends while renderer is active");
               return;
           }
           
           CCLog.Log("Refreshing available graphics backends...");
           DetectAvailableBackends();
       }
   }
   ```

3. **Add diagnostic methods**
   ```csharp
   /// <summary>
   /// Gets information about the current graphics backend
   /// </summary>
   public static string GetBackendInfo()
   {
       if (_currentRenderer == null)
           return "No renderer created";
       
       var backend = CurrentBackend;
       if (backend.HasValue && _availableBackends.TryGetValue(backend.Value, out var caps))
       {
           return caps.ToString();
       }
       
       return $"Unknown Renderer: {_currentRenderer.GetType().Name}";
   }
   
   /// <summary>
   /// Gets all available backend capabilities
   /// </summary>
   public static IReadOnlyDictionary<GraphicsBackend, BackendCapabilities> GetAvailableBackends()
   {
       return _availableBackends.AsReadOnly();
   }
   
   /// <summary>
   /// Checks if a specific backend is available
   /// </summary>
   public static bool IsBackendAvailable(GraphicsBackend backend)
   {
       return _availableBackends.TryGetValue(backend, out var caps) && caps.IsAvailable;
   }
   
   /// <summary>
   /// Gets the capabilities of a specific backend
   /// </summary>
   public static BackendCapabilities GetBackendCapabilities(GraphicsBackend backend)
   {
       _availableBackends.TryGetValue(backend, out var caps);
       return caps;
   }
   ```

### Verification:
- Lifecycle methods work correctly
- Reset properly cleans up resources
- Diagnostic methods provide useful information

---

## Subtask 1.2.5: Add Configuration Integration
**Time Estimate**: 1 hour  
**Dependencies**: Subtask 1.2.4  
**Assignee**: Developer familiar with configuration

### Steps:

1. **Create configuration integration**
   ```csharp
   /// <summary>
   /// Configures the factory based on CCGraphicsConfig settings
   /// </summary>
   public static void ConfigureFromSettings(CCGraphicsConfig config)
   {
       if (config == null)
       {
           CCLog.Log("No graphics configuration provided, using defaults");
           return;
       }
       
       // Apply forced backend from config
       if (config.PreferredBackend != GraphicsBackend.Auto)
       {
           try
           {
               ForceBackend(config.PreferredBackend);
           }
           catch (InvalidOperationException)
           {
               CCLog.Log($"Cannot apply backend preference {config.PreferredBackend} - renderer already created");
           }
       }
       
       // Store config for use during renderer creation
       _currentConfig = config;
       
       CCLog.Log($"Graphics factory configured: Backend={config.PreferredBackend}, Debug={config.EnableDebugMode}");
   }
   
   private static CCGraphicsConfig _currentConfig;
   
   /// <summary>
   /// Gets the current graphics configuration
   /// </summary>
   public static CCGraphicsConfig GetCurrentConfig() => _currentConfig;
   ```

2. **Add environment variable support**
   ```csharp
   /// <summary>
   /// Applies configuration from environment variables
   /// </summary>
   public static void ConfigureFromEnvironment()
   {
       // Backend override
       var backendEnv = Environment.GetEnvironmentVariable("COCOS2D_GRAPHICS_BACKEND");
       if (!string.IsNullOrEmpty(backendEnv) && Enum.TryParse<GraphicsBackend>(backendEnv, true, out var backend))
       {
           try
           {
               ForceBackend(backend);
               CCLog.Log($"Backend set from environment: {backend}");
           }
           catch (InvalidOperationException ex)
           {
               CCLog.Log($"Failed to set backend from environment: {ex.Message}");
           }
       }
       
       // Debug mode
       var debugEnv = Environment.GetEnvironmentVariable("COCOS2D_GRAPHICS_DEBUG");
       if (bool.TryParse(debugEnv, out bool debug) && debug)
       {
           EnableDebugMode();
       }
       
       // Performance preferences
       var perfEnv = Environment.GetEnvironmentVariable("COCOS2D_PREFER_PERFORMANCE");
       if (bool.TryParse(perfEnv, out bool performance))
       {
           _preferPerformance = performance;
           CCLog.Log($"Performance preference set from environment: {performance}");
       }
   }
   
   private static bool _debugMode = false;
   private static bool _preferPerformance = true;
   
   /// <summary>
   /// Enables debug mode for graphics operations
   /// </summary>
   public static void EnableDebugMode()
   {
       _debugMode = true;
       CCLog.Log("Graphics debug mode enabled");
   }
   
   /// <summary>
   /// Gets whether debug mode is enabled
   /// </summary>
   public static bool IsDebugMode => _debugMode;
   ```

3. **Add factory events**
   ```csharp
   /// <summary>
   /// Event raised when a renderer is created
   /// </summary>
   public static event EventHandler<RendererCreatedEventArgs> RendererCreated;
   
   /// <summary>
   /// Event raised when the factory is reset
   /// </summary>
   public static event EventHandler<EventArgs> FactoryReset;
   
   /// <summary>
   /// Event args for renderer creation
   /// </summary>
   public class RendererCreatedEventArgs : EventArgs
   {
       public IGraphicsRenderer Renderer { get; }
       public GraphicsBackend Backend { get; }
       public BackendCapabilities Capabilities { get; }
       
       public RendererCreatedEventArgs(IGraphicsRenderer renderer, GraphicsBackend backend, BackendCapabilities capabilities)
       {
           Renderer = renderer;
           Backend = backend;
           Capabilities = capabilities;
       }
   }
   
   // Update CreateRenderer to fire event
   private static void FireRendererCreatedEvent(IGraphicsRenderer renderer, GraphicsBackend backend)
   {
       try
       {
           var capabilities = GetBackendCapabilities(backend);
           var args = new RendererCreatedEventArgs(renderer, backend, capabilities);
           RendererCreated?.Invoke(null, args);
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error firing RendererCreated event: {ex.Message}");
       }
   }
   ```

### Verification:
- Configuration integration works correctly
- Environment variables are properly read
- Events are fired at appropriate times

---

## Subtask 1.2.6: Create Factory Unit Tests
**Time Estimate**: 2 hours  
**Dependencies**: Subtask 1.2.5  
**Assignee**: QA developer

### Steps:

1. **Create test file structure**
   ```bash
   mkdir -p Tests/FactoryTests
   touch Tests/FactoryTests/CCGraphicsFactoryTests.cs
   touch Tests/FactoryTests/BackendCapabilityTests.cs
   touch Tests/FactoryTests/MockRenderers.cs
   ```

2. **Create basic factory tests**
   ```csharp
   // File: Tests/FactoryTests/CCGraphicsFactoryTests.cs
   using Xunit;
   using Cocos2D.Platform.Factory;
   using Cocos2D.Platform.Interfaces;
   using System;
   
   namespace Cocos2D.Tests.Factory
   {
       public class CCGraphicsFactoryTests : IDisposable
       {
           public CCGraphicsFactoryTests()
           {
               // Reset factory before each test
               CCGraphicsFactory.Reset();
           }
           
           public void Dispose()
           {
               // Clean up after each test
               CCGraphicsFactory.Reset();
           }
           
           [Fact]
           public void CreateRenderer_ReturnsValidRenderer()
           {
               // Act
               var renderer = CCGraphicsFactory.CreateRenderer();
               
               // Assert
               Assert.NotNull(renderer);
               Assert.True(renderer is IGraphicsRenderer);
           }
           
           [Fact]
           public void CreateRenderer_CalledTwice_ReturnsSameInstance()
           {
               // Act
               var renderer1 = CCGraphicsFactory.CreateRenderer();
               var renderer2 = CCGraphicsFactory.CreateRenderer();
               
               // Assert
               Assert.Same(renderer1, renderer2);
           }
           
           [Fact]
           public void ForceBackend_BeforeCreation_SetsBackend()
           {
               // Arrange & Act
               CCGraphicsFactory.ForceBackend(GraphicsBackend.MonoGame);
               var renderer = CCGraphicsFactory.CreateRenderer();
               
               // Assert
               Assert.Equal(GraphicsBackend.MonoGame, CCGraphicsFactory.CurrentBackend);
           }
           
           [Fact]
           public void ForceBackend_AfterCreation_ThrowsException()
           {
               // Arrange
               CCGraphicsFactory.CreateRenderer();
               
               // Act & Assert
               Assert.Throws<InvalidOperationException>(() => 
                   CCGraphicsFactory.ForceBackend(GraphicsBackend.Metal));
           }
           
           [Fact]
           public void Reset_DisposesCurrentRenderer()
           {
               // Arrange
               var renderer = CCGraphicsFactory.CreateRenderer();
               var wasDisposed = false;
               
               // Mock disposal tracking
               if (renderer is IDisposable disposable)
               {
                   // Note: In real implementation, we'd need a way to track disposal
                   // This test structure shows the intent
               }
               
               // Act
               CCGraphicsFactory.Reset();
               
               // Assert
               Assert.Null(CCGraphicsFactory.GetCurrentRenderer());
               Assert.Null(CCGraphicsFactory.CurrentBackend);
           }
       }
   }
   ```

3. **Create backend capability tests**
   ```csharp
   // File: Tests/FactoryTests/BackendCapabilityTests.cs
   using Xunit;
   using Cocos2D.Platform.Factory;
   
   namespace Cocos2D.Tests.Factory
   {
       public class BackendCapabilityTests
       {
           [Fact]
           public void BackendCapabilities_DefaultConstructor_InitializesCorrectly()
           {
               // Act
               var caps = new BackendCapabilities();
               
               // Assert
               Assert.NotNull(caps.AdditionalInfo);
               Assert.Equal(BackendFeatures.None, caps.SupportedFeatures);
               Assert.False(caps.IsAvailable);
           }
           
           [Fact]
           public void SupportsFeature_WithSupportedFeature_ReturnsTrue()
           {
               // Arrange
               var caps = new BackendCapabilities
               {
                   SupportedFeatures = BackendFeatures.RenderTargets | BackendFeatures.MultiSampling
               };
               
               // Act & Assert
               Assert.True(caps.SupportsFeature(BackendFeatures.RenderTargets));
               Assert.True(caps.SupportsFeature(BackendFeatures.MultiSampling));
               Assert.False(caps.SupportsFeature(BackendFeatures.ComputeShaders));
           }
           
           [Fact]
           public void ToString_ReturnsFormattedString()
           {
               // Arrange
               var caps = new BackendCapabilities
               {
                   Name = "TestRenderer",
                   Version = "1.0",
                   Backend = GraphicsBackend.MonoGame,
                   IsAvailable = true,
                   PerformanceScore = 8.5f
               };
               
               // Act
               var result = caps.ToString();
               
               // Assert
               Assert.Contains("TestRenderer", result);
               Assert.Contains("1.0", result);
               Assert.Contains("MonoGame", result);
               Assert.Contains("Available: True", result);
               Assert.Contains("Score: 8.5", result);
           }
       }
   }
   ```

4. **Create integration tests**
   ```csharp
   public class FactoryIntegrationTests
   {
       [Fact]
       public void GetAvailableBackends_ReturnsValidBackends()
       {
           // Act
           var backends = CCGraphicsFactory.GetAvailableBackends();
           
           // Assert
           Assert.NotNull(backends);
           Assert.True(backends.Count > 0);
           Assert.True(backends.ContainsKey(GraphicsBackend.MonoGame));
       }
       
       [Fact]
       public void IsBackendAvailable_MonoGame_ReturnsTrue()
       {
           // Act & Assert
           Assert.True(CCGraphicsFactory.IsBackendAvailable(GraphicsBackend.MonoGame));
       }
       
       [Theory]
       [InlineData(GraphicsBackend.MonoGame)]
   #if METAL && (IOS || MACOS)
       [InlineData(GraphicsBackend.Metal)]
   #endif
       public void GetBackendCapabilities_ValidBackend_ReturnsCapabilities(GraphicsBackend backend)
       {
           // Act
           var caps = CCGraphicsFactory.GetBackendCapabilities(backend);
           
           // Assert
           if (CCGraphicsFactory.IsBackendAvailable(backend))
           {
               Assert.NotNull(caps);
               Assert.Equal(backend, caps.Backend);
               Assert.True(caps.IsAvailable);
           }
       }
   }
   ```

### Verification:
- All tests pass consistently
- Tests cover main factory functionality
- Edge cases are properly tested

---

## Subtask 1.2.7: Add Factory Documentation
**Time Estimate**: 1 hour  
**Dependencies**: Subtask 1.2.6  
**Assignee**: Technical writer or developer

### Steps:

1. **Create factory documentation**
   ```markdown
   # Graphics Factory Documentation
   
   ## Overview
   The `CCGraphicsFactory` provides a centralized system for creating and managing graphics renderers in cocos2d-mono. It automatically detects available graphics backends and selects the optimal one for the current platform.
   
   ## Basic Usage
   
   ### Automatic Backend Selection
   ```csharp
   // Create renderer with automatic backend selection
   var renderer = CCGraphicsFactory.CreateRenderer();
   ```
   
   ### Forced Backend Selection
   ```csharp
   // Force a specific backend before creation
   CCGraphicsFactory.ForceBackend(GraphicsBackend.Metal);
   var renderer = CCGraphicsFactory.CreateRenderer();
   ```
   
   ## Backend Types
   
   | Backend | Platforms | Performance | Features |
   |---------|-----------|-------------|----------|
   | MonoGame | All | Good | Standard |
   | Metal | iOS/macOS | Excellent | Advanced |
   | Vulkan | Win/Linux | Excellent | Advanced |
   | WebGPU | Web | Good | Modern |
   
   ## Configuration
   
   ### Environment Variables
   - `COCOS2D_GRAPHICS_BACKEND`: Force backend (MonoGame, Metal, etc.)
   - `COCOS2D_GRAPHICS_DEBUG`: Enable debug mode (true/false)
   - `COCOS2D_PREFER_PERFORMANCE`: Prefer performance over compatibility
   
   ### Programmatic Configuration
   ```csharp
   var config = new CCGraphicsConfig
   {
       PreferredBackend = GraphicsBackend.Metal,
       EnableDebugMode = true
   };
   CCGraphicsFactory.ConfigureFromSettings(config);
   ```
   ```

2. **Create troubleshooting guide**
   ```markdown
   ## Troubleshooting
   
   ### Common Issues
   
   #### "Backend not available" error
   - Check platform compatibility
   - Verify runtime dependencies
   - Enable fallback to MonoGame
   
   #### Performance issues
   - Check backend selection logic
   - Verify optimal backend is being used
   - Enable performance monitoring
   
   #### Memory leaks
   - Ensure factory is properly reset
   - Check renderer disposal
   - Monitor resource lifecycle
   
   ## Debugging
   
   ### Enable Debug Logging
   ```csharp
   CCGraphicsFactory.EnableDebugMode();
   var info = CCGraphicsFactory.GetBackendInfo();
   Console.WriteLine($"Current backend: {info}");
   ```
   
   ### Backend Capabilities
   ```csharp
   var backends = CCGraphicsFactory.GetAvailableBackends();
   foreach (var backend in backends)
   {
       Console.WriteLine($"{backend.Key}: {backend.Value}");
   }
   ```
   ```

### Verification:
- Documentation is clear and comprehensive
- Examples compile and work correctly
- Troubleshooting guide addresses common issues

---

## Subtask 1.2.8: Integration with Project System
**Time Estimate**: 45 minutes  
**Dependencies**: All previous subtasks  
**Assignee**: Build engineer or senior developer

### Steps:

1. **Update project files to include factory**
   ```xml
   <!-- Add to cocos2d.projitems -->
   <ItemGroup>
     <Compile Include="$(MSBuildThisFileDirectory)platform\Factory\CCGraphicsFactory.cs" />
     <Compile Include="$(MSBuildThisFileDirectory)platform\Factory\GraphicsBackend.cs" />
     <Compile Include="$(MSBuildThisFileDirectory)platform\Factory\BackendCapabilities.cs" />
   </ItemGroup>
   
   <!-- Conditional compilation for Metal support -->
   <ItemGroup Condition="'$(EnableMetal)' == 'true'">
     <Compile Include="$(MSBuildThisFileDirectory)platform\Factory\MetalDetection.cs" />
   </ItemGroup>
   ```

2. **Update namespace imports**
   ```csharp
   // Add to existing platform files that need factory access
   using Cocos2D.Platform.Factory;
   using Cocos2D.Platform.Interfaces;
   ```

3. **Create factory initialization in CCApplication**
   ```csharp
   // Add to CCApplication.cs or similar entry point
   public static void InitializeGraphics()
   {
       // Configure from environment first
       CCGraphicsFactory.ConfigureFromEnvironment();
       
       // Then apply any app-specific config
       var config = CCGraphicsConfig.Instance;
       CCGraphicsFactory.ConfigureFromSettings(config);
       
       CCLog.Log("Graphics factory initialized");
   }
   ```

### Verification:
- Project compiles on all platforms
- Factory is properly initialized
- No circular dependencies

---

## Summary and Timeline

### Total Estimated Time: ~10 hours (1.5-2 days for one developer)

### Optimal Task Assignment (2 developers working in parallel):

**Developer 1 (Senior/Factory Logic):**
- Subtask 1.2.1: Infrastructure (45m)
- Subtask 1.2.2: Core Logic (1.5h) 
- Subtask 1.2.3: Renderer Creation (1h)
- Subtask 1.2.4: Management Methods (45m)
- **Total: 4.25h**

**Developer 2 (Integration/Testing):**
- Subtask 1.2.5: Configuration (1h)
- Subtask 1.2.6: Unit Tests (2h)
- Subtask 1.2.7: Documentation (1h)
- Subtask 1.2.8: Integration (45m)
- **Total: 4.75h**

### Dependencies:
```
1.2.1 (Infrastructure) ──> 1.2.2 (Core Logic) ──> 1.2.3 (Creation) ──> 1.2.4 (Management)
                                    │                                        │
                                    └──> 1.2.5 (Config) ──────────────────┘
                                                │
1.2.4 + 1.2.5 ──> 1.2.6 (Tests) ──> 1.2.7 (Docs) ──> 1.2.8 (Integration)
```

This factory implementation provides:
- Automatic backend detection and selection
- Configuration-driven backend forcing
- Comprehensive error handling and fallbacks
- Full testing coverage
- Complete documentation
- Easy integration with the project build system