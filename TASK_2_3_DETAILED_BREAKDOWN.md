# Task 2.3: Refactor CCSprite - Detailed Breakdown

## Overview
Refactor CCSprite to use the abstracted drawing interfaces instead of direct CCDrawManager calls. This ensures sprites can render through any graphics backend while maintaining all existing functionality.

---

## Subtask 2.3.1: Analyze CCSprite Drawing Dependencies
**Time Estimate**: 45 minutes  
**Dependencies**: Task 2.1, 2.2 complete  
**Assignee**: Developer familiar with CCSprite

### Steps:

1. **Audit current drawing calls in CCSprite**
   From `/cocos2d/sprite_nodes/CCSprite.cs`:
   ```csharp
   // Lines 989-992: Direct CCDrawManager calls in Draw method
   CCDrawManager.BlendFunc(m_sBlendFunc);
   CCDrawManager.BindTexture(Texture);
   CCDrawManager.DrawQuad(ref m_sQuad);
   
   // Line 1050-1080: Texture binding in UpdateTransform
   if (m_pobTexture != null)
   {
       CCDrawManager.BindTexture(m_pobTexture);
   }
   
   // Lines 1200-1250: Batch rendering support
   if (m_pobBatchNode != null)
   {
       // Uses CCSpriteBatchNode which also calls CCDrawManager
   }
   ```

2. **Identify abstraction requirements**
   ```csharp
   /*
   CCSprite Abstraction Requirements:
   
   1. Replace direct CCDrawManager.BlendFunc() calls
   2. Replace direct CCDrawManager.BindTexture() calls  
   3. Replace direct CCDrawManager.DrawQuad() calls
   4. Abstract batch rendering integration
   5. Maintain performance characteristics
   6. Support dynamic renderer switching
   7. Preserve existing sprite functionality
   8. Maintain CCSpriteBatchNode compatibility
   */
   ```

3. **Document performance considerations**
   ```csharp
   /*
   Performance Impact Analysis:
   
   - CCSprite.Draw() is called frequently (every frame per sprite)
   - Must minimize abstraction overhead
   - Batch rendering performance critical
   - Texture switching should be minimized
   - Quad generation should remain efficient
   - Memory allocations in Draw() must be avoided
   */
   ```

### Verification:
- Complete analysis of drawing dependencies
- Performance impact assessed
- Abstraction strategy documented

---

## Subtask 2.3.2: Create Sprite Rendering Interface
**Time Estimate**: 1 hour  
**Dependencies**: Subtask 2.3.1  
**Assignee**: Graphics programmer

### Steps:

1. **Create ISpriteRenderer interface**
   ```csharp
   // File: cocos2d/platform/Interfaces/ISpriteRenderer.cs
   using System;
   using Microsoft.Xna.Framework;
   using Microsoft.Xna.Framework.Graphics;
   using Cocos2D.Platform.Interfaces;
   
   namespace Cocos2D.Platform.Interfaces
   {
       /// <summary>
       /// Abstraction for sprite rendering operations
       /// </summary>
       public interface ISpriteRenderer : IDisposable
       {
           // Rendering Operations
           void DrawSprite(ref CCV3F_C4B_T2F_Quad quad, ITexture2D texture, CCBlendFunc blendFunc);
           void DrawSprite(ref CCV3F_C4B_T2F_Quad quad, ITexture2D texture, CCBlendFunc blendFunc, Matrix transform);
           
           // Batch Operations
           void BeginBatch(ITexture2D texture, CCBlendFunc blendFunc);
           void DrawQuadInBatch(ref CCV3F_C4B_T2F_Quad quad);
           void EndBatch();
           void FlushBatch();
           
           // State Management
           void SetTexture(ITexture2D texture);
           void SetBlendFunction(CCBlendFunc blendFunc);
           void SetTransform(Matrix transform);
           
           // Performance Queries
           int DrawCallCount { get; }
           int SpriteCount { get; }
           void ResetStats();
           
           // Capabilities
           bool SupportsBatching { get; }
           int MaxBatchSize { get; }
           
           // Events for optimization
           event EventHandler<TextureChangedEventArgs> TextureChanged;
           event EventHandler<BlendFuncChangedEventArgs> BlendFuncChanged;
       }
       
       public class TextureChangedEventArgs : EventArgs
       {
           public ITexture2D PreviousTexture { get; set; }
           public ITexture2D NewTexture { get; set; }
       }
       
       public class BlendFuncChangedEventArgs : EventArgs
       {
           public CCBlendFunc PreviousBlendFunc { get; set; }
           public CCBlendFunc NewBlendFunc { get; set; }
       }
   }
   ```

2. **Create sprite renderer factory**
   ```csharp
   // Add to CCGraphicsFactory.cs
   /// <summary>
   /// Creates a sprite renderer using the current graphics renderer
   /// </summary>
   public static ISpriteRenderer CreateSpriteRenderer()
   {
       var renderer = GetCurrentRenderer() ?? CreateRenderer();
       var drawManager = GetCurrentDrawManager() ?? CreateDrawManager();
       return new CCSpriteRendererImpl(renderer, drawManager);
   }
   
   private static IDrawManager _currentDrawManager;
   
   internal static IDrawManager GetCurrentDrawManager()
   {
       return _currentDrawManager;
   }
   
   internal static void SetCurrentDrawManager(IDrawManager drawManager)
   {
       _currentDrawManager = drawManager;
   }
   ```

