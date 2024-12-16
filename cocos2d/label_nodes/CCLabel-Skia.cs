#if DESKTOPGL && (LINUX || MACOS)
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

        private static SKBitmap _bitmap;
        private static SKCanvas _canvas;
        private static Dictionary<char, KerningInfo> _kerningInfo = new Dictionary<char, KerningInfo>();
        private static Dictionary<string, SKTypeface> _typefaceCache = new Dictionary<string, SKTypeface>();

        private string CreateFont(string fontName, float fontSize, CCRawList<char> charset)
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

            GetKerningInfo(charset);

            CreateBitmap(1, 1);

            return _currentTypeface.FamilyName;
        }

        private static void GetKerningInfo(CCRawList<char> charset)
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

        private float GetFontHeight()
        {
            using var paint = new SKPaint
            {
                Typeface = _currentTypeface,
                TextSize = _currentFontSize
            };
            return paint.FontMetrics.CapHeight;
        }

        private CCSize GetMeasureString(string text)
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

        private void CreateBitmap(int width, int height)
        {
            if (_bitmap == null || (_bitmap.Width < width || _bitmap.Height < height))
            {
                _bitmap?.Dispose();

                _bitmap = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
                _canvas = new SKCanvas(_bitmap);
            }
        }

        private unsafe byte* GetBitmapData(string s, out int stride)
        {
            var size = GetMeasureString(s);

            var w = (int)Math.Ceiling(size.Width + 2);
            var h = (int)Math.Ceiling(size.Height + 2);

            CreateBitmap(w, h);

            _canvas.Clear(SKColors.Transparent);

            using var paint = new SKPaint
            {
                Typeface = _currentTypeface,
                TextSize = _currentFontSize,
                IsAntialias = true,
                Color = SKColors.White
            };


            var font = new SKFont(_currentTypeface, _currentFontSize);

            _canvas.DrawText(s, 0, 10, SKTextAlign.Left, font, paint);

            stride = _bitmap.RowBytes;

            fixed (byte* ptr = _bitmap.GetPixelSpan())
            {
                return ptr;
            }
        }

        private KerningInfo GetKerningInfo(char ch)
        {
            return _kerningInfo[ch];
        }
    }
}
#endif