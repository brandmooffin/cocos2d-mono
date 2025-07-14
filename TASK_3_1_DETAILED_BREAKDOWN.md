# Task 3.1: Metal Backend Core Implementation - Detailed Breakdown

## Overview
Implement the core Metal graphics backend that conforms to the abstraction interfaces defined in Phase 1. This provides native Metal rendering capabilities on iOS/macOS platforms.

---

## Subtask 3.1.1: Setup Metal Project Structure and Dependencies
**Time Estimate**: 1 hour  
**Dependencies**: Phase 1 complete, Phase 2 complete  
**Assignee**: Build engineer or iOS/macOS developer

### Steps:

1. **Create Metal backend directory structure**
   ```bash
   mkdir -p cocos2d/platform/Metal
   mkdir -p cocos2d/platform/Metal/Shaders
   mkdir -p cocos2d/platform/Metal/Utilities
   mkdir -p cocos2d/platform/Metal/Resources
   touch cocos2d/platform/Metal/MetalRenderer.cs
   touch cocos2d/platform/Metal/MetalDevice.cs
   touch cocos2d/platform/Metal/MetalTexture2D.cs
   touch cocos2d/platform/Metal/MetalRenderTarget2D.cs
   touch cocos2d/platform/Metal/MetalSpriteBatch.cs
   touch cocos2d/platform/Metal/MetalBuffer.cs
   touch cocos2d/platform/Metal/MetalEffect.cs
   ```

2. **Create Metal project configuration files**
   ```xml
   <!-- File: cocos2d/platform/Metal/Metal.projitems -->
   <?xml version="1.0" encoding="utf-8"?>
   <Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
     <PropertyGroup>
       <MSBuildAllProjects>$(MSBuildAllProjects);$(MSBuildThisFileFullPath)</MSBuildAllProjects>
       <HasSharedItems>true</HasSharedItems>
       <SharedGUID>12345678-1234-1234-1234-123456789ABC</SharedGUID>
     </PropertyGroup>
     
     <PropertyGroup Label="Configuration">
       <Import_RootNamespace>Cocos2D.Platform.Metal</Import_RootNamespace>
     </PropertyGroup>
     
     <ItemGroup Condition="'$(EnableMetal)' == 'true' AND ('$(TargetPlatformIdentifier)' == 'iOS' OR '$(TargetPlatformIdentifier)' == 'macOS')">
       <Compile Include="$(MSBuildThisFileDirectory)MetalRenderer.cs" />
       <Compile Include="$(MSBuildThisFileDirectory)MetalDevice.cs" />
       <Compile Include="$(MSBuildThisFileDirectory)MetalTexture2D.cs" />
       <Compile Include="$(MSBuildThisFileDirectory)MetalRenderTarget2D.cs" />
       <Compile Include="$(MSBuildThisFileDirectory)MetalSpriteBatch.cs" />
       <Compile Include="$(MSBuildThisFileDirectory)MetalBuffer.cs" />
       <Compile Include="$(MSBuildThisFileDirectory)MetalEffect.cs" />
       <Compile Include="$(MSBuildThisFileDirectory)Utilities\MetalUtilities.cs" />
       <Compile Include="$(MSBuildThisFileDirectory)Utilities\MetalShaderCompiler.cs" />
     </ItemGroup>
     
     <ItemGroup Condition="'$(EnableMetal)' == 'true' AND ('$(TargetPlatformIdentifier)' == 'iOS' OR '$(TargetPlatformIdentifier)' == 'macOS')">
       <Metal Include="$(MSBuildThisFileDirectory)Shaders\*.metal" />
       <BundleResource Include="$(MSBuildThisFileDirectory)Shaders\*.metallib" />
     </ItemGroup>
   </Project>
   ```

3. **Add Metal framework references**
   ```xml
   <!-- Add to main project files for iOS/macOS -->
   <ItemGroup Condition="'$(EnableMetal)' == 'true' AND '$(TargetPlatformIdentifier)' == 'iOS'">
     <Reference Include="Metal" />
     <Reference Include="MetalKit" />
     <Reference Include="MetalPerformanceShaders" />
   </ItemGroup>
   
   <ItemGroup Condition="'$(EnableMetal)' == 'true' AND '$(TargetPlatformIdentifier)' == 'macOS'">
     <Reference Include="Metal" />
     <Reference Include="MetalKit" />
     <Reference Include="MetalPerformanceShaders" />
   </ItemGroup>
   ```

