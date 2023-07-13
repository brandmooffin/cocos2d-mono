using System;
using CoreVideo;

namespace Cocos2D
{
    //
    // TODO: Add CCGesture
    //

    public class CCTouch
    {
        private int m_nId;

        /// <summary>
        /// Point of action
        /// </summary>
        private CCPoint m_point;

        /// <summary>
        /// Previous point in the action
        /// </summary>
        private CCPoint m_prevPoint;

        private CCPoint m_startPoint;
        private bool m_startPointCaptured;

        internal CCNode Target { get; set; }
        internal TimeSpan TimeStamp { get; private set; }

        public CCTouch()
            : this(0, 0, 0)
        {
        }

        public CCTouch(int id, float x, float y)
        {
            m_nId = id;
            m_point = new CCPoint(x, y);
            m_prevPoint = new CCPoint(x, y);
        }

        internal CCTouch(int id, float x, float y, TimeSpan timeStamp)
        {
            m_nId = id;
            TimeStamp = timeStamp;
            m_point = new CCPoint(x, y);
            m_prevPoint = m_point;
            m_startPoint = m_point;
        }

        internal CCTouch(int id, CCPoint pos, TimeSpan timeStamp)
            : this(id, pos.X, pos.Y, timeStamp)
        {
        }

        /** returns the start touch location in OpenGL coordinates */
        public CCPoint StartLocation
        {
            get { return CCDirector.SharedDirector.ConvertToGl(m_startPoint); }
        }

        public CCPoint LocationInView
        {
            get { return m_point; }
        }

        /** returns the start touch location in screen coordinates */
        public CCPoint StartLocationInView
        {
            get { return m_startPoint; }
        }

        public CCPoint PreviousLocationInView
        {
            get { return m_prevPoint; }
        }

        /// <summary>
        /// Returns the location of the touch point in GL coordinates using ConvertToGl in CCDirector.
        /// </summary>
        public CCPoint Location
        {
            get { return CCDirector.SharedDirector.ConvertToGl(m_point); }
        }

        /// <summary>
        /// Returns the previous location of the touch point in GL coordinates using ConvertToGl in CCDirector.
        /// </summary>
        public CCPoint PreviousLocation
        {
            get { return CCDirector.SharedDirector.ConvertToGl(m_prevPoint); }
        }


        public int Id
        {
            get { return m_nId; }
        }

        /// <summary>
        /// Returns the difference, in GL coordinate space, of the last location and this current location.
        /// </summary>
        public CCPoint Delta
        {
            get { return Location - PreviousLocation; }
        }

        /// <summary>
        /// The touch delegate that consumed this touch. This is designed only for the one-at-a-time handler
        /// of touches.
        /// </summary>
        internal CCTargetedTouchHandler Consumer
        {
            get;
            set;
        }

        public void SetTouchInfo(int id, float x, float y)
        {
            m_nId = id;
            m_prevPoint = m_point;
            m_point.X = x;
            m_point.Y = y;
            if (!m_startPointCaptured)
            {
                m_startPoint = m_point;
                m_startPointCaptured = true;
            }
        }

        internal void UpdateTouchInfo(int id, float x, float y, TimeSpan timeStamp)
        {
            m_nId = id;
            m_prevPoint = m_point;
            m_point.X = x;
            m_point.Y = y;
            TimeStamp = timeStamp;
        }
    }
}