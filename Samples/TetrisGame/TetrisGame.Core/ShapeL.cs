using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TetrisGame.Core
{
	/// <summary>
	/// Represents L Tetris shape.
	/// </summary>
	public class ShapeL : Shape
	{
		/// <summary>
		/// Instantiates ShapeL.
		/// </summary>
		/// <param name="board">The board where the ShapeL should be created.</param>
		public ShapeL(IBoard board) : base(board, setBlocks(board), setOffsets())
		{ }

		//Creates the blocks for ShapeL with orange colour and pivot block starting at (6,0)
		private static Block[] setBlocks(IBoard board)
		{
			Block[] blocks = new Block[4];
			blocks[0] = new Block(board, Color.DarkOrange, new Point(5, 1));
			blocks[1] = new Block(board, Color.DarkOrange, new Point(5, 0));
			blocks[2] = new Block(board, Color.DarkOrange, new Point(6, 0));
			blocks[3] = new Block(board, Color.DarkOrange, new Point(7, 0));
			return blocks;
		}

		//Creates offsets for each block used for rotation.
		//ShapeL has four possible rotations.
		private static Point[][] setOffsets()
		{
			Point[][] offsets = new Point[4][];
			offsets[0] = new Point[4] { new Point(2, 0), new Point(1, 1), new Point(0, 0), new Point(-1, -1) };
			offsets[1] = new Point[4] { new Point(0, -2), new Point(1, -1), new Point(0, 0), new Point(-1, 1) };
			offsets[2] = new Point[4] { new Point(-2, 0), new Point(-1, -1), new Point(0, 0), new Point(1, 1) };
			offsets[3] = new Point[4] { new Point(0, 2), new Point(-1, 1), new Point(0, 0), new Point(1, -1) };
			return offsets;
		}
	}
}
