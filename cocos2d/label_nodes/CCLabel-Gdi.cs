#if DESKTOPGL
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Cocos2D
{
    public partial class CCLabel
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct ABCFloat
        {
            public float abcfA; // The A spacing of the character.
            public float abcfB; // The B spacing of the character.
            public float abcfC; // The C spacing of the character.
        }

        private static SKTypeface _defaultFont;
        private static SKTypeface _currentFont;
        private static float _currentFontSize;
        private static readonly float _defaultFontSize = 12;

        private static SKBitmap _bitmap;
        private static SKCanvas _canvas;
        private static SKPaint _paint;
        private static Dictionary<char, KerningInfo> _abcValues = new Dictionary<char, KerningInfo>();
        private static Dictionary<string, SKTypeface> _fontFamilyCache = new Dictionary<string, SKTypeface>();

        private string CreateFont(string fontName, float fontSize, CCRawList<char> charset)
        {
            if (_defaultFont == null)
            {

                _defaultFont = SKTypeface.FromFamilyName("Sans-serif");
            }

            if (!_fontFamilyCache.TryGetValue(fontName, out _currentFont))
            {
                var ext = Path.GetExtension(fontName);

                _currentFont = _defaultFont;
                _currentFontSize = _defaultFontSize;

                if (!String.IsNullOrEmpty(ext) && ext.ToLower() == ".ttf")
                {
                    var appPath = AppDomain.CurrentDomain.BaseDirectory;
                    var contentPath = Path.Combine(appPath, CCContentManager.SharedContentManager.RootDirectory);
                    var fontPath = Path.Combine(contentPath, fontName);

                    if (File.Exists(fontPath))
                    {
                        try
                        {
                            _currentFont = SKTypeface.FromFile(fontPath);
                            _currentFontSize = fontSize;
                        }
                        catch
                        {
                            _currentFont = _defaultFont;
                            _currentFontSize = _defaultFontSize;
                        }
                    }
                }
                else
                {
                    _currentFont = SKTypeface.FromFamilyName(fontName);
                    _currentFontSize = fontSize;
                }

                _fontFamilyCache.Add(fontName, _currentFont);
            }

            GetKerningInfo(charset);

            return _currentFont.FamilyName;
        }

        private void GetKerningInfo(CCRawList<char> charset)
        {
            _abcValues.Clear();

            using (SKPaint paint = new SKPaint { Typeface = _currentFont })
            {
                foreach (var ch in charset)
                {
                    if (!_abcValues.ContainsKey(ch))
                    {
                        var width = paint.MeasureText(ch.ToString());
                        _abcValues.Add(ch, new KerningInfo { A = 0, B = width, C = 0 });
                    }
                }
            }
        }

        private float GetFontHeight()
        {
            using (SKPaint paint = new SKPaint { Typeface = _currentFont })
            {
                return paint.FontMetrics.CapHeight;
            }
        }

        private CCSize GetMeasureString(string text)
        {
            using (SKPaint paint = new SKPaint { Typeface = _currentFont, TextSize = _currentFontSize })
            {
                var size = new SKRect();
                paint.MeasureText(text, ref size);
                return new CCSize(size.Width, size.Height);
            }
        }

        private void CreateBitmap(int width, int height)
        {
            if (_bitmap == null || (_bitmap.Width < width || _bitmap.Height < height))
            {
                _bitmap = new SKBitmap(width, height);

                _canvas = new SKCanvas(_bitmap);

                _paint = new SKPaint
                {
                    IsAntialias = true,
                    FilterQuality = SKFilterQuality.High,
                    TextSize = 12,
                    Color = SKColors.White
                };
            }
        }

        private void FreeBitmapData()
        {
            // SkiaSharp does not need bitmap data to be unlocked
        }

        private KerningInfo GetKerningInfo(char ch)
        {
            return _abcValues[ch];
        }

        private unsafe byte* GetBitmapData(string s, out int stride)
        {
            FreeBitmapData();

            var size = GetMeasureString(s);

            var w = (int)(Math.Ceiling(size.Width + 2));
            var h = (int)(Math.Ceiling(size.Height + 2));

            CreateBitmap(w, h);

            _canvas.Clear(SKColors.Transparent);
            _canvas.DrawText(s, 0, size.Height, _paint);

            stride = _bitmap.RowBytes;

            return (byte*)_bitmap.GetPixels();
        }
    }
}
#endif