using Cocos2D;
using Xunit;

namespace Cocos2DMono.UnitTests;

public class CCVector2Tests
{
    private const int P = 5;

    [Fact]
    public void Ctor_SetsXY()
    {
        var v = new CCVector2(3f, 4f);
        Assert.Equal(3f, v.X);
        Assert.Equal(4f, v.Y);
    }

    [Fact]
    public void StaticConstants()
    {
        Assert.Equal(new CCVector2(0f, 0f), CCVector2.Zero);
        Assert.Equal(new CCVector2(1f, 1f), CCVector2.One);
        Assert.Equal(new CCVector2(1f, 0f), CCVector2.UnitX);
        Assert.Equal(new CCVector2(0f, 1f), CCVector2.UnitY);
    }

    [Fact]
    public void Addition_Subtraction_Negation()
    {
        Assert.Equal(new CCVector2(4f, 6f), new CCVector2(1f, 2f) + new CCVector2(3f, 4f));
        Assert.Equal(new CCVector2(2f, 2f), new CCVector2(5f, 7f) - new CCVector2(3f, 5f));
        Assert.Equal(new CCVector2(-3f, 4f), -new CCVector2(3f, -4f));
    }

    [Fact]
    public void ScalarMultiply_BothOrders_And_Divide()
    {
        Assert.Equal(new CCVector2(2f, 4f), new CCVector2(1f, 2f) * 2f);
        Assert.Equal(new CCVector2(2f, 4f), 2f * new CCVector2(1f, 2f));
        Assert.Equal(new CCVector2(1f, 2f), new CCVector2(2f, 4f) / 2f);
    }

    [Fact]
    public void Dot_Product()
    {
        // 1*3 + 2*4 = 11
        Assert.Equal(11f, CCVector2.Dot(new CCVector2(1f, 2f), new CCVector2(3f, 4f)), P);
    }

    [Fact]
    public void Distance_And_DistanceSquared()
    {
        Assert.Equal(5f, CCVector2.Distance(new CCVector2(0f, 0f), new CCVector2(3f, 4f)), P);
        Assert.Equal(25f, CCVector2.DistanceSquared(new CCVector2(0f, 0f), new CCVector2(3f, 4f)), P);
    }

    [Fact]
    public void Length_And_LengthSquared()
    {
        var v = new CCVector2(3f, 4f);
        Assert.Equal(5f, v.Length(), P);
        Assert.Equal(25f, v.LengthSquared(), P);
    }

    [Fact]
    public void Normalize_Static_ProducesUnitVector()
    {
        var n = CCVector2.Normalize(new CCVector2(3f, 4f));
        Assert.Equal(0.6f, n.X, P);
        Assert.Equal(0.8f, n.Y, P);
        Assert.Equal(1f, n.Length(), P);
    }

    [Fact]
    public void Lerp_AtHalf_IsMidpoint()
    {
        Assert.Equal(new CCVector2(5f, 10f), CCVector2.Lerp(new CCVector2(0f, 0f), new CCVector2(10f, 20f), 0.5f));
    }

    [Fact]
    public void Equality_OperatorsAndEquals()
    {
        var a = new CCVector2(1f, 2f);
        var b = new CCVector2(1f, 2f);
        var c = new CCVector2(2f, 1f);

        Assert.True(a == b);
        Assert.True(a != c);
        Assert.True(a.Equals(b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }
}
