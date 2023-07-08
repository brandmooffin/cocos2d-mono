using Cocos2D;
using System;

namespace AppGame.Shared.Scenes
{
    public class SpritesheetScene : CCScene
    {
        CCLayerColor backgroundLayer;
        public override void OnEnter()
        {
            TouchEnabled = true;
            Console.WriteLine("Entering scene...");
            base.OnEnter();

            Console.WriteLine("Setting up scene...");
            var size = CCDirector.SharedDirector.WinSize;

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
                Position = size.Center + new CCPoint(0, 200),
                Scale = 0.5f
            };

            CCSpriteBatchNode BatchNode = new CCSpriteBatchNode("sprites/tileset");
            CCSprite largeTileSprite = new CCSprite(BatchNode.Texture, new CCRect(0, 0, 96, 96))
            {
                Position = size.Center + new CCPoint(-100, -100)
            };

            CCSprite platformTileSprite = new CCSprite(BatchNode.Texture, new CCRect(96, 0, 96, 32))
            {
                Position = size.Center + new CCPoint(-50, -200)
            };

            CCSprite leftDirtTileSprite = new CCSprite(BatchNode.Texture, new CCRect(96, 32, 96, 32))
            {
                Position = size.Center + new CCPoint(0, -100)
            };
            CCSprite rightDirtTileSprite = new CCSprite(BatchNode.Texture, new CCRect(96, 32, 96, 32))
            {
                Position = size.Center + new CCPoint(0, -100)
            };

            CCSprite columnTileSprite = new CCSprite(BatchNode.Texture, new CCRect(192, 0, 96, 96))
            {
                Position = size.Center + new CCPoint(100, -100)
            };



            var rotateAction = new CCRepeatForever(new CCRotateBy(0.5f, 15));
            logo.RunAction(rotateAction);

            backgroundLayer.AddChild(label);
            backgroundLayer.AddChild(logo);

            backgroundLayer.AddChild(pMenu, 11);

            backgroundLayer.AddChild(largeTileSprite);
            backgroundLayer.AddChild(platformTileSprite);
            backgroundLayer.AddChild(leftDirtTileSprite);
            backgroundLayer.AddChild(rightDirtTileSprite);
            backgroundLayer.AddChild(columnTileSprite);


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