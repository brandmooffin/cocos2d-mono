using Cocos2D;
using TetrisGame.Core;

namespace TetrisGame.Core.Scenes
{
    public class GameScene : CCScene
    {

        Board Board;
        Score Score;

        public GameScene()
        {
            Board board = new Board(this);
            Score score = new Score(this, board);

            board.GameOver += gameOver;
            // create and initialize a Label
            var label = new CCLabelTTF("Hello World!", "MarkerFelt", 22)
            {
                // position the label on the center of the screen
                Position = CCDirector.SharedDirector.WinSize.Center
            };

            // add the label as a child to this Layer
            AddChild(label);
        }

        public static CCScene Scene
        {
            get
            {
                // return the scene
                return new GameScene();
            }

        }

        public override void Update(float gameTime)
        {
            Board.Update();
            Score.Update();

            base.Update(gameTime);
        }

        //If the game is over, the shape is removed, and the appropriate message is displayed.
        private void gameOver()
        {
            // TODO: Remove all shapes from scene
            Score.HandleGameOver();
        }
    }
}
