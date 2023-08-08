using System;

namespace Cocos2D
{
    public class CCScale9SpriteFrame : CCScale9Sprite
    {
        [Obsolete("This will be removed in a future release. Please use constructor from CCScale9Sprite instead.")]
        public CCScale9SpriteFrame(CCSpriteFrame spriteFrame, CCRect capInsets)
        {
            InitWithSpriteFrame(spriteFrame, capInsets);
        }

        [Obsolete("This will be removed in a future release. Please use constructor from CCScale9Sprite instead.")]
        public CCScale9SpriteFrame(CCSpriteFrame spriteFrame)
        {
            InitWithSpriteFrame(spriteFrame);
        }

        [Obsolete("This will be removed in a future release. Please use constructor from CCScale9Sprite instead.")]
        public CCScale9SpriteFrame(string spriteFrameName, CCRect capInsets)
        {
            InitWithSpriteFrameName(spriteFrameName, capInsets);
        }

        [Obsolete("This will be removed in a future release. Please use constructor from CCScale9Sprite instead.")]
        public CCScale9SpriteFrame(string alias)
        {
            InitWithSpriteFrameName(alias);
        }
    }
}
