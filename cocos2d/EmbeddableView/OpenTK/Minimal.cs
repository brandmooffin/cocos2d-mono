using System;
namespace cocos2d.EmbeddableView.OpenTK
{
    public sealed class Icon : IDisposable
    {
        private IntPtr handle;

        public Icon(Icon icon, int width, int height)
        {
            handle = icon.Handle;
            Width = width;
            Height = height;
        }

        public IntPtr Handle { get { return handle; } set { handle = value; } }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Bitmap ToBitmap()
        {
            return new Bitmap(Width, Height);
        }

        public void Dispose()
        { }

        public static Icon ExtractAssociatedIcon(string location)
        {
            return null;
        }
    }
}

