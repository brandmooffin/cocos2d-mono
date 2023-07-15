using System;
namespace cocos2d.EmbeddableView.OpenTK.Input
{
    /// <summary>
    /// Enumerates modifier keys.
    /// </summary>
    [Flags]
    public enum KeyModifiers : byte
    {
        /// <summary>
        /// The alt key modifier (option on Mac).
        /// </summary>
        Alt = 1 << 0,

        /// <summary>
        /// The control key modifier.
        /// </summary>
        Control = 1 << 1,

        /// <summary>
        /// The shift key modifier.
        /// </summary>
        Shift = 1 << 2
    }
}

