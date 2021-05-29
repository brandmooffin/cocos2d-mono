using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisGame.Core
{
	/// <summary>
	/// Represents game score by keeping the total current score, level and cleared lines.
	/// </summary>
	public class Score
	{
		private int level = 1;
		private int lines = 0;
		private int score = 0;
		private IBoard board;

		/// <summary>
		/// Instantiates score object.
		/// Listens for the LinesCleared event.
		/// </summary>
		/// <param name="board">The Tetris board</param>
		public Score(IBoard board)
		{
			if (board == null)
				throw new ArgumentNullException();

			this.board = board;
			board.LinesCleared += incrementLinesCleared;
		}

		/// <summary>
		/// The current level of the game.
		/// </summary>
		public int Level
		{ get { return level; } }

		/// <summary>
		/// The number of lines cleared up to this moment in the game.
		/// </summary>
		public int Lines
		{ get { return lines; } }

		/// <summary>
		/// The current score of the game.
		/// </summary>
		public int ScoreValue
		{ get { return score; } }

		//Calculates the current score based on how many were cleared with the sigle move.
		//1 line gives 100 points.
		//4 lines give 800 points.
		private void calculateScore(int num)
		{
			if (num <= 0)
				throw new ArgumentException("Argument value is illegal: " + num);

			if (num < 4)
				score += num * 100;
			else
				score += (num / 4 * 800 + (num - num / 4 * 4) * 100);
		}

		//Calculates the current level.
		//The highest level is 10.
		private void calculateLevel()
		{
			int lvl = lines / 10 + 1;
			if (lvl <= 10)
				level = lvl;
			else
				level = 10;
		}

		//LinesCleared even handler.
		//Modifies the level and score based on how many lines cleared.
		//num - the number of lines cleared (cannot be negative or more than the board height).
		private void incrementLinesCleared(int num)
		{
			if (num < 0 || num >= board.GetLength(1))
				throw new ArgumentException("The number of lines value is out of range: " + num);

			lines += num;

			calculateLevel();

			calculateScore(num);
		}
	}
}
