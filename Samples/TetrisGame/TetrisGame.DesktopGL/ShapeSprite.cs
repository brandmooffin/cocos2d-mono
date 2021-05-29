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
	/// The class that draws and updates the Shape movements on the board.
	/// </summary>
	public class ShapeSprite : DrawableGameComponent
	{
		IShape shape;

		Score score;
		int counterMoveDown = 0;

		KeyboardState oldState;
		int counterInput = 0;
		int threshold;

		Game1 game;
		SpriteBatch spriteBatch;

		Texture2D filledBlock;

		//To pause the game. The game is paused if the user selects "P" key or the space bar.
		private bool paused = false;
		private bool keyPaused = false;

		/// <summary>
		/// Initializes the ShapeSprite object.
		/// </summary>
		/// <param name="game">Reference to the Game1 object.</param>
		/// <param name="board">Logical representation of the board.</param>
		/// <param name="score">Logical representation of the score.</param>
		public ShapeSprite(Game1 game, IBoard board, Score score) : base(game)
		{
			this.game = game;
			this.score = score;
			this.shape = board.Shape;
		}

		/// <summary>
		/// Allows to perform any initialization before the game is started.
		/// </summary>
		public override void Initialize()
		{
			oldState = Keyboard.GetState();
			threshold = 25;

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
		/// Allows updating the Shape object.
		/// Pauses the game if the user requests it.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			checkPauseKey(Keyboard.GetState());
			if (!paused)
			{
				double delay = (11 - score.Level) * 0.05 * 60;
				if (counterMoveDown > delay)
				{
					shape.MoveDown();
					counterMoveDown = 0;
				}
				else
				{
					counterMoveDown++;
					checkInput();
				}
			}
			base.Update(gameTime);
		}

		/// <summary>
		/// Called when the Shape is to be drawn.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Draw(GameTime gameTime)
		{
			spriteBatch.Begin();

			for (int i = 0; i < shape.Length; i++)
			{
				int x = shape[i].Position.X;
				int y = shape[i].Position.Y;
				if (y > 0)
					spriteBatch.Draw(filledBlock, new Vector2(20 + x * 20, 35 + y * 20), shape[i].Colour);
			}

			spriteBatch.End();

			base.Draw(gameTime);
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
				checkCounter(Keys.Right, shape.MoveRight);
			else if (newState.IsKeyDown(Keys.Left))
				checkCounter(Keys.Left, shape.MoveLeft);
			else if (newState.IsKeyDown(Keys.Down))
				checkCounter(Keys.Down, shape.MoveDown);
			else if (newState.IsKeyDown(Keys.Up))
				checkCounter(Keys.Up, shape.Rotate);
			//in order to drop the figure, the user should press enter
			else if (newState.IsKeyDown(Keys.Enter))
				checkCounter(Keys.Enter, shape.Drop);

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
