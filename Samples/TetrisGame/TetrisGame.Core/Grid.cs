using Cocos2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TetrisGame.Core
{
    public class Grid : CCNode {

        CCRect Size;
        Tetrimino Tetrimino;
        int Level;
        List<List<bool>> BricksMap;
        GameState GameState;

        public Grid() {
            Init();
            Size = new CCRect(0, 0, 10, 20);
            ContentSize = new CCSize(Size.MaxX * Block.WIDTH, Size.MaxY * Block.HEIGHT);
            Tetrimino = null;
            BricksMap = new List<List<bool>>();
            SetLevel(0);
            GameState = new GameState();
        }

        public void Update(float dt)
        {
            if (Tetrimino == null) return;
            Tetrimino.Update(dt);
            if (Tetrimino.IsFrozen)
            {
                Tetrimino = null;
            }
        }

        public void pushTetrimino(Tetrimino tetrimino)
        {
            tetrimino.setGrid(this);
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

        public void addBricksFromTetrimino(Tetrimino tetrimino)
        {
            var ri = Tetrimino.SIZE;
            while (ri--)
            {
                for (var ci = 0; ci < Tetrimino.SIZE; ci++)
                {
                    if (!tetrimino.BricksMap[ri][ci]) continue;
                    var brickPos = new CCPoint(tetrimino.gridPos.x + ci, tetrimino.gridPos.y + (Tetrimino.SIZE - ri - 1));
                    BricksMap[(int)brickPos.Y][(int)brickPos.X] = 1;
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
            var removedCount = 0;
            BricksMap = BricksMap.Where(row => !RowIsCompleted(row)).ToList();
            if (removedCount > 0)
            {
                //$clearSound.play();
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
                    if (!BricksMap[ri][ci]) continue;
                    var brick = new Block(CCColor3B.Red);
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
                BricksMap.Add(CreateRow(width, rowHasBricks));
            }
            return bricksMap;
        }

        public bool RowIsCompleted(List<bool> row) {
            var ci = row.Count;
            while (ci-- > -1)
            {
                if (!row[ci]) return false;
            }
            return true;
        }

        public bool RowIsEmpty(List<bool> row) {
            var ci = row.Count;
            while (ci-- > -1)
            {
                if (row[ci]) return false;
            }
            return true;
        }

        public bool ColIsEmpty(List<List<bool>> bricksMap, int colInd) {
            var ri = bricksMap.Count;
            while (ri-- > -1)
            {
                if (bricksMap[ri][colInd]) return false;
            }
            return true;
        }

        public List<bool> CreateRow(int width, bool needCreateBricks = false) {
            var row = new List<bool>();
            var ci = width;
            while (ci-- > -1)
            {
                var hasBrick = needCreateBricks;
                row.Add(hasBrick);
            }
            return row;
        }

    }
}