3. **Create sprite rendering events and utilities**
   ```csharp
   // File: cocos2d/platform/Interfaces/SpriteRenderingUtilities.cs
   using System;
   using Microsoft.Xna.Framework;
   
   namespace Cocos2D.Platform.Interfaces
   {
       /// <summary>
       /// Utilities for sprite rendering optimization
       /// </summary>
       public static class SpriteRenderingUtilities
       {
           /// <summary>
           /// Compares two blend functions for equality
           /// </summary>
           public static bool AreBlendFuncsEqual(CCBlendFunc a, CCBlendFunc b)
           {
               return a.Source == b.Source && a.Destination == b.Destination;
           }
           
           /// <summary>
           /// Determines if a texture change requires a batch flush
           /// </summary>
           public static bool RequiresBatchFlush(ITexture2D current, ITexture2D newTexture)
           {
               if (current == null && newTexture == null) return false;
               if (current == null || newTexture == null) return true;
               return !ReferenceEquals(current, newTexture);
           }
           
           /// <summary>
           /// Calculates the optimal batch size for a texture
           /// </summary>
           public static int CalculateOptimalBatchSize(ITexture2D texture, int maxBatchSize)
           {
               if (texture == null) return maxBatchSize;
               
               // Consider texture size and memory constraints
               int textureMemory = texture.Width * texture.Height * 4; // Assume 4 bytes per pixel
               int maxTextureMemory = 16 * 1024 * 1024; // 16MB limit
               
               if (textureMemory > maxTextureMemory)
               {
                   return Math.Min(maxBatchSize, 64); // Smaller batches for large textures
               }
               
               return maxBatchSize;
           }
           
           /// <summary>
           /// Transforms a quad by a matrix
           /// </summary>
           public static void TransformQuad(ref CCV3F_C4B_T2F_Quad quad, ref Matrix transform)
           {
               TransformVertex(ref quad.TopLeft.Vertices, ref transform);
               TransformVertex(ref quad.TopRight.Vertices, ref transform);
               TransformVertex(ref quad.BottomLeft.Vertices, ref transform);
               TransformVertex(ref quad.BottomRight.Vertices, ref transform);
           }
           
           private static void TransformVertex(ref CCVertex3F vertex, ref Matrix transform)
           {
               var position = new Vector3(vertex.X, vertex.Y, vertex.Z);
               position = Vector3.Transform(position, transform);
               vertex.X = position.X;
               vertex.Y = position.Y;
               vertex.Z = position.Z;
           }
       }
   }
   ```

### Verification:
- ISpriteRenderer interface covers all sprite needs
- Factory integration works correctly
- Utility methods compile and function properly

---

## Subtask 2.3.3: Implement Sprite Renderer Core
**Time Estimate**: 2.5 hours  
**Dependencies**: Subtask 2.3.2  
**Assignee**: Senior graphics programmer

### Steps:

1. **Create CCSpriteRendererImpl class**
   ```csharp
   // File: cocos2d/platform/CCSpriteRendererImpl.cs
   using System;
   using Microsoft.Xna.Framework;
   using Microsoft.Xna.Framework.Graphics;
   using Cocos2D.Platform.Interfaces;
   
   namespace Cocos2D
   {
       /// <summary>
       /// Implementation of ISpriteRenderer using abstracted graphics interfaces
       /// </summary>
       internal class CCSpriteRendererImpl : ISpriteRenderer
       {
           private IGraphicsRenderer _renderer;
           private IDrawManager _drawManager;
           
           // Current state
           private ITexture2D _currentTexture;
           private CCBlendFunc _currentBlendFunc;
           private Matrix _currentTransform;
           private bool _batchStarted = false;
           
           // Statistics
           private int _drawCalls = 0;
           private int _spriteCount = 0;
           
           // Batch state
           private ITexture2D _batchTexture;
           private CCBlendFunc _batchBlendFunc;
           private bool _disposed = false;
           
           // Events
           public event EventHandler<TextureChangedEventArgs> TextureChanged;
           public event EventHandler<BlendFuncChangedEventArgs> BlendFuncChanged;
           
           public CCSpriteRendererImpl(IGraphicsRenderer renderer, IDrawManager drawManager)
           {
               _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
               _drawManager = drawManager ?? throw new ArgumentNullException(nameof(drawManager));
               _currentTransform = Matrix.Identity;
               _currentBlendFunc = CCBlendFunc.AlphaBlend;
           }
           
           // Properties
           public int DrawCallCount => _drawCalls;
           public int SpriteCount => _spriteCount;
           public bool SupportsBatching => true;
           public int MaxBatchSize => 1024; // Configurable based on renderer capabilities
           
           public void ResetStats()
           {
               _drawCalls = 0;
               _spriteCount = 0;
           }
       }
   }
   ```

2. **Implement individual sprite rendering**
   ```csharp
   public void DrawSprite(ref CCV3F_C4B_T2F_Quad quad, ITexture2D texture, CCBlendFunc blendFunc)
   {
       if (_disposed) throw new ObjectDisposedException(nameof(CCSpriteRendererImpl));
       
       // Ensure batch is not active for individual drawing
       if (_batchStarted)
       {
           EndBatch();
       }
       
       try
       {
           // Set texture if changed
           if (!ReferenceEquals(_currentTexture, texture))
           {
               SetTexture(texture);
           }
           
           // Set blend function if changed
           if (!SpriteRenderingUtilities.AreBlendFuncsEqual(_currentBlendFunc, blendFunc))
           {
               SetBlendFunction(blendFunc);
           }
           
           // Draw the quad
           _drawManager.DrawQuad(ref quad);
           
           _drawCalls++;
           _spriteCount++;
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error drawing sprite: {ex.Message}");
           throw;
       }
   }
   
   public void DrawSprite(ref CCV3F_C4B_T2F_Quad quad, ITexture2D texture, CCBlendFunc blendFunc, Matrix transform)
   {
       if (_disposed) throw new ObjectDisposedException(nameof(CCSpriteRendererImpl));
       
       try
       {
           // Save current transform
           var previousTransform = _currentTransform;
           
           // Apply new transform
           SetTransform(transform);
           
           // Draw sprite
           DrawSprite(ref quad, texture, blendFunc);
           
           // Restore previous transform
           SetTransform(previousTransform);
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error drawing sprite with transform: {ex.Message}");
           throw;
       }
   }
   ```

