# Task 2.1: Refactor CCDrawManager - Detailed Breakdown

## Overview
Refactor CCDrawManager to use the new abstraction interfaces instead of direct MonoGame dependencies. This is the most critical refactoring as CCDrawManager is the central graphics coordination point.

---

## Subtask 2.1.1: Analyze Current CCDrawManager Dependencies
**Time Estimate**: 45 minutes  
**Dependencies**: Task 1.1, 1.2, 1.3 complete  
**Assignee**: Senior developer familiar with CCDrawManager

### Steps:

1. **Audit current MonoGame dependencies in CCDrawManager**
   From `/cocos2d/platform/CCDrawManager.cs`:
   ```csharp
   // Lines 38-42: Direct MonoGame dependencies
   internal static GraphicsDevice graphicsDevice;
   internal static SpriteBatch spriteBatch;
   internal static BasicEffect basicEffect;
   internal static AlphaTestEffect alphaTestEffect;
   internal static EffectPass effectPass;
   ```

2. **Identify all graphics operations requiring abstraction**
   Key methods to analyze:
   - `Initialize()` (lines 90-150): Device initialization
   - `SetRenderTarget()` (lines 200-220): Render target management
   - `DrawQuad()` (lines 400-450): Primitive rendering
   - `BindTexture()` (lines 350-380): Texture binding
   - `BlendFunc()` (lines 300-340): Blend state management

3. **Document abstraction requirements**
   ```csharp
   // Create analysis document
   /*
   CCDrawManager Abstraction Requirements:
   
   1. Replace GraphicsDevice with IGraphicsDevice
   2. Replace SpriteBatch with ISpriteBatch  
   3. Replace BasicEffect with IEffect
   4. Replace Texture2D operations with ITexture2D
   5. Abstract render target operations
   6. Abstract blend state management
   7. Abstract viewport and scissor operations
   */
   ```

### Verification:
- Complete dependency analysis documented
- All abstraction points identified
- Impact assessment completed

---

## Subtask 2.1.2: Create CCDrawManager Interface
**Time Estimate**: 1 hour  
**Dependencies**: Subtask 2.1.1  
**Assignee**: Senior developer

### Steps:

1. **Create IDrawManager interface**
   ```csharp
   // File: cocos2d/platform/Interfaces/IDrawManager.cs
   using System;
   using Microsoft.Xna.Framework;
   using Microsoft.Xna.Framework.Graphics;
   using Cocos2D.Platform.Interfaces;
   
   namespace Cocos2D.Platform.Interfaces
   {
       /// <summary>
       /// Abstraction for the draw manager to decouple from specific graphics implementations
       /// </summary>
       public interface IDrawManager
       {
           // Graphics Device Access
           IGraphicsDevice GraphicsDevice { get; }
           ISpriteBatch SpriteBatch { get; }
           
           // Initialization
           void Initialize(IGraphicsDevice device);
           void LoadContent();
           void UnloadContent();
           
           // Render Target Management
           void PushRenderTarget(IRenderTarget2D renderTarget);
           void PopRenderTarget();
           IRenderTarget2D CurrentRenderTarget { get; }
           
           // Drawing Operations
           void DrawQuad(ref CCV3F_C4B_T2F_Quad quad);
           void DrawTriangles(CCV3F_C4B_T2F[] vertices, int count);
           void DrawLine(CCPoint from, CCPoint to, float width, CCColor4B color);
           
           // State Management
           void BindTexture(ITexture2D texture);
           void BlendFunc(CCBlendFunc blendFunc);
           void SetViewport(int x, int y, int width, int height);
           void SetScissorRect(Rectangle? rect);
           
           // Matrix Operations
           void PushMatrix();
           void PopMatrix();
           void LoadMatrix(ref Matrix matrix);
           void MultMatrix(ref Matrix matrix);
           Matrix CurrentMatrix { get; }
           
           // Batch Operations
           void BeginBatch();
           void EndBatch();
           void FlushBatch();
           
           // Debug and Diagnostics
           int DrawCallCount { get; }
           int TriangleCount { get; }
           void ResetStats();
       }
   }
   ```

