using System;

namespace Cocos2D
{
    public class CCMenuItemSprite : CCMenuItem
    {
        protected float m_fOriginalScale;
        private CCNode m_pDisabledImage;
        private CCNode m_pNormalImage;

        private CCNode m_pSelectedImage;

        public CCNode NormalImage
        {
            get { return m_pNormalImage; }
            set
            {
                if (value != null)
                {
                    AddChild(value);
                    value.AnchorPoint = CCPoint.Zero;
                    ContentSize = value.ContentSize;
                }

                if (m_pNormalImage != null)
                {
                    value.Position = m_pNormalImage.Position;
                    RemoveChild(m_pNormalImage, true);
                }

                m_pNormalImage = value;
                UpdateImagesVisibility();
            }
        }

        public CCNode SelectedImage
        {
            get { return m_pSelectedImage; }
            set
            {
                if (value != null)
                {
                    AddChild(value);
                    value.AnchorPoint = CCPoint.Zero;
                }

                if (m_pSelectedImage != null)
                {
                    value.Position = m_pSelectedImage.Position;
                    RemoveChild(m_pSelectedImage, true);
                }

                m_pSelectedImage = value;
                UpdateImagesVisibility();
            }
        }

        public CCNode DisabledImage
        {
            get { return m_pDisabledImage; }
            set
            {
                if (value != null)
                {
                    AddChild(value);
                    value.AnchorPoint = CCPoint.Zero;
                }

                if (m_pDisabledImage != null)
                {
                    value.Position = m_pDisabledImage.Position;
                    RemoveChild(m_pDisabledImage, true);
                }

                m_pDisabledImage = value;
                UpdateImagesVisibility();
            }
        }

