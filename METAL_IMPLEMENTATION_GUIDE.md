# Cocos2D-Mono Metal & Abstraction Layer - Detailed Implementation Guide

## Table of Contents
1. [Project Overview](#project-overview)
2. [Architecture Design](#architecture-design)
3. [Phase 1: Core Graphics Abstraction Layer](#phase-1-core-graphics-abstraction-layer)
4. [Phase 2: Refactor Core Classes](#phase-2-refactor-core-classes)
5. [Phase 3: Metal Backend Implementation](#phase-3-metal-backend-implementation)
6. [Phase 4: Platform Integration](#phase-4-platform-integration)
7. [Phase 5: Testing Framework](#phase-5-testing-framework)
8. [Phase 6: Documentation & Migration](#phase-6-documentation--migration)
9. [Implementation Notes](#implementation-notes)

## Project Overview

This project decouples cocos2d-mono from MonoGame-specific dependencies by creating an abstraction layer that supports both MonoGame and Metal rendering backends. The goal is to maintain backward compatibility while enabling native Metal performance on iOS/macOS platforms.

### Key Objectives:
- **Zero Breaking Changes**: Existing cocos2d-mono projects continue to work without modification
- **Performance Improvement**: Native Metal rendering on iOS/macOS
- **Future-Proofing**: Abstraction enables additional backends (Vulkan, WebGPU)
- **Gradual Migration**: Phased implementation with incremental testing

## Architecture Design

### Current Architecture Issues:
```csharp
// Current tight coupling - CCDrawManager.cs:38
internal static GraphicsDevice graphicsDevice;
internal static SpriteBatch spriteBatch;

// Current texture coupling - CCTexture2D.cs:60
private Texture2D m_Texture2D;

// Current drawing coupling - CCSprite.cs:989-992
CCDrawManager.BlendFunc(m_sBlendFunc);
CCDrawManager.BindTexture(Texture);
CCDrawManager.DrawQuad(ref m_sQuad);
```

### Target Architecture:
```csharp
// New abstracted architecture
interface IGraphicsDevice { }
interface ISpriteBatch { }
interface ITexture2D { }
interface IGraphicsRenderer { }

// Factory-based creation
IGraphicsRenderer renderer = CCGraphicsFactory.CreateRenderer();
```

---

## Phase 1: Core Graphics Abstraction Layer (4-6 weeks)

### Task 1.1: Define Core Interfaces (Week 1)

**Location**: Create new file `/cocos2d/platform/CCGraphicsInterfaces.cs`

```csharp
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cocos2D
{
    /// <summary>
    /// Core graphics device abstraction to decouple from MonoGame GraphicsDevice
    /// </summary>
    public interface IGraphicsDevice
    {
        // Core rendering operations
        void Clear(Color color);
        void Clear(ClearOptions options, Color color, float depth, int stencil);
        void Present();
        
        // State management
        BlendState BlendState { get; set; }
        DepthStencilState DepthStencilState { get; set; }
        RasterizerState RasterizerState { get; set; }
        Viewport Viewport { get; set; }
        Rectangle ScissorRectangle { get; set; }
        
        // Buffer management
        void SetVertexBuffer(VertexBuffer vertexBuffer);
        void SetIndices(IndexBuffer indexBuffer);
        
        // Rendering
        void DrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int primitiveCount) where T : struct, IVertexType;
        void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, short[] indexData, int indexOffset, int primitiveCount) where T : struct, IVertexType;
        void DrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex, int minVertexIndex, int numVertices, int startIndex, int primitiveCount);
        
        // Texture management
        ITexture2D CreateTexture2D(int width, int height, bool mipMap, SurfaceFormat format);
        IRenderTarget2D CreateRenderTarget2D(int width, int height, bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage);
        
        // Device state
        GraphicsDeviceStatus GraphicsDeviceStatus { get; }
        bool IsDisposed { get; }
        
        // Events for device lifecycle
        event EventHandler<EventArgs> DeviceReset;
        event EventHandler<EventArgs> DeviceResetting;
        event EventHandler<EventArgs> Disposing;
    }

    /// <summary>
    /// Texture abstraction to decouple from MonoGame Texture2D
    /// </summary>
    public interface ITexture2D : IDisposable
    {
        // Properties
        int Width { get; }
        int Height { get; }
        SurfaceFormat Format { get; }
        int LevelCount { get; }
        bool IsDisposed { get; }
        
        // Data operations
        void SetData<T>(T[] data) where T : struct;
        void SetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct;
        void GetData<T>(T[] data) where T : struct;
        void GetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct;
        
        // Save operations
        void SaveAsJpeg(System.IO.Stream stream, int width, int height);
        void SaveAsPng(System.IO.Stream stream, int width, int height);
        
        // Native texture access (for platform-specific operations)
        object NativeTexture { get; }
    }

    /// <summary>
    /// Render target abstraction
    /// </summary>
    public interface IRenderTarget2D : ITexture2D
    {
        RenderTargetUsage RenderTargetUsage { get; }
        DepthFormat DepthStencilFormat { get; }
        int MultiSampleCount { get; }
    }

    /// <summary>
    /// Sprite batch abstraction for 2D rendering
    /// </summary>
    public interface ISpriteBatch : IDisposable
    {
        void Begin();
        void Begin(SpriteSortMode sortMode, BlendState blendState);
        void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState);
        void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect);
        void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix transformMatrix);
        
        void Draw(ITexture2D texture, Rectangle destinationRectangle, Color color);
        void Draw(ITexture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color);
        void Draw(ITexture2D texture, Vector2 position, Color color);
        void Draw(ITexture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color);
        void Draw(ITexture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
        void Draw(ITexture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);
        void Draw(ITexture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth);
        
        void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color);
        void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
        void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);
        
        void End();
    }

    /// <summary>
    /// High-level graphics renderer interface
    /// </summary>
    public interface IGraphicsRenderer
    {
        // Core operations
        void Initialize();
        void BeginDraw();
        void EndDraw();
        void Clear(Color clearColor);
        void Present();
        
        // Device access
        IGraphicsDevice GraphicsDevice { get; }
        ISpriteBatch SpriteBatch { get; }
        
        // Resource creation
        ITexture2D CreateTexture(int width, int height, bool mipMap = false);
        IRenderTarget2D CreateRenderTarget(int width, int height, SurfaceFormat colorFormat, DepthFormat depthFormat, RenderTargetUsage usage);
        
        // Rendering operations
        void DrawTexture(ITexture2D texture, Vector2 position, Rectangle? sourceRectangle = null, Color color = default);
        void DrawQuad(ref CCV3F_C4B_T2F_Quad quad, ITexture2D texture);
        
        // State management
        void SetRenderTarget(IRenderTarget2D renderTarget);
        void BindTexture(ITexture2D texture);
        void SetBlendState(BlendState blendState);
        
        // Platform-specific optimizations
        bool SupportsFeature(string featureName);
        void OptimizeForPlatform();
    }
}
```

**Implementation Steps**:
1. Create the file and add interfaces
2. Add reference to existing projects
3. Build and verify no compilation errors
4. Create unit tests for interface contracts

### Task 1.2: Create Graphics Factory (Week 1)

**Location**: Create new file `/cocos2d/platform/CCGraphicsFactory.cs`

```csharp
using System;
using System.Diagnostics;

namespace Cocos2D
{
    /// <summary>
    /// Factory for creating platform-appropriate graphics renderers
    /// </summary>
    public static class CCGraphicsFactory
    {
        private static IGraphicsRenderer _currentRenderer;
        private static GraphicsBackend _forcedBackend = GraphicsBackend.Auto;
        
        public enum GraphicsBackend
        {
            Auto,           // Automatically select best available
            MonoGame,       // Force MonoGame renderer
            Metal,          // Force Metal renderer (iOS/macOS only)
            Vulkan,         // Future: Vulkan renderer
            WebGPU          // Future: WebGPU renderer
        }
        
        /// <summary>
        /// Forces a specific graphics backend. Call before any renderer creation.
        /// </summary>
        public static void ForceBackend(GraphicsBackend backend)
        {
            if (_currentRenderer != null)
            {
                throw new InvalidOperationException("Cannot change backend after renderer is created. Call ForceBackend before any graphics operations.");
            }
            _forcedBackend = backend;
        }
        
        /// <summary>
        /// Creates the appropriate graphics renderer for the current platform
        /// </summary>
        public static IGraphicsRenderer CreateRenderer()
        {
            if (_currentRenderer != null)
            {
                return _currentRenderer;
            }
            
            var backend = DetermineBackend();
            CCLog.Log($"Creating graphics renderer: {backend}");
            
            switch (backend)
            {
#if METAL && (IOS || MACOS)
                case GraphicsBackend.Metal:
                    _currentRenderer = new MetalRenderer();
                    break;
#endif
                case GraphicsBackend.MonoGame:
                default:
                    _currentRenderer = new MonoGameRenderer();
                    break;
            }
            
            return _currentRenderer;
        }
        
        /// <summary>
        /// Gets the current renderer without creating a new one
        /// </summary>
        public static IGraphicsRenderer GetCurrentRenderer()
        {
            return _currentRenderer;
        }
        
        /// <summary>
        /// Determines the best graphics backend for the current platform and configuration
        /// </summary>
        private static GraphicsBackend DetermineBackend()
        {
            if (_forcedBackend != GraphicsBackend.Auto)
            {
                return ValidateBackend(_forcedBackend);
            }
            
#if METAL && (IOS || MACOS)
            // Check if Metal is available and preferred
            if (MetalRenderer.IsSupported())
            {
                return GraphicsBackend.Metal;
            }
#endif
            
            // Default to MonoGame for maximum compatibility
            return GraphicsBackend.MonoGame;
        }
        
        /// <summary>
        /// Validates that the requested backend is available on the current platform
        /// </summary>
        private static GraphicsBackend ValidateBackend(GraphicsBackend backend)
        {
            switch (backend)
            {
#if METAL && (IOS || MACOS)
                case GraphicsBackend.Metal:
                    if (!MetalRenderer.IsSupported())
                    {
                        CCLog.Log("Metal backend requested but not supported. Falling back to MonoGame.");
                        return GraphicsBackend.MonoGame;
                    }
                    return backend;
#endif
                case GraphicsBackend.MonoGame:
                    return backend;
                    
                case GraphicsBackend.Vulkan:
                case GraphicsBackend.WebGPU:
                    CCLog.Log($"{backend} backend not yet implemented. Falling back to MonoGame.");
                    return GraphicsBackend.MonoGame;
                    
                default:
                    Debug.Assert(false, $"Unknown graphics backend: {backend}");
                    return GraphicsBackend.MonoGame;
            }
        }
        
        /// <summary>
        /// Releases the current renderer and allows creation of a new one
        /// </summary>
        internal static void Reset()
        {
            _currentRenderer?.Dispose();
            _currentRenderer = null;
        }
        
        /// <summary>
        /// Gets information about the current graphics backend
        /// </summary>
        public static string GetBackendInfo()
        {
            if (_currentRenderer == null)
                return "No renderer created";
                
            var type = _currentRenderer.GetType().Name;
#if METAL && (IOS || MACOS)
            if (_currentRenderer is MetalRenderer metalRenderer)
            {
                return $"Metal Renderer - Device: {metalRenderer.DeviceName}";
            }
#endif
            if (_currentRenderer is MonoGameRenderer monoGameRenderer)
            {
                return $"MonoGame Renderer - Adapter: {monoGameRenderer.AdapterDescription}";
            }
            
            return $"Unknown Renderer: {type}";
        }
    }
}
```

**Implementation Steps**:
1. Create factory class with platform detection
2. Add conditional compilation directives
3. Implement renderer selection logic
4. Add logging for debugging backend selection

### Task 1.3: Extract MonoGame Implementation (Week 1-2)

**Location**: Create new file `/cocos2d/platform/MonoGameRenderer.cs`

```csharp
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cocos2D
{
    /// <summary>
    /// MonoGame implementation of graphics interfaces - maintains current functionality
    /// </summary>
    public class MonoGameRenderer : IGraphicsRenderer, IDisposable
    {
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private MonoGameDevice _deviceWrapper;
        private MonoGameSpriteBatch _spriteBatchWrapper;
        
        public IGraphicsDevice GraphicsDevice => _deviceWrapper;
        public ISpriteBatch SpriteBatch => _spriteBatchWrapper;
        
        public string AdapterDescription => _graphicsDevice?.Adapter?.Description ?? "Unknown";
        
        public void Initialize()
        {
            // Will be initialized externally by CCDrawManager
            // This maintains backward compatibility
        }
        
        internal void Initialize(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            
            _deviceWrapper = new MonoGameDevice(_graphicsDevice);
            _spriteBatchWrapper = new MonoGameSpriteBatch(_spriteBatch);
        }
        
        public void BeginDraw()
        {
            // Maintained for interface compatibility
            // Actual begin/end logic stays in CCDrawManager for now
        }
        
        public void EndDraw()
        {
            // Maintained for interface compatibility
        }
        
        public void Clear(Color clearColor)
        {
            _graphicsDevice.Clear(clearColor);
        }
        
        public void Present()
        {
            // Present is handled by the Game framework
        }
        
        public ITexture2D CreateTexture(int width, int height, bool mipMap = false)
        {
            var texture = new Texture2D(_graphicsDevice, width, height, mipMap, SurfaceFormat.Color);
            return new MonoGameTexture2D(texture, false);
        }
        
        public IRenderTarget2D CreateRenderTarget(int width, int height, SurfaceFormat colorFormat, DepthFormat depthFormat, RenderTargetUsage usage)
        {
            var renderTarget = new RenderTarget2D(_graphicsDevice, width, height, false, colorFormat, depthFormat, 0, usage);
            return new MonoGameRenderTarget2D(renderTarget, false);
        }
        
        public void DrawTexture(ITexture2D texture, Vector2 position, Rectangle? sourceRectangle = null, Color color = default)
        {
            if (color == default) color = Color.White;
            
            _spriteBatch.Begin();
            _spriteBatch.Draw(((MonoGameTexture2D)texture).NativeTexture as Texture2D, position, sourceRectangle, color);
            _spriteBatch.End();
        }
        
        public void DrawQuad(ref CCV3F_C4B_T2F_Quad quad, ITexture2D texture)
        {
            // Use existing CCDrawManager quad drawing logic
            CCDrawManager.DrawQuad(ref quad);
        }
        
        public void SetRenderTarget(IRenderTarget2D renderTarget)
        {
            var nativeTarget = renderTarget?.NativeTexture as RenderTarget2D;
            _graphicsDevice.SetRenderTarget(nativeTarget);
        }
        
        public void BindTexture(ITexture2D texture)
        {
            // This will be handled by the abstracted CCDrawManager.BindTexture
        }
        
        public void SetBlendState(BlendState blendState)
        {
            _graphicsDevice.BlendState = blendState;
        }
        
        public bool SupportsFeature(string featureName)
        {
            switch (featureName.ToLowerInvariant())
            {
                case "rendertargets": return true;
                case "multisampling": return true;
                case "anisotropicfiltering": return true;
                case "separatealphablendfunctions": return true;
                default: return false;
            }
        }
        
        public void OptimizeForPlatform()
        {
            // Platform-specific optimizations for MonoGame
            // Most optimizations are handled at the MonoGame level
        }
        
        public void Dispose()
        {
            _spriteBatchWrapper?.Dispose();
            _deviceWrapper?.Dispose();
            _spriteBatch?.Dispose();
            // Note: Don't dispose _graphicsDevice as it's managed by MonoGame
        }
    }
    
    /// <summary>
    /// Wraps MonoGame GraphicsDevice to implement IGraphicsDevice
    /// </summary>
    public class MonoGameDevice : IGraphicsDevice
    {
        private readonly GraphicsDevice _device;
        
        public MonoGameDevice(GraphicsDevice device)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
        }
        
        // Properties
        public BlendState BlendState 
        { 
            get => _device.BlendState; 
            set => _device.BlendState = value; 
        }
        
        public DepthStencilState DepthStencilState 
        { 
            get => _device.DepthStencilState; 
            set => _device.DepthStencilState = value; 
        }
        
        public RasterizerState RasterizerState 
        { 
            get => _device.RasterizerState; 
            set => _device.RasterizerState = value; 
        }
        
        public Viewport Viewport 
        { 
            get => _device.Viewport; 
            set => _device.Viewport = value; 
        }
        
        public Rectangle ScissorRectangle 
        { 
            get => _device.ScissorRectangle; 
            set => _device.ScissorRectangle = value; 
        }
        
        public GraphicsDeviceStatus GraphicsDeviceStatus => _device.GraphicsDeviceStatus;
        public bool IsDisposed => _device.IsDisposed;
        
        // Events
        public event EventHandler<EventArgs> DeviceReset
        {
            add => _device.DeviceReset += value;
            remove => _device.DeviceReset -= value;
        }
        
        public event EventHandler<EventArgs> DeviceResetting
        {
            add => _device.DeviceResetting += value;
            remove => _device.DeviceResetting -= value;
        }
        
        public event EventHandler<EventArgs> Disposing
        {
            add => _device.Disposing += value;
            remove => _device.Disposing -= value;
        }
        
        // Methods
        public void Clear(Color color) => _device.Clear(color);
        public void Clear(ClearOptions options, Color color, float depth, int stencil) => _device.Clear(options, color, depth, stencil);
        public void Present() { /* Handled by framework */ }
        
        public void SetVertexBuffer(VertexBuffer vertexBuffer) => _device.SetVertexBuffer(vertexBuffer);
        public void SetIndices(IndexBuffer indexBuffer) => _device.Indices = indexBuffer;
        
        public void DrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int primitiveCount) where T : struct, IVertexType
            => _device.DrawUserPrimitives(primitiveType, vertexData, vertexOffset, primitiveCount);
            
        public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, short[] indexData, int indexOffset, int primitiveCount) where T : struct, IVertexType
            => _device.DrawUserIndexedPrimitives(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount);
            
        public void DrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex, int minVertexIndex, int numVertices, int startIndex, int primitiveCount)
            => _device.DrawIndexedPrimitives(primitiveType, baseVertex, minVertexIndex, numVertices, startIndex, primitiveCount);
        
        public ITexture2D CreateTexture2D(int width, int height, bool mipMap, SurfaceFormat format)
        {
            var texture = new Texture2D(_device, width, height, mipMap, format);
            return new MonoGameTexture2D(texture, false);
        }
        
        public IRenderTarget2D CreateRenderTarget2D(int width, int height, bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage)
        {
            var renderTarget = new RenderTarget2D(_device, width, height, mipMap, preferredFormat, preferredDepthFormat, preferredMultiSampleCount, usage);
            return new MonoGameRenderTarget2D(renderTarget, false);
        }
        
        public void Dispose()
        {
            // Don't dispose the wrapped device - it's managed externally
        }
    }
    
    /// <summary>
    /// Wraps MonoGame Texture2D to implement ITexture2D
    /// </summary>
    public class MonoGameTexture2D : ITexture2D
    {
        private readonly Texture2D _texture;
        private readonly bool _ownsTexture;
        
        public MonoGameTexture2D(Texture2D texture, bool ownsTexture = true)
        {
            _texture = texture ?? throw new ArgumentNullException(nameof(texture));
            _ownsTexture = ownsTexture;
        }
        
        public int Width => _texture.Width;
        public int Height => _texture.Height;
        public SurfaceFormat Format => _texture.Format;
        public int LevelCount => _texture.LevelCount;
        public bool IsDisposed => _texture.IsDisposed;
        public object NativeTexture => _texture;
        
        public void SetData<T>(T[] data) where T : struct => _texture.SetData(data);
        public void SetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct 
            => _texture.SetData(level, rect, data, startIndex, elementCount);
            
        public void GetData<T>(T[] data) where T : struct => _texture.GetData(data);
        public void GetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct 
            => _texture.GetData(level, rect, data, startIndex, elementCount);
            
        public void SaveAsJpeg(System.IO.Stream stream, int width, int height) => _texture.SaveAsJpeg(stream, width, height);
        public void SaveAsPng(System.IO.Stream stream, int width, int height) => _texture.SaveAsPng(stream, width, height);
        
        public void Dispose()
        {
            if (_ownsTexture && !_texture.IsDisposed)
            {
                _texture.Dispose();
            }
        }
    }
    
    /// <summary>
    /// Wraps MonoGame RenderTarget2D to implement IRenderTarget2D
    /// </summary>
    public class MonoGameRenderTarget2D : MonoGameTexture2D, IRenderTarget2D
    {
        private readonly RenderTarget2D _renderTarget;
        
        public MonoGameRenderTarget2D(RenderTarget2D renderTarget, bool ownsTexture = true) 
            : base(renderTarget, ownsTexture)
        {
            _renderTarget = renderTarget;
        }
        
        public RenderTargetUsage RenderTargetUsage => _renderTarget.RenderTargetUsage;
        public DepthFormat DepthStencilFormat => _renderTarget.DepthStencilFormat;
        public int MultiSampleCount => _renderTarget.MultiSampleCount;
    }
    
    /// <summary>
    /// Wraps MonoGame SpriteBatch to implement ISpriteBatch
    /// </summary>
    public class MonoGameSpriteBatch : ISpriteBatch
    {
        private readonly SpriteBatch _spriteBatch;
        
        public MonoGameSpriteBatch(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));
        }
        
        public void Begin() => _spriteBatch.Begin();
        public void Begin(SpriteSortMode sortMode, BlendState blendState) => _spriteBatch.Begin(sortMode, blendState);
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState) 
            => _spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState);
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect) 
            => _spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect);
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix transformMatrix) 
            => _spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
        
        public void Draw(ITexture2D texture, Rectangle destinationRectangle, Color color)
            => _spriteBatch.Draw(GetNativeTexture(texture), destinationRectangle, color);
        public void Draw(ITexture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
            => _spriteBatch.Draw(GetNativeTexture(texture), destinationRectangle, sourceRectangle, color);
        public void Draw(ITexture2D texture, Vector2 position, Color color)
            => _spriteBatch.Draw(GetNativeTexture(texture), position, color);
        public void Draw(ITexture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
            => _spriteBatch.Draw(GetNativeTexture(texture), position, sourceRectangle, color);
        public void Draw(ITexture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
            => _spriteBatch.Draw(GetNativeTexture(texture), position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        public void Draw(ITexture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
            => _spriteBatch.Draw(GetNativeTexture(texture), position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        public void Draw(ITexture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
            => _spriteBatch.Draw(GetNativeTexture(texture), destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
        
        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
            => _spriteBatch.DrawString(spriteFont, text, position, color);
        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
            => _spriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
            => _spriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
        
        public void End() => _spriteBatch.End();
        
        private Texture2D GetNativeTexture(ITexture2D texture)
        {
            return texture?.NativeTexture as Texture2D ?? throw new ArgumentException("Invalid texture type for MonoGame renderer");
        }
        
        public void Dispose()
        {
            // Don't dispose the wrapped SpriteBatch - it's managed externally
        }
    }
}
```

**Implementation Steps**:
1. Create MonoGame wrapper classes
2. Test with existing functionality to ensure no regressions
3. Add proper error handling and validation
4. Create unit tests for wrapper functionality

---

## Phase 2: Refactor Core Classes (3-4 weeks)

### Task 2.1: Refactor CCDrawManager (Week 3-4)

**Location**: Modify `/cocos2d/platform/CCDrawManager.cs`

**Step 1**: Add abstraction fields at the top of CCDrawManager class:

```csharp
// Add these fields after line 38
private static IGraphicsRenderer s_renderer;
private static IGraphicsDevice s_abstractDevice;
private static ISpriteBatch s_abstractSpriteBatch;

// Backward compatibility properties
internal static GraphicsDevice graphicsDevice => 
    (s_abstractDevice as MonoGameDevice)?.NativeDevice ?? 
    throw new InvalidOperationException("Legacy GraphicsDevice access not available with current renderer");

public static SpriteBatch spriteBatch => 
    (s_abstractSpriteBatch as MonoGameSpriteBatch)?.NativeSpriteBatch ?? 
    throw new InvalidOperationException("Legacy SpriteBatch access not available with current renderer");
```

**Step 2**: Modify Init method around line 397:

```csharp
public static void Init(GraphicsDevice graphicsDevice)
{
    // Initialize the abstraction layer
    s_renderer = CCGraphicsFactory.CreateRenderer();
    
    // For MonoGame renderer, initialize with the provided GraphicsDevice
    if (s_renderer is MonoGameRenderer monoGameRenderer)
    {
        monoGameRenderer.Initialize(graphicsDevice);
    }
    
    s_abstractDevice = s_renderer.GraphicsDevice;
    s_abstractSpriteBatch = s_renderer.SpriteBatch;
    
    // Keep existing initialization for backward compatibility
    CCDrawManager.graphicsDevice = graphicsDevice;
    spriteBatch = new SpriteBatch(graphicsDevice);
    
    // ... rest of existing Init method remains the same
}
```

**Step 3**: Update texture binding method around line 843:

```csharp
public static void BindTexture(CCTexture2D texture)
{
    // Use abstracted texture binding
    if (s_renderer != null)
    {
        s_renderer.BindTexture(texture?.AbstractTexture);
    }
    
    // Maintain backward compatibility
    Texture2D tex = texture?.XNATexture;
    
    if (!graphicsDevice.IsDisposed && graphicsDevice.GraphicsDeviceStatus == GraphicsDeviceStatus.Normal)
    {
        if (tex == null)
        {
            graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            TextureEnabled = false;
        }
        else
        {
            graphicsDevice.SamplerStates[0] = texture.SamplerState;
            TextureEnabled = true;
        }

        if (m_currentTexture != tex)
        {
            m_currentTexture = tex;
            m_textureChanged = true;
        }
    }
}
```

**Step 4**: Update quad drawing methods around line 1036:

```csharp
public static void DrawQuad(ref CCV3F_C4B_T2F_Quad quad)
{
    // Use abstracted rendering if available
    if (s_renderer != null && m_currentTexture != null)
    {
        // Create abstracted texture wrapper for current texture
        var abstractTexture = new MonoGameTexture2D(m_currentTexture, false);
        s_renderer.DrawQuad(ref quad, abstractTexture);
        return;
    }
    
    // Fallback to existing implementation
    CCV3F_C4B_T2F[] vertices = m_quadVertices;

    if (vertices == null)
    {
        vertices = m_quadVertices = new CCV3F_C4B_T2F[4];
        CheckQuadsIndexBuffer(1);
    }

    vertices[0] = quad.TopLeft;
    vertices[1] = quad.BottomLeft;
    vertices[2] = quad.TopRight;
    vertices[3] = quad.BottomRight;

    DrawIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, 4, m_quadsIndexBuffer.Data.Elements, 0, 2);
}
```

**Step 5**: Add abstracted render target methods:

```csharp
// Add these methods to CCDrawManager
public static void SetRenderTarget(ITexture2D texture)
{
    if (s_renderer != null && texture is IRenderTarget2D renderTarget)
    {
        s_renderer.SetRenderTarget(renderTarget);
    }
    
    // Maintain backward compatibility
    if (texture is CCTexture2D ccTexture)
    {
        SetRenderTarget(ccTexture);
    }
}

public static IRenderTarget2D CreateAbstractRenderTarget(int width, int height, SurfaceFormat colorFormat, DepthFormat depthFormat, RenderTargetUsage usage)
{
    if (s_renderer != null)
    {
        return s_renderer.CreateRenderTarget(width, height, colorFormat, depthFormat, usage);
    }
    
    // Fallback to existing method
    var renderTarget = CreateRenderTarget(width, height, colorFormat, depthFormat, usage);
    return new MonoGameRenderTarget2D(renderTarget, false);
}
```

### Task 2.2: Refactor CCTexture2D (Week 4-5)

**Location**: Modify `/cocos2d/textures/CCTexture2D.cs`

**Step 1**: Add abstraction field to CCTexture2D class after line 60:

```csharp
// Add after line 60
private ITexture2D m_AbstractTexture;
private readonly bool m_UseAbstraction;

// Add property for abstracted access
public ITexture2D AbstractTexture
{
    get
    {
        if (m_UseAbstraction && m_AbstractTexture != null)
        {
            return m_AbstractTexture;
        }
        
        // Create wrapper for legacy texture
        if (m_Texture2D != null && !m_Texture2D.IsDisposed)
        {
            return new MonoGameTexture2D(m_Texture2D, false);
        }
        
        return null;
    }
}
```

**Step 2**: Add abstracted constructor:

```csharp
// Add new constructor
public CCTexture2D(ITexture2D abstractTexture, bool useAbstraction = true) : this()
{
    m_UseAbstraction = useAbstraction;
    m_AbstractTexture = abstractTexture;
    
    if (abstractTexture != null)
    {
        m_uPixelsWide = abstractTexture.Width;
        m_uPixelsHigh = abstractTexture.Height;
        m_ePixelFormat = abstractTexture.Format;
        m_tContentSize = new CCSize(abstractTexture.Width, abstractTexture.Height);
        
        // If it's a MonoGame texture, also set the legacy field
        if (abstractTexture is MonoGameTexture2D monoTexture)
        {
            m_Texture2D = monoTexture.NativeTexture as Texture2D;
        }
    }
}
```

**Step 3**: Update XNATexture property around line 103:

```csharp
public Texture2D XNATexture
{
    get
    {
        // Return from abstraction if available
        if (m_UseAbstraction && m_AbstractTexture is MonoGameTexture2D monoTexture)
        {
            return monoTexture.NativeTexture as Texture2D;
        }
        
        // Fallback to legacy implementation
        if (m_Texture2D == null || m_Texture2D.IsDisposed)
        {
            Reinit();
        }
        return m_Texture2D;
    }
}
```

**Step 4**: Update InitWithTexture method around line 687:

```csharp
internal bool InitWithTexture(Texture2D texture, SurfaceFormat format, bool premultipliedAlpha, bool managed)
{
    // Create abstracted texture
    if (texture != null)
    {
        m_AbstractTexture = new MonoGameTexture2D(texture, !managed);
        m_UseAbstraction = true;
    }
    
    // Continue with existing implementation for backward compatibility
    m_bManaged = managed;

    if (null == texture)
    {
        return false;
    }

    // ... rest of existing method remains the same
}
```

**Step 5**: Update data operations to use abstraction:

```csharp
// Add abstracted data methods
public void SetAbstractData<T>(T[] data) where T : struct
{
    if (m_UseAbstraction && m_AbstractTexture != null)
    {
        m_AbstractTexture.SetData(data);
        return;
    }
    
    // Fallback to legacy
    XNATexture.SetData(data);
}

public void GetAbstractData<T>(T[] data) where T : struct
{
    if (m_UseAbstraction && m_AbstractTexture != null)
    {
        m_AbstractTexture.GetData(data);
        return;
    }
    
    // Fallback to legacy
    XNATexture.GetData(data);
}
```

### Task 2.3: Refactor CCSprite (Week 5)

**Location**: Modify `/cocos2d/sprite_nodes/CCSprite.cs`

**Step 1**: Update Draw method around line 985:

```csharp
public override void Draw()
{
    Debug.Assert(m_pobBatchNode == null);

    // Use abstracted rendering
    var renderer = CCGraphicsFactory.GetCurrentRenderer();
    if (renderer != null)
    {
        renderer.SetBlendState(CCBlendFunc.ToBlendState(m_sBlendFunc));
        renderer.BindTexture(Texture?.AbstractTexture);
        renderer.DrawQuad(ref m_sQuad, Texture?.AbstractTexture);
        return;
    }
    
    // Fallback to legacy rendering
    CCDrawManager.BlendFunc(m_sBlendFunc);
    CCDrawManager.BindTexture(Texture);
    CCDrawManager.DrawQuad(ref m_sQuad);
}
```

**Step 2**: Add helper extension for blend state conversion:

```csharp
// Add this as a separate file: /cocos2d/platform/CCBlendFuncExtensions.cs
using Microsoft.Xna.Framework.Graphics;

namespace Cocos2D
{
    public static class CCBlendFuncExtensions
    {
        public static BlendState ToBlendState(this CCBlendFunc blendFunc)
        {
            if (blendFunc == CCBlendFunc.AlphaBlend)
                return BlendState.AlphaBlend;
            if (blendFunc == CCBlendFunc.Additive)
                return BlendState.Additive;
            if (blendFunc == CCBlendFunc.NonPremultiplied)
                return BlendState.NonPremultiplied;
            if (blendFunc == CCBlendFunc.Opaque)
                return BlendState.Opaque;
                
            // Create custom blend state for other combinations
            return new BlendState
            {
                ColorSourceBlend = CCOGLES.GetXNABlend(blendFunc.Source),
                AlphaSourceBlend = CCOGLES.GetXNABlend(blendFunc.Source),
                ColorDestinationBlend = CCOGLES.GetXNABlend(blendFunc.Destination),
                AlphaDestinationBlend = CCOGLES.GetXNABlend(blendFunc.Destination)
            };
        }
    }
}
```

---

## Phase 3: Metal Backend Implementation (6-8 weeks)

### Task 3.1: Metal Infrastructure (Week 6-7)

**Location**: Create new file `/cocos2d/platform/MetalRenderer.cs`

**Important**: This requires Metal C# bindings. For iOS/macOS, you'll need to use Xamarin.iOS or .NET MAUI Metal bindings.

```csharp
#if METAL && (IOS || MACOS)
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Metal;
using MetalKit;
using Foundation;

namespace Cocos2D
{
    /// <summary>
    /// Metal implementation of graphics renderer for iOS/macOS
    /// </summary>
    public class MetalRenderer : IGraphicsRenderer, IDisposable
    {
        private IMTLDevice _device;
        private IMTLCommandQueue _commandQueue;
        private IMTLRenderCommandEncoder _currentEncoder;
        private IMTLCommandBuffer _currentCommandBuffer;
        private MTKView _metalView;
        
        private MetalDevice _deviceWrapper;
        private MetalSpriteBatch _spriteBatchWrapper;
        
        // Pipeline states for different rendering modes
        private IMTLRenderPipelineState _spritePipelineState;
        private IMTLRenderPipelineState _primitivePipelineState;
        
        // Vertex/Index buffers for quad rendering
        private IMTLBuffer _quadVertexBuffer;
        private IMTLBuffer _quadIndexBuffer;
        private int _currentQuadCount;
        private const int MaxQuadsPerBatch = 10922; // 65536 vertices / 6 vertices per quad
        
        public IGraphicsDevice GraphicsDevice => _deviceWrapper;
        public ISpriteBatch SpriteBatch => _spriteBatchWrapper;
        
        public string DeviceName => _device?.Name ?? "Unknown Metal Device";
        
        public static bool IsSupported()
        {
            // Check if Metal is available on this device
            try
            {
                var device = MTLDevice.SystemDefault;
                return device != null;
            }
            catch
            {
                return false;
            }
        }
        
        public void Initialize()
        {
            InitializeMetal();
            CreatePipelineStates();
            CreateBuffers();
            
            _deviceWrapper = new MetalDevice(_device, _commandQueue);
            _spriteBatchWrapper = new MetalSpriteBatch(this);
        }
        
        private void InitializeMetal()
        {
            _device = MTLDevice.SystemDefault;
            if (_device == null)
                throw new InvalidOperationException("Metal is not supported on this device");
                
            _commandQueue = _device.CreateCommandQueue();
            if (_commandQueue == null)
                throw new InvalidOperationException("Failed to create Metal command queue");
                
            CCLog.Log($"Metal Renderer initialized with device: {_device.Name}");
        }
        
        private void CreatePipelineStates()
        {
            // Create shader library from embedded shaders
            var library = CreateShaderLibrary();
            
            // Sprite rendering pipeline
            var spriteDescriptor = new MTLRenderPipelineDescriptor
            {
                VertexFunction = library.CreateFunction("sprite_vertex"),
                FragmentFunction = library.CreateFunction("sprite_fragment"),
                ColorAttachments = { [0] = new MTLRenderPipelineColorAttachmentDescriptor
                {
                    PixelFormat = MTLPixelFormat.BGRA8Unorm,
                    BlendingEnabled = true,
                    SourceRgbBlendFactor = MTLBlendFactor.SourceAlpha,
                    DestinationRgbBlendFactor = MTLBlendFactor.OneMinusSourceAlpha,
                    SourceAlphaBlendFactor = MTLBlendFactor.One,
                    DestinationAlphaBlendFactor = MTLBlendFactor.OneMinusSourceAlpha
                }}
            };
            
            var error = new NSError();
            _spritePipelineState = _device.CreateRenderPipelineState(spriteDescriptor, out error);
            if (error != null)
                throw new InvalidOperationException($"Failed to create sprite pipeline: {error.LocalizedDescription}");
        }
        
        private IMTLLibrary CreateShaderLibrary()
        {
            // In a real implementation, you would load compiled Metal shaders
            // For now, we'll create them from source (this is slower but easier for development)
            
            string shaderSource = @"
#include <metal_stdlib>
using namespace metal;

struct VertexIn {
    float3 position [[attribute(0)]];
    float4 color [[attribute(1)]];
    float2 texCoord [[attribute(2)]];
};

struct VertexOut {
    float4 position [[position]];
    float4 color;
    float2 texCoord;
};

struct Uniforms {
    float4x4 modelViewProjectionMatrix;
};

vertex VertexOut sprite_vertex(VertexIn in [[stage_in]],
                              constant Uniforms& uniforms [[buffer(1)]]) {
    VertexOut out;
    out.position = uniforms.modelViewProjectionMatrix * float4(in.position, 1.0);
    out.color = in.color;
    out.texCoord = in.texCoord;
    return out;
}

fragment float4 sprite_fragment(VertexOut in [[stage_in]],
                               texture2d<float> colorTexture [[texture(0)]],
                               sampler colorSampler [[sampler(0)]]) {
    float4 color = colorTexture.sample(colorSampler, in.texCoord) * in.color;
    return color;
}
";
            
            var error = new NSError();
            var library = _device.CreateLibrary(shaderSource, new MTLCompileOptions(), out error);
            if (error != null)
                throw new InvalidOperationException($"Failed to compile shaders: {error.LocalizedDescription}");
                
            return library;
        }
        
        private void CreateBuffers()
        {
            // Create vertex buffer for quad rendering (same format as CCV3F_C4B_T2F_Quad)
            var vertexBufferSize = MaxQuadsPerBatch * 4 * System.Runtime.InteropServices.Marshal.SizeOf<CCV3F_C4B_T2F>();
            _quadVertexBuffer = _device.CreateBuffer((nuint)vertexBufferSize, MTLResourceOptions.CpuCacheModeWriteCombined);
            
            // Create index buffer for quad rendering
            var indices = new ushort[MaxQuadsPerBatch * 6];
            for (int i = 0; i < MaxQuadsPerBatch; i++)
            {
                indices[i * 6 + 0] = (ushort)(i * 4 + 0);
                indices[i * 6 + 1] = (ushort)(i * 4 + 1);
                indices[i * 6 + 2] = (ushort)(i * 4 + 2);
                indices[i * 6 + 3] = (ushort)(i * 4 + 1);
                indices[i * 6 + 4] = (ushort)(i * 4 + 3);
                indices[i * 6 + 5] = (ushort)(i * 4 + 2);
            }
            
            _quadIndexBuffer = _device.CreateBuffer(indices, MTLResourceOptions.CpuCacheModeDefaultCache);
        }
        
        public void BeginDraw()
        {
            _currentCommandBuffer = _commandQueue.CommandBuffer();
            
            // Create render pass descriptor
            var renderPassDescriptor = new MTLRenderPassDescriptor();
            renderPassDescriptor.ColorAttachments[0].Texture = GetCurrentDrawableTexture();
            renderPassDescriptor.ColorAttachments[0].LoadAction = MTLLoadAction.Clear;
            renderPassDescriptor.ColorAttachments[0].StoreAction = MTLStoreAction.Store;
            renderPassDescriptor.ColorAttachments[0].ClearColor = new MTLClearColor(0, 0, 0, 1);
            
            _currentEncoder = _currentCommandBuffer.CreateRenderCommandEncoder(renderPassDescriptor);
            _currentEncoder.SetRenderPipelineState(_spritePipelineState);
            
            _currentQuadCount = 0;
        }
        
        public void EndDraw()
        {
            if (_currentQuadCount > 0)
            {
                FlushQuadBatch();
            }
            
            _currentEncoder?.EndEncoding();
            _currentCommandBuffer?.PresentDrawable(GetCurrentDrawable());
            _currentCommandBuffer?.Commit();
            
            _currentEncoder = null;
            _currentCommandBuffer = null;
        }
        
        public void Clear(Color clearColor)
        {
            // Clear is handled in BeginDraw via render pass descriptor
            // This method could update the clear color for the next frame
        }
        
        public void Present()
        {
            // Present is handled in EndDraw
        }
        
        public ITexture2D CreateTexture(int width, int height, bool mipMap = false)
        {
            var descriptor = new MTLTextureDescriptor
            {
                TextureType = MTLTextureType.Type2D,
                PixelFormat = MTLPixelFormat.RGBA8Unorm,
                Width = (nuint)width,
                Height = (nuint)height,
                MipmapLevelCount = mipMap ? (nuint)Math.Floor(Math.Log2(Math.Max(width, height))) + 1 : 1,
                Usage = MTLTextureUsage.ShaderRead
            };
            
            var metalTexture = _device.CreateTexture(descriptor);
            return new MetalTexture2D(metalTexture, descriptor);
        }
        
        public IRenderTarget2D CreateRenderTarget(int width, int height, SurfaceFormat colorFormat, DepthFormat depthFormat, RenderTargetUsage usage)
        {
            var descriptor = new MTLTextureDescriptor
            {
                TextureType = MTLTextureType.Type2D,
                PixelFormat = ConvertSurfaceFormat(colorFormat),
                Width = (nuint)width,
                Height = (nuint)height,
                MipmapLevelCount = 1,
                Usage = MTLTextureUsage.RenderTarget | MTLTextureUsage.ShaderRead
            };
            
            var metalTexture = _device.CreateTexture(descriptor);
            return new MetalRenderTarget2D(metalTexture, descriptor, usage, depthFormat);
        }
        
        public void DrawTexture(ITexture2D texture, Vector2 position, Rectangle? sourceRectangle = null, Color color = default)
        {
            if (color == default) color = Color.White;
            
            // Create a quad for this texture
            var quad = CreateTextureQuad(texture, position, sourceRectangle, color);
            DrawQuad(ref quad, texture);
        }
        
        public void DrawQuad(ref CCV3F_C4B_T2F_Quad quad, ITexture2D texture)
        {
            if (_currentQuadCount >= MaxQuadsPerBatch)
            {
                FlushQuadBatch();
            }
            
            // Copy quad data to vertex buffer
            unsafe
            {
                var vertexPtr = (CCV3F_C4B_T2F*)_quadVertexBuffer.Contents.ToPointer();
                var quadPtr = vertexPtr + (_currentQuadCount * 4);
                
                quadPtr[0] = quad.TopLeft;
                quadPtr[1] = quad.BottomLeft;
                quadPtr[2] = quad.TopRight;
                quadPtr[3] = quad.BottomRight;
            }
            
            _currentQuadCount++;
        }
        
        private void FlushQuadBatch()
        {
            if (_currentQuadCount == 0) return;
            
            _currentEncoder.SetVertexBuffer(_quadVertexBuffer, 0, 0);
            _currentEncoder.DrawIndexedPrimitives(MTLPrimitiveType.Triangle, (nuint)(_currentQuadCount * 6), MTLIndexType.UInt16, _quadIndexBuffer, 0);
            
            _currentQuadCount = 0;
        }
        
        public void SetRenderTarget(IRenderTarget2D renderTarget)
        {
            // Metal render targets are set via render pass descriptors
            // This would require ending current encoding and starting a new one
            throw new NotImplementedException("Metal render target switching requires render pass management");
        }
        
        public void BindTexture(ITexture2D texture)
        {
            if (texture is MetalTexture2D metalTexture)
            {
                _currentEncoder?.SetFragmentTexture(metalTexture.NativeTexture as IMTLTexture, 0);
            }
        }
        
        public void SetBlendState(BlendState blendState)
        {
            // Metal blend states are compiled into pipeline states
            // Dynamic blend state changes require different pipeline states
            // For now, we'll use the default alpha blend
        }
        
        public bool SupportsFeature(string featureName)
        {
            switch (featureName.ToLowerInvariant())
            {
                case "rendertargets": return true;
                case "multisampling": return true;
                case "anisotropicfiltering": return true;
                case "separatealphablendfunctions": return true;
                case "computeshaders": return true;
                case "tessellation": return _device.MaxTessellationFactor > 0;
                default: return false;
            }
        }
        
        public void OptimizeForPlatform()
        {
            // Metal-specific optimizations
            // - Use memoryless render targets where appropriate
            // - Enable GPU-driven rendering
            // - Optimize buffer usage patterns
        }
        
        private MTLPixelFormat ConvertSurfaceFormat(SurfaceFormat format)
        {
            switch (format)
            {
                case SurfaceFormat.Color: return MTLPixelFormat.RGBA8Unorm;
                case SurfaceFormat.Bgr565: return MTLPixelFormat.B5G6R5Unorm;
                case SurfaceFormat.Bgra5551: return MTLPixelFormat.BGR5A1Unorm;
                case SurfaceFormat.Bgra4444: return MTLPixelFormat.ABGR4Unorm;
                case SurfaceFormat.Alpha8: return MTLPixelFormat.A8Unorm;
                default: return MTLPixelFormat.RGBA8Unorm;
            }
        }
        
        // Helper methods for getting current drawable (these would be set up by the Metal view)
        private IMTLTexture GetCurrentDrawableTexture()
        {
            return _metalView?.CurrentDrawable?.Texture;
        }
        
        private IMTLDrawable GetCurrentDrawable()
        {
            return _metalView?.CurrentDrawable;
        }
        
        private CCV3F_C4B_T2F_Quad CreateTextureQuad(ITexture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            var quad = new CCV3F_C4B_T2F_Quad();
            
            var size = sourceRectangle?.Size ?? new Point(texture.Width, texture.Height);
            var source = sourceRectangle ?? new Rectangle(0, 0, texture.Width, texture.Height);
            
            // Vertex positions
            quad.TopLeft.Vertices = new CCVertex3F(position.X, position.Y + size.Y, 0);
            quad.TopRight.Vertices = new CCVertex3F(position.X + size.X, position.Y + size.Y, 0);
            quad.BottomLeft.Vertices = new CCVertex3F(position.X, position.Y, 0);
            quad.BottomRight.Vertices = new CCVertex3F(position.X + size.X, position.Y, 0);
            
            // Texture coordinates
            float left = (float)source.X / texture.Width;
            float right = (float)(source.X + source.Width) / texture.Width;
            float top = (float)source.Y / texture.Height;
            float bottom = (float)(source.Y + source.Height) / texture.Height;
            
            quad.TopLeft.TexCoords = new CCTex2F(left, top);
            quad.TopRight.TexCoords = new CCTex2F(right, top);
            quad.BottomLeft.TexCoords = new CCTex2F(left, bottom);
            quad.BottomRight.TexCoords = new CCTex2F(right, bottom);
            
            // Colors
            var ccColor = new CCColor4B(color.R, color.G, color.B, color.A);
            quad.TopLeft.Colors = ccColor;
            quad.TopRight.Colors = ccColor;
            quad.BottomLeft.Colors = ccColor;
            quad.BottomRight.Colors = ccColor;
            
            return quad;
        }
        
        public void Dispose()
        {
            _currentEncoder?.Dispose();
            _currentCommandBuffer?.Dispose();
            _commandQueue?.Dispose();
            _quadVertexBuffer?.Dispose();
            _quadIndexBuffer?.Dispose();
            _spritePipelineState?.Dispose();
            _primitivePipelineState?.Dispose();
        }
    }
}
#endif // METAL && (IOS || MACOS)
```

### Task 3.2: Metal Texture Implementation (Week 7-8)

**Location**: Create new file `/cocos2d/platform/MetalTexture2D.cs`

```csharp
#if METAL && (IOS || MACOS)
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Metal;
using Foundation;

namespace Cocos2D
{
    /// <summary>
    /// Metal implementation of ITexture2D using MTLTexture
    /// </summary>
    public class MetalTexture2D : ITexture2D
    {
        private readonly IMTLTexture _texture;
        private readonly MTLTextureDescriptor _descriptor;
        private readonly bool _ownsTexture;
        
        public MetalTexture2D(IMTLTexture texture, MTLTextureDescriptor descriptor, bool ownsTexture = true)
        {
            _texture = texture ?? throw new ArgumentNullException(nameof(texture));
            _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            _ownsTexture = ownsTexture;
        }
        
        public int Width => (int)_texture.Width;
        public int Height => (int)_texture.Height;
        public SurfaceFormat Format => ConvertFromMetalFormat(_texture.PixelFormat);
        public int LevelCount => (int)_texture.MipmapLevelCount;
        public bool IsDisposed { get; private set; }
        public object NativeTexture => _texture;
        
        public void SetData<T>(T[] data) where T : struct
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(MetalTexture2D));
            
            SetData(0, null, data, 0, data.Length);
        }
        
        public void SetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(MetalTexture2D));
            
            var region = rect.HasValue 
                ? new MTLRegion(rect.Value.X, rect.Value.Y, rect.Value.Width, rect.Value.Height)
                : new MTLRegion(0, 0, Width >> level, Height >> level);
                
            var bytesPerPixel = GetBytesPerPixel();
            var rowBytes = region.Size.Width * (nuint)bytesPerPixel;
            
            unsafe
            {
                var dataHandle = System.Runtime.InteropServices.GCHandle.Alloc(data, System.Runtime.InteropServices.GCHandleType.Pinned);
                try
                {
                    var dataPtr = new IntPtr(dataHandle.AddrOfPinnedObject().ToInt64() + startIndex * System.Runtime.InteropServices.Marshal.SizeOf<T>());
                    _texture.ReplaceRegion(region, (nuint)level, dataPtr, rowBytes);
                }
                finally
                {
                    dataHandle.Free();
                }
            }
        }
        
        public void GetData<T>(T[] data) where T : struct
        {
            GetData(0, null, data, 0, data.Length);
        }
        
        public void GetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(MetalTexture2D));
            
            // Metal doesn't support direct texture readback like MonoGame
            // This would require creating a blit command encoder and copying to a CPU-accessible buffer
            throw new NotImplementedException("Metal texture readback requires additional implementation for GPU-to-CPU data transfer");
        }
        
        public void SaveAsJpeg(System.IO.Stream stream, int width, int height)
        {
            // Would require conversion to CGImage or similar platform-specific image format
            throw new NotImplementedException("Metal texture saving requires platform-specific image conversion");
        }
        
        public void SaveAsPng(System.IO.Stream stream, int width, int height)
        {
            // Would require conversion to CGImage or similar platform-specific image format
            throw new NotImplementedException("Metal texture saving requires platform-specific image conversion");
        }
        
        private int GetBytesPerPixel()
        {
            switch (_texture.PixelFormat)
            {
                case MTLPixelFormat.RGBA8Unorm:
                case MTLPixelFormat.BGRA8Unorm:
                    return 4;
                case MTLPixelFormat.RG8Unorm:
                    return 2;
                case MTLPixelFormat.R8Unorm:
                case MTLPixelFormat.A8Unorm:
                    return 1;
                case MTLPixelFormat.RGBA16Float:
                    return 8;
                case MTLPixelFormat.RGBA32Float:
                    return 16;
                default:
                    return 4; // Default assumption
            }
        }
        
        private SurfaceFormat ConvertFromMetalFormat(MTLPixelFormat format)
        {
            switch (format)
            {
                case MTLPixelFormat.RGBA8Unorm: return SurfaceFormat.Color;
                case MTLPixelFormat.BGRA8Unorm: return SurfaceFormat.Color;
                case MTLPixelFormat.B5G6R5Unorm: return SurfaceFormat.Bgr565;
                case MTLPixelFormat.BGR5A1Unorm: return SurfaceFormat.Bgra5551;
                case MTLPixelFormat.ABGR4Unorm: return SurfaceFormat.Bgra4444;
                case MTLPixelFormat.A8Unorm: return SurfaceFormat.Alpha8;
                default: return SurfaceFormat.Color;
            }
        }
        
        public void Dispose()
        {
            if (!IsDisposed && _ownsTexture)
            {
                _texture?.Dispose();
            }
            IsDisposed = true;
        }
    }
    
    /// <summary>
    /// Metal implementation of IRenderTarget2D
    /// </summary>
    public class MetalRenderTarget2D : MetalTexture2D, IRenderTarget2D
    {
        private readonly RenderTargetUsage _usage;
        private readonly DepthFormat _depthFormat;
        
        public MetalRenderTarget2D(IMTLTexture texture, MTLTextureDescriptor descriptor, RenderTargetUsage usage, DepthFormat depthFormat, bool ownsTexture = true)
            : base(texture, descriptor, ownsTexture)
        {
            _usage = usage;
            _depthFormat = depthFormat;
        }
        
        public RenderTargetUsage RenderTargetUsage => _usage;
        public DepthFormat DepthStencilFormat => _depthFormat;
        public int MultiSampleCount => 0; // Metal MSAA would require additional setup
    }
    
    /// <summary>
    /// Metal implementation of IGraphicsDevice wrapper
    /// </summary>
    public class MetalDevice : IGraphicsDevice
    {
        private readonly IMTLDevice _device;
        private readonly IMTLCommandQueue _commandQueue;
        private BlendState _blendState = BlendState.AlphaBlend;
        private DepthStencilState _depthStencilState = DepthStencilState.Default;
        private RasterizerState _rasterizerState = RasterizerState.CullNone;
        private Viewport _viewport = new Viewport(0, 0, 800, 600);
        private Rectangle _scissorRectangle = new Rectangle(0, 0, 800, 600);
        
        public MetalDevice(IMTLDevice device, IMTLCommandQueue commandQueue)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
            _commandQueue = commandQueue ?? throw new ArgumentNullException(nameof(commandQueue));
        }
        
        // State properties
        public BlendState BlendState
        {
            get => _blendState;
            set => _blendState = value ?? BlendState.AlphaBlend;
        }
        
        public DepthStencilState DepthStencilState
        {
            get => _depthStencilState;
            set => _depthStencilState = value ?? DepthStencilState.Default;
        }
        
        public RasterizerState RasterizerState
        {
            get => _rasterizerState;
            set => _rasterizerState = value ?? RasterizerState.CullNone;
        }
        
        public Viewport Viewport
        {
            get => _viewport;
            set => _viewport = value;
        }
        
        public Rectangle ScissorRectangle
        {
            get => _scissorRectangle;
            set => _scissorRectangle = value;
        }
        
        public GraphicsDeviceStatus GraphicsDeviceStatus => GraphicsDeviceStatus.Normal;
        public bool IsDisposed { get; private set; }
        
        // Events (Metal doesn't have direct equivalents, but we can simulate them)
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;
        public event EventHandler<EventArgs> Disposing;
        
        // Rendering methods
        public void Clear(Color color)
        {
            // Metal clearing is handled via render pass descriptors
            // This method would update the clear color for the next render pass
        }
        
        public void Clear(ClearOptions options, Color color, float depth, int stencil)
        {
            // Metal clearing is handled via render pass descriptors
        }
        
        public void Present()
        {
            // Present is handled by the command buffer
        }
        
        // Buffer management (Metal uses different paradigms)
        public void SetVertexBuffer(VertexBuffer vertexBuffer)
        {
            // Metal buffer binding is done in the render command encoder
            throw new NotImplementedException("Metal vertex buffer binding is handled differently");
        }
        
        public void SetIndices(IndexBuffer indexBuffer)
        {
            // Metal index buffer binding is done in draw calls
            throw new NotImplementedException("Metal index buffer binding is handled differently");
        }
        
        // Drawing methods
        public void DrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int primitiveCount) where T : struct, IVertexType
        {
            throw new NotImplementedException("Metal user primitives require command encoder context");
        }
        
        public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, short[] indexData, int indexOffset, int primitiveCount) where T : struct, IVertexType
        {
            throw new NotImplementedException("Metal indexed primitives require command encoder context");
        }
        
        public void DrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex, int minVertexIndex, int numVertices, int startIndex, int primitiveCount)
        {
            throw new NotImplementedException("Metal indexed primitives require command encoder context");
        }
        
        // Resource creation
        public ITexture2D CreateTexture2D(int width, int height, bool mipMap, SurfaceFormat format)
        {
            var descriptor = new MTLTextureDescriptor
            {
                TextureType = MTLTextureType.Type2D,
                PixelFormat = ConvertToMetalFormat(format),
                Width = (nuint)width,
                Height = (nuint)height,
                MipmapLevelCount = mipMap ? (nuint)Math.Floor(Math.Log2(Math.Max(width, height))) + 1 : 1,
                Usage = MTLTextureUsage.ShaderRead
            };
            
            var texture = _device.CreateTexture(descriptor);
            return new MetalTexture2D(texture, descriptor);
        }
        
        public IRenderTarget2D CreateRenderTarget2D(int width, int height, bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage)
        {
            var descriptor = new MTLTextureDescriptor
            {
                TextureType = MTLTextureType.Type2D,
                PixelFormat = ConvertToMetalFormat(preferredFormat),
                Width = (nuint)width,
                Height = (nuint)height,
                MipmapLevelCount = mipMap ? (nuint)Math.Floor(Math.Log2(Math.Max(width, height))) + 1 : 1,
                Usage = MTLTextureUsage.RenderTarget | MTLTextureUsage.ShaderRead
            };
            
            var texture = _device.CreateTexture(descriptor);
            return new MetalRenderTarget2D(texture, descriptor, usage, preferredDepthFormat);
        }
        
        private MTLPixelFormat ConvertToMetalFormat(SurfaceFormat format)
        {
            switch (format)
            {
                case SurfaceFormat.Color: return MTLPixelFormat.RGBA8Unorm;
                case SurfaceFormat.Bgr565: return MTLPixelFormat.B5G6R5Unorm;
                case SurfaceFormat.Bgra5551: return MTLPixelFormat.BGR5A1Unorm;
                case SurfaceFormat.Bgra4444: return MTLPixelFormat.ABGR4Unorm;
                case SurfaceFormat.Alpha8: return MTLPixelFormat.A8Unorm;
                default: return MTLPixelFormat.RGBA8Unorm;
            }
        }
        
        public void Dispose()
        {
            if (!IsDisposed)
            {
                Disposing?.Invoke(this, EventArgs.Empty);
                IsDisposed = true;
            }
        }
    }
    
    /// <summary>
    /// Metal implementation of ISpriteBatch
    /// </summary>
    public class MetalSpriteBatch : ISpriteBatch
    {
        private readonly MetalRenderer _renderer;
        private bool _isInBeginEndPair;
        
        public MetalSpriteBatch(MetalRenderer renderer)
        {
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        }
        
        public void Begin()
        {
            Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }
        
        public void Begin(SpriteSortMode sortMode, BlendState blendState)
        {
            Begin(sortMode, blendState, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
        }
        
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState)
        {
            Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, null);
        }
        
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect)
        {
            Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, Matrix.Identity);
        }
        
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix transformMatrix)
        {
            if (_isInBeginEndPair)
                throw new InvalidOperationException("Begin cannot be called again until End has been successfully called.");
                
            _isInBeginEndPair = true;
            
            // Configure Metal rendering state based on parameters
            _renderer.SetBlendState(blendState);
            // Note: Other states would be configured here in a full implementation
        }
        
        public void Draw(ITexture2D texture, Rectangle destinationRectangle, Color color)
        {
            Draw(texture, destinationRectangle, null, color);
        }
        
        public void Draw(ITexture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            if (!_isInBeginEndPair)
                throw new InvalidOperationException("Draw was called, but Begin has not yet been called.");
                
            _renderer.DrawTexture(texture, new Vector2(destinationRectangle.X, destinationRectangle.Y), sourceRectangle, color);
        }
        
        public void Draw(ITexture2D texture, Vector2 position, Color color)
        {
            Draw(texture, position, null, color);
        }
        
        public void Draw(ITexture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            if (!_isInBeginEndPair)
                throw new InvalidOperationException("Draw was called, but Begin has not yet been called.");
                
            _renderer.DrawTexture(texture, position, sourceRectangle, color);
        }
        
        public void Draw(ITexture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            Draw(texture, position, sourceRectangle, color, rotation, origin, new Vector2(scale), effects, layerDepth);
        }
        
        public void Draw(ITexture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            if (!_isInBeginEndPair)
                throw new InvalidOperationException("Draw was called, but Begin has not yet been called.");
                
            // For Metal implementation, complex transforms would require matrix calculations
            // This is a simplified version that doesn't handle rotation, origin, scale, or effects
            _renderer.DrawTexture(texture, position, sourceRectangle, color);
        }
        
        public void Draw(ITexture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        {
            if (!_isInBeginEndPair)
                throw new InvalidOperationException("Draw was called, but Begin has not yet been called.");
                
            // Simplified implementation
            _renderer.DrawTexture(texture, new Vector2(destinationRectangle.X, destinationRectangle.Y), sourceRectangle, color);
        }
        
        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
        {
            // Metal text rendering would require glyph atlases and additional complexity
            throw new NotImplementedException("Metal text rendering requires glyph atlas implementation");
        }
        
        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            throw new NotImplementedException("Metal text rendering requires glyph atlas implementation");
        }
        
        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            throw new NotImplementedException("Metal text rendering requires glyph atlas implementation");
        }
        
        public void End()
        {
            if (!_isInBeginEndPair)
                throw new InvalidOperationException("End was called, but Begin has not yet been called.");
                
            _isInBeginEndPair = false;
            
            // Flush any pending draw calls
            // In Metal, this might involve submitting the current command buffer
        }
        
        public void Dispose()
        {
            // Metal SpriteBatch cleanup
        }
    }
}
#endif // METAL && (IOS || MACOS)
```

---

## Phase 4: Platform Integration (2-3 weeks)

### Task 4.1: Project Configuration (Week 9)

**Step 1**: Update iOS project file `cocos2d.Core.iOS.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0-ios</TargetFramework>
    <DefineConstants>$(DefineConstants);IOS</DefineConstants>
    <!-- Add Metal support conditionally -->
    <DefineConstants Condition="'$(EnableMetal)' == 'true'">$(DefineConstants);METAL</DefineConstants>
  </PropertyGroup>

  <!-- Metal-specific references -->
  <ItemGroup Condition="'$(EnableMetal)' == 'true'">
    <PackageReference Include="Microsoft.iOS.Ref" Version="[latest]" />
  </ItemGroup>

  <!-- Conditional compilation includes -->
  <ItemGroup Condition="'$(EnableMetal)' == 'true'">
    <Compile Include="../platform/MetalRenderer.cs" />
    <Compile Include="../platform/MetalTexture2D.cs" />
  </ItemGroup>

  <!-- Always include abstraction layer -->
  <ItemGroup>
    <Compile Include="../platform/CCGraphicsInterfaces.cs" />
    <Compile Include="../platform/CCGraphicsFactory.cs" />
    <Compile Include="../platform/MonoGameRenderer.cs" />
    <Compile Include="../platform/CCBlendFuncExtensions.cs" />
  </ItemGroup>

  <ItemGroup>
    <Reference Include="../cocos2d.projitems" />
  </ItemGroup>
</Project>
```

**Step 2**: Update macOS project file `cocos2d.Core.macOS.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0-macos</TargetFramework>
    <DefineConstants>$(DefineConstants);MACOS</DefineConstants>
    <!-- Add Metal support conditionally -->
    <DefineConstants Condition="'$(EnableMetal)' == 'true'">$(DefineConstants);METAL</DefineConstants>
  </PropertyGroup>

  <!-- Metal-specific references -->
  <ItemGroup Condition="'$(EnableMetal)' == 'true'">
    <PackageReference Include="Microsoft.macOS.Ref" Version="[latest]" />
  </ItemGroup>

  <!-- Same conditional compilation as iOS -->
  <ItemGroup Condition="'$(EnableMetal)' == 'true'">
    <Compile Include="../platform/MetalRenderer.cs" />
    <Compile Include="../platform/MetalTexture2D.cs" />
  </ItemGroup>

  <ItemGroup>
    <Compile Include="../platform/CCGraphicsInterfaces.cs" />
    <Compile Include="../platform/CCGraphicsFactory.cs" />
    <Compile Include="../platform/MonoGameRenderer.cs" />
    <Compile Include="../platform/CCBlendFuncExtensions.cs" />
  </ItemGroup>

  <ItemGroup>
    <Reference Include="../cocos2d.projitems" />
  </ItemGroup>
</Project>
```

**Step 3**: Create configuration file `Directory.Build.props` in the root:

```xml
<Project>
  <PropertyGroup>
    <!-- Global Metal configuration -->
    <EnableMetal Condition="'$(EnableMetal)' == ''">true</EnableMetal>
    <WarningsAsErrors />
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
  </PropertyGroup>

  <!-- Platform-specific Metal support -->
  <PropertyGroup Condition="'$(TargetFramework)' == 'net8.0-ios' OR '$(TargetFramework)' == 'net8.0-macos'">
    <EnableMetal Condition="'$(EnableMetal)' == ''">true</EnableMetal>
  </PropertyGroup>

  <!-- Disable Metal for other platforms -->
  <PropertyGroup Condition="'$(TargetFramework)' != 'net8.0-ios' AND '$(TargetFramework)' != 'net8.0-macos'">
    <EnableMetal>false</EnableMetal>
  </PropertyGroup>
</Project>
```

### Task 4.2: Runtime Selection (Week 9)

**Location**: Create new file `/cocos2d/platform/CCGraphicsConfig.cs`

```csharp
using System;
using System.IO;
using System.Text.Json;

namespace Cocos2D
{
    /// <summary>
    /// Configuration for graphics backend selection and optimization
    /// </summary>
    public class CCGraphicsConfig
    {
        private static CCGraphicsConfig _instance;
        private static readonly object _lock = new object();
        
        public static CCGraphicsConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = LoadConfig();
                        }
                    }
                }
                return _instance;
            }
        }
        
        // Configuration properties
        public CCGraphicsFactory.GraphicsBackend PreferredBackend { get; set; } = CCGraphicsFactory.GraphicsBackend.Auto;
        public bool EnableMetalValidation { get; set; } = false;
        public bool EnableDebugMode { get; set; } = false;
        public bool PreferIntegratedGPU { get; set; } = false;
        public int MaxTextureSize { get; set; } = 4096;
        public bool EnableAnisotropicFiltering { get; set; } = true;
        public int AnisotropyLevel { get; set; } = 4;
        public bool EnableVSync { get; set; } = true;
        
        // Performance settings
        public int MaxQuadsPerBatch { get; set; } = 10922;
        public bool EnableGPUProfiling { get; set; } = false;
        public bool OptimizeForBattery { get; set; } = false;
        
        // Metal-specific settings
        public bool MetalUseSharedCommandQueue { get; set; } = true;
        public bool MetalEnableResourceHeaps { get; set; } = true;
        public int MetalMaxBuffersInFlight { get; set; } = 3;
        
        private static CCGraphicsConfig LoadConfig()
        {
            var config = new CCGraphicsConfig();
            
            try
            {
                // Try to load from file first
                string configPath = GetConfigPath();
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    var loadedConfig = JsonSerializer.Deserialize<CCGraphicsConfig>(json);
                    if (loadedConfig != null)
                    {
                        config = loadedConfig;
                    }
                }
                
                // Apply environment variable overrides
                ApplyEnvironmentOverrides(config);
                
                // Apply platform-specific defaults
                ApplyPlatformDefaults(config);
            }
            catch (Exception ex)
            {
                CCLog.Log($"Failed to load graphics config: {ex.Message}. Using defaults.");
            }
            
            return config;
        }
        
        private static string GetConfigPath()
        {
            // Look for config in multiple locations
            string[] searchPaths = {
                "cocos2d-graphics.json",
                "config/cocos2d-graphics.json",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "cocos2d-mono", "graphics.json")
            };
            
            foreach (string path in searchPaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }
            
            return searchPaths[0]; // Default location for saving
        }
        
        private static void ApplyEnvironmentOverrides(CCGraphicsConfig config)
        {
            // Allow environment variables to override settings
            if (Enum.TryParse<CCGraphicsFactory.GraphicsBackend>(Environment.GetEnvironmentVariable("COCOS2D_GRAPHICS_BACKEND"), out var backend))
            {
                config.PreferredBackend = backend;
            }
            
            if (bool.TryParse(Environment.GetEnvironmentVariable("COCOS2D_METAL_VALIDATION"), out bool validation))
            {
                config.EnableMetalValidation = validation;
            }
            
            if (bool.TryParse(Environment.GetEnvironmentVariable("COCOS2D_DEBUG_GRAPHICS"), out bool debug))
            {
                config.EnableDebugMode = debug;
            }
            
            if (bool.TryParse(Environment.GetEnvironmentVariable("COCOS2D_OPTIMIZE_BATTERY"), out bool battery))
            {
                config.OptimizeForBattery = battery;
            }
        }
        
        private static void ApplyPlatformDefaults(CCGraphicsConfig config)
        {
#if IOS
            // iOS-specific defaults
            config.OptimizeForBattery = true;
            config.MaxTextureSize = 2048; // Older devices
            config.AnisotropyLevel = 2;   // Conserve GPU bandwidth
#elif MACOS
            // macOS-specific defaults
            config.OptimizeForBattery = false;
            config.MaxTextureSize = 8192;
            config.AnisotropyLevel = 8;
#elif ANDROID
            // Android-specific defaults
            config.OptimizeForBattery = true;
            config.MaxTextureSize = 2048;
            config.AnisotropyLevel = 2;
#else
            // Desktop defaults
            config.OptimizeForBattery = false;
            config.MaxTextureSize = 8192;
            config.AnisotropyLevel = 16;
#endif
        }
        
        public void Save()
        {
            try
            {
                string configPath = GetConfigPath();
                string directory = Path.GetDirectoryName(configPath);
                
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                
                string json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(configPath, json);
                
                CCLog.Log($"Graphics configuration saved to: {configPath}");
            }
            catch (Exception ex)
            {
                CCLog.Log($"Failed to save graphics config: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Validates the current configuration and logs any issues
        /// </summary>
        public void Validate()
        {
            // Check for invalid combinations
            if (PreferredBackend == CCGraphicsFactory.GraphicsBackend.Metal)
            {
#if !METAL || (!IOS && !MACOS)
                CCLog.Log("Metal backend requested but not available on this platform. Will fallback to MonoGame.");
#endif
            }
            
            // Validate texture sizes
            if (MaxTextureSize < 512 || MaxTextureSize > 16384)
            {
                CCLog.Log($"MaxTextureSize {MaxTextureSize} is outside recommended range (512-16384). Performance may be affected.");
            }
            
            // Validate anisotropy level
            if (AnisotropyLevel < 1 || AnisotropyLevel > 16)
            {
                CCLog.Log($"AnisotropyLevel {AnisotropyLevel} is outside valid range (1-16). Will be clamped.");
                AnisotropyLevel = Math.Max(1, Math.Min(16, AnisotropyLevel));
            }
            
            // Performance warnings
            if (OptimizeForBattery && EnableGPUProfiling)
            {
                CCLog.Log("GPU profiling enabled while battery optimization is on. This may increase power consumption.");
            }
        }
        
        /// <summary>
        /// Creates a configuration optimized for development/debugging
        /// </summary>
        public static CCGraphicsConfig CreateDebugConfig()
        {
            return new CCGraphicsConfig
            {
                PreferredBackend = CCGraphicsFactory.GraphicsBackend.Auto,
                EnableDebugMode = true,
                EnableMetalValidation = true,
                EnableGPUProfiling = true,
                OptimizeForBattery = false,
                EnableVSync = false, // Better for testing frame rates
                MaxQuadsPerBatch = 1000 // Smaller batches for debugging
            };
        }
        
        /// <summary>
        /// Creates a configuration optimized for release/performance
        /// </summary>
        public static CCGraphicsConfig CreateReleaseConfig()
        {
            return new CCGraphicsConfig
            {
                PreferredBackend = CCGraphicsFactory.GraphicsBackend.Auto,
                EnableDebugMode = false,
                EnableMetalValidation = false,
                EnableGPUProfiling = false,
                OptimizeForBattery = true,
                EnableVSync = true,
                MaxQuadsPerBatch = 10922 // Maximum performance
            };
        }
    }
}
```

---

## Phase 5: Testing Framework (3-4 weeks)

### Task 5.1: Visual Regression Testing (Week 10-11)

**Location**: Create new file `/Tests/VisualRegressionTests/CCVisualTestFramework.cs`

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace Cocos2D.Tests
{
    /// <summary>
    /// Framework for visual regression testing of different graphics backends
    /// </summary>
    public class CCVisualTestFramework : IDisposable
    {
        private readonly IGraphicsRenderer _renderer;
        private readonly string _testOutputPath;
        private readonly string _baselinePath;
        private readonly float _tolerance;
        
        public CCVisualTestFramework(IGraphicsRenderer renderer, string testOutputPath = "TestResults", float tolerance = 0.01f)
        {
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            _testOutputPath = testOutputPath;
            _baselinePath = Path.Combine(testOutputPath, "Baselines");
            _tolerance = tolerance;
            
            EnsureDirectoryExists(_testOutputPath);
            EnsureDirectoryExists(_baselinePath);
        }
        
        /// <summary>
        /// Captures and compares a rendered frame against a baseline
        /// </summary>
        public async Task<VisualTestResult> TestRenderingAsync(string testName, Action<IGraphicsRenderer> renderAction)
        {
            try
            {
                // Capture the current rendering
                var actualBytes = await CaptureRenderingAsync(renderAction);
                
                // Load or create baseline
                string baselinePath = Path.Combine(_baselinePath, $"{testName}.png");
                
                if (!File.Exists(baselinePath))
                {
                    // Create new baseline
                    await File.WriteAllBytesAsync(baselinePath, actualBytes);
                    return new VisualTestResult
                    {
                        TestName = testName,
                        Passed = true,
                        Message = "New baseline created",
                        DifferencePercentage = 0.0f
                    };
                }
                
                // Load baseline and compare
                var baselineBytes = await File.ReadAllBytesAsync(baselinePath);
                var comparison = await CompareImagesAsync(actualBytes, baselineBytes);
                
                var result = new VisualTestResult
                {
                    TestName = testName,
                    Passed = comparison.DifferencePercentage <= _tolerance,
                    Message = comparison.Message,
                    DifferencePercentage = comparison.DifferencePercentage
                };
                
                // Save actual image if test failed
                if (!result.Passed)
                {
                    string failedPath = Path.Combine(_testOutputPath, "Failed");
                    EnsureDirectoryExists(failedPath);
                    
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    await File.WriteAllBytesAsync(Path.Combine(failedPath, $"{testName}_{timestamp}_actual.png"), actualBytes);
                    await File.WriteAllBytesAsync(Path.Combine(failedPath, $"{testName}_{timestamp}_baseline.png"), baselineBytes);
                    
                    if (comparison.DifferenceImage != null)
                    {
                        await File.WriteAllBytesAsync(Path.Combine(failedPath, $"{testName}_{timestamp}_diff.png"), comparison.DifferenceImage);
                    }
                }
                
                return result;
            }
            catch (Exception ex)
            {
                return new VisualTestResult
                {
                    TestName = testName,
                    Passed = false,
                    Message = $"Test execution failed: {ex.Message}",
                    DifferencePercentage = 1.0f
                };
            }
        }
        
        /// <summary>
        /// Captures rendering output as PNG bytes
        /// </summary>
        private async Task<byte[]> CaptureRenderingAsync(Action<IGraphicsRenderer> renderAction)
        {
            const int captureWidth = 800;
            const int captureHeight = 600;
            
            // Create render target for capture
            var renderTarget = _renderer.CreateRenderTarget(captureWidth, captureHeight, SurfaceFormat.Color, DepthFormat.None, RenderTargetUsage.DiscardContents);
            
            try
            {
                // Set render target and clear
                _renderer.SetRenderTarget(renderTarget);
                _renderer.Clear(Microsoft.Xna.Framework.Color.Transparent);
                
                // Execute rendering
                _renderer.BeginDraw();
                renderAction(_renderer);
                _renderer.EndDraw();
                
                // Capture pixel data
                var pixelData = new byte[captureWidth * captureHeight * 4];
                renderTarget.GetData(pixelData);
                
                // Convert to PNG
                return await ConvertToPngAsync(pixelData, captureWidth, captureHeight);
            }
            finally
            {
                _renderer.SetRenderTarget(null);
                renderTarget.Dispose();
            }
        }
        
        /// <summary>
        /// Converts RGBA pixel data to PNG bytes
        /// </summary>
        private async Task<byte[]> ConvertToPngAsync(byte[] pixelData, int width, int height)
        {
            using var image = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(pixelData, width, height);
            using var stream = new MemoryStream();
            await image.SaveAsPngAsync(stream);
            return stream.ToArray();
        }
        
        /// <summary>
        /// Compares two images and returns difference metrics
        /// </summary>
        private async Task<ImageComparisonResult> CompareImagesAsync(byte[] actualBytes, byte[] expectedBytes)
        {
            using var actualImage = SixLabors.ImageSharp.Image.Load<Rgba32>(actualBytes);
            using var expectedImage = SixLabors.ImageSharp.Image.Load<Rgba32>(expectedBytes);
            
            if (actualImage.Width != expectedImage.Width || actualImage.Height != expectedImage.Height)
            {
                return new ImageComparisonResult
                {
                    DifferencePercentage = 1.0f,
                    Message = $"Image dimensions differ: actual {actualImage.Width}x{actualImage.Height}, expected {expectedImage.Width}x{expectedImage.Height}"
                };
            }
            
            float totalDifference = 0;
            int totalPixels = actualImage.Width * actualImage.Height;
            using var diffImage = new SixLabors.ImageSharp.Image<Rgba32>(actualImage.Width, actualImage.Height);
            
            for (int y = 0; y < actualImage.Height; y++)
            {
                for (int x = 0; x < actualImage.Width; x++)
                {
                    var actualPixel = actualImage[x, y];
                    var expectedPixel = expectedImage[x, y];
                    
                    var pixelDiff = CalculatePixelDifference(actualPixel, expectedPixel);
                    totalDifference += pixelDiff;
                    
                    // Create difference visualization
                    if (pixelDiff > 0.01f) // Highlight significant differences
                    {
                        diffImage[x, y] = new Rgba32(255, 0, 0, (byte)(pixelDiff * 255));
                    }
                    else
                    {
                        diffImage[x, y] = new Rgba32(0, 0, 0, 0);
                    }
                }
            }
            
            float averageDifference = totalDifference / totalPixels;
            
            byte[] diffImageBytes = null;
            if (averageDifference > _tolerance)
            {
                using var diffStream = new MemoryStream();
                await diffImage.SaveAsPngAsync(diffStream);
                diffImageBytes = diffStream.ToArray();
            }
            
            return new ImageComparisonResult
            {
                DifferencePercentage = averageDifference,
                Message = averageDifference <= _tolerance ? "Images match within tolerance" : $"Images differ by {averageDifference:P2}",
                DifferenceImage = diffImageBytes
            };
        }
        
        /// <summary>
        /// Calculates the difference between two pixels (0.0 = identical, 1.0 = completely different)
        /// </summary>
        private float CalculatePixelDifference(Rgba32 actual, Rgba32 expected)
        {
            float rDiff = Math.Abs(actual.R - expected.R) / 255.0f;
            float gDiff = Math.Abs(actual.G - expected.G) / 255.0f;
            float bDiff = Math.Abs(actual.B - expected.B) / 255.0f;
            float aDiff = Math.Abs(actual.A - expected.A) / 255.0f;
            
            return (rDiff + gDiff + bDiff + aDiff) / 4.0f;
        }
        
        private void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        
        public void Dispose()
        {
            // Cleanup resources if needed
        }
    }
    
    /// <summary>
    /// Result of a visual regression test
    /// </summary>
    public class VisualTestResult
    {
        public string TestName { get; set; }
        public bool Passed { get; set; }
        public string Message { get; set; }
        public float DifferencePercentage { get; set; }
    }
    
    /// <summary>
    /// Result of comparing two images
    /// </summary>
    public class ImageComparisonResult
    {
        public float DifferencePercentage { get; set; }
        public string Message { get; set; }
        public byte[] DifferenceImage { get; set; }
    }
    
    /// <summary>
    /// Base class for visual regression tests
    /// </summary>
    public abstract class VisualTestBase : IDisposable
    {
        protected readonly CCVisualTestFramework Framework;
        protected readonly IGraphicsRenderer Renderer;
        
        protected VisualTestBase()
        {
            Renderer = CCGraphicsFactory.CreateRenderer();
            Framework = new CCVisualTestFramework(Renderer);
        }
        
        protected async Task VerifyRenderingAsync(string testName, Action<IGraphicsRenderer> renderAction)
        {
            var result = await Framework.TestRenderingAsync(testName, renderAction);
            
            Assert.True(result.Passed, $"Visual test '{testName}' failed: {result.Message} (Difference: {result.DifferencePercentage:P2})");
        }
        
        public virtual void Dispose()
        {
            Framework?.Dispose();
            Renderer?.Dispose();
        }
    }
}
```

### Task 5.2: Performance Testing (Week 11-12)

**Location**: Create new file `/Tests/PerformanceTests/CCPerformanceTestFramework.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Xunit;

namespace Cocos2D.Tests
{
    /// <summary>
    /// Framework for performance testing and benchmarking different graphics backends
    /// </summary>
    public class CCPerformanceTestFramework
    {
        private readonly IGraphicsRenderer _renderer;
        
        public CCPerformanceTestFramework(IGraphicsRenderer renderer)
        {
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        }
        
        /// <summary>
        /// Benchmarks a rendering operation and returns performance metrics
        /// </summary>
        public async Task<PerformanceResult> BenchmarkAsync(string testName, Action<IGraphicsRenderer> renderAction, BenchmarkConfig config = null)
        {
            config ??= BenchmarkConfig.Default;
            
            var results = new List<double>();
            var memoryResults = new List<long>();
            
            // Warmup phase
            for (int i = 0; i < config.WarmupIterations; i++)
            {
                renderAction(_renderer);
            }
            
            // Force garbage collection before measurement
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            var stopwatch = new Stopwatch();
            
            // Measurement phase
            for (int i = 0; i < config.MeasurementIterations; i++)
            {
                var memoryBefore = GC.GetTotalMemory(false);
                
                stopwatch.Restart();
                renderAction(_renderer);
                stopwatch.Stop();
                
                var memoryAfter = GC.GetTotalMemory(false);
                
                results.Add(stopwatch.Elapsed.TotalMilliseconds);
                memoryResults.Add(memoryAfter - memoryBefore);
                
                // Small delay to prevent overheating in intensive tests
                if (config.DelayBetweenIterations > TimeSpan.Zero)
                {
                    await Task.Delay(config.DelayBetweenIterations);
                }
            }
            
            return new PerformanceResult
            {
                TestName = testName,
                RendererType = _renderer.GetType().Name,
                AverageTimeMs = results.Average(),
                MinTimeMs = results.Min(),
                MaxTimeMs = results.Max(),
                StandardDeviationMs = CalculateStandardDeviation(results),
                MedianTimeMs = CalculateMedian(results),
                IterationsPerSecond = 1000.0 / results.Average(),
                AverageMemoryAllocatedBytes = memoryResults.Average(),
                TotalMemoryAllocatedBytes = memoryResults.Sum(),
                Iterations = config.MeasurementIterations
            };
        }
        
        /// <summary>
        /// Compares performance between different renderers
        /// </summary>
        public async Task<ComparisonResult> CompareRenderersAsync(string testName, Action<IGraphicsRenderer> renderAction, params IGraphicsRenderer[] renderers)
        {
            var results = new List<PerformanceResult>();
            
            foreach (var renderer in renderers)
            {
                var framework = new CCPerformanceTestFramework(renderer);
                var result = await framework.BenchmarkAsync($"{testName}_{renderer.GetType().Name}", renderAction);
                results.Add(result);
            }
            
            return new ComparisonResult
            {
                TestName = testName,
                Results = results.ToArray(),
                FastestRenderer = results.OrderBy(r => r.AverageTimeMs).First().RendererType,
                SlowestRenderer = results.OrderByDescending(r => r.AverageTimeMs).First().RendererType,
                PerformanceSpread = results.Max(r => r.AverageTimeMs) / results.Min(r => r.AverageTimeMs)
            };
        }
        
        /// <summary>
        /// Benchmarks sprite rendering with varying batch sizes
        /// </summary>
        public async Task<BatchingBenchmarkResult> BenchmarkSpriteBatchingAsync(ITexture2D texture, int[] batchSizes)
        {
            var results = new List<(int BatchSize, PerformanceResult Performance)>();
            
            foreach (int batchSize in batchSizes)
            {
                var renderAction = new Action<IGraphicsRenderer>(renderer =>
                {
                    renderer.BeginDraw();
                    
                    for (int i = 0; i < batchSize; i++)
                    {
                        var position = new Vector2(
                            (i % 10) * 64,  // 10 sprites per row
                            (i / 10) * 64
                        );
                        renderer.DrawTexture(texture, position, null, Microsoft.Xna.Framework.Color.White);
                    }
                    
                    renderer.EndDraw();
                });
                
                var result = await BenchmarkAsync($"SpriteBatch_{batchSize}", renderAction);
                results.Add((batchSize, result));
            }
            
            return new BatchingBenchmarkResult
            {
                TestName = "SpriteBatching",
                RendererType = _renderer.GetType().Name,
                Results = results.ToArray(),
                OptimalBatchSize = results.OrderBy(r => r.Performance.AverageTimeMs / r.BatchSize).First().BatchSize
            };
        }
        
        /// <summary>
        /// Stress tests the renderer with increasing load until performance degrades
        /// </summary>
        public async Task<StressTestResult> StressTestAsync(Func<int, Action<IGraphicsRenderer>> renderActionFactory, int startLoad = 100, int maxLoad = 10000, int loadIncrement = 100)
        {
            var results = new List<(int Load, PerformanceResult Performance)>();
            double baselinePerformance = 0;
            
            for (int load = startLoad; load <= maxLoad; load += loadIncrement)
            {
                var renderAction = renderActionFactory(load);
                var result = await BenchmarkAsync($"StressTest_{load}", renderAction, new BenchmarkConfig
                {
                    WarmupIterations = 5,
                    MeasurementIterations = 10
                });
                
                results.Add((load, result));
                
                if (baselinePerformance == 0)
                {
                    baselinePerformance = result.AverageTimeMs;
                }
                
                // Stop if performance has degraded significantly
                if (result.AverageTimeMs > baselinePerformance * 5) // 5x slower than baseline
                {
                    break;
                }
            }
            
            return new StressTestResult
            {
                TestName = "StressTest",
                RendererType = _renderer.GetType().Name,
                Results = results.ToArray(),
                MaxSustainableLoad = results.LastOrDefault().Load,
                PerformanceDegradationThreshold = baselinePerformance * 2
            };
        }
        
        private double CalculateStandardDeviation(IEnumerable<double> values)
        {
            double avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }
        
        private double CalculateMedian(List<double> values)
        {
            values.Sort();
            int count = values.Count;
            
            if (count % 2 == 0)
            {
                return (values[count / 2 - 1] + values[count / 2]) / 2.0;
            }
            else
            {
                return values[count / 2];
            }
        }
    }
    
    /// <summary>
    /// Configuration for benchmark tests
    /// </summary>
    public class BenchmarkConfig
    {
        public int WarmupIterations { get; set; } = 10;
        public int MeasurementIterations { get; set; } = 100;
        public TimeSpan DelayBetweenIterations { get; set; } = TimeSpan.Zero;
        
        public static BenchmarkConfig Default => new BenchmarkConfig();
        
        public static BenchmarkConfig Quick => new BenchmarkConfig
        {
            WarmupIterations = 3,
            MeasurementIterations = 20
        };
        
        public static BenchmarkConfig Thorough => new BenchmarkConfig
        {
            WarmupIterations = 20,
            MeasurementIterations = 500,
            DelayBetweenIterations = TimeSpan.FromMilliseconds(1)
        };
    }
    
    /// <summary>
    /// Result of a performance benchmark
    /// </summary>
    public class PerformanceResult
    {
        public string TestName { get; set; }
        public string RendererType { get; set; }
        public double AverageTimeMs { get; set; }
        public double MinTimeMs { get; set; }
        public double MaxTimeMs { get; set; }
        public double MedianTimeMs { get; set; }
        public double StandardDeviationMs { get; set; }
        public double IterationsPerSecond { get; set; }
        public double AverageMemoryAllocatedBytes { get; set; }
        public long TotalMemoryAllocatedBytes { get; set; }
        public int Iterations { get; set; }
        
        public override string ToString()
        {
            return $"{TestName} ({RendererType}): {AverageTimeMs:F2}ms avg, {IterationsPerSecond:F0} fps, {AverageMemoryAllocatedBytes:F0}B alloc";
        }
    }
    
    /// <summary>
    /// Result of comparing multiple renderers
    /// </summary>
    public class ComparisonResult
    {
        public string TestName { get; set; }
        public PerformanceResult[] Results { get; set; }
        public string FastestRenderer { get; set; }
        public string SlowestRenderer { get; set; }
        public double PerformanceSpread { get; set; } // Ratio of slowest to fastest
    }
    
    /// <summary>
    /// Result of sprite batching benchmark
    /// </summary>
    public class BatchingBenchmarkResult
    {
        public string TestName { get; set; }
        public string RendererType { get; set; }
        public (int BatchSize, PerformanceResult Performance)[] Results { get; set; }
        public int OptimalBatchSize { get; set; }
    }
    
    /// <summary>
    /// Result of stress testing
    /// </summary>
    public class StressTestResult
    {
        public string TestName { get; set; }
        public string RendererType { get; set; }
        public (int Load, PerformanceResult Performance)[] Results { get; set; }
        public int MaxSustainableLoad { get; set; }
        public double PerformanceDegradationThreshold { get; set; }
    }
}
```

---

## Implementation Notes

### Testing Strategy

1. **Unit Tests**: Test each abstraction layer component independently
2. **Integration Tests**: Test renderer interchangeability
3. **Visual Regression Tests**: Ensure rendering output consistency
4. **Performance Tests**: Benchmark and compare renderer performance
5. **Platform Tests**: Test on actual iOS/macOS/Windows devices

### Migration Strategy

1. **Phase 1**: Implement abstraction layer alongside existing code
2. **Phase 2**: Gradually migrate core classes to use abstraction
3. **Phase 3**: Add Metal backend with feature parity
4. **Phase 4**: Optimize and add Metal-specific features
5. **Phase 5**: Comprehensive testing and validation

### Error Handling

- Graceful fallback from Metal to MonoGame if Metal initialization fails
- Comprehensive logging for debugging backend selection and performance
- Clear error messages for common configuration issues
- Validation of graphics capabilities before attempting advanced features

### Performance Considerations

- Minimize abstraction overhead through careful interface design
- Use compilation directives to avoid Metal code on non-Apple platforms
- Implement efficient batching for both MonoGame and Metal renderers
- Profile memory allocation patterns to prevent garbage collection spikes

### Documentation Requirements

- API documentation for all new interfaces and classes
- Migration guide for existing cocos2d-mono projects
- Platform-specific setup instructions for Metal backend
- Performance tuning guide for optimal configuration

This detailed implementation guide provides step-by-step instructions, complete code examples, and comprehensive testing strategies to efficiently implement the Metal abstraction layer while maintaining backward compatibility with existing cocos2d-mono projects.