2. **Create DrawManager factory method**
   ```csharp
   // Add to CCGraphicsFactory.cs
   /// <summary>
   /// Creates a draw manager using the current graphics renderer
   /// </summary>
   public static IDrawManager CreateDrawManager()
   {
       var renderer = GetCurrentRenderer() ?? CreateRenderer();
       return new CCDrawManagerImpl(renderer);
   }
   ```

### Verification:
- Interface covers all CCDrawManager functionality
- Interface compiles without errors
- Factory method integration complete

---

## Subtask 2.1.3: Implement Abstracted CCDrawManager Core
**Time Estimate**: 2 hours  
**Dependencies**: Subtask 2.1.2  
**Assignee**: Senior developer

### Steps:

1. **Create new CCDrawManagerImpl class**
   ```csharp
   // File: cocos2d/platform/CCDrawManagerImpl.cs
   using System;
   using System.Collections.Generic;
   using Microsoft.Xna.Framework;
   using Microsoft.Xna.Framework.Graphics;
   using Cocos2D.Platform.Interfaces;
   using Cocos2D.Platform.Factory;
   
   namespace Cocos2D
   {
       /// <summary>
       /// Implementation of IDrawManager using abstracted graphics interfaces
       /// </summary>
       internal class CCDrawManagerImpl : IDrawManager
       {
           private IGraphicsRenderer _renderer;
           private IGraphicsDevice _graphicsDevice;
           private ISpriteBatch _spriteBatch;
           private Stack<IRenderTarget2D> _renderTargetStack;
           private Stack<Matrix> _matrixStack;
           private Matrix _currentMatrix;
           
           // Statistics
           private int _drawCalls;
           private int _triangleCount;
           
           public CCDrawManagerImpl(IGraphicsRenderer renderer)
           {
               _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
               _renderTargetStack = new Stack<IRenderTarget2D>();
               _matrixStack = new Stack<Matrix>();
               _currentMatrix = Matrix.Identity;
           }
           
           public IGraphicsDevice GraphicsDevice => _graphicsDevice;
           public ISpriteBatch SpriteBatch => _spriteBatch;
           public Matrix CurrentMatrix => _currentMatrix;
           public int DrawCallCount => _drawCalls;
           public int TriangleCount => _triangleCount;
           
           public IRenderTarget2D CurrentRenderTarget => 
               _renderTargetStack.Count > 0 ? _renderTargetStack.Peek() : null;
       }
   }
   ```

2. **Implement initialization methods**
   ```csharp
   public void Initialize(IGraphicsDevice device)
   {
       _graphicsDevice = device ?? throw new ArgumentNullException(nameof(device));
       _spriteBatch = _renderer.CreateSpriteBatch(_graphicsDevice);
       
       CCLog.Log("CCDrawManager initialized with abstracted renderer");
   }
   
   public void LoadContent()
   {
       // Load default effects and resources through abstracted interfaces
       try
       {
           // Implementation will depend on renderer capabilities
           CCLog.Log("CCDrawManager content loaded");
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error loading CCDrawManager content: {ex.Message}");
           throw;
       }
   }
   
   public void UnloadContent()
   {
       try
       {
           _spriteBatch?.Dispose();
           _spriteBatch = null;
           
           // Clear stacks
           _renderTargetStack.Clear();
           _matrixStack.Clear();
           
           CCLog.Log("CCDrawManager content unloaded");
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error unloading CCDrawManager content: {ex.Message}");
       }
   }
   ```

