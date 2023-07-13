using System;
namespace cocos2d.Events
{
    public class CCEventGamePadDPad : CCEventGamePad
    {
        public CCGamePadButtonStatus Left { get; internal set; }
        public CCGamePadButtonStatus Up { get; internal set; }
        public CCGamePadButtonStatus Right { get; internal set; }
        public CCGamePadButtonStatus Down { get; internal set; }
        public CCPlayerIndex Player { get; internal set; }

        internal CCEventGamePadDPad()
            : base(CCGamePadEventType.GAMEPAD_DPAD)
        { }
    }
}

