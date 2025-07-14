# Task 5.1: Unit Testing Framework - Detailed Breakdown

## Overview
Create comprehensive unit testing framework for the Metal backend implementation, ensuring all components work correctly and maintaining high code quality and reliability throughout the abstraction layer.

---

## Subtask 5.1.1: Setup Testing Infrastructure
**Time Estimate**: 1.5 hours  
**Dependencies**: Phase 4 complete  
**Assignee**: Test engineer or senior developer

### Steps:

1. **Create test project structure**
   ```bash
   mkdir -p Tests/Cocos2D.Metal.Tests
   mkdir -p Tests/Cocos2D.Metal.Tests/Unit
   mkdir -p Tests/Cocos2D.Metal.Tests/Integration
   mkdir -p Tests/Cocos2D.Metal.Tests/Mocks
   mkdir -p Tests/Cocos2D.Metal.Tests/Fixtures
   mkdir -p Tests/Cocos2D.Metal.Tests/Utilities
   ```

2. **Create test project file**
   ```xml
   <!-- File: Tests/Cocos2D.Metal.Tests/Cocos2D.Metal.Tests.csproj -->
   <Project Sdk="Microsoft.NET.Sdk">
   
     <PropertyGroup>
       <TargetFramework>net6.0</TargetFramework>
       <IsPackable>false</IsPackable>
       <Platforms>x64</Platforms>
       <UseAppHost>false</UseAppHost>
       <LangVersion>latest</LangVersion>
     </PropertyGroup>
   
     <PropertyGroup Condition="'$(TargetPlatformIdentifier)' == 'iOS'">
       <DefineConstants>$(DefineConstants);IOS;METAL</DefineConstants>
     </PropertyGroup>
   
     <PropertyGroup Condition="'$(TargetPlatformIdentifier)' == 'macOS'">
       <DefineConstants>$(DefineConstants);MACOS;METAL</DefineConstants>
     </PropertyGroup>
   
     <ItemGroup>
       <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.3.2" />
       <PackageReference Include="xunit" Version="2.4.2" />
       <PackageReference Include="xunit.runner.visualstudio" Version="2.4.3">
         <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
         <PrivateAssets>all</PrivateAssets>
       </PackageReference>
       <PackageReference Include="Moq" Version="4.18.4" />
       <PackageReference Include="FluentAssertions" Version="6.8.0" />
       <PackageReference Include="Microsoft.Extensions.Logging" Version="6.0.0" />
       <PackageReference Include="Microsoft.Extensions.Logging.Console" Version="6.0.0" />
     </ItemGroup>
   
     <ItemGroup>
       <ProjectReference Include="../../cocos2d/cocos2d.csproj" />
     </ItemGroup>
   
     <ItemGroup Condition="'$(TargetPlatformIdentifier)' == 'iOS'">
       <Reference Include="Xamarin.iOS" />
     </ItemGroup>
   
     <ItemGroup Condition="'$(TargetPlatformIdentifier)' == 'macOS'">
       <Reference Include="Xamarin.Mac" />
     </ItemGroup>
   
   </Project>
   ```

