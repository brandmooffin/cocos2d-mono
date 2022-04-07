using Cocos2D;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
using System.Linq;
using TetrisGame.Core.Enums;
using TetrisGame.Core.Managers;

namespace TetrisGame.Core
{
    /// <summary>
    /// Represents a shape.
    /// </summary>
    public class Tetrimino : CCSprite
    {

        float FallSpeed;
        float TimeToFall;

        public const int ACTION_NONE = 0;
        public const int ACTION_MOVE_LEFT = 1;
        public const int ACTION_MOVE_RIGHT = 2;
        public const int ACTION_MOVE_DOWN = 3;
        public const int ACTION_STOP_MOVE = 4;
        public const int ACTION_ROTATE = 5;
        public const int SIZE = 4;

        Grid Grid;
        public CCPoint GridPos;
        List<List<List<int>>> BricksPlan;
        public List<List<bool>> BricksMap;
        int RotationInd;
        int Action;
        public bool IsFrozen;
        bool IsAccelerated;

        int InputDelay = 5;

        /// <summary>
        /// Create a new shape to be added to the grid
        /// </summary>
        /// <param name="fallSpeed">The rate the shape will fall</param>
        /// <param name="grid">Grid to add the shape to</param>
        public Tetrimino(float fallSpeed, Grid grid)
        {
            FallSpeed = fallSpeed;
            TimeToFall = fallSpeed;
            ContentSize = new CCSize(Block.WIDTH * SIZE, Block.HEIGHT * SIZE);
            GridPos = new CCPoint(0, 0);
            BricksPlan = Utils.GetRandomItem(SHAPES);
            RotationInd = Utils.GetRandomInt(BricksPlan.Count);
            BricksMap = new List<List<bool>>();
            Action = ACTION_NONE;
            IsFrozen = false;
            IsAccelerated = false;
            Grid = grid;


            Render();
            SetGridPos(new CCPoint(3, 16 + GetPaddings().MaxY));
        }

        public override void Update(float dt)
        {
            if (InputDelay < 0)
            {
                OnKeyboardEvent(Keyboard.GetState());
                InputDelay = 5;
            }
            InputDelay -= 1;
            switch (Action)
            {
                case ACTION_MOVE_LEFT: MoveLeft(); break;
                case ACTION_MOVE_RIGHT: MoveRight(); break;
                case ACTION_MOVE_DOWN: Accelerate(); break;
                case ACTION_ROTATE: Rotate(); break;
                case ACTION_STOP_MOVE: StopAcceleration(); break;
            }
            Action = ACTION_NONE;

            if (IsAccelerated)
            {
                MoveDown();
            }

            TimeToFall -= dt;
            if (TimeToFall <= 0)
            {

                TimeToFall = FallSpeed;
                if (CanMoveDown())
                {
                    MoveDown();
                }
                else
                {
                    AudioManager.Instance.PlaySoundEffect("click");
                    Freeze();
                }
            }
        }

        public void MoveRight()
        {
            var newPos = new CCPoint(GridPos.X + 1, GridPos.Y);
            if (IsValidPosition(newPos)) SetGridPos(newPos);
            AudioManager.Instance.PlaySoundEffect("click");
        }

        public void MoveLeft()
        {
            var newPos = new CCPoint(GridPos.X - 1, GridPos.Y);
            if (IsValidPosition(newPos)) SetGridPos(newPos);
            AudioManager.Instance.PlaySoundEffect("click");
        }

        public void MoveDown()
        {
            if (CanMoveDown())
            {
                SetGridPos(new CCPoint(GridPos.X, GridPos.Y - 1));
                AudioManager.Instance.PlaySoundEffect("click");
            }
        }

        public bool CanMoveDown()
        {
            return IsValidPosition(new CCPoint(GridPos.X, GridPos.Y - 1));
        }

        public void Accelerate()
        {
            IsAccelerated = true;
        }

        public void StopAcceleration()
        {
            IsAccelerated = false;
        }

        public void Rotate()
        {
            var rotatedBricksMap = GetRotatedBricksMap().ToList().Select(bp => bp.ToList().Select(p => p > 0).ToList()).ToList();
            var rotatedPaddings = GetPaddings(rotatedBricksMap);
            var canRotate = false;
            if (IsValidPosition(GridPos, rotatedBricksMap))
            {
                canRotate = true;
            }
            else
            {
                var leftLedge = -(GridPos.X + rotatedPaddings.MinX);
                var rightLedge = (GridPos.X + SIZE - rotatedPaddings.MaxX) - Grid.Size.MaxX;
                var correctToRightPos = new CCPoint(GridPos.X + leftLedge, GridPos.Y);
                var correctToLeftPos = new CCPoint(GridPos.X - rightLedge, GridPos.Y);
                if (leftLedge > 0 && IsValidPosition(correctToRightPos, rotatedBricksMap))
                {
                    SetGridPos(correctToRightPos);
                    canRotate = true;
                }
                else if (rightLedge > 0 && IsValidPosition(correctToLeftPos, rotatedBricksMap))
                {
                    SetGridPos(correctToLeftPos);
                    canRotate = true;
                }
            }
            if (canRotate)
            {
                AudioManager.Instance.PlaySoundEffect("click");
                RotationInd = GetNextRotationInd();
                Render();
            }
        }

