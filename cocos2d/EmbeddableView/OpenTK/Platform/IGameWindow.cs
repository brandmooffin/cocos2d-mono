using System;

namespace cocos2d.EmbeddableView.OpenTK.Platform
{
    /// <summary>
    /// Defines the interface for a GameWindow.
    /// </summary>
    public interface IGameWindow : INativeWindow
    {
        /// <summary>
        /// Enters the game loop of the GameWindow using the maximum update rate.
        /// </summary>
        /// <seealso cref="Run(double)"/>
        void Run();

        /// <summary>
        /// Enters the game loop of the GameWindow using the specified update rate.
        /// </summary>
        void Run(double updateRate);

        /// <summary>
        /// Makes the GraphicsContext current on the calling thread.
        /// </summary>
        void MakeCurrent();

        /// <summary>
        /// Swaps the front and back buffers of the current GraphicsContext, presenting the rendered scene to the user.
        /// </summary>
        void SwapBuffers();

        /// <summary>
        /// Occurs before the window is displayed for the first time.
        /// </summary>
        event EventHandler<EventArgs> Load;

        /// <summary>
        /// Occurs before the window is destroyed.
        /// </summary>
        event EventHandler<EventArgs> Unload;

        /// <summary>
        /// Occurs when it is time to update a frame.
        /// </summary>
        event EventHandler<FrameEventArgs> UpdateFrame;

        /// <summary>
        /// Occurs when it is time to render a frame.
        /// </summary>
        event EventHandler<FrameEventArgs> RenderFrame;
    }
}

