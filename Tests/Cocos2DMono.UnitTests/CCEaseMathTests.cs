using Cocos2D;
using Xunit;

namespace Cocos2DMono.UnitTests;

public class CCEaseMathTests
{
    // Decimal-place precision for float comparisons (trig/pow eases carry small error).
    private const int P = 3;

    [Fact]
    public void Linear_IsIdentity()
    {
        Assert.Equal(0f, CCEaseMath.Linear(0f), P);
        Assert.Equal(0.5f, CCEaseMath.Linear(0.5f), P);
        Assert.Equal(1f, CCEaseMath.Linear(1f), P);
    }

    [Theory]
    [InlineData(0f, 0f)]
    [InlineData(0.5f, 0.25f)]
    [InlineData(1f, 1f)]
    public void QuadIn_MatchesSquare(float t, float expected) => Assert.Equal(expected, CCEaseMath.QuadIn(t), P);

    [Theory]
    [InlineData(0f, 0f)]
    [InlineData(0.5f, 0.75f)]
    [InlineData(1f, 1f)]
    public void QuadOut_MatchesFormula(float t, float expected) => Assert.Equal(expected, CCEaseMath.QuadOut(t), P);

    [Theory]
    [InlineData(0f, 0f)]
    [InlineData(0.25f, 0.125f)]
    [InlineData(0.5f, 0.5f)]
    [InlineData(1f, 1f)]
    public void QuadInOut_MatchesFormula(float t, float expected) => Assert.Equal(expected, CCEaseMath.QuadInOut(t), P);

    [Theory]
    [InlineData(0f, 0f)]
    [InlineData(0.5f, 0.125f)]
    [InlineData(1f, 1f)]
    public void CubicIn_MatchesCube(float t, float expected) => Assert.Equal(expected, CCEaseMath.CubicIn(t), P);

    [Theory]
    [InlineData(0f, 0f)]
    [InlineData(0.5f, 0.875f)]
    [InlineData(1f, 1f)]
    public void CubicOut_MatchesFormula(float t, float expected) => Assert.Equal(expected, CCEaseMath.CubicOut(t), P);

    [Theory]
    [InlineData(0f, 0f)]
    [InlineData(0.5f, 0.5f)]
    [InlineData(1f, 1f)]
    public void CubicInOut_MatchesFormula(float t, float expected) => Assert.Equal(expected, CCEaseMath.CubicInOut(t), P);

    [Fact]
    public void Back_HitsEndpoints_AndOvershoots()
    {
        Assert.Equal(0f, CCEaseMath.BackIn(0f), P);
        Assert.Equal(1f, CCEaseMath.BackIn(1f), P);
        Assert.True(CCEaseMath.BackIn(0.2f) < 0f, "BackIn should dip below 0 (anticipation)");

        Assert.Equal(0f, CCEaseMath.BackOut(0f), P);
        Assert.Equal(1f, CCEaseMath.BackOut(1f), P);
        Assert.True(CCEaseMath.BackOut(0.8f) > 1f, "BackOut should overshoot above 1");
    }

    [Fact]
    public void BackInOut_HitsEndpoints()
    {
        Assert.Equal(0f, CCEaseMath.BackInOut(0f), P);
        Assert.Equal(1f, CCEaseMath.BackInOut(1f), P);
    }

    [Fact]
    public void Bounce_HitsEndpoints()
    {
        Assert.Equal(0f, CCEaseMath.BounceOut(0f), P);
        Assert.Equal(1f, CCEaseMath.BounceOut(1f), P);
        Assert.Equal(0f, CCEaseMath.BounceIn(0f), P);
        Assert.Equal(1f, CCEaseMath.BounceIn(1f), P);
        Assert.Equal(0f, CCEaseMath.BounceInOut(0f), P);
        Assert.Equal(1f, CCEaseMath.BounceInOut(1f), P);
    }

    [Fact]
    public void Sine_HitsEndpoints()
    {
        Assert.Equal(0f, CCEaseMath.SineIn(0f), P);
        Assert.Equal(1f, CCEaseMath.SineIn(1f), P);
        Assert.Equal(0f, CCEaseMath.SineOut(0f), P);
        Assert.Equal(1f, CCEaseMath.SineOut(1f), P);
        Assert.Equal(0f, CCEaseMath.SineInOut(0f), P);
        Assert.Equal(1f, CCEaseMath.SineInOut(1f), P);
    }

    [Fact]
    public void Exponential_Endpoints()
    {
        Assert.Equal(0f, CCEaseMath.ExponentialOut(0f), P);
        Assert.Equal(1f, CCEaseMath.ExponentialOut(1f), P);
        // Characterization: ExponentialInOut does not special-case endpoints; it lands at 1/2048 at t=0 and 2047/2048 at t=1.
        Assert.Equal(0.00048828125f, CCEaseMath.ExponentialInOut(0f), 8);
        Assert.Equal(0.9995117f, CCEaseMath.ExponentialInOut(1f), 7);

        Assert.Equal(0f, CCEaseMath.ExponentialIn(0f), P);
        // Characterization: ExponentialIn applies a small (-0.001) offset, so at t=1 it
        // lands at ~0.999 rather than exactly 1. Pinned so a future change is a conscious one.
        Assert.Equal(0.999f, CCEaseMath.ExponentialIn(1f), P);
    }

    [Fact]
    public void Elastic_HitsEndpoints()
    {
        Assert.Equal(0f, CCEaseMath.ElasticIn(0f), P);
        Assert.Equal(1f, CCEaseMath.ElasticIn(1f), P);
        Assert.Equal(0f, CCEaseMath.ElasticOut(0f), P);
        Assert.Equal(1f, CCEaseMath.ElasticOut(1f), P);
        Assert.Equal(0f, CCEaseMath.ElasticInOut(0f), P);
        Assert.Equal(1f, CCEaseMath.ElasticInOut(1f), P);
    }

    [Fact]
    public void Lerp_Interpolates()
    {
        Assert.Equal(0f, CCEaseMath.Lerp(0f, 10f, 0f), P);
        Assert.Equal(5f, CCEaseMath.Lerp(0f, 10f, 0.5f), P);
        Assert.Equal(10f, CCEaseMath.Lerp(0f, 10f, 1f), P);
    }

    [Fact]
    public void ExpSmooth_AtDtZero_DoesNotMove()
    {
        Assert.Equal(3f, CCEaseMath.ExpSmooth(3f, 10f, 0.5f, 0f), P);
    }

    [Fact]
    public void ExpSmooth_MovesTowardTarget()
    {
        // smoothing 0.5, dt 1 => factor 1 - 0.5^1 = 0.5 => halfway from 0 to 10
        Assert.Equal(5f, CCEaseMath.ExpSmooth(0f, 10f, 0.5f, 1f), P);
    }

    [Fact]
    public void ExpSmooth_ClampsNegativeDtToZero()
    {
        Assert.Equal(2f, CCEaseMath.ExpSmooth(2f, 10f, 0.5f, -5f), P);
    }
}
