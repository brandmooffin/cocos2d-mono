using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TetrisGame.Core
{
	/// <summary>
	/// Represents Z Tetris shape.
	/// </summary>
	public class ShapeZ : Shape
	{
		/// <summary>
		/// Instantiates ShapeZ.
		/// </summary>
		/// <param name="board">The board where the ShapeZ should be created.</param>
		public ShapeZ(IBoard board) : base(board, setBlocks(board), setOffsets())
		{ }

		//Creates the blocks for ShapeZ with red colour and pivot block starting at (6,0)
		private static Block[] setBlocks(IBoard board)
		{
			Block[] blocks = new Block[4];
			blocks[0] = new Block(board, Color.DarkRed, new Point(5, 0));
			blocks[1] = new Block(board, Color.DarkRed, new Point(6, 0));
			blocks[2] = new Block(board, Color.DarkRed, new Point(6, 1));
			blocks[3] = new Block(board, Color.DarkRed, new Point(7, 1));
			return blocks;
		}

		//Creates offsets for each block used for rotation.
		//Since ShapeZ has 2 rotating positions, offset values are the opposites of each other for every rotation.
		private static Point[][] setOffsets()
		{
			Point[][] offsets = new Point[2][];
			offsets[0] = new Point[4] { new Point(1, 1), new Point(0, 0), new Point(1, -1), new Point(0, -2) };
			offsets[1] = new Point[4];
			for (int i = 0; i < 4; i++)
				offsets[1][i] = new Point(-offsets[0][i].X, -offsets[0][i].Y);
			return offsets;
		}
	}
}