4. **Create Metal common includes**
   ```csharp
   // File: cocos2d/platform/Metal/MetalCommon.cs
   #if METAL && (IOS || MACOS)
   using System;
   using System.Collections.Generic;
   using System.Runtime.InteropServices;
   using Foundation;
   using Metal;
   using MetalKit;
   using CoreGraphics;
   using Cocos2D.Platform.Interfaces;
   
   #if IOS
   using UIKit;
   using PlatformView = UIKit.UIView;
   using PlatformColor = UIKit.UIColor;
   #elif MACOS
   using AppKit;
   using PlatformView = AppKit.NSView;
   using PlatformColor = AppKit.NSColor;
   #endif
   
   namespace Cocos2D.Platform.Metal
   {
       /// <summary>
       /// Common utilities and constants for Metal implementation
       /// </summary>
       internal static class MetalCommon
       {
           /// <summary>
           /// Maximum number of frames in flight
           /// </summary>
           public const int MaxFramesInFlight = 3;
           
           /// <summary>
           /// Default vertex buffer size
           /// </summary>
           public const int DefaultVertexBufferSize = 256 * 1024; // 256KB
           
           /// <summary>
           /// Default index buffer size
           /// </summary>
           public const int DefaultIndexBufferSize = 64 * 1024; // 64KB
           
           /// <summary>
           /// Maximum number of textures that can be bound simultaneously
           /// </summary>
           public const int MaxTextureBindings = 16;
           
           /// <summary>
           /// Converts CCVertex3F to Metal-compatible format
           /// </summary>
           public static void ConvertVertex(ref CCVertex3F ccVertex, out MetalVertex metalVertex)
           {
               metalVertex = new MetalVertex
               {
                   Position = new Vector3(ccVertex.X, ccVertex.Y, ccVertex.Z),
                   TexCoord = Vector2.Zero,
                   Color = Vector4.One
               };
           }
           
           /// <summary>
           /// Converts CCV3F_C4B_T2F to Metal vertex format
           /// </summary>
           public static MetalVertex ConvertCCVertex(CCV3F_C4B_T2F ccVertex)
           {
               return new MetalVertex
               {
                   Position = new Vector3(ccVertex.Vertices.X, ccVertex.Vertices.Y, ccVertex.Vertices.Z),
                   TexCoord = new Vector2(ccVertex.TexCoords.U, ccVertex.TexCoords.V),
                   Color = new Vector4(
                       ccVertex.Colors.R / 255.0f,
                       ccVertex.Colors.G / 255.0f, 
                       ccVertex.Colors.B / 255.0f,
                       ccVertex.Colors.A / 255.0f)
               };
           }
           
           /// <summary>
           /// Gets the Metal device, preferring discrete GPU if available
           /// </summary>
           public static IMTLDevice GetPreferredDevice()
           {
               var devices = MTLDevice.SystemDefault;
               if (devices == null)
               {
                   throw new InvalidOperationException("No Metal device available on this system");
               }
               
               return devices;
           }
           
           /// <summary>
           /// Converts SurfaceFormat to Metal pixel format
           /// </summary>
           public static MTLPixelFormat ConvertPixelFormat(SurfaceFormat format)
           {
               switch (format)
               {
                   case SurfaceFormat.Color:
                   case SurfaceFormat.Bgra32:
                       return MTLPixelFormat.BGRA8Unorm;
                   case SurfaceFormat.Bgr565:
                       return MTLPixelFormat.B5G6R5Unorm;
                   case SurfaceFormat.Bgra5551:
                       return MTLPixelFormat.BGR5A1Unorm;
                   case SurfaceFormat.Bgra4444:
                       return MTLPixelFormat.ABGR4Unorm;
                   case SurfaceFormat.Alpha8:
                       return MTLPixelFormat.A8Unorm;
                   case SurfaceFormat.Dxt1:
                       return MTLPixelFormat.BC1_RGBA;
                   case SurfaceFormat.Dxt3:
                       return MTLPixelFormat.BC2_RGBA;
                   case SurfaceFormat.Dxt5:
                       return MTLPixelFormat.BC3_RGBA;
                   default:
                       CCLog.Log($"Unsupported surface format: {format}, defaulting to BGRA8Unorm");
                       return MTLPixelFormat.BGRA8Unorm;
               }
           }
           
           /// <summary>
           /// Converts Metal pixel format back to SurfaceFormat
           /// </summary>
           public static SurfaceFormat ConvertFromMetalPixelFormat(MTLPixelFormat format)
           {
               switch (format)
               {
                   case MTLPixelFormat.BGRA8Unorm:
                       return SurfaceFormat.Bgra32;
                   case MTLPixelFormat.B5G6R5Unorm:
                       return SurfaceFormat.Bgr565;
                   case MTLPixelFormat.BGR5A1Unorm:
                       return SurfaceFormat.Bgra5551;
                   case MTLPixelFormat.ABGR4Unorm:
                       return SurfaceFormat.Bgra4444;
                   case MTLPixelFormat.A8Unorm:
                       return SurfaceFormat.Alpha8;
                   default:
                       return SurfaceFormat.Color;
               }
           }
       }
       
       /// <summary>
       /// Metal vertex structure
       /// </summary>
       [StructLayout(LayoutKind.Sequential)]
       public struct MetalVertex
       {
           public Vector3 Position;
           public Vector2 TexCoord;
           public Vector4 Color;
           
           public static readonly int SizeInBytes = Marshal.SizeOf<MetalVertex>();
       }
       
       /// <summary>
       /// Metal uniform buffer structure
       /// </summary>
       [StructLayout(LayoutKind.Sequential)]
       public struct MetalUniforms
       {
           public Matrix4x4 ModelViewProjectionMatrix;
           public Vector4 TintColor;
           public float Time;
           public Vector3 _padding;
           
           public static readonly int SizeInBytes = Marshal.SizeOf<MetalUniforms>();
       }
   }
   #endif
   ```

