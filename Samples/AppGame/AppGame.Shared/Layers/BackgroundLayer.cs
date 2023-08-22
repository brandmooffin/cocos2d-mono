using System;
using System.Reflection.Emit;
using Cocos2D;

namespace AppGame.Shared.Layers
{
    public class BackgroundLayer : CCLayer
    {
        CCLabel label;
        CCTexture2D SampleTexture;
        public override void OnEnter()
        {
            TouchEnabled = true;
            TouchMode = CCTouchMode.OneByOne;
            TouchPriority = -1;
            base.OnEnter();

            var size = CCDirector.SharedDirector.WinSize;
            SampleTexture = CCTextureCache.SharedTextureCache.AddImage("sprites/SpookyPeas");

            label = new CCLabel("Tap Here\nBackground Layer", "Arial", 24)
            {
                Color = CCColor3B.White,
                Position = new CCPoint(size.Center.X, size.Center.Y),
                Scale = 2
            };

        
            AddChild(label);
        }

        public override bool TouchBegan(CCTouch touch)
        {
            Console.WriteLine("Touches began...");
            if (label.WorldBoundingBox.ContainsPoint(touch.Location) && Visible)
            {
                var logo = new CCSprite(SampleTexture)
                {
                    Position = touch.Location,
                    Scale = 3
                };
                AddChild(logo);
                return true;
            }
            return false;
        }
    }
}

