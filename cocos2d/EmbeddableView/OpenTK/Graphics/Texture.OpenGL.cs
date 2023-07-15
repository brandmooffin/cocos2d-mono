using System;
using cocos2d.EmbeddableView.OpenTK.Graphics.ES20;

namespace cocos2d.EmbeddableView.OpenTK.Graphics
{
    public abstract partial class Texture
    {
        internal int glTexture = -1;
        internal TextureTarget glTarget;
        internal TextureUnit glTextureUnit = TextureUnit.Texture0;
        internal PixelInternalFormat glInternalFormat;
        internal PixelFormat glFormat;
        internal PixelType glType;
        internal SamplerState glLastSamplerState;

        private void PlatformGraphicsDeviceResetting()
        {
            DeleteGLTexture();
            glLastSamplerState = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                DeleteGLTexture();
                glLastSamplerState = null;
            }

            base.Dispose(disposing);
        }

        private void DeleteGLTexture()
        {
            if (glTexture > 0)
            {
                int texture = glTexture;
                Threading.BlockOnUIThread(() =>
                {
                    GL.DeleteTextures(1, ref texture);
                    GraphicsExtensions.CheckGLError();
                });
            }
            glTexture = -1;
        }
    }
}

