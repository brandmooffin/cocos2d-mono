using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TetrisGame.DesktopGL;


namespace TetrisGame.Core
{
	/// <summary>
	/// The class that draws and updates the tetris Board.
	/// </summary>
	public class BoardSprite : DrawableGameComponent
	{
		IBoard board;
		Game1 game;
		SpriteBatch spriteBatch;
		Texture2D filledBlock;

		//To show the ghost shape if the "G" key was pressed.
		private bool drawGhost = false;
		private bool keyGhost = false;

		/// <summary>
		/// Initializes the BoardSprite object.
		/// </summary>
		/// <param name="game">Reference to the Game1 object.</param>
		/// <param name="board">Logical representation of the board.</param>
		public BoardSprite(Game1 game, IBoard board) : base(game)
		{
			this.game = game;
			this.board = board;
		}

		/// <summary>
		/// Allows to perform any initialization before the game is started.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
		}

		/// <summary>
		/// Loads all necessary content for the game.
		/// </summary>
		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			filledBlock = game.Content.Load<Texture2D>("FilledBlock");

			base.LoadContent();
		}

		/// <summary>
		/// Allows updating the Board.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			checkGhostKey(Keyboard.GetState());
			base.Update(gameTime);
		}

		/// <summary>
		/// Called when the Board is to be drawn.
		/// Responsible for drawing ghost shapes and the next shape.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Draw(GameTime gameTime)
		{
			spriteBatch.Begin();

			//creates a copy of current shape to draw it as a ghost
			Block[] shapeCopy = new Block[board.Shape.Length];
			fill(ref shapeCopy);

			for (int i = 0; i < board.GetLength(0); i++)
			{
				for (int j = 1; j < board.GetLength(1); j++)
				{
					if (isGhostPosition(shapeCopy, i, j) && drawGhost)
						spriteBatch.Draw(filledBlock, new Vector2(20 + i * 20, 35 + j * 20),
							shapeCopy[0].Colour * 0.3f);
					else
						spriteBatch.Draw(filledBlock, new Vector2(20 + i * 20, 35 + j * 20), board[i, j]);
				}
			}

			//draws the next shape
			IShape next = ((Board)board).NextShape;
			for (int i = 0; i < next.Length; i++)
			{
				spriteBatch.Draw(filledBlock, new Vector2(160 + next[i].Position.X * 20, 110 + next[i].Position.Y * 20),
				next[i].Colour);
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}

		//Checks if the user requested a ghost mode.
		private void checkGhostKey(KeyboardState keyboardState)
		{
			bool keyGhostNow = (keyboardState.IsKeyDown(Keys.G));

			if (!keyGhost && keyGhostNow)
				drawGhost = !drawGhost;

			keyGhost = keyGhostNow;
		}

		//Deetrmines if the ghost should be drawn at this position.
		private bool isGhostPosition(Block[] shapeGhost, int x, int y)
		{
			for (int i = 0; i < shapeGhost.Length; i++)
				if (shapeGhost[i].Position.X == x && shapeGhost[i].Position.Y == y)
					return true;
			return false;
		}

		//Creates a ghost shape, which represents Shape's final position
		//if the user decides to drop it.
		private void fill(ref Block[] shapeGhost)
		{
			//copy the shape
			for (int i = 0; i < board.Shape.Length; i++)
			{
				shapeGhost[i] = board.Shape[i];
			}
			//drop it to its final position
			while (tryMoveDown(shapeGhost))
			{
				for (int i = 0; i < shapeGhost.Length; i++)
					shapeGhost[i].MoveDown();
			}
			for (int i = 0; i < board.Shape.Length; i++)
			{
				if (shapeGhost[i].Position.Y < 2)
					drawGhost = false;
			}
		}

		//Tries to move down the blocks. Returns true if it is possible for every block.
		private bool tryMoveDown(Block[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].TryMoveDown())
					return false;
			}
			return true;
		}
	}
}
