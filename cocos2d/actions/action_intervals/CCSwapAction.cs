using cocos2d.misc_nodes;
using Cocos2D;

namespace cocos2d.actions.action_intervals
{
    public class CCSwapAction : CCParallel
    {
        public new CCFiniteTimeAction[] Actions { get; protected set; }

        #region Constructors
        public CCSwapAction(CCSwappableNode fistCard, CCSwappableNode secondCard, float duration = 0.5f) : base(new CCFiniteTimeAction[0])
        {
            var relativePoint = (fistCard.CurrentTargetPos - secondCard.CurrentTargetPos);
            var distance = CCPoint.Distance(fistCard.CurrentTargetPos, secondCard.CurrentTargetPos);
            var angleBetween = relativePoint.Angle;

            var temp = new CCPoint2D(relativePoint.X, relativePoint.Y);
            temp.Length = (fistCard.ContentSize.Height > 0) ? fistCard.ContentSize.Height * 0.8f : distance * 0.25f;
            temp.Angle = angleBetween + CCMathHelper.Pi_2;

            var bezierConfig = new CCBezierConfig();
            bezierConfig.ControlPoint1 = fistCard.CurrentTargetPos + new CCPoint(temp.X, temp.Y);
            bezierConfig.ControlPoint2 = secondCard.CurrentTargetPos + new CCPoint(temp.X, temp.Y);
            bezierConfig.EndPosition = secondCard.CurrentTargetPos;


            var moveFirst = new CCBezierTo(duration, bezierConfig);

            temp.Angle = angleBetween - CCMathHelper.Pi_2;
            bezierConfig = new CCBezierConfig();
            bezierConfig.ControlPoint1 = secondCard.CurrentTargetPos + new CCPoint(temp.X, temp.Y);
            bezierConfig.ControlPoint2 = fistCard.CurrentTargetPos + new CCPoint(temp.X, temp.Y);
            bezierConfig.EndPosition = fistCard.CurrentTargetPos;

            fistCard.CurrentTargetPos = secondCard.CurrentTargetPos;
            secondCard.CurrentTargetPos = bezierConfig.EndPosition;

            var moveSecond = new CCBezierTo(duration, bezierConfig);


            var actions = new CCFiniteTimeAction[] { new CCTargetedAction(fistCard, moveFirst), new CCTargetedAction(secondCard, moveSecond) };


            // Can't call base(duration) because max action duration needs to be determined here
            float maxDuration = 0.0f;
            foreach (CCFiniteTimeAction action in actions)
            {
                if (action.Duration > maxDuration)
                {
                    maxDuration = action.Duration;
                }
            }
            Duration = maxDuration;

            Actions = actions;

            for (int i = 0; i < Actions.Length; i++)
            {
                var actionDuration = Actions[i].Duration;
                if (actionDuration < Duration)
                {
                    Actions[i] = new CCSequence(Actions[i], new CCDelayTime(Duration - actionDuration));
                }
            }
        }
        #endregion Constructors

        protected internal override CCActionState StartAction(CCNode target)
        {
            return new CCSwapState(this, target);
        }


    }

    public class CCSwapState : CCParallelState
    {
        public CCSwapState(CCSwapAction action, CCNode target) : base(action, target)
        {
            Actions = action.Actions;
            ActionStates = new CCFiniteTimeActionState[Actions.Length];

            for (int i = 0; i < Actions.Length; i++)
            {
                ActionStates[i] = new CCTargetedActionState((CCTargetedAction)Actions[i], target);
            }
        }
    }
}
