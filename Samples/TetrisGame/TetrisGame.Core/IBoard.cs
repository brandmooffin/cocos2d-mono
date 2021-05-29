using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TetrisGame.Core
{
	/// <summary>
	/// Delegate for GameOver event.
	/// </summary>
	public delegate void GameOverHandle();

	/// <summary>
	/// Delegate for LinesCleared event.
	/// </summary>
	/// <param name="num">The number of lines cleared</param>
	public delegate void LinesClearedHandle(int num);

	/// <summary>
	/// Represents the Tetris board.
	/// </summary>
	public interface IBoard
	{
		/// <summary>
		/// Fires when the game is over.
		/// </summary>
		event GameOverHandle GameOver;
		/// <summary>
		/// Fires when some Board lines are cleared after the Shape joins the pile.
		/// </summary>
		event LinesClearedHandle LinesCleared;

		/// <summary>
		/// The colour of the specific place at the board.
		/// </summary>
		/// <param name="i">The coordinate in x-direction</param>
		/// <param name="j">The coordinate in y-direction</param>
		/// <returns>the colour of the specific place at the board</returns>
		Color this[int i, int j]
		{ get; }

		/// <summary>
		/// The current Shape.
		/// </summary>
		IShape Shape
		{ get; }

		/// <summary>
		/// Returns the number of elements in the specified dimension of board.
		/// </summary>
		/// <param name="rank">Dimension at which the length will be determined</param>
		/// <returns>the number of elements in the specified dimension of board</returns>
		int GetLength(int rank);
	}
}
