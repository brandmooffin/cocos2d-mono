using Cocos2D;

namespace tests
{
    public class ZwoptexTest : CCLayer
    {
        public static int MAX_LAYER = 1;

        public static int sceneIdx = -1;

        public override void OnEnter()
        {
            base.OnEnter();

            CCSize s = CCDirector.SharedDirector.WinSize;

            CCLabelTTF label = new CCLabelTTF(title(), "arial", 24);
            Parent.AddChild(label, 11);
            label.Position = (new CCPoint(s.Width / 2, s.Height - 10));

            string strSubTitle = subtitle();
            if (strSubTitle.Length > 0)
            {
                label.Text += $" - {strSubTitle}";
            }

            CCMenuItemImage item1 = new CCMenuItemImage(TestResource.s_pPathB1, TestResource.s_pPathB2, backCallback);
            CCMenuItemImage item2 = new CCMenuItemImage(TestResource.s_pPathR1, TestResource.s_pPathR2, restartCallback);
            CCMenuItemImage item3 = new CCMenuItemImage(TestResource.s_pPathF1, TestResource.s_pPathF2, nextCallback);

            CCMenu menu = new CCMenu(item1, item2, item3);

            menu.Position = (new CCPoint(0, 0));
            item1.Position = (new CCPoint(s.Width / 2 - 100, 20));
            item2.Position = (new CCPoint(s.Width / 2, 20));
            item3.Position = (new CCPoint(s.Width / 2 + 100, 20));
            item1.Scale = 0.5f;
            item2.Scale = 0.5f;
            item3.Scale = 0.5f;
            AddChild(menu, 11);
        }

        public void restartCallback(object pSender)
        {
            CCScene s = ZwoptexTestScene.node();
            s.AddChild(restartZwoptexTest());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        public void nextCallback(object pSender)
        {
            CCScene s = ZwoptexTestScene.node();
            s.AddChild(nextZwoptexTest());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        public void backCallback(object pSender)
        {
            CCScene s = ZwoptexTestScene.node();
            s.AddChild(backZwoptexTest());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        public virtual string title()
        {
            return "No title";
        }

        public virtual string subtitle()
        {
            return "";
        }


        public static CCLayer createZwoptexLayer(int nIndex)
        {
            switch (nIndex)
            {
                case 0:
                    return new ZwoptexGenericTest();
            }

            return null;
        }

        public static CCLayer nextZwoptexTest()
        {
            sceneIdx++;
            sceneIdx = sceneIdx % MAX_LAYER;

            CCLayer pLayer = createZwoptexLayer(sceneIdx);

            return pLayer;
        }

        public static CCLayer backZwoptexTest()
        {
            sceneIdx--;
            int total = MAX_LAYER;
            if (sceneIdx < 0)
                sceneIdx += total;

            CCLayer pLayer = createZwoptexLayer(sceneIdx);

            return pLayer;
        }

        public static CCLayer restartZwoptexTest()
        {
            CCLayer pLayer = createZwoptexLayer(sceneIdx);

            return pLayer;
        }
    }
}