3. **Implement render target management**
   ```csharp
   public void PushRenderTarget(IRenderTarget2D renderTarget)
   {
       if (renderTarget == null)
           throw new ArgumentNullException(nameof(renderTarget));
       
       // Store current render target
       var current = _graphicsDevice.GetRenderTargets();
       if (current != null && current.Length > 0 && current[0].RenderTarget is IRenderTarget2D currentTarget)
       {
           _renderTargetStack.Push(currentTarget);
       }
       
       // Set new render target
       _graphicsDevice.SetRenderTarget(renderTarget);
       CCLog.Log($"Pushed render target: {renderTarget.Width}x{renderTarget.Height}");
   }
   
   public void PopRenderTarget()
   {
       if (_renderTargetStack.Count == 0)
       {
           _graphicsDevice.SetRenderTarget(null);
           return;
       }
       
       var previous = _renderTargetStack.Pop();
       _graphicsDevice.SetRenderTarget(previous);
       CCLog.Log("Popped render target");
   }
   ```

### Verification:
- Core implementation compiles successfully
- Initialization works with abstracted interfaces
- Render target management functions correctly

---

## Subtask 2.1.4: Implement Drawing Operations
**Time Estimate**: 2.5 hours  
**Dependencies**: Subtask 2.1.3  
**Assignee**: Graphics programmer

### Steps:

1. **Implement quad drawing with abstraction**
   ```csharp
   public void DrawQuad(ref CCV3F_C4B_T2F_Quad quad)
   {
       try
       {
           // Convert quad to vertex array compatible with abstracted interface
           var vertices = new CCV3F_C4B_T2F[4];
           vertices[0] = quad.TopLeft;
           vertices[1] = quad.BottomLeft;
           vertices[2] = quad.TopRight;
           vertices[3] = quad.BottomRight;
           
           // Use abstracted drawing method
           _graphicsDevice.DrawUserPrimitives(
               PrimitiveType.TriangleStrip,
               vertices,
               0,
               2
           );
           
           _drawCalls++;
           _triangleCount += 2;
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error drawing quad: {ex.Message}");
           throw;
       }
   }
   
   public void DrawTriangles(CCV3F_C4B_T2F[] vertices, int count)
   {
       if (vertices == null || count <= 0)
           return;
       
       try
       {
           int triangleCount = count / 3;
           _graphicsDevice.DrawUserPrimitives(
               PrimitiveType.TriangleList,
               vertices,
               0,
               triangleCount
           );
           
           _drawCalls++;
           _triangleCount += triangleCount;
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error drawing triangles: {ex.Message}");
           throw;
       }
   }
   ```

2. **Implement texture and state management**
   ```csharp
   private ITexture2D _currentTexture;
   private CCBlendFunc _currentBlendFunc;
   
   public void BindTexture(ITexture2D texture)
   {
       if (_currentTexture == texture)
           return;
       
       _currentTexture = texture;
       
       // Apply texture to graphics device through abstraction
       _renderer.SetTexture(0, texture);
   }
   
   public void BlendFunc(CCBlendFunc blendFunc)
   {
       if (_currentBlendFunc.Equals(blendFunc))
           return;
       
       _currentBlendFunc = blendFunc;
       
       // Convert CCBlendFunc to abstracted blend state
       var blendState = ConvertBlendFunc(blendFunc);
       _graphicsDevice.BlendState = blendState;
   }
   
   private BlendState ConvertBlendFunc(CCBlendFunc blendFunc)
   {
       // Convert cocos2d blend function to MonoGame BlendState
       // This logic matches current CCDrawManager implementation
       if (blendFunc.Source == CCOGLES.GL_ONE && blendFunc.Destination == CCOGLES.GL_ZERO)
           return BlendState.Opaque;
       
       if (blendFunc.Source == CCOGLES.GL_SRC_ALPHA && blendFunc.Destination == CCOGLES.GL_ONE_MINUS_SRC_ALPHA)
           return BlendState.AlphaBlend;
       
       if (blendFunc.Source == CCOGLES.GL_SRC_ALPHA && blendFunc.Destination == CCOGLES.GL_ONE)
           return BlendState.Additive;
       
       // For custom blend functions, create custom BlendState
       return CreateCustomBlendState(blendFunc);
   }
   
   private BlendState CreateCustomBlendState(CCBlendFunc blendFunc)
   {
       var customBlend = new BlendState
       {
           ColorSourceBlend = ConvertBlendFactor(blendFunc.Source),
           ColorDestinationBlend = ConvertBlendFactor(blendFunc.Destination),
           AlphaSourceBlend = ConvertBlendFactor(blendFunc.Source),
           AlphaDestinationBlend = ConvertBlendFactor(blendFunc.Destination)
       };
       
       return customBlend;
   }
   ```

