using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;
using System.Diagnostics;

namespace tests
{
    public class SpriteReset : SpriteTestDemo
    {
        CCSprite sprite1;

        bool shouldRemove = true;

        public SpriteReset()
        {
            CCSize s = CCDirector.SharedDirector.WinSize;

            sprite1 = new CCSprite("Images/SpookyPeas");
            sprite1.Position = (new CCPoint(s.Width / 2 - 100, s.Height / 2));
            sprite1.Scale = 10;
            AddChild(sprite1, 0, (int)kTagSprite.kTagSprite1);

            Schedule(ResetSprite, 1);
        }

        public void ResetSprite(float dt)
        {
            if (shouldRemove)
            {
                RemoveChild(sprite1, true);
                
                shouldRemove = false;
            }
            else
            {
                AddChild(sprite1, 0, (int)kTagSprite.kTagSprite1);
                shouldRemove = true;
            }
        }

        public override string title()
        {
            return "Sprite ContentSize Change";
        }
    }
}
