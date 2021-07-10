using System;
using System.Collections.Generic;
using System.Text;
using Cocos2D;
using Microsoft.Xna.Framework;

namespace TetrisGame.Core
{
	/// <summary>
	/// Represents a block that shapes consist of.
	/// </summary>
	public class Block : CCSprite
	{
		public const float WIDTH = 14.2f;
		public const float HEIGHT = 14.2f;

		public Block(CCColor3B color)
		{
			InitWithFile("FilledBlock", new CCRect(0, 0, WIDTH, HEIGHT));
			ContentSize = new CCSize(WIDTH, HEIGHT);
			Color = color;
		}
	}
}