        public override bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                base.Enabled = value;
                UpdateImagesVisibility();
            }
        }

        public override CCColor3B Color
        {
            get { return _realColor; }
            set
            {
                _displayedColor = _realColor = value;
                if (NormalImage is CCSprite)
                {
                    (NormalImage as CCSprite).Color = value;
                }

                if (SelectedImage is CCSprite)
                {
                    (SelectedImage as CCSprite).Color = value;
                }

                if (DisabledImage is CCSprite)
                {
                    (DisabledImage as CCSprite).Color = value;
                }

                if (_cascadeColorEnabled)
                {
                    var parentColor = CCTypes.CCWhite;
                    var parent = m_pParent as ICCRGBAProtocol;
                    if (parent != null && parent.CascadeColorEnabled)
                    {
                        parentColor = parent.DisplayedColor;
                    }

                    UpdateDisplayedColor(parentColor);
                }
            }
        }

        public override byte Opacity
        {
            get { return _realOpacity; }
            set
            {
                _displayedOpacity = _realOpacity = value;
                if (NormalImage is CCSprite)
                {
                    (NormalImage as CCSprite).Opacity = value;
                }

                if (SelectedImage is CCSprite)
                {
                    (SelectedImage as CCSprite).Opacity = value;
                }

                if (DisabledImage is CCSprite)
                {
                    (DisabledImage as CCSprite).Opacity = value;
                }

                if (_cascadeOpacityEnabled)
                {
                    byte parentOpacity = 255;
                    var pParent = m_pParent as ICCRGBAProtocol;
                    if (pParent != null && pParent.CascadeOpacityEnabled)
                    {
                        parentOpacity = pParent.DisplayedOpacity;
                    }
                    UpdateDisplayedOpacity(parentOpacity);
                }
            }
        }

        public CCMenuItemSprite()
            : this(null, null, null, null)
        {
            ZoomBehaviorOnTouch = false;
        }

        public CCMenuItemSprite(Action<CCMenuItem> selector)
            : base(selector)
        {
        }

        public CCMenuItemSprite(string normalSprite, Action<CCMenuItem> selector)
            : this(new CCSprite(normalSprite), new CCSprite(normalSprite), new CCSprite(normalSprite), selector)
        {
        }

        public CCMenuItemSprite(string normalSprite, string selectedSprite, Action<CCMenuItem> selector)
            :this(new CCSprite(normalSprite), new CCSprite(selectedSprite), new CCSprite(normalSprite), selector)
        {
        }

        public CCMenuItemSprite(CCTexture2D normalSprite, CCTexture2D selectedSprite, Action<CCMenuItem> selector)
            : this(new CCSprite(normalSprite), new CCSprite(selectedSprite), new CCSprite(normalSprite), selector)
        {
        }

        public CCMenuItemSprite(CCNode normalSprite, CCNode selectedSprite)
            :this(normalSprite, selectedSprite, normalSprite, null)
        {
        }

		public CCMenuItemSprite(CCNode normalSprite, CCNode selectedSprite, Action<CCMenuItem> selector)
            :this(normalSprite, selectedSprite, normalSprite, selector)
        {
        }

        public CCMenuItemSprite(CCNode normalSprite, CCNode selectedSprite, CCNode disabledSprite, Action<CCMenuItem> selector)
        {
            InitWithTarget(selector);

            NormalImage = normalSprite;
            SelectedImage = selectedSprite;
            DisabledImage = disabledSprite;

            if (m_pNormalImage != null)
            {
                ContentSize = m_pNormalImage.ContentSize;
            }

            CascadeColorEnabled = true;
            CascadeOpacityEnabled = true;
        }

        /// <summary>
        /// Set this to true if you want to zoom-in/out on the button image like the CCMenuItemLabel works.
        /// </summary>
        public bool ZoomBehaviorOnTouch { get; set; }

        public override void Selected()
        {
            base.Selected();

            if (m_pNormalImage != null)
            {
                if (m_pDisabledImage != null)
                {
                    m_pDisabledImage.Visible = false;
                }

                if (m_pSelectedImage != null)
                {
                    m_pNormalImage.Visible = false;
                    m_pSelectedImage.Visible = true;
                }
                else
                {
                    m_pNormalImage.Visible = true;
                    if (ZoomBehaviorOnTouch)
                    {
                        CCAction action = GetAction(unchecked((int)kZoomActionTag));
                        if (action != null)
                        {
                            StopAction(action);
                        }
                        else
                        {
                            m_fOriginalScale = Scale;
                        }

                        CCAction zoomAction = new CCScaleTo(0.1f, m_fOriginalScale * 1.2f);
                        zoomAction.Tag = unchecked((int)kZoomActionTag);
                        RunAction(zoomAction);
                    }
                }
            }
        }

        public override void Unselected()
        {
            base.Unselected();
            if (m_pNormalImage != null)
            {
                m_pNormalImage.Visible = true;

                if (m_pSelectedImage != null)
                {
                    m_pSelectedImage.Visible = false;
                }
                if (ZoomBehaviorOnTouch)
                {
                    StopAction(unchecked((int)kZoomActionTag));
                    CCAction zoomAction = new CCScaleTo(0.1f, m_fOriginalScale);
                    zoomAction.Tag = unchecked((int)kZoomActionTag);
                    RunAction(zoomAction);
                }

                if (m_pDisabledImage != null)
                {
                    m_pDisabledImage.Visible = false;
                }
            }
        }

        public override void Activate()
        {
            if (m_bIsEnabled)
            {
                if (ZoomBehaviorOnTouch)
                {
                    StopAllActions();
                    Scale = m_fOriginalScale;
                }
                base.Activate();
            }
        }

        // Helper 
        private void UpdateImagesVisibility()
        {
            if (m_bIsEnabled)
            {
                if (m_pNormalImage != null) m_pNormalImage.Visible = true;
                if (m_pSelectedImage != null) m_pSelectedImage.Visible = false;
                if (m_pDisabledImage != null) m_pDisabledImage.Visible = false;
            }
            else
            {
                if (m_pDisabledImage != null)
                {
                    if (m_pNormalImage != null) m_pNormalImage.Visible = false;
                    if (m_pSelectedImage != null) m_pSelectedImage.Visible = false;
                    if (m_pDisabledImage != null) m_pDisabledImage.Visible = true;
                }
                else
                {
                    if (m_pNormalImage != null) m_pNormalImage.Visible = true;
                    if (m_pSelectedImage != null) m_pSelectedImage.Visible = false;
                    if (m_pDisabledImage != null) m_pDisabledImage.Visible = false;
                }
            }
        }
    }
}