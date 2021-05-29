using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisGame.Core
{
	/// <summary>
	/// Represents the current Shape on the board.
	/// </summary>
	public class ShapeProxy : IShapeFactory, IShape
	{
		private static Random random = new Random();
		private IBoard board;
		private IShape current;

		//Fires when the Shape is about to join the board pile.
		public event JoinPileHandler JoinPile;

		/// <summary>
		/// Instantiates ShapeProxy object and creates the new Shape.
		/// </summary>
		/// <param name="board">The Tetris board</param>
		public ShapeProxy(IBoard board)
		{
			if (board == null)
				throw new ArgumentNullException();
			this.board = board;
			DeployNewShape();
		}

		/// <summary>
		/// The length of the current Shape 
		/// (i.e. the number of blocks the Shape consists of).
		/// </summary>
		/// <returns>the length of the current Shape</returns>
		public int Length
		{
			get { return current.Length; }
		}

		/// <summary>
		/// Returns one of the Blocks of the current Shape depending on the index.
		/// </summary>
		/// <param name="i">The index of the required Block</param>
		/// <returns>the Block with the given index</returns>
		public Block this[int i]
		{
			get { return current[i]; }
		}

		/// <summary>
		/// Randomly creates the new Shape and adds a listener for the JoinPile event.
		/// </summary>
		public void DeployNewShape()
		{
			int shape = random.Next(0, 7);
			switch (shape)
			{
				case 0:
					current = new ShapeO(board);
					break;
				case 1:
					current = new ShapeI(board);
					break;
				case 2:
					current = new ShapeS(board);
					break;
				case 3:
					current = new ShapeT(board);
					break;
				case 4:
					current = new ShapeZ(board);
					break;
				case 5:
					current = new ShapeL(board);
					break;
				default:
					current = new ShapeJ(board);
					break;
			}
			current.JoinPile += joinPileHandler;
		}

		/// <summary>
		/// Creates a new Shape according to the given nextShape and adds a listener for the JoinPile event.
		/// </summary>
		/// <param name="nextShape">The shape after which the new shape should be modeled.</param>
		public void DeployNewShape(ShapeProxy nextShape)
		{
			IShape shape = nextShape.current;
			if (shape is ShapeL)
				current = new ShapeL(board);
			else if (shape is ShapeI)
				current = new ShapeI(board);
			else if (shape is ShapeS)
				current = new ShapeS(board);
			else if (shape is ShapeZ)
				current = new ShapeZ(board);
			else if (shape is ShapeT)
				current = new ShapeT(board);
			else if (shape is ShapeO)
				current = new ShapeO(board);
			else
				current = new ShapeJ(board);

			current.JoinPile += joinPileHandler;
		}

		/// <summary>
		/// Drops the current Shape to the bottom of the board.
		/// </summary>
		public void Drop()
		{
			current.Drop();
		}

		/// <summary>
		/// Moves the current Shape down.
		/// </summary>
		public void MoveDown()
		{
			current.MoveDown();
		}

		/// <summary>
		/// Moves the current Shape left.
		/// </summary>
		public void MoveLeft()
		{
			current.MoveLeft();
		}

		/// <summary>
		/// Moves the current Shape right.
		/// </summary>
		public void MoveRight()
		{
			current.MoveRight();
		}

		/// <summary>
		/// Rotates the current Shape.
		/// </summary>
		public void Rotate()
		{
			current.Rotate();
		}

		/// <summary>
		/// Fires JoinPile event.
		/// </summary>
		protected virtual void OnJoinPile()
		{
			if (JoinPile != null)
				JoinPile();
		}

		//Handles the JoinPile event from current
		private void joinPileHandler()
		{
			OnJoinPile();
		}
	}
}
