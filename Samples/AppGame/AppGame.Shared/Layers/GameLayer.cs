using System;
using Cocos2D;

namespace AppGame.Shared.Layers
{
    public class GameLayer : CCLayer
    {
        CCTexture2D SampleTexture;
        CCLabel label;
        public override void OnEnter()
        {
            TouchEnabled = true;
            TouchMode = CCTouchMode.OneByOne;
            TouchPriority = -3;

            base.OnEnter();

            var size = CCDirector.SharedDirector.WinSize;

            SampleTexture = CCTextureCache.SharedTextureCache.AddImage("sprites/SpookyPeas");

            label = new CCLabel("Tap Here\nGame Layer", "Arial", 12)
            {
                Color = CCColor3B.White,
                Position = new CCPoint(size.Center.X, size.Center.Y + 400),
                Scale = 2
            };


            AddChild(label);
        }

        public override bool TouchBegan(CCTouch touch)
        {
            Console.WriteLine("GameLayer Touches began...");
            if (label.WorldBoundingBox.ContainsPoint(touch.Location) && Visible)
            {
                var logo = new CCSprite(SampleTexture)
                {
                    Position = touch.Location
                };
                AddChild(logo);

                RunAction(new CCSequence(new CCDelayTime(0.3f), new CCRepeat(new CCSequence(new CCMoveBy(0.025f, new CCPoint(10, 0)), new CCMoveBy(0.05f, new CCPoint(-20, 0)), new CCMoveBy(0.025f, new CCPoint(10, 0))), 3)));
                return true;
            }
            return false;
        }
    }
}

