using System;
namespace cocos2d.EmbeddableView.OpenTK.Graphics
{
    /// <summary>
    /// Enumerates various flags that affect the creation of new GraphicsContexts.
    /// </summary>
    [Flags]
    public enum GraphicsContextFlags
    {
        /// <summary>
        /// The default value of the GraphicsContextFlags enumeration.
        /// </summary>
        Default = 0x0000,
        /// <summary>
        /// Indicates that this is a debug GraphicsContext. Debug contexts may provide
        /// additional debugging information at the cost of performance.
        /// </summary>
        /// <remarks></remarks>
        Debug = 0x0001,
        /// <summary>
        /// Indicates that this is a forward compatible GraphicsContext. Forward-compatible contexts
        /// do not support functionality marked as deprecated in the current GraphicsContextVersion.
        /// </summary>
        /// <remarks>Forward-compatible contexts are defined only for OpenGL versions 3.0 and later.</remarks>
        ForwardCompatible = 0x0002,
        /// <summary>
        /// Indicates that this GraphicsContext is targeting OpenGL|ES.
        /// </summary>
        Embedded = 0x0004,
        /// <summary>
        /// Indicates that this GraphicsContext is intended for offscreen rendering.
        /// </summary>
        Offscreen = 0x0008,
        /// <summary>
        /// Indicates that this GraphicsContext is targeting OpenGL|ES via Angle
        /// and that angle-specific extensions are available.
        /// </summary>
        Angle = 0x0010,
        /// <summary>
        /// Indicates that this GraphicsContext is targeting OpenGL|ES via Angle
        /// and uses Direct3D9 as rendering backend.
        /// </summary>
        AngleD3D9 = 0x0020,
        /// <summary>
        /// Indicates that this GraphicsContext is targeting OpenGL|ES via Angle
        /// and uses Direct3D11 as rendering backend.
        /// </summary>
        AngleD3D11 = 0x0040,
        /// <summary>
        /// Indicates that this GraphicsContext is targeting OpenGL|ES via Angle
        /// and uses OpenGL as rendering backend.
        /// </summary>
        AngleOpenGL = 0x0080,
    }
}

