using Cocos2D;
using Xunit;

namespace Cocos2DMono.UnitTests;

public class CCSizeTests
{
    [Fact]
    public void Ctor_SetsWidthAndHeight()
    {
        var s = new CCSize(640f, 480f);
        Assert.Equal(640f, s.Width);
        Assert.Equal(480f, s.Height);
    }

    [Fact]
    public void Equality_OperatorsAndHashCode()
    {
        Assert.True(new CCSize(1f, 2f) == new CCSize(1f, 2f));
        Assert.True(new CCSize(1f, 2f) != new CCSize(2f, 1f));
        Assert.Equal(new CCSize(1f, 2f).GetHashCode(), new CCSize(1f, 2f).GetHashCode());
    }

    [Fact]
    public void ScalarMultiply_And_Divide()
    {
        Assert.True(new CCSize(2f, 4f) == new CCSize(1f, 2f) * 2f);
        Assert.True(new CCSize(1f, 2f) == new CCSize(2f, 4f) / 2f);
    }

    [Fact]
    public void Center_IsHalfExtents()
    {
        Assert.Equal(new CCPoint(5f, 10f), new CCSize(10f, 20f).Center);
    }

    [Fact]
    public void Zero_IsEmpty()
    {
        Assert.True(CCSize.Zero == new CCSize(0f, 0f));
    }
}
