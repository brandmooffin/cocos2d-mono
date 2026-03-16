using System;
using Cocos2D;
using Microsoft.Xna.Framework.Graphics;

namespace tests
{
    public class BasePixelPerfectTest : CCLayer
    {
        public virtual string title() { return "Pixel Perfect Test"; }
        public virtual string subtitle() { return ""; }

        public override bool Init()
        {
            base.Init();
            CCSize s = CCDirector.SharedDirector.WinSize;

            var label = new CCLabelTTF(title(), "arial", 24);
            AddChild(label, 100);
            label.Position = new CCPoint(s.Width / 2, s.Height - 30);

            string sub = subtitle();
            if (sub.Length > 0)
            {
                label.Text += $" - {sub}";
            }

            var item1 = new CCMenuItemImage(TestResource.s_pPathB1, TestResource.s_pPathB2, backCallback);
            var item2 = new CCMenuItemImage(TestResource.s_pPathR1, TestResource.s_pPathR2, restartCallback);
            var item3 = new CCMenuItemImage(TestResource.s_pPathF1, TestResource.s_pPathF2, nextCallback);

            var menu = new CCMenu(item1, item2, item3);
            menu.Position = CCPoint.Zero;
            item1.Position = new CCPoint(s.Width / 2 - 100, 20);
            item2.Position = new CCPoint(s.Width / 2, 20);
            item3.Position = new CCPoint(s.Width / 2 + 100, 20);
            item1.Scale = 0.5f;
            item2.Scale = 0.5f;
            item3.Scale = 0.5f;
            AddChild(menu, 100);

            return true;
        }

        public void restartCallback(object pSender)
        {
            CCScene s = new PixelPerfectTestScene();
            s.AddChild(PixelPerfectTestScene.restartTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        public void nextCallback(object pSender)
        {
            CCScene s = new PixelPerfectTestScene();
            s.AddChild(PixelPerfectTestScene.nextTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }

        public void backCallback(object pSender)
        {
            CCScene s = new PixelPerfectTestScene();
            s.AddChild(PixelPerfectTestScene.backTestAction());
            CCDirector.SharedDirector.ReplaceScene(s);
        }
    }

    /// <summary>
    /// Tests CCTexture2D.DefaultAntialiased toggle.
    /// Creates labels with different settings side-by-side.
    /// </summary>
    public class DefaultAntialiasedTest : BasePixelPerfectTest
    {
        private CCLabelTTF _statusLabel;

        public override string title() { return "DefaultAntialiased"; }
        public override string subtitle() { return "Left=antialiased, Right=pixel-crisp. Touch to toggle global."; }

        public override bool Init()
        {
            base.Init();
            CCSize s = CCDirector.SharedDirector.WinSize;

            // Save original and set to true
            CCTexture2D.DefaultAntialiased = true;

            // Create antialiased label
            var aaLabel = new CCLabelTTF("Smooth Text", "arial", 32);
            aaLabel.Position = new CCPoint(s.Width * 0.25f, s.Height * 0.5f);
            AddChild(aaLabel);

            // Switch default to false
            CCTexture2D.DefaultAntialiased = false;

            // Create pixel label
            var pixelLabel = new CCLabelTTF("Crisp Text", "arial", 32);
            pixelLabel.Position = new CCPoint(s.Width * 0.75f, s.Height * 0.5f);
            AddChild(pixelLabel);

            // Restore default
            CCTexture2D.DefaultAntialiased = true;

            _statusLabel = new CCLabelTTF($"DefaultAntialiased: {CCTexture2D.DefaultAntialiased}", "arial", 16);
            _statusLabel.Position = new CCPoint(s.Width / 2, s.Height - 60);
            AddChild(_statusLabel, 100);

            var leftLabel = new CCLabelTTF("IsAntialiased = true", "arial", 14);
            leftLabel.Position = new CCPoint(s.Width * 0.25f, s.Height * 0.35f);
            AddChild(leftLabel);

            var rightLabel = new CCLabelTTF("IsAntialiased = false", "arial", 14);
            rightLabel.Position = new CCPoint(s.Width * 0.75f, s.Height * 0.35f);
            AddChild(rightLabel);

            TouchEnabled = true;
            TouchMode = CCTouchMode.OneByOne;
            return true;
        }

        public override bool TouchBegan(CCTouch touch)
        {
            CCTexture2D.DefaultAntialiased = !CCTexture2D.DefaultAntialiased;
            _statusLabel.Text = $"DefaultAntialiased: {CCTexture2D.DefaultAntialiased} (new textures only)";
            return true;
        }

        public override void OnExit()
        {
            base.OnExit();
            // Restore defaults
            CCTexture2D.DefaultAntialiased = true;
        }
    }

    /// <summary>
    /// Tests CCDrawManager.DefaultSamplerState toggle.
    /// Shows how switching between LinearClamp and PointClamp affects rendering.
    /// </summary>
    public class DefaultSamplerStateTest : BasePixelPerfectTest
    {
        private CCLabelTTF _statusLabel;

        public override string title() { return "DefaultSamplerState"; }
        public override string subtitle() { return "Touch to toggle PointClamp/LinearClamp"; }

        public override bool Init()
        {
            base.Init();
            CCSize s = CCDirector.SharedDirector.WinSize;

            // Create a scaled-up sprite to show sampling difference
            CCDrawNode drawing = new CCDrawNode();
            AddChild(drawing, 10);

            // Draw a pattern that shows aliasing clearly
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    var color = ((x + y) % 2 == 0)
                        ? new CCColor4F(0.2f, 0.6f, 1f, 1f)
                        : new CCColor4F(1f, 0.4f, 0.1f, 1f);
                    drawing.DrawRect(new CCRect(
                        s.Width * 0.3f + x * 20, s.Height * 0.3f + y * 20,
                        20, 20), new CCColor4B(
                            (byte)(color.R * 255), (byte)(color.G * 255),
                            (byte)(color.B * 255), 255));
                }
            }

            _statusLabel = new CCLabelTTF(
                $"SamplerState: {CCDrawManager.DefaultSamplerState}", "arial", 16);
            _statusLabel.Position = new CCPoint(s.Width / 2, s.Height - 60);
            AddChild(_statusLabel, 100);

            var hint = new CCLabelTTF("Pattern above — toggle sampler to see rendering difference", "arial", 14);
            hint.Position = new CCPoint(s.Width / 2, s.Height * 0.2f);
            AddChild(hint, 100);

            TouchEnabled = true;
            TouchMode = CCTouchMode.OneByOne;
            return true;
        }

        public override bool TouchBegan(CCTouch touch)
        {
            if (CCDrawManager.DefaultSamplerState == SamplerState.LinearClamp)
                CCDrawManager.DefaultSamplerState = SamplerState.PointClamp;
            else
                CCDrawManager.DefaultSamplerState = SamplerState.LinearClamp;

            _statusLabel.Text = $"SamplerState: {CCDrawManager.DefaultSamplerState}";
            return true;
        }

        public override void OnExit()
        {
            base.OnExit();
            CCDrawManager.DefaultSamplerState = SamplerState.LinearClamp;
        }
    }

