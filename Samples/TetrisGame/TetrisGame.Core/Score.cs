using Cocos2D;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TetrisGame.Core.Scenes;

namespace TetrisGame.Core
{
	/// <summary>
	/// Represents game score by keeping the total current score, level and cleared lines.
	/// </summary>
	public class Score : CCSprite
	{
		private int level = 1;
		private int lines = 0;
		private int score = 0;
		private IBoard board;
		private String gameover = "";
		private CCColor3B fontColour = new CCColor3B(Microsoft.Xna.Framework.Color.Silver);
		//the previous high score
		int highScore = 0;

		/// <summary>
		/// Instantiates score object.
		/// Listens for the LinesCleared event.
		/// </summary>
		/// <param name="board">The Tetris board</param>
		public Score(GameScene game, Board board)
		{
			if (board == null)
				throw new ArgumentNullException();

			this.board = board;
			board.LinesCleared += incrementLinesCleared;
			//game.Exiting += saveResults;
			findHighScore();
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

		/// <summary>
		/// Draws the Score, including the score, the level, the number of cleared lines,
		/// the previous hign score, and the title of the game.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public void Draw()
		{
			//Color baseTxt = Color.White;
			//Color emTxt = Color.SeaGreen;

			//spriteBatch.DrawString(fontEm,
			//	"Score: " + score.ScoreValue + "\nLevel: " + score.Level + "\nNumber of cleared lines: "
			//	+ score.Lines + gameover, new Vector2(20, GraphicsDevice.Viewport.Height - 90), fontColour);
			//spriteBatch.DrawString(fontTitle, "TETRIS", new Vector2(75, 15), Color.LimeGreen);

			//spriteBatch.DrawString(font, "The next shape:", new Vector2(230, 80), baseTxt);
			//spriteBatch.DrawString(font, "The Highest Score:", new Vector2(230, 180), baseTxt);
			//spriteBatch.DrawString(fontTitle, "" + highScore, new Vector2(230, 200), emTxt);

			//spriteBatch.DrawString(font, "To pause/resume\nthe game press ", new Vector2(230, 250), baseTxt);
			//spriteBatch.DrawString(fontEm, "P", new Vector2(365, 270), emTxt);
			//spriteBatch.DrawString(font, "key", new Vector2(380, 270), baseTxt);
			//spriteBatch.DrawString(font, "or a", new Vector2(230, 290), baseTxt);
			//spriteBatch.DrawString(fontEm, "SPACEBAR.", new Vector2(270, 290), emTxt);

			//spriteBatch.DrawString(font, "To enter the Ghost Mode\npress", new Vector2(230, 330), baseTxt);
			//spriteBatch.DrawString(fontEm, "G", new Vector2(285, 350), emTxt);
			//spriteBatch.DrawString(font, "key.", new Vector2(305, 350), baseTxt);


			//spriteBatch.DrawString(font, "To drop the shape\npress", new Vector2(230, 390), baseTxt);
			//spriteBatch.DrawString(fontEm, "ENTER.", new Vector2(285, 410), emTxt);

		}

		//Displays the game over message.
		public void HandleGameOver()
		{
			gameover = "\nGAME OVER";
			fontColour = CCColor3B.Red;
		}

		//Determines the previous high score from the file.
		private void findHighScore()
		{
			try
			{
				if (!File.Exists("score.txt"))
				{
					File.WriteAllLines("score.txt", new string[1] { "0" });
				}
				else
				{
					string[] array = File.ReadAllLines("score.txt");
					highScore = Int32.Parse(array[0]);
				}
			}
			catch (IOException io)
			{ }
		}

		//If the current score is higher than the previous high score, saves it to the file.
		private void saveResults(object sender, EventArgs e)
		{
			if (ScoreValue > highScore)
			{
				try
				{
					File.WriteAllLines("score.txt", new string[1] { ScoreValue + "" });
				}
				catch (IOException io)
				{ }
			}
		}
	}
}
