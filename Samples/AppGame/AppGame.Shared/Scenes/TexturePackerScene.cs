using System;
using Cocos2D;

namespace AppGame.Shared.Scenes
{
	public class TexturePackerScene : CCScene
    {
        CCLayerColor backgroundLayer;
        public override void OnEnter()
        {
            TouchEnabled = true;
            Console.WriteLine("Entering scene...");
            base.OnEnter();

            Console.WriteLine("Setting up scene...");
            CCSize size = CCDirector.SharedDirector.WinSize;

            var pCloseItem = new CCMenuItemImage("sprites/close", "sprites/close", CloseCallback);
            var pMenu = new CCMenu(pCloseItem)
            {
                Position = CCPoint.Zero
            };
            pCloseItem.Position = new CCPoint(size.Width - pCloseItem.ContentSize.Width / 2, size.Height - pCloseItem.ContentSize.Height / 2);

            backgroundLayer = new CCLayerColor()
            {
                Color = new CCColor3B(Microsoft.Xna.Framework.Color.Blue),
                Opacity = 255
            };

            CCSpriteFrameCache.SharedSpriteFrameCache.AddSpriteFramesWithFile("sprites/ui.plist");

            float x = size.Width / 4;
            float y = 0 + (size.Height / 2);

            CCLog.Log("S9_TexturePacker ...");

            var s = new CCScale9Sprite();
            s.InitWithSpriteFrameName("button_normal.png");
            CCLog.Log("... created");

            s.Position = new CCPoint(x, y);
            CCLog.Log("... setPosition");

            s.ContentSize = new CCSize(14 * 32, 10 * 32);
            CCLog.Log("... setContentSize");

            
            CCLog.Log("AddChild");

            x = size.Width * 3 / 4;

            var s2 = new CCScale9Sprite();
            s2.InitWithSpriteFrameName("button_actived.png");
            CCLog.Log("... created");

            s2.Position = new CCPoint(x, y);
            CCLog.Log("... setPosition");

            s2.ContentSize = new CCSize(14 * 16, 10 * 16);
            CCLog.Log("... setContentSize");

            CCLog.Log("AddChild");

            CCLog.Log("... S9_TexturePacker done.");

            backgroundLayer.AddChild(s);
            backgroundLayer.AddChild(s2);

            backgroundLayer.AddChild(pMenu, 11);

            AddChild(backgroundLayer);

            ScheduleUpdate();
        }

        public void CloseCallback(object pSender)
        {
            SampleGame.IsExiting = true;
        }

        public override void OnExit()
        {
            base.OnExit();
            Console.WriteLine("Exiting scene...");

            RemoveAllChildren();
            RemoveFromParent();

            UnscheduleUpdate();
        }

        public override void Update(float gameTime)
        {

        }
    }
}

