# Task 2.2: Refactor CCTexture2D - Detailed Breakdown

## Overview
Refactor CCTexture2D to use abstracted ITexture2D interface instead of direct MonoGame Texture2D dependency. This maintains all existing functionality while enabling backend flexibility.

---

## Subtask 2.2.1: Analyze CCTexture2D Current Dependencies
**Time Estimate**: 45 minutes  
**Dependencies**: Task 2.1 complete  
**Assignee**: Developer familiar with texture systems

### Steps:

1. **Audit current MonoGame dependencies**
   From `/cocos2d/textures/CCTexture2D.cs`:
   ```csharp
   // Line 60: Direct MonoGame dependency
   private Texture2D m_Texture2D;
   
   // Lines 100-120: Direct Texture2D property access
   public bool IsTextureDefined
   {
       get { return (m_Texture2D != null && !m_Texture2D.IsDisposed); }
   }
   
   public Texture2D XNATexture
   {
       get { return m_Texture2D; }
   }
   ```

2. **Identify all texture operations requiring abstraction**
   Key methods to analyze:
   - `InitWithData()` (lines 200-300): Texture creation from data
   - `InitWithFile()` (lines 400-500): Texture loading from file
   - `InitWithString()` (lines 600-700): Text texture creation
   - `SaveToFile()` (lines 800-850): Texture serialization
   - Property accessors for Width, Height, PixelFormat

3. **Document abstraction requirements**
   ```csharp
   /*
   CCTexture2D Abstraction Requirements:
   
   1. Replace Texture2D m_Texture2D with ITexture2D m_abstractTexture
   2. Maintain XNATexture property for backward compatibility
   3. Abstract texture creation operations
   4. Abstract data access (GetData<T>, SetData<T>)
   5. Abstract format and dimension queries
   6. Maintain texture caching functionality
   7. Abstract disposal patterns
   */
   ```

### Verification:
- Complete dependency analysis documented
- All abstraction requirements identified
- Impact on existing code assessed

---

## Subtask 2.2.2: Create CCTexture2D Interface Extension
**Time Estimate**: 1 hour  
**Dependencies**: Subtask 2.2.1  
**Assignee**: Senior developer

### Steps:

1. **Extend ITexture2D interface for cocos2d-specific features**
   ```csharp
   // File: cocos2d/platform/Interfaces/ICCTexture2D.cs
   using System;
   using Microsoft.Xna.Framework;
   using Microsoft.Xna.Framework.Graphics;
   
   namespace Cocos2D.Platform.Interfaces
   {
       /// <summary>
       /// Extended texture interface with cocos2d-specific functionality
       /// </summary>
       public interface ICCTexture2D : ITexture2D
       {
           // Cocos2D-specific properties
           CCSize ContentSize { get; }
           int PixelsWide { get; }
           int PixelsHigh { get; }
           bool HasPremultipliedAlpha { get; }
           bool HasMipmaps { get; }
           SamplerState SamplerState { get; set; }
           
           // Cocos2D-specific operations
           bool InitWithData(byte[] data, SurfaceFormat pixelFormat, bool mipMap = false);
           bool InitWithFile(string fileName);
           bool InitWithString(string text, CCSize dimensions, CCTextAlignment hAlignment, 
                             CCVerticalTextAlignment vAlignment, string fontName, float fontSize);
           
           void SaveToFile(string fileName, CCImageFormat format);
           void Reinit();
           
           // Cache management
           CCTextureCacheInfo CacheInfo { get; set; }
           
           // Anti-aliasing
           bool IsAntialiased { get; set; }
           
           // Events
           Action OnReInit { get; set; }
       }
   }
   ```

2. **Create texture factory methods**
   ```csharp
   // Add to CCGraphicsFactory.cs
   /// <summary>
   /// Creates a CCTexture2D using the current graphics renderer
   /// </summary>
   public static ICCTexture2D CreateCCTexture2D()
   {
       var renderer = GetCurrentRenderer() ?? CreateRenderer();
       return renderer.CreateCCTexture2D();
   }
   
   /// <summary>
   /// Creates a CCTexture2D from file using the current graphics renderer
   /// </summary>
   public static ICCTexture2D CreateCCTexture2D(string fileName)
   {
       var texture = CreateCCTexture2D();
       if (!texture.InitWithFile(fileName))
       {
           texture.Dispose();
           return null;
       }
       return texture;
   }
   
   /// <summary>
   /// Creates a CCTexture2D from data using the current graphics renderer
   /// </summary>
   public static ICCTexture2D CreateCCTexture2D(byte[] data, SurfaceFormat format, bool mipMap = false)
   {
       var texture = CreateCCTexture2D();
       if (!texture.InitWithData(data, format, mipMap))
       {
           texture.Dispose();
           return null;
       }
       return texture;
   }
   ```

### Verification:
- Extended interface covers all CCTexture2D functionality
- Factory methods integrate with graphics factory
- Interface compiles successfully

---

## Subtask 2.2.3: Implement Abstracted CCTexture2D Core
**Time Estimate**: 2.5 hours  
**Dependencies**: Subtask 2.2.2  
**Assignee**: Graphics programmer

### Steps:

1. **Create new CCTexture2DImpl class**
   ```csharp
   // File: cocos2d/platform/CCTexture2DImpl.cs
   using System;
   using System.IO;
   using Microsoft.Xna.Framework;
   using Microsoft.Xna.Framework.Graphics;
   using Cocos2D.Platform.Interfaces;
   
   namespace Cocos2D
   {
       /// <summary>
       /// Implementation of ICCTexture2D using abstracted graphics interfaces
       /// </summary>
       internal class CCTexture2DImpl : ICCTexture2D
       {
           private ITexture2D _abstractTexture;
           private IGraphicsRenderer _renderer;
           private CCTextureCacheInfo _cacheInfo;
           private SamplerState _samplerState;
           private bool _hasPremultipliedAlpha;
           private bool _hasMipmaps;
           private bool _isAntialiased = true;
           private bool _disposed = false;
           
           // Cocos2D-specific properties
           private CCSize _contentSize;
           private int _pixelsWide;
           private int _pixelsHigh;
           
           public CCTexture2DImpl(IGraphicsRenderer renderer)
           {
               _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
               _samplerState = SamplerState.LinearClamp;
               RefreshAntialiasSetting();
           }
           
           // ICCTexture2D Implementation
           public CCSize ContentSize => _contentSize;
           public int PixelsWide => _pixelsWide;
           public int PixelsHigh => _pixelsHigh;
           public bool HasPremultipliedAlpha => _hasPremultipliedAlpha;
           public bool HasMipmaps => _hasMipmaps;
           
           public SamplerState SamplerState
           {
               get => _samplerState;
               set
               {
                   _samplerState = value;
                   RefreshAntialiasSetting();
               }
           }
           
           public bool IsAntialiased
           {
               get => _isAntialiased;
               set
               {
                   _isAntialiased = value;
                   RefreshAntialiasSetting();
               }
           }
           
           public CCTextureCacheInfo CacheInfo
           {
               get => _cacheInfo;
               set => _cacheInfo = value;
           }
           
           public Action OnReInit { get; set; }
           
           // ITexture2D Implementation
           public int Width => _abstractTexture?.Width ?? 0;
           public int Height => _abstractTexture?.Height ?? 0;
           public SurfaceFormat Format => _abstractTexture?.Format ?? SurfaceFormat.Color;
           public int LevelCount => _abstractTexture?.LevelCount ?? 1;
           
           private void RefreshAntialiasSetting()
           {
               if (_isAntialiased)
               {
                   _samplerState = SamplerState.LinearClamp;
               }
               else
               {
                   _samplerState = SamplerState.PointClamp;
               }
           }
       }
   }
   ```

2. **Implement texture initialization methods**
   ```csharp
   public bool InitWithData(byte[] data, SurfaceFormat pixelFormat, bool mipMap = false)
   {
       if (data == null || data.Length == 0)
       {
           CCLog.Log("CCTexture2D.InitWithData: Invalid data provided");
           return false;
       }
       
       try
       {
           // Dispose existing texture
           _abstractTexture?.Dispose();
           
           // Create texture through renderer abstraction
           _abstractTexture = _renderer.CreateTexture2DFromData(data, pixelFormat, mipMap);
           
           if (_abstractTexture == null)
           {
               CCLog.Log("CCTexture2D.InitWithData: Failed to create texture from data");
               return false;
           }
           
           // Update properties
           _pixelsWide = _abstractTexture.Width;
           _pixelsHigh = _abstractTexture.Height;
           _contentSize = new CCSize(_pixelsWide, _pixelsHigh);
           _hasMipmaps = mipMap;
           _hasPremultipliedAlpha = CCTexture2D.OptimizeForPremultipliedAlpha;
           
           // Update cache info
           _cacheInfo = new CCTextureCacheInfo
           {
               CacheType = CCTextureCacheType.Data,
               Data = data
           };
           
           CCLog.Log($"CCTexture2D initialized from data: {_pixelsWide}x{_pixelsHigh}, format: {pixelFormat}");
           return true;
       }
       catch (Exception ex)
       {
           CCLog.Log($"CCTexture2D.InitWithData failed: {ex.Message}");
           return false;
       }
   }
   
   public bool InitWithFile(string fileName)
   {
       if (string.IsNullOrEmpty(fileName))
       {
           CCLog.Log("CCTexture2D.InitWithFile: Invalid file name");
           return false;
       }
       
       try
       {
           // Dispose existing texture
           _abstractTexture?.Dispose();
           
           // Load texture through renderer abstraction
           _abstractTexture = _renderer.LoadTexture2DFromFile(fileName);
           
           if (_abstractTexture == null)
           {
               CCLog.Log($"CCTexture2D.InitWithFile: Failed to load texture from {fileName}");
               return false;
           }
           
           // Update properties
           _pixelsWide = _abstractTexture.Width;
           _pixelsHigh = _abstractTexture.Height;
           _contentSize = new CCSize(_pixelsWide, _pixelsHigh);
           _hasMipmaps = _abstractTexture.LevelCount > 1;
           _hasPremultipliedAlpha = CCTexture2D.OptimizeForPremultipliedAlpha;
           
           // Update cache info
           _cacheInfo = new CCTextureCacheInfo
           {
               CacheType = CCTextureCacheType.AssetFile,
               Data = fileName
           };
           
           CCLog.Log($"CCTexture2D loaded from file: {fileName} ({_pixelsWide}x{_pixelsHigh})");
           return true;
       }
       catch (Exception ex)
       {
           CCLog.Log($"CCTexture2D.InitWithFile failed for {fileName}: {ex.Message}");
           return false;
       }
   }
   ```