3. **Implement viewport and scissor operations**
   ```csharp
   public void SetViewport(int x, int y, int width, int height)
   {
       var viewport = new Viewport(x, y, width, height);
       _graphicsDevice.Viewport = viewport;
   }
   
   public void SetScissorRect(Rectangle? rect)
   {
       if (rect.HasValue)
       {
           _graphicsDevice.ScissorRectangle = rect.Value;
           // Enable scissor test in rasterizer state
           var rasterizerState = new RasterizerState
           {
               ScissorTestEnable = true
           };
           _graphicsDevice.RasterizerState = rasterizerState;
       }
       else
       {
           // Disable scissor test
           var rasterizerState = new RasterizerState
           {
               ScissorTestEnable = false
           };
           _graphicsDevice.RasterizerState = rasterizerState;
       }
   }
   ```

### Verification:
- Drawing operations work with abstracted interfaces
- Texture binding and state management functional
- Viewport and scissor operations working correctly

---

## Subtask 2.1.5: Implement Matrix Operations
**Time Estimate**: 1 hour  
**Dependencies**: Subtask 2.1.4  
**Assignee**: Math/graphics programmer

### Steps:

1. **Implement matrix stack operations**
   ```csharp
   public void PushMatrix()
   {
       _matrixStack.Push(_currentMatrix);
   }
   
   public void PopMatrix()
   {
       if (_matrixStack.Count > 0)
       {
           _currentMatrix = _matrixStack.Pop();
           UpdateMatrixUniforms();
       }
       else
       {
           CCLog.Log("Warning: Attempted to pop from empty matrix stack");
       }
   }
   
   public void LoadMatrix(ref Matrix matrix)
   {
       _currentMatrix = matrix;
       UpdateMatrixUniforms();
   }
   
   public void MultMatrix(ref Matrix matrix)
   {
       _currentMatrix = Matrix.Multiply(_currentMatrix, matrix);
       UpdateMatrixUniforms();
   }
   
   private void UpdateMatrixUniforms()
   {
       // Update shader uniforms through abstracted renderer
       _renderer.SetWorldMatrix(_currentMatrix);
   }
   ```

2. **Implement batch operations**
   ```csharp
   private bool _batchStarted = false;
   
   public void BeginBatch()
   {
       if (_batchStarted)
       {
           CCLog.Log("Warning: BeginBatch called while batch already started");
           return;
       }
       
       _spriteBatch?.Begin();
       _batchStarted = true;
   }
   
   public void EndBatch()
   {
       if (!_batchStarted)
       {
           CCLog.Log("Warning: EndBatch called while no batch started");
           return;
       }
       
       _spriteBatch?.End();
       _batchStarted = false;
   }
   
   public void FlushBatch()
   {
       if (_batchStarted)
       {
           EndBatch();
           BeginBatch();
       }
   }
   ```

