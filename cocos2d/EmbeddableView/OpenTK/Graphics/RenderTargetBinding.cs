using System;
using Microsoft.Xna.Framework.Graphics;

namespace cocos2d.EmbeddableView.OpenTK.Graphics
{
    // http://msdn.microsoft.com/en-us/library/ff434403.aspx
    public struct RenderTargetBinding
    {
        private readonly Texture _renderTarget;
        private readonly int _arraySlice;

        public Texture RenderTarget
        {
            get { return _renderTarget; }
        }

        public int ArraySlice
        {
            get { return _arraySlice; }
        }

        public RenderTargetBinding(RenderTarget2D renderTarget)
        {
            if (renderTarget == null)
                throw new ArgumentNullException("renderTarget");

            _renderTarget = renderTarget;
            _arraySlice = (int)CubeMapFace.PositiveX;
        }

        public RenderTargetBinding(RenderTargetCube renderTarget, CubeMapFace cubeMapFace)
        {
            if (renderTarget == null)
                throw new ArgumentNullException("renderTarget");
            if (cubeMapFace < CubeMapFace.PositiveX || cubeMapFace > CubeMapFace.NegativeZ)
                throw new ArgumentOutOfRangeException("cubeMapFace");

            _renderTarget = renderTarget;
            _arraySlice = (int)cubeMapFace;
        }

#if DIRECTX

        public RenderTargetBinding(RenderTarget2D renderTarget, int arraySlice)
        {
            if (renderTarget == null)
                throw new ArgumentNullException("renderTarget");
            if (arraySlice < 0 || arraySlice >= renderTarget.ArraySize)
                throw new ArgumentOutOfRangeException("arraySlice");
            if (!renderTarget.GraphicsDevice.GraphicsCapabilities.SupportsTextureArrays)
                throw new InvalidOperationException("Texture arrays are not supported on this graphics device");

            _renderTarget = renderTarget;
            _arraySlice = arraySlice;
        }

        public RenderTargetBinding(RenderTarget3D renderTarget)
        {
            if (renderTarget == null)
                throw new ArgumentNullException("renderTarget");

            _renderTarget = renderTarget;
            _arraySlice = 0;
        }

        public RenderTargetBinding(RenderTarget3D renderTarget, int arraySlice)
        {
            if (renderTarget == null)
                throw new ArgumentNullException("renderTarget");
            if (arraySlice < 0 || arraySlice >= renderTarget.Depth)
                throw new ArgumentOutOfRangeException("arraySlice");

            _renderTarget = renderTarget;
            _arraySlice = arraySlice;
        }

#endif 

        public static implicit operator RenderTargetBinding(RenderTarget2D renderTarget)
        {
            return new RenderTargetBinding(renderTarget);
        }

#if DIRECTX

        public static implicit operator RenderTargetBinding(RenderTarget3D renderTarget)
        {
            return new RenderTargetBinding(renderTarget);
        }

#endif
    }
}

