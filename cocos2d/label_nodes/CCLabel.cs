using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using cocos2d.label_nodes;
using Microsoft.Xna.Framework.Graphics;

namespace Cocos2D
{
    public partial class CCLabel : CCLabelBMFont
    {
        private struct KerningInfo
        {
            /// <summary>Specifies the A spacing of the character. The A spacing is the distance to add to the current
            /// position before drawing the character glyph.</summary>
            public float A;
            /// <summary>Specifies the B spacing of the character. The B spacing is the width of the drawn portion of
            /// the character glyph.</summary>
            public float B;
            /// <summary>Specifies the C spacing of the character. The C spacing is the distance to add to the current
            /// position to provide white space to the right of the character glyph.</summary>
            public float C;
        }

        public CCTexture2D m_pTexture;
        protected bool m_bTextureDirty = true;

        protected string m_FontName;
        protected float m_FontSize;
        protected int m_FontSpacing;
        protected bool m_bFontDirty;

        [Obsolete("This will be removed in a future release. Please use SystemFont instead.")]
        public string FontName
        {
            get { return m_FontName; }
            set
            {
                if (m_FontName != value)
                {
                    m_FontName = value;
                    m_bFontDirty = true;
                }
            }
        }

        [Obsolete("This will be removed in a future release. Please use SystemFontSize instead.")]
        public float FontSize
        {
            get { return m_FontSize; }
            set
            {
                if (m_FontSize != value)
                {
                    m_FontSize = value;
                    m_bFontDirty = true;
                }
            }
        }

        public string SystemFont
        {
            get { return m_FontName; }
            set
            {
                if (m_FontName != value)
                {
                    m_FontName = value;
                    m_bFontDirty = true;
                }
            }
        }

        public float SystemFontSize
        {
            get { return m_FontSize; }
            set
            {
                if (m_FontSize != value)
                {
                    m_FontSize = value;
                    m_pConfiguration = InitializeFont(m_FontName, m_FontSize, Text);
                    m_bFontDirty = true;
                }
            }
        }

        public int SystemFontSpacing
        {
            get { return m_FontSpacing; }
            set
            {
                if (m_FontSpacing != value)
                {
                    m_FontSpacing = value;
                    m_pConfiguration = InitializeFont(m_FontName, m_FontSize, Text);
                    m_bFontDirty = true;
                }
            }
        }

        public void InitializeTTFAtlas(int width, int height)
        {
            m_nWidth = width;
            m_nHeight = height;
            m_nDepth = 4;

            m_pTexture = new CCTexture2D();
            m_pData = new int[width * height];

            m_pNodes.Clear();
            m_pNodes.Add(new ivec3() { x = 1, y = 1, z = m_nWidth - 2 });

            m_pTexture.InitWithRawData(m_pData, SurfaceFormat.Color, m_nWidth, m_nHeight, true);
        }

        public CCLabel()
        {
        }

        public CCLabel(string text, string fontName, float fontSize) :
            this(text, fontName, fontSize, CCSize.Zero, CCTextAlignment.Left, CCVerticalTextAlignment.Top)
        { }

        public CCLabel(string text, string fontName, float fontSize, CCTextAlignment hAlignment) :
            this(text, fontName, fontSize, CCSize.Zero, hAlignment, CCVerticalTextAlignment.Top)
        { }

        public CCLabel(string text, string fontName, float fontSize, CCTextAlignment hAlignment, CCVerticalTextAlignment vAlignment) :
            this(text, fontName, fontSize, CCSize.Zero, hAlignment, vAlignment)
        { }

        public CCLabel(string text, string fontName, float fontSize, CCSize dimensions) :
            this(text, fontName, fontSize, dimensions, CCTextAlignment.Left, CCVerticalTextAlignment.Top)
        {
        }

        public CCLabel(string text, string fontName, float fontSize, CCSize dimensions, CCTextAlignment hAlignment) :
            this(text, fontName, fontSize, dimensions, hAlignment, CCVerticalTextAlignment.Top)
        {
        }

        public CCLabel(string text, string fontName, float fontSize, CCSize dimensions, CCTextAlignment hAlignment, CCVerticalTextAlignment vAlignment)
        {
            InitWithString(text, fontName, fontSize, dimensions, hAlignment, vAlignment);
        }

        public bool InitWithString(string text, string fontName, float fontSize)
        {
            return InitWithString(text, fontName, fontSize, CCSize.Zero, CCTextAlignment.Left, CCVerticalTextAlignment.Top);
        }

        public bool InitWithString(string text, string fontName, float fontSize, CCSize dimensions, CCTextAlignment hAlignment)
        {
            return InitWithString(text, fontName, fontSize, dimensions, hAlignment, CCVerticalTextAlignment.Top);
        }