3. **Implement statistics and debugging**
   ```csharp
   public void ResetStats()
   {
       _drawCalls = 0;
       _triangleCount = 0;
   }
   
   public void DrawLine(CCPoint from, CCPoint to, float width, CCColor4B color)
   {
       // Create line geometry
       var vertices = CreateLineVertices(from, to, width, color);
       
       // Draw using abstracted interface
       _graphicsDevice.DrawUserPrimitives(
           PrimitiveType.TriangleList,
           vertices,
           0,
           2
       );
       
       _drawCalls++;
       _triangleCount += 2;
   }
   
   private CCV3F_C4B_T2F[] CreateLineVertices(CCPoint from, CCPoint to, float width, CCColor4B color)
   {
       // Calculate perpendicular vector for line width
       var direction = CCPoint.Normalize(to - from);
       var perpendicular = new CCPoint(-direction.Y, direction.X) * (width * 0.5f);
       
       var vertices = new CCV3F_C4B_T2F[6];
       
       // First triangle
       vertices[0] = new CCV3F_C4B_T2F
       {
           Vertices = new CCVertex3F(from.X - perpendicular.X, from.Y - perpendicular.Y, 0),
           Colors = color,
           TexCoords = new CCTex2F(0, 0)
       };
       
       vertices[1] = new CCV3F_C4B_T2F
       {
           Vertices = new CCVertex3F(from.X + perpendicular.X, from.Y + perpendicular.Y, 0),
           Colors = color,
           TexCoords = new CCTex2F(0, 1)
       };
       
       vertices[2] = new CCV3F_C4B_T2F
       {
           Vertices = new CCVertex3F(to.X - perpendicular.X, to.Y - perpendicular.Y, 0),
           Colors = color,
           TexCoords = new CCTex2F(1, 0)
       };
       
       // Second triangle
       vertices[3] = vertices[1];
       vertices[4] = vertices[2];
       
       vertices[5] = new CCV3F_C4B_T2F
       {
           Vertices = new CCVertex3F(to.X + perpendicular.X, to.Y + perpendicular.Y, 0),
           Colors = color,
           TexCoords = new CCTex2F(1, 1)
       };
       
       return vertices;
   }
   ```

### Verification:
- Matrix operations work correctly
- Batch operations maintain state properly
- Line drawing produces correct geometry

---

## Subtask 2.1.6: Update Original CCDrawManager to Use Abstraction
**Time Estimate**: 1.5 hours  
**Dependencies**: Subtask 2.1.5  
**Assignee**: Senior developer

### Steps:

1. **Modify CCDrawManager static fields**
   ```csharp
   // File: cocos2d/platform/CCDrawManager.cs
   // Replace lines 38-42:
   // OLD:
   // internal static GraphicsDevice graphicsDevice;
   // internal static SpriteBatch spriteBatch;
   
   // NEW:
   private static IDrawManager s_drawManager;
   private static bool s_initialized = false;
   
   // Backward compatibility properties
   internal static GraphicsDevice graphicsDevice 
   { 
       get 
       { 
           EnsureInitialized();
           return s_drawManager?.GraphicsDevice as GraphicsDevice; 
       } 
   }
   
   internal static SpriteBatch spriteBatch 
   { 
       get 
       { 
           EnsureInitialized();
           return s_drawManager?.SpriteBatch as SpriteBatch; 
       } 
   }
   ```

2. **Add initialization method**
   ```csharp
   /// <summary>
   /// Ensures the draw manager is initialized with the current graphics factory
   /// </summary>
   private static void EnsureInitialized()
   {
       if (!s_initialized || s_drawManager == null)
       {
           InitializeWithFactory();
       }
   }
   
   /// <summary>
   /// Initializes CCDrawManager using the graphics factory
   /// </summary>
   public static void InitializeWithFactory()
   {
       try
       {
           s_drawManager = CCGraphicsFactory.CreateDrawManager();
           s_initialized = true;
           CCLog.Log("CCDrawManager initialized with factory");
       }
       catch (Exception ex)
       {
           CCLog.Log($"Failed to initialize CCDrawManager with factory: {ex.Message}");
           throw;
       }
   }
   
   /// <summary>
   /// Manual initialization for backward compatibility
   /// </summary>
   public static void Initialize(GraphicsDevice device, Game game)
   {
       // Create renderer from MonoGame objects if factory not used
       if (!s_initialized)
       {
           var renderer = CCGraphicsFactory.CreateRenderer();
           s_drawManager = new CCDrawManagerImpl(renderer);
           s_drawManager.Initialize(new MonoGameDevice(device));
           s_initialized = true;
       }
   }
   ```

