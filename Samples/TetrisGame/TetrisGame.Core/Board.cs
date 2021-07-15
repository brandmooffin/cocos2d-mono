//using System;
//using System.Collections.Generic;
//using System.Text;
//using Cocos2D;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using TetrisGame.Core.Scenes;

//namespace TetrisGame.Core
//{
//	/// <summary>
//	/// Delegate for GameOver event.
//	/// </summary>
//	public delegate void GameOverHandle();

//	/// <summary>
//	/// Delegate for LinesCleared event.
//	/// </summary>
//	/// <param name="num">The number of lines cleared</param>
//	public delegate void LinesClearedHandle(int num);

//	/// <summary>
//	/// Represents the Tetris board.
//	/// </summary>
//	public class Board : CCSprite
//	{
//		private Color background = new Color(20, 20, 20);
//		private CCColor3B infoFontColor = new CCColor3B(Microsoft.Xna.Framework.Color.SeaGreen);
//		private Color[,] board;
//		private Block[,] blocks;
//		private ShapeProxy shape;
//		private ShapeProxy nextShape;

//		GameScene gameScene;
//		CCLabelTTF TitleLabel;

//		CCLabelTTF HighestScoreLabel;
//		CCLabelTTF NextShapeLabel;
//		CCLabelTTF PauseResumeInfoLabel;
//		CCLabelTTF PauseResumeKeyLabel;

//        public Score Score;

//		/// <summary>
//		/// Fires when the game is over.
//		/// </summary>
//		public event GameOverHandle GameOver;
//		/// <summary>
//		/// Fires when some Board lines are cleared after the Shape joins the pile.
//		/// </summary>
//		public event LinesClearedHandle LinesCleared;

//		/// <summary>
//		/// Instantiates the Board object.
//		/// </summary>
//		public Board(GameScene gameScene)
//		{
//			this.gameScene = gameScene;
			

//			blocks = new Block[10, 21];
//			board = new Color[10, 21];
//			for (int i = 0; i < 10; i++)
//			{
//				for (int j = 0; j < 21; j++)
//				{
//					board[i, j] = background;
//					var block = new Block(this, background, new CCPoint(0, 0));
//					blocks[i,j] = block;
//					AddChild(block);
//				}
//			}

//			TitleLabel = new CCLabelTTF("TETRIS", "MarkerFelt", 18);
//			TitleLabel.Color = new CCColor3B(Microsoft.Xna.Framework.Color.LimeGreen);
//			TitleLabel.Position = new CCPoint(75, CCApplication.SharedApplication.GraphicsDevice.Viewport.Height - 15);

//			gameScene.AddChild(TitleLabel);

//			this.shape = new ShapeProxy(this);
//			shape.JoinPile += addToPile;
//			this.nextShape = new ShapeProxy(this);

//			NextShapeLabel = new CCLabelTTF("The Next Shape:", "MarkerFelt", 13);
//			NextShapeLabel.Color = CCColor3B.White;
//			NextShapeLabel.Position = new CCPoint(230, CCApplication.SharedApplication.GraphicsDevice.Viewport.Height - 80);
//			NextShapeLabel.VerticalAlignment = CCVerticalTextAlignment.Top;
//			NextShapeLabel.HorizontalAlignment = CCTextAlignment.Left;
//			NextShapeLabel.AnchorPoint = new CCPoint(0, 0);

//			HighestScoreLabel = new CCLabelTTF("The Highest Score:", "MarkerFelt", 13);
//			HighestScoreLabel.Color = CCColor3B.White;
//			HighestScoreLabel.Position = new CCPoint(230, CCApplication.SharedApplication.GraphicsDevice.Viewport.Height - 180);
//			HighestScoreLabel.VerticalAlignment = CCVerticalTextAlignment.Top;
//			HighestScoreLabel.HorizontalAlignment = CCTextAlignment.Left;
//			HighestScoreLabel.AnchorPoint = new CCPoint(0, 0);

//			PauseResumeInfoLabel = new CCLabelTTF("To pause/resume press ", "MarkerFelt", 13);
//			PauseResumeInfoLabel.Color = CCColor3B.White;
//			PauseResumeInfoLabel.Position = new CCPoint(230, CCApplication.SharedApplication.GraphicsDevice.Viewport.Height - 250);
//			PauseResumeInfoLabel.VerticalAlignment = CCVerticalTextAlignment.Top;
//			PauseResumeInfoLabel.HorizontalAlignment = CCTextAlignment.Left;
//			PauseResumeInfoLabel.AnchorPoint = new CCPoint(0, 0);

//			PauseResumeKeyLabel = new CCLabelTTF("SPACEBAR", "MarkerFelt", 18);
//			PauseResumeKeyLabel.Color = infoFontColor;
//			PauseResumeKeyLabel.Position = new CCPoint(230, CCApplication.SharedApplication.GraphicsDevice.Viewport.Height - 280);
//			PauseResumeKeyLabel.VerticalAlignment = CCVerticalTextAlignment.Top;
//			PauseResumeKeyLabel.HorizontalAlignment = CCTextAlignment.Left;
//			PauseResumeKeyLabel.AnchorPoint = new CCPoint(0, 0);

//			gameScene.AddChild(NextShapeLabel, 99);
//			gameScene.AddChild(HighestScoreLabel, 99);
//			gameScene.AddChild(PauseResumeInfoLabel, 99);
//			gameScene.AddChild(PauseResumeKeyLabel, 99);

//			Score = new Score(gameScene, this);
//		}

//		/// <summary>
//		/// The current Shape.
//		/// </summary>
//		public Shape Shape
//		{
//			get { return shape.Shape; }
//		}

//		/// <summary>
//		/// The next Shape.
//		/// </summary>
//		public Shape NextShape
//		{
//			get { return nextShape.Shape; }
//		}

