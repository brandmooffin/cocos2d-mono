using Cocos2D;
using Microsoft.Xna.Framework;
using System;

namespace tests
{
    public class LabelTTFAutoSizeTest : AtlasDemo
    {
        public LabelTTFAutoSizeTest()
        {
            var blockSize = new CCSize(400, 160);
            CCSize s = CCDirector.SharedDirector.WinSize;

            CCLayerColor colorLayer = new CCLayerColor(new CCColor4B(100, 100, 100, 255), blockSize.Width, blockSize.Height);
            colorLayer.AnchorPoint = new CCPoint(0, 0);
            colorLayer.Position = new CCPoint((s.Width - blockSize.Width) / 2, (s.Height - blockSize.Height) / 2);

            AddChild(colorLayer);

            updateAlignment();
        }

        private void updateAlignment()
        {
            var blockSize = new CCSize(400, 160);
            CCSize s = CCDirector.SharedDirector.WinSize;

            var titleLabel = new CCLabelTTF("AutoSize Font #1", "SFFedoraTitles", 32);
            titleLabel.AnchorPoint = new CCPoint(0, 0);
            titleLabel.Position = new CCPoint((s.Width - blockSize.Width) / 2, (s.Height - blockSize.Height + 150) / 2);
            AddChild(titleLabel);
            var customFontLabel = new CCLabelTTF("AutoSize Custom Font 2", "SFFedora", 32);
            customFontLabel.AnchorPoint = new CCPoint(0, 0);
            customFontLabel.Position = new CCPoint((s.Width - blockSize.Width) / 2, (s.Height - blockSize.Height) / 2);
            AddChild(customFontLabel);
            var abductionFontLabel = new CCLabelTTF("AutoSize Font", "Abduction", 26);
            abductionFontLabel.Position = new CCPoint((s.Width - blockSize.Width) / 2, (s.Height - blockSize.Height) / 4);
            AddChild(abductionFontLabel);
            var arialFontLabel = new CCLabelTTF("AutoSize Font 4", "arial", 64);
            arialFontLabel.Position = new CCPoint((s.Width - blockSize.Width) / 2, (s.Height - blockSize.Height) / 6);
            AddChild(arialFontLabel);
        }

        public override string title()
        {
            return "Testing CCLabelTTF AutoSize";
        }

        public override string subtitle()
        {
            return "You should see a label with auto size";
        }
    }
}