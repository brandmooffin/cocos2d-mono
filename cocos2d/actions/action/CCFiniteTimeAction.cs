using System;

namespace Cocos2D
{
    public class CCFiniteTimeAction : CCAction
    {
        protected float m_fDuration;

        protected CCFiniteTimeAction()
        {
        }

        protected CCFiniteTimeAction(CCFiniteTimeAction finiteTimeAction) : base(finiteTimeAction)
        {
            m_fDuration = finiteTimeAction.m_fDuration;
        }

        /// <summary>
        /// Get/set the duration of this action
        /// </summary>
        public float Duration
        {
            get { return m_fDuration; }
            set { m_fDuration = value; }
        }

        /// <summary>
        /// Does nothing by default. 
        /// </summary>
        /// <returns></returns>
        public virtual CCFiniteTimeAction Reverse()
        {
            return null;
        }
    }

    public class CCFiniteTimeActionState : CCActionState
    {
        bool firstTick;

        #region Properties

        public virtual float Duration { get; set; }
        public float Elapsed { get; private set; }

        public override bool IsDone
        {
            get { return Elapsed >= Duration; }
        }

        #endregion Properties


        public CCFiniteTimeActionState(CCFiniteTimeAction action, CCNode target)
            : base(action, target)
        {
            Duration = action.Duration;
            Elapsed = 0.0f;
            firstTick = true;
        }

        protected internal override void Step(float dt)
        {
            if (firstTick)
            {
                firstTick = false;
                Elapsed = 0f;
            }
            else
            {
                Elapsed += dt;
            }

            Update(Math.Max(0f,
                Math.Min(1, Elapsed / Math.Max(Duration, float.Epsilon)
                )
            )
            );
        }

    }
}