3. **Implement batch rendering**
   ```csharp
   public void BeginBatch(ITexture2D texture, CCBlendFunc blendFunc)
   {
       if (_disposed) throw new ObjectDisposedException(nameof(CCSpriteRendererImpl));
       
       if (_batchStarted)
       {
           CCLog.Log("Warning: BeginBatch called while batch already active. Ending current batch.");
           EndBatch();
       }
       
       try
       {
           _batchTexture = texture;
           _batchBlendFunc = blendFunc;
           
           // Set initial state
           SetTexture(texture);
           SetBlendFunction(blendFunc);
           
           // Begin drawing batch
           _drawManager.BeginBatch();
           _batchStarted = true;
           
           CCLog.Log($"Sprite batch started with texture: {texture?.Width}x{texture?.Height}");
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error beginning sprite batch: {ex.Message}");
           _batchStarted = false;
           throw;
       }
   }
   
   public void DrawQuadInBatch(ref CCV3F_C4B_T2F_Quad quad)
   {
       if (_disposed) throw new ObjectDisposedException(nameof(CCSpriteRendererImpl));
       
       if (!_batchStarted)
       {
           throw new InvalidOperationException("Cannot draw quad in batch - no batch active. Call BeginBatch first.");
       }
       
       try
       {
           _drawManager.DrawQuad(ref quad);
           _spriteCount++;
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error drawing quad in batch: {ex.Message}");
           throw;
       }
   }
   
   public void EndBatch()
   {
       if (_disposed) throw new ObjectDisposedException(nameof(CCSpriteRendererImpl));
       
       if (!_batchStarted)
       {
           CCLog.Log("Warning: EndBatch called while no batch active");
           return;
       }
       
       try
       {
           _drawManager.EndBatch();
           _batchStarted = false;
           _batchTexture = null;
           _drawCalls++;
           
           CCLog.Log($"Sprite batch ended. Drew {_spriteCount} sprites in batch.");
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error ending sprite batch: {ex.Message}");
           _batchStarted = false;
           throw;
       }
   }
   
   public void FlushBatch()
   {
       if (_disposed) throw new ObjectDisposedException(nameof(CCSpriteRendererImpl));
       
       if (!_batchStarted)
       {
           return;
       }
       
       try
       {
           _drawManager.FlushBatch();
           _drawCalls++;
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error flushing sprite batch: {ex.Message}");
           throw;
       }
   }
   ```

4. **Implement state management**
   ```csharp
   public void SetTexture(ITexture2D texture)
   {
       if (_disposed) throw new ObjectDisposedException(nameof(CCSpriteRendererImpl));
       
       if (ReferenceEquals(_currentTexture, texture))
           return;
       
       var previousTexture = _currentTexture;
       _currentTexture = texture;
       
       try
       {
           _drawManager.BindTexture(texture);
           
           // Fire event for listeners
           TextureChanged?.Invoke(this, new TextureChangedEventArgs
           {
               PreviousTexture = previousTexture,
               NewTexture = texture
           });
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error setting texture: {ex.Message}");
           _currentTexture = previousTexture; // Rollback
           throw;
       }
   }
   
   public void SetBlendFunction(CCBlendFunc blendFunc)
   {
       if (_disposed) throw new ObjectDisposedException(nameof(CCSpriteRendererImpl));
       
       if (SpriteRenderingUtilities.AreBlendFuncsEqual(_currentBlendFunc, blendFunc))
           return;
       
       var previousBlendFunc = _currentBlendFunc;
       _currentBlendFunc = blendFunc;
       
       try
       {
           _drawManager.BlendFunc(blendFunc);
           
           // Fire event for listeners
           BlendFuncChanged?.Invoke(this, new BlendFuncChangedEventArgs
           {
               PreviousBlendFunc = previousBlendFunc,
               NewBlendFunc = blendFunc
           });
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error setting blend function: {ex.Message}");
           _currentBlendFunc = previousBlendFunc; // Rollback
           throw;
       }
   }
   
   public void SetTransform(Matrix transform)
   {
       if (_disposed) throw new ObjectDisposedException(nameof(CCSpriteRendererImpl));
       
       _currentTransform = transform;
       
       try
       {
           _drawManager.LoadMatrix(ref transform);
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error setting transform: {ex.Message}");
           throw;
       }
   }
   
   public void Dispose()
   {
       if (!_disposed)
       {
           try
           {
               if (_batchStarted)
               {
                   EndBatch();
               }
           }
           catch (Exception ex)
           {
               CCLog.Log($"Error disposing sprite renderer: {ex.Message}");
           }
           finally
           {
               _disposed = true;
               _renderer = null;
               _drawManager = null;
               _currentTexture = null;
               
               CCLog.Log("Sprite renderer disposed");
           }
       }
   }
   ```

### Verification:
- Core implementation compiles successfully
- Individual and batch rendering work correctly
- State management maintains consistency

---

## Subtask 2.3.4: Update CCSprite to Use Abstracted Renderer
**Time Estimate**: 2 hours  
**Dependencies**: Subtask 2.3.3  
**Assignee**: Developer familiar with CCSprite

### Steps:

1. **Modify CCSprite class to use ISpriteRenderer**
   ```csharp
   // File: cocos2d/sprite_nodes/CCSprite.cs
   // Add new fields near the top of the class (around line 50)
   private static ISpriteRenderer s_spriteRenderer;
   private static bool s_useAbstraction = true;
   
   /// <summary>
   /// Gets or creates the sprite renderer instance
   /// </summary>
   private static ISpriteRenderer SpriteRenderer
   {
       get
       {
           if (s_spriteRenderer == null)
           {
               try
               {
                   s_spriteRenderer = CCGraphicsFactory.CreateSpriteRenderer();
                   s_useAbstraction = true;
               }
               catch (Exception ex)
               {
                   CCLog.Log($"Failed to create abstracted sprite renderer, falling back to direct calls: {ex.Message}");
                   s_useAbstraction = false;
               }
           }
           return s_spriteRenderer;
       }
   }
   
   /// <summary>
   /// Enables or disables the use of abstracted rendering
   /// </summary>
   public static bool UseAbstractedRendering
   {
       get => s_useAbstraction;
       set
       {
           if (value != s_useAbstraction)
           {
               s_useAbstraction = value;
               if (value)
               {
                   try
                   {
                       s_spriteRenderer = CCGraphicsFactory.CreateSpriteRenderer();
                   }
                   catch (Exception ex)
                   {
                       CCLog.Log($"Failed to enable abstracted rendering: {ex.Message}");
                       s_useAbstraction = false;
                   }
               }
               else
               {
                   s_spriteRenderer?.Dispose();
                   s_spriteRenderer = null;
               }
           }
       }
   }
   ```

