using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TetrisGame.Core
{
	/// <summary>
	/// Represents a block that shapes consist of.
	/// </summary>
	public class Block
	{
		private Color background = new Color(20, 20, 20);
		private IBoard board;
		private Color colour;
		private Point position;

		/// <summary>
		/// Instantiates Block object.
		/// The coordinates with the values smaller than 0 
		/// and bigger than the width/height of the board are considered invalid.
		/// </summary>
		/// <param name="board">The Tetris board</param>
		/// <param name="colour">The colour of the block</param>
		/// <param name="position">The initial position of the block</param>
		public Block(IBoard board, Color colour, Point position)
		{
			if (board == null || colour == null || position == null)
				throw new ArgumentNullException();

			this.board = board;
			this.colour = colour;
			checkCoordinate(position, board);
			this.position = position;
		}

		/// <summary>
		/// Returns Block's colour.
		/// </summary>
		public Color Colour
		{ get { return colour; } }

		/// <summary>
		/// Returns Block's current position.
		/// </summary>
		public Point Position
		{
			get { return new Point(position.X, position.Y); }
			set
			{
				checkCoordinate(value, board);
				position.X = value.X;
				position.Y = value.Y;
			}
		}

		/// <summary>
		/// Moves the Block down if it is possible 
		/// (i.e. there are no other blocks and it is not the bottom of the board).
		/// </summary>
		public void MoveDown()
		{
			if (TryMoveDown())
				position.Y++;
		}

		/// <summary>
		/// Moves the Block left if it is possible
		/// (i.e. there are no blocks and it is not the wall of the board).
		/// </summary>
		public void MoveLeft()
		{
			if (TryMoveLeft())
				position.X--;
		}

		/// <summary>
		/// Moves the Block right if it is possible
		/// (i.e. there are no blocks and it is not the wall of the board).
		/// </summary>
		public void MoveRight()
		{
			if (TryMoveRight())
				position.X++;
		}

		/// <summary>
		/// Rotates the Block if it is possible
		/// (i.e. there are no blocks and it is not the wall of the board).
		/// </summary>
		/// <param name="offset">The coordinate by which the block changes the position</param>
		public void Rotate(Point offset)
		{
			if (TryRotate(offset))
			{
				position.X += offset.X;
				position.Y += offset.Y;
			}
		}

		/// <summary>
		/// Checks whether it is possible to move the Block down.
		/// (i.e. there are no other blocks and it is not the bottom of the board).
		/// </summary>
		/// <returns>true if it is possible; false otherwise</returns>
		public bool TryMoveDown()
		{
			int y = position.Y + 1;
			//the empty space is determined whether the colour at this position is board's background colour
			if (y >= board.GetLength(1) || board[position.X, y] != background)
				return false;
			return true;
		}

		/// <summary>
		/// Checks whether it is possible to move the Block left
		/// (i.e. there are no blocks and it is not the wall of the board).
		/// </summary>
		/// <returns>true if it is possible; false otherwise</returns>
		public bool TryMoveLeft()
		{
			int x = position.X - 1;
			//the empty space is determined whether the colour at this position is board's background colour
			if (x < 0 || board[x, position.Y] != background)
				return false;
			return true;
		}

		/// <summary>
		/// Checks whether it is possible to move the Block right
		/// (i.e. there are no blocks and it is not the wall of the board).
		/// </summary>
		/// <returns>true if it is possible; false otherwise</returns>
		public bool TryMoveRight()
		{
			int x = position.X + 1;
			//the empty space is determined whether the colour at this position is board's background colour
			if (x >= board.GetLength(0) || board[x, position.Y] != background)
				return false;
			return true;
		}

		/// <summary>
		/// Checks whether it is possible to rotate the Block
		/// (i.e. there are no blocks and it is not the wall of the board).
		/// </summary>
		/// <param name="offset"></param>
		/// <returns>true if it is possible; false otherwise</returns>
		public bool TryRotate(Point offset)
		{
			int x = position.X + offset.X;
			int y = position.Y + offset.Y;

			//the empty space is determined whether the colour at this position is board's background colour
			if (x < 0 || y < 0 || x >= board.GetLength(0) ||
				y >= board.GetLength(1) || board[x, y] != background)
				return false;
			return true;
		}

		//Checks whether given coordinates are valid
		//(i.e. they are not outside of the board boundaries.
		private static void checkCoordinate(Point coord, IBoard board)
		{
			if (coord.X < 0 || coord.Y < 0)
				throw new ArgumentException("Given coordinate is negative.");
			if (coord.X >= board.GetLength(0) || coord.Y >= board.GetLength(1))
				throw new ArgumentException("Given coordinate is out of border bounds.");
		}

	}
}