        public void Freeze()
        {
            IsFrozen = true;
            Grid.AddBricksFromTetrimino(this);
        }

        public int GetNextRotationInd()
        {
            var nextRotationInd = RotationInd + 1;
            return nextRotationInd < BricksPlan.Count ? nextRotationInd : 0;
        }

        public List<List<int>> GetRotatedBricksMap()
        {
            return BricksPlan[GetNextRotationInd()];
        }

        public void SetGridPos(CCPoint gridPos)
        {
            GridPos.X = gridPos.X >= -1 ? gridPos.X : GridPos.X;
            GridPos.Y = gridPos.Y >= -1 ? gridPos.Y : GridPos.Y;
            Position = new CCPoint(
              GridPos.X * Block.WIDTH + ContentSize.Width / 2,
              GridPos.Y * Block.HEIGHT + ContentSize.Height / 2
            );
        }

        public bool IsValidPosition(CCPoint gridPos, List<List<bool>> bricksMap = null)
        {
            bricksMap = bricksMap ?? BricksMap;
            var ri = SIZE;
            while (ri-- > 0)
            {
                for (var ci = 0; ci < SIZE; ci++)
                {
                    if (!bricksMap[ri][ci]) continue;
                    var brickPos = new CCPoint(gridPos.X + ci, gridPos.Y + SIZE - ri - 1);
                    var isOutOfBounds = (
                      brickPos.Y < 0 || brickPos.X < 0 ||
                      brickPos.X >= Grid.Size.MaxX ||
                      brickPos.Y >= Grid.Size.MaxY
                    );
                    if (isOutOfBounds) return false;
                    var isCollidesBricks = Grid.BricksMap[(int)brickPos.Y][(int)brickPos.X];
                    if (isCollidesBricks) return false;
                }
            }
            return true;
        }

        public void Render()
        {
            RemoveAllChildren();
            BricksMap = BricksPlan[RotationInd].ToList().Select(bp => bp.ToList().Select(p => p > 0).ToList()).ToList();
            var rowInd = SIZE;
            while (rowInd-- > 0)
            {
                for (var colInd = 0; colInd < Tetrimino.SIZE; colInd++)
                {
                    if (!BricksMap[rowInd][colInd]) continue;
                    var brick = new Block(CCColor3B.Green);
                    brick.Position = new CCPoint(
                        colInd * Block.WIDTH + Block.WIDTH / 2,
                        (SIZE - rowInd - 1) * Block.HEIGHT
                    );
                    AddChild(brick);
                }
            }
        }

        public void OnKeyboardEvent(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyUp(Keys.Down))
            {
                Action = ACTION_STOP_MOVE;
            }

            if (keyboardState.IsKeyDown(Keys.Right) 
                || InputManager.Instance.CurrentFlickDirection == FlickDirection.Right)
            {
                Action = ACTION_MOVE_RIGHT;
            }
            if (keyboardState.IsKeyDown(Keys.Left)
                || InputManager.Instance.CurrentFlickDirection == FlickDirection.Left)
            {
                Action = ACTION_MOVE_LEFT;
            }
            if (keyboardState.IsKeyDown(Keys.Down)
                || InputManager.Instance.CurrentFlickDirection == FlickDirection.Down)
            {
                Action = ACTION_MOVE_DOWN;
            }
            if (keyboardState.IsKeyDown(Keys.Space) || InputManager.Instance.IsTapping)
            {
                Action = ACTION_ROTATE;
            }

            InputManager.Instance.ClearInputs();
        }

        public void SetGrid(Grid tetrisGrid)
        {
            Grid = tetrisGrid;
        }

        public CCRect GetPaddings()
        {
            return GetPaddings(BricksMap);
        }
        public CCRect GetPaddings(List<List<bool>> bricksMap)
        {
            var paddingsLeft = 0;
            var paddingsRight = 0;
            var paddingsTop = 0;
            var ri = SIZE;
            while (ri-- > -1)
            {
                if (!Grid.RowIsEmpty(bricksMap[ri])) break;
                paddingsTop++;
            }

            for (var ci = 0; ci < SIZE; ci++)
            {
                if (!Grid.ColIsEmpty(bricksMap, ci)) break;
                paddingsLeft++;
            }

            var cSize = SIZE;
            while (cSize-- > -1)
            {
                if (!Grid.ColIsEmpty(bricksMap, cSize)) break;
                paddingsRight++;
            }
            return new CCRect(paddingsLeft, 0, paddingsRight, paddingsTop); ;
        }

