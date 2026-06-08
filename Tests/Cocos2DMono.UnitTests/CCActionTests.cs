using Cocos2D;
using Xunit;

namespace Cocos2DMono.UnitTests;

// Actions are started via the public RunAction (which calls the internal StartWithTarget
// synchronously). A fresh node is not "running", so the action is added paused and will not
// auto-step; we then drive it deterministically with the public Update(normalizedTime).
public class CCActionTests
{
    private const int P = 4;

    [Fact]
    public void MoveTo_InterpolatesAbsolutePosition()
    {
        var node = new CCNode { Position = new CCPoint(0f, 0f) };
        var move = new CCMoveTo(1f, new CCPoint(10f, 20f));
        node.RunAction(move);

        move.Update(0f);
        Assert.Equal(new CCPoint(0f, 0f), node.Position);
        move.Update(0.5f);
        Assert.Equal(new CCPoint(5f, 10f), node.Position);
        move.Update(1f);
        Assert.Equal(new CCPoint(10f, 20f), node.Position);
    }

    [Fact]
    public void MoveBy_AddsDeltaToStart()
    {
        var node = new CCNode { Position = new CCPoint(5f, 5f) };
        var move = new CCMoveBy(1f, new CCPoint(10f, 0f));
        node.RunAction(move);

        move.Update(1f);
        Assert.Equal(new CCPoint(15f, 5f), node.Position);
    }

    [Fact]
    public void ScaleTo_Uniform_InterpolatesScale()
    {
        var node = new CCNode();   // default scale 1
        var scale = new CCScaleTo(1f, 3f);
        node.RunAction(scale);

        scale.Update(0.5f);
        Assert.Equal(2f, node.ScaleX, P);
        Assert.Equal(2f, node.ScaleY, P);
        scale.Update(1f);
        Assert.Equal(3f, node.ScaleX, P);
        Assert.Equal(3f, node.ScaleY, P);
    }

    [Fact]
    public void ScaleTo_NonUniform_InterpolatesEachAxis()
    {
        var node = new CCNode();
        var scale = new CCScaleTo(1f, 2f, 4f);
        node.RunAction(scale);

        scale.Update(1f);
        Assert.Equal(2f, node.ScaleX, P);
        Assert.Equal(4f, node.ScaleY, P);
    }

    [Fact]
    public void RotateTo_InterpolatesRotation()
    {
        var node = new CCNode();   // default rotation 0
        var rotate = new CCRotateTo(1f, 90f);
        node.RunAction(rotate);

        rotate.Update(0.5f);
        Assert.Equal(45f, node.RotationX, P);
        rotate.Update(1f);
        Assert.Equal(90f, node.RotationX, P);
        Assert.Equal(90f, node.RotationY, P);
    }

    [Fact]
    public void FadeTo_InterpolatesOpacity()
    {
        var node = new CCNode();   // default opacity 255
        var fade = new CCFadeTo(1f, 0);
        node.RunAction(fade);

        fade.Update(0.5f);
        Assert.Equal(127, node.Opacity);  // (byte)(255 + (0-255)*0.5) = 127
        fade.Update(1f);
        Assert.Equal(0, node.Opacity);
    }

    [Fact]
    public void DelayTime_DoesNotChangeTarget()
    {
        var node = new CCNode { Position = new CCPoint(5f, 5f) };
        var delay = new CCDelayTime(1f);
        node.RunAction(delay);

        delay.Update(0.5f);
        Assert.Equal(new CCPoint(5f, 5f), node.Position);
    }

    [Fact]
    public void Sequence_FirstHalf_RunsFirstAction()
    {
        var node = new CCNode { Position = new CCPoint(0f, 0f) };
        var seq = new CCSequence(new CCMoveBy(1f, new CCPoint(10f, 0f)),
                                 new CCMoveBy(1f, new CCPoint(0f, 10f)));
        node.RunAction(seq);

        // total duration 2, split at 0.5; t=0.25 -> first action at its halfway point
        seq.Update(0.25f);
        Assert.Equal(new CCPoint(5f, 0f), node.Position);
    }

    [Fact]
    public void Sequence_AtEnd_AppliesBothActions()
    {
        var node = new CCNode { Position = new CCPoint(0f, 0f) };
        var seq = new CCSequence(new CCMoveBy(1f, new CCPoint(10f, 0f)),
                                 new CCMoveBy(1f, new CCPoint(0f, 10f)));
        node.RunAction(seq);

        seq.Update(1f);
        Assert.Equal(new CCPoint(10f, 10f), node.Position);
    }

    [Fact]
    public void Spawn_RunsBothActionsInParallel()
    {
        var node = new CCNode { Position = new CCPoint(0f, 0f) };  // default scale 1
        var spawn = new CCSpawn(new CCMoveBy(1f, new CCPoint(10f, 20f)),
                                new CCScaleTo(1f, 2f));
        node.RunAction(spawn);

        spawn.Update(1f);
        Assert.Equal(new CCPoint(10f, 20f), node.Position);
        Assert.Equal(2f, node.ScaleX, P);
        Assert.Equal(2f, node.ScaleY, P);
    }
}
