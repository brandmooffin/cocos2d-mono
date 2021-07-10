using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisGame.Core
{
    class GameState
    {
        const float INITIAL_SPEED = 0.5f;
        int Level = 0;
        int Points = 0;
        int Lines = 0;
        int LinesCountToLevelUp = 10;
        float Speed = INITIAL_SPEED;
        bool IsGameOver = false;

        public GameState()
        {
            Level = 0;
            Points = 0;
            Lines = 0;
            LinesCountToLevelUp = 10;
            Speed = INITIAL_SPEED;
            IsGameOver = false;
        }

        public void AddPointsForRowsCount(int destroyedRowsCount)
        {
            Lines += destroyedRowsCount;
            Points += 1 * destroyedRowsCount * destroyedRowsCount;
        }

        public void SetLevel(int level)
        {
            Lines = 0;
            Level = level;
            Speed = INITIAL_SPEED - (0.05f * level);
        }

        public bool Check()
        {
            if (Lines >= LinesCountToLevelUp)
            {
                SetLevel(Level + 1);
                //$levelupSound.play();
                return true;
            }
            if (IsGameOver)
            {
                SetLevel(0);
                IsGameOver = false;
                Points = 0;
                return true;
            }
            return false;
        }

        public void GameOver()
        {
            IsGameOver = true;
        }
    }
}
