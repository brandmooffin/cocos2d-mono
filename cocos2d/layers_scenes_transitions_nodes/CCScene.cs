using System;
using System.IO;
using cocos2d.EmbeddableView;
using cocos2d.Events;
using Microsoft.Xna.Framework.Graphics;

namespace Cocos2D
{
    /// <summary>
    /// brief CCScene is a subclass of CCNode that is used only as an abstract concept.
    /// CCScene and CCNode are almost identical with the difference that CCScene has it's
    /// anchor point (by default) at the center of the screen. Scenes have state management
    /// where they can serialize their state and have it reconstructed upon resurrection.
    ///  It is a good practice to use and CCScene as the parent of all your nodes.
    /// </summary>
    public class CCScene : CCNode
    {
        CCGameView gameView;

        #region Properties

        //public CCRect VisibleBoundsScreenspace
        //{
        //    get { return new CCRect(Viewport.Bounds); }
        //}

        public override CCScene Scene
        {
            get { return this; }
        }

        public override CCGameView GameView
        {
            get { return gameView; }
            protected set
            {
                gameView = value;

                if (gameView != null)
                    InitializeLazySceneGraph(Children);
            }
        }

        //public override CCDirector Director { get { return GameView.Director; } }

        public override CCLayer Layer
        {
            get
            {
                return null;
            }

            internal set
            {
            }
        }

        //public override CCCamera Camera
        //{
        //    get { return null; }
        //    set
        //    {
        //    }
        //}

        internal override CCEventDispatcher EventDispatcher
        {
            get { return GameView != null ? GameView.EventDispatcher : null; }
        }

        public override CCAffineTransform AffineLocalTransform
        {
            get
            {
                return CCAffineTransform.Identity;
            }
        }

        #endregion Properties

        public CCScene(CCGameView gameView)
        {
            GameView = gameView;
        }

        public CCScene(CCScene scene)
            : this(scene.GameView)
        {
        }

        /// <summary>
        /// Sets the anchor point to the middle of the scene but ignores the anchor for positioning.
        /// </summary>
        public CCScene()
        {
            m_bIgnoreAnchorPointForPosition = true;
            AnchorPoint = new CCPoint(0.5f, 0.5f);
            ContentSize = CCDirector.SharedDirector.WinSize;
        }

        void InitializeLazySceneGraph(CCRawList<CCNode> children)
        {
            if (children == null)
                return;

            foreach (var child in children)
            {
                if (child != null)
                {
                    child.AttachEvents();
                    child.AttachActions();
                    child.AttachSchedules();
                    InitializeLazySceneGraph(child.Children);
                }
            }
        }

        /// <summary>
        /// Returns false always unless this is a transition scene.
        /// </summary>
        public virtual bool IsTransition
        {
            get
            {
                return (false);
            }
        }
        /// <summary>
        /// Initialize this scene
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {
            return (true);
        }

        public void AddLayer(CCLayer layer, int zOrder = 0, int tag = 0)
        {
            AddChild(layer, zOrder, tag);
        }
    }
}