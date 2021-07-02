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
		float WIDTH = 14.2f;
		float HEIGHT = 14.2f;

		public Block(CCColor3B color)
		{
			InitWithFile("FilledBlock", new CCRect(0, 0, WIDTH, HEIGHT));
			Color = color;
		}
	}
}