3. **Update existing method implementations**
   ```csharp
   // Replace existing DrawQuad method (around line 400)
   public static void DrawQuad(ref CCV3F_C4B_T2F_Quad quad)
   {
       EnsureInitialized();
       s_drawManager.DrawQuad(ref quad);
   }
   
   // Replace existing BindTexture method (around line 350)
   public static void BindTexture(CCTexture2D texture)
   {
       EnsureInitialized();
       
       // Convert CCTexture2D to ITexture2D
       ITexture2D abstractTexture = texture?.GetAbstractedTexture();
       s_drawManager.BindTexture(abstractTexture);
   }
   
   // Replace existing BlendFunc method (around line 300)
   public static void BlendFunc(CCBlendFunc blendFunc)
   {
       EnsureInitialized();
       s_drawManager.BlendFunc(blendFunc);
   }
   
   // Replace SetRenderTarget methods (around line 200)
   public static void PushRenderTarget(CCRenderTexture renderTexture)
   {
       EnsureInitialized();
       
       if (renderTexture != null)
       {
           var abstractRT = renderTexture.GetAbstractedRenderTarget();
           s_drawManager.PushRenderTarget(abstractRT);
       }
   }
   
   public static void PopRenderTarget()
   {
       EnsureInitialized();
       s_drawManager.PopRenderTarget();
   }
   ```

### Verification:
- Original CCDrawManager API remains unchanged
- All existing calls work through abstraction
- Backward compatibility maintained

---

## Subtask 2.1.7: Add Migration Utilities
**Time Estimate**: 1 hour  
**Dependencies**: Subtask 2.1.6  
**Assignee**: Any developer

### Steps:

1. **Create migration helper methods**
   ```csharp
   // File: cocos2d/platform/CCDrawManagerMigration.cs
   using System;
   using Cocos2D.Platform.Interfaces;
   using Microsoft.Xna.Framework.Graphics;
   
   namespace Cocos2D
   {
       /// <summary>
       /// Helper methods for migrating from direct MonoGame usage to abstracted interfaces
       /// </summary>
       public static class CCDrawManagerMigration
       {
           /// <summary>
           /// Gets the underlying MonoGame GraphicsDevice for legacy compatibility
           /// </summary>
           [Obsolete("Use IGraphicsDevice interface instead")]
           public static GraphicsDevice GetLegacyGraphicsDevice()
           {
               return CCDrawManager.graphicsDevice;
           }
           
           /// <summary>
           /// Gets the underlying MonoGame SpriteBatch for legacy compatibility
           /// </summary>
           [Obsolete("Use ISpriteBatch interface instead")]
           public static SpriteBatch GetLegacySpriteBatch()
           {
               return CCDrawManager.spriteBatch;
           }
           
           /// <summary>
           /// Converts MonoGame Texture2D to abstracted ITexture2D
           /// </summary>
           public static ITexture2D ConvertTexture(Texture2D texture)
           {
               if (texture == null) return null;
               return new MonoGameTexture2D(texture);
           }
           
           /// <summary>
           /// Converts abstracted ITexture2D back to MonoGame Texture2D
           /// </summary>
           public static Texture2D ConvertToMonoGame(ITexture2D texture)
           {
               if (texture is MonoGameTexture2D mgTexture)
                   return mgTexture.UnderlyingTexture;
               
               throw new InvalidOperationException($"Cannot convert {texture?.GetType()} to MonoGame Texture2D");
           }
       }
   }
   ```

