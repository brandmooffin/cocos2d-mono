using Cocos2D;

namespace tests
{
    public class SpriteDemo : CCLayer
    {
        private string s_pPathB1 = "Images/b1";
        private string s_pPathB2 = "Images/b2";
        private string s_pPathF1 = "Images/f1";
        private string s_pPathF2 = "Images/f2";
        private string s_pPathR1 = "Images/r1";
        private string s_pPathR2 = "Images/r2";

        public virtual string title()
        {
            return "ProgressActionsTest";
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
            if (strSubtitle != null)
            {
                label.Text += $" - {strSubtitle}";
            }

            CCMenuItemImage item1 = new CCMenuItemImage(s_pPathB1, s_pPathB2, backCallback);
            CCMenuItemImage item2 = new CCMenuItemImage(s_pPathR1, s_pPathR2, restartCallback);
            CCMenuItemImage item3 = new CCMenuItemImage(s_pPathF1, s_pPathF2, nextCallback);

            CCMenu menu = new CCMenu(item1, item2, item3);

            menu.Position = CCPoint.Zero;
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
            CCScene s = new ProgressActionsTestScene();
            s.AddChild(ProgressActionsTestScene.restartAction());

            CCDirector.SharedDirector.ReplaceScene(s);
            //s->release();
        }

        public void nextCallback(object pSender)
        {
            CCScene s = new ProgressActionsTestScene();
            s.AddChild(ProgressActionsTestScene.nextAction());
            CCDirector.SharedDirector.ReplaceScene(s);
            //s->release();
        }

        public void backCallback(object pSender)
        {
            CCScene s = new ProgressActionsTestScene();
            s.AddChild(ProgressActionsTestScene.backAction());
            CCDirector.SharedDirector.ReplaceScene(s);
            //s->release();
        }
    }
}