using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisGame.Core
{
    class GameState
    {
        const float INITIAL_SPEED = 0.5f;
        int level = 0;
        int points = 0;
        int lines = 0;
        int linesCountToLevelUp = 10;
        float speed = INITIAL_SPEED;
        bool isGameOver = false;

        public GameState()
        {
            level = 0;
            points = 0;
            lines = 0;
            linesCountToLevelUp = 10;
            speed = INITIAL_SPEED;
            isGameOver = false;
        }

        public void addPointsForRowsCount(int destroyedRowsCount)
        {
            this.lines += destroyedRowsCount;
            this.points += 1 * destroyedRowsCount * destroyedRowsCount;
        }

        public void setLevel(int level)
        {
            this.lines = 0;
            this.level = level;
            this.speed = INITIAL_SPEED - (0.05f * level);
        }

        public bool check()
        {
            if (this.lines >= this.linesCountToLevelUp)
            {
                this.setLevel(this.level + 1);
                //$levelupSound.play();
                return true;
            }
            if (this.isGameOver)
            {
                this.setLevel(0);
                this.isGameOver = false;
                this.points = 0;
                return true;
            }
            return false;
        }

        public void gameOver()
        {
            this.isGameOver = true;
        }
    }
}