2. **Add compatibility extension methods**
   ```csharp
   /// <summary>
   /// Extension methods for backward compatibility
   /// </summary>
   public static class CCDrawManagerExtensions
   {
       /// <summary>
       /// Gets the abstracted texture interface from CCTexture2D
       /// </summary>
       public static ITexture2D GetAbstractedTexture(this CCTexture2D texture)
       {
           if (texture?.XNATexture != null)
           {
               return new MonoGameTexture2D(texture.XNATexture);
           }
           return null;
       }
       
       /// <summary>
       /// Gets the abstracted render target interface from CCRenderTexture
       /// </summary>
       public static IRenderTarget2D GetAbstractedRenderTarget(this CCRenderTexture renderTexture)
       {
           if (renderTexture?.RenderTarget != null)
           {
               return new MonoGameRenderTarget2D(renderTexture.RenderTarget);
           }
           return null;
       }
       
       /// <summary>
       /// Checks if draw manager is using abstracted interfaces
       /// </summary>
       public static bool IsUsingAbstraction()
       {
           return CCDrawManager.IsAbstractionEnabled;
       }
   }
   ```

### Verification:
- Migration utilities compile successfully
- Extension methods work correctly
- Backward compatibility maintained

---

## Subtask 2.1.8: Create Unit Tests for Refactored CCDrawManager
**Time Estimate**: 2 hours  
**Dependencies**: Subtask 2.1.7  
**Assignee**: QA developer

### Steps:

1. **Create test structure**
   ```bash
   mkdir -p Tests/DrawManagerTests
   touch Tests/DrawManagerTests/CCDrawManagerImplTests.cs
   touch Tests/DrawManagerTests/CCDrawManagerCompatibilityTests.cs
   touch Tests/DrawManagerTests/MockGraphicsObjects.cs
   ```

2. **Create basic functionality tests**
   ```csharp
   // File: Tests/DrawManagerTests/CCDrawManagerImplTests.cs
   using Xunit;
   using Moq;
   using Cocos2D;
   using Cocos2D.Platform.Interfaces;
   using Microsoft.Xna.Framework;
   
   namespace Cocos2D.Tests.DrawManager
   {
       public class CCDrawManagerImplTests : IDisposable
       {
           private Mock<IGraphicsRenderer> _mockRenderer;
           private Mock<IGraphicsDevice> _mockDevice;
           private Mock<ISpriteBatch> _mockSpriteBatch;
           private CCDrawManagerImpl _drawManager;
           
           public CCDrawManagerImplTests()
           {
               _mockRenderer = new Mock<IGraphicsRenderer>();
               _mockDevice = new Mock<IGraphicsDevice>();
               _mockSpriteBatch = new Mock<ISpriteBatch>();
               
               _mockRenderer.Setup(r => r.CreateSpriteBatch(It.IsAny<IGraphicsDevice>()))
                          .Returns(_mockSpriteBatch.Object);
               
               _drawManager = new CCDrawManagerImpl(_mockRenderer.Object);
           }
           
           public void Dispose()
           {
               _drawManager?.UnloadContent();
           }
           
           [Fact]
           public void Initialize_WithValidDevice_SetsUpCorrectly()
           {
               // Act
               _drawManager.Initialize(_mockDevice.Object);
               
               // Assert
               Assert.Equal(_mockDevice.Object, _drawManager.GraphicsDevice);
               Assert.Equal(_mockSpriteBatch.Object, _drawManager.SpriteBatch);
           }
           
           [Fact]
           public void DrawQuad_ValidQuad_CallsCorrectMethods()
           {
               // Arrange
               _drawManager.Initialize(_mockDevice.Object);
               var quad = new CCV3F_C4B_T2F_Quad();
               
               // Act
               _drawManager.DrawQuad(ref quad);
               
               // Assert
               Assert.Equal(1, _drawManager.DrawCallCount);
               Assert.Equal(2, _drawManager.TriangleCount);
               
               _mockDevice.Verify(d => d.DrawUserPrimitives(
                   It.IsAny<PrimitiveType>(),
                   It.IsAny<CCV3F_C4B_T2F[]>(),
                   It.IsAny<int>(),
                   It.IsAny<int>()), Times.Once);
           }
           
           [Fact]
           public void PushPopMatrix_MaintainsStack()
           {
               // Arrange
               _drawManager.Initialize(_mockDevice.Object);
               var testMatrix = Matrix.CreateTranslation(10, 20, 0);
               
               // Act
               _drawManager.PushMatrix();
               _drawManager.LoadMatrix(ref testMatrix);
               var currentMatrix = _drawManager.CurrentMatrix;
               _drawManager.PopMatrix();
               var restoredMatrix = _drawManager.CurrentMatrix;
               
               // Assert
               Assert.Equal(testMatrix, currentMatrix);
               Assert.Equal(Matrix.Identity, restoredMatrix);
           }
       }
   }
   ```