3. **Implement text texture creation**
   ```csharp
   public bool InitWithString(string text, CCSize dimensions, CCTextAlignment hAlignment, 
                            CCVerticalTextAlignment vAlignment, string fontName, float fontSize)
   {
       if (string.IsNullOrEmpty(text))
       {
           CCLog.Log("CCTexture2D.InitWithString: Empty text provided");
           return false;
       }
       
       try
       {
           // Dispose existing texture
           _abstractTexture?.Dispose();
           
           // Create text texture through renderer abstraction
           var textureData = _renderer.CreateTextTexture(text, fontName, fontSize, 
                                                       dimensions, hAlignment, vAlignment);
           
           if (textureData.Data == null)
           {
               CCLog.Log("CCTexture2D.InitWithString: Failed to create text texture");
               return false;
           }
           
           // Create texture from generated data
           _abstractTexture = _renderer.CreateTexture2DFromData(textureData.Data, 
                                                              textureData.Format, 
                                                              false);
           
           if (_abstractTexture == null)
           {
               CCLog.Log("CCTexture2D.InitWithString: Failed to create texture from text data");
               return false;
           }
           
           // Update properties
           _pixelsWide = _abstractTexture.Width;
           _pixelsHigh = _abstractTexture.Height;
           _contentSize = new CCSize(textureData.ContentWidth, textureData.ContentHeight);
           _hasMipmaps = false;
           _hasPremultipliedAlpha = true;
           
           // Update cache info
           _cacheInfo = new CCTextureCacheInfo
           {
               CacheType = CCTextureCacheType.String,
               Data = new CCStringCache
               {
                   Text = text,
                   Dimensions = dimensions,
                   HAlignment = hAlignment,
                   VAlignment = vAlignment,
                   FontName = fontName,
                   FontSize = fontSize
               }
           };
           
           CCLog.Log($"CCTexture2D created from string: '{text.Substring(0, Math.Min(text.Length, 20))}...' ({_pixelsWide}x{_pixelsHigh})");
           return true;
       }
       catch (Exception ex)
       {
           CCLog.Log($"CCTexture2D.InitWithString failed: {ex.Message}");
           return false;
       }
   }
   ```

### Verification:
- Core implementation compiles successfully
- Initialization methods work with abstracted interfaces
- Properties are correctly maintained

---

## Subtask 2.2.4: Implement Advanced Texture Operations
**Time Estimate**: 2 hours  
**Dependencies**: Subtask 2.2.3  
**Assignee**: Graphics programmer

### Steps:

1. **Implement data access methods**
   ```csharp
   public void GetData<T>(T[] data) where T : struct
   {
       if (_abstractTexture == null)
           throw new InvalidOperationException("Texture not initialized");
       
       _abstractTexture.GetData(data);
   }
   
   public void GetData<T>(T[] data, int startIndex, int elementCount) where T : struct
   {
       if (_abstractTexture == null)
           throw new InvalidOperationException("Texture not initialized");
       
       _abstractTexture.GetData(data, startIndex, elementCount);
   }
   
   public void GetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct
   {
       if (_abstractTexture == null)
           throw new InvalidOperationException("Texture not initialized");
       
       _abstractTexture.GetData(level, rect, data, startIndex, elementCount);
   }
   
   public void SetData<T>(T[] data) where T : struct
   {
       if (_abstractTexture == null)
           throw new InvalidOperationException("Texture not initialized");
       
       _abstractTexture.SetData(data);
   }
   
   public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
   {
       if (_abstractTexture == null)
           throw new InvalidOperationException("Texture not initialized");
       
       _abstractTexture.SetData(data, startIndex, elementCount);
   }
   
   public void SetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct
   {
       if (_abstractTexture == null)
           throw new InvalidOperationException("Texture not initialized");
       
       _abstractTexture.SetData(level, rect, data, startIndex, elementCount);
   }
   ```

2. **Implement texture saving and export**
   ```csharp
   public void SaveToFile(string fileName, CCImageFormat format)
   {
       if (_abstractTexture == null)
           throw new InvalidOperationException("Texture not initialized");
       
       if (string.IsNullOrEmpty(fileName))
           throw new ArgumentException("Invalid file name", nameof(fileName));
       
       try
       {
           // Get texture data
           var data = new Color[_pixelsWide * _pixelsHigh];
           _abstractTexture.GetData(data);
           
           // Save through renderer abstraction
           _renderer.SaveTextureToFile(_abstractTexture, fileName, format);
           
           CCLog.Log($"Texture saved to: {fileName}");
       }
       catch (Exception ex)
       {
           CCLog.Log($"Failed to save texture to {fileName}: {ex.Message}");
           throw;
       }
   }
   
   public byte[] GetTextureData()
   {
       if (_abstractTexture == null)
           return null;
       
       try
       {
           var colorData = new Color[_pixelsWide * _pixelsHigh];
           _abstractTexture.GetData(colorData);
           
           // Convert Color[] to byte[]
           var byteData = new byte[colorData.Length * 4];
           for (int i = 0; i < colorData.Length; i++)
           {
               byteData[i * 4] = colorData[i].R;
               byteData[i * 4 + 1] = colorData[i].G;
               byteData[i * 4 + 2] = colorData[i].B;
               byteData[i * 4 + 3] = colorData[i].A;
           }
           
           return byteData;
       }
       catch (Exception ex)
       {
           CCLog.Log($"Failed to get texture data: {ex.Message}");
           return null;
       }
   }
   ```

