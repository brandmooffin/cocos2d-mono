using Cocos2D;
using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisGame.Core.Scenes
{
    public class TetrisScene : CCScene
    {
        public Grid Grid;
        public Tetrimino NextTetrimino;
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
                var background = new CCSprite("background.png");
                background.Position = new CCPoint(size.Width / 2, size.Height / 2);
                background.Scale = 0.5f;
                AddChild(background, 0);
                Grid = new Grid(GameState);
                Grid.Position = new CCPoint(74, 70);
                AddChild(Grid);

                PointsLabel = new CCLabelTTF("111", "MarkerFelt", 13);
                PointsLabel.Color = CCColor3B.White;
                PointsLabel.Position = new CCPoint(size.Width - 100, size.Height - 130);
                AddChild(PointsLabel);

                LevelLabel = new CCLabelTTF("111", "MarkerFelt", 13);
                LevelLabel.Color = CCColor3B.White;
                LevelLabel.Position = new CCPoint(size.Width - 100, size.Height - 250);
                AddChild(LevelLabel);

                LinesLabel = new CCLabelTTF("111", "MarkerFelt", 13);
                LinesLabel.Color = CCColor3B.White;
                LinesLabel.Position = new CCPoint(size.Width - 100, size.Height - 210);
                AddChild(LinesLabel);

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
            LinesLabel.Text = gameState.Lines.ToString();
            PointsLabel.Text = gameState.Points.ToString();
            LevelLabel.Text = gameState.Level.ToString();
            var needChangeLevel = gameState.Check();

            if (needChangeLevel)
            {
                RemoveChild(NextTetrimino);
                NextTetrimino = null;
                Grid.SetLevel(gameState.Level);
            }

        }
    }
}
