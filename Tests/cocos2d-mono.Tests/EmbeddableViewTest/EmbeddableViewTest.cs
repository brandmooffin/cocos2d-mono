using System;
using Cocos2D;

namespace tests
{
    public class EmbeddableViewTest : CCLayer
    {
        public static int sceneIdx = -1;
        public static int MAX_LAYER = 2;

        public static CCLayer createTestLayer(int nIndex)
        {
            switch (nIndex)
            {
                case 0: return new BasicViewTest();
                case 1: return new SplitScreenViewTest();
            }

            return null;
        }

        public static CCLayer nextTestAction()
        {
            sceneIdx++;
            sceneIdx %= MAX_LAYER;
            return createTestLayer(sceneIdx);
        }

        public static CCLayer backTestAction()
        {
            sceneIdx--;
            if (sceneIdx < 0)
                sceneIdx += MAX_LAYER;
            return createTestLayer(sceneIdx);
        }

        public static CCLayer restartTestAction()
        {
            return createTestLayer(sceneIdx);
        }

        public virtual string title()
        {
            return "No title";
        }

        public virtual string subtitle()
        {
            return "";
        }

        public override void OnEnter()
        {
            base.OnEnter();

            CCSize s = CCDirector.SharedDirector.WinSize;

            CCLabelTTF label = new CCLabelTTF(title(), "arial", 24);
            Parent.AddChild(label, 11);
            label.Position = new CCPoint(s.Width / 2, s.Height - 10);

            string strSubtitle = subtitle();
            if (!string.IsNullOrEmpty(strSubtitle))
            {
                CCLabelTTF subtitleLabel = new CCLabelTTF(strSubtitle, "arial", 16);
                Parent.AddChild(subtitleLabel, 11);
                subtitleLabel.Position = new CCPoint(s.Width / 2, s.Height - 36);
            }

            CCMenuItemImage item1 = new CCMenuItemImage(TestResource.s_pPathB1, TestResource.s_pPathB2, backCallback);
            CCMenuItemImage item2 = new CCMenuItemImage(TestResource.s_pPathR1, TestResource.s_pPathR2, restartCallback);
            CCMenuItemImage item3 = new CCMenuItemImage(TestResource.s_pPathF1, TestResource.s_pPathF2, nextCallback);

            CCMenu menu = new CCMenu(item1, item2, item3);

            menu.Position = CCPoint.Zero;
            item1.Position = new CCPoint(s.Width / 2 - 100, 20);
            item2.Position = new CCPoint(s.Width / 2, 20);
            item3.Position = new CCPoint(s.Width / 2 + 100, 20);

            item1.Scale = 0.5f;
            item2.Scale = 0.5f;
            item3.Scale = 0.5f;

            AddChild(menu, 11);
        }

        public void restartCallback(object pSender)
        {
            CCScene s = new EmbeddableViewTestScene();
            s.AddChild(restartTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        public void nextCallback(object pSender)
        {
            CCScene s = new EmbeddableViewTestScene();
            s.AddChild(nextTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        public void backCallback(object pSender)
        {
            CCScene s = new EmbeddableViewTestScene();
            s.AddChild(backTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }
    }
}
