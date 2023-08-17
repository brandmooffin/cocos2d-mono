using Cocos2D;
using tests;

namespace cocos2d_mono.Tests.SpriteTest
{
    internal class SpriteOpacityTest : SpriteTestDemo
    {
        CCSprite opacitySprite;
        public SpriteOpacityTest()
        {
            CCSize s = CCDirector.SharedDirector.WinSize;

            opacitySprite = new CCSprite("Images/background2")
            {
                Position = s.Center
            };
            AddChild(opacitySprite);

            Schedule(flipSprites, 0.05f);
        }

        public void flipSprites(float dt)
        {
            opacitySprite.Opacity -= 2;
        }

        public override string title()
        {
            return "Sprite Opacity Test";
        }
    }
}