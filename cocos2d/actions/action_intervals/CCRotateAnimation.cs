using Cocos2D;
using Java.Lang.Annotation;
using System;
using System.Collections.Generic;
using System.Text;

namespace cocos2d.actions.action_intervals
{
    public class CCRotateAnimation : CCFiniteTimeAction
    {
        readonly float _targetRotation;

        public CCRotateAnimation(float duration, float rotation) : base(duration)
        {
            _targetRotation = rotation;
        }

        public override CCFiniteTimeAction Reverse()
        {
            throw new NotImplementedException();
        }

        protected internal override CCActionState StartAction(CCNode target)
        {
            return new RotationState(this, target, _targetRotation);
        }

        protected class RotationState : CCFiniteTimeActionState
        {
            readonly float _targetAngle;
            readonly float _startAngle;

            public RotationState(CCRotateAnimation action, CCNode target, float targetAngle) : base(action, target)
            {
                _targetAngle = targetAngle;
                _startAngle = (Target as ICCRotationAnimationGetter).CurrentRotation;
            }

            public override void Update(float time)
            {
                if (Target != null)
                {
                    // calculate current percentage
                    Target.Rotation = _startAngle + (_targetAngle - _startAngle) * time;
                }
            }
        }
    }
}