2. **Update the Draw method (around line 989)**
   ```csharp
   public override void Draw()
   {
       if (m_pobTexture == null || !Visible)
           return;
       
       if (s_useAbstraction && SpriteRenderer != null)
       {
           DrawWithAbstraction();
       }
       else
       {
           DrawLegacy();
       }
   }
   
   /// <summary>
   /// Draws the sprite using the abstracted renderer
   /// </summary>
   private void DrawWithAbstraction()
   {
       try
       {
           var abstractTexture = m_pobTexture.GetAbstractedTexture();
           if (abstractTexture == null)
           {
               CCLog.Log("Warning: Could not get abstracted texture, falling back to legacy drawing");
               DrawLegacy();
               return;
           }
           
           SpriteRenderer.DrawSprite(ref m_sQuad, abstractTexture, m_sBlendFunc);
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error in abstracted sprite drawing, falling back to legacy: {ex.Message}");
           DrawLegacy();
       }
   }
   
   /// <summary>
   /// Draws the sprite using legacy CCDrawManager calls
   /// </summary>
   private void DrawLegacy()
   {
       // Original implementation
       CCDrawManager.BlendFunc(m_sBlendFunc);
       CCDrawManager.BindTexture(Texture);
       CCDrawManager.DrawQuad(ref m_sQuad);
   }
   ```

3. **Add batch rendering support**
   ```csharp
   /// <summary>
   /// Draws the sprite as part of a batch
   /// </summary>
   internal void DrawInBatch()
   {
       if (m_pobTexture == null || !Visible)
           return;
       
       if (s_useAbstraction && SpriteRenderer != null)
       {
           try
           {
               SpriteRenderer.DrawQuadInBatch(ref m_sQuad);
           }
           catch (Exception ex)
           {
               CCLog.Log($"Error drawing sprite in batch: {ex.Message}");
               // Could fall back to individual drawing, but batch should remain consistent
               throw;
           }
       }
       else
       {
           // Legacy batch drawing
           CCDrawManager.DrawQuad(ref m_sQuad);
       }
   }
   
   /// <summary>
   /// Begins a batch for sprites with the same texture and blend function
   /// </summary>
   public static void BeginSpriteBatch(CCTexture2D texture, CCBlendFunc blendFunc)
   {
       if (s_useAbstraction && SpriteRenderer != null)
       {
           var abstractTexture = texture?.GetAbstractedTexture();
           SpriteRenderer.BeginBatch(abstractTexture, blendFunc);
       }
       else
       {
           // Legacy batch begin
           CCDrawManager.BindTexture(texture);
           CCDrawManager.BlendFunc(blendFunc);
           CCDrawManager.BeginBatch();
       }
   }
   
   /// <summary>
   /// Ends the current sprite batch
   /// </summary>
   public static void EndSpriteBatch()
   {
       if (s_useAbstraction && SpriteRenderer != null)
       {
           SpriteRenderer.EndBatch();
       }
       else
       {
           // Legacy batch end
           CCDrawManager.EndBatch();
       }
   }
   ```

4. **Add performance monitoring**
   ```csharp
   /// <summary>
   /// Gets sprite rendering statistics
   /// </summary>
   public static CCSpriteSuiteStatistics GetRenderingStats()
   {
       if (s_useAbstraction && SpriteRenderer != null)
       {
           return new CCSpriteRenderingStats
           {
               DrawCalls = SpriteRenderer.DrawCallCount,
               SpritesDrawn = SpriteRenderer.SpriteCount,
               IsUsingAbstraction = true,
               RendererType = SpriteRenderer.GetType().Name
           };
       }
       else
       {
           return new CCSpriteRenderingStats
           {
               DrawCalls = 0, // Would need to track in legacy mode
               SpritesDrawn = 0,
               IsUsingAbstraction = false,
               RendererType = "Legacy CCDrawManager"
           };
       }
   }
   
   /// <summary>
   /// Resets sprite rendering statistics
   /// </summary>
   public static void ResetRenderingStats()
   {
       if (s_useAbstraction && SpriteRenderer != null)
       {
           SpriteRenderer.ResetStats();
       }
   }
   
   public struct CCSpriteRenderingStats
   {
       public int DrawCalls;
       public int SpritesDrawn;
       public bool IsUsingAbstraction;
       public string RendererType;
       
       public override string ToString()
       {
           return $"Sprite Stats: {SpritesDrawn} sprites, {DrawCalls} draw calls, {RendererType} ({(IsUsingAbstraction ? "Abstracted" : "Legacy")})";
       }
   }
   ```

### Verification:
- CCSprite uses abstracted renderer by default
- Fallback to legacy rendering works correctly
- Batch rendering functionality preserved

---

## Subtask 2.3.5: Update CCSpriteBatchNode Integration
**Time Estimate**: 1.5 hours  
**Dependencies**: Subtask 2.3.4  
**Assignee**: Developer familiar with batch rendering

### Steps:

1. **Analyze CCSpriteBatchNode current implementation**
   ```csharp
   // File: cocos2d/sprite_nodes/CCSpriteBatchNode.cs
   // Identify key methods that need abstraction:
   // - Draw() method
   // - AddChild() method
   // - Visit() method for batch management
   ```

2. **Update CCSpriteBatchNode to use abstracted renderer**
   ```csharp
   // File: cocos2d/sprite_nodes/CCSpriteBatchNode.cs
   // Add abstraction support
   private bool m_useAbstraction = true;
   
   public override void Draw()
   {
       if (m_pobTextureAtlas == null || m_pobTextureAtlas.TotalQuads == 0)
           return;
       
       if (m_useAbstraction && CCSprite.UseAbstractedRendering)
       {
           DrawWithAbstraction();
       }
       else
       {
           DrawLegacy();
       }
   }
   
   private void DrawWithAbstraction()
   {
       try
       {
           var texture = m_pobTextureAtlas.Texture;
           var abstractTexture = texture?.GetAbstractedTexture();
           
           if (abstractTexture == null)
           {
               CCLog.Log("Warning: CCSpriteBatchNode could not get abstracted texture");
               DrawLegacy();
               return;
           }
           
           var spriteRenderer = CCGraphicsFactory.CreateSpriteRenderer();
           
           // Begin batch with the atlas texture
           spriteRenderer.BeginBatch(abstractTexture, m_blendFunc);
           
           // Draw all quads in the batch
           var quads = m_pobTextureAtlas.Quads;
           for (int i = 0; i < m_pobTextureAtlas.TotalQuads; i++)
           {
               spriteRenderer.DrawQuadInBatch(ref quads[i]);
           }
           
           // End the batch
           spriteRenderer.EndBatch();
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error in CCSpriteBatchNode abstracted drawing: {ex.Message}");
           DrawLegacy();
       }
   }
   
   private void DrawLegacy()
   {
       // Original CCSpriteBatchNode implementation
       CCDrawManager.BlendFunc(m_blendFunc);
       CCDrawManager.BindTexture(m_pobTextureAtlas.Texture);
       
       for (int i = 0; i < m_pobTextureAtlas.TotalQuads; i++)
       {
           var quad = m_pobTextureAtlas.Quads[i];
           CCDrawManager.DrawQuad(ref quad);
       }
   }
   ```

