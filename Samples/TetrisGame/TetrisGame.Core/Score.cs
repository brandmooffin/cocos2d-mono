//using Cocos2D;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using TetrisGame.Core.Scenes;

//namespace TetrisGame.Core
//{
//	/// <summary>
//	/// Represents game score by keeping the total current score, level and cleared lines.
//	/// </summary>
//	public class Score : CCSprite
//	{
//		private int level = 1;
//		private int lines = 0;
//		private int score = 0;
//		private Board board;
//		private String gameover = "";
//		private GameScene gameScene;

//		private CCColor3B scoreFontColor = new CCColor3B(Microsoft.Xna.Framework.Color.Silver);
//		private CCColor3B infoFontColor = new CCColor3B(Microsoft.Xna.Framework.Color.SeaGreen);

//		//the previous high score
//		int highScore = 0;

//		CCLabelTTF ScoreLabel;
//		CCLabelTTF LevelLabel;
//		CCLabelTTF LinesClearedLabel;
//		CCLabelTTF GameOverLabel;
//		CCLabelTTF HighScoreLabel;

//		/// <summary>
//		/// Instantiates score object.
//		/// Listens for the LinesCleared event.
//		/// </summary>
//		/// <param name="board">The Tetris board</param>
//		public Score(GameScene gameScene, Board board)
//		{
//			if (board == null)
//			{
//				throw new ArgumentNullException();
//			}

//			this.board = board;
//			this.gameScene = gameScene;
//			board.LinesCleared += incrementLinesCleared;
//			//game.Exiting += saveResults;
//			findHighScore();

//			SetupScoreLabels();
//		}

//		/// <summary>
//		/// The current level of the game.
//		/// </summary>
//		public int Level
//		{ get { return level; } }

//		/// <summary>
//		/// The number of lines cleared up to this moment in the game.
//		/// </summary>
//		public int Lines
//		{ get { return lines; } }

//		/// <summary>
//		/// The current score of the game.
//		/// </summary>
//		public int ScoreValue
//		{ get { return score; } }

//		//Calculates the current score based on how many were cleared with the sigle move.
//		//1 line gives 100 points.
//		//4 lines give 800 points.
//		private void calculateScore(int num)
//		{
//			if (num <= 0)
//			{
//				throw new ArgumentException("Argument value is illegal: " + num);
//			}

//			if (num < 4)
//			{
//				score += num * 100;
//			}
//			else
//			{
//				score += (num / 4 * 800 + (num - num / 4 * 4) * 100);
//			}
//		}

//		//Calculates the current level.
//		//The highest level is 10.
//		private void calculateLevel()
//		{
//			int lvl = lines / 10 + 1;
//			if (lvl <= 10)
//				level = lvl;
//			else
//				level = 10;
//		}

//		//LinesCleared even handler.
//		//Modifies the level and score based on how many lines cleared.
//		//num - the number of lines cleared (cannot be negative or more than the board height).
//		private void incrementLinesCleared(int num)
//		{
//			if (num < 0 || num >= board.GetLength(1))
//			{
//				throw new ArgumentException("The number of lines value is out of range: " + num);
//			}

//			lines += num;

//			calculateLevel();

//			calculateScore(num);
//		}

//		public void Update()
//        {
//			UpdateLabels();
//        }

//		/// <summary>
//		/// Draws the Score, including the score, the level, the number of cleared lines,
//		/// the previous hign score, and the title of the game.
//		/// </summary>
//		/// <param name="gameTime">Provides a snapshot of timing values.</param>
//		public void UpdateLabels()
//		{

//			//spriteBatch.DrawString(fontEm,
//			//	"Score: " + score.ScoreValue + "\nLevel: " + score.Level + "\nNumber of cleared lines: "
//			//	+ score.Lines + gameover, new Vector2(20, GraphicsDevice.Viewport.Height - 90), fontColour);
//			ScoreLabel.Text = "Score: " + ScoreValue;
//			LevelLabel.Text = "Level:" + Level;
//			LinesClearedLabel.Text = "Number of cleared lines: " + Lines;

