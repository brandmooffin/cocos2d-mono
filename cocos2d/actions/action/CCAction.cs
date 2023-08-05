namespace Cocos2D
{

    public class CCAction : ICCCopyable
    {
        public const int kInvalidTag = -1;

        protected int m_nTag = kInvalidTag;
        protected CCNode m_pOriginalTarget;
        protected CCNode m_pTarget;

        public CCAction()
        {
        }

        protected CCAction(CCAction action)
        {
            m_nTag = action.m_nTag;
            m_pOriginalTarget = action.m_pOriginalTarget;
            m_pTarget = action.m_pTarget;
        }

        public CCNode Target
        {
            get { return m_pTarget; }
            set { m_pTarget = value; }
        }

        public CCNode OriginalTarget
        {
            get { return m_pOriginalTarget; }
        }

        public int Tag
        {
            get { return m_nTag; }
            set { m_nTag = value; }
        }

        public virtual CCAction Copy()
        {
            return (CCAction) Copy(null);
        }

        /// <summary>
        /// Copy/Duplicatae protocol for making a self copy of this object instance. If null is 
        /// given as the parameter then selfie of this instance is returned. Otherwise, the state
        /// of this instance is copied to the given target.
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public virtual object Copy(ICCCopyable zone)
        {
            if (zone != null)
            {
                CCAction action = (CCAction)zone;
                action.m_pOriginalTarget = m_pOriginalTarget;
                action.m_pTarget = m_pTarget;
                action.m_nTag = m_nTag;
                return zone;
            }
            else
            {
                return new CCAction(this);
            }
        }

        public virtual bool IsDone
        {
            get { return true; }
        }

        protected internal virtual void StartWithTarget(CCNode target)
        {
            m_pOriginalTarget = m_pTarget = target;
        }

        protected internal virtual CCActionState StartAction(CCNode target)
        {
            return null;

        }

        public virtual void Stop()
        {
            m_pTarget = null;
        }

        public virtual void Step(float dt)
        {
#if DEBUG
            CCLog.Log("[Action step]. override me");
#endif
        }

        public virtual void Update(float time)
        {
#if DEBUG
            CCLog.Log("[Action update]. override me");
#endif
        }
    }

    public abstract class CCActionState
    {
        /// <summary>
        /// Gets or sets the target.
        /// 
        /// Will be set with the 'StartAction' method of the corresponding Action. 
        /// When the 'Stop' method is called, Target will be set to null. 
        /// 
        /// </summary>
        /// <value>The target.</value>

        #region Properties

        public CCNode Target { get; protected set; }
        public CCNode OriginalTarget { get; protected set; }
        public CCAction Action { get; protected set; }

        protected CCScene Scene { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is done.
        /// </summary>
        /// <value><c>true</c> if this instance is done; otherwise, <c>false</c>.</value>
        public virtual bool IsDone
        {
            get { return true; }
        }

        #endregion Properties


        public CCActionState(CCAction action, CCNode target)
        {
            this.Action = action;
            this.Target = target;
            this.OriginalTarget = target;

        }

        /// <summary>
        /// Called after the action has finished.
        /// It will set the 'Target' to null. 
        /// IMPORTANT: You should never call this method manually. Instead, use: "target.StopAction(actionState);"
        /// </summary>
        protected internal virtual void Stop()
        {
            Target = null;
        }

        /// <summary>
        /// Called every frame with it's delta time. 
        /// 
        /// DON'T override unless you know what you are doing.
        /// 
        /// </summary>
        /// <param name="dt">Delta Time</param>
        protected internal virtual void Step(float dt)
        {
#if DEBUG
            CCLog.Log ("[Action State step]. override me");
#endif
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        /// <param name="time">A value between 0 and 1
        ///
        /// For example:
        ///
        /// 0 means that the action just started
        /// 0.5 means that the action is in the middle
        /// 1 means that the action is over</param>
        public virtual void Update(float time)
        {
#if DEBUG
            CCLog.Log ("[Action State update]. override me");
#endif
        }
    }
}