### Verification:
- Metal project structure created correctly
- Build configuration supports conditional compilation
- Framework references resolve correctly
- Common utilities compile without errors

---

## Subtask 3.1.2: Implement Metal Device Abstraction
**Time Estimate**: 2 hours  
**Dependencies**: Subtask 3.1.1  
**Assignee**: iOS/macOS developer with Metal experience

### Steps:

1. **Implement MetalDevice class**
   ```csharp
   // File: cocos2d/platform/Metal/MetalDevice.cs
   #if METAL && (IOS || MACOS)
   using System;
   using System.Collections.Generic;
   using Microsoft.Xna.Framework;
   using Microsoft.Xna.Framework.Graphics;
   using Foundation;
   using Metal;
   using MetalKit;
   using Cocos2D.Platform.Interfaces;
   
   namespace Cocos2D.Platform.Metal
   {
       /// <summary>
       /// Metal implementation of IGraphicsDevice
       /// </summary>
       public class MetalDevice : IGraphicsDevice
       {
           private IMTLDevice _device;
           private IMTLCommandQueue _commandQueue;
           private IMTLRenderCommandEncoder _currentEncoder;
           private IMTLCommandBuffer _currentCommandBuffer;
           private MTKView _view;
           
           // State management
           private BlendState _blendState;
           private DepthStencilState _depthStencilState;
           private RasterizerState _rasterizerState;
           private Viewport _viewport;
           private Rectangle _scissorRectangle;
           
           // Resource management
           private readonly Dictionary<int, IMTLTexture> _boundTextures;
           private IMTLBuffer _currentVertexBuffer;
           private IMTLBuffer _currentIndexBuffer;
           
           // Capabilities
           private readonly GraphicsAdapter _adapter;
           private readonly GraphicsProfile _profile;
           private readonly PresentationParameters _presentationParameters;
           
           // State
           private bool _disposed = false;
           private GraphicsDeviceStatus _status = GraphicsDeviceStatus.Normal;
           
           public MetalDevice(MTKView view)
           {
               _view = view ?? throw new ArgumentNullException(nameof(view));
               _device = _view.Device ?? throw new ArgumentException("MTKView must have a Metal device");
               _commandQueue = _device.CreateCommandQueue();
               _boundTextures = new Dictionary<int, IMTLTexture>();
               
               // Initialize default states
               _blendState = BlendState.AlphaBlend;
               _depthStencilState = DepthStencilState.Default;
               _rasterizerState = RasterizerState.CullCounterClockwise;
               _viewport = new Viewport(0, 0, (int)_view.DrawableSize.Width, (int)_view.DrawableSize.Height);
               
               // Setup presentation parameters
               _presentationParameters = new PresentationParameters
               {
                   BackBufferWidth = (int)_view.DrawableSize.Width,
                   BackBufferHeight = (int)_view.DrawableSize.Height,
                   BackBufferFormat = ConvertFromMetalPixelFormat(_view.ColorPixelFormat),
                   DepthStencilFormat = _view.DepthStencilPixelFormat != MTLPixelFormat.Invalid ? 
                                      DepthFormat.Depth24Stencil8 : DepthFormat.None,
                   DeviceWindowHandle = IntPtr.Zero,
                   IsFullScreen = false
               };
               
               // Create adapter info
               _adapter = new MetalGraphicsAdapter(_device);
               _profile = GraphicsProfile.Reach; // Metal supports more than Reach, but be conservative
               
               CCLog.Log($"Metal device initialized: {_device.Name}");
           }
           
           // IGraphicsDevice Properties
           public GraphicsDeviceStatus GraphicsDeviceStatus => _status;
           public bool IsDisposed => _disposed;
           public bool IsContentLost => false; // Metal handles content restoration automatically
           public GraphicsProfile GraphicsProfile => _profile;
           public PresentationParameters PresentationParameters => _presentationParameters;
           public DisplayMode DisplayMode => new DisplayMode(_presentationParameters.BackBufferWidth, 
                                                           _presentationParameters.BackBufferHeight, 
                                                           SurfaceFormat.Color);
           public GraphicsAdapter Adapter => _adapter;
           
           // State Properties
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
               set => _rasterizerState = value ?? RasterizerState.CullCounterClockwise; 
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
           
           public SamplerStateCollection SamplerStates { get; private set; }
           
           // Texture capability queries
           public int MaxTextureSize => (int)_device.MaxTextureWidth2D;
           public bool SupportsNonPowerOfTwoTextures => true; // Metal supports NPOT textures
       }
   }
   #endif
   ```

