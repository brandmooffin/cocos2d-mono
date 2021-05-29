using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TetrisGame.Core
{
	/// <summary>
	/// Represents the Tetris board.
	/// </summary>
	public class Board : IBoard
	{
		private Color background = new Color(20, 20, 20);
		private Color[,] board;
		private IShape shape;
		private IShape nextShape;

		/// <summary>
		/// Fires when the game is over.
		/// </summary>
		public event GameOverHandle GameOver;
		/// <summary>
		/// Fires when some Board lines are cleared after the Shape joins the pile.
		/// </summary>
		public event LinesClearedHandle LinesCleared;

		/// <summary>
		/// Instantiates the Board object.
		/// </summary>
		public Board()
		{
			board = new Color[10, 21];
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 21; j++)
					board[i, j] = background;
			}

			this.shape = new ShapeProxy(this);
			shape.JoinPile += addToPile;
			this.nextShape = new ShapeProxy(this);
		}

		/// <summary>
		/// The current Shape.
		/// </summary>
		public IShape Shape
		{
			get { return shape; }
		}

		/// <summary>
		/// The next Shape.
		/// </summary>
		public IShape NextShape
		{
			get { return nextShape; }
		}

		/// <summary>
		/// The colour of the specific place at the board.
		/// </summary>
		/// <param name="i">The coordinate in x-direction</param>
		/// <param name="j">The coordinate in y-direction</param>
		/// <returns>the colour of the specific place at the board</returns>
		public Color this[int i, int j]
		{
			get
			{
				checkCoordinate(i, j);
				return board[i, j];
			}
		}

		/// <summary>
		/// Returns the number of elements in the specified dimension of board.
		/// </summary>
		/// <param name="rank">Dimension at which the length will be determined</param>
		/// <returns>the number of elements in the specified dimension of board</returns>
		public int GetLength(int rank)
		{
			if (rank < 0 || rank >= board.Rank)
				throw new IndexOutOfRangeException("Index: " + rank + ". Rank: " + board.Rank);
			return board.GetLength(rank);
		}

		/// <summary>
		/// Fires GameOver event.
		/// </summary>
		protected virtual void OnGameOver()
		{
			if (GameOver != null)
				GameOver();
		}

		/// <summary>
		/// Fires LinesCleared event.
		/// </summary>
		/// <param name="lines">The number of lines cleared</param>
		protected virtual void OnLinesCleared(int lines)
		{
			if (LinesCleared != null)
				LinesCleared(lines);
		}

		//Handles JoinPile event by adding the Shape to the board.
		//The game is over if the shape touches the top of the board
		//(i.e. no place left for the next one in this position).
		private void addToPile()
		{
			for (int i = 0; i < shape.Length; i++)
			{
				Block block = shape[i];
				board[block.Position.X, block.Position.Y] = block.Colour;
			}

			checkClearedLines();

			if (!checkGameOver())
			{
				((ShapeProxy)shape).DeployNewShape((ShapeProxy)nextShape);
				((IShapeFactory)nextShape).DeployNewShape();
			}
		}

		//Checks whether the full row is formed and modifies the Board accordingly.
		//If some lines are cleared, fires LinesCleared event.
		private void checkClearedLines()
		{
			int lines = 0;
			bool noBlack = true;
			int boardWidth = this.GetLength(0);
			int boardHeight = this.GetLength(1);

			for (int y = 0; y < boardHeight; y++)
			{
				for (int x = 0; x < boardWidth && noBlack; x++)
				{
					if (board[x, y] == background)
						noBlack = false;
					else if (x == (boardWidth - 1))
					{
						clearLine(y, boardWidth, boardHeight);
						lines++;
					}
				}
				noBlack = true;
			}
			if (lines > 0)
			{
				OnLinesCleared(lines);
			}
		}

		//Checks whether the provided coordinate(x,y) is within the board boundaries.
		private void checkCoordinate(int x, int y)
		{
			if (x < 0 || y < 0)
				throw new IndexOutOfRangeException("Given coordinate is negative.");
			if (x >= this.GetLength(0) || y >= this.GetLength(1))
				throw new IndexOutOfRangeException("Given coordinate is out of border bounds.");
		}

		//Checks if the game is over. Returns true and fires the GameOver if it is the case; 
		//false otherwise.
		private bool checkGameOver()
		{
			int boardWidth = this.GetLength(0);
			//x=0 to check all columns
			for (int i = 0; i < boardWidth; i++)
			{
				//y=0 row is not a part of the game board and serves as a buffer for newly-created shapes
				if (board[i, 1] != background)
				{
					OnGameOver();
					return true;
				}
			}
			return false;
		}

		//Removes the full row from the board.
		//line - the line to be removed
		private void clearLine(int line, int boardWidth, int boardHeight)
		{
			for (int y = line; y > 0; y--)
			{
				for (int x = 0; x < boardWidth; x++)
					board[x, y] = board[x, y - 1];
			}
			for (int x = 0; x < boardWidth; x++)
				board[x, 0] = background;
		}
	}
}