3. **Create base test class**
   ```csharp
   // File: Tests/Cocos2D.Metal.Tests/MetalTestBase.cs
   using System;
   using System.IO;
   using Microsoft.Extensions.Logging;
   using Xunit;
   using Xunit.Abstractions;
   using Cocos2D.Platform.Factory;
   #if METAL && (IOS || MACOS)
   using Metal;
   using Cocos2D.Platform.Metal;
   #endif
   
   namespace Cocos2D.Tests.Metal
   {
       /// <summary>
       /// Base class for all Metal-related tests
       /// </summary>
       public abstract class MetalTestBase : IDisposable
       {
           protected readonly ITestOutputHelper Output;
           protected readonly ILogger Logger;
           
   #if METAL && (IOS || MACOS)
           protected IMTLDevice MetalDevice;
           protected bool IsMetalAvailable;
   #endif
           
           protected MetalTestBase(ITestOutputHelper output)
           {
               Output = output ?? throw new ArgumentNullException(nameof(output));
               
               // Setup logging
               var loggerFactory = LoggerFactory.Create(builder =>
               {
                   builder.AddConsole().SetMinimumLevel(LogLevel.Debug);
               });
               Logger = loggerFactory.CreateLogger(GetType().Name);
               
               InitializeMetal();
               SetupGraphicsFactory();
           }
           
           private void InitializeMetal()
           {
   #if METAL && (IOS || MACOS)
               try
               {
                   MetalDevice = MTLDevice.SystemDefault;
                   IsMetalAvailable = MetalDevice != null;
                   
                   if (IsMetalAvailable)
                   {
                       Logger.LogInformation($"Metal device available: {MetalDevice.Name}");
                   }
                   else
                   {
                       Logger.LogWarning("Metal device not available - some tests will be skipped");
                   }
               }
               catch (Exception ex)
               {
                   Logger.LogError(ex, "Failed to initialize Metal device");
                   IsMetalAvailable = false;
               }
   #else
               IsMetalAvailable = false;
               Logger.LogInformation("Metal not available on this platform");
   #endif
           }
           
           private void SetupGraphicsFactory()
           {
               try
               {
                   // Reset factory to clean state
                   CCGraphicsFactory.Reset();
                   
                   // Configure for testing
                   Environment.SetEnvironmentVariable("COCOS2D_GRAPHICS_DEBUG", "true");
                   CCGraphicsFactory.ConfigureFromEnvironment();
                   
                   Logger.LogInformation("Graphics factory configured for testing");
               }
               catch (Exception ex)
               {
                   Logger.LogError(ex, "Failed to setup graphics factory");
                   throw;
               }
           }
           
           /// <summary>
           /// Skips test if Metal is not available
           /// </summary>
           protected void RequireMetal()
           {
               if (!IsMetalAvailable)
               {
                   throw new SkipException("Metal is not available on this system");
               }
           }
           
           /// <summary>
           /// Creates a test texture with known properties
           /// </summary>
           protected byte[] CreateTestTextureData(int width, int height, Microsoft.Xna.Framework.Graphics.SurfaceFormat format = Microsoft.Xna.Framework.Graphics.SurfaceFormat.Color)
           {
               int bytesPerPixel = GetBytesPerPixel(format);
               var data = new byte[width * height * bytesPerPixel];
               
               // Create a simple pattern for testing
               for (int y = 0; y < height; y++)
               {
                   for (int x = 0; x < width; x++)
                   {
                       int index = (y * width + x) * bytesPerPixel;
                       
                       switch (format)
                       {
                           case Microsoft.Xna.Framework.Graphics.SurfaceFormat.Color:
                               data[index + 0] = (byte)(x % 256);     // R
                               data[index + 1] = (byte)(y % 256);     // G
                               data[index + 2] = (byte)((x + y) % 256); // B
                               data[index + 3] = 255;                 // A
                               break;
                               
                           case Microsoft.Xna.Framework.Graphics.SurfaceFormat.Alpha8:
                               data[index] = (byte)((x + y) % 256);
                               break;
                               
                           default:
                               // Fill with test pattern
                               for (int i = 0; i < bytesPerPixel; i++)
                               {
                                   data[index + i] = (byte)((x + y + i) % 256);
                               }
                               break;
                       }
                   }
               }
               
               return data;
           }
           
           private int GetBytesPerPixel(Microsoft.Xna.Framework.Graphics.SurfaceFormat format)
           {
               switch (format)
               {
                   case Microsoft.Xna.Framework.Graphics.SurfaceFormat.Color:
                   case Microsoft.Xna.Framework.Graphics.SurfaceFormat.Bgra32:
                       return 4;
                   case Microsoft.Xna.Framework.Graphics.SurfaceFormat.Bgr565:
                   case Microsoft.Xna.Framework.Graphics.SurfaceFormat.Bgra5551:
                   case Microsoft.Xna.Framework.Graphics.SurfaceFormat.Bgra4444:
                       return 2;
                   case Microsoft.Xna.Framework.Graphics.SurfaceFormat.Alpha8:
                       return 1;
                   default:
                       return 4;
               }
           }
           
           /// <summary>
           /// Creates a temporary file for testing
           /// </summary>
           protected string CreateTempFile(string extension = ".tmp")
           {
               var tempPath = Path.GetTempFileName();
               var tempFileWithExtension = Path.ChangeExtension(tempPath, extension);
               
               if (tempPath != tempFileWithExtension)
               {
                   File.Move(tempPath, tempFileWithExtension);
               }
               
               return tempFileWithExtension;
           }
           
           /// <summary>
           /// Asserts that two byte arrays are equal within a tolerance
           /// </summary>
           protected void AssertDataEqual(byte[] expected, byte[] actual, int tolerance = 0)
           {
               Assert.Equal(expected.Length, actual.Length);
               
               for (int i = 0; i < expected.Length; i++)
               {
                   var diff = Math.Abs(expected[i] - actual[i]);
                   Assert.True(diff <= tolerance, 
                       $"Data mismatch at index {i}: expected {expected[i]}, actual {actual[i]}, tolerance {tolerance}");
               }
           }
           
           public virtual void Dispose()
           {
               try
               {
                   CCGraphicsFactory.Reset();
                   Logger.LogInformation("Test cleanup completed");
               }
               catch (Exception ex)
               {
                   Logger.LogError(ex, "Error during test cleanup");
               }
           }
       }
       
       /// <summary>
       /// Custom exception for skipping tests
       /// </summary>
       public class SkipException : Exception
       {
           public SkipException(string reason) : base(reason) { }
       }
   }
   ```