//			HighScoreLabel.Text = highScore.ToString();

//			LevelLabel.Color = scoreFontColor;
//			LinesClearedLabel.Color = scoreFontColor;
//			ScoreLabel.Color = scoreFontColor;

//		}

//		//Displays the game over message.
//		public void HandleGameOver()
//		{
//			scoreFontColor = CCColor3B.Red;
//			GameOverLabel.Visible = true;
//		}

//		private void SetupScoreLabels()
//        {
//			ScoreLabel = new CCLabelTTF("Score: test", "MarkerFelt", 13);
//			ScoreLabel.Color = scoreFontColor;
//			ScoreLabel.Position = new CCPoint(20, 90);
//			ScoreLabel.VerticalAlignment = CCVerticalTextAlignment.Top;
//			ScoreLabel.HorizontalAlignment = CCTextAlignment.Left;
//			ScoreLabel.AnchorPoint = new CCPoint(0, 0);

//			LevelLabel = new CCLabelTTF("Score: test", "MarkerFelt", 13);
//			LevelLabel.Color = scoreFontColor;
//			LevelLabel.Position = new CCPoint(20, 70);
//			LevelLabel.VerticalAlignment = CCVerticalTextAlignment.Top;
//			LevelLabel.HorizontalAlignment = CCTextAlignment.Left;
//			LevelLabel.AnchorPoint = new CCPoint(0, 0);

//			LinesClearedLabel = new CCLabelTTF("Score: test", "MarkerFelt", 13);
//			LinesClearedLabel.Color = scoreFontColor;
//			LinesClearedLabel.Position = new CCPoint(20, 50);
//			LinesClearedLabel.VerticalAlignment = CCVerticalTextAlignment.Top;
//			LinesClearedLabel.HorizontalAlignment = CCTextAlignment.Left;
//			LinesClearedLabel.AnchorPoint = new CCPoint(0, 0);

//			GameOverLabel = new CCLabelTTF("GAME OVER", "MarkerFelt", 13);
//			GameOverLabel.Color = CCColor3B.Red;
//			GameOverLabel.Position = new CCPoint(20, 30);
//			GameOverLabel.VerticalAlignment = CCVerticalTextAlignment.Top;
//			GameOverLabel.HorizontalAlignment = CCTextAlignment.Left;
//			GameOverLabel.AnchorPoint = new CCPoint(0, 0);
//			GameOverLabel.Visible = false;

//			HighScoreLabel = new CCLabelTTF("0", "MarkerFelt", 18);
//			HighScoreLabel.Color = infoFontColor;
//			HighScoreLabel.Position = new CCPoint(230, CCApplication.SharedApplication.GraphicsDevice.Viewport.Height - 200);
//			HighScoreLabel.VerticalAlignment = CCVerticalTextAlignment.Top;
//			HighScoreLabel.HorizontalAlignment = CCTextAlignment.Left;
//			HighScoreLabel.AnchorPoint = new CCPoint(0, 0);

//			gameScene.AddChild(ScoreLabel, 99);
//			gameScene.AddChild(LevelLabel, 99);
//			gameScene.AddChild(LinesClearedLabel, 99);
//			gameScene.AddChild(GameOverLabel, 99);
//			gameScene.AddChild(HighScoreLabel, 99);
//		}

//		//Determines the previous high score from the file.
//		private void findHighScore()
//		{
//			try
//			{
//				if (!File.Exists("score.txt"))
//				{
//					File.WriteAllLines("score.txt", new string[1] { "0" });
//				}
//				else
//				{
//					string[] array = File.ReadAllLines("score.txt");
//					highScore = Int32.Parse(array[0]);
//				}
//			}
//			catch (IOException io)
//			{ }
//		}

//		//If the current score is higher than the previous high score, saves it to the file.
//		private void saveResults(object sender, EventArgs e)
//		{
//			if (ScoreValue > highScore)
//			{
//				try
//				{
//					File.WriteAllLines("score.txt", new string[1] { ScoreValue + "" });
//				}
//				catch (IOException io)
//				{ }
//			}
//		}
//	}
//}
