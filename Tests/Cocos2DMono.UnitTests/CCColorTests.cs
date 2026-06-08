using Cocos2D;
using Xunit;

namespace Cocos2DMono.UnitTests;

public class CCColorTests
{
    // --- CCColor3B ---

    [Fact]
    public void Color3B_Ctor_SetsComponents()
    {
        var c = new CCColor3B(10, 20, 30);
        Assert.Equal(10, c.R);
        Assert.Equal(20, c.G);
        Assert.Equal(30, c.B);
    }

    [Fact]
    public void Color3B_PredefinedValues()
    {
        Assert.Equal(new CCColor3B(255, 255, 255), CCColor3B.White);
        Assert.Equal(new CCColor3B(255, 0, 0), CCColor3B.Red);
        Assert.Equal(new CCColor3B(0, 255, 0), CCColor3B.Green);
        Assert.Equal(new CCColor3B(0, 0, 255), CCColor3B.Blue);
        Assert.Equal(new CCColor3B(255, 127, 0), CCColor3B.Orange);
        Assert.Equal(new CCColor3B(166, 166, 166), CCColor3B.Gray);
        Assert.Equal(new CCColor3B(0, 0, 0), CCColor3B.Black);
    }

    [Fact]
    public void Color3B_AsColor4B_AddsOpaqueAlpha()
    {
        var c = new CCColor3B(10, 20, 30).AsColor4B();
        Assert.Equal(10, c.R);
        Assert.Equal(20, c.G);
        Assert.Equal(30, c.B);
        Assert.Equal(255, c.A);
    }

    [Fact]
    public void Color3B_AsColor4B_WithExplicitAlpha()
    {
        Assert.Equal(128, new CCColor3B(10, 20, 30).AsColor4B(128).A);
    }

    // --- CCColor4B ---

    [Fact]
    public void Color4B_ByteCtor_DefaultsAlphaTo255()
    {
        Assert.Equal(255, new CCColor4B(1, 2, 3).A);
    }

    [Fact]
    public void Color4B_PredefinedValues()
    {
        Assert.Equal(new CCColor4B(255, 255, 255, 255), CCColor4B.White);
        Assert.Equal(new CCColor4B(0, 0, 0, 0), CCColor4B.Transparent);
        Assert.Equal(new CCColor4B(255, 127, 0, 255), CCColor4B.Orange);
    }

    [Fact]
    public void Color4B_FloatCtor_TruncatesNotNormalizes()
    {
        // Characterization: the float ctor casts straight to byte ((byte)inr) and treats
        // inputs as already 0-255, NOT normalized 0-1. So 200.9f -> 200, and 0.9f -> 0.
        var c = new CCColor4B(200.9f, 100f, 50f, 255f);
        Assert.Equal(200, c.R);
        Assert.Equal(100, c.G);
        Assert.Equal(50, c.B);
        Assert.Equal(255, c.A);

        var sub = new CCColor4B(0.9f, 0.5f, 0.1f, 1f);
        Assert.Equal(0, sub.R);
        Assert.Equal(0, sub.G);
        Assert.Equal(0, sub.B);
        Assert.Equal(1, sub.A);
    }

    [Fact]
    public void Color4B_EqualityOperators()
    {
        Assert.True(new CCColor4B(1, 2, 3, 4) == new CCColor4B(1, 2, 3, 4));
        Assert.True(new CCColor4B(1, 2, 3, 4) != new CCColor4B(1, 2, 3, 5));
    }

    [Fact]
    public void Color4B_ToString_And_Parse_RoundTrip()
    {
        var c = new CCColor4B(10, 20, 30, 40);
        Assert.Equal("10,20,30,40", c.ToString());

        var parsed = CCColor4B.Parse("10,20,30,40");
        Assert.True(c == parsed);
    }

    [Fact]
    public void Color4B_Lerp_Midpoint_IsByteTruncated()
    {
        // 50% between (0,0,0,0) and (100,200,50,255): A=127.5 truncates to 127.
        var mid = CCColor4B.Lerp(new CCColor4B(0, 0, 0, 0), new CCColor4B(100, 200, 50, 255), 0.5f);
        Assert.Equal(50, mid.R);
        Assert.Equal(100, mid.G);
        Assert.Equal(25, mid.B);
        Assert.Equal(127, mid.A);
    }

    // --- Conversions ---

    [Fact]
    public void Implicit_Color3B_To_Color4B_AddsOpaqueAlpha()
    {
        CCColor4B c = new CCColor3B(10, 20, 30);
        Assert.Equal(10, c.R);
        Assert.Equal(20, c.G);
        Assert.Equal(30, c.B);
        Assert.Equal(255, c.A);
    }

    [Fact]
    public void Implicit_Color4B_To_Color3B_DropsAlpha()
    {
        CCColor3B c = new CCColor4B(10, 20, 30, 40);
        Assert.Equal(10, c.R);
        Assert.Equal(20, c.G);
        Assert.Equal(30, c.B);
    }
}
