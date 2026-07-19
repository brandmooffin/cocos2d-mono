using Cocos2D;
using Xunit;

namespace Cocos2DMono.UnitTests;

// Regression test for C2D-222: CCTextFieldTTF.CheckTouchState must clear the internal
// "registered" flag when it unregisters from the TouchDispatcher, so a field that goes
// editable -> readOnly can re-register when it becomes editable again. Registration is
// observed through the public TouchDispatcher.FindHandler (non-null == registered).
public class CCTextFieldTTFTests : System.IDisposable
{
    private CCTextFieldTTF? _field;

    [Fact]
    public void ReadOnlyToggle_ReRegistersTouchHandler()
    {
        var dispatcher = CCDirector.SharedDirector.TouchDispatcher;
        var field = new CCTextFieldTTF("hi", "arial", 12) { AutoEdit = true };
        _field = field; // tracked for teardown

        field.OnEnter();                                     // running + AutoEdit -> registers
        Assert.NotNull(dispatcher.FindHandler(field));

        field.ReadOnly = true;                               // unregisters
        Assert.Null(dispatcher.FindHandler(field));

        field.ReadOnly = false;                              // must re-register (the C2D-222 bug: it did not)
        Assert.NotNull(dispatcher.FindHandler(field));
    }

    public void Dispose()
    {
        // CCTextFieldTTF registers with the shared TouchDispatcher and, under DESKTOPGL,
        // schedules an Update on the shared scheduler. OnExit unregisters the touch delegate
        // (CheckTouchState runs with m_bRunning == false); Cleanup unschedules the update
        // (UnscheduleAllForTarget). Together they keep the shared singletons clean for later tests.
        if (_field != null)
        {
            _field.OnExit();
            _field.Cleanup();
            _field = null;
        }
    }
}