3. **Implement reinitilization and caching**
   ```csharp
   public void Reinit()
   {
       if (_cacheInfo.CacheType == CCTextureCacheType.None)
       {
           CCLog.Log("Cannot reinitialize texture - no cache info available");
           return;
       }
       
       try
       {
           bool success = false;
           
           switch (_cacheInfo.CacheType)
           {
               case CCTextureCacheType.AssetFile:
                   success = InitWithFile((string)_cacheInfo.Data);
                   break;
                   
               case CCTextureCacheType.Data:
                   success = InitWithData((byte[])_cacheInfo.Data, Format, _hasMipmaps);
                   break;
                   
               case CCTextureCacheType.String:
                   var stringCache = (CCStringCache)_cacheInfo.Data;
                   success = InitWithString(stringCache.Text, stringCache.Dimensions,
                                          stringCache.HAlignment, stringCache.VAlignment,
                                          stringCache.FontName, stringCache.FontSize);
                   break;
           }
           
           if (success)
           {
               OnReInit?.Invoke();
               CCLog.Log("Texture reinitialized successfully");
           }
           else
           {
               CCLog.Log("Failed to reinitialize texture");
           }
       }
       catch (Exception ex)
       {
           CCLog.Log($"Error during texture reinitialization: {ex.Message}");
       }
   }
   
   public bool IsTextureDefined => _abstractTexture != null && !_disposed;
   
   public void Dispose()
   {
       if (!_disposed)
       {
           _abstractTexture?.Dispose();
           _abstractTexture = null;
           _disposed = true;
           CCLog.Log("CCTexture2D disposed");
       }
   }
   ```

### Verification:
- Data access methods work correctly
- Texture saving functionality works
- Reinitialization maintains state properly

---

## Subtask 2.2.5: Update Original CCTexture2D Class
**Time Estimate**: 2 hours  
**Dependencies**: Subtask 2.2.4  
**Assignee**: Senior developer

### Steps:

1. **Modify CCTexture2D to use abstraction**
   ```csharp
   // File: cocos2d/textures/CCTexture2D.cs
   // Replace private field (line 60):
   // OLD: private Texture2D m_Texture2D;
   // NEW:
   private ICCTexture2D m_abstractTexture;
   private bool m_usingAbstraction = true;
   
   // Backward compatibility - maintain MonoGame access
   private Texture2D m_legacyTexture2D;
   
   /// <summary>
   /// Gets the underlying MonoGame texture for backward compatibility
   /// </summary>
   public Texture2D XNATexture
   {
       get
       {
           // If using abstraction, try to get MonoGame texture
           if (m_usingAbstraction && m_abstractTexture != null)
           {
               if (m_abstractTexture is IMonoGameTexture2D mgTexture)
               {
                   return mgTexture.UnderlyingTexture;
               }
           }
           
           // Fallback to legacy texture
           return m_legacyTexture2D;
       }
   }
   
   /// <summary>
   /// Gets the abstracted texture interface
   /// </summary>
   public ICCTexture2D AbstractTexture => m_abstractTexture;
   ```

2. **Update constructor and initialization**
   ```csharp
   public CCTexture2D()
   {
       // Try to use abstraction by default
       try
       {
           m_abstractTexture = CCGraphicsFactory.CreateCCTexture2D();
           m_usingAbstraction = true;
       }
       catch (Exception ex)
       {
           CCLog.Log($"Failed to create abstracted texture, falling back to legacy: {ex.Message}");
           m_usingAbstraction = false;
       }
       
       m_samplerState = SamplerState.LinearClamp;
       IsAntialiased = true;
       RefreshAntialiasSetting();
   }
   
   public CCTexture2D(string fileName) : this()
   {
       if (!InitWithFile(fileName))
       {
           CCLog.Log("CCTexture2D (string fileName): Problems initializing class");
       }
   }
   
   public CCTexture2D(byte[] data, SurfaceFormat pixelFormat = SurfaceFormat.Color, bool mipMap = false) : this()
   {
       if (!InitWithData(data, pixelFormat, mipMap))
       {
           CCLog.Log("CCTexture2D: Problems initializing class");
       }
   }
   ```

3. **Update property implementations**
   ```csharp
   public bool IsTextureDefined
   {
       get
       {
           if (m_usingAbstraction)
           {
               return m_abstractTexture?.IsTextureDefined ?? false;
           }
           return m_legacyTexture2D != null && !m_legacyTexture2D.IsDisposed;
       }
   }
   
   public CCSize ContentSize
   {
       get
       {
           if (m_usingAbstraction)
           {
               return m_abstractTexture?.ContentSize ?? CCSize.Zero;
           }
           return m_tContentSize;
       }
   }
   
   public int PixelsWide
   {
       get
       {
           if (m_usingAbstraction)
           {
               return m_abstractTexture?.PixelsWide ?? 0;
           }
           return m_uPixelsWide;
       }
   }
   
   public int PixelsHigh
   {
       get
       {
           if (m_usingAbstraction)
           {
               return m_abstractTexture?.PixelsHigh ?? 0;
           }
           return m_uPixelsHigh;
       }
   }
   
   public SurfaceFormat PixelFormat
   {
       get
       {
           if (m_usingAbstraction)
           {
               return m_abstractTexture?.Format ?? SurfaceFormat.Color;
           }
           return m_ePixelFormat;
       }
   }
   
   public bool HasPremultipliedAlpha
   {
       get
       {
           if (m_usingAbstraction)
           {
               return m_abstractTexture?.HasPremultipliedAlpha ?? false;
           }
           return m_bHasPremultipliedAlpha;
       }
   }
   ```

