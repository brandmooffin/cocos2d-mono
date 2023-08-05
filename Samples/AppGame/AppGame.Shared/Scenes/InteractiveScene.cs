using Cocos2D;
using System;

namespace AppGame.Shared.Scenes
{
    public class InteractiveScene : CCScene
    {
        CCLayerColor backgroundLayer;
        CCTexture2D SampleTexture;
        public override void OnEnter()
        {
            TouchEnabled = true;
            Console.WriteLine("Entering scene...");
            base.OnEnter();

            Console.WriteLine("Setting up scene...");
            var size = CCDirector.SharedDirector.WinSize;

            SampleTexture = CCTextureCache.SharedTextureCache.AddImage("sprites/SpookyPeas");

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

            var label = new CCLabelBMFont("Hello", "fonts/bitmapFontTest3.fnt")
            {
                Color = CCColor3B.White,
                Position = new CCPoint(size.Center.X, size.Center.Y + 500),
                Scale = 2
            };

            var logo = new CCSprite("sprites/logo-small")
            {
                Position = size.Center + new CCPoint(0, -100),
                Scale = 0.5f
            };

            var rotateAction = new CCRepeatForever(new CCRotateBy(0.5f, 15));
            logo.RunAction(rotateAction);

            backgroundLayer.AddChild(label);
            backgroundLayer.AddChild(logo);

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

        public override bool TouchBegan(CCTouch touch)
        {
            Console.WriteLine("Touches began...");
            var logo = new CCSprite(SampleTexture)
            {
                Position = touch.Location
            };
            backgroundLayer.AddChild(logo);
            return base.TouchBegan(touch);
        }
    }
}