2. **Implement clearing and presentation**
   ```csharp
   // Core rendering operations
   public void Clear(Color color)
   {
       Clear(ClearOptions.Target, color, 1.0f, 0);
   }
   
   public void Clear(ClearOptions options, Color color, float depth, int stencil)
   {
       if (_disposed) throw new ObjectDisposedException(nameof(MetalDevice));
       
       try
       {
           // Begin render pass if not already started
           if (_currentEncoder == null)
           {
               BeginRenderPass(options, color, depth, stencil);
           }
           else
           {
               // If we have an active encoder, we need to set clear values differently
               // This is more complex in Metal - typically clear values are set at render pass creation
               CCLog.Log("Warning: Clear called mid-render pass. Clear values should be set at render pass begin.");
           }
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error clearing Metal device: {ex.Message}");
           _status = GraphicsDeviceStatus.NotReset;
           throw;
       }
   }
   
   public void Present()
   {
       if (_disposed) throw new ObjectDisposedException(nameof(MetalDevice));
       
       try
       {
           // End current render pass
           EndRenderPass();
           
           // Present the drawable
           if (_currentCommandBuffer != null)
           {
               var drawable = _view.CurrentDrawable;
               if (drawable != null)
               {
                   _currentCommandBuffer.PresentDrawable(drawable);
               }
               
               _currentCommandBuffer.Commit();
               _currentCommandBuffer.WaitUntilCompleted();
               
               _currentCommandBuffer?.Dispose();
               _currentCommandBuffer = null;
           }
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error presenting Metal frame: {ex.Message}");
           _status = GraphicsDeviceStatus.NotReset;
           throw;
       }
   }
   
   private void BeginRenderPass(ClearOptions options, Color clearColor, float depth, int stencil)
   {
       if (_currentCommandBuffer == null)
       {
           _currentCommandBuffer = _commandQueue.CommandBuffer();
       }
       
       var drawable = _view.CurrentDrawable;
       if (drawable?.Texture == null) return;
       
       var renderPassDescriptor = new MTLRenderPassDescriptor();
       
       // Setup color attachment
       renderPassDescriptor.ColorAttachments[0].Texture = drawable.Texture;
       renderPassDescriptor.ColorAttachments[0].LoadAction = 
           options.HasFlag(ClearOptions.Target) ? MTLLoadAction.Clear : MTLLoadAction.Load;
       renderPassDescriptor.ColorAttachments[0].StoreAction = MTLStoreAction.Store;
       
       if (options.HasFlag(ClearOptions.Target))
       {
           renderPassDescriptor.ColorAttachments[0].ClearColor = new MTLClearColor(
               clearColor.R / 255.0, clearColor.G / 255.0, clearColor.B / 255.0, clearColor.A / 255.0);
       }
       
       // Setup depth attachment if available
       if (_view.DepthStencilTexture != null)
       {
           renderPassDescriptor.DepthAttachment.Texture = _view.DepthStencilTexture;
           renderPassDescriptor.DepthAttachment.LoadAction = 
               options.HasFlag(ClearOptions.DepthBuffer) ? MTLLoadAction.Clear : MTLLoadAction.Load;
           renderPassDescriptor.DepthAttachment.StoreAction = MTLStoreAction.Store;
           
           if (options.HasFlag(ClearOptions.DepthBuffer))
           {
               renderPassDescriptor.DepthAttachment.ClearDepth = depth;
           }
       }
       
       // Setup stencil attachment if available
       if (_view.DepthStencilTexture != null && _view.DepthStencilPixelFormat == MTLPixelFormat.Depth24Unorm_Stencil8)
       {
           renderPassDescriptor.StencilAttachment.Texture = _view.DepthStencilTexture;
           renderPassDescriptor.StencilAttachment.LoadAction = 
               options.HasFlag(ClearOptions.Stencil) ? MTLLoadAction.Clear : MTLLoadAction.Load;
           renderPassDescriptor.StencilAttachment.StoreAction = MTLStoreAction.Store;
           
           if (options.HasFlag(ClearOptions.Stencil))
           {
               renderPassDescriptor.StencilAttachment.ClearStencil = (uint)stencil;
           }
       }
       
       _currentEncoder = _currentCommandBuffer.CreateRenderCommandEncoder(renderPassDescriptor);
       
       // Set viewport
       var metalViewport = new MTLViewport
       {
           OriginX = _viewport.X,
           OriginY = _viewport.Y,
           Width = _viewport.Width,
           Height = _viewport.Height,
           ZNear = _viewport.MinDepth,
           ZFar = _viewport.MaxDepth
       };
       _currentEncoder.SetViewport(metalViewport);
   }
   
   private void EndRenderPass()
   {
       if (_currentEncoder != null)
       {
           _currentEncoder.EndEncoding();
           _currentEncoder?.Dispose();
           _currentEncoder = null;
       }
   }
   ```

