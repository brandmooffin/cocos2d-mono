using System;
using Cocos2D;

namespace tests
{
    public class LabelTTFAutoSizeTest : AtlasDemo
    {
        private CCTextAlignment m_eHorizAlign;
        private CCVerticalTextAlignment m_eVertAlign;
        private CCLabelTTF m_plabel;
        private CCLabelTTF m_label2;

        public LabelTTFAutoSizeTest()
        {
            var blockSize = new CCSize(400, 160);
            CCSize s = CCDirector.SharedDirector.WinSize;

            CCLayerColor colorLayer = new CCLayerColor(new CCColor4B(100, 100, 100, 255), blockSize.Width, blockSize.Height);
            colorLayer.AnchorPoint = new CCPoint(0, 0);
            colorLayer.Position = new CCPoint((s.Width - blockSize.Width) / 2, (s.Height - blockSize.Height) / 2);

            AddChild(colorLayer);

            m_plabel = null;
            updateAlignment();
        }

        private void updateAlignment()
        {
            var blockSize = new CCSize(400, 160);
            CCSize s = CCDirector.SharedDirector.WinSize;

            m_label2 = new CCLabelTTF("AutoSize Font", "SFFedoraTitles", 32, new CCSize(400, 64), CCTextAlignment.Left, CCVerticalTextAlignment.Center);
            m_label2.AnchorPoint = new CCPoint(0, 0);
            m_label2.Position = new CCPoint((s.Width - blockSize.Width) / 2, (s.Height - blockSize.Height +150) / 2);

            AddChild(m_label2);

            m_plabel = new CCLabelTTF("AutoSize Custom Font", "SFFedoraTitles", 32);
            m_plabel.AnchorPoint = new CCPoint(0, 0);
            //m_plabel.VerticalAlignment = CCVerticalTextAlignment.Center;
            m_plabel.Position = new CCPoint((s.Width - blockSize.Width) / 2, (s.Height - blockSize.Height) / 2);


            AddChild(m_plabel);
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