    /// <summary>
    /// Tests CCDrawManager.ConvertScreenToGameCoords.
    /// Touch the screen and see the converted coordinates.
    /// </summary>
    public class ScreenToGameCoordsTest : BasePixelPerfectTest
    {
        private CCLabelTTF _statusLabel;
        private CCDrawNode _marker;

        public override string title() { return "ConvertScreenToGameCoords"; }
        public override string subtitle() { return "Touch anywhere. Shows screen vs game coords."; }

        public override bool Init()
        {
            base.Init();
            CCSize s = CCDirector.SharedDirector.WinSize;

            _marker = new CCDrawNode();
            AddChild(_marker, 10);

            // Draw crosshair grid for reference
            CCDrawNode grid = new CCDrawNode();
            AddChild(grid, 1);
            grid.DrawSegment(new CCPoint(s.Width / 2, 50), new CCPoint(s.Width / 2, s.Height - 50), 0.5f,
                new CCColor4F(0.3f, 0.3f, 0.3f, 1f));
            grid.DrawSegment(new CCPoint(50, s.Height / 2), new CCPoint(s.Width - 50, s.Height / 2), 0.5f,
                new CCColor4F(0.3f, 0.3f, 0.3f, 1f));

            _statusLabel = new CCLabelTTF("Touch to test coordinate conversion", "arial", 16);
            _statusLabel.Position = new CCPoint(s.Width / 2, s.Height - 60);
            AddChild(_statusLabel, 100);

            TouchEnabled = true;
            TouchMode = CCTouchMode.OneByOne;
            return true;
        }

        public override bool TouchBegan(CCTouch touch)
        {
            // Get the touch location in game coordinates via the existing method
            var gameCoords = touch.Location;

            // Also test our new ConvertScreenToGameCoords
            var screenPos = touch.LocationInView;
            var converted = CCDrawManager.ConvertScreenToGameCoords(screenPos.X, screenPos.Y);

            _marker.Clear();
            _marker.DrawFilledCircle(new CCPoint(gameCoords.X, gameCoords.Y), 8f, CCColor4F.Yellow);
            _marker.DrawFilledCircle(new CCPoint(converted.X, converted.Y), 5f, CCColor4F.Red);

            _statusLabel.Text = $"Screen: ({screenPos.X:F0},{screenPos.Y:F0}) | " +
                                $"Game(touch): ({gameCoords.X:F0},{gameCoords.Y:F0}) | " +
                                $"Game(convert): ({converted.X:F0},{converted.Y:F0})";
            return true;
        }
    }
}