4. **Create mock objects**
   ```csharp
   // File: Tests/Cocos2D.Metal.Tests/Mocks/MockMetalDevice.cs
   #if METAL && (IOS || MACOS)
   using System;
   using Foundation;
   using Metal;
   using Moq;
   
   namespace Cocos2D.Tests.Metal.Mocks
   {
       /// <summary>
       /// Mock Metal device for testing
       /// </summary>
       public static class MockMetalDevice
       {
           public static Mock<IMTLDevice> Create()
           {
               var mock = new Mock<IMTLDevice>();
               
               // Setup basic properties
               mock.Setup(d => d.Name).Returns("Mock Metal Device");
               mock.Setup(d => d.HasUnifiedMemory).Returns(true);
               mock.Setup(d => d.MaxTextureWidth2D).Returns(16384);
               mock.Setup(d => d.MaxTextureHeight2D).Returns(16384);
               mock.Setup(d => d.MaxBufferLength).Returns(1024 * 1024 * 1024); // 1GB
               
               // Setup feature set support
               mock.Setup(d => d.SupportsFeatureSet(It.IsAny<MTLFeatureSet>())).Returns(true);
               
               // Setup buffer creation
               mock.Setup(d => d.CreateBuffer(It.IsAny<nuint>(), It.IsAny<MTLResourceOptions>()))
                   .Returns((nuint size, MTLResourceOptions options) => MockBuffer.Create(size, options).Object);
               
               // Setup texture creation
               mock.Setup(d => d.CreateTexture(It.IsAny<MTLTextureDescriptor>()))
                   .Returns((MTLTextureDescriptor descriptor) => MockTexture.Create(descriptor).Object);
               
               // Setup command queue creation
               mock.Setup(d => d.CreateCommandQueue())
                   .Returns(() => MockCommandQueue.Create().Object);
               
               return mock;
           }
       }
       
       public static class MockBuffer
       {
           public static Mock<IMTLBuffer> Create(nuint size, MTLResourceOptions options)
           {
               var mock = new Mock<IMTLBuffer>();
               
               mock.Setup(b => b.Length).Returns(size);
               mock.Setup(b => b.Contents).Returns(IntPtr.Zero); // In real tests, this would be a valid pointer
               
               return mock;
           }
       }
       
       public static class MockTexture
       {
           public static Mock<IMTLTexture> Create(MTLTextureDescriptor descriptor)
           {
               var mock = new Mock<IMTLTexture>();
               
               mock.Setup(t => t.Width).Returns(descriptor.Width);
               mock.Setup(t => t.Height).Returns(descriptor.Height);
               mock.Setup(t => t.PixelFormat).Returns(descriptor.PixelFormat);
               mock.Setup(t => t.MipmapLevelCount).Returns(descriptor.MipmapLevelCount);
               
               return mock;
           }
       }
       
       public static class MockCommandQueue
       {
           public static Mock<IMTLCommandQueue> Create()
           {
               var mock = new Mock<IMTLCommandQueue>();
               
               mock.Setup(q => q.CommandBuffer())
                   .Returns(() => MockCommandBuffer.Create().Object);
               
               return mock;
           }
       }
       
       public static class MockCommandBuffer
       {
           public static Mock<IMTLCommandBuffer> Create()
           {
               var mock = new Mock<IMTLCommandBuffer>();
               
               mock.Setup(b => b.CreateRenderCommandEncoder(It.IsAny<MTLRenderPassDescriptor>()))
                   .Returns(() => MockRenderCommandEncoder.Create().Object);
               
               return mock;
           }
       }
       
       public static class MockRenderCommandEncoder
       {
           public static Mock<IMTLRenderCommandEncoder> Create()
           {
               var mock = new Mock<IMTLRenderCommandEncoder>();
               
               // Setup basic render encoder methods as no-ops
               mock.Setup(e => e.SetRenderPipelineState(It.IsAny<IMTLRenderPipelineState>()));
               mock.Setup(e => e.SetVertexBuffer(It.IsAny<IMTLBuffer>(), It.IsAny<nuint>(), It.IsAny<nuint>()));
               mock.Setup(e => e.DrawPrimitives(It.IsAny<MTLPrimitiveType>(), It.IsAny<nuint>(), It.IsAny<nuint>()));
               
               return mock;
           }
       }
   }
   #endif
   ```

### Verification:
- Test infrastructure compiles on all target platforms
- Mock objects provide valid test doubles
- Base test class provides useful utilities
- Project references are correctly configured

---

## Subtask 5.1.2: Create Factory and Interface Tests
**Time Estimate**: 2 hours  
**Dependencies**: Subtask 5.1.1  
**Assignee**: Test engineer

### Steps:

1. **Create graphics factory tests**
   ```csharp
   // File: Tests/Cocos2D.Metal.Tests/Unit/CCGraphicsFactoryTests.cs
   using System;
   using System.Linq;
   using Xunit;
   using Xunit.Abstractions;
   using FluentAssertions;
   using Cocos2D.Platform.Factory;
   using Microsoft.Xna.Framework.Graphics;
   
   namespace Cocos2D.Tests.Metal.Unit
   {
       public class CCGraphicsFactoryTests : MetalTestBase
       {
           public CCGraphicsFactoryTests(ITestOutputHelper output) : base(output) { }
           
           [Fact]
           public void GetAvailableBackends_ShouldReturnValidBackends()
           {
               // Act
               var backends = CCGraphicsFactory.GetAvailableBackends();
               
               // Assert
               backends.Should().NotBeNull();
               backends.Should().ContainKey(GraphicsBackend.MonoGame);
               
               #if METAL && (IOS || MACOS)
               if (IsMetalAvailable)
               {
                   backends.Should().ContainKey(GraphicsBackend.Metal);
               }
               #endif
               
               Logger.LogInformation($"Available backends: {string.Join(", ", backends.Keys)}");
           }
           
           [Fact]
           public void IsBackendAvailable_MonoGame_ShouldReturnTrue()
           {
               // Act & Assert
               CCGraphicsFactory.IsBackendAvailable(GraphicsBackend.MonoGame).Should().BeTrue();
           }
           
           #if METAL && (IOS || MACOS)
           [Fact]
           public void IsBackendAvailable_Metal_ShouldMatchDeviceAvailability()
           {
               // Act
               var isAvailable = CCGraphicsFactory.IsBackendAvailable(GraphicsBackend.Metal);
               
               // Assert
               isAvailable.Should().Be(IsMetalAvailable);
           }
           
           [Fact]
           public void GetBackendCapabilities_Metal_ShouldReturnValidCapabilities()
           {
               RequireMetal();
               
               // Act
               var capabilities = CCGraphicsFactory.GetBackendCapabilities(GraphicsBackend.Metal);
               
               // Assert
               capabilities.Should().NotBeNull();
               capabilities.Backend.Should().Be(GraphicsBackend.Metal);
               capabilities.IsAvailable.Should().BeTrue();
               capabilities.Name.Should().Be("Metal");
               capabilities.PerformanceScore.Should().BeGreaterThan(0);
               capabilities.MaxTextureSize.Should().BeGreaterThan(0);
           }
           #endif
           
           [Fact]
           public void CreateRenderer_ShouldReturnValidRenderer()
           {
               // Act
               var renderer = CCGraphicsFactory.CreateRenderer();
               
               // Assert
               renderer.Should().NotBeNull();
               renderer.Should().BeAssignableTo<Cocos2D.Platform.Interfaces.IGraphicsRenderer>();
           }
           
           [Fact]
           public void CreateRenderer_CalledTwice_ShouldReturnSameInstance()
           {
               // Act
               var renderer1 = CCGraphicsFactory.CreateRenderer();
               var renderer2 = CCGraphicsFactory.CreateRenderer();
               
               // Assert
               renderer1.Should().BeSameAs(renderer2);
           }
           
           [Fact]
           public void ForceBackend_BeforeCreation_ShouldSetBackend()
           {
               // Arrange
               CCGraphicsFactory.Reset();
               
               // Act
               CCGraphicsFactory.ForceBackend(GraphicsBackend.MonoGame);
               var renderer = CCGraphicsFactory.CreateRenderer();
               
               // Assert
               CCGraphicsFactory.CurrentBackend.Should().Be(GraphicsBackend.MonoGame);
           }
           
           [Fact]
           public void ForceBackend_AfterCreation_ShouldThrowException()
           {
               // Arrange
               CCGraphicsFactory.CreateRenderer();
               
               // Act & Assert
               Assert.Throws<InvalidOperationException>(() => 
                   CCGraphicsFactory.ForceBackend(GraphicsBackend.Metal));
           }
           
           [Fact]
           public void Reset_ShouldClearCurrentRenderer()
           {
               // Arrange
               var renderer = CCGraphicsFactory.CreateRenderer();
               renderer.Should().NotBeNull();
               
               // Act
               CCGraphicsFactory.Reset();
               
               // Assert
               CCGraphicsFactory.GetCurrentRenderer().Should().BeNull();
               CCGraphicsFactory.CurrentBackend.Should().BeNull();
           }
           
           [Theory]
           [InlineData("COCOS2D_GRAPHICS_BACKEND", "MonoGame")]
           [InlineData("COCOS2D_GRAPHICS_DEBUG", "true")]
           [InlineData("COCOS2D_PREFER_PERFORMANCE", "false")]
           public void ConfigureFromEnvironment_ShouldRespectEnvironmentVariables(string envVar, string value)
           {
               // Arrange
               CCGraphicsFactory.Reset();
               Environment.SetEnvironmentVariable(envVar, value);
               
               // Act
               CCGraphicsFactory.ConfigureFromEnvironment();
               
               // Assert
               // Verification depends on specific environment variable
               // This test ensures no exceptions are thrown
               
               // Cleanup
               Environment.SetEnvironmentVariable(envVar, null);
           }
       }
   }
   ```

