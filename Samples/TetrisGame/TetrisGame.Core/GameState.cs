using TetrisGame.Core.Managers;

namespace TetrisGame.Core
{
    public class GameState
    {
        const float INITIAL_SPEED = 0.5f;
        public int Level = 0;
        public int Points = 0;
        public int Lines = 0;
        public int LinesCountToLevelUp = 10;
        public float Speed = INITIAL_SPEED;
        public bool IsGameOver = false;

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
                AudioManager.Instance.PlaySoundEffect("levelup");
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
            if (AppDataManager.Instance.AppSettings.HighScore < Points)
            {
                AppDataManager.Instance.AppSettings.HighScore = Points;
                AppDataManager.Instance.SaveData();
            }
        }
    }
}