4. **Update initialization methods**
   ```csharp
   public bool InitWithFile(string fileName)
   {
       if (m_usingAbstraction)
       {
           return m_abstractTexture?.InitWithFile(fileName) ?? false;
       }
       
       // Legacy implementation fallback
       return InitWithFileLegacy(fileName);
   }
   
   public bool InitWithData(byte[] data, SurfaceFormat pixelFormat = SurfaceFormat.Color, bool mipMap = false)
   {
       if (m_usingAbstraction)
       {
           return m_abstractTexture?.InitWithData(data, pixelFormat, mipMap) ?? false;
       }
       
       // Legacy implementation fallback
       return InitWithDataLegacy(data, pixelFormat, mipMap);
   }
   
   public bool InitWithString(string text, CCSize dimensions, CCTextAlignment hAlignment, 
                            CCVerticalTextAlignment vAlignment, string fontName, float fontSize)
   {
       if (m_usingAbstraction)
       {
           return m_abstractTexture?.InitWithString(text, dimensions, hAlignment, vAlignment, fontName, fontSize) ?? false;
       }
       
       // Legacy implementation fallback
       return InitWithStringLegacy(text, dimensions, hAlignment, vAlignment, fontName, fontSize);
   }
   ```

### Verification:
- Original CCTexture2D API unchanged
- Abstraction layer working correctly
- Backward compatibility maintained

---

## Subtask 2.2.6: Create Legacy Fallback Methods
**Time Estimate**: 1.5 hours  
**Dependencies**: Subtask 2.2.5  
**Assignee**: Developer familiar with CCTexture2D

### Steps:

1. **Extract current implementation as legacy methods**
   ```csharp
   // File: cocos2d/textures/CCTexture2D.Legacy.cs
   using System;
   using System.IO;
   using Microsoft.Xna.Framework;
   using Microsoft.Xna.Framework.Graphics;
   
   namespace Cocos2D
   {
       public partial class CCTexture2D
       {
           /// <summary>
           /// Legacy MonoGame-based file loading for fallback
           /// </summary>
           private bool InitWithFileLegacy(string fileName)
           {
               try
               {
                   // Use current CCTexture2D implementation logic
                   // (Move existing InitWithFile implementation here)
                   
                   var device = CCDrawManager.graphicsDevice;
                   if (device == null)
                   {
                       CCLog.Log("InitWithFileLegacy: GraphicsDevice not available");
                       return false;
                   }
                   
                   using (var stream = CCContentManager.SharedContentManager.GetAssetStream(fileName))
                   {
                       if (stream == null)
                       {
                           CCLog.Log($"InitWithFileLegacy: Could not load {fileName}");
                           return false;
                       }
                       
                       m_legacyTexture2D = Texture2D.FromStream(device, stream);
                   }
                   
                   if (m_legacyTexture2D == null)
                   {
                       return false;
                   }
                   
                   // Update legacy properties
                   m_uPixelsWide = m_legacyTexture2D.Width;
                   m_uPixelsHigh = m_legacyTexture2D.Height;
                   m_tContentSize = new CCSize(m_uPixelsWide, m_uPixelsHigh);
                   m_ePixelFormat = m_legacyTexture2D.Format;
                   m_bHasPremultipliedAlpha = OptimizeForPremultipliedAlpha;
                   
                   // Update cache info
                   m_CacheInfo = new CCTextureCacheInfo
                   {
                       CacheType = CCTextureCacheType.AssetFile,
                       Data = fileName
                   };
                   
                   return true;
               }
               catch (Exception ex)
               {
                   CCLog.Log($"InitWithFileLegacy failed for {fileName}: {ex.Message}");
                   return false;
               }
           }
           
           /// <summary>
           /// Legacy MonoGame-based data initialization for fallback
           /// </summary>
           private bool InitWithDataLegacy(byte[] data, SurfaceFormat pixelFormat, bool mipMap)
           {
               try
               {
                   var device = CCDrawManager.graphicsDevice;
                   if (device == null)
                   {
                       CCLog.Log("InitWithDataLegacy: GraphicsDevice not available");
                       return false;
                   }
                   
                   // Determine texture dimensions from data
                   int width, height;
                   if (!DetermineTextureDimensions(data, pixelFormat, out width, out height))
                   {
                       CCLog.Log("InitWithDataLegacy: Could not determine texture dimensions");
                       return false;
                   }
                   
                   m_legacyTexture2D = new Texture2D(device, width, height, mipMap, pixelFormat);
                   m_legacyTexture2D.SetData(data);
                   
                   // Update legacy properties
                   m_uPixelsWide = width;
                   m_uPixelsHigh = height;
                   m_tContentSize = new CCSize(width, height);
                   m_ePixelFormat = pixelFormat;
                   m_bHasMipmaps = mipMap;
                   m_bHasPremultipliedAlpha = OptimizeForPremultipliedAlpha;
                   
                   // Update cache info
                   m_CacheInfo = new CCTextureCacheInfo
                   {
                       CacheType = CCTextureCacheType.Data,
                       Data = data
                   };
                   
                   return true;
               }
               catch (Exception ex)
               {
                   CCLog.Log($"InitWithDataLegacy failed: {ex.Message}");
                   return false;
               }
           }
           
           /// <summary>
           /// Legacy MonoGame-based string rendering for fallback
           /// </summary>
           private bool InitWithStringLegacy(string text, CCSize dimensions, CCTextAlignment hAlignment, 
                                          CCVerticalTextAlignment vAlignment, string fontName, float fontSize)
           {
               try
               {
                   // Use existing string-to-texture implementation
                   var device = CCDrawManager.graphicsDevice;
                   if (device == null)
                   {
                       CCLog.Log("InitWithStringLegacy: GraphicsDevice not available");
                       return false;
                   }
                   
                   // Create render target for text rendering
                   var renderTarget = new RenderTarget2D(device, (int)dimensions.Width, (int)dimensions.Height);
                   
                   // Render text to target using existing logic
                   // (Implementation details would use existing CCTexture2D text rendering)
                   
                   m_legacyTexture2D = renderTarget;
                   
                   // Update legacy properties
                   m_uPixelsWide = (int)dimensions.Width;
                   m_uPixelsHigh = (int)dimensions.Height;
                   m_tContentSize = dimensions;
                   m_ePixelFormat = SurfaceFormat.Color;
                   m_bHasPremultipliedAlpha = true;
                   
                   return true;
               }
               catch (Exception ex)
               {
                   CCLog.Log($"InitWithStringLegacy failed: {ex.Message}");
                   return false;
               }
           }
           
           private bool DetermineTextureDimensions(byte[] data, SurfaceFormat format, out int width, out int height)
           {
               // Simple heuristic based on data length and format
               int bytesPerPixel = GetBytesPerPixel(format);
               int totalPixels = data.Length / bytesPerPixel;
               
               // Assume square texture for simplicity, or use common ratios
               width = (int)Math.Sqrt(totalPixels);
               height = totalPixels / width;
               
               return width > 0 && height > 0 && (width * height * bytesPerPixel) == data.Length;
           }
           
           private int GetBytesPerPixel(SurfaceFormat format)
           {
               switch (format)
               {
                   case SurfaceFormat.Color:
                   case SurfaceFormat.Bgra32:
                       return 4;
                   case SurfaceFormat.Bgr565:
                   case SurfaceFormat.Bgra5551:
                   case SurfaceFormat.Bgra4444:
                       return 2;
                   case SurfaceFormat.Alpha8:
                       return 1;
                   default:
                       return 4; // Default assumption
               }
           }
       }
   }
   ```