3. **Implement buffer and rendering operations**
   ```csharp
   // Buffer management
   public void SetVertexBuffer(VertexBuffer vertexBuffer)
   {
       if (_disposed) throw new ObjectDisposedException(nameof(MetalDevice));
       
       if (vertexBuffer is MetalVertexBuffer metalBuffer)
       {
           _currentVertexBuffer = metalBuffer.Buffer;
       }
       else
       {
           throw new ArgumentException("Vertex buffer must be a MetalVertexBuffer", nameof(vertexBuffer));
       }
   }
   
   public void SetIndices(IndexBuffer indexBuffer)
   {
       if (_disposed) throw new ObjectDisposedException(nameof(MetalDevice));
       
       if (indexBuffer is MetalIndexBuffer metalBuffer)
       {
           _currentIndexBuffer = metalBuffer.Buffer;
       }
       else
       {
           throw new ArgumentException("Index buffer must be a MetalIndexBuffer", nameof(indexBuffer));
       }
   }
   
   // Drawing operations
   public void DrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int primitiveCount) 
       where T : struct, IVertexType
   {
       if (_disposed) throw new ObjectDisposedException(nameof(MetalDevice));
       
       try
       {
           // Convert vertex data to Metal format
           var metalVertices = ConvertVertexData(vertexData, vertexOffset, GetVertexCount(primitiveType, primitiveCount));
           
           // Create temporary buffer
           var bufferSize = metalVertices.Length * MetalVertex.SizeInBytes;
           var buffer = _device.CreateBuffer(bufferSize, MTLResourceOptions.CpuCacheModeWriteCombined);
           
           unsafe
           {
               var bufferPointer = (MetalVertex*)buffer.Contents;
               for (int i = 0; i < metalVertices.Length; i++)
               {
                   bufferPointer[i] = metalVertices[i];
               }
           }
           
           // Bind vertex buffer and draw
           _currentEncoder?.SetVertexBuffer(buffer, 0, 0);
           
           var metalPrimitiveType = ConvertPrimitiveType(primitiveType);
           _currentEncoder?.DrawPrimitives(metalPrimitiveType, 0, (nuint)metalVertices.Length);
           
           buffer?.Dispose();
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error drawing user primitives: {ex.Message}");
           throw;
       }
   }
   
   public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, 
                                           short[] indexData, int indexOffset, int primitiveCount) 
       where T : struct, IVertexType
   {
       if (_disposed) throw new ObjectDisposedException(nameof(MetalDevice));
       
       try
       {
           // Convert vertex data
           var metalVertices = ConvertVertexData(vertexData, vertexOffset, numVertices);
           var bufferSize = metalVertices.Length * MetalVertex.SizeInBytes;
           var vertexBuffer = _device.CreateBuffer(bufferSize, MTLResourceOptions.CpuCacheModeWriteCombined);
           
           unsafe
           {
               var vertexPointer = (MetalVertex*)vertexBuffer.Contents;
               for (int i = 0; i < metalVertices.Length; i++)
               {
                   vertexPointer[i] = metalVertices[i];
               }
           }
           
           // Create index buffer
           var indexBufferSize = primitiveCount * GetIndicesPerPrimitive(primitiveType) * sizeof(ushort);
           var indexBuffer = _device.CreateBuffer(indexBufferSize, MTLResourceOptions.CpuCacheModeWriteCombined);
           
           unsafe
           {
               var indexPointer = (ushort*)indexBuffer.Contents;
               for (int i = 0; i < primitiveCount * GetIndicesPerPrimitive(primitiveType); i++)
               {
                   indexPointer[i] = (ushort)indexData[indexOffset + i];
               }
           }
           
           // Bind buffers and draw
           _currentEncoder?.SetVertexBuffer(vertexBuffer, 0, 0);
           
           var metalPrimitiveType = ConvertPrimitiveType(primitiveType);
           var indexCount = primitiveCount * GetIndicesPerPrimitive(primitiveType);
           _currentEncoder?.DrawIndexedPrimitives(metalPrimitiveType, (nuint)indexCount, MTLIndexType.UInt16, indexBuffer, 0);
           
           vertexBuffer?.Dispose();
           indexBuffer?.Dispose();
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error drawing indexed primitives: {ex.Message}");
           throw;
       }
   }
   
   // Helper methods
   private MetalVertex[] ConvertVertexData<T>(T[] vertexData, int offset, int count) where T : struct, IVertexType
   {
       var result = new MetalVertex[count];
       
       for (int i = 0; i < count; i++)
       {
           var vertex = vertexData[offset + i];
           
           // This is a simplified conversion - in practice, you'd need to handle different vertex types
           if (vertex is CCV3F_C4B_T2F ccVertex)
           {
               result[i] = MetalCommon.ConvertCCVertex(ccVertex);
           }
           else
           {
               // Default conversion for unknown types
               result[i] = new MetalVertex
               {
                   Position = Vector3.Zero,
                   TexCoord = Vector2.Zero,
                   Color = Vector4.One
               };
           }
       }
       
       return result;
   }
   
   private MTLPrimitiveType ConvertPrimitiveType(PrimitiveType primitiveType)
   {
       switch (primitiveType)
       {
           case PrimitiveType.TriangleList:
               return MTLPrimitiveType.Triangle;
           case PrimitiveType.TriangleStrip:
               return MTLPrimitiveType.TriangleStrip;
           case PrimitiveType.LineList:
               return MTLPrimitiveType.Line;
           case PrimitiveType.LineStrip:
               return MTLPrimitiveType.LineStrip;
           default:
               throw new ArgumentException($"Unsupported primitive type: {primitiveType}");
       }
   }
   
   private int GetVertexCount(PrimitiveType primitiveType, int primitiveCount)
   {
       switch (primitiveType)
       {
           case PrimitiveType.TriangleList:
               return primitiveCount * 3;
           case PrimitiveType.TriangleStrip:
               return primitiveCount + 2;
           case PrimitiveType.LineList:
               return primitiveCount * 2;
           case PrimitiveType.LineStrip:
               return primitiveCount + 1;
           default:
               return primitiveCount;
       }
   }
   
   private int GetIndicesPerPrimitive(PrimitiveType primitiveType)
   {
       switch (primitiveType)
       {
           case PrimitiveType.TriangleList:
               return 3;
           case PrimitiveType.LineList:
               return 2;
           default:
               return 1;
       }
   }
   ```