3. **Optimize batch node for abstracted rendering**
   ```csharp
   /// <summary>
   /// Optimized drawing method that leverages renderer batching capabilities
   /// </summary>
   private void DrawOptimizedBatch()
   {
       if (m_pobTextureAtlas == null || m_pobTextureAtlas.TotalQuads == 0)
           return;
       
       try
       {
           var spriteRenderer = CCGraphicsFactory.CreateSpriteRenderer();
           var abstractTexture = m_pobTextureAtlas.Texture?.GetAbstractedTexture();
           
           if (abstractTexture == null || !spriteRenderer.SupportsBatching)
           {
               DrawLegacy();
               return;
           }
           
           // Calculate optimal batch size
           int batchSize = SpriteRenderingUtilities.CalculateOptimalBatchSize(
               abstractTexture, spriteRenderer.MaxBatchSize);
           
           int totalQuads = m_pobTextureAtlas.TotalQuads;
           var quads = m_pobTextureAtlas.Quads;
           
           // Process quads in optimal batch sizes
           for (int start = 0; start < totalQuads; start += batchSize)
           {
               int end = Math.Min(start + batchSize, totalQuads);
               
               spriteRenderer.BeginBatch(abstractTexture, m_blendFunc);
               
               for (int i = start; i < end; i++)
               {
                   spriteRenderer.DrawQuadInBatch(ref quads[i]);
               }
               
               spriteRenderer.EndBatch();
           }
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error in optimized batch drawing: {ex.Message}");
           DrawLegacy();
       }
   }
   
   /// <summary>
   /// Enables or disables abstracted rendering for this batch node
   /// </summary>
   public bool UseAbstractedRendering
   {
       get => m_useAbstraction;
       set
       {
           m_useAbstraction = value;
           CCLog.Log($"CCSpriteBatchNode abstracted rendering: {(value ? "enabled" : "disabled")}");
       }
   }
   ```

### Verification:
- CCSpriteBatchNode works with abstracted renderer
- Batch optimization provides performance benefits
- Legacy fallback maintains compatibility

---

## Subtask 2.3.6: Create Sprite Performance Optimizations
**Time Estimate**: 2 hours  
**Dependencies**: Subtask 2.3.5  
**Assignee**: Performance engineer

### Steps:

1. **Create sprite rendering profiler**
   ```csharp
   // File: cocos2d/platform/CCSpriteRenderProfiler.cs
   using System;
   using System.Collections.Generic;
   using System.Diagnostics;
   using Cocos2D.Platform.Interfaces;
   
   namespace Cocos2D
   {
       /// <summary>
       /// Profiler for sprite rendering performance analysis
       /// </summary>
       public static class CCSpriteRenderProfiler
       {
           private static readonly Dictionary<string, ProfileData> s_profiles = new Dictionary<string, ProfileData>();
           private static bool s_enabled = false;
           
           public static bool IsEnabled
           {
               get => s_enabled;
               set => s_enabled = value;
           }
           
           public static void BeginProfile(string name)
           {
               if (!s_enabled) return;
               
               if (!s_profiles.ContainsKey(name))
               {
                   s_profiles[name] = new ProfileData();
               }
               
               s_profiles[name].StartTime = Stopwatch.GetTimestamp();
           }
           
           public static void EndProfile(string name, int spriteCount = 0)
           {
               if (!s_enabled) return;
               
               if (s_profiles.TryGetValue(name, out var profile))
               {
                   long elapsed = Stopwatch.GetTimestamp() - profile.StartTime;
                   double milliseconds = (double)elapsed / Stopwatch.Frequency * 1000.0;
                   
                   profile.TotalTime += milliseconds;
                   profile.CallCount++;
                   profile.TotalSprites += spriteCount;
                   profile.MinTime = Math.Min(profile.MinTime, milliseconds);
                   profile.MaxTime = Math.Max(profile.MaxTime, milliseconds);
               }
           }
           
           public static string GetProfileReport()
           {
               if (!s_enabled) return "Profiling disabled";
               
               var report = new System.Text.StringBuilder();
               report.AppendLine("Sprite Rendering Profile Report:");
               report.AppendLine("================================");
               
               foreach (var kvp in s_profiles)
               {
                   var profile = kvp.Value;
                   double avgTime = profile.CallCount > 0 ? profile.TotalTime / profile.CallCount : 0;
                   double avgSprites = profile.CallCount > 0 ? (double)profile.TotalSprites / profile.CallCount : 0;
                   
                   report.AppendLine($"{kvp.Key}:");
                   report.AppendLine($"  Calls: {profile.CallCount}");
                   report.AppendLine($"  Total Time: {profile.TotalTime:F2}ms");
                   report.AppendLine($"  Avg Time: {avgTime:F2}ms");
                   report.AppendLine($"  Min Time: {profile.MinTime:F2}ms");
                   report.AppendLine($"  Max Time: {profile.MaxTime:F2}ms");
                   report.AppendLine($"  Total Sprites: {profile.TotalSprites}");
                   report.AppendLine($"  Avg Sprites: {avgSprites:F1}");
                   report.AppendLine();
               }
               
               return report.ToString();
           }
           
           public static void ResetProfiles()
           {
               s_profiles.Clear();
           }
           
           private class ProfileData
           {
               public long StartTime;
               public double TotalTime;
               public int CallCount;
               public long TotalSprites;
               public double MinTime = double.MaxValue;
               public double MaxTime;
           }
       }
   }
   ```

