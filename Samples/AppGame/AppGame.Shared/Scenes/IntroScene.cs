using Cocos2D;

namespace AppGame.Shared.Scenes
{
    public class IntroScene : CCScene
    {
        public override void OnEnter()
        {
            base.OnEnter();
            var size = CCDirector.SharedDirector.WinSize;

            var highScoreTitleLabel = new CCLabelTTF("Hello World!", "MarkerFelt", 13);
            highScoreTitleLabel.Color = CCColor3B.White;
            highScoreTitleLabel.Position = new CCPoint(size.Width - 100, size.Height - 30);
            AddChild(highScoreTitleLabel);

            ScheduleUpdate();
        }

        public override void Update(float gameTime)
        {
          
        }
    }
}
