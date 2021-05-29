using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisGame.Core
{
	/// <summary>
	/// Delegate for JoinPile event.
	/// </summary>
	/// <param name="shape">The Shape to join the pile</param>
	public delegate void JoinPileHandler();

	/// <summary>
	/// Represents the Tetris Shape.
	/// </summary>
	public interface IShape
	{
		//Fires when the Shape is about to join the board pile.
		event JoinPileHandler JoinPile;

		/// <summary>
		/// The length of the Shape 
		/// (i.e. the number of blocks the Shape consists of).
		/// </summary>
		/// <returns>the length of the Shape</returns>
		int Length
		{ get; }

		/// <summary>
		/// Returns one of the Blocks of the Shape depending on the index.
		/// </summary>
		/// <param name="i">The index of the required Block</param>
		/// <returns>the Block with the given index</returns>
		Block this[int i]
		{ get; }

		/// <summary>
		/// Drops the Shape to the bottom of the board until the free space is available.
		/// </summary>
		void Drop();
		/// <summary>
		/// Moves the current Shape down.
		/// </summary>
		void MoveDown();
		/// <summary>
		/// Moves the current Shape left.
		/// </summary>
		void MoveLeft();
		/// <summary>
		/// Moves the current Shape right.
		/// </summary>
		void MoveRight();
		/// <summary>
		/// Rotates the current Shape.
		/// </summary>
		void Rotate();

	}
}
