using Cocos2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace tests
{
    public class LabelTTFAutoSizeTest : AtlasDemo
    {
        private CCTextAlignment m_eHorizAlign;
        private CCVerticalTextAlignment m_eVertAlign;

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

            var m_label2 = new CCLabelTTF("AutoSize Font #1", "SFFedoraTitles", 32);
            m_label2.AnchorPoint = new CCPoint(0, 0);
            m_label2.Position = new CCPoint((s.Width - blockSize.Width) / 2, (s.Height - blockSize.Height +150) / 2);

            AddChild(m_label2);

            var m_plabel = new CCLabelTTF("AutoSize Custom Font 2", "SFFedora", 32);
            m_plabel.AnchorPoint = new CCPoint(0, 0);
            m_plabel.Position = new CCPoint((s.Width - blockSize.Width) / 2, (s.Height - blockSize.Height) / 2);


            AddChild(m_plabel);

            var m_label3 = new CCLabelTTF("AutoSize Font", "Abduction", 26);
            m_label3.Position = new CCPoint((s.Width - blockSize.Width) / 2, (s.Height - blockSize.Height) / 4);
            AddChild(m_label3);

            var m_label4 = new CCLabelTTF("AutoSize Font 4", "arial", 64);
            m_label4.Position = new CCPoint((s.Width - blockSize.Width) / 2, (s.Height - blockSize.Height) / 6);
            AddChild(m_label4);
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