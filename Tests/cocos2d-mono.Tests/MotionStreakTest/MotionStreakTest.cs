using Cocos2D;

namespace tests
{
    public class MotionStreakTest : CCLayer
    {
        private static int sceneIdx = 0;
        private static int MAX_LAYER = 4;
        
        private string s_pPathB1 = "Images/b1";
        private string s_pPathB2 = "Images/b2";
        private string s_pPathF1 = "Images/f1";
        private string s_pPathF2 = "Images/f2";
        private string s_pPathR1 = "Images/r1";
        private string s_pPathR2 = "Images/r2";

        private const int kTagLabel = 2;

        protected CCMotionStreak streak;

        public static CCLayer createMotionLayer(int nIndex)
        {
            switch (nIndex)
            {
                case 0:
                    return new MotionStreakTest1();
                case 1:
                    return new MotionStreakTest2();
                case 2:
                    return new Issue1358();
                case 3:
                    return new LightningStreakTest();
            }

            return null;
        }


        public static CCLayer nextMotionAction()
        {
            sceneIdx++;
            sceneIdx = sceneIdx % MAX_LAYER;

            CCLayer pLayer = createMotionLayer(sceneIdx);
            return pLayer;
        }

        public static CCLayer backMotionAction()
        {
            sceneIdx--;
            int total = MAX_LAYER;
            if (sceneIdx < 0)
                sceneIdx += total;

            CCLayer pLayer = createMotionLayer(sceneIdx);
            return pLayer;
        }

        public static CCLayer restartMotionAction()
        {
            CCLayer pLayer = createMotionLayer(sceneIdx);
            return pLayer;
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

            var label = new CCLabelTTF(title(), "arial", 24);
            Parent.AddChild(label, 11, kTagLabel);
            label.Position = new CCPoint(s.Width / 2, s.Height - 10);

            string subTitle = this.subtitle();
            if (subTitle.Length > 0)
            {
                label.Text += $" - {subTitle}";
            }

            var item1 = new CCMenuItemImage(s_pPathB1, s_pPathB2, backCallback);
            var item2 = new CCMenuItemImage(s_pPathR1, s_pPathR2, restartCallback);
            var item3 = new CCMenuItemImage(s_pPathF1, s_pPathF2, nextCallback);

            var menu = new CCMenu(item1, item2, item3);

            menu.Position = CCPoint.Zero;
            item1.Position = new CCPoint(s.Width / 2 - item2.ContentSize.Width * 2, item2.ContentSize.Height / 2);
            item2.Position = new CCPoint(s.Width / 2, item2.ContentSize.Height / 2);
            item3.Position = new CCPoint(s.Width / 2 + item2.ContentSize.Width * 2, item2.ContentSize.Height / 2);
            item1.Scale = 0.5f;
            item2.Scale = 0.5f;
            item3.Scale = 0.5f;
            AddChild(menu, 11);

            var itemMode = new CCMenuItemToggle(modeCallback,
                                                   new CCMenuItemFont("Use High Quality Mode"),
                                                   new CCMenuItemFont("Use Fast Mode")
                );

            var menuMode = new CCMenu(itemMode);
            AddChild(menuMode);

            menuMode.Position = new CCPoint(s.Width / 2, s.Height / 4);
        }

        private void modeCallback(object pSender)
        {
            if (streak != null)
            {
                bool fastMode = streak.FastMode;
                streak.FastMode = !fastMode;
            }
        }

        public void restartCallback(object pSender)
        {
            CCScene s = new MotionStreakTestScene(); 
            s.AddChild(restartMotionAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        public void nextCallback(object pSender)
        {
            CCScene s = new MotionStreakTestScene(); 
            s.AddChild(nextMotionAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        public void backCallback(object pSender)
        {
            CCScene s = new MotionStreakTestScene(); 
            s.AddChild(backMotionAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }
    }
}