### Verification:
- Legacy methods compile successfully
- Fallback functionality works correctly
- No regression in existing behavior

---

## Subtask 2.2.7: Create Migration and Compatibility Layer
**Time Estimate**: 1 hour  
**Dependencies**: Subtask 2.2.6  
**Assignee**: Any developer

### Steps:

1. **Create texture migration utilities**
   ```csharp
   // File: cocos2d/platform/CCTexture2DMigration.cs
   using System;
   using Cocos2D.Platform.Interfaces;
   using Microsoft.Xna.Framework.Graphics;
   
   namespace Cocos2D
   {
       /// <summary>
       /// Helper methods for migrating texture code to use abstraction
       /// </summary>
       public static class CCTexture2DMigration
       {
           /// <summary>
           /// Creates a CCTexture2D using the abstraction layer
           /// </summary>
           public static CCTexture2D CreateAbstractedTexture()
           {
               return new CCTexture2D();
           }
           
           /// <summary>
           /// Creates a CCTexture2D from MonoGame Texture2D for compatibility
           /// </summary>
           public static CCTexture2D CreateFromMonoGameTexture(Texture2D texture)
           {
               if (texture == null) return null;
               
               var ccTexture = new CCTexture2D();
               
               // If using abstraction, wrap the MonoGame texture
               if (ccTexture.m_usingAbstraction)
               {
                   ccTexture.m_abstractTexture = new MonoGameTexture2DWrapper(texture);
               }
               else
               {
                   ccTexture.m_legacyTexture2D = texture;
                   ccTexture.m_uPixelsWide = texture.Width;
                   ccTexture.m_uPixelsHigh = texture.Height;
                   ccTexture.m_tContentSize = new CCSize(texture.Width, texture.Height);
                   ccTexture.m_ePixelFormat = texture.Format;
               }
               
               return ccTexture;
           }
           
           /// <summary>
           /// Checks if a CCTexture2D is using the abstraction layer
           /// </summary>
           public static bool IsUsingAbstraction(this CCTexture2D texture)
           {
               return texture.m_usingAbstraction;
           }
           
           /// <summary>
           /// Forces a CCTexture2D to use legacy MonoGame implementation
           /// </summary>
           public static void ForceLegacyMode(this CCTexture2D texture)
           {
               if (texture.m_usingAbstraction && texture.m_abstractTexture != null)
               {
                   // Migrate from abstraction to legacy
                   if (texture.m_abstractTexture is IMonoGameTexture2D mgTexture)
                   {
                       texture.m_legacyTexture2D = mgTexture.UnderlyingTexture;
                   }
                   
                   texture.m_abstractTexture?.Dispose();
                   texture.m_abstractTexture = null;
                   texture.m_usingAbstraction = false;
               }
           }
       }
   }
   ```

