using System;
using AppGame.Shared.Buttons;
using AppGame.Shared.Layers;
using Cocos2D;

namespace AppGame.Shared.Scenes
{
    public class NestingScene : CCScene
    {
        public override void OnEnter()
        {
            TouchEnabled = true;
            TouchPriority = 100;
            Console.WriteLine("Entering scene...");
            base.OnEnter();

            Console.WriteLine("Setting up scene...");
            var size = CCDirector.SharedDirector.WinSize;

            var pCloseItem = new CCMenuItemImage("sprites/close", "sprites/close", CloseCallback);
            var pMenu = new CCMenu(pCloseItem)
            {
                Position = CCPoint.Zero,
            };
            pCloseItem.Position = new CCPoint(size.Width - pCloseItem.ContentSize.Width / 2, size.Height - pCloseItem.ContentSize.Height / 2);

            var backgroundLayer = new BackgroundLayer()
            {
                Color = new CCColor3B(Microsoft.Xna.Framework.Color.Blue),
                Opacity = 255,
                TouchPriority = -10,
                TouchEnabled = true
            };

            backgroundLayer.AddChild(pMenu, 11);

            var messageBoxLayer = new MessageBoxLayer()
            {
                Color = new CCColor3B(Microsoft.Xna.Framework.Color.Green),
                Opacity = 255
            };

            var gameLayer = new GameLayer()
            {
                Color = new CCColor3B(Microsoft.Xna.Framework.Color.DarkOliveGreen),
                Opacity = 255
            };

            AddChild(backgroundLayer, 0);

            AddChild(messageBoxLayer, 1);

            AddChild(gameLayer, 2);

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
            return base.TouchBegan(touch);
        }
    }
}
