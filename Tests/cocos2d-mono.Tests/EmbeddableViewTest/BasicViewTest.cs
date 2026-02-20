using System;
using Cocos2D;
using Microsoft.Xna.Framework;

namespace tests
{
    /// <summary>
    /// Demonstrates the basic CCGameView embeddable view concept.
    /// The view area continuously resizes to show how the design resolution
    /// maps into different view sizes under various resolution policies.
    /// Tap/click to cycle through resolution policies.
    /// </summary>
    public class BasicViewTest : EmbeddableViewTest
    {
        CCDrawNode _viewBorder;
        CCLabelTTF _policyLabel;
        CCLabelTTF _viewSizeLabel;
        CCSprite _grossini;
        CCSprite _sister1;
        CCSprite _sister2;

        // Design resolution is fixed; view size animates
        const float DesignW = 480f;
        const float DesignH = 320f;

        // View area bounds (the view animates within these)
        float _viewCenterX;
        float _viewCenterY;
        float _viewMinWidth;
        float _viewMaxWidth;
        float _viewMinHeight;
        float _viewMaxHeight;
        float _viewAreaTop;

        // Current animated view size
        float _currentViewW;
        float _currentViewH;
        float _elapsed;

        int _currentPolicy = 0;
        string[] _policyNames = {
            "ShowAll",
            "ExactFit",
            "NoBorder",
            "FixedWidth",
            "FixedHeight"
        };
        string[] _policyDescs = {
            "Entire content visible, may letterbox",
            "Stretch to fill, may distort",
            "Fill view, content may be cropped",
            "Match view width, scale height",
            "Match view height, scale width"
        };

        public override string title()
        {
            return "CCGameView - Dynamic View Resize";
        }

        public override string subtitle()
        {
            return "Tap/click to cycle resolution policies. View size animates.";
        }

        public override void OnEnter()
        {
            base.OnEnter();

            TouchEnabled = true;

            CCSize s = CCDirector.SharedDirector.WinSize;

            _viewCenterX = s.Width / 2f;
            _viewCenterY = s.Height / 2f - 20f;
            _viewMinWidth = 250f;
            _viewMaxWidth = s.Width * 0.75f;
            _viewMinHeight = 180f;
            _viewMaxHeight = s.Height * 0.55f;
            _viewAreaTop = _viewCenterY + _viewMaxHeight / 2f;

            _currentViewW = _viewMaxWidth;
            _currentViewH = _viewMaxHeight;

            _viewBorder = new CCDrawNode();
            AddChild(_viewBorder, 0);

            // Sprites placed at the center of the view - they represent the "game content"
            _grossini = new CCSprite("Images/grossini");
            _grossini.Position = new CCPoint(_viewCenterX - 100, _viewCenterY);
            AddChild(_grossini, 1);

            _sister1 = new CCSprite("Images/grossinis_sister1");
            _sister1.Position = new CCPoint(_viewCenterX, _viewCenterY);
            AddChild(_sister1, 1);

            _sister2 = new CCSprite("Images/grossinis_sister2");
            _sister2.Position = new CCPoint(_viewCenterX + 100, _viewCenterY);
            AddChild(_sister2, 1);

            // Simple animations
            var moveUp = new CCMoveBy(1.5f, new CCPoint(0, 30));
            _grossini.RunAction(new CCRepeatForever(new CCSequence(moveUp, moveUp.Reverse())));

            _sister1.RunAction(new CCRepeatForever(new CCRotateBy(3f, 360)));

            var scale = new CCScaleBy(1f, 1.2f);
            _sister2.RunAction(new CCRepeatForever(new CCSequence(scale, scale.Reverse())));

            // Policy label
            _policyLabel = new CCLabelTTF(GetPolicyText(), "arial", 18);
            _policyLabel.Position = new CCPoint(_viewCenterX, _viewAreaTop + 40);
            _policyLabel.Color = new CCColor3B(255, 255, 100);
            AddChild(_policyLabel, 2);

            // View size label (updates every frame)
            _viewSizeLabel = new CCLabelTTF("", "arial", 14);
            _viewSizeLabel.Position = new CCPoint(_viewCenterX, _viewAreaTop + 20);
            _viewSizeLabel.Color = new CCColor3B(180, 180, 180);
            AddChild(_viewSizeLabel, 2);

            // Design resolution label
            CCLabelTTF designLabel = new CCLabelTTF(
                string.Format("DesignResolution: {0} x {1} (fixed)", (int)DesignW, (int)DesignH),
                "arial", 14);
            designLabel.Position = new CCPoint(_viewCenterX, _viewCenterY - _viewMaxHeight / 2f - 20);
            designLabel.Color = new CCColor3B(200, 200, 200);
            AddChild(designLabel, 2);

            // Description
            CCLabelTTF descLabel = new CCLabelTTF(
                "Blue border = ViewSize (animating)  |  Yellow border = Design resolution mapped by policy",
                "arial", 12);
            descLabel.Position = new CCPoint(_viewCenterX, _viewCenterY - _viewMaxHeight / 2f - 38);
            descLabel.Color = new CCColor3B(150, 150, 150);
            AddChild(descLabel, 2);

            Schedule(UpdateView);
        }

