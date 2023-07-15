using System;
using Microsoft.Xna.Framework.Graphics;

namespace cocos2d.EmbeddableView.OpenTK.Graphics
{
    /// <summary>
    /// Represents a render target.
    /// </summary>
    internal interface IRenderTarget
    {
        /// <summary>
        /// Gets the width of the render target in pixels
        /// </summary>
        /// <value>The width of the render target in pixels.</value>
        int Width { get; }

        /// <summary>
        /// Gets the height of the render target in pixels
        /// </summary>
        /// <value>The height of the render target in pixels.</value>
        int Height { get; }

        /// <summary>
        /// Gets the usage mode of the render target.
        /// </summary>
        /// <value>The usage mode of the render target.</value>
        RenderTargetUsage RenderTargetUsage { get; }

#if DIRECTX
        /// <summary>
        /// Gets the <see cref="RenderTargetView"/> for the specified array slice.
        /// </summary>
        /// <param name="arraySlice">The array slice.</param>
        /// <returns>The <see cref="RenderTargetView"/>.</returns>
        /// <remarks>
        /// For texture cubes: The array slice is the index of the cube map face.
        /// </remarks>
        RenderTargetView GetRenderTargetView(int arraySlice);

        /// <summary>
        /// Gets the <see cref="DepthStencilView"/>.
        /// </summary>
        /// <returns>The <see cref="DepthStencilView"/>. Can be <see langword="null"/>.</returns>
        DepthStencilView GetDepthStencilView();
#endif
    }
}

