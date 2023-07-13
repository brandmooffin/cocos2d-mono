using System;
using Cocos2D;

namespace cocos2d.Events
{
    public class CCEventAccelerate : CCEvent
    {

        // Set the Acceleration data 
        public CCAcceleration Acceleration { get; internal set; }

        internal CCEventAccelerate()
            : base(CCEventType.ACCELERATION)
        { }
    }
}

