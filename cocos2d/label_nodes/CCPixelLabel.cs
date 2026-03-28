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

        private Dictionary<char, CCTexture2D> m_charTextures;
        private Dictionary<char, float> m_charWidths;
        private float m_charHeight;

        private CCSprite[] m_glyphs;
        private int m_maxChars;
        private string m_text = "";
        private CCTextAlignment m_alignment;
        private bool m_antialiased;
        private string m_fontName;
        private float m_fontSize;
        private string m_cacheKey;

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
                CacheCharacters(m_text);
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
            get { return base.Color; }
            set
            {
                base.Color = value;
                for (int i = 0; i < m_glyphs.Length; i++)
                    m_glyphs[i].Color = value;
            }
        }

        /// <summary>
        /// Gets or sets the opacity applied to all glyph sprites.
        /// </summary>
        public override byte Opacity
        {
            get { return base.Opacity; }
            set
            {
                base.Opacity = value;
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
                if (m_antialiased == value) return;

                m_antialiased = value;
                m_cacheKey = m_fontName + "_" + m_fontSize + (value ? "_aa" : "_px");

                // Switch to a separate texture cache for this filtering mode
                // so shared textures are not mutated across labels
                Dictionary<char, CCTexture2D> textures;
                if (!s_fontCache.TryGetValue(m_cacheKey, out textures))
                {
                    textures = new Dictionary<char, CCTexture2D>();
                    var widths = new Dictionary<char, float>();
                    float maxH = 0;

                    foreach (char c in DefaultCharSet)
                    {
                        if (c == ' ') continue;

                        var label = new CCLabelTTF(c.ToString(), m_fontName, m_fontSize);
                        if (label.Texture != null)
                        {
                            label.Texture.IsAntialiased = value;
                            textures[c] = label.Texture;
                            float w = label.Texture.ContentSize.Width;
                            float h = label.Texture.ContentSize.Height;
                            widths[c] = w;
                            if (h > maxH) maxH = h;
                        }
                    }

                    s_fontCache[m_cacheKey] = textures;
                    s_widthCache[m_cacheKey] = widths;
                    s_heightCache[m_cacheKey] = maxH;
                }

                m_charTextures = textures;
                m_charWidths = s_widthCache[m_cacheKey];
                m_charHeight = s_heightCache[m_cacheKey];

                Layout();
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
            m_cacheKey = fontName + "_" + fontSize + (antialiased ? "_aa" : "_px");

            Dictionary<char, CCTexture2D> textures;
            if (!s_fontCache.TryGetValue(m_cacheKey, out textures))
            {
                textures = new Dictionary<char, CCTexture2D>();
                var widths = new Dictionary<char, float>();
                float maxH = 0;

                foreach (char c in DefaultCharSet)
                {
                    if (c == ' ') continue;

                    var label = new CCLabelTTF(c.ToString(), fontName, fontSize);
                    if (label.Texture != null)
                    {
                        label.Texture.IsAntialiased = antialiased;
                        textures[c] = label.Texture;
                        float w = label.Texture.ContentSize.Width;
                        float h = label.Texture.ContentSize.Height;
                        widths[c] = w;
                        if (h > maxH) maxH = h;
                    }
                }

                s_fontCache[m_cacheKey] = textures;
                s_widthCache[m_cacheKey] = widths;
                s_heightCache[m_cacheKey] = maxH;
            }

            m_charTextures = textures;
            m_charWidths = s_widthCache[m_cacheKey];
            m_charHeight = s_heightCache[m_cacheKey];

            AllocateGlyphs(maxChars);

            m_text = text ?? "";
            if (m_text.Length > 0)
            {
                CacheCharacters(m_text);
                Layout();
            }
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
            foreach (char c in characters)
            {
                if (c == ' ' || m_charTextures.ContainsKey(c))
                    continue;

                var label = new CCLabelTTF(c.ToString(), m_fontName, m_fontSize);
                if (label.Texture != null)
                {
                    label.Texture.IsAntialiased = m_antialiased;
                    m_charTextures[c] = label.Texture;
                    float w = label.Texture.ContentSize.Width;
                    float h = label.Texture.ContentSize.Height;
                    m_charWidths[c] = w;

                    if (h > m_charHeight)
                    {
                        m_charHeight = h;
                        s_heightCache[m_cacheKey] = h;
                    }
                }
            }
        }

        /// <summary>
        /// Grows the glyph sprite pool if the current maxChars is insufficient.
        /// </summary>
        public void SetMaxChars(int maxChars)
        {
            if (maxChars <= m_maxChars) return;

            for (int i = 0; i < m_glyphs.Length; i++)
            {
                RemoveChild(m_glyphs[i], true);
            }

            m_maxChars = maxChars;
            AllocateGlyphs(maxChars);
            Layout();
        }

        /// <summary>
        /// Measures the width of a string in points without rendering it.
        /// </summary>
        public float MeasureString(string text)
        {
            if (text == null) return 0f;

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
        /// Clears all static font caches and disposes cached textures.
        /// Call this when changing scenes or when you need to free memory
        /// from fonts that are no longer used.
        /// </summary>
        public static void PurgeCachedData()
        {
            foreach (var fontEntry in s_fontCache.Values)
            {
                foreach (var texture in fontEntry.Values)
                {
                    texture.Dispose();
                }
            }

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
            // First pass: measure total width across full string
            float totalWidth = 0;
            for (int i = 0; i < m_text.Length; i++)
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

            // Second pass: position glyphs, iterating the full string
            // and only counting rendered characters against the sprite budget
            float x = offsetX;
            int glyphIdx = 0;

            for (int i = 0; i < m_text.Length; i++)
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
                glyph.Color = base.Color;
                glyph.Opacity = base.Opacity;
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