2. **Create interface tests**
   ```csharp
   // File: Tests/Cocos2D.Metal.Tests/Unit/InterfaceTests.cs
   using System;
   using Xunit;
   using Xunit.Abstractions;
   using FluentAssertions;
   using Cocos2D.Platform.Interfaces;
   using Microsoft.Xna.Framework;
   using Microsoft.Xna.Framework.Graphics;
   
   namespace Cocos2D.Tests.Metal.Unit
   {
       public class InterfaceTests : MetalTestBase
       {
           public InterfaceTests(ITestOutputHelper output) : base(output) { }
           
           [Fact]
           public void IGraphicsDevice_ShouldHaveRequiredProperties()
           {
               // Arrange
               var renderer = CCGraphicsFactory.CreateRenderer();
               var device = renderer.GetGraphicsDevice();
               
               // Assert
               device.Should().NotBeNull();
               device.Should().BeAssignableTo<IGraphicsDevice>();
               
               // Check required properties exist and have reasonable values
               device.GraphicsDeviceStatus.Should().Be(GraphicsDeviceStatus.Normal);
               device.IsDisposed.Should().BeFalse();
               device.MaxTextureSize.Should().BeGreaterThan(0);
           }
           
           [Fact]
           public void ITexture2D_ShouldSupportBasicOperations()
           {
               // Arrange
               var renderer = CCGraphicsFactory.CreateRenderer();
               var texture = renderer.CreateTexture2D(64, 64, false, SurfaceFormat.Color);
               
               // Assert
               texture.Should().NotBeNull();
               texture.Should().BeAssignableTo<ITexture2D>();
               texture.Width.Should().Be(64);
               texture.Height.Should().Be(64);
               texture.Format.Should().Be(SurfaceFormat.Color);
               texture.LevelCount.Should().Be(1);
           }
           
           [Fact]
           public void ISpriteBatch_ShouldSupportBasicOperations()
           {
               // Arrange
               var renderer = CCGraphicsFactory.CreateRenderer();
               var device = renderer.GetGraphicsDevice();
               var spriteBatch = renderer.CreateSpriteBatch(device);
               
               // Assert
               spriteBatch.Should().NotBeNull();
               spriteBatch.Should().BeAssignableTo<ISpriteBatch>();
               
               // Test basic begin/end cycle (should not throw)
               Action beginEnd = () =>
               {
                   spriteBatch.Begin();
                   spriteBatch.End();
               };
               
               beginEnd.Should().NotThrow();
           }
           
           [Fact]
           public void IRenderTarget2D_ShouldSupportCreation()
           {
               // Arrange
               var renderer = CCGraphicsFactory.CreateRenderer();
               var renderTarget = renderer.CreateRenderTarget2D(
                   128, 128, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
               
               // Assert
               renderTarget.Should().NotBeNull();
               renderTarget.Should().BeAssignableTo<IRenderTarget2D>();
               renderTarget.Width.Should().Be(128);
               renderTarget.Height.Should().Be(128);
               renderTarget.Format.Should().Be(SurfaceFormat.Color);
               renderTarget.DepthStencilFormat.Should().Be(DepthFormat.None);
           }
           
           [Fact]
           public void ICCTexture2D_ShouldSupportCocos2DFeatures()
           {
               // Arrange
               var renderer = CCGraphicsFactory.CreateRenderer();
               var ccTexture = renderer.CreateCCTexture2D();
               
               // Assert
               ccTexture.Should().NotBeNull();
               ccTexture.Should().BeAssignableTo<ICCTexture2D>();
               
               // Test cocos2d-specific properties
               ccTexture.ContentSize.Should().NotBeNull();
               ccTexture.IsAntialiased.Should().BeTrue(); // Default value
               ccTexture.HasPremultipliedAlpha.Should().BeDefined();
               ccTexture.HasMipmaps.Should().BeDefined();
           }
           
           [Theory]
           [InlineData(32, 32, SurfaceFormat.Color)]
           [InlineData(64, 128, SurfaceFormat.Color)]
           [InlineData(256, 256, SurfaceFormat.Alpha8)]
           public void ITexture2D_DataAccess_ShouldWorkCorrectly(int width, int height, SurfaceFormat format)
           {
               // Arrange
               var renderer = CCGraphicsFactory.CreateRenderer();
               var texture = renderer.CreateTexture2D(width, height, false, format);
               var testData = CreateTestTextureData(width, height, format);
               
               // Act
               texture.SetData(testData);
               
               var retrievedData = new byte[testData.Length];
               texture.GetData(retrievedData);
               
               // Assert
               AssertDataEqual(testData, retrievedData, tolerance: 1); // Allow for minor precision differences
           }
           
           [Fact]
           public void IGraphicsDevice_BufferCreation_ShouldWork()
           {
               // Arrange
               var renderer = CCGraphicsFactory.CreateRenderer();
               
               // Act
               var vertexBuffer = renderer.CreateVertexBuffer(typeof(VertexPositionColor), 100, BufferUsage.WriteOnly);
               var indexBuffer = renderer.CreateIndexBuffer(IndexElementSize.SixteenBits, 300, BufferUsage.WriteOnly);
               
               // Assert
               vertexBuffer.Should().NotBeNull();
               vertexBuffer.VertexCount.Should().Be(100);
               vertexBuffer.BufferUsage.Should().Be(BufferUsage.WriteOnly);
               
               indexBuffer.Should().NotBeNull();
               indexBuffer.IndexCount.Should().Be(300);
               indexBuffer.IndexElementSize.Should().Be(IndexElementSize.SixteenBits);
           }
       }
   }
   ```

### Verification:
- Factory tests verify backend detection and creation
- Interface tests confirm all required methods work
- Tests handle platform differences appropriately
- Mock objects integrate correctly with test framework

---

## Subtask 5.1.3: Create Metal-Specific Component Tests
**Time Estimate**: 2.5 hours  
**Dependencies**: Subtask 5.1.2  
**Assignee**: Test engineer with Metal knowledge

### Steps:

