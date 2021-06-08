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
            Board = new Board(this);
            Score = new Score(this, Board);

            Board.GameOver += gameOver;

            AddChild(Board);

            ScheduleUpdate();
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
