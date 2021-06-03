using System;
using System.Collections.Generic;
using System.Text;
using Cocos2D;
using Microsoft.Xna.Framework;

namespace TetrisGame.Core
{
	/// <summary>
	/// Represents S Tetris shape.
	/// </summary>
	public class ShapeS : Shape
	{
		private static CCColor3B ShapeColor = new CCColor3B(Microsoft.Xna.Framework.Color.Chartreuse);
		/// <summary>
		/// Instantiates ShapeS.
		/// </summary>
		/// <param name="board">The board where the ShapeS should be created.</param>
		public ShapeS(IBoard board) : base(board, setBlocks(board), setOffsets())
		{ }

		//Creates the blocks for ShapeS with green colour and pivot block starting at (6,0)
		private static Block[] setBlocks(IBoard board)
		{
			Block[] blocks = new Block[4];
			blocks[0] = new Block(board, ShapeColor, new CCPoint(6, 0));
			blocks[1] = new Block(board, ShapeColor, new CCPoint(7, 0));
			blocks[2] = new Block(board, ShapeColor, new CCPoint(5, 1));
			blocks[3] = new Block(board, ShapeColor, new CCPoint(6, 1));
			return blocks;
		}

		//Creates offsets for each block used for rotation.
		//Since ShapeS has 2 rotating positions, offset values are the opposites of each other for every rotation.
		private static Point[][] setOffsets()
		{
			Point[][] offsets = new Point[2][];
			offsets[0] = new Point[4] { new Point(0, 0), new Point(-1, -1), new Point(2, 0), new Point(1, -1) };
			offsets[1] = new Point[4];
			for (int i = 0; i < 4; i++)
				offsets[1][i] = new Point(-offsets[0][i].X, -offsets[0][i].Y);
			return offsets;
		}
	}
}