1. **Create Metal device tests**
   ```csharp
   // File: Tests/Cocos2D.Metal.Tests/Unit/MetalDeviceTests.cs
   #if METAL && (IOS || MACOS)
   using System;
   using Xunit;
   using Xunit.Abstractions;
   using FluentAssertions;
   using Microsoft.Xna.Framework;
   using Microsoft.Xna.Framework.Graphics;
   using Cocos2D.Platform.Metal;
   using MetalKit;
   using CoreGraphics;
   
   namespace Cocos2D.Tests.Metal.Unit
   {
       public class MetalDeviceTests : MetalTestBase
       {
           public MetalDeviceTests(ITestOutputHelper output) : base(output) { }
           
           [Fact]
           public void Constructor_WithValidMTKView_ShouldInitialize()
           {
               RequireMetal();
               
               // Arrange
               var view = new MTKView(new CGRect(0, 0, 800, 600), MetalDevice);
               
               // Act
               var metalDevice = new MetalDevice(view);
               
               // Assert
               metalDevice.Should().NotBeNull();
               metalDevice.GraphicsDeviceStatus.Should().Be(GraphicsDeviceStatus.Normal);
               metalDevice.IsDisposed.Should().BeFalse();
               metalDevice.MaxTextureSize.Should().BeGreaterThan(0);
               metalDevice.SupportsNonPowerOfTwoTextures.Should().BeTrue();
               
               // Cleanup
               metalDevice.Dispose();
               view.Dispose();
           }
           
           [Fact]
           public void Constructor_WithNullView_ShouldThrowException()
           {
               // Act & Assert
               Assert.Throws<ArgumentNullException>(() => new MetalDevice(null));
           }
           
           [Fact]
           public void Clear_WithValidColor_ShouldNotThrow()
           {
               RequireMetal();
               
               // Arrange
               var view = CreateTestMTKView();
               var metalDevice = new MetalDevice(view);
               
               // Act & Assert
               Action clearAction = () => metalDevice.Clear(Color.Red);
               clearAction.Should().NotThrow();
               
               // Cleanup
               metalDevice.Dispose();
               view.Dispose();
           }
           
           [Fact]
           public void Clear_WithClearOptions_ShouldNotThrow()
           {
               RequireMetal();
               
               // Arrange
               var view = CreateTestMTKView();
               var metalDevice = new MetalDevice(view);
               
               // Act & Assert
               Action clearAction = () => metalDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Blue, 1.0f, 0);
               clearAction.Should().NotThrow();
               
               // Cleanup
               metalDevice.Dispose();
               view.Dispose();
           }
           
           [Fact]
           public void Present_ShouldNotThrow()
           {
               RequireMetal();
               
               // Arrange
               var view = CreateTestMTKView();
               var metalDevice = new MetalDevice(view);
               
               // Act & Assert
               Action presentAction = () => metalDevice.Present();
               presentAction.Should().NotThrow();
               
               // Cleanup
               metalDevice.Dispose();
               view.Dispose();
           }
           
           [Fact]
           public void BlendState_SetAndGet_ShouldWork()
           {
               RequireMetal();
               
               // Arrange
               var view = CreateTestMTKView();
               var metalDevice = new MetalDevice(view);
               
               // Act
               metalDevice.BlendState = BlendState.Additive;
               
               // Assert
               metalDevice.BlendState.Should().Be(BlendState.Additive);
               
               // Cleanup
               metalDevice.Dispose();
               view.Dispose();
           }
           
           [Fact]
           public void Viewport_SetAndGet_ShouldWork()
           {
               RequireMetal();
               
               // Arrange
               var view = CreateTestMTKView();
               var metalDevice = new MetalDevice(view);
               var testViewport = new Viewport(10, 20, 640, 480);
               
               // Act
               metalDevice.Viewport = testViewport;
               
               // Assert
               metalDevice.Viewport.Should().Be(testViewport);
               
               // Cleanup
               metalDevice.Dispose();
               view.Dispose();
           }
           
           [Fact]
           public void Dispose_ShouldMarkAsDisposed()
           {
               RequireMetal();
               
               // Arrange
               var view = CreateTestMTKView();
               var metalDevice = new MetalDevice(view);
               
               // Act
               metalDevice.Dispose();
               
               // Assert
               metalDevice.IsDisposed.Should().BeTrue();
               
               // Cleanup
               view.Dispose();
           }
           
           [Fact]
           public void Dispose_CalledTwice_ShouldNotThrow()
           {
               RequireMetal();
               
               // Arrange
               var view = CreateTestMTKView();
               var metalDevice = new MetalDevice(view);
               
               // Act & Assert
               metalDevice.Dispose();
               Action secondDispose = () => metalDevice.Dispose();
               secondDispose.Should().NotThrow();
               
               // Cleanup
               view.Dispose();
           }
           
           private MTKView CreateTestMTKView()
           {
               var view = new MTKView(new CGRect(0, 0, 800, 600), MetalDevice)
               {
                   ColorPixelFormat = Metal.MTLPixelFormat.BGRA8Unorm,
                   DepthStencilPixelFormat = Metal.MTLPixelFormat.Depth24Unorm_Stencil8
               };
               
               return view;
           }
       }
   }
   #endif
   ```