        private string GetPolicyText()
        {
            return string.Format("Policy: {0} - {1}", _policyNames[_currentPolicy], _policyDescs[_currentPolicy]);
        }

        private void UpdateView(float dt)
        {
            _elapsed += dt;

            // Animate view width and height with different frequencies so the aspect ratio changes over time
            float tW = (float)(Math.Sin(_elapsed * 0.8) * 0.5 + 0.5); // 0..1
            float tH = (float)(Math.Sin(_elapsed * 0.5 + 1.0) * 0.5 + 0.5); // 0..1, offset phase
            _currentViewW = _viewMinWidth + (_viewMaxWidth - _viewMinWidth) * tW;
            _currentViewH = _viewMinHeight + (_viewMaxHeight - _viewMinHeight) * tH;

            _viewSizeLabel.Text = string.Format("ViewSize: {0} x {1}", (int)_currentViewW, (int)_currentViewH);

            RedrawView();
        }

        private void RedrawView()
        {
            _viewBorder.Clear();
            _viewBorder.Opacity = 255;

            float vx = _viewCenterX - _currentViewW / 2f;
            float vy = _viewCenterY - _currentViewH / 2f;

            // Draw the view area (dark fill with blue border)
            // CCColor4F uses 0.0-1.0 float range
            _viewBorder.DrawRect(
                new CCRect(vx, vy, _currentViewW, _currentViewH),
                new CCColor4F(0.08f, 0.10f, 0.18f, 1f),
                2f,
                new CCColor4F(0.31f, 0.63f, 1f, 1f)
            );

            // Compute the design resolution region based on the current policy
            float contentW, contentH;
            float viewAspect = _currentViewW / _currentViewH;
            float designAspect = DesignW / DesignH;

            switch (_currentPolicy)
            {
                case 0: // ShowAll - fit inside view, letterbox
                    if (viewAspect > designAspect)
                    {
                        contentH = _currentViewH;
                        contentW = contentH * designAspect;
                    }
                    else
                    {
                        contentW = _currentViewW;
                        contentH = contentW / designAspect;
                    }
                    break;

                case 1: // ExactFit - stretch to fill entire view
                    contentW = _currentViewW;
                    contentH = _currentViewH;
                    break;

                case 2: // NoBorder - fill view, overflow clipped
                    if (viewAspect > designAspect)
                    {
                        contentW = _currentViewW;
                        contentH = contentW / designAspect;
                    }
                    else
                    {
                        contentH = _currentViewH;
                        contentW = contentH * designAspect;
                    }
                    break;

                case 3: // FixedWidth - match width, height scales
                    contentW = _currentViewW;
                    contentH = contentW / designAspect;
                    break;

                case 4: // FixedHeight - match height, width scales
                    contentH = _currentViewH;
                    contentW = contentH * designAspect;
                    break;

                default:
                    contentW = _currentViewW;
                    contentH = _currentViewH;
                    break;
            }

            float cx = _viewCenterX - contentW / 2f;
            float cy = _viewCenterY - contentH / 2f;

            // Draw the content region (yellow border with very slight fill so border renders)
            // DrawRect only draws borders when fillColor.A > 0
            _viewBorder.DrawRect(
                new CCRect(cx, cy, contentW, contentH),
                new CCColor4F(0f, 0f, 0f, 0.01f),
                2f,
                new CCColor4F(1f, 1f, 0.4f, 0.9f)
            );
        }

        public override void TouchesEnded(System.Collections.Generic.List<CCTouch> touches)
        {
            _currentPolicy = (_currentPolicy + 1) % _policyNames.Length;
            _policyLabel.Text = GetPolicyText();
        }
    }
}
