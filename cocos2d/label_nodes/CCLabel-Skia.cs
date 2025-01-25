#if DESKTOPGL 
using System;
using System.IO;
using System.Collections.Generic;
using SkiaSharp;


namespace Cocos2D
{
    public partial class CCLabel
    {
        private static SKTypeface _defaultTypeface;
        private static SKTypeface _currentTypeface;
        private static float _currentFontSize;

        private static SKBitmap _bitmapSkia;
        private static SKCanvas _canvas;
        private static Dictionary<char, KerningInfo> _kerningInfo = new Dictionary<char, KerningInfo>();
        private static Dictionary<string, SKTypeface> _typefaceCache = new Dictionary<string, SKTypeface>();

#if (LINUX || MACOS)

        private string CreateFont(string fontName, float fontSize, CCRawList<char> charset)
        {
            return CreateFontSkia(fontName, fontSize, charset);
        }

        private static void GetKerningInfo(CCRawList<char> charset)
        {
            GetKerningInfoSkia(charset);
        }

        private float GetFontHeight()
        {
            return GetFontHeightSkia();
        }

        private CCSize GetMeasureString(string text)
        {
            return GetMeasureStringSkia(text);
        }

        private void CreateBitmap(int width, int height)
        {
            CreateBitmapSkia(width, height);
        }

        private unsafe byte* GetBitmapData(string s, out int stride)
        {
           GetBitmapDataSkia(s, out stride);
        }

        private KerningInfo GetKerningInfo(char ch)
        {
            return GetKerningInfoSkia(ch);
        }
#endif
        private string CreateFontSkia(string fontName, float fontSize, CCRawList<char> charset)
        {
            if (_defaultTypeface == null)
            {
                _defaultTypeface = SKTypeface.Default;
            }

            if (!_typefaceCache.TryGetValue(fontName, out var typeface))
            {
                var ext = Path.GetExtension(fontName);

                _currentTypeface = _defaultTypeface;

                if (!string.IsNullOrEmpty(ext) && ext.ToLower() == ".ttf")
                {
                    var appPath = AppDomain.CurrentDomain.BaseDirectory;
                    var contentPath = Path.Combine(appPath, CCContentManager.SharedContentManager.RootDirectory);
                    var fontPath = Path.Combine(contentPath, fontName);

                    if (File.Exists(fontPath))
                    {
                        try
                        {
                            typeface = SKTypeface.FromFile(fontPath);
                            _currentTypeface = typeface;
                        }
                        catch
                        {
                            _currentTypeface = _defaultTypeface;
                        }
                    }
                }
                else
                {
                    _currentTypeface = SKTypeface.FromFamilyName(fontName) ?? _defaultTypeface;
                }

                _typefaceCache.Add(fontName, _currentTypeface);
            }
            else
            {
                _currentTypeface = typeface;
            }

            _currentFontSize = fontSize;

            GetKerningInfoSkia(charset);

            CreateBitmapSkia(1, 1);

            return _currentTypeface.FamilyName;
        }

        private static void GetKerningInfoSkia(CCRawList<char> charset)
        {
            _kerningInfo.Clear();

            using var paint = new SKPaint
            {
                Typeface = _currentTypeface,
                TextSize = _currentFontSize
            };

            foreach (var ch in charset)
            {
                if (!_kerningInfo.ContainsKey(ch))
                {
                    var width = paint.MeasureText(ch.ToString());
                    var with2 = paint.GetGlyphWidths(new[] { ch });
                    if (width > 0)
                    {
                        _kerningInfo[ch] = new KerningInfo
                        {
                            A = 1, // SkiaSharp doesn't provide explicit ABC spacing; adjust as needed
                            B = width,
                            C = 1
                        };
                    }
                }
            }
        }

        private float GetFontHeightSkia()
        {
            using var paint = new SKPaint
            {
                Typeface = _currentTypeface,
                TextSize = _currentFontSize
            };
            return paint.FontMetrics.CapHeight;
        }

        private CCSize GetMeasureStringSkia(string text)
        {
            using var paint = new SKPaint
            {
                Typeface = _currentTypeface,
                TextSize = _currentFontSize
            };

            var bounds = new SKRect();
            paint.MeasureText(text, ref bounds);
            return new CCSize(bounds.Width, bounds.Height);
        }

        private void CreateBitmapSkia(int width, int height)
        {
            if (_bitmapSkia == null || (_bitmapSkia.Width < width || _bitmapSkia.Height < height))
            {
                _bitmapSkia?.Dispose();

                _bitmapSkia = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
                _canvas = new SKCanvas(_bitmapSkia);
            }
        }

        private unsafe byte* GetBitmapDataSkia(string s, out int stride)
        {
            var size = GetMeasureStringSkia(s);

            var w = (int)Math.Ceiling(size.Width + 2);
            var h = (int)Math.Ceiling(size.Height + 2);

            CreateBitmapSkia(w, h);

            _canvas.Clear(SKColors.Transparent);

            using var paint = new SKPaint
            {
                Typeface = _currentTypeface,
                TextSize = _currentFontSize,
                IsAntialias = true,
                Color = SKColors.White
            };


            var font = new SKFont(_currentTypeface, _currentFontSize *.68f);

            _canvas.DrawText(s, 0, _currentFontSize/2, SKTextAlign.Left, font, paint);

            stride = _bitmapSkia.RowBytes;

            fixed (byte* ptr = _bitmapSkia.GetPixelSpan())
            {
                return ptr;
            }
        }

        private KerningInfo GetKerningInfoSkia(char ch)
        {
            return _kerningInfo[ch];
        }
    }
}
#endif