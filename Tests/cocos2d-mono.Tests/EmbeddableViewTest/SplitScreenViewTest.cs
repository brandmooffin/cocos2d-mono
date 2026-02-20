using System;
using Cocos2D;
using Microsoft.Xna.Framework;

namespace tests
{
    /// <summary>
    /// Demonstrates the multi-view / split-screen concept.
    /// Shows two side-by-side areas each running independent content,
    /// similar to how CCGameView supports split-screen and secondary views.
    /// </summary>
    public class SplitScreenViewTest : EmbeddableViewTest
    {
        CCDrawNode _drawNode;
        CCSprite _leftSprite;
        CCSprite _rightSprite;
        CCLabelTTF _leftFpsLabel;
        CCLabelTTF _rightFpsLabel;

        public override string title()
        {
            return "CCGameView - Multi-View / Split Screen";
        }

        public override string subtitle()
        {
            return "Two independent views with separate content";
        }

        public override void OnEnter()
        {
            base.OnEnter();

            CCSize s = CCDirector.SharedDirector.WinSize;

            _drawNode = new CCDrawNode();
            AddChild(_drawNode, 0);

            float viewTop = s.Height - 55;
            float viewBottom = 50f;
            float viewHeight = viewTop - viewBottom;
            float gapWidth = 4f;
            float leftViewWidth = (s.Width - gapWidth) / 2f;
            float rightViewX = leftViewWidth + gapWidth;
            float rightViewWidth = s.Width - rightViewX;

            // Draw left view background (dark blue fill, blue border)
            // CCColor4F uses 0.0-1.0 float range
            _drawNode.DrawRect(
                new CCRect(0, viewBottom, leftViewWidth, viewHeight),
                new CCColor4F(0.10f, 0.14f, 0.24f, 1f),
                1f,
                new CCColor4F(0.31f, 0.63f, 1f, 1f)
            );

            // Draw right view background (dark purple fill, orange border)
            _drawNode.DrawRect(
                new CCRect(rightViewX, viewBottom, rightViewWidth, viewHeight),
                new CCColor4F(0.14f, 0.10f, 0.20f, 1f),
                1f,
                new CCColor4F(1f, 0.47f, 0.31f, 1f)
            );

            // Draw center divider
            _drawNode.DrawRect(
                new CCRect(leftViewWidth, viewBottom, gapWidth, viewHeight),
                new CCColor4B(80, 80, 80, 255)
            );

            // Left view label
            CCLabelTTF leftLabel = new CCLabelTTF("Primary View", "arial", 16);
            leftLabel.Position = new CCPoint(leftViewWidth / 2f, viewTop - 15);
            leftLabel.Color = new CCColor3B(80, 160, 255);
            AddChild(leftLabel, 2);

            // Right view label
            CCLabelTTF rightLabel = new CCLabelTTF("Secondary View", "arial", 16);
            rightLabel.Position = new CCPoint(rightViewX + rightViewWidth / 2f, viewTop - 15);
            rightLabel.Color = new CCColor3B(255, 120, 80);
            AddChild(rightLabel, 2);

            // Left view content - sprite with movement actions
            float leftCenterX = leftViewWidth / 2f;
            float leftCenterY = viewBottom + viewHeight / 2f;

            _leftSprite = new CCSprite("Images/grossini");
            _leftSprite.Position = new CCPoint(leftCenterX, leftCenterY);
            AddChild(_leftSprite, 1);

            // Circular movement for left sprite
            var moveRight = new CCMoveBy(1f, new CCPoint(80, 0));
            var moveUp = new CCMoveBy(1f, new CCPoint(0, 60));
            var moveLeft = new CCMoveBy(1f, new CCPoint(-80, 0));
            var moveDown = new CCMoveBy(1f, new CCPoint(0, -60));
            var circleSeq = new CCSequence(moveRight, moveUp, moveLeft, moveDown);
            _leftSprite.RunAction(new CCRepeatForever(circleSeq));

            // Additional left view sprites
            CCSprite leftBg1 = new CCSprite("Images/grossinis_sister1");
            leftBg1.Position = new CCPoint(leftCenterX - 100, leftCenterY - 60);
            leftBg1.Scale = 0.7f;
            leftBg1.RunAction(new CCRepeatForever(new CCRotateBy(4f, 360)));
            AddChild(leftBg1, 1);

            CCSprite leftBg2 = new CCSprite("Images/grossinis_sister2");
            leftBg2.Position = new CCPoint(leftCenterX + 100, leftCenterY - 60);
            leftBg2.Scale = 0.7f;
            leftBg2.RunAction(new CCRepeatForever(new CCRotateBy(4f, -360)));
            AddChild(leftBg2, 1);

            // Right view content - different scene with particle-like effect
            float rightCenterX = rightViewX + rightViewWidth / 2f;
            float rightCenterY = viewBottom + viewHeight / 2f;

            _rightSprite = new CCSprite("Images/grossini_dance_01");
            _rightSprite.Position = new CCPoint(rightCenterX, rightCenterY);
            AddChild(_rightSprite, 1);

            // Bounce animation for right sprite
            var jumpAction = new CCJumpBy(2f, new CCPoint(0, 0), 80, 3);
            _rightSprite.RunAction(new CCRepeatForever(jumpAction));

            // Scale animation for right sprite
            var pulseScale = new CCScaleTo(0.5f, 1.2f);
            var pulseBack = new CCScaleTo(0.5f, 1.0f);
            _rightSprite.RunAction(new CCRepeatForever(new CCSequence(pulseScale, pulseBack)));

            // Additional right view sprites with dance frames
            for (int i = 0; i < 3; i++)
            {
                string frameName = string.Format("Images/grossini_dance_{0:D2}", (i * 4) + 2);
                CCSprite dancer = new CCSprite(frameName);
                dancer.Position = new CCPoint(
                    rightCenterX + (i - 1) * 90,
                    rightCenterY - 80
                );
                dancer.Scale = 0.6f;

                var fadeOut = new CCFadeOut(1.5f + i * 0.3f);
                var fadeIn = new CCFadeIn(1.5f + i * 0.3f);
                dancer.RunAction(new CCRepeatForever(new CCSequence(fadeOut, fadeIn)));

                AddChild(dancer, 1);
            }

            // Info labels for each view
            _leftFpsLabel = new CCLabelTTF("Scene: Sprites + Rotation", "arial", 12);
            _leftFpsLabel.Position = new CCPoint(leftCenterX, viewBottom + 15);
            _leftFpsLabel.Color = new CCColor3B(150, 150, 150);
            AddChild(_leftFpsLabel, 2);

            _rightFpsLabel = new CCLabelTTF("Scene: Dance + Effects", "arial", 12);
            _rightFpsLabel.Position = new CCPoint(rightCenterX, viewBottom + 15);
            _rightFpsLabel.Color = new CCColor3B(150, 150, 150);
            AddChild(_rightFpsLabel, 2);

            // Description text below
            CCLabelTTF descLabel = new CCLabelTTF(
                "CCGameView supports multiple views via AttachSecondaryView()\n" +
                "and built-in split-screen with SetSplitScreenScene().",
                "arial", 14);
            descLabel.Position = new CCPoint(s.Width / 2f, viewBottom - 15);
            descLabel.Color = new CCColor3B(200, 200, 200);
            AddChild(descLabel, 2);

            // API reference labels
            float apiY = viewBottom - 38;
            CCLabelTTF apiLabel1 = new CCLabelTTF(
                "Primary: RunWithScene(scene)  |  Secondary: AttachSecondaryView(view)",
                "arial", 12);
            apiLabel1.Position = new CCPoint(s.Width / 2f, apiY);
            apiLabel1.Color = new CCColor3B(140, 140, 140);
            AddChild(apiLabel1, 2);
        }
    }
}
