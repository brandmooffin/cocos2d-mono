using System;
namespace cocos2d.EmbeddableView.OpenTK
{
    /// <summary>
    /// Defines bitwise combianations of GameWindow construction options.
    /// </summary>
    [Flags]
    public enum GameWindowFlags
    {
        /// <summary>
        /// Indicates default construction options.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Indicates that the GameWindow should cover the whole screen.
        /// </summary>
        Fullscreen = 1,

        /// <summary>
        /// Indicates that the GameWindow should be a fixed window.
        /// </summary>
        FixedWindow = 2,
    }
}