//		/// <summary>
//		/// The colour of the specific place at the board.
//		/// </summary>
//		/// <param name="i">The coordinate in x-direction</param>
//		/// <param name="j">The coordinate in y-direction</param>
//		/// <returns>the colour of the specific place at the board</returns>
//		public Color this[int i, int j]
//		{
//			get
//			{
//				checkCoordinate(i, j);
//				return board[i, j];
//			}
//		}

//		/// <summary>
//		/// Returns the number of elements in the specified dimension of board.
//		/// </summary>
//		/// <param name="rank">Dimension at which the length will be determined</param>
//		/// <returns>the number of elements in the specified dimension of board</returns>
//		public int GetLength(int rank)
//		{
//			if (rank < 0 || rank >= board.Rank)
//			{
//				throw new IndexOutOfRangeException("Index: " + rank + ". Rank: " + board.Rank);
//			}
//			return board.GetLength(rank);
//		}

//		/// <summary>
//		/// Fires GameOver event.
//		/// </summary>
//		protected virtual void OnGameOver()
//		{
//			if (GameOver != null)
//			{
//				GameOver();
//			}
//		}

//		/// <summary>
//		/// Fires LinesCleared event.
//		/// </summary>
//		/// <param name="lines">The number of lines cleared</param>
//		protected virtual void OnLinesCleared(int lines)
//		{
//			if (LinesCleared != null)
//			{
//				LinesCleared(lines);
//			}
//		}

//		//Handles JoinPile event by adding the Shape to the board.
//		//The game is over if the shape touches the top of the board
//		//(i.e. no place left for the next one in this position).
//		private void addToPile()
//		{
//			for (int i = 0; i < shape.Length; i++)
//			{
//				Block block = shape[i];
//				board[(int)block.Position.X, (int)block.Position.Y] = block.Color;
//			}

//			checkClearedLines();

//			if (!checkGameOver())
//			{
//				shape.DeployNewShape(nextShape);
//				nextShape.DeployNewShape();
//			}
//		}

//		//Checks whether the full row is formed and modifies the Board accordingly.
//		//If some lines are cleared, fires LinesCleared event.
//		private void checkClearedLines()
//		{
//			int lines = 0;
//			bool noBlack = true;
//			int boardWidth = this.GetLength(0);
//			int boardHeight = this.GetLength(1);

//			for (int y = 0; y < boardHeight; y++)
//			{
//				for (int x = 0; x < boardWidth && noBlack; x++)
//				{
//					if (board[x, y] == background)
//					{
//						noBlack = false;
//					}
//					else if (x == (boardWidth - 1))
//					{
//						clearLine(y, boardWidth, boardHeight);
//						lines++;
//					}
//				}
//				noBlack = true;
//			}
//			if (lines > 0)
//			{
//				OnLinesCleared(lines);
//			}
//		}

//		//Checks whether the provided coordinate(x,y) is within the board boundaries.
//		private void checkCoordinate(int x, int y)
//		{
//			if (x < 0 || y < 0)
//			{
//				throw new IndexOutOfRangeException("Given coordinate is negative.");
//			}
//			if (x >= this.GetLength(0) || y >= this.GetLength(1))
//			{
//				throw new IndexOutOfRangeException("Given coordinate is out of border bounds.");
//			}
//		}

//		//Checks if the game is over. Returns true and fires the GameOver if it is the case; 
//		//false otherwise.
//		private bool checkGameOver()
//		{
//			int boardWidth = this.GetLength(0);
//			//x=0 to check all columns
//			for (int i = 0; i < boardWidth; i++)
//			{
//				//y=0 row is not a part of the game board and serves as a buffer for newly-created shapes
//				if (board[i, 1] != background)
//				{
//					//OnGameOver();
//					//return true;
//				}
//			}
//			return false;
//		}

//		//Removes the full row from the board.
//		//line - the line to be removed
//		private void clearLine(int line, int boardWidth, int boardHeight)
//		{
//			for (int y = line; y > 0; y--)
//			{
//				for (int x = 0; x < boardWidth; x++)
//				{
//					board[x, y] = board[x, y - 1];
//					blocks[x, y] = blocks[x, y - 1];
//				}
//			}
//			for (int x = 0; x < boardWidth; x++)
//			{
//				board[x, 0] = background;
//				blocks[x, 0].Color = new CCColor3B(background);
//			}
//		}

//		/// <summary>
//		/// Allows updating the Board.
//		/// </summary>
//		/// <param name="gameTime">Provides a snapshot of timing values.</param>
//		public void Update()
//		{
//			UpdateBlocks();
//			UpdateLabels();
//			Score.Update();
//		}

//		/// <summary>
//		/// Called when the Board is to be drawn.
//		/// </summary>
//		/// <param name="gameTime">Provides a snapshot of timing values.</param>
//		public void UpdateBlocks()
//		{
//			for (int i = 0; i < board.GetLength(0); i++)
//			{
//				for (int j = 1; j < board.GetLength(1); j++)
//				{
//					blocks[i, j].Position = new Vector2(20 + i * 20, CCApplication.SharedApplication.GraphicsDevice.Viewport.Height - (35 + (j + 1) * 20));
//					blocks[i, j].Color = new CCColor3B(board[i, j]);
//				}
//			}

//			//draws the next shape
//			Shape next = NextShape;
//			for (int i = 0; i < next.Length; i++)
//			{
//				next[i].Position = new Vector2(160 + next[i].Position.X * 20, CCApplication.SharedApplication.GraphicsDevice.Viewport.Height - 110 + next[i].Position.Y * 20);
//			}

//			Shape.Update();
//			NextShape.Update();
//		}

//		private void UpdateLabels()
//        {
		
//		}
//	}
//}
