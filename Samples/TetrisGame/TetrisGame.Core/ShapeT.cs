using System;
using System.Collections.Generic;
using System.Text;
using Cocos2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TetrisGame.Core
{
	/// <summary>
	/// Represents T Tetris shape.
	/// </summary>
	public class ShapeT : Shape
	{
		private static CCColor3B ShapeColor = new CCColor3B(Microsoft.Xna.Framework.Color.Purple);
		/// <summary>
		/// Instantiates ShapeT.
		/// </summary>
		/// <param name="board">The board where the ShapeT should be created.</param>
		public ShapeT(Board board) : base(board, setBlocks(board), setOffsets())
		{ }

		//Creates the blocks for ShapeT with purple colour and pivot block starting at (6,0)
		private static Block[] setBlocks(Board board)
		{
			Block[] blocks = new Block[4];
			blocks[0] = new Block(board, ShapeColor, new CCPoint(5, 0));
			blocks[1] = new Block(board, ShapeColor, new CCPoint(6, 0));
			blocks[2] = new Block(board, ShapeColor, new CCPoint(7, 0));
			blocks[3] = new Block(board, ShapeColor, new CCPoint(6, 1));
			return blocks;
		}

		//Creates offsets for each block used for rotation.
		//ShapeT has four possible rotations.
		private static CCPoint[][] setOffsets()
		{
			CCPoint[][] offsets = new CCPoint[4][];
			offsets[0] = new CCPoint[4] { new CCPoint(1, 1), new CCPoint(0, 0), new CCPoint(-1, -1), new CCPoint(1, -1) };
			offsets[1] = new CCPoint[4] { new CCPoint(1, -1), new CCPoint(0, 0), new CCPoint(-1, 1), new CCPoint(-1, -1) };
			offsets[2] = new CCPoint[4] { new CCPoint(-1, -1), new CCPoint(0, 0), new CCPoint(1, 1), new CCPoint(-1, 1) };
			offsets[3] = new CCPoint[4] { new CCPoint(-1, 1), new CCPoint(0, 0), new CCPoint(1, -1), new CCPoint(1, 1) };
			return offsets;
		}
	}
}
