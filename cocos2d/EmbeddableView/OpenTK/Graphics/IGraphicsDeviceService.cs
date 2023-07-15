using System;
namespace cocos2d.EmbeddableView.OpenTK.Graphics
{
    public interface IGraphicsDeviceService
    {
        GraphicsDevice GraphicsDevice { get; }

        event EventHandler<EventArgs> DeviceCreated;
        event EventHandler<EventArgs> DeviceDisposing;
        event EventHandler<EventArgs> DeviceReset;
        event EventHandler<EventArgs> DeviceResetting;
    }
}

