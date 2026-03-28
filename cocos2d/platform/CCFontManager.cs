using System;
using System.Collections.Generic;
using System.IO;

namespace Cocos2D
{
    /// <summary>
    /// Manages font registration and provides utilities for working with fonts
    /// across the framework. Supports both content pipeline fonts (.xnb via SpriteFont)
    /// and direct TTF file loading (bypasses the content pipeline).
    ///
    /// For direct TTF loading (no content pipeline required), use CCLabel:
    /// <code>
    /// var label = new CCLabel("Hello", "fonts/myfont.ttf", 24);
    /// </code>
    ///
    /// For content pipeline fonts, use CCLabelTTF:
    /// <code>
    /// var label = new CCLabelTTF("Hello", "arial", 24);
    /// </code>
    ///
    /// If the content pipeline fails for certain fonts (e.g. MonoGame's
    /// FontDescriptionProcessor rejects the font), switch to CCLabel with
    /// a .ttf file path instead.
    /// </summary>
    public static class CCFontManager
    {
        private static readonly HashSet<string> s_registeredTTFPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Registers a TTF font file path for use with CCLabel.
        /// The path should be relative to the content root (e.g. "fonts/myfont.ttf").
        /// The TTF file must be included in your content directory.
        /// </summary>
        /// <param name="relativePath">Path relative to content root, e.g. "fonts/myfont.ttf".</param>
        /// <returns>True if the font file exists and was registered.</returns>
        public static bool RegisterTTF(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return false;

            string fullPath = ResolveFontPath(relativePath);

            if (fullPath != null && File.Exists(fullPath))
            {
                s_registeredTTFPaths.Add(relativePath);
                return true;
            }

            CCLog.Log("CCFontManager: TTF file not found at '{0}'", relativePath);
            return false;
        }

        /// <summary>
        /// Checks whether a TTF font has been registered.
        /// </summary>
        public static bool IsTTFRegistered(string relativePath)
        {
            return s_registeredTTFPaths.Contains(relativePath);
        }

        /// <summary>
        /// Returns all registered TTF font paths.
        /// </summary>
        public static IEnumerable<string> RegisteredTTFFonts
        {
            get { return s_registeredTTFPaths; }
        }

        /// <summary>
        /// Resolves a font-relative path to a full filesystem path.
        /// Returns null if the content manager is not initialized.
        /// </summary>
        public static string ResolveFontPath(string relativePath)
        {
            try
            {
                string rootDir = null;

                if (CCContentManager.SharedContentManager != null)
                {
                    rootDir = CCContentManager.SharedContentManager.RootDirectory;
                }
                else if (CCApplication.SharedApplication != null)
                {
                    rootDir = CCApplication.SharedApplication.Content.RootDirectory;
                }

                if (rootDir != null)
                {
                    string appPath = AppDomain.CurrentDomain.BaseDirectory;
                    return Path.Combine(appPath, rootDir, relativePath);
                }
            }
            catch (Exception ex)
            {
                CCLog.Log("CCFontManager: Error resolving font path: {0}", ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Checks whether a font name refers to a TTF file (ends with .ttf extension).
        /// </summary>
        public static bool IsTTFFont(string fontName)
        {
            if (string.IsNullOrEmpty(fontName))
                return false;

            return fontName.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Creates the appropriate label type for a given font.
        /// If the font name ends with .ttf, creates a CCLabel (native TTF rendering).
        /// Otherwise, creates a CCLabelTTF (content pipeline SpriteFont rendering).
        /// This is the recommended way to create labels when you want automatic
        /// content pipeline fallback.
        /// </summary>
        /// <param name="text">Text to display.</param>
        /// <param name="fontName">Font name or TTF path (e.g. "arial" or "fonts/myfont.ttf").</param>
        /// <param name="fontSize">Font size in points.</param>
        /// <returns>A CCNode that displays the text (either CCLabel or CCLabelTTF).</returns>
        public static CCNode CreateLabel(string text, string fontName, float fontSize)
        {
            if (IsTTFFont(fontName))
            {
                return new CCLabel(text, fontName, fontSize);
            }
            return new CCLabelTTF(text, fontName, fontSize);
        }

        /// <summary>
        /// Creates the appropriate label type with alignment support.
        /// </summary>
        public static CCNode CreateLabel(string text, string fontName, float fontSize,
            CCSize dimensions, CCTextAlignment hAlignment, CCVerticalTextAlignment vAlignment)
        {
            if (IsTTFFont(fontName))
            {
                return new CCLabel(text, fontName, fontSize, dimensions, hAlignment, vAlignment);
            }
            return new CCLabelTTF(text, fontName, fontSize, dimensions, hAlignment, vAlignment);
        }

        /// <summary>
        /// Clears all registered fonts.
        /// </summary>
        public static void Clear()
        {
            s_registeredTTFPaths.Clear();
        }
    }
}
