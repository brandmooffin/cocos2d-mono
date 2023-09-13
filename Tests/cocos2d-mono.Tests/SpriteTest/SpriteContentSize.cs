using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;
using System.Diagnostics;

namespace tests
{
    public class SpriteContentSize : SpriteTestDemo
    {
        CCSprite sprite1;
        public SpriteContentSize()
        {
            CCSize s = CCDirector.SharedDirector.WinSize;

            sprite1 = new CCSprite("Images/SpookyPeas");
            sprite1.Position = (new CCPoint(s.Width / 2 - 100, s.Height / 2));
            AddChild(sprite1, 0, (int)kTagSprite.kTagSprite1);

            Schedule(ChangeContentSizeSprite, 1);
        }

        public void ChangeContentSizeSprite(float dt)
        {
            sprite1.ContentSize = new CCSize(sprite1.ContentSize.Width * 2, sprite1.ContentSize.Height * 2);
        }

        public override string title()
        {
            return "Sprite ContentSize Change";
        }
    }
}