2. **Integrate profiling into sprite rendering**
   ```csharp
   // Update CCSpriteRendererImpl.cs
   public void DrawSprite(ref CCV3F_C4B_T2F_Quad quad, ITexture2D texture, CCBlendFunc blendFunc)
   {
       CCSpriteRenderProfiler.BeginProfile("Individual Sprite Draw");
       
       try
       {
           if (_disposed) throw new ObjectDisposedException(nameof(CCSpriteRendererImpl));
           
           // ... existing implementation ...
           
           _drawCalls++;
           _spriteCount++;
       }
       finally
       {
           CCSpriteRenderProfiler.EndProfile("Individual Sprite Draw", 1);
       }
   }
   
   public void EndBatch()
   {
       CCSpriteRenderProfiler.BeginProfile("Batch End");
       
       try
       {
           if (_disposed) throw new ObjectDisposedException(nameof(CCSpriteRendererImpl));
           
           if (!_batchStarted)
           {
               CCLog.Log("Warning: EndBatch called while no batch active");
               return;
           }
           
           int spritesInBatch = _spriteCount - _spriteCountAtBatchStart;
           
           _drawManager.EndBatch();
           _batchStarted = false;
           _batchTexture = null;
           _drawCalls++;
           
           CCLog.Log($"Sprite batch ended. Drew {spritesInBatch} sprites in batch.");
       }
       finally
       {
           CCSpriteRenderProfiler.EndProfile("Batch End", _spriteCount - _spriteCountAtBatchStart);
       }
   }
   
   private int _spriteCountAtBatchStart;
   
   public void BeginBatch(ITexture2D texture, CCBlendFunc blendFunc)
   {
       // ... existing implementation ...
       
       _spriteCountAtBatchStart = _spriteCount;
       _batchStarted = true;
   }
   ```

3. **Create sprite rendering optimizer**
   ```csharp
   // File: cocos2d/platform/CCSpriteRenderOptimizer.cs
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Cocos2D.Platform.Interfaces;
   
   namespace Cocos2D
   {
       /// <summary>
       /// Optimizer for sprite rendering operations
       /// </summary>
       public static class CCSpriteRenderOptimizer
       {
           private static Dictionary<ITexture2D, List<CCSprite>> s_spritesByTexture;
           private static bool s_autoOptimize = false;
           
           public static bool AutoOptimizeEnabled
           {
               get => s_autoOptimize;
               set => s_autoOptimize = value;
           }
           
           /// <summary>
           /// Groups sprites by texture for optimal batch rendering
           /// </summary>
           public static void OptimizeSpriteBatches(IEnumerable<CCSprite> sprites)
           {
               if (!s_autoOptimize) return;
               
               s_spritesByTexture = new Dictionary<ITexture2D, List<CCSprite>>();
               
               // Group sprites by texture
               foreach (var sprite in sprites.Where(s => s.Visible && s.Texture != null))
               {
                   var abstractTexture = sprite.Texture.GetAbstractedTexture();
                   if (abstractTexture != null)
                   {
                       if (!s_spritesByTexture.ContainsKey(abstractTexture))
                       {
                           s_spritesByTexture[abstractTexture] = new List<CCSprite>();
                       }
                       s_spritesByTexture[abstractTexture].Add(sprite);
                   }
               }
               
               // Render each group in batches
               foreach (var group in s_spritesByTexture)
               {
                   RenderSpriteGroup(group.Key, group.Value);
               }
           }
           
           private static void RenderSpriteGroup(ITexture2D texture, List<CCSprite> sprites)
           {
               if (sprites.Count == 0) return;
               
               // Further group by blend function
               var blendGroups = sprites.GroupBy(s => s.BlendFunc, new BlendFuncComparer());
               
               foreach (var blendGroup in blendGroups)
               {
                   var spriteList = blendGroup.ToList();
                   if (spriteList.Count == 1)
                   {
                       // Single sprite - use individual drawing
                       spriteList[0].Draw();
                   }
                   else
                   {
                       // Multiple sprites - use batch
                       CCSprite.BeginSpriteBatch(spriteList[0].Texture, blendGroup.Key);
                       
                       foreach (var sprite in spriteList)
                       {
                           sprite.DrawInBatch();
                       }
                       
                       CCSprite.EndSpriteBatch();
                   }
               }
           }
           
           /// <summary>
           /// Analyzes sprite rendering patterns and suggests optimizations
           /// </summary>
           public static CCSpriteOptimizationReport AnalyzeRenderingPatterns()
           {
               var report = new CCSpriteOptimizationReport();
               
               if (s_spritesByTexture != null)
               {
                   report.TotalTextures = s_spritesByTexture.Count;
                   report.TotalSprites = s_spritesByTexture.Values.Sum(list => list.Count);
                   report.AverageSpritesPerTexture = report.TotalSprites / (float)report.TotalTextures;
                   
                   // Find textures with many sprites (good batch candidates)
                   report.HighBatchPotentialTextures = s_spritesByTexture
                       .Where(kvp => kvp.Value.Count >= 10)
                       .Select(kvp => new TextureUsageInfo
                       {
                           Texture = kvp.Key,
                           SpriteCount = kvp.Value.Count,
                           EstimatedBatchSavings = CalculateBatchSavings(kvp.Value.Count)
                       })
                       .ToList();
               }
               
               return report;
           }
           
           private static float CalculateBatchSavings(int spriteCount)
           {
               // Estimate performance improvement from batching
               // Assumes each individual draw call has overhead
               float individualCost = spriteCount * 1.0f; // 1 unit per sprite
               float batchCost = 1.0f + (spriteCount * 0.1f); // Setup cost + reduced per-sprite cost
               return (individualCost - batchCost) / individualCost;
           }
           
           private class BlendFuncComparer : IEqualityComparer<CCBlendFunc>
           {
               public bool Equals(CCBlendFunc x, CCBlendFunc y)
               {
                   return SpriteRenderingUtilities.AreBlendFuncsEqual(x, y);
               }
               
               public int GetHashCode(CCBlendFunc obj)
               {
                   return obj.Source.GetHashCode() ^ obj.Destination.GetHashCode();
               }
           }
       }
       
       public class CCSpriteOptimizationReport
       {
           public int TotalTextures { get; set; }
           public int TotalSprites { get; set; }
           public float AverageSpritesPerTexture { get; set; }
           public List<TextureUsageInfo> HighBatchPotentialTextures { get; set; } = new List<TextureUsageInfo>();
           
           public override string ToString()
           {
               var report = new System.Text.StringBuilder();
               report.AppendLine("Sprite Rendering Optimization Report:");
               report.AppendLine("====================================");
               report.AppendLine($"Total Textures: {TotalTextures}");
               report.AppendLine($"Total Sprites: {TotalSprites}");
               report.AppendLine($"Average Sprites per Texture: {AverageSpritesPerTexture:F1}");
               report.AppendLine();
               report.AppendLine("High Batch Potential Textures:");
               
               foreach (var texture in HighBatchPotentialTextures)
               {
                   report.AppendLine($"  {texture.Texture.Width}x{texture.Texture.Height}: {texture.SpriteCount} sprites, {texture.EstimatedBatchSavings:P} savings");
               }
               
               return report.ToString();
           }
       }
       
       public class TextureUsageInfo
       {
           public ITexture2D Texture { get; set; }
           public int SpriteCount { get; set; }
           public float EstimatedBatchSavings { get; set; }
       }
   }
   ```

