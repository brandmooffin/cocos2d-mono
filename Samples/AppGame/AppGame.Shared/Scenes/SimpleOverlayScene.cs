using AppGame.Shared.Buttons;
using Cocos2D;
using System;

namespace AppGame.Shared.Scenes
{
	public class SimpleOverlayScene : CCScene
    {
        CCSize ScreenSize;
        CCSprite logo;
        public override void OnEnter()
        {
            Console.WriteLine("Entering scene...");
            base.OnEnter();

            Console.WriteLine("Setting up scene...");
            ScreenSize = CCDirector.SharedDirector.WinSize;

            var pCloseItem = new CCMenuItemImage("sprites/close", "sprites/close", CloseCallback);
            var pMenu = new CCMenu(pCloseItem)
            {
                Position = CCPoint.Zero,
            };
            pCloseItem.Position = new CCPoint(ScreenSize.Width - pCloseItem.ContentSize.Width / 2, ScreenSize.Height - pCloseItem.ContentSize.Height / 2 - 50);

            var backgroundLayer = new CCLayerColor()
            {
                Color = new CCColor3B(Microsoft.Xna.Framework.Color.DarkGoldenrod),
                Opacity = 255
            };

            var label = new CCLabelTTF("Hello", "MarkerFelt", 64)
            {
                Color = CCColor3B.White,
                Position = new CCPoint(ScreenSize.Center.X, ScreenSize.Center.Y + 500)
            };

            logo = new CCSprite("sprites/logo-small")
            {
                Position = ScreenSize.Center,
                Scale = 0.5f
            };


            backgroundLayer.AddChild(label);
            backgroundLayer.AddChild(logo);

            backgroundLayer.AddChild(pMenu, 11);

            AddChild(backgroundLayer, -1);

            var descriptionLabel = new CCLabel("Test with multi\nlines and different characters 複数行と異なる文字でテストする マルチでテストする 行と異なる", "Arial", 22)
            {
                Dimensions = new CCSize(ScreenSize.Width - 40, 0),
                Color = CCColor3B.White,
                Position = new CCPoint(ScreenSize.Center.X + 100, ScreenSize.Center.Y - 450)
            };

            AddChild(descriptionLabel);

            ContinueButton continueButton = new ContinueButton();
            AddChild(continueButton);

            ScheduleUpdate();

            Schedule(changeSpriteOpacity, 0.05f);
        }

        public void changeSpriteOpacity(float dt)
        {
            logo.Opacity -= 5;
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

        public override void Draw()
        {
            //base.Draw();

            CCDrawingPrimitives.Begin();

            // draw a simple line
            CCPoint[] vertices3 =
            {
                    new CCPoint(0, ScreenSize.Center.Y - 300), new CCPoint(ScreenSize.Width, ScreenSize.Center.Y - 300),
                    new CCPoint(ScreenSize.Width, ScreenSize.Center.Y - 600), new CCPoint(0, ScreenSize.Center.Y - 600)
            };
            CCDrawingPrimitives.DrawSolidPoly(vertices3, 4, new CCColor4B(0,0,0,125));


            CCDrawingPrimitives.End();
        }
    }
}