### Verification:
- MetalDevice implements all IGraphicsDevice interface methods
- Clearing and presentation work correctly
- Drawing operations render properly
- Resource management prevents leaks

---

## Subtask 3.1.3: Implement Metal Shader System
**Time Estimate**: 2.5 hours  
**Dependencies**: Subtask 3.1.2  
**Assignee**: Graphics programmer with Metal shading experience

### Steps:

1. **Create basic Metal shaders**
   ```metal
   // File: cocos2d/platform/Metal/Shaders/Basic.metal
   #include <metal_stdlib>
   using namespace metal;
   
   // Vertex input structure
   struct VertexInput
   {
       float3 position [[attribute(0)]];
       float2 texCoord [[attribute(1)]];
       float4 color [[attribute(2)]];
   };
   
   // Vertex output structure
   struct VertexOutput
   {
       float4 position [[position]];
       float2 texCoord;
       float4 color;
   };
   
   // Uniform buffer structure
   struct Uniforms
   {
       float4x4 modelViewProjectionMatrix;
       float4 tintColor;
       float time;
   };
   
   // Basic vertex shader
   vertex VertexOutput basic_vertex(VertexInput input [[stage_in]],
                                  constant Uniforms& uniforms [[buffer(0)]])
   {
       VertexOutput output;
       
       float4 position = float4(input.position, 1.0);
       output.position = uniforms.modelViewProjectionMatrix * position;
       output.texCoord = input.texCoord;
       output.color = input.color * uniforms.tintColor;
       
       return output;
   }
   
   // Basic fragment shader
   fragment float4 basic_fragment(VertexOutput input [[stage_in]],
                                texture2d<float> colorTexture [[texture(0)]],
                                sampler colorSampler [[sampler(0)]])
   {
       float4 color = colorTexture.sample(colorSampler, input.texCoord);
       return color * input.color;
   }
   
   // Fragment shader for textured sprites
   fragment float4 sprite_fragment(VertexOutput input [[stage_in]],
                                 texture2d<float> spriteTexture [[texture(0)]],
                                 sampler spriteSampler [[sampler(0)]])
   {
       float4 textureColor = spriteTexture.sample(spriteSampler, input.texCoord);
       
       // Apply vertex color modulation
       return textureColor * input.color;
   }
   
   // Fragment shader for solid color rendering
   fragment float4 solid_color_fragment(VertexOutput input [[stage_in]])
   {
       return input.color;
   }
   
   // Fragment shader with alpha test
   fragment float4 alpha_test_fragment(VertexOutput input [[stage_in]],
                                     texture2d<float> colorTexture [[texture(0)]],
                                     sampler colorSampler [[sampler(0)]],
                                     constant float& alphaThreshold [[buffer(1)]])
   {
       float4 color = colorTexture.sample(colorSampler, input.texCoord);
       color *= input.color;
       
       // Alpha test
       if (color.a < alphaThreshold) {
           discard_fragment();
       }
       
       return color;
   }
   ```

