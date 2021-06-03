using System;
using System.Collections.Generic;
using System.Text;
using Cocos2D;
using Microsoft.Xna.Framework;

namespace TetrisGame.Core
{
	/// <summary>
	/// Represents J Tetris shape.
	/// </summary>
	public class ShapeJ : Shape
	{
		private static CCColor3B ShapeColor = new CCColor3B(Microsoft.Xna.Framework.Color.MidnightBlue);
		/// <summary>
		/// Instantiates ShapeJ.
		/// </summary>
		/// <param name="board">The board where the ShapeJ should be created.</param>
		public ShapeJ(IBoard board) : base(board, setBlocks(board), setOffsets())
		{ }

		//Creates the blocks for ShapeJ with blue colour and pivot block starting at (6,0)
		private static Block[] setBlocks(IBoard board)
		{
			Block[] blocks = new Block[4];
			blocks[0] = new Block(board, ShapeColor, new CCPoint(7, 1));
			blocks[1] = new Block(board, ShapeColor, new CCPoint(7, 0));
			blocks[2] = new Block(board, ShapeColor, new CCPoint(6, 0));
			blocks[3] = new Block(board, ShapeColor, new CCPoint(5, 0));
			return blocks;
		}

		//Creates offsets for each block used for rotation.
		//ShapeJ has four possible rotations.
		private static CCPoint[][] setOffsets()
		{
			CCPoint[][] offsets = new CCPoint[4][];
			offsets[0] = new CCPoint[4] { new CCPoint(0, -2), new CCPoint(-1, -1), new CCPoint(0, 0), new CCPoint(1, 1) };
			offsets[1] = new CCPoint[4] { new CCPoint(-2, 0), new CCPoint(-1, 1), new CCPoint(0, 0), new CCPoint(1, -1) };
			offsets[2] = new CCPoint[4] { new CCPoint(0, 2), new CCPoint(1, 1), new CCPoint(0, 0), new CCPoint(-1, -1) };
			offsets[3] = new CCPoint[4] { new CCPoint(2, 0), new CCPoint(1, -1), new CCPoint(0, 0), new CCPoint(-1, 1) };
			return offsets;
		}
	}
}
