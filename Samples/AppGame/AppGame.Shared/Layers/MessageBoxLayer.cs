using System;
using System.Reflection.Emit;
using AppGame.Shared.Buttons;
using Cocos2D;

namespace AppGame.Shared.Layers
{
    public class MessageBoxLayer : CCLayer
    {
        CCTexture2D SampleTexture;
        CCLabel label;
        public override void OnEnter()
        {
            TouchEnabled = true;
            TouchMode = CCTouchMode.OneByOne;
            TouchPriority = -2;
            base.OnEnter();

            var size = CCDirector.SharedDirector.WinSize;
            SampleTexture = CCTextureCache.SharedTextureCache.AddImage("sprites/SpookyPeas");

            label = new CCLabel("Tap Here\nMessageBox Layer", "Arial", 24)
            {
                Color = CCColor3B.White,
                Position = new CCPoint(size.Center.X, size.Center.Y - 400),
                Scale = 2
            };


            AddChild(label);

            ContinueButton continueButton = new ContinueButton();
            AddChild(continueButton);

        }

        public override bool TouchBegan(CCTouch touch)
        {
            if (label.WorldBoundingBox.ContainsPoint(touch.Location) && Visible)
            {
                var logo = new CCSprite(SampleTexture)
                {
                    Position = touch.Location,
                    Scale = 2
                };
                AddChild(logo);
                return true;
            }
            return false;
        }
    }
}

