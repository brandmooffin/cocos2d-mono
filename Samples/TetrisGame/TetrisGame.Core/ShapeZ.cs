using System;
using System.Collections.Generic;
using System.Text;
using Cocos2D;
using Microsoft.Xna.Framework;

namespace TetrisGame.Core
{
	/// <summary>
	/// Represents Z Tetris shape.
	/// </summary>
	public class ShapeZ : Shape
	{
		private static CCColor3B ShapeColor = new CCColor3B(Microsoft.Xna.Framework.Color.DarkRed);
		/// <summary>
		/// Instantiates ShapeZ.
		/// </summary>
		/// <param name="board">The board where the ShapeZ should be created.</param>
		public ShapeZ(Board board) : base(board, setBlocks(board), setOffsets())
		{ }

		//Creates the blocks for ShapeZ with red colour and pivot block starting at (6,0)
		private static Block[] setBlocks(Board board)
		{
			Block[] blocks = new Block[4];
			blocks[0] = new Block(board, ShapeColor, new CCPoint(5, 0));
			blocks[1] = new Block(board, ShapeColor, new CCPoint(6, 0));
			blocks[2] = new Block(board, ShapeColor, new CCPoint(6, 1));
			blocks[3] = new Block(board, ShapeColor, new CCPoint(7, 1));
			return blocks;
		}

		//Creates offsets for each block used for rotation.
		//Since ShapeZ has 2 rotating positions, offset values are the opposites of each other for every rotation.
		private static CCPoint[][] setOffsets()
		{
			CCPoint[][] offsets = new CCPoint[2][];
			offsets[0] = new CCPoint[4] { new CCPoint(1, 1), new CCPoint(0, 0), new CCPoint(1, -1), new CCPoint(0, -2) };
			offsets[1] = new CCPoint[4];
			for (int i = 0; i < 4; i++)
				offsets[1][i] = new CCPoint(-offsets[0][i].X, -offsets[0][i].Y);
			return offsets;
		}
	}
}
