using System;
using System.Diagnostics;
using Cocos2D;

namespace cocos2d.Renderer.RenderCommands
{
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public class CCCustomCommand : CCRenderCommand
    {
        bool disposed;


        #region Properties

        public Action Action { get; internal set; }

        #endregion Properties


        #region Constructors

        public CCCustomCommand(float globalZOrder, CCAffineTransform worldTransform, Action action)
            : base(globalZOrder, worldTransform)
        {
            Action = action;
        }

        public CCCustomCommand(float globalZOrder, Action action)
            : this(globalZOrder, CCAffineTransform.Identity, action)
        {
        }

        public CCCustomCommand(Action action)
            : this(0.0f, CCAffineTransform.Identity, action)
        {
        }

        protected CCCustomCommand(CCCustomCommand copy)
            : base(copy)
        {
            Action = copy.Action;
        }

        public override CCRenderCommand Copy()
        {
            return new CCCustomCommand(this);
        }

        #endregion Constructors


        #region Cleaning up

        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (Action != null)
                {
                    Action = null;
                }
            }

            base.Dispose(disposing);

            disposed = true;
        }

        #endregion Cleaning up


        internal override void RequestRenderCommand(CCRenderer renderer)
        {
            if (Action != null)
                renderer.ProcessCustomRenderCommand(this);
        }

        internal void RenderCustomCommand()
        {
            bool originalDepthTestState = CCDrawManager.DepthTest;
            CCDrawManager.DepthTest = UsingDepthTest;

            CCDrawManager.PushMatrix();
            CCDrawManager.SetIdentityMatrix();

            if (WorldTransform != CCAffineTransform.Identity)
            {
                var worldTrans = WorldTransform.XnaMatrix;
                CCDrawManager.MultMatrix(ref worldTrans);
            }
            Action();

            CCDrawManager.PopMatrix();

            CCDrawManager.DepthTest = originalDepthTestState;
        }

        internal new string DebugDisplayString
        {
            get
            {
                return ToString();
            }
        }

        public override string ToString()
        {
            return string.Concat("[CCCustomCommand: Group ", Group.ToString(), " Depth ", GlobalDepth.ToString(), "]");
        }
    }
}