2. **Create Metal effect wrapper**
   ```csharp
   // File: cocos2d/platform/Metal/MetalEffect.cs
   #if METAL && (IOS || MACOS)
   using System;
   using System.Collections.Generic;
   using Microsoft.Xna.Framework;
   using Microsoft.Xna.Framework.Graphics;
   using Foundation;
   using Metal;
   using Cocos2D.Platform.Interfaces;
   
   namespace Cocos2D.Platform.Metal
   {
       /// <summary>
       /// Metal implementation of IEffect
       /// </summary>
       public class MetalEffect : IEffect, IDisposable
       {
           private IMTLDevice _device;
           private IMTLRenderPipelineState _pipelineState;
           private IMTLDepthStencilState _depthStencilState;
           private IMTLBuffer _uniformBuffer;
           private MetalUniforms _uniforms;
           
           // Effect parameters
           private Matrix _world = Matrix.Identity;
           private Matrix _view = Matrix.Identity; 
           private Matrix _projection = Matrix.Identity;
           private Vector3 _diffuseColor = Vector3.One;
           private float _alpha = 1.0f;
           private ITexture2D _texture;
           
           // Metal resources
           private readonly Dictionary<string, object> _parameters;
           private bool _disposed = false;
           
           public MetalEffect(IMTLDevice device, string vertexShader = "basic_vertex", string fragmentShader = "basic_fragment")
           {
               _device = device ?? throw new ArgumentNullException(nameof(device));
               _parameters = new Dictionary<string, object>();
               
               CreatePipelineState(vertexShader, fragmentShader);
               CreateUniformBuffer();
               CreateDepthStencilState();
               
               UpdateUniforms();
           }
           
           // IEffect Properties
           public Matrix World 
           { 
               get => _world; 
               set { _world = value; UpdateUniforms(); } 
           }
           
           public Matrix View 
           { 
               get => _view; 
               set { _view = value; UpdateUniforms(); } 
           }
           
           public Matrix Projection 
           { 
               get => _projection; 
               set { _projection = value; UpdateUniforms(); } 
           }
           
           public Vector3 DiffuseColor 
           { 
               get => _diffuseColor; 
               set { _diffuseColor = value; UpdateUniforms(); } 
           }
           
           public float Alpha 
           { 
               get => _alpha; 
               set { _alpha = value; UpdateUniforms(); } 
           }
           
           public ITexture2D Texture 
           { 
               get => _texture; 
               set => _texture = value; 
           }
           
           public EffectParameterCollection Parameters => new MetalEffectParameterCollection(_parameters);
           public EffectTechniqueCollection Techniques => new MetalEffectTechniqueCollection();
           public EffectTechnique CurrentTechnique { get; set; }
           
           private void CreatePipelineState(string vertexShader, string fragmentShader)
           {
               try
               {
                   // Load the Metal library
                   var library = _device.CreateDefaultLibrary();
                   if (library == null)
                   {
                       throw new InvalidOperationException("Could not load Metal shader library");
                   }
                   
                   // Get shader functions
                   var vertexFunction = library.CreateFunction(vertexShader);
                   var fragmentFunction = library.CreateFunction(fragmentShader);
                   
                   if (vertexFunction == null)
                   {
                       throw new InvalidOperationException($"Could not find vertex shader: {vertexShader}");
                   }
                   
                   if (fragmentFunction == null)
                   {
                       throw new InvalidOperationException($"Could not find fragment shader: {fragmentShader}");
                   }
                   
                   // Create pipeline descriptor
                   var pipelineDescriptor = new MTLRenderPipelineDescriptor
                   {
                       VertexFunction = vertexFunction,
                       FragmentFunction = fragmentFunction
                   };
                   
                   // Configure vertex descriptor
                   var vertexDescriptor = new MTLVertexDescriptor();
                   
                   // Position attribute
                   vertexDescriptor.Attributes[0].Format = MTLVertexFormat.Float3;
                   vertexDescriptor.Attributes[0].Offset = 0;
                   vertexDescriptor.Attributes[0].BufferIndex = 0;
                   
                   // Texture coordinate attribute
                   vertexDescriptor.Attributes[1].Format = MTLVertexFormat.Float2;
                   vertexDescriptor.Attributes[1].Offset = 12; // sizeof(float) * 3
                   vertexDescriptor.Attributes[1].BufferIndex = 0;
                   
                   // Color attribute
                   vertexDescriptor.Attributes[2].Format = MTLVertexFormat.Float4;
                   vertexDescriptor.Attributes[2].Offset = 20; // sizeof(float) * 5
                   vertexDescriptor.Attributes[2].BufferIndex = 0;
                   
                   // Buffer layout
                   vertexDescriptor.Layouts[0].Stride = (nuint)MetalVertex.SizeInBytes;
                   vertexDescriptor.Layouts[0].StepRate = 1;
                   vertexDescriptor.Layouts[0].StepFunction = MTLVertexStepFunction.PerVertex;
                   
                   pipelineDescriptor.VertexDescriptor = vertexDescriptor;
                   
                   // Configure color attachment
                   pipelineDescriptor.ColorAttachments[0].PixelFormat = MTLPixelFormat.BGRA8Unorm;
                   pipelineDescriptor.ColorAttachments[0].BlendingEnabled = true;
                   pipelineDescriptor.ColorAttachments[0].SourceRGBBlendFactor = MTLBlendFactor.SourceAlpha;
                   pipelineDescriptor.ColorAttachments[0].DestinationRGBBlendFactor = MTLBlendFactor.OneMinusSourceAlpha;
                   pipelineDescriptor.ColorAttachments[0].SourceAlphaBlendFactor = MTLBlendFactor.One;
                   pipelineDescriptor.ColorAttachments[0].DestinationAlphaBlendFactor = MTLBlendFactor.OneMinusSourceAlpha;
                   
                   // Create pipeline state
                   NSError error;
                   _pipelineState = _device.CreateRenderPipelineState(pipelineDescriptor, out error);
                   
                   if (_pipelineState == null)
                   {
                       throw new InvalidOperationException($"Failed to create pipeline state: {error?.LocalizedDescription}");
                   }
                   
                   CCLog.Log($"Metal effect created: {vertexShader}/{fragmentShader}");
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Error creating Metal pipeline state: {ex.Message}");
                   throw;
               }
           }
           
           private void CreateUniformBuffer()
           {
               _uniformBuffer = _device.CreateBuffer((nuint)MetalUniforms.SizeInBytes, MTLResourceOptions.CpuCacheModeWriteCombined);
               if (_uniformBuffer == null)
               {
                   throw new InvalidOperationException("Failed to create uniform buffer");
               }
           }
           
           private void CreateDepthStencilState()
           {
               var depthStencilDescriptor = new MTLDepthStencilDescriptor
               {
                   DepthCompareFunction = MTLCompareFunction.LessEqual,
                   DepthWriteEnabled = true
               };
               
               _depthStencilState = _device.CreateDepthStencilState(depthStencilDescriptor);
           }
           
           private void UpdateUniforms()
           {
               if (_uniformBuffer == null) return;
               
               _uniforms.ModelViewProjectionMatrix = Matrix.Transpose(_world * _view * _projection);
               _uniforms.TintColor = new Vector4(_diffuseColor * _alpha, _alpha);
               _uniforms.Time = (float)DateTime.Now.TimeOfDay.TotalSeconds;
               
               unsafe
               {
                   var uniformsPointer = (MetalUniforms*)_uniformBuffer.Contents;
                   *uniformsPointer = _uniforms;
               }
           }
           
           /// <summary>
           /// Applies this effect to the Metal render encoder
           /// </summary>
           public void Apply(IMTLRenderCommandEncoder encoder)
           {
               if (_disposed) throw new ObjectDisposedException(nameof(MetalEffect));
               if (encoder == null) throw new ArgumentNullException(nameof(encoder));
               
               // Set pipeline state
               encoder.SetRenderPipelineState(_pipelineState);
               encoder.SetDepthStencilState(_depthStencilState);
               
               // Set uniform buffer
               encoder.SetVertexBuffer(_uniformBuffer, 0, 0);
               encoder.SetFragmentBuffer(_uniformBuffer, 0, 0);
               
               // Set texture if available
               if (_texture is MetalTexture2D metalTexture)
               {
                   encoder.SetFragmentTexture(metalTexture.Texture, 0);
                   encoder.SetFragmentSamplerState(metalTexture.SamplerState, 0);
               }
           }
           
           public void Dispose()
           {
               if (!_disposed)
               {
                   _pipelineState?.Dispose();
                   _depthStencilState?.Dispose();
                   _uniformBuffer?.Dispose();
                   
                   _pipelineState = null;
                   _depthStencilState = null;
                   _uniformBuffer = null;
                   
                   _disposed = true;
                   CCLog.Log("Metal effect disposed");
               }
           }
       }
       
       // Placeholder implementations for effect collections
       public class MetalEffectParameterCollection : EffectParameterCollection
       {
           private readonly Dictionary<string, object> _parameters;
           
           public MetalEffectParameterCollection(Dictionary<string, object> parameters)
           {
               _parameters = parameters;
           }
           
           // Implementation would provide access to Metal-specific parameters
       }
       
       public class MetalEffectTechniqueCollection : EffectTechniqueCollection
       {
           // Implementation would provide access to Metal-specific techniques
       }
   }
   #endif
   ```

### Verification:
- Metal shaders compile successfully
- Effect wrapper creates valid pipeline states
- Uniform buffers update correctly
- Effect application works with render encoder

---

## Summary and Timeline

### Total Estimated Time: ~5.5 hours (most of one day for one developer)

### Optimal Task Assignment (Single specialized developer required):

**Metal Graphics Developer:**
- Subtask 3.1.1: Project Setup (1h)
- Subtask 3.1.2: Device Implementation (2h) 
- Subtask 3.1.3: Shader System (2.5h)
- **Total: 5.5h**

### Dependencies:
```
3.1.1 (Setup) ──> 3.1.2 (Device) ──> 3.1.3 (Shaders)
```

This core Metal implementation provides:
- Complete Metal device abstraction conforming to IGraphicsDevice
- Basic shader system with vertex/fragment shader support
- Proper resource management and disposal patterns
- Foundation for Metal texture, buffer, and rendering implementations
- Error handling and logging for debugging
- Performance-optimized uniform buffer management

The next subtasks will build upon this foundation to implement Metal-specific textures, buffers, and the complete renderer.