        /// <summary>
        /// List of possible shapes that can be created
        /// </summary>
        public static List<List<List<List<int>>>> SHAPES = new List<List<List<List<int>>>> {

            // square
            new List<List<List<int>>> 
            { 
                new List<List<int>>
                {
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 0, 1, 1, 0 },
                    new List<int> { 0, 1, 1, 0 },
                    new List<int> { 0, 0, 0, 0 }
                }
            },
            // T
            new List<List<List<int>>> 
            {
                new List<List<int>>
                {
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 0, 1, 0, 0 },
                    new List<int> { 1, 1, 1, 0 },
                    new List<int> { 0, 0, 0, 0 }
                },
                new List<List<int>>
                {
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 0, 1, 0, 0 },
                    new List<int> { 0, 1, 1, 0 },
                    new List<int> { 0, 1, 0, 0 }
                },
                new List<List<int>>
                {
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 1, 1, 1, 0 },
                    new List<int> { 0, 1, 0, 0 }
                },
                new List<List<int>>
                {
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 0, 0, 1, 0 },
                    new List<int> { 0, 1, 1, 0 },
                    new List<int> { 0, 0, 1, 0 }
                }
            },
            // I
            new List<List<List<int>>> 
            {
                new List<List<int>>
                {
                    new List<int> { 0, 1, 0, 0 },
                    new List<int> { 0, 1, 0, 0 },
                    new List<int> { 0, 1, 0, 0 },
                    new List<int> { 0, 1, 0, 0 }
                },
                new List<List<int>>
                {
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 1, 1, 1, 1 },
                    new List<int> { 0, 0, 0, 0 }
                }
            },
            // L
            new List<List<List<int>>> 
            {
                new List<List<int>>
                {
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 0, 1, 0, 0 },
                    new List<int> { 0, 1, 0, 0 },
                    new List<int> { 0, 1, 1, 0 }
                },
                new List<List<int>>
                {
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 0, 1, 1, 1 },
                    new List<int> { 0, 1, 0, 0 },
                    new List<int> { 0, 0, 0, 0 }
                },
                new List<List<int>>
                {
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 0, 1, 1, 0 },
                    new List<int> { 0, 0, 1, 0 },
                    new List<int> { 0, 0, 1, 0 }
                },
                new List<List<int>>
                {
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 0, 0, 1, 0 },
                    new List<int> { 1, 1, 1, 0 },
                    new List<int> { 0, 0, 0, 0 }
                }
            },

            // L - reversed
            new List<List<List<int>>> 
            {
                new List<List<int>>
                {
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 0, 0, 1, 0 },
                    new List<int> { 0, 0, 1, 0 },
                    new List<int> { 0, 1, 1, 0 }
                },
                new List<List<int>>
                {
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 0, 1, 0, 0 },
                    new List<int> { 0, 1, 1, 1 },
                    new List<int> { 0, 0, 0, 0 }
                },
                new List<List<int>>
                {
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 0, 1, 1, 0 },
                    new List<int> { 0, 1, 0, 0 },
                    new List<int> { 0, 1, 0, 0 }
                },
                new List<List<int>>
                {
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 1, 1, 1, 0 },
                    new List<int> { 0, 0, 1, 0 },
                    new List<int> { 0, 0, 0, 0 }
                }
            },


            // Z
            new List<List<List<int>>> 
            {
                new List<List<int>>
                {
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 0, 0, 1, 0 },
                    new List<int> { 0, 1, 1, 0 },
                    new List<int> { 0, 1, 0, 0 }
                },
                new List<List<int>>
                {
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 0, 1, 1, 0 },
                    new List<int> { 0, 0, 1, 1 },
                    new List<int> { 0, 0, 0, 0 }
                }
            },

            // Z - reversed
            new List<List<List<int>>> 
            {
                new List<List<int>>
                {
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 0, 1, 0, 0 },
                    new List<int> { 0, 1, 1, 0 },
                    new List<int> { 0, 0, 1, 0 }
                },
                new List<List<int>>
                {
                    new List<int> { 0, 0, 0, 0 },
                    new List<int> { 0, 0, 1, 1 },
                    new List<int> { 0, 1, 1, 0 },
                    new List<int> { 0, 0, 0, 0 }
                }
            }
        };
    }
}
