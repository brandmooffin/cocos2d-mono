using System;
using System.Collections.Generic;
using System.Text;
using Cocos2D;
using Microsoft.Xna.Framework;

namespace TetrisGame.Core
{
	/// <summary>
	/// Represents O Tetris shape.
	/// </summary>
	public class ShapeO : Shape
	{
		private static CCColor3B ShapeColor = new CCColor3B(Microsoft.Xna.Framework.Color.Gold);
		/// <summary>
		/// Instantiates ShapeO.
		/// </summary>
		/// <param name="board">The board where the ShapeO should be created.</param>
		public ShapeO(IBoard board) : base(board, setBlocks(board), null)
		{ }

		/// <summary>
		/// Does nothing since O has only 1 rotation configuration.
		/// </summary>
		public override void Rotate()
		{ }

		//Creates the blocks for ShapeO with yellow colour and pivot block starting at (6,0)
		private static Block[] setBlocks(IBoard board)
		{
			Block[] blocks = new Block[4];
			blocks[0] = new Block(board, ShapeColor, new CCPoint(5, 0));
			blocks[1] = new Block(board, ShapeColor, new CCPoint(6, 0));
			blocks[2] = new Block(board, ShapeColor, new CCPoint(5, 1));
			blocks[3] = new Block(board, ShapeColor, new CCPoint(6, 1));
			return blocks;
		}
	}
}
