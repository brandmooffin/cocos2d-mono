using System;
using System.Collections.Generic;
using System.Text;
using Cocos2D;
using Microsoft.Xna.Framework;

namespace TetrisGame.Core
{
	/// <summary>
	/// Represents a block that shapes consist of.
	/// </summary>
	public class Block : CCSprite
	{
		private Color background = new Color(20, 20, 20);
		private IBoard board;

		/// <summary>
		/// Instantiates Block object.
		/// The coordinates with the values smaller than 0 
		/// and bigger than the width/height of the board are considered invalid.
		/// </summary>
		/// <param name="board">The Tetris board</param>
		/// <param name="colour">The colour of the block</param>
		/// <param name="position">The initial position of the block</param>
		public Block(IBoard board, Color color, CCPoint position)
		{
			if (board == null || color == null || position == null)
				throw new ArgumentNullException();

			this.board = board;
			Color = new CCColor3B(color);
			checkCoordinate(position, board);
			Position = position;
		}

		/// <summary>
		/// Moves the Block down if it is possible 
		/// (i.e. there are no other blocks and it is not the bottom of the board).
		/// </summary>
		public void MoveDown()
		{
			if (TryMoveDown())
			{
				Position = new CCPoint(Position.X, Position.Y - 1);
			}
		}

		/// <summary>
		/// Moves the Block left if it is possible
		/// (i.e. there are no blocks and it is not the wall of the board).
		/// </summary>
		public void MoveLeft()
		{
			if (TryMoveLeft())
			{
				Position = new CCPoint(Position.X - 1, Position.Y);
			}
		}

		/// <summary>
		/// Moves the Block right if it is possible
		/// (i.e. there are no blocks and it is not the wall of the board).
		/// </summary>
		public void MoveRight()
		{
			if (TryMoveRight())
			{
				Position = new CCPoint(Position.X + 1, Position.Y);
			}
		}

		/// <summary>
		/// Rotates the Block if it is possible
		/// (i.e. there are no blocks and it is not the wall of the board).
		/// </summary>
		/// <param name="offset">The coordinate by which the block changes the position</param>
		public void Rotate(CCPoint offset)
		{
			if (TryRotate(offset))
			{
				Position = new CCPoint(Position.X + offset.X, PositionY + offset.Y);
			}
		}

		/// <summary>
		/// Checks whether it is possible to move the Block down.
		/// (i.e. there are no other blocks and it is not the bottom of the board).
		/// </summary>
		/// <returns>true if it is possible; false otherwise</returns>
		public bool TryMoveDown()
		{
			int y = (int)Position.Y + 1;
			//the empty space is determined whether the colour at this position is board's background colour
			if (y >= board.GetLength(1) || board[(int)Position.X, y] != background)
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
			int x = (int)Position.X - 1;
			//the empty space is determined whether the colour at this position is board's background colour
			if (x < 0 || board[x, (int)Position.Y] != background)
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
			int x = (int)(Position.X + 1);
			//the empty space is determined whether the colour at this position is board's background colour
			if (x >= board.GetLength(0) || board[x, (int)Position.Y] != background)
				return false;
			return true;
		}

		/// <summary>
		/// Checks whether it is possible to rotate the Block
		/// (i.e. there are no blocks and it is not the wall of the board).
		/// </summary>
		/// <param name="offset"></param>
		/// <returns>true if it is possible; false otherwise</returns>
		public bool TryRotate(CCPoint offset)
		{
			int x = (int)(Position.X + offset.X);
			int y = (int)(Position.Y + offset.Y);

			//the empty space is determined whether the colour at this position is board's background colour
			if (x < 0 || y < 0 || x >= board.GetLength(0) ||
				y >= board.GetLength(1) || board[x, y] != background)
				return false;
			return true;
		}

		//Checks whether given coordinates are valid
		//(i.e. they are not outside of the board boundaries.
		private static void checkCoordinate(CCPoint coord, IBoard board)
		{
			if (coord.X < 0 || coord.Y < 0)
				throw new ArgumentException("Given coordinate is negative.");
			if (coord.X >= board.GetLength(0) || coord.Y >= board.GetLength(1))
				throw new ArgumentException("Given coordinate is out of border bounds.");
		}

		public void Update()
        {
			checkCoordinate(Position, board);
		}

	}
}
