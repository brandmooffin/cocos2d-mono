using System;
using System.Collections.Generic;
using System.Text;
using Cocos2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TetrisGame.Core
{
	/// <summary>
	/// Represents the Tetris Shape.
	/// </summary>
	public abstract class Shape : CCSprite
	{
		protected Block[] blocks;
		protected int currentRotation = 0;
		protected CCPoint[][] rotationOffset;
		private Board board;

		//Fires when the Shape is about to join the board pile.
		public event JoinPileHandler JoinPile;

		int counterMoveDown = 0;

		KeyboardState oldState;
		int counterInput = 0;
		int threshold;

		//To pause the game. The game is paused if the user selects "P" key or the space bar.
		private bool paused = false;
		private bool keyPaused = false;

		/// <summary>
		/// A base constructor for different Shapes.
		/// </summary>
		/// <param name="board">the Tetris board</param>
		/// <param name="blocks">the Blocks the Shape consists of</param>
		/// <param name="offset">the different offset combinations for Shape rotation</param>
		public Shape(Board board, Block[] blocks, CCPoint[][] offset)
		{
			if (board == null || blocks == null)
				throw new ArgumentNullException();

			for (int i = 0; i < blocks.Length; i++)
				if (blocks[i] == null)
					throw new ArgumentNullException("One of the blocks in the blocks array is null.");

			if (offset != null)
			{
				for (int i = 0; i < offset.Length; i++)
					if (offset[i].Length < blocks.Length)
						throw new ArgumentException("Offset array length: " + offset[i].Length +
													". Blocks array length: " + blocks.Length);
				for (int i = 0; i < offset.Length; i++)
				{
					for (int j = 0; j < offset.Length; j++)
						if (offset[i][j] == null)
							throw new ArgumentNullException("One of the offset values in the offset array is null.");
				}
			}

			this.board = board;
			this.blocks = blocks;
			rotationOffset = offset;

			oldState = Keyboard.GetState();
			threshold = 25;
		}

		/// <summary>
		/// The length of the Shape 
		/// (i.e. the number of blocks the Shape consists of).
		/// </summary>
		/// <returns>the length of the Shape</returns>
		public virtual int Length
		{ get { return blocks.Length; } }

		/// <summary>
		/// Returns one of the Blocks of the Shape depending on the index.
		/// </summary>
		/// <param name="i">The index of the required Block</param>
		/// <returns>the Block with the given index</returns>
		public Block this[int i]
		{
			get
			{
				checkIndex(i);
				return new Block(board, blocks[i].Color, blocks[i].Position);
			}
		}

		/// <summary>
		/// Drops the Shape to the bottom of the board (until there is a free space available).
		/// </summary>
		public virtual void Drop()
		{
			while (tryMoveDown())
				MoveDown();
		}

		/// <summary>
		/// Moves the current Shape down.
		/// </summary>
		public virtual void MoveDown()
		{
			if (tryMoveDown())
			{
				for (int i = 0; i < this.Length; i++)
					blocks[i].MoveDown();
			}
		}

		/// <summary>
		/// Moves the current Shape left.
		/// </summary>
		public virtual void MoveLeft()
		{
			bool moveAllow = true;

			//verifies that each Block of the Shape can move
			for (int i = 0; i < this.Length && moveAllow; i++)
			{
				if (!blocks[i].TryMoveLeft())
					moveAllow = false;
			}
			//moves the Shape
			if (moveAllow)
			{
				for (int i = 0; i < this.Length; i++)
					blocks[i].MoveLeft();
			}
		}

		/// <summary>
		/// Moves the current Shape right.
		/// </summary>
		public virtual void MoveRight()
		{
			bool moveAllow = true;

			//verifies that each Block of the Shape can move
			for (int i = 0; i < this.Length && moveAllow; i++)
			{
				if (!blocks[i].TryMoveRight())
					moveAllow = false;
			}
			//moves the Shape
			if (moveAllow)
			{
				for (int i = 0; i < this.Length; i++)
					blocks[i].MoveRight();
			}
		}

		/// <summary>
		/// Rotates the current Shape.
		/// </summary>
		public virtual void Rotate()
		{
			bool moveAllow = true;

			//verifies that each Block of the Shape can move
			for (int i = 0; i < this.Length && moveAllow; i++)
			{
				if (!blocks[i].TryRotate(rotationOffset[currentRotation][i]))
					moveAllow = false;
			}
			//moves the Shape
			if (moveAllow)
			{
				for (int i = 0; i < this.Length; i++)
					blocks[i].Rotate(rotationOffset[currentRotation][i]);

				currentRotation++;
				//reset the rotation if necessary
				if (currentRotation >= rotationOffset.Length)
					currentRotation = 0;
			}
		}

		/// <summary>
		/// Fires JoinPile event.
		/// </summary>
		protected virtual void OnJoinPile()
		{
			if (JoinPile != null)
				JoinPile();
		}

		//Checks whether the provided index corresponds to the Shape length.
		private void checkIndex(int i)
		{
			if (i < 0 || i >= this.Length)
				throw new IndexOutOfRangeException("Index: " + i + ". Size: " + this.Length);
		}

		//Verifies whether it is possible to move the Shape down;
		//if not, JoinPile event is fired since the Shape has reached the bottom.
		private bool tryMoveDown()
		{
			for (int i = 0; i < this.Length; i++)
			{
				if (!blocks[i].TryMoveDown())
				{
					OnJoinPile();
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Allows updating the Shape object.
		/// Pauses the game if the user requests it.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public void Update()
		{
			checkPauseKey(Keyboard.GetState());
			if (!paused)
			{
				double delay = (11 - board.Score.Level) * 0.05 * 60;
				if (counterMoveDown > delay)
				{
					MoveDown();
					counterMoveDown = 0;
				}
				else
				{
					counterMoveDown++;
					checkInput();
				}
			}
		}

		/// <summary>
		/// Called when the Shape is to be drawn.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public void Draw()
		{
			//for (int i = 0; i < shape.Length; i++)
			//{
			//	int x = shape[i].Position.X;
			//	int y = shape[i].Position.Y;
			//	if (y > 0)
			//		spriteBatch.Draw(filledBlock, new Vector2(20 + x * 20, 35 + y * 20), shape[i].Colour);
			//}
		}

		//Moves the Shape if the according key was pressed.
		//type - key type to compare with.
		//move - the movement the shape should perform depending on the key.
		private void checkCounter(Keys type, MoveFunction move)
		{
			// If not down last update, key has just been pressed.
			if (!oldState.IsKeyDown(type))
			{
				move();
				counterInput = 0; //reset counter with every new keystroke
			}
			else
			{
				counterInput++;
				if (counterInput > threshold)
					move();
			}
		}

		//Checks which key was pressed.
		private void checkInput()
		{
			KeyboardState newState = Keyboard.GetState();
			if (newState.IsKeyDown(Keys.Right))
				checkCounter(Keys.Right, MoveRight);
			else if (newState.IsKeyDown(Keys.Left))
				checkCounter(Keys.Left, MoveLeft);
			else if (newState.IsKeyDown(Keys.Down))
				checkCounter(Keys.Down, MoveDown);
			else if (newState.IsKeyDown(Keys.Up))
				checkCounter(Keys.Up, Rotate);
			//in order to drop the figure, the user should press enter
			else if (newState.IsKeyDown(Keys.Enter))
				checkCounter(Keys.Enter, Drop);

			// Once finished checking all keys, update old state.
			oldState = newState;
		}

		//Checks if the "p" key or a space bar was pressed.
		private void checkPauseKey(KeyboardState keyboardState)
		{
			bool keyPausedNow = (keyboardState.IsKeyDown(Keys.P) ||
				keyboardState.IsKeyDown(Keys.Space));

			if (!keyPaused && keyPausedNow)
				paused = !paused;

			keyPaused = keyPausedNow;
		}
	}

	//delegate representing one of the Shape movements.
	public delegate void MoveFunction();
}
