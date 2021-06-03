using Cocos2D;
using TetrisGame.Core;

namespace TetrisGame.Core.Scenes
{
    public class GameScene : CCScene
    {
        BoardSprite boardSprite;
        ShapeSprite shapeSprite;
        ScoreSprite scoreSprite;

        public GameScene()
        {
            Board board = new Board(this);
            Score score = new Score(board);

            boardSprite = new BoardSprite(this, board);
            shapeSprite = new ShapeSprite(this, board, score);
            scoreSprite = new ScoreSprite(this, score);

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
            boardSprite.Update();
            base.Update(gameTime);
        }

        //If the game is over, the shape is removed, and the appropriate message is displayed.
        private void gameOver()
        {
            Components.Remove(shapeSprite);
            scoreSprite.HandleGameOver();
        }
    }
}