2. **Create wrapper for MonoGame textures**
   ```csharp
   // File: cocos2d/platform/MonoGame/MonoGameTexture2DWrapper.cs
   using System;
   using Microsoft.Xna.Framework;
   using Microsoft.Xna.Framework.Graphics;
   using Cocos2D.Platform.Interfaces;
   
   namespace Cocos2D
   {
       /// <summary>
       /// Wraps a MonoGame Texture2D to implement ICCTexture2D interface
       /// </summary>
       internal class MonoGameTexture2DWrapper : ICCTexture2D, IMonoGameTexture2D
       {
           private Texture2D _texture;
           private CCSize _contentSize;
           private SamplerState _samplerState;
           private bool _disposed = false;
           
           public MonoGameTexture2DWrapper(Texture2D texture)
           {
               _texture = texture ?? throw new ArgumentNullException(nameof(texture));
               _contentSize = new CCSize(texture.Width, texture.Height);
               _samplerState = SamplerState.LinearClamp;
           }
           
           // ICCTexture2D Implementation
           public CCSize ContentSize => _contentSize;
           public int PixelsWide => _texture.Width;
           public int PixelsHigh => _texture.Height;
           public bool HasPremultipliedAlpha => true; // MonoGame default
           public bool HasMipmaps => _texture.LevelCount > 1;
           public bool IsAntialiased { get; set; } = true;
           public SamplerState SamplerState { get => _samplerState; set => _samplerState = value; }
           public CCTextureCacheInfo CacheInfo { get; set; }
           public Action OnReInit { get; set; }
           
           // ITexture2D Implementation
           public int Width => _texture.Width;
           public int Height => _texture.Height;
           public SurfaceFormat Format => _texture.Format;
           public int LevelCount => _texture.LevelCount;
           
           // IMonoGameTexture2D Implementation
           public Texture2D UnderlyingTexture => _texture;
           
           public bool IsTextureDefined => _texture != null && !_texture.IsDisposed;
           
           // Methods that aren't supported in wrapper mode
           public bool InitWithData(byte[] data, SurfaceFormat pixelFormat, bool mipMap = false)
           {
               throw new NotSupportedException("Cannot reinitialize wrapped MonoGame texture");
           }
           
           public bool InitWithFile(string fileName)
           {
               throw new NotSupportedException("Cannot reinitialize wrapped MonoGame texture");
           }
           
           public bool InitWithString(string text, CCSize dimensions, CCTextAlignment hAlignment, 
                                     CCVerticalTextAlignment vAlignment, string fontName, float fontSize)
           {
               throw new NotSupportedException("Cannot reinitialize wrapped MonoGame texture");
           }
           
           public void SaveToFile(string fileName, CCImageFormat format)
           {
               throw new NotSupportedException("Save not supported for wrapped MonoGame texture");
           }
           
           public void Reinit()
           {
               // No-op for wrapped textures
           }
           
           // Data access delegates to underlying texture
           public void GetData<T>(T[] data) where T : struct => _texture.GetData(data);
           public void GetData<T>(T[] data, int startIndex, int elementCount) where T : struct => _texture.GetData(data, startIndex, elementCount);
           public void GetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct => _texture.GetData(level, rect, data, startIndex, elementCount);
           public void SetData<T>(T[] data) where T : struct => _texture.SetData(data);
           public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct => _texture.SetData(data, startIndex, elementCount);
           public void SetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct => _texture.SetData(level, rect, data, startIndex, elementCount);
           
           public void Dispose()
           {
               if (!_disposed)
               {
                   // Don't dispose the underlying texture - it's owned by something else
                   _texture = null;
                   _disposed = true;
               }
           }
       }
   }
   ```

### Verification:
- Migration utilities work correctly
- Wrapper implementation functional
- Compatibility maintained

---

## Subtask 2.2.8: Create Unit Tests for Refactored CCTexture2D
**Time Estimate**: 2 hours  
**Dependencies**: Subtask 2.2.7  
**Assignee**: QA developer

### Steps:

1. **Create test structure**
   ```bash
   mkdir -p Tests/TextureTests
   touch Tests/TextureTests/CCTexture2DImplTests.cs
   touch Tests/TextureTests/CCTexture2DCompatibilityTests.cs
   touch Tests/TextureTests/CCTexture2DMigrationTests.cs
   ```

