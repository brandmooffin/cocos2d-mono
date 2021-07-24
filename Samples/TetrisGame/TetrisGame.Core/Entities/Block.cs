using Cocos2D;

namespace TetrisGame.Core
{
    /// <summary>
    /// Represents a block that shapes consist of.
    /// </summary>
    public class Block : CCSprite
	{
		public const float WIDTH = 14.2f;
		public const float HEIGHT = 14.2f;

		/// <summary>
		/// Create block with a default color.
		/// </summary>
		public Block(CCColor3B color)
		{
			InitWithFile("FilledBlock", new CCRect(0, 0, WIDTH, HEIGHT));
			ContentSize = new CCSize(WIDTH, HEIGHT);
			Color = color;
		}
	}
}