3. **Create compatibility tests**
   ```csharp
   // File: Tests/DrawManagerTests/CCDrawManagerCompatibilityTests.cs
   using Xunit;
   using Cocos2D;
   using Cocos2D.Platform.Factory;
   
   namespace Cocos2D.Tests.DrawManager
   {
       public class CCDrawManagerCompatibilityTests : IDisposable
       {
           public CCDrawManagerCompatibilityTests()
           {
               // Reset factory state before each test
               CCGraphicsFactory.Reset();
           }
           
           public void Dispose()
           {
               CCGraphicsFactory.Reset();
           }
           
           [Fact]
           public void StaticProperties_AfterInitialization_ReturnValidValues()
           {
               // Arrange
               CCDrawManager.InitializeWithFactory();
               
               // Act
               var device = CCDrawManager.graphicsDevice;
               var spriteBatch = CCDrawManager.spriteBatch;
               
               // Assert
               Assert.NotNull(device);
               Assert.NotNull(spriteBatch);
           }
           
           [Fact]
           public void DrawOperations_ThroughStaticAPI_WorkCorrectly()
           {
               // Arrange
               CCDrawManager.InitializeWithFactory();
               var quad = new CCV3F_C4B_T2F_Quad();
               
               // Act & Assert (should not throw)
               CCDrawManager.DrawQuad(ref quad);
           }
           
           [Fact]
           public void BlendFunc_ThroughStaticAPI_AppliesCorrectly()
           {
               // Arrange
               CCDrawManager.InitializeWithFactory();
               var blendFunc = CCBlendFunc.AlphaBlend;
               
               // Act & Assert (should not throw)
               CCDrawManager.BlendFunc(blendFunc);
           }
       }
   }
   ```

### Verification:
- All tests pass consistently
- Compatibility tests verify backward compatibility
- Tests cover major functionality paths

---

## Summary and Timeline

### Total Estimated Time: ~12 hours (1.5-2 days for one developer)

### Optimal Task Assignment (2 developers working in parallel):

**Developer 1 (Core Refactoring):**
- Subtask 2.1.1: Analysis (45m)
- Subtask 2.1.2: Interface Creation (1h) 
- Subtask 2.1.3: Core Implementation (2h)
- Subtask 2.1.4: Drawing Operations (2.5h)
- **Total: 6.25h**

**Developer 2 (Integration/Testing):**
- Subtask 2.1.5: Matrix Operations (1h)
- Subtask 2.1.6: Original CCDrawManager Update (1.5h)
- Subtask 2.1.7: Migration Utilities (1h)
- Subtask 2.1.8: Unit Tests (2h)
- **Total: 5.5h**

### Dependencies:
```
2.1.1 (Analysis) ──> 2.1.2 (Interface) ──> 2.1.3 (Core) ──> 2.1.4 (Drawing)
                                                                │
                                                                ├──> 2.1.5 (Matrix)
                                                                │
                                                                └──> 2.1.6 (Update Original)
                                                                            │
                                                                            └──> 2.1.7 (Migration) ──> 2.1.8 (Tests)
```

This refactoring provides:
- Complete abstraction from MonoGame dependencies
- Maintained backward compatibility
- Comprehensive testing coverage
- Clear migration path for future updates
- Performance optimization opportunities through renderer selection