2. **Create functionality tests**
   ```csharp
   // File: Tests/TextureTests/CCTexture2DImplTests.cs
   using Xunit;
   using Moq;
   using Cocos2D;
   using Cocos2D.Platform.Interfaces;
   using Microsoft.Xna.Framework.Graphics;
   
   namespace Cocos2D.Tests.Texture
   {
       public class CCTexture2DImplTests : IDisposable
       {
           private Mock<IGraphicsRenderer> _mockRenderer;
           private Mock<ITexture2D> _mockTexture;
           private CCTexture2DImpl _textureImpl;
           
           public CCTexture2DImplTests()
           {
               _mockRenderer = new Mock<IGraphicsRenderer>();
               _mockTexture = new Mock<ITexture2D>();
               
               _mockTexture.Setup(t => t.Width).Returns(256);
               _mockTexture.Setup(t => t.Height).Returns(256);
               _mockTexture.Setup(t => t.Format).Returns(SurfaceFormat.Color);
               
               _textureImpl = new CCTexture2DImpl(_mockRenderer.Object);
           }
           
           public void Dispose()
           {
               _textureImpl?.Dispose();
           }
           
           [Fact]
           public void InitWithData_ValidData_ReturnsTrue()
           {
               // Arrange
               var testData = new byte[256 * 256 * 4];
               _mockRenderer.Setup(r => r.CreateTexture2DFromData(testData, SurfaceFormat.Color, false))
                          .Returns(_mockTexture.Object);
               
               // Act
               var result = _textureImpl.InitWithData(testData, SurfaceFormat.Color, false);
               
               // Assert
               Assert.True(result);
               Assert.Equal(256, _textureImpl.PixelsWide);
               Assert.Equal(256, _textureImpl.PixelsHigh);
           }
           
           [Fact]
           public void InitWithFile_ValidFile_ReturnsTrue()
           {
               // Arrange
               var fileName = "test.png";
               _mockRenderer.Setup(r => r.LoadTexture2DFromFile(fileName))
                          .Returns(_mockTexture.Object);
               
               // Act
               var result = _textureImpl.InitWithFile(fileName);
               
               // Assert
               Assert.True(result);
               Assert.True(_textureImpl.IsTextureDefined);
           }
           
           [Fact]
           public void Dispose_ReleasesResources()
           {
               // Arrange
               var testData = new byte[64 * 64 * 4];
               _mockRenderer.Setup(r => r.CreateTexture2DFromData(testData, SurfaceFormat.Color, false))
                          .Returns(_mockTexture.Object);
               _textureImpl.InitWithData(testData, SurfaceFormat.Color);
               
               // Act
               _textureImpl.Dispose();
               
               // Assert
               Assert.False(_textureImpl.IsTextureDefined);
               _mockTexture.Verify(t => t.Dispose(), Times.Once);
           }
       }
   }
   ```

3. **Create compatibility tests**
   ```csharp
   // File: Tests/TextureTests/CCTexture2DCompatibilityTests.cs
   using Xunit;
   using Cocos2D;
   using Cocos2D.Platform.Factory;
   using Microsoft.Xna.Framework.Graphics;
   
   namespace Cocos2D.Tests.Texture
   {
       public class CCTexture2DCompatibilityTests : IDisposable
       {
           public CCTexture2DCompatibilityTests()
           {
               CCGraphicsFactory.Reset();
           }
           
           public void Dispose()
           {
               CCGraphicsFactory.Reset();
           }
           
           [Fact]
           public void Constructor_CreatesValidTexture()
           {
               // Act
               var texture = new CCTexture2D();
               
               // Assert
               Assert.NotNull(texture);
               Assert.True(texture.IsUsingAbstraction());
           }
           
           [Fact]
           public void XNATexture_WithAbstraction_ReturnsUnderlyingTexture()
           {
               // Arrange
               var texture = new CCTexture2D();
               var testData = new byte[64 * 64 * 4];
               texture.InitWithData(testData, SurfaceFormat.Color);
               
               // Act
               var xnaTexture = texture.XNATexture;
               
               // Assert
               // Should either return the underlying MonoGame texture or null if not MonoGame backend
               Assert.True(xnaTexture != null || !CCGraphicsFactory.IsBackendAvailable(GraphicsBackend.MonoGame));
           }
           
           [Fact]
           public void Properties_WithAbstraction_ReturnCorrectValues()
           {
               // Arrange
               var texture = new CCTexture2D();
               var testData = new byte[128 * 64 * 4];
               
               // Act
               texture.InitWithData(testData, SurfaceFormat.Color);
               
               // Assert
               Assert.True(texture.PixelsWide > 0);
               Assert.True(texture.PixelsHigh > 0);
               Assert.True(texture.ContentSize.Width > 0);
               Assert.True(texture.ContentSize.Height > 0);
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

### Total Estimated Time: ~13 hours (1.5-2 days for one developer)

### Optimal Task Assignment (2 developers working in parallel):

**Developer 1 (Core Implementation):**
- Subtask 2.2.1: Analysis (45m)
- Subtask 2.2.2: Interface Extension (1h) 
- Subtask 2.2.3: Core Implementation (2.5h)
- Subtask 2.2.4: Advanced Operations (2h)
- **Total: 6.25h**

**Developer 2 (Integration/Compatibility):**
- Subtask 2.2.5: Update Original Class (2h)
- Subtask 2.2.6: Legacy Fallback (1.5h)
- Subtask 2.2.7: Migration Layer (1h)
- Subtask 2.2.8: Unit Tests (2h)
- **Total: 6.5h**

### Dependencies:
```
2.2.1 (Analysis) ──> 2.2.2 (Interface) ──> 2.2.3 (Core) ──> 2.2.4 (Advanced)
                                                                │
                                                                ├──> 2.2.5 (Update Original)
                                                                │         │
                                                                │         └──> 2.2.6 (Legacy)
                                                                │               │
                                                                │               └──> 2.2.7 (Migration)
                                                                │                     │
                                                                └─────────────────────┴──> 2.2.8 (Tests)
```

This refactoring provides:
- Complete abstraction from MonoGame Texture2D dependencies
- Maintained backward compatibility with existing CCTexture2D API
- Enhanced texture creation and management capabilities
- Comprehensive testing coverage
- Clear migration path for advanced graphics backends