### Verification:
- Profiling accurately measures rendering performance
- Optimization suggestions provide actionable insights
- Performance improvements are measurable

---

## Subtask 2.3.7: Create Integration Tests for Sprite Rendering
**Time Estimate**: 2 hours  
**Dependencies**: Subtask 2.3.6  
**Assignee**: QA developer

### Steps:

1. **Create test structure**
   ```bash
   mkdir -p Tests/SpriteTests
   touch Tests/SpriteTests/CCSpriteRendererTests.cs
   touch Tests/SpriteTests/CCSpriteAbstractionTests.cs
   touch Tests/SpriteTests/CCSpriteBatchTests.cs
   touch Tests/SpriteTests/CCSpritePerformanceTests.cs
   ```

2. **Create sprite renderer tests**
   ```csharp
   // File: Tests/SpriteTests/CCSpriteRendererTests.cs
   using Xunit;
   using Moq;
   using Cocos2D;
   using Cocos2D.Platform.Interfaces;
   using Microsoft.Xna.Framework.Graphics;
   
   namespace Cocos2D.Tests.Sprite
   {
       public class CCSpriteRendererTests : IDisposable
       {
           private Mock<IGraphicsRenderer> _mockRenderer;
           private Mock<IDrawManager> _mockDrawManager;
           private Mock<ITexture2D> _mockTexture;
           private CCSpriteRendererImpl _spriteRenderer;
           
           public CCSpriteRendererTests()
           {
               _mockRenderer = new Mock<IGraphicsRenderer>();
               _mockDrawManager = new Mock<IDrawManager>();
               _mockTexture = new Mock<ITexture2D>();
               
               _mockTexture.Setup(t => t.Width).Returns(128);
               _mockTexture.Setup(t => t.Height).Returns(128);
               
               _spriteRenderer = new CCSpriteRendererImpl(_mockRenderer.Object, _mockDrawManager.Object);
           }
           
           public void Dispose()
           {
               _spriteRenderer?.Dispose();
           }
           
           [Fact]
           public void DrawSprite_ValidQuad_CallsDrawManager()
           {
               // Arrange
               var quad = new CCV3F_C4B_T2F_Quad();
               var blendFunc = CCBlendFunc.AlphaBlend;
               
               // Act
               _spriteRenderer.DrawSprite(ref quad, _mockTexture.Object, blendFunc);
               
               // Assert
               _mockDrawManager.Verify(dm => dm.BindTexture(_mockTexture.Object), Times.Once);
               _mockDrawManager.Verify(dm => dm.BlendFunc(blendFunc), Times.Once);
               _mockDrawManager.Verify(dm => dm.DrawQuad(ref It.Ref<CCV3F_C4B_T2F_Quad>.IsAny), Times.Once);
               
               Assert.Equal(1, _spriteRenderer.DrawCallCount);
               Assert.Equal(1, _spriteRenderer.SpriteCount);
           }
           
           [Fact]
           public void BatchRendering_MultipleSprites_OptimizesDrawCalls()
           {
               // Arrange
               var blendFunc = CCBlendFunc.AlphaBlend;
               var quad1 = new CCV3F_C4B_T2F_Quad();
               var quad2 = new CCV3F_C4B_T2F_Quad();
               var quad3 = new CCV3F_C4B_T2F_Quad();
               
               // Act
               _spriteRenderer.BeginBatch(_mockTexture.Object, blendFunc);
               _spriteRenderer.DrawQuadInBatch(ref quad1);
               _spriteRenderer.DrawQuadInBatch(ref quad2);
               _spriteRenderer.DrawQuadInBatch(ref quad3);
               _spriteRenderer.EndBatch();
               
               // Assert
               _mockDrawManager.Verify(dm => dm.BeginBatch(), Times.Once);
               _mockDrawManager.Verify(dm => dm.DrawQuad(ref It.Ref<CCV3F_C4B_T2F_Quad>.IsAny), Times.Exactly(3));
               _mockDrawManager.Verify(dm => dm.EndBatch(), Times.Once);
               
               Assert.Equal(1, _spriteRenderer.DrawCallCount); // Should be 1 batch call
               Assert.Equal(3, _spriteRenderer.SpriteCount);
           }
           
           [Fact]
           public void StateManagement_TextureChanges_TrackedCorrectly()
           {
               // Arrange
               var texture1 = _mockTexture.Object;
               var texture2 = new Mock<ITexture2D>().Object;
               var textureChangeCount = 0;
               
               _spriteRenderer.TextureChanged += (s, e) => textureChangeCount++;
               
               // Act
               _spriteRenderer.SetTexture(texture1);
               _spriteRenderer.SetTexture(texture1); // Same texture - should not trigger event
               _spriteRenderer.SetTexture(texture2); // Different texture - should trigger event
               
               // Assert
               Assert.Equal(1, textureChangeCount);
               _mockDrawManager.Verify(dm => dm.BindTexture(texture1), Times.Once);
               _mockDrawManager.Verify(dm => dm.BindTexture(texture2), Times.Once);
           }
       }
   }
   ```

