using System;
using System.Collections.Generic;

namespace Cocos2D
{
    /// <summary>
    /// A label optimized for frequent text updates. Pre-caches individual character
    /// textures at construction time, then repositions sprites on text changes —
    /// no texture regeneration. Ideal for scores, timers, health counters, and
    /// other HUD elements that change every frame.
    /// </summary>
    public class CCPixelLabel : CCNode
    {
        private static readonly Dictionary<string, Dictionary<char, CCTexture2D>> s_fontCache
            = new Dictionary<string, Dictionary<char, CCTexture2D>>();
        private static readonly Dictionary<string, Dictionary<char, float>> s_widthCache
            = new Dictionary<string, Dictionary<char, float>>();
        private static readonly Dictionary<string, float> s_heightCache
            = new Dictionary<string, float>();

        private readonly Dictionary<char, CCTexture2D> m_charTextures;
        private readonly Dictionary<char, float> m_charWidths;
        private readonly float m_charHeight;

        private CCSprite[] m_glyphs;
        private int m_maxChars;
        private string m_text = "";
        private CCTextAlignment m_alignment;
        private CCColor3B m_color = new CCColor3B(255, 255, 255);
        private byte m_opacity = 255;
        private bool m_antialiased;
        private string m_fontName;
        private float m_fontSize;

        private const string DefaultCharSet =
            "0123456789+-/:*x[](){}ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz.,!?@#$%&=<>\"'`~^_;\\| ";

        /// <summary>
        /// Gets or sets the displayed text. Changing this repositions existing
        /// glyph sprites without regenerating any textures.
        /// </summary>
        public string Text
        {
            get { return m_text; }
            set
            {
                string newText = value ?? "";
                if (m_text == newText) return;
                m_text = newText;
                Layout();
            }
        }

        /// <summary>
        /// Gets or sets text alignment (Left, Center, Right).
        /// </summary>
        public CCTextAlignment Alignment
        {
            get { return m_alignment; }
            set
            {
                if (m_alignment == value) return;
                m_alignment = value;
                Layout();
            }
        }

        /// <summary>
        /// Gets or sets the color applied to all glyph sprites.
        /// </summary>
        public override CCColor3B Color
        {
            get { return m_color; }
            set
            {
                m_color = value;
                for (int i = 0; i < m_glyphs.Length; i++)
                    m_glyphs[i].Color = value;
            }
        }

        /// <summary>
        /// Gets or sets the opacity applied to all glyph sprites.
        /// </summary>
        public override byte Opacity
        {
            get { return m_opacity; }
            set
            {
                m_opacity = value;
                for (int i = 0; i < m_glyphs.Length; i++)
                    m_glyphs[i].Opacity = value;
            }
        }

        /// <summary>
        /// Gets the font name used by this label.
        /// </summary>
        public string FontName
        {
            get { return m_fontName; }
        }

        /// <summary>
        /// Gets the font size used by this label.
        /// </summary>
        public float FontSize
        {
            get { return m_fontSize; }
        }

        /// <summary>
        /// Gets the measured height of a single character in this font.
        /// </summary>
        public float CharHeight
        {
            get { return m_charHeight; }
        }

        /// <summary>
        /// Gets or sets whether glyph sprites use antialiased (linear) filtering.
        /// When false, uses point filtering for pixel-perfect rendering.
        /// </summary>
        public bool IsAntialiased
        {
            get { return m_antialiased; }
            set
            {
                m_antialiased = value;
                for (int i = 0; i < m_glyphs.Length; i++)
                    m_glyphs[i].IsAntialiased = value;
            }
        }

        /// <summary>
        /// Creates a new CCPixelLabel.
        /// </summary>
        /// <param name="text">Initial text to display.</param>
        /// <param name="fontName">System font name (e.g. "arial").</param>
        /// <param name="fontSize">Font size in points.</param>
        /// <param name="alignment">Text alignment.</param>
        /// <param name="antialiased">Whether to use antialiased filtering.</param>
        /// <param name="maxChars">Maximum number of visible characters. Pre-allocates this many sprites.</param>
        public CCPixelLabel(string text, string fontName, float fontSize,
            CCTextAlignment alignment = CCTextAlignment.Left,
            bool antialiased = false, int maxChars = 32)
        {
            m_fontName = fontName;
            m_fontSize = fontSize;
            m_alignment = alignment;
            m_antialiased = antialiased;
            m_maxChars = maxChars;

            string cacheKey = fontName + "_" + fontSize;

            if (!s_fontCache.TryGetValue(cacheKey, out m_charTextures))
            {
                m_charTextures = new Dictionary<char, CCTexture2D>();
                var widths = new Dictionary<char, float>();
                float maxH = 0;

                foreach (char c in DefaultCharSet)
                {
                    if (c == ' ') continue;

                    var label = new CCLabelTTF(c.ToString(), fontName, fontSize);
                    if (label.Texture != null)
                    {
                        m_charTextures[c] = label.Texture;
                        float w = label.Texture.ContentSizeInPixels.Width;
                        float h = label.Texture.ContentSizeInPixels.Height;
                        widths[c] = w;
                        if (h > maxH) maxH = h;
                    }
                }

                s_fontCache[cacheKey] = m_charTextures;
                s_widthCache[cacheKey] = widths;
                s_heightCache[cacheKey] = maxH;
            }

            m_charWidths = s_widthCache[cacheKey];
            m_charHeight = s_heightCache[cacheKey];

            AllocateGlyphs(maxChars);

            m_text = text ?? "";
            if (m_text.Length > 0)
                Layout();
        }

