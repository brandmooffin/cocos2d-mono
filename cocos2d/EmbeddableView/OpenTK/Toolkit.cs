using System;
using cocos2d.EmbeddableView.OpenTK.Platform;
using System.Diagnostics;

namespace cocos2d.EmbeddableView.OpenTK
{
    /// <summary>
    /// Provides static methods to manage an OpenTK application.
    /// </summary>
    public sealed class Toolkit : IDisposable
    {
        private Factory platform_factory;
        private static Toolkit toolkit;

        private volatile static bool initialized;
        private static readonly object InitLock = new object();

        private Toolkit(Factory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }
            platform_factory = factory;
        }

        /// <summary>
        /// Initializes OpenTK with default options.
        /// </summary>
        /// <remarks>
        /// <para>
        /// You *must* call this method if you are combining OpenTK with a
        /// third-party windowing toolkit (e.g. GTK#). In this case, this should be the
        /// first method called by your application:
        /// <code>
        /// static void Main()
        /// {
        ///     using (OpenTK.Toolkit.Init())
        ///     {
        ///      ...
        ///     }
        /// }
        /// </code>
        /// </para>
        /// <para>
        /// The reason is that some toolkits do not configure the underlying platform
        /// correctly or configure it in a way that is incompatible with OpenTK.
        /// Calling this method first ensures that OpenTK is given the chance to
        /// initialize itself and configure the platform correctly.
        /// </para>
        /// </remarks>
        /// <returns>
        /// An IDisposable instance that you can use to dispose of the resources
        /// consumed by OpenTK.
        /// </returns>
        public static Toolkit Init()
        {
            return Init(ToolkitOptions.Default);
        }

        /// <summary>
        /// Initializes OpenTK with the specified options. Use this method
        /// to influence the OpenTK.Platform implementation that will be used.
        /// </summary>
        /// <remarks>
        /// <para>
        /// You *must* call this method if you are combining OpenTK with a
        /// third-party windowing toolkit (e.g. GTK#). In this case, this should be the
        /// first method called by your application:
        /// <code>
        /// static void Main()
        /// {
        ///     using (OpenTK.Toolkit.Init())
        ///     {
        ///      ...
        ///     }
        /// }
        /// </code>
        /// </para>
        /// <para>
        /// The reason is that some toolkits do not configure the underlying platform
        /// correctly or configure it in a way that is incompatible with OpenTK.
        /// Calling this method first ensures that OpenTK is given the chance to
        /// initialize itself and configure the platform correctly.
        /// </para>
        /// </remarks>
        /// <param name="options">A <c>ToolkitOptions</c> instance
        /// containing the desired options.</param>
        /// <returns>
        /// An IDisposable instance that you can use to dispose of the resources
        /// consumed by OpenTK.
        /// </returns>
        public static Toolkit Init(ToolkitOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            lock (InitLock)
            {
                if (!initialized)
                {
                    initialized = true;
                    Configuration.Init(options);
                    Options = options;

                    // The actual initialization takes place in the
                    // platform-specific factory constructors.
                    toolkit = new Toolkit(new Factory());
                }
                return toolkit;
            }
        }

        internal static ToolkitOptions Options { get; private set; }

        /// <summary>
        /// Disposes of the resources consumed by this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool manual)
        {
            if (manual)
            {
                lock (InitLock)
                {
                    if (initialized)
                    {
                        platform_factory.Dispose();
                        platform_factory = null;
                        toolkit = null;
                        initialized = false;
                    }
                }
            }
        }

#if DEBUG
        /// <summary>
        /// Finalizes this instance.
        /// </summary>
        ~Toolkit()
        {
            Debug.Print("[Warning] {0} leaked, did you forget to call Dispose()?");
            // We may not Dispose() the toolkit from the finalizer thread,
            // as that will crash on many operating systems.
        }
#endif
    }
}

