using Cocos2D;
using System.Collections.Generic;
using System.Linq;
using TetrisGame.Core.Managers;

namespace TetrisGame.Core
{
    /// <summary>
    /// Represents the main Tetris grid.
    /// </summary>
    public class Grid : CCNode {

        public CCRect Size;
        public Tetrimino Tetrimino;
        int Level;
        public List<List<bool>> BricksMap;
        GameState GameState;

        public Grid(GameState gameState) {
            Init();
            Size = new CCRect(0, 0, 10, 20);
            ContentSize = new CCSize(Size.MaxX * Block.WIDTH, Size.MaxY * Block.HEIGHT);
            Tetrimino = null;
            BricksMap = new List<List<bool>>();
            SetLevel(0);
            GameState = gameState;
        }

        public override void Update(float dt)
        {
            if (Tetrimino == null) return;

            Tetrimino.Update(dt);

            if (Tetrimino.IsFrozen)
            {
                Tetrimino = null;
            }
        }

        /// <summary>
        /// Add Tetrimino to Grid
        /// </summary>
        public void PushTetrimino(Tetrimino tetrimino)
        {
            tetrimino.SetGrid(this);
            AddChild(tetrimino);
            Tetrimino = tetrimino;
        }


        public void SetLevel(int level)
        {
            Level = level;
            BricksMap = CreateBricksMap((int)Size.MaxX, (int)Size.MaxY, level * 2);
            Tetrimino = null;

            UpdateBricks();
        }

        /// <summary>
        /// Add bricks to board based on Tetrimino shape.
        /// </summary>
        public void AddBricksFromTetrimino(Tetrimino tetrimino)
        {
            var ri = Tetrimino.SIZE;
            while (ri-- > 0)
            {
                for (var ci = 0; ci < Tetrimino.SIZE; ci++)
                {
                    if (!tetrimino.BricksMap[ri][ci]) continue;
                    var brickPos = new CCPoint(tetrimino.GridPos.X + ci, tetrimino.GridPos.Y + (Tetrimino.SIZE - ri - 1));
                    BricksMap[(int)brickPos.Y][(int)brickPos.X] = true;
                }
            }

            var gameOver = !RowIsEmpty(BricksMap[(int)Size.MaxY - 1]);
            if (gameOver) {
                GameState.GameOver();
            }
            UpdateBricks();
        }

        public void UpdateBricks() {
            RemoveAllChildren();

            // remove completed rows
            var removedCount = BricksMap.Where(row => RowIsCompleted(row)).ToList().Count;
            BricksMap = BricksMap.Where(row => !RowIsCompleted(row)).ToList();
            if (removedCount > 0)
            {
                AudioManager.Instance.PlaySoundEffect("clear");
                GameState.AddPointsForRowsCount(removedCount);
            }
            while (removedCount-- > -1)
            {
                BricksMap.Add(CreateRow((int)Size.MaxX));
            }

            // create bricks sprites
            for (var ri = 0; ri < Size.MaxY; ri++)
            {
                for (var ci = 0; ci < Size.MaxX; ci++)
                {
                    var brick = new Block(!BricksMap[ri][ci] ? CCColor3B.Gray : CCColor3B.Red);
                    brick.Position = new CCPoint(
                        ci * Block.WIDTH + Block.WIDTH / 2,
                        ri * Block.HEIGHT
                    );
                    AddChild(brick);
                }
            }
        }

        public List<List<bool>> CreateBricksMap(int width, int height, int level) {
            var bricksMap = new List<List<bool>>();
            for (var ri = 0; ri < height; ri++)
            {
                var rowHasBricks = ri < level;
                bricksMap.Add(CreateRow(width, rowHasBricks));
            }
            return bricksMap;
        }

        public bool RowIsCompleted(List<bool> row) {
            var ci = row.Count;
            while (ci-- > 0)
            {
                if (!row[ci]) return false;
            }
            return true;
        }

        public bool RowIsEmpty(List<bool> row) {
            var ci = row.Count;
            while (ci-- > 0)
            {
                if (row[ci]) return false;
            }
            return true;
        }

        public bool ColIsEmpty(List<List<bool>> bricksMap, int colInd) {
            var ri = bricksMap.Count;
            while (ri-- > 0)
            {
                if (bricksMap[ri][colInd]) return false;
            }
            return true;
        }

        public List<bool> CreateRow(int width, bool needCreateBricks = false) {
            var row = new List<bool>();
            var ci = width;
            while (ci-- > 0)
            {
                var hasBrick = needCreateBricks;
                row.Add(hasBrick);
            }
            return row;
        }

    }
}
