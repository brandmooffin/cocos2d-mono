using Cocos2D;
using Microsoft.Xna.Framework;

namespace TetrisGame.Core
{
	/// <summary>
	/// Represents I Tetris shape.
	/// </summary>
	public class ShapeI : Shape
	{
		/// <summary>
		/// Instantiates ShapeI.
		/// </summary>
		/// <param name="board">The board where the ShapeI should be created.</param>
		public ShapeI(Board board) : base(board, setBlocks(board), setOffsets())
		{ }

		//Creates the blocks for ShapeI with cyan colour and pivot block starting at (6,0)
		private static Block[] setBlocks(Board board)
		{
			Block[] blocks = new Block[4];
			for (int i = 0; i < 4; i++)
				blocks[i] = new Block(board, new CCColor3B(Microsoft.Xna.Framework.Color.DodgerBlue), new CCPoint(4 + i, 0));
			return blocks;
		}

		//Creates offsets for each block used for rotation.
		//Since ShapeI has 2 rotating positions, offset values are the opposites of each other for every rotation.
		private static CCPoint[][] setOffsets()
		{
			CCPoint[][] offsets = new CCPoint[2][];
			offsets[0] = new CCPoint[4] { new CCPoint(2, 2), new CCPoint(1, 1), new CCPoint(0, 0), new CCPoint(-1, -1) };
			offsets[1] = new CCPoint[4];
			for (int i = 0; i < 4; i++)
				offsets[1][i] = new CCPoint(-offsets[0][i].X, -offsets[0][i].Y);
			return offsets;
		}
	}
}
