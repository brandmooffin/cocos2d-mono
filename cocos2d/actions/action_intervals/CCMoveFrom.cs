using Cocos2D;
using System;

namespace cocos2d.actions.action_intervals
{
    public class CCMoveFrom : CCFiniteTimeAction
    {
        readonly CCPoint _targetFrom;

        public CCMoveFrom(CCPoint ccFrom, float duration) : base(duration)
        {
            _targetFrom = ccFrom;
        }

        public override CCFiniteTimeAction Reverse()
        {
            throw new NotImplementedException();
        }

        protected internal override CCActionState StartAction(CCNode target)
        {
            return new CCMoveFromState(this, target, _targetFrom);
        }

        protected class CCMoveFromState : CCFiniteTimeActionState
        {
            readonly CCPoint _targetFrom;
            readonly CCPoint _targetTo;

            public CCMoveFromState(CCMoveFrom action, CCNode target, CCPoint targetFrom) : base(action, target)
            {
                _targetFrom = targetFrom;
                _targetTo = Target.Position;
            }

            public override void Update(float time)
            {
                if (Target != null)
                {
                    // calculate current position
                    var newPointX = _targetFrom.X + ((_targetTo.X - _targetFrom.X) * time);
                    var newPointY = _targetFrom.Y + ((_targetTo.Y - _targetFrom.Y) * time);

                    Target.Position = new CCPoint(newPointX, newPointY);
                }
            }
        }
    }
}
