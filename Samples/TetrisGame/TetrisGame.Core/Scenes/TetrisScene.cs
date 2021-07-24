using Cocos2D;
using TetrisGame.Core.Managers;

namespace TetrisGame.Core.Scenes
{
    public class TetrisScene : CCScene
    {
        public Grid Grid;
        public Tetrimino NextTetrimino;
        public CCLabelTTF HighScoreLabel;
        public CCLabelTTF PointsLabel;
        public CCLabelTTF LevelLabel;
        public CCLabelTTF LinesLabel;
        GameState GameState;

        bool Initialized = false;

        public TetrisScene()
        {
            Grid = null;
            NextTetrimino = null;
            PointsLabel = null;
            LevelLabel = null;
            LinesLabel = null;
            GameState = new GameState();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (!Initialized)
            {
                var size = CCDirector.SharedDirector.WinSize;
               
                Grid = new Grid(GameState);
                Grid.Position = new CCPoint(74, 70);
                AddChild(Grid);

                var highScoreTitleLabel = new CCLabelTTF("High Score", "MarkerFelt", 13);
                highScoreTitleLabel.Color = CCColor3B.White;
                highScoreTitleLabel.Position = new CCPoint(size.Width - 100, size.Height - 30);
                AddChild(highScoreTitleLabel);

                HighScoreLabel = new CCLabelTTF($"{AppDataManager.Instance.AppSettings.HighScore}", "MarkerFelt", 13);
                HighScoreLabel.Color = CCColor3B.White;
                HighScoreLabel.Position = new CCPoint(size.Width - 100, size.Height - 50);
                AddChild(HighScoreLabel);

                var pointsTitleLabel = new CCLabelTTF("Score", "MarkerFelt", 13);
                pointsTitleLabel.Color = CCColor3B.White;
                pointsTitleLabel.Position = new CCPoint(size.Width - 100, size.Height - 110);
                AddChild(pointsTitleLabel);

                PointsLabel = new CCLabelTTF("111", "MarkerFelt", 13);
                PointsLabel.Color = CCColor3B.White;
                PointsLabel.Position = new CCPoint(size.Width - 100, size.Height - 130);
                AddChild(PointsLabel);

                var linesTitleLabel = new CCLabelTTF("Lines", "MarkerFelt", 13);
                linesTitleLabel.Color = CCColor3B.White;
                linesTitleLabel.Position = new CCPoint(size.Width - 100, size.Height - 190);
                AddChild(linesTitleLabel);

                LinesLabel = new CCLabelTTF("111", "MarkerFelt", 13);
                LinesLabel.Color = CCColor3B.White;
                LinesLabel.Position = new CCPoint(size.Width - 100, size.Height - 210);
                AddChild(LinesLabel);

                var levelTitleLabel = new CCLabelTTF("Level", "MarkerFelt", 13);
                levelTitleLabel.Color = CCColor3B.White;
                levelTitleLabel.Position = new CCPoint(size.Width - 100, size.Height - 230);
                AddChild(levelTitleLabel);

                LevelLabel = new CCLabelTTF("111", "MarkerFelt", 13);
                LevelLabel.Color = CCColor3B.White;
                LevelLabel.Position = new CCPoint(size.Width - 100, size.Height - 250);
                AddChild(LevelLabel);

                var nextShapLabel = new CCLabelTTF("Next", "MarkerFelt", 13);
                nextShapLabel.Color = CCColor3B.White;
                nextShapLabel.Position = new CCPoint(size.Width - 100, size.Height - 290);
                AddChild(nextShapLabel);

                ScheduleUpdate();
                Initialized = true;
            }
        }

        public override void Update(float gameTime)
        {
            var gameState = GameState;
            var needToPushNextTetrimino = Grid.Tetrimino == null && NextTetrimino != null;

            if (needToPushNextTetrimino)
            {
                RemoveChild(NextTetrimino);
                Grid.PushTetrimino(NextTetrimino);
                NextTetrimino = null;
            }

            // generate next tetrimino
            if (NextTetrimino == null)
            {
                var tet = new Tetrimino(gameState.Speed, Grid);
                AddChild(tet);
                tet.Position = new CCPoint(223 + tet.ContentSize.Width / 2, 70 + tet.ContentSize.Height / 2);
                NextTetrimino = tet;
            }

            Grid.Update(gameTime);
            LinesLabel.Text = $"{gameState.Lines}";
            PointsLabel.Text = $"{gameState.Points}";
            LevelLabel.Text = $"{gameState.Level}";
            var needChangeLevel = gameState.Check();

            if (needChangeLevel)
            {
                RemoveChild(NextTetrimino);
                NextTetrimino = null;
                Grid.SetLevel(gameState.Level);
            }

            if (AppDataManager.Instance.AppSettings.HighScore < GameState.Points)
            {
                HighScoreLabel.Text = $"{GameState.Points}";
            }
        }
    }
}
