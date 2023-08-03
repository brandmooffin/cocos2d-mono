using Cocos2D;

namespace cocos2d.misc_nodes
{
    public class CCSwappableNode : CCNode
    {
        private CCPoint _currentTargetPos;
        public virtual CCPoint CurrentTargetPos
        {
            get
            {
                if (_currentTargetPos != default(CCPoint))
                {
                    return _currentTargetPos;
                }
                else
                {
                    return Position;
                }
            }
            set
            {
                _currentTargetPos = value;
            }
        }

        public CCSwappableNode()
        {
        }
    }
}