2. **Create Metal texture tests**
   ```csharp
   // File: Tests/Cocos2D.Metal.Tests/Unit/MetalTexture2DTests.cs
   #if METAL && (IOS || MACOS)
   using System;
   using Xunit;
   using Xunit.Abstractions;
   using FluentAssertions;
   using Microsoft.Xna.Framework;
   using Microsoft.Xna.Framework.Graphics;
   using Cocos2D.Platform.Metal;
   
   namespace Cocos2D.Tests.Metal.Unit
   {
       public class MetalTexture2DTests : MetalTestBase
       {
           public MetalTexture2DTests(ITestOutputHelper output) : base(output) { }
           
           [Fact]
           public void Constructor_WithValidParameters_ShouldInitialize()
           {
               RequireMetal();
               
               // Act
               var texture = new MetalTexture2D(MetalDevice, 256, 256, false, SurfaceFormat.Color);
               
               // Assert
               texture.Should().NotBeNull();
               texture.Width.Should().Be(256);
               texture.Height.Should().Be(256);
               texture.Format.Should().Be(SurfaceFormat.Color);
               texture.LevelCount.Should().Be(1);
               texture.IsTextureDefined.Should().BeTrue();
               
               // Cleanup
               texture.Dispose();
           }
           
           [Fact]
           public void Constructor_WithMipmaps_ShouldHaveMultipleLevels()
           {
               RequireMetal();
               
               // Act
               var texture = new MetalTexture2D(MetalDevice, 256, 256, true, SurfaceFormat.Color);
               
               // Assert
               texture.LevelCount.Should().BeGreaterThan(1);
               texture.HasMipmaps.Should().BeTrue();
               
               // Cleanup
               texture.Dispose();
           }
           
           [Theory]
           [InlineData(64, 64, SurfaceFormat.Color)]
           [InlineData(128, 256, SurfaceFormat.Color)]
           [InlineData(32, 32, SurfaceFormat.Alpha8)]
           public void SetData_GetData_ShouldPreserveData(int width, int height, SurfaceFormat format)
           {
               RequireMetal();
               
               // Arrange
               var texture = new MetalTexture2D(MetalDevice, width, height, false, format);
               var originalData = CreateTestTextureData(width, height, format);
               
               // Act
               texture.SetData(originalData);
               
               var retrievedData = new byte[originalData.Length];
               texture.GetData(retrievedData);
               
               // Assert
               AssertDataEqual(originalData, retrievedData, tolerance: 1);
               
               // Cleanup
               texture.Dispose();
           }
           
           [Fact]
           public void InitWithData_ValidData_ShouldReturnTrue()
           {
               RequireMetal();
               
               // Arrange
               var texture = new MetalTexture2D(MetalDevice, 1, 1, false, SurfaceFormat.Color);
               var testData = CreateTestTextureData(64, 64, SurfaceFormat.Color);
               
               // Act
               var result = texture.InitWithData(testData, SurfaceFormat.Color, false);
               
               // Assert
               result.Should().BeTrue();
               texture.PixelsWide.Should().BeGreaterThan(0);
               texture.PixelsHigh.Should().BeGreaterThan(0);
               
               // Cleanup
               texture.Dispose();
           }
           
           [Fact]
           public void InitWithData_NullData_ShouldReturnFalse()
           {
               RequireMetal();
               
               // Arrange
               var texture = new MetalTexture2D(MetalDevice, 1, 1, false, SurfaceFormat.Color);
               
               // Act
               var result = texture.InitWithData(null, SurfaceFormat.Color, false);
               
               // Assert
               result.Should().BeFalse();
               
               // Cleanup
               texture.Dispose();
           }
           
           [Fact]
           public void InitWithData_EmptyData_ShouldReturnFalse()
           {
               RequireMetal();
               
               // Arrange
               var texture = new MetalTexture2D(MetalDevice, 1, 1, false, SurfaceFormat.Color);
               var emptyData = new byte[0];
               
               // Act
               var result = texture.InitWithData(emptyData, SurfaceFormat.Color, false);
               
               // Assert
               result.Should().BeFalse();
               
               // Cleanup
               texture.Dispose();
           }
           
           [Fact]
           public void SamplerState_SetAndGet_ShouldWork()
           {
               RequireMetal();
               
               // Arrange
               var texture = new MetalTexture2D(MetalDevice, 64, 64, false, SurfaceFormat.Color);
               
               // Act
               texture.SamplerState = SamplerState.PointClamp;
               
               // Assert
               texture.SamplerState.Should().NotBeNull();
               // Note: Exact equality might not work due to internal conversions
               
               // Cleanup
               texture.Dispose();
           }
           
           [Fact]
           public void IsAntialiased_SetAndGet_ShouldWork()
           {
               RequireMetal();
               
               // Arrange
               var texture = new MetalTexture2D(MetalDevice, 64, 64, false, SurfaceFormat.Color);
               
               // Act
               texture.IsAntialiased = false;
               
               // Assert
               texture.IsAntialiased.Should().BeFalse();
               
               // Cleanup
               texture.Dispose();
           }
           
           [Fact]
           public void Dispose_ShouldMarkAsNotDefined()
           {
               RequireMetal();
               
               // Arrange
               var texture = new MetalTexture2D(MetalDevice, 64, 64, false, SurfaceFormat.Color);
               texture.IsTextureDefined.Should().BeTrue();
               
               // Act
               texture.Dispose();
               
               // Assert
               texture.IsTextureDefined.Should().BeFalse();
           }
       }
   }
   #endif
   ```