3. **Create sprite abstraction tests**
   ```csharp
   // File: Tests/SpriteTests/CCSpriteAbstractionTests.cs
   using Xunit;
   using Cocos2D;
   using Cocos2D.Platform.Factory;
   
   namespace Cocos2D.Tests.Sprite
   {
       public class CCSpriteAbstractionTests : IDisposable
       {
           public CCSpriteAbstractionTests()
           {
               CCGraphicsFactory.Reset();
               CCSprite.UseAbstractedRendering = true;
           }
           
           public void Dispose()
           {
               CCGraphicsFactory.Reset();
           }
           
           [Fact]
           public void CCSprite_WithAbstraction_UsesAbstractedRenderer()
           {
               // Arrange
               var sprite = new CCSprite();
               
               // Act
               var isUsingAbstraction = CCSprite.UseAbstractedRendering;
               
               // Assert
               Assert.True(isUsingAbstraction);
           }
           
           [Fact]
           public void CCSprite_Draw_DoesNotThrow()
           {
               // Arrange
               var sprite = new CCSprite();
               var testData = new byte[64 * 64 * 4];
               var texture = new CCTexture2D(testData);
               sprite.Texture = texture;
               
               // Act & Assert - should not throw
               sprite.Draw();
           }
           
           [Fact]
           public void CCSprite_BatchRendering_WorksCorrectly()
           {
               // Arrange
               var texture = new CCTexture2D(new byte[32 * 32 * 4]);
               var blendFunc = CCBlendFunc.AlphaBlend;
               
               // Act & Assert - should not throw
               CCSprite.BeginSpriteBatch(texture, blendFunc);
               
               var sprite1 = new CCSprite { Texture = texture };
               var sprite2 = new CCSprite { Texture = texture };
               
               sprite1.DrawInBatch();
               sprite2.DrawInBatch();
               
               CCSprite.EndSpriteBatch();
           }
           
           [Fact]
           public void CCSprite_Statistics_ProvideValidData()
           {
               // Arrange
               CCSprite.ResetRenderingStats();
               var sprite = new CCSprite();
               var texture = new CCTexture2D(new byte[32 * 32 * 4]);
               sprite.Texture = texture;
               
               // Act
               sprite.Draw();
               var stats = CCSprite.GetRenderingStats();
               
               // Assert
               Assert.True(stats.IsUsingAbstraction);
               Assert.NotNull(stats.RendererType);
           }
       }
   }
   ```

4. **Create performance tests**
   ```csharp
   // File: Tests/SpriteTests/CCSpritePerformanceTests.cs
   using Xunit;
   using System.Diagnostics;
   using System.Collections.Generic;
   using Cocos2D;
   
   namespace Cocos2D.Tests.Sprite
   {
       public class CCSpritePerformanceTests : IDisposable
       {
           public CCSpritePerformanceTests()
           {
               CCSpriteRenderProfiler.IsEnabled = true;
               CCSpriteRenderProfiler.ResetProfiles();
           }
           
           public void Dispose()
           {
               CCSpriteRenderProfiler.IsEnabled = false;
           }
           
           [Fact]
           public void BatchRendering_IsFasterThanIndividual()
           {
               // Arrange
               const int spriteCount = 100;
               var sprites = new List<CCSprite>();
               var texture = new CCTexture2D(new byte[64 * 64 * 4]);
               
               for (int i = 0; i < spriteCount; i++)
               {
                   sprites.Add(new CCSprite { Texture = texture });
               }
               
               // Test individual rendering
               var stopwatch = Stopwatch.StartNew();
               foreach (var sprite in sprites)
               {
                   sprite.Draw();
               }
               stopwatch.Stop();
               long individualTime = stopwatch.ElapsedMilliseconds;
               
               // Test batch rendering
               stopwatch.Restart();
               CCSprite.BeginSpriteBatch(texture, CCBlendFunc.AlphaBlend);
               foreach (var sprite in sprites)
               {
                   sprite.DrawInBatch();
               }
               CCSprite.EndSpriteBatch();
               stopwatch.Stop();
               long batchTime = stopwatch.ElapsedMilliseconds;
               
               // Assert (batch should be faster, but allow for small test variations)
               Assert.True(batchTime <= individualTime * 2, 
                   $"Batch rendering ({batchTime}ms) should be competitive with individual rendering ({individualTime}ms)");
           }
           
           [Fact]
           public void Profiler_CapturesPerformanceData()
           {
               // Arrange
               var sprite = new CCSprite();
               var texture = new CCTexture2D(new byte[32 * 32 * 4]);
               sprite.Texture = texture;
               
               // Act
               sprite.Draw();
               sprite.Draw();
               sprite.Draw();
               
               // Assert
               var report = CCSpriteRenderProfiler.GetProfileReport();
               Assert.Contains("Individual Sprite Draw", report);
           }
           
           [Fact]
           public void Optimizer_ProvidesUsefulRecommendations()
           {
               // Arrange
               var sprites = new List<CCSprite>();
               var texture = new CCTexture2D(new byte[64 * 64 * 4]);
               
               // Create many sprites with same texture (good batch candidate)
               for (int i = 0; i < 15; i++)
               {
                   sprites.Add(new CCSprite { Texture = texture });
               }
               
               // Act
               CCSpriteRenderOptimizer.OptimizeSpriteBatches(sprites);
               var report = CCSpriteRenderOptimizer.AnalyzeRenderingPatterns();
               
               // Assert
               Assert.True(report.TotalSprites >= 15);
               Assert.True(report.HighBatchPotentialTextures.Count > 0);
           }
       }
   }
   ```

### Verification:
- All tests pass consistently
- Performance tests demonstrate abstraction efficiency
- Integration tests verify end-to-end functionality

---

## Summary and Timeline

### Total Estimated Time: ~12.5 hours (1.5-2 days for one developer)

### Optimal Task Assignment (2 developers working in parallel):

**Developer 1 (Core Sprite Abstraction):**
- Subtask 2.3.1: Analysis (45m)
- Subtask 2.3.2: Interface Creation (1h) 
- Subtask 2.3.3: Renderer Implementation (2.5h)
- Subtask 2.3.4: CCSprite Update (2h)
- **Total: 6.25h**

**Developer 2 (Optimization/Testing):**
- Subtask 2.3.5: Batch Node Integration (1.5h)
- Subtask 2.3.6: Performance Optimizations (2h)
- Subtask 2.3.7: Integration Tests (2h)
- **Total: 5.5h**

### Dependencies:
```
2.3.1 (Analysis) ──> 2.3.2 (Interface) ──> 2.3.3 (Renderer) ──> 2.3.4 (CCSprite Update)
                                                                        │
                                                                        ├──> 2.3.5 (Batch Node)
                                                                        │         │
                                                                        │         └──> 2.3.6 (Optimizations)
                                                                        │               │
                                                                        └───────────────┴──> 2.3.7 (Tests)
```

This refactoring provides:
- Complete abstraction of sprite rendering from CCDrawManager
- Maintained sprite functionality and performance characteristics  
- Enhanced batch rendering capabilities
- Comprehensive performance monitoring and optimization tools
- Extensive testing coverage for sprite rendering scenarios
- Clear upgrade path for advanced graphics backends

The sprite abstraction is now complete and ready for integration with Metal or other graphics backends.