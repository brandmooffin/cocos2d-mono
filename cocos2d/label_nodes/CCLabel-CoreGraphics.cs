#if IOS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CoreGraphics;
using System.Runtime.InteropServices;

#if MACOS
using MonoMac.CoreGraphics;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreText;
using MonoMac.ImageIO;

#else
using CoreGraphics;
using UIKit;
using Foundation;
using CoreText;
using ImageIO;
#endif

namespace Cocos2D
{
	public partial class CCLabel
	{

		private static CTFont _font;
		private static CGBitmapContext _bitmap;
		private static IntPtr _bitmapData;
		private static CCColor4B _brush;
		private static Dictionary<char, KerningInfo> _abcValues = new Dictionary<char, KerningInfo>();

		private void CreateFont(string fontName, float fontSize, CCRawList<char> charset)
		{

			_font = CCLabelUtilities.CreateFont (fontName, fontSize);

			var value = new CCLabelUtilities.ABCFloat[1];

			_abcValues.Clear();;

			for (int i = 0; i < charset.Count; i++)
			{
				var ch = charset[i];
				CCLabelUtilities.GetCharABCWidthsFloat(ch, _font, out value);
				_abcValues.Add(ch, new KerningInfo() { A = value[0].abcfA, B = value[0].abcfB, C = value[0].abcfC });
			}

		}

		private float GetFontHeight()
		{
			return _font.GetHeight();
		}

		private CCSize GetMeasureString(string text)
		{
			return CCLabelUtilities.MeasureString(text, _font);
		}

		private KerningInfo GetKerningInfo(char ch)
		{
			return _abcValues[ch];
		}

		private void CreateBitmap(int width, int height)
		{
//			if (_bitmap == null || (_bitmap.Width < width || _bitmap.Height < height))
//			{

				_bitmap = CCLabelUtilities.CreateBitmap (width, height);
			//}

			//if (_brush == null)
			//{
				_brush = new CCColor4B(Microsoft.Xna.Framework.Color.White);
			//}
		}

		private unsafe byte* GetBitmapData(string s, out int stride)
		{

			var size = GetMeasureString(s);

			var w = (int)(Math.Ceiling(size.Width += 2));
			var h = (int)(Math.Ceiling(size.Height += 2));

			CreateBitmap(w, h);

			CCLabelUtilities.NativeDrawString(_bitmap, s, _font, _brush, new CGRect(0,0,w,h));
			_bitmapData = _bitmap.Data;

            stride = (int)_bitmap.Width * 4;

			return (byte*)_bitmapData;
		}

	}
}
#endif