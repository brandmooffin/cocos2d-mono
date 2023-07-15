using System;
using cocos2d.EmbeddableView.OpenTK.Graphics;
using System.Diagnostics;

namespace cocos2d.EmbeddableView.OpenTK.Platform
{
    // Provides the foundation for all desktop IGraphicsContext implementations.
    internal abstract class EmbeddedGraphicsContext : GraphicsContextBase
    {
        public override void LoadAll()
        {
            Stopwatch time = Stopwatch.StartNew();

#if OPENGLES
            new OpenTK.Graphics.ES11.GL().LoadEntryPoints();
            new OpenTK.Graphics.ES20.GL().LoadEntryPoints();
            new OpenTK.Graphics.ES30.GL().LoadEntryPoints();
#endif

            Debug.Print("Bindings loaded in {0} ms.", time.Elapsed.TotalMilliseconds);
        }
    }
}