        /// <summary>
        /// Creates a new CCPixelLabel with default settings.
        /// </summary>
        /// <param name="text">Initial text to display.</param>
        /// <param name="fontName">System font name.</param>
        /// <param name="fontSize">Font size in points.</param>
        public CCPixelLabel(string text, string fontName, float fontSize)
            : this(text, fontName, fontSize, CCTextAlignment.Left, false, 32)
        {
        }

        /// <summary>
        /// Pre-caches additional characters beyond the default character set.
        /// Call this once at startup if your text will include characters not in
        /// the default set (e.g. Unicode or special symbols).
        /// </summary>
        public void CacheCharacters(string characters)
        {
            string cacheKey = m_fontName + "_" + m_fontSize;

            foreach (char c in characters)
            {
                if (c == ' ' || m_charTextures.ContainsKey(c))
                    continue;

                var label = new CCLabelTTF(c.ToString(), m_fontName, m_fontSize);
                if (label.Texture != null)
                {
                    m_charTextures[c] = label.Texture;
                    float w = label.Texture.ContentSizeInPixels.Width;
                    m_charWidths[c] = w;
                }
            }
        }

        /// <summary>
        /// Grows the glyph sprite pool if the current maxChars is insufficient.
        /// </summary>
        public void SetMaxChars(int maxChars)
        {
            if (maxChars <= m_maxChars) return;

            // Hide and remove old glyphs
            for (int i = 0; i < m_glyphs.Length; i++)
            {
                RemoveChild(m_glyphs[i], true);
            }

            m_maxChars = maxChars;
            AllocateGlyphs(maxChars);
            Layout();
        }

        /// <summary>
        /// Measures the pixel width of a string without rendering it.
        /// </summary>
        public float MeasureString(string text)
        {
            float width = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == ' ')
                    width += m_charHeight * 0.4f;
                else if (m_charWidths.TryGetValue(c, out float w))
                    width += w;
            }
            return width;
        }

        /// <summary>
        /// Clears all static font caches. Call this when changing scenes or
        /// when you need to free memory from fonts that are no longer used.
        /// </summary>
        public static void PurgeCachedData()
        {
            s_fontCache.Clear();
            s_widthCache.Clear();
            s_heightCache.Clear();
        }

        private void AllocateGlyphs(int count)
        {
            m_glyphs = new CCSprite[count];
            for (int i = 0; i < count; i++)
            {
                m_glyphs[i] = new CCSprite();
                m_glyphs[i].AnchorPoint = CCPoint.Zero;
                m_glyphs[i].Visible = false;
                AddChild(m_glyphs[i]);
            }
        }

        private void Layout()
        {
            int count = Math.Min(m_text.Length, m_maxChars);

            // First pass: measure total width
            float totalWidth = 0;
            for (int i = 0; i < count; i++)
            {
                char c = m_text[i];
                if (c == ' ')
                    totalWidth += m_charHeight * 0.4f;
                else if (m_charWidths.TryGetValue(c, out float w))
                    totalWidth += w;
            }

            // Alignment offset
            float offsetX = 0;
            switch (m_alignment)
            {
                case CCTextAlignment.Center:
                    offsetX = -totalWidth / 2f;
                    break;
                case CCTextAlignment.Right:
                    offsetX = -totalWidth;
                    break;
            }

            float offsetY = -m_charHeight;

            // Second pass: position glyphs
            float x = offsetX;
            int glyphIdx = 0;

            for (int i = 0; i < count; i++)
            {
                char c = m_text[i];
                if (c == ' ')
                {
                    x += m_charHeight * 0.4f;
                    continue;
                }

                if (!m_charTextures.TryGetValue(c, out var tex))
                    continue;

                if (glyphIdx >= m_maxChars) break;

                var glyph = m_glyphs[glyphIdx];
                glyph.InitWithTexture(tex);
                glyph.IsAntialiased = m_antialiased;
                glyph.AnchorPoint = CCPoint.Zero;
                glyph.PositionX = x;
                glyph.PositionY = offsetY;
                glyph.Color = m_color;
                glyph.Opacity = m_opacity;
                glyph.Visible = true;
                glyphIdx++;

                x += m_charWidths.TryGetValue(c, out float w) ? w : m_charHeight * 0.5f;
            }

            // Hide unused glyphs
            for (int i = glyphIdx; i < m_maxChars; i++)
                m_glyphs[i].Visible = false;

            // Update content size
            ContentSize = new CCSize(totalWidth, m_charHeight);
        }
    }
}
