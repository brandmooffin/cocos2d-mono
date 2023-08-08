using Cocos2D;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cocos2d.base_nodes
{
    public class CCTapNode<T> : CCNode, IDisposable
    {
        public delegate void TapHandler(T data, CCNode node);
        public event TapHandler OnTapped;
        public delegate void TouchBeginHandler(T data, CCNode node);
        public event TouchBeginHandler OnTouchBegin;
        private bool _active;
        protected bool _disposed { get; private set; }

        public CCTapNode(bool isSwallowTouches = true)
        {
            TouchMode = CCTouchMode.OneByOne;
            IsSwallowTouches = isSwallowTouches;

            Active = true;
        }

        public bool IsSwallowTouches { get; set; }

        public virtual T Data { get; set; }

        public bool Active
        {
            get
            {
                return TouchEnabled;
            }
            set
            {
                TouchEnabled = value;
            }
        }

        public override bool TouchBegan(CCTouch touch)
        {
            try
            {
                if (WorldBoundingBox.ContainsPoint(touch.Location) && this.Visible)
                {
                    TouchBegan();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Touch began failed with exception " + ex.Message);
            }
            return false;
        }

        public override void TouchEnded(CCTouch touch)
        {
            try
            {
                if (this.WorldBoundingBox.ContainsPoint(touch.Location) && this.Visible)
                {
                    Tapped();
                }
                else
                {
                    ReleasedOutside();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Touch ended failed with exception " + ex.Message);
                ReleasedOutside();
            }
        }

        public override void TouchMoved(CCTouch touch)
        {
            try
            {
                if (this.WorldBoundingBox.ContainsPoint(touch.Location) && this.Visible)
                {
                    DragInside(touch.Location);
                }
                else
                {
                    DragOutside(touch.Location);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Touch moved failed with exception " + ex.Message);
                DragOutside(touch.Location);
            }
        }

        /// <summary>
        /// Adjust content size to its children;
        /// </summary>
        protected void RefreshContentSize()
        {
            CCSize newContentSize = new CCSize();
            foreach (var child in Children)
            {
                newContentSize.Height = Math.Max(newContentSize.Height, child.ContentSize.Height);
                newContentSize.Width = Math.Max(newContentSize.Width, child.ContentSize.Width);
            }
            ContentSize = newContentSize;
        }

        protected virtual void Tapped()
        {
            OnTapped?.Invoke(Data, this);
        }

        protected virtual void ReleasedOutside()
        {

        }

        protected virtual void TouchBegan()
        {
            OnTouchBegin?.Invoke(Data, this);
        }

        protected virtual void DragInside(CCPoint position)
        {

        }

        protected virtual void DragOutside(CCPoint position)
        {

        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                Active = false;
            }
            base.Dispose(disposing);
        }
    }
}
