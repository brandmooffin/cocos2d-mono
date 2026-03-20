using System;
using Cocos2D;

namespace tests
{
    /// <summary>
    /// Tests that IsAntialiased persists across text changes on CCLabelTTF.
    /// This was a known bug: changing .Text would regenerate the backing texture,
    /// resetting IsAntialiased to the default (true).
    /// </summary>
    public class LabelTTFAntialiasedTest : AtlasDemo
    {
        private CCLabelTTF _antialiasedLabel;
        private CCLabelTTF _pixelLabel;
        private CCLabelTTF _statusLabel;
        private int _counter;

        public LabelTTFAntialiasedTest()
        {
            CCSize s = CCDirector.SharedDirector.WinSize;

            // Label WITH antialiasing (default)
            _antialiasedLabel = new CCLabelTTF("Antialiased: 0", "arial", 28);
            _antialiasedLabel.Position = new CCPoint(s.Width / 2, s.Height * 0.65f);
            AddChild(_antialiasedLabel);

            // Label WITHOUT antialiasing (pixel-perfect)
            _pixelLabel = new CCLabelTTF("Pixel: 0", "arial", 28);
            _pixelLabel.Position = new CCPoint(s.Width / 2, s.Height * 0.45f);
            _pixelLabel.IsAntialiased = false;
            AddChild(_pixelLabel);

            // Status label
            _statusLabel = new CCLabelTTF("", "arial", 16);
            _statusLabel.Position = new CCPoint(s.Width / 2, s.Height * 0.25f);
            AddChild(_statusLabel);

            _counter = 0;
            Schedule(UpdateLabels, 0.5f);
        }

        private void UpdateLabels(float dt)
        {
            _counter++;

            // Change text on both labels
            _antialiasedLabel.Text = $"Antialiased: {_counter}";
            _pixelLabel.Text = $"Pixel: {_counter}";

            // Verify IsAntialiased persists
            bool aaCorrect = _antialiasedLabel.IsAntialiased == true;
            bool pixelCorrect = _pixelLabel.IsAntialiased == false;

            _statusLabel.Text = $"AA={_antialiasedLabel.IsAntialiased} (expect true: {(aaCorrect ? "PASS" : "FAIL")}) | " +
                                $"Pixel={_pixelLabel.IsAntialiased} (expect false: {(pixelCorrect ? "PASS" : "FAIL")})";

            if (!aaCorrect || !pixelCorrect)
            {
                _statusLabel.Color = CCColor3B.Red;
            }
            else
            {
                _statusLabel.Color = CCColor3B.Green;
            }
        }

        public override string title()
        {
            return "LabelTTF IsAntialiased Fix";
        }

        public override string subtitle()
        {
            return "IsAntialiased should persist across text changes";
        }
    }
}
