using Cocos2D;

namespace TetrisGame.Core.Scenes
{
    public class GameScene : CCScene
    {

        Board Board;

        public GameScene()
        {
            Board = new Board(this);
            
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

            base.Update(gameTime);
        }

        //If the game is over, the shape is removed, and the appropriate message is displayed.
        private void gameOver()
        {
            // TODO: Remove all shapes from scene
            Board.Score.HandleGameOver();
        }
    }
}
