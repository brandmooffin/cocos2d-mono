using System;
using Microsoft.Xna.Framework.Graphics;

#if MONOMAC
using MonoMac.OpenGL;
#elif DESKTOPGL
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
#elif GLES
using OpenTK.Graphics.ES20;
#endif

namespace cocos2d.EmbeddableView.OpenTK.Graphics
{
    public partial class RenderTarget2D
    {
        internal int glColorBuffer;
        internal int glDepthBuffer;
        internal int glStencilBuffer;

        private void PlatformConstruct(GraphicsDevice graphicsDevice, int width, int height, bool mipMap,
            SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage, bool shared)
        {
            Threading.BlockOnUIThread(() =>
            {
                graphicsDevice.PlatformCreateRenderTarget(this, width, height, mipMap, preferredFormat, preferredDepthFormat, preferredMultiSampleCount, usage);
            });


        }

        private void PlatformGraphicsDeviceResetting()
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                Threading.BlockOnUIThread(() =>
                {
                    this.GraphicsDevice.PlatformDeleteRenderTarget(this);
                });
            }

            base.Dispose(disposing);
        }
    }
}

