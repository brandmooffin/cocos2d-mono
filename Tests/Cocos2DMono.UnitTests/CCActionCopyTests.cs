using Cocos2D;
using Xunit;

namespace Cocos2DMono.UnitTests;

// Regression tests for the instant actions' copy constructors.
//
// Each of these actions exposes Copy(ICCCopyable). When passed null it routes to the
// protected copy constructor - `return new CCFlipX(this)` - which is the only way to
// reach that constructor from outside. The copied action's state is private, so these
// tests observe it the way a consumer would: run the copy and assert on the target.
public class CCActionCopyTests : System.IDisposable
{
    [Fact]
    public void CCPlace_Copy_PreservesPosition()
    {
        var expected = new CCPoint(10f, 20f);
        var place = new CCPlace(expected);

        var copy = (CCPlace)place.Copy(null);

        var node = new CCNode();
        node.RunAction(copy);
        Assert.Equal(expected, node.Position);
    }

    [Fact]
    public void CCFlipX_Copy_PreservesFlip()
    {
        var flip = new CCFlipX(true);

        var copy = (CCFlipX)flip.Copy(null);

        var sprite = new CCSprite();
        sprite.RunAction(copy);
        Assert.True(sprite.FlipX);
    }

    [Fact]
    public void CCFlipY_Copy_PreservesFlip()
    {
        var flip = new CCFlipY(true);

        var copy = (CCFlipY)flip.Copy(null);

        var sprite = new CCSprite();
        sprite.RunAction(copy);
        Assert.True(sprite.FlipY);
    }

    [Fact]
    public void CCCallFuncN_InitWithTarget_ReturnsTrueOnSuccess()
    {
        // Matches CCCallFuncO.InitWithTarget, which returns true.
        var callFuncN = new CCCallFuncN(_ => { });

        Assert.True(callFuncN.InitWithTarget(_ => { }));
    }

    [Fact]
    public void CCCallFuncN_Copy_PreservesSelector()
    {
        var invoked = false;
        var callFuncN = new CCCallFuncN(_ => invoked = true);

        var copy = (CCCallFuncN)callFuncN.Copy(null);

        // CCCallFunc fires its selector in Update (not StartWithTarget), so drive it:
        // RunAction sets the target, then Update triggers Execute.
        var node = new CCNode();
        node.RunAction(copy);
        copy.Update(1f);
        Assert.True(invoked);
    }

    public void Dispose()
    {
        // CCNode.RunAction registers actions with the shared ActionManager; clean it up to keep tests isolated.
        CCDirector.SharedDirector.ActionManager.RemoveAllActions();
    }
}
