using cocos2d.base_nodes;
using Cocos2D;
using tests;

namespace cocos2d_mono.Tests.TapTest
{
    public class TapTouchTestLayer : CCTapNode<CCLayer>
    {
        public override bool Init()
        {
            if (base.Init())
            {
                Data = new CCLayer();

                CCDirector director = CCDirector.SharedDirector;
                ContentSize = director.WinSize;

                OnTapped += Layer_OnTapped;
                return true;
            }
            return false;
        }

        private void Layer_OnTapped(CCLayer layer, CCNode node, CCPoint tapLocation)
        {
            RemoveAllChildren();

            TouchPoint touchPoint = TouchPoint.TouchPointWithParent(this);
            CCPoint location = tapLocation;

            touchPoint.SetTouchPos(location);
            touchPoint.SetTouchColor(CCColor3B.Yellow);

            AddChild(touchPoint);
        }

        public override void RegisterWithTouchDispatcher()
        {
            base.RegisterWithTouchDispatcher();
            CCDirector.SharedDirector.TouchDispatcher.AddStandardDelegate(this, 0);
        }

    }

    public class TapTouchTestScene : TestScene
    {
        
        public override void runThisTest()
        {
            TapTouchTestLayer layer = new TapTouchTestLayer();

            AddChild(layer, 0);

            CCDirector.SharedDirector.ReplaceScene(this);
        }

        protected override void NextTestCase()
        {
        }

        protected override void PreviousTestCase()
        {
        }

        protected override void RestTestCase()
        {
        }
    }

    public class TouchPoint : CCNode
    {

        public override void Draw()
        {
            CCDrawingPrimitives.Begin();
            CCDrawingPrimitives.DrawLine(new CCPoint(0, _touchPoint.Y), new CCPoint(ContentSize.Width, _touchPoint.Y),
                                         new CCColor4B(_touchColor.R, _touchColor.G, _touchColor.B, 255));
            CCDrawingPrimitives.DrawLine(new CCPoint(_touchPoint.X, 0), new CCPoint(_touchPoint.X, ContentSize.Height),
                                         new CCColor4B(_touchColor.R, _touchColor.G, _touchColor.B, 255));
            CCDrawingPrimitives.DrawPoint(_touchPoint, 30,
                                          new CCColor4B(_touchColor.R, _touchColor.G, _touchColor.B, 255));
            CCDrawingPrimitives.End();
        }

        public void SetTouchPos(CCPoint pt)
        {
            _touchPoint = pt;
        }

        public void SetTouchColor(CCColor3B color)
        {
            _touchColor = color;
        }

        public static TouchPoint TouchPointWithParent(CCNode pParent)
        {
            TouchPoint pRet = new TouchPoint();
            pRet.ContentSize = pParent.ContentSize;
            pRet.AnchorPoint = new CCPoint(0.0f, 0.0f);
            return pRet;
        }

        private CCPoint _touchPoint;
        private CCColor3B _touchColor;
    }
}
