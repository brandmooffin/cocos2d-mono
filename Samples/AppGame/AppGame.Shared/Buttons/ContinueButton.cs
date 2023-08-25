using Cocos2D;
using cocos2d.base_nodes;

namespace AppGame.Shared.Buttons
{
    public class ContinueButton : CCTapNode<CCLayer>
    {
        public bool TappedOn;
        CCLabelTTF ButtonLabel;

        public override bool Init()
        {
            if (base.Init())
            {
                ContentSize = new CCSize(200, 200);

                OnTapped += Layer_OnTapped;

                ButtonLabel = new CCLabelTTF("Continue", "MarkerFelt", 22)
                {
                    Color = CCColor3B.White,
                    Position = new CCPoint(ContentSize.Center.X, ContentSize.Center.Y)
                };

                AddChild(ButtonLabel);

                return true;
            }
            return false;
        }

        private void Layer_OnTapped(CCLayer layer, CCNode node, CCPoint tapLocation)
        {
            TappedOn = !TappedOn;
            if (TappedOn)
            {
                ButtonLabel.RunAction(new CCScaleTo(3, 2));
            } else
            {
                ButtonLabel.RunAction(new CCScaleTo(3, 1));
            }
        }

        public override void RegisterWithTouchDispatcher()
        {
            base.RegisterWithTouchDispatcher();
            CCDirector.SharedDirector.TouchDispatcher.AddStandardDelegate(this, 0);
        }

    }
}

