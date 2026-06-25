using Cocos2D;
using Xunit;

namespace Cocos2DMono.UnitTests;

public class CCNodeTests
{
    [Fact]
    public void Construct_Headless_DefaultsAreEmpty()
    {
        // Proves CCNode constructs without a graphics device: its ctor pulls the
        // Scheduler/ActionManager from the lazily-created shared director, which
        // initializes without GL.
        var node = new CCNode();

        Assert.Null(node.Parent);
        Assert.Equal(0, node.ChildrenCount);
    }

    [Fact]
    public void AddChild_SetsParentAndCount()
    {
        var parent = new CCNode();
        var child = new CCNode();

        parent.AddChild(child);

        Assert.Same(parent, child.Parent);
        Assert.Equal(1, parent.ChildrenCount);
    }

    [Fact]
    public void AddChild_WithTag_IsFoundByGetChildByTag()
    {
        var parent = new CCNode();
        var child = new CCNode();

        parent.AddChild(child, 0, 42);

        Assert.Equal(42, child.Tag);
        Assert.Same(child, parent.GetChildByTag(42));
    }

    [Fact]
    public void GetChildByTag_MissingTag_ReturnsNull()
    {
        var parent = new CCNode();
        parent.AddChild(new CCNode(), 0, 1);

        Assert.Null(parent.GetChildByTag(999));
    }

    [Fact]
    public void RemoveChild_ClearsParent_Count_AndTagIndex()
    {
        var parent = new CCNode();
        var child = new CCNode();
        parent.AddChild(child, 0, 7);

        parent.RemoveChild(child);

        Assert.Null(child.Parent);
        Assert.Equal(0, parent.ChildrenCount);
        // The tag index stays in sync: a removed child is no longer resolvable by its tag.
        Assert.Null(parent.GetChildByTag(7));
    }

    [Fact]
    public void RemoveChildByTag_RemovesTheTaggedChild()
    {
        var parent = new CCNode();
        var child = new CCNode();
        parent.AddChild(child, 0, 5);

        parent.RemoveChildByTag(5);

        Assert.Null(child.Parent);
        Assert.Equal(0, parent.ChildrenCount);
        Assert.Null(parent.GetChildByTag(5));
    }

    [Fact]
    public void RemoveAllChildren_EmptiesAndClearsParents()
    {
        var parent = new CCNode();
        var a = new CCNode();
        var b = new CCNode();
        parent.AddChild(a, 0, 1);
        parent.AddChild(b, 0, 2);

        parent.RemoveAllChildren();

        Assert.Equal(0, parent.ChildrenCount);
        Assert.Null(a.Parent);
        Assert.Null(b.Parent);
        Assert.Null(parent.GetChildByTag(1));
        Assert.Null(parent.GetChildByTag(2));
    }

    [Fact]
    public void Tag_ChangeAfterAdd_UpdatesParentIndex()
    {
        // Changing an attached child's Tag must re-register it in the parent's tag index.
        var parent = new CCNode();
        var child = new CCNode();
        parent.AddChild(child, 0, 10);

        child.Tag = 20;

        Assert.Null(parent.GetChildByTag(10));         // old tag no longer resolves
        Assert.Same(child, parent.GetChildByTag(20));  // new tag resolves
    }
}
