using Cocos2D;

namespace cocos2d.actions.action_intervals
{
    public class TimerAction : CCFiniteTimeAction
    {
        readonly float _duration;

        public TimerAction(float duration) : base(duration)
        {
            _duration = duration;
        }

        public override CCFiniteTimeAction Reverse()
        {
            return new TimerAction(_duration);
        }

        protected internal override CCActionState StartAction(CCNode target)
        {
            return new TimerActionState(this, target, _duration);
        }
    }

    public class TimerActionState : CCFiniteTimeActionState
    {
        readonly float _duration;
        readonly ITimerActionListener _castedActionTarget;

        public TimerActionState(TimerAction action, CCNode target, float duration) : base(action, target)
        {
            _castedActionTarget = target as ITimerActionListener;
            _duration = duration;
        }

        public override void Update(float time)
        {
            _castedActionTarget?.TimerActionUpdate(_duration * time, _duration);
        }
    }

    public interface ITimerActionListener
    {
        void TimerActionUpdate(float elapsedTime, float totalTime);
    }
}
