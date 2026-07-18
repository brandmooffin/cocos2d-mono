using Cocos2D;
using Xunit;

namespace Cocos2DMono.UnitTests;

// Regression tests for CCMenuItemSprite image-setter null handling.
//
// NormalImage / SelectedImage / DisabledImage all support assigning null to clear the image -
// their "add" branch is explicitly guarded by `if (value != null)`. But when an image was
// already present, the replacement path copied the old image's Position onto `value` with no
// null check, so clearing an existing image to null threw a NullReferenceException. These tests
// pin the clear-to-null behavior for each setter and confirm a non-null replacement still
// inherits the previous image's position (the behavior the null guard must preserve).
public class CCMenuItemSpriteTests
{
    [Fact]
    public void NormalImage_SetToNull_ClearsWithoutThrowing()
    {
        var item = new CCMenuItemSprite();
        item.NormalImage = new CCNode();

        item.NormalImage = null; // regression: previously threw NullReferenceException

        Assert.Null(item.NormalImage);
    }

    [Fact]
    public void SelectedImage_SetToNull_ClearsWithoutThrowing()
    {
        var item = new CCMenuItemSprite();
        item.SelectedImage = new CCNode();

        item.SelectedImage = null;

        Assert.Null(item.SelectedImage);
    }

    [Fact]
    public void DisabledImage_SetToNull_ClearsWithoutThrowing()
    {
        var item = new CCMenuItemSprite();
        item.DisabledImage = new CCNode();

        item.DisabledImage = null;

        Assert.Null(item.DisabledImage);
    }

    [Fact]
    public void NormalImage_ReplacedWithNonNull_InheritsPreviousPosition()
    {
        var item = new CCMenuItemSprite();
        var first = new CCNode { Position = new CCPoint(15f, 25f) };
        item.NormalImage = first;

        var replacement = new CCNode();
        item.NormalImage = replacement;

        Assert.Same(replacement, item.NormalImage);
        Assert.Equal(new CCPoint(15f, 25f), replacement.Position);
    }
}
