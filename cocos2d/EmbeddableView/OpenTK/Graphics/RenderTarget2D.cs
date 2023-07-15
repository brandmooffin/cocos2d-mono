using System;
using Microsoft.Xna.Framework.Graphics;

namespace cocos2d.EmbeddableView.OpenTK.Graphics
{
    public partial class RenderTarget2D : Texture2D, IRenderTarget
    {
        public DepthFormat DepthStencilFormat { get; private set; }

        public int MultiSampleCount { get; private set; }

        public RenderTargetUsage RenderTargetUsage { get; private set; }

        public bool IsContentLost { get { return false; } }

        public event EventHandler<EventArgs> ContentLost;

        private bool SuppressEventHandlerWarningsUntilEventsAreProperlyImplemented()
        {
            return ContentLost != null;
        }

        public RenderTarget2D(GraphicsDevice graphicsDevice, int width, int height, bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage, bool shared, int arraySize)
            : base(graphicsDevice, width, height, mipMap, preferredFormat, SurfaceType.RenderTarget, shared, arraySize)
        {
            DepthStencilFormat = preferredDepthFormat;
            MultiSampleCount = preferredMultiSampleCount;
            RenderTargetUsage = usage;

            PlatformConstruct(graphicsDevice, width, height, mipMap, preferredFormat, preferredDepthFormat, preferredMultiSampleCount, usage, shared);
        }

        public RenderTarget2D(GraphicsDevice graphicsDevice, int width, int height, bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage, bool shared)
            : this(graphicsDevice, width, height, mipMap, preferredFormat, preferredDepthFormat, preferredMultiSampleCount, usage, shared, 1)
        {

        }

        public RenderTarget2D(GraphicsDevice graphicsDevice, int width, int height, bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage)
            : this(graphicsDevice, width, height, mipMap, preferredFormat, preferredDepthFormat, preferredMultiSampleCount, usage, false)
        { }

        public RenderTarget2D(GraphicsDevice graphicsDevice, int width, int height, bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat)
            : this(graphicsDevice, width, height, mipMap, preferredFormat, preferredDepthFormat, 0, RenderTargetUsage.DiscardContents)
        { }

        public RenderTarget2D(GraphicsDevice graphicsDevice, int width, int height)
            : this(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents)
        { }

        /// <summary>
        /// Allows child class to specify the surface type, eg: a swap chain.
        /// </summary>        
        protected RenderTarget2D(GraphicsDevice graphicsDevice,
                        int width,
                        int height,
                        bool mipMap,
                        SurfaceFormat format,
                        DepthFormat depthFormat,
                        int preferredMultiSampleCount,
                        RenderTargetUsage usage,
                        SurfaceType surfaceType)
            : base(graphicsDevice, width, height, mipMap, format, surfaceType)
        {
            DepthStencilFormat = depthFormat;
            MultiSampleCount = preferredMultiSampleCount;
            RenderTargetUsage = usage;
        }

        protected internal override void GraphicsDeviceResetting()
        {
            PlatformGraphicsDeviceResetting();
            base.GraphicsDeviceResetting();
        }
    }
}