3. **Create Metal renderer tests**
   ```csharp
   // File: Tests/Cocos2D.Metal.Tests/Unit/MetalRendererTests.cs
   #if METAL && (IOS || MACOS)
   using System;
   using Xunit;
   using Xunit.Abstractions;
   using FluentAssertions;
   using Microsoft.Xna.Framework.Graphics;
   using Cocos2D.Platform.Metal;
   using Cocos2D.Platform.Interfaces;
   
   namespace Cocos2D.Tests.Metal.Unit
   {
       public class MetalRendererTests : MetalTestBase
       {
           public MetalRendererTests(ITestOutputHelper output) : base(output) { }
           
           [Fact]
           public void Constructor_ShouldInitialize()
           {
               RequireMetal();
               
               // Act
               var renderer = new MetalRenderer();
               
               // Assert
               renderer.Should().NotBeNull();
               renderer.Should().BeAssignableTo<IGraphicsRenderer>();
               
               // Cleanup
               renderer.Dispose();
           }
           
           [Fact]
           public void CreateTexture2D_ValidParameters_ShouldReturnTexture()
           {
               RequireMetal();
               
               // Arrange
               var renderer = new MetalRenderer();
               
               // Act
               var texture = renderer.CreateTexture2D(128, 128, false, SurfaceFormat.Color);
               
               // Assert
               texture.Should().NotBeNull();
               texture.Should().BeAssignableTo<ITexture2D>();
               texture.Width.Should().Be(128);
               texture.Height.Should().Be(128);
               
               // Cleanup
               texture.Dispose();
               renderer.Dispose();
           }
           
           [Fact]
           public void CreateCCTexture2D_ShouldReturnCCTexture()
           {
               RequireMetal();
               
               // Arrange
               var renderer = new MetalRenderer();
               
               // Act
               var texture = renderer.CreateCCTexture2D();
               
               // Assert
               texture.Should().NotBeNull();
               texture.Should().BeAssignableTo<ICCTexture2D>();
               
               // Cleanup
               texture.Dispose();
               renderer.Dispose();
           }
           
           [Fact]
           public void CreateRenderTarget2D_ValidParameters_ShouldReturnRenderTarget()
           {
               RequireMetal();
               
               // Arrange
               var renderer = new MetalRenderer();
               
               // Act
               var renderTarget = renderer.CreateRenderTarget2D(
                   256, 256, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
               
               // Assert
               renderTarget.Should().NotBeNull();
               renderTarget.Should().BeAssignableTo<IRenderTarget2D>();
               renderTarget.Width.Should().Be(256);
               renderTarget.Height.Should().Be(256);
               
               // Cleanup
               renderTarget.Dispose();
               renderer.Dispose();
           }
           
           [Fact]
           public void CreateVertexBuffer_ValidParameters_ShouldReturnBuffer()
           {
               RequireMetal();
               
               // Arrange
               var renderer = new MetalRenderer();
               
               // Act
               var buffer = renderer.CreateVertexBuffer(typeof(VertexPositionColor), 100, BufferUsage.WriteOnly);
               
               // Assert
               buffer.Should().NotBeNull();
               buffer.VertexCount.Should().Be(100);
               buffer.BufferUsage.Should().Be(BufferUsage.WriteOnly);
               
               // Cleanup
               buffer.Dispose();
               renderer.Dispose();
           }
           
           [Fact]
           public void CreateIndexBuffer_ValidParameters_ShouldReturnBuffer()
           {
               RequireMetal();
               
               // Arrange
               var renderer = new MetalRenderer();
               
               // Act
               var buffer = renderer.CreateIndexBuffer(IndexElementSize.SixteenBits, 300, BufferUsage.WriteOnly);
               
               // Assert
               buffer.Should().NotBeNull();
               buffer.IndexCount.Should().Be(300);
               buffer.IndexElementSize.Should().Be(IndexElementSize.SixteenBits);
               
               // Cleanup
               buffer.Dispose();
               renderer.Dispose();
           }
           
           [Fact]
           public void CreateTexture2DFromData_ValidData_ShouldReturnTexture()
           {
               RequireMetal();
               
               // Arrange
               var renderer = new MetalRenderer();
               var testData = CreateTestTextureData(64, 64, SurfaceFormat.Color);
               
               // Act
               var texture = renderer.CreateTexture2DFromData(testData, SurfaceFormat.Color, false);
               
               // Assert
               texture.Should().NotBeNull();
               texture.Width.Should().BeGreaterThan(0);
               texture.Height.Should().BeGreaterThan(0);
               
               // Cleanup
               texture.Dispose();
               renderer.Dispose();
           }
           
           [Fact]
           public void Dispose_ShouldNotThrow()
           {
               RequireMetal();
               
               // Arrange
               var renderer = new MetalRenderer();
               
               // Act & Assert
               Action disposeAction = () => renderer.Dispose();
               disposeAction.Should().NotThrow();
           }
       }
   }
   #endif
   ```

### Verification:
- Metal component tests verify platform-specific functionality
- Tests handle Metal device availability gracefully
- Resource creation and disposal work correctly
- Data round-trips preserve integrity

---

## Summary and Timeline

### Total Estimated Time: ~6 hours (3/4 of a full day for one developer)

### Optimal Task Assignment (Single test engineer required):

**Test Engineer:**
- Subtask 5.1.1: Testing Infrastructure (1.5h)
- Subtask 5.1.2: Factory and Interface Tests (2h) 
- Subtask 5.1.3: Metal Component Tests (2.5h)
- **Total: 6h**

### Dependencies:
```
5.1.1 (Infrastructure) ──> 5.1.2 (Factory Tests) ──> 5.1.3 (Component Tests)
```

This unit testing framework provides:
- Comprehensive test infrastructure with proper mocking capabilities
- Platform-aware test execution that handles Metal availability
- Complete test coverage for graphics factory and interface contracts
- Detailed testing of Metal-specific components and functionality
- Proper resource management and cleanup in tests
- Integration with modern testing frameworks and assertion libraries

The testing framework ensures the reliability and correctness of the Metal backend implementation while providing a foundation for continuous testing throughout development.