        public bool InitWithString(string text, string fontName, float fontSize, CCSize dimensions, CCTextAlignment hAlignment, CCVerticalTextAlignment vAlignment)
        {
            InitializeFont(fontName, fontSize, text);
			m_FontName = fontName;
			m_FontSize = fontSize;

            s_pTextures[GetFontKey(m_FontName, m_FontSize)] = new CCBMFontTexture()
            {
                m_pNodes = m_pNodes,
                m_nUsed = m_nUsed,
                m_nWidth = m_nWidth,
                m_nHeight = m_nHeight,
                m_nDepth = m_nDepth,
                m_pData = m_pData,
                m_pTexture = m_pTexture
            };

            return base.InitWithString(text, GetFontKey(fontName, fontSize), dimensions.PointsToPixels(), hAlignment, vAlignment, CCPoint.Zero, m_pTexture);
        }

        private CCBMFontConfiguration InitializeFont(string fontName, float fontSize, string charset)
        {
            var fontKey = GetFontKey(fontName, fontSize);

            if (s_pTextures.ContainsKey(fontKey))
            {
                var m_pBMFontTexture = s_pTextures[fontKey];
                m_pTexture = m_pBMFontTexture.m_pTexture;
                m_pData = m_pBMFontTexture.m_pData;
                m_nWidth = m_pBMFontTexture.m_nWidth;
                m_nHeight = m_pBMFontTexture.m_nHeight;
                m_pNodes = m_pBMFontTexture.m_pNodes;
                m_nUsed = m_pBMFontTexture.m_nUsed;
                m_nDepth = m_pBMFontTexture.m_nDepth;
            }

            if (m_pData == null)
            {
                InitializeTTFAtlas((int)(1024 * (fontSize/24)), (int)(1024 * (fontSize / 24)));
            }

            if (string.IsNullOrEmpty(charset))
            {
                charset = " ";
            }

            var chars = new CCRawList<char>();

            CCBMFontConfiguration fontConfig;

            if (!s_pConfigurations.TryGetValue(fontKey, out fontConfig))
            {
                fontConfig = new CCBMFontConfiguration();
                s_pConfigurations.Add(fontKey, fontConfig);
            }

            for (int i = 0; i < charset.Length; i++)
            {
                var ch = charset[i];
                if (!fontConfig.m_pFontDefDictionary.ContainsKey(ch) && chars.IndexOf(ch) == -1)
                {
                    chars.Add(ch);
                }
                else if (fontConfig.m_pFontDefDictionary.ContainsKey(ch))
                {
                    var fontDef = fontConfig.m_pFontDefDictionary[ch];
                    fontDef.xAdvance = fontDef.xKern + m_FontSpacing;
                }
            }

            if (chars.Count == 0)
            {
                return fontConfig;
            }

            CreateFont(fontName, fontSize * CCMacros.CCContentScaleFactor(), chars);

            fontConfig.m_nCommonHeight = (int)Math.Ceiling(GetFontHeight());

            int[] data = null;

            for (int i = 0; i < chars.Count; i++)
            {
                var s = chars[i].ToString();

                var charSize = GetMeasureString(s);

                int w = (int)Math.Ceiling(charSize.Width + 2);
                int h = (int)Math.Ceiling(charSize.Height + 2);

                if (data == null || data.Length < (w * h))
                {
                    data = new int[w * h];
                }

                unsafe
                {
                    int stride;
                    byte* pBase = GetBitmapData(s, out stride);

                    int minX = w;
                    int maxX = 0;
                    int minY = h;
                    int maxY = 0;

                    for (int y = 0; y < h; y++)
                    {
                        var row = (int*)(pBase + y * stride);

                        for (int x = 0; x < w; x++)
                        {
                            if (row[x] != 0)
                            {
                                minX = Math.Min(minX, x);
                                maxX = Math.Max(maxX, x);
                                minY = Math.Min(minY, y);
                                maxY = Math.Max(maxY, y);
                            }
                        }
                    }

                    w = Math.Max(maxX - minX + 1, 1);
                    h = Math.Max(maxY - minY + 1, 1);

                    int index = 0;
                    for (int y = minY; y <= maxY; y++)
                    {
                        var row = (int*)(pBase + y * stride);
                        for (int x = minX; x <= maxX; x++)
                        {
                            data[index] = row[x];
                            index++;
                        }
                    }

                    var region = AllocateRegion(w, h);

                    if (region.x >= 0)
                    {
                        SetRegionData(region, data, w);

                        var info = GetKerningInfo(chars[i]);
                        var regionX = region.x + 0.015f;
                        var regionWidth = region.width;
#if ANDROID
                        regionX = region.x + 0.55f;
#endif

#if IOS
                        regionX = region.x + 0.33f;
                        regionWidth -= 1;
#endif

                        var fontDef = new CCBMFontConfiguration.CCBMFontDef()
                        {
                            charID = chars[i],
                            rect = new CCRect(regionX, region.y, regionWidth, region.height),
                            xOffset = minX,
                            yOffset = minY,
                            xKern = (int)Math.Ceiling(info.A + info.B + info.C),
                            xAdvance = (int)Math.Ceiling(info.A + info.B + info.C) + m_FontSpacing
                        };

                        fontConfig.CharacterSet.Add(chars[i]);
                        fontConfig.m_pFontDefDictionary.Add(chars[i], fontDef);
                    }
                    else
                    {
                        CCLog.Log("Texture atlas is full");
                    }
                }
            }

            m_bTextureDirty = true;

            return fontConfig;
        }

        public override void Draw()
        {
            if (m_bFontDirty)
            {
                m_pConfiguration = InitializeFont(m_FontName, m_FontSize, Text);
                m_bFontDirty = false;
            }

            if (m_bTextureDirty)
            {
                m_pTexture.InitWithRawData(m_pData, SurfaceFormat.Color, m_nWidth, m_nHeight, true);
                m_bTextureDirty = false;
            }

            base.Draw();
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (m_sInitialString != value)
                {
                    InitializeFont(FontName, FontSize, value);
                    base.Text = value;
                }
            }
        }


        private string GetFontKey(string fontName, float fontSize)
        {
            return String.Format("ttf-{0}-{1}", fontName, fontSize);
        }

        #region Skyline Bottom Left

        public struct ivec3
        {
            public int x;
            public int y;
            public int z;
        }

        public struct ivec4
        {
            public int x;
            public int y;
            public int width;
            public int height;
        }

        private CCRawList<ivec3> m_pNodes = new CCRawList<ivec3>();
        private int m_nUsed;
        private int m_nWidth;
        private int m_nHeight;
        private int m_nDepth;
        public int[] m_pData;

        private int Fit(int index, int width, int height)
        {
            var node = m_pNodes[index];

            var x = node.x;
            var y = node.y;
            var widthLeft = width;
            var i = index;

            if ((x + width) > (m_nWidth - 1))
            {
                return -1;
            }

            while (widthLeft > 0)
            {
                node = m_pNodes[i];

                if (node.y > y)
                {
                    y = node.y;
                }

                if ((y + height) > (m_nHeight - 1))
                {
                    return -1;
                }

                widthLeft -= node.z;

                ++i;
            }
            return y;
        }

        private void Merge()
        {
            var nodes = m_pNodes.Elements;
            for (int i = 0, count = m_pNodes.Count; i < count - 1; ++i)
            {
                if (nodes[i].y == nodes[i + 1].y)
                {
                    nodes[i].z += nodes[i + 1].z;
                    m_pNodes.RemoveAt(i + 1);
                    --count;
                    --i;
                }
            }
        }

        public ivec4 AllocateRegion(int width, int height)
        {
            ivec3 node, prev;
            ivec4 region = new ivec4() { x = 0, y = 0, width = width, height = height };
            int i;

            int bestHeight = int.MaxValue;
            int bestIndex = -1;
            int bestWidth = int.MaxValue;

            for (i = 0; i < m_pNodes.Count; ++i)
            {
                int y = Fit(i, width, height);

                if (y >= 0)
                {
                    node = m_pNodes[i];
                    if (((y + height) < bestHeight) || (((y + height) == bestHeight) && (node.z < bestWidth)))
                    {
                        bestHeight = y + height;
                        bestIndex = i;
                        bestWidth = node.z;
                        region.x = node.x;
                        region.y = y;
                    }
                }
            }

            if (bestIndex == -1)
            {
                region.x = -1;
                region.y = -1;
                region.width = 0;
                region.height = 0;
                return region;
            }

            //New node
            node.x = region.x;
            node.y = region.y + height;
            node.z = width;
            m_pNodes.Insert(bestIndex, node);

            for (i = bestIndex + 1; i < m_pNodes.Count; ++i)
            {
                node = m_pNodes[i];
                prev = m_pNodes[i - 1];

                if (node.x < (prev.x + prev.z))
                {
                    int shrink = prev.x + prev.z - node.x;
                    node.x += shrink;
                    node.z -= shrink;
                    if (node.z <= 0)
                    {
                        m_pNodes.RemoveAt(i);
                        --i;
                    }
                    else
                    {
                        m_pNodes[i] = node;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            Merge();

            m_nUsed += width * height;

            return region;
        }

        public void SetRegionData(ivec4 region, int[] data, int stride)
        {
            var x = region.x;
            var y = region.y;
            var width = region.width;
            var height = region.height;

            Debug.Assert(x > 0);
            Debug.Assert(y > 0);
            Debug.Assert(x < (m_nWidth - 1));
            Debug.Assert((x + width) <= (m_nWidth - 1));
            Debug.Assert(y < (m_nHeight - 1));
            Debug.Assert((y + height) <= (m_nHeight - 1));

            var depth = m_nDepth;
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; j++)
                {
                    var b = (byte)((data[i * stride + j] & 0xFF0000) >> 16);
                    m_pData[((y + i) * m_nWidth + x) + j] = b << 24 | b << 16 | b << 8 | b;
                }
                //    Array.Copy(data, (i * stride), m_pData, ((y + i) * m_nWidth + x), width);
                //                Buffer.BlockCopy(data, (i * stride), m_pData, ((y + i) * m_nWidth + x) * depth, width * depth);
            }
        }

        #endregion
    }

}
