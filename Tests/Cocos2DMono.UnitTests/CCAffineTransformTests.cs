using Cocos2D;
using Xunit;

namespace Cocos2DMono.UnitTests;

public class CCAffineTransformTests
{
    private const int Precision = 5;

    [Fact]
    public void Identity_LeavesPointUnchanged()
    {
        var p = new CCPoint(3f, 4f);
        Assert.Equal(p, CCAffineTransform.Transform(p, CCAffineTransform.Identity));
    }

    [Fact]
    public void Translate_OffsetsPoint()
    {
        var t = CCAffineTransform.Translate(CCAffineTransform.Identity, 5f, 7f);
        Assert.Equal(new CCPoint(6f, 9f), CCAffineTransform.Transform(new CCPoint(1f, 2f), t));
    }

    [Fact]
    public void Scale_ScalesPoint()
    {
        var t = CCAffineTransform.Scale(CCAffineTransform.Identity, 2f, 3f);
        Assert.Equal(new CCPoint(8f, 15f), CCAffineTransform.Transform(new CCPoint(4f, 5f), t));
    }

    [Fact]
    public void Concat_WithIdentity_IsNoOp()
    {
        var t = new CCAffineTransform(2f, 0f, 0f, 3f, 5f, 7f);
        Assert.True(CCAffineTransform.Equal(t, CCAffineTransform.Concat(t, CCAffineTransform.Identity)));
        Assert.True(CCAffineTransform.Equal(t, CCAffineTransform.Concat(CCAffineTransform.Identity, t)));
    }

    [Fact]
    public void Invert_RoundTripsPoint()
    {
        // scale by (2,4) then translate by (3,5)
        var t = CCAffineTransform.Translate(
            CCAffineTransform.Scale(CCAffineTransform.Identity, 2f, 4f), 3f, 5f);
        var p = new CCPoint(7f, 9f);

        var forward = CCAffineTransform.Transform(p, t);
        var back = CCAffineTransform.Transform(forward, CCAffineTransform.Invert(t));

        Assert.Equal(p.X, back.X, Precision);
        Assert.Equal(p.Y, back.Y, Precision);
    }

    [Fact]
    public void Rotate90_MapsXAxisToYAxis()
    {
        var t = CCAffineTransform.Rotate(CCAffineTransform.Identity, (float)(Math.PI / 2.0));
        var r = CCAffineTransform.Transform(new CCPoint(1f, 0f), t);
        Assert.Equal(0f, r.X, Precision);
        Assert.Equal(1f, r.Y, Precision);
    }

    [Fact]
    public void Concat_ScaleThenTranslate_TransformsPoint()
    {
        var scale = CCAffineTransform.Scale(CCAffineTransform.Identity, 2f, 2f);
        var translate = CCAffineTransform.Translate(CCAffineTransform.Identity, 3f, 5f);

        // Concat(scale, translate) applies the scale first, then the translate.
        var combined = CCAffineTransform.Concat(scale, translate);

        // (1,1) -> *2 -> (2,2) -> +(3,5) -> (5,7)
        Assert.Equal(new CCPoint(5f, 7f), CCAffineTransform.Transform(new CCPoint(1f, 1f), combined));
    }

    [Fact]
    public void Transform_Rect_ProducesScaledAabb()
    {
        var scale = CCAffineTransform.Scale(CCAffineTransform.Identity, 2f, 2f);

        // Rect (1,1)-(3,3) scaled by 2 -> (2,2)-(6,6)
        var r = scale.Transform(new CCRect(1f, 1f, 2f, 2f));

        Assert.Equal(2f, r.MinX);
        Assert.Equal(2f, r.MinY);
        Assert.Equal(4f, r.Size.Width);
        Assert.Equal(4f, r.Size.Height);
    }
}
