using System;
namespace cocos2d.EmbeddableView.OpenTK
{
    /// <summary>
    /// Enumerates options regarding OpenTK.Platform
    /// implementations.
    /// </summary>
    public enum PlatformBackend
    {
        /// <summary>
        /// Select the optimal OpenTK.Platform implementation
        /// for the current operating system. This is the default
        /// option.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Prefer native OpenTK.Platform implementations.
        /// Platform abstractions such as SDL will not be considered,
        /// even if available. Use this if you need support for multiple
        /// mice or keyboards.
        /// </summary>
        PreferNative,
        /// <summary>
        /// Prefer an X11 OpenTK.Platform implementation,
        /// even if a different implementation is available. This option
        /// allows you to use X11 on Windows or Mac OS X when an
        /// X11 server is installed.
        /// </summary>
        PreferX11
    }

    /// <summary>
    /// Contains configuration options for OpenTK.
    /// <see cref="Toolkit.Init(ToolkitOptions)"/>
    /// </summary>
    public class ToolkitOptions
    {
        static ToolkitOptions()
        {
            Default = new ToolkitOptions();
            Default.EnableHighResolution = true;
        }

        /// <summary>
        /// Get or set the desired <c>PlatformBackend</c>
        /// for the OpenTK.Platform implementation.
        /// </summary>
        public PlatformBackend Backend { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether high
        /// resolution modes are supported on high-DPI
        /// ("Retina") displays. Enabled by default.
        /// Set to false for applications that are not
        /// DPI-aware (e.g. WinForms.)
        /// </summary>
        /// See: http://msdn.microsoft.com/en-us/library/windows/desktop/ee308410(v=vs.85).aspx
        public bool EnableHighResolution { get; set; }

        /// <summary>
        /// Gets a <c>ToolkitOptions</c> instance with
        /// default values.
        /// </summary>
        public static ToolkitOptions Default { get; private set; }
    }
}

