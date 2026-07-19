using Cocos2D;
using Xunit;

namespace Cocos2DMono.UnitTests;

// Regression test for C2D-222: CCTextFieldTTF.CheckTouchState must clear the internal
// "registered" flag when it unregisters from the TouchDispatcher, so a field that goes
// editable -> readOnly can re-register when it becomes editable again. Registration is
// observed through the public TouchDispatcher.FindHandler (non-null == registered).
public class CCTextFieldTTFTests : System.IDisposable
{
    private CCTextFieldTTF _field;

    [Fact]
    public void ReadOnlyToggle_ReRegistersTouchHandler()
    {
        var dispatcher = CCDirector.SharedDirector.TouchDispatcher;
        _field = new CCTextFieldTTF("hi", "arial", 12) { AutoEdit = true };

        _field.OnEnter();                                    // running + AutoEdit -> registers
        Assert.NotNull(dispatcher.FindHandler(_field));

        _field.ReadOnly = true;                              // unregisters
        Assert.Null(dispatcher.FindHandler(_field));

        _field.ReadOnly = false;                             // must re-register (the C2D-222 bug: it did not)
        Assert.NotNull(dispatcher.FindHandler(_field));
    }

    public void Dispose()
    {
        if (_field != null)
        {
            CCDirector.SharedDirector.TouchDispatcher.RemoveDelegate(